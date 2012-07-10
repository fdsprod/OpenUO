#region License Header
/***************************************************************************
 *   Copyright (c) 2011 OpenUO Software Team.
 *   All Right Reserved.
 *
 *   $Id: $:
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 ***************************************************************************/
 #endregion

using SharpDX;
using SharpDX.Direct3D9;

namespace Client.Graphics
{
    public class ElementCamera : ICamera
    {
        internal static int _cameraMatrixBaseIndex = 2;

        private BoundingFrustum _frustum = new BoundingFrustum(Matrix.Identity);
        private readonly Plane[] _frustumPlanes = new Plane[6];
        private Matrix _cameraMatrix = Matrix.Identity;
        private Matrix _viewMatrix = Matrix.Identity;
        private bool _normalised = true;
        private bool _dirty = true;
        private int _rtWidth, _rtHeight;
        private float _rtWidthf, _rtHeightf;
        private Vector2 _bottomLeft, _topRight;
        private bool _frustumDirty = true;
        private bool _viewMatrixDirty = true;
        private int _cameraMatrixIndex = 1;
        private bool _reverseBFC;
        
        public bool ReverseBackfaceCulling
        {
            get { return _reverseBFC; }
            set { _reverseBFC = value; }
        }

        public Matrix CameraMatrix
        {
            get
            {
                if (_dirty)
                    BuildView();

                return _cameraMatrix;
            }
        }

        public bool UseNormalisedCoordinates
        {
            get { return _normalised; }
            set { if (_normalised != value) { SetDirty(); _normalised = value; } }
        }

        public ElementCamera() { }

        public ElementCamera(bool useNormalisedCoordinates)
        {
            this._normalised = useNormalisedCoordinates;
        }

        protected virtual void GetView(out Vector2 bottomLeft, out Vector2 topRight)
        {

            if (_normalised)
            {
                topRight = new Vector2(1, 0);
                bottomLeft = new Vector2(0, 1);
            }
            else
            {
                topRight = new Vector2(_rtWidthf, 0);
                bottomLeft = new Vector2(0, _rtHeightf);
            }
        }

        protected void SetDirty()
        {
            _dirty = true;
            _cameraMatrixIndex = System.Threading.Interlocked.Increment(ref _cameraMatrixBaseIndex);
        }
        public Plane[] GetCullingPlanes()
        {
            if (_dirty)
                BuildView();

            if (_frustumDirty)
            {
                UpdateFrustum();
            }

            return _frustumPlanes;
        }

        private void UpdateFrustum()
        {
            if (_viewMatrixDirty)
            {
                Matrix.Invert(ref _cameraMatrix, out _viewMatrix);
                _viewMatrixDirty = false;
            }

            _frustum.SetMatrix(ref _viewMatrix);

            _frustumPlanes[0] = _frustum.Near;
            _frustumPlanes[1] = _frustum.Far;
            _frustumPlanes[2] = _frustum.Left;
            _frustumPlanes[3] = _frustum.Right;
            _frustumPlanes[4] = _frustum.Bottom;
            _frustumPlanes[5] = _frustum.Top;

            _frustumDirty = false;
        }

        public void GetCameraMatrix(out Matrix matrix)
        {
            if (_dirty)
                BuildView();

            matrix = _cameraMatrix;
        }

        public void GetCameraPosition(out Vector3 viewPoint)
        {
            if (_dirty)
                BuildView();

            viewPoint.X = _cameraMatrix.M41;
            viewPoint.Y = _cameraMatrix.M42;
            viewPoint.Z = _cameraMatrix.M43;
        }

        public void GetCameraViewDirection(out Vector3 viewDirection)
        {
            if (_dirty)
                BuildView();

            viewDirection.X = -_cameraMatrix.M31;
            viewDirection.Y = -_cameraMatrix.M32;
            viewDirection.Z = -_cameraMatrix.M33;
        }

        public bool ProjectToTarget(ref Vector3 position, out Vector2 coordinate, Viewport viewport)
        {
            Vector2 size = new Vector2(viewport.Width, viewport.Height);

            bool result = Camera3D.ProjectToCoordinate(this, ref position, out coordinate, ref size);

            coordinate.X = size.X * (coordinate.X * 0.5f + 0.5f);
            coordinate.Y = size.Y * (coordinate.Y * 0.5f + 0.5f);

            return result;
        }

        public void ProjectFromTarget(ref Vector2 coordinate, float projectDepth, out Vector3 position, Viewport viewport)
        {
            Vector2 size = new Vector2(viewport.Width, viewport.Height);
            Camera3D.ProjectFromCoordinate(this, true, ref coordinate, projectDepth, out position, ref size);
        }

        public bool ProjectToCoordinate(ref Vector3 position, out Vector2 coordinate)
        {
            Vector2 size = Vector2.One;
            return Camera3D.ProjectToCoordinate(this, ref position, out coordinate, ref size);
        }

        public void ProjectFromCoordinate(ref Vector2 coordinate, float projectDepth, out Vector3 position)
        {
            Vector2 size = Vector2.One;
            Camera3D.ProjectFromCoordinate(this, true, ref coordinate, projectDepth, out position, ref size);
        }

        internal void Begin(DrawState state)
        {
            int w = state.Context.Viewport.Width;
            int h = state.Context.Viewport.Height;

            _dirty |= w != _rtWidth;
            _dirty |= h != _rtHeight;

            _rtWidth = w;
            _rtHeight = h;
            _rtHeightf = (float)h;
            _rtWidthf = (float)w;
        }

        private void BuildView()
        {
            Vector2 bl, tr;
            GetView(out bl, out tr);

            if (_bottomLeft != bl || _topRight != tr)
            {
                Matrix mat = Matrix.Identity;
                Matrix.Scaling(0.5f, 0.5f, 1, out mat);
                mat.M41 = 0.5f;
                mat.M42 = 0.5f;

                if (tr.X != 1 || tr.Y != 1 ||
                    bl.X != 0 || bl.Y != 0)
                {
                    Matrix mat2;
                    Matrix.Scaling((tr.X - bl.X), (tr.Y - bl.Y), 1, out mat2);
                    mat2.M41 = bl.X;
                    mat2.M42 = bl.Y;

                    Matrix.Multiply(ref mat, ref mat2, out _cameraMatrix);
                }
                else
                {
                    _cameraMatrix = mat;
                }

                _dirty = false;
                _viewMatrixDirty = true;
                _frustumDirty = true;

                _topRight = tr;
                _bottomLeft = bl;
            }
        }

        void ICamera.GetCameraHorizontalVerticalFov(out Vector2 v)
        {
            v = new Vector2(1, 1);
        }

        bool ICamera.GetCameraHorizontalVerticalFov(ref Vector2 v, ref int changeIndex)
        {
            if (changeIndex != 1)
            {
                v.X = 1;
                v.Y = 1;
                changeIndex = 1;
                return true;
            }
            return false;
        }

        void ICamera.GetCameraHorizontalVerticalFovTangent(out Vector2 v)
        {
            v = new Vector2(1, 1);
        }

        bool ICamera.GetCameraHorizontalVerticalFovTangent(ref Vector2 v, ref int changeIndex)
        {
            if (changeIndex != 1)
            {
                v.X = 1;
                v.Y = 1;
                changeIndex = 1;
                return true;
            }
            return false;
        }

        void ICamera.GetCameraNearFarClip(out Vector2 v)
        {
            v = new Vector2(0, 1);
        }

        bool ICamera.GetCameraNearFarClip(ref Vector2 v, ref int changeIndex)
        {
            if (changeIndex != 1)
            {
                v.X = 0;
                v.Y = 1;
                changeIndex = 1;
                return true;
            }
            return false;
        }

        bool ICamera.GetProjectionMatrix(ref Matrix matrix, ref Vector2 drawTargetSize, ref int changeIndex)
        {
            if (changeIndex != 1)
            {
                changeIndex = 1;
                matrix = Matrix.Identity;
                return true;
            }
            return false;
        }

        bool ICamera.GetViewMatrix(ref Matrix matrix, ref int changeIndex)
        {
            if (changeIndex != _cameraMatrixIndex)
            {
                changeIndex = _cameraMatrixIndex;
                ((ICamera)this).GetViewMatrix(out matrix);
                return true;
            }
            return false;
        }

        bool ICamera.GetCameraMatrix(ref Matrix matrix, ref int changeIndex)
        {
            if (changeIndex != _cameraMatrixIndex)
            {
                changeIndex = _cameraMatrixIndex;
                ((ICamera)this).GetCameraMatrix(out matrix);
                return true;
            }
            return false;
        }

        void ICamera.GetProjectionMatrix(out Matrix matrix, ref Vector2 drawTargetSize)
        {
            matrix = Matrix.Identity;
        }

        void ICamera.GetProjectionMatrix(out Matrix matrix, Vector2 drawTargetSize)
        {
            matrix = Matrix.Identity;
        }

        void ICamera.GetViewMatrix(out Matrix matrix)
        {
            if (_dirty)
                BuildView();

            if (_viewMatrixDirty)
            {
                Matrix.Invert(ref _cameraMatrix, out _viewMatrix);
                _viewMatrixDirty = false;
            }

            matrix = _viewMatrix;
        }

        bool ICamera.GetCameraPosition(ref Vector3 viewPoint, ref int changeIndex)
        {
            if (_dirty)
                BuildView();
            if (changeIndex != _cameraMatrixIndex)
            {
                viewPoint.X = _cameraMatrix.M41;
                viewPoint.Y = _cameraMatrix.M42;
                viewPoint.Z = _cameraMatrix.M43;
                changeIndex = _cameraMatrixIndex;
                return true;
            }
            return false;
        }

        bool ICamera.GetCameraViewDirection(ref Vector3 viewDirection, ref int changeIndex)
        {
            if (_dirty)
                BuildView();
            if (changeIndex != _cameraMatrixIndex)
            {
                viewDirection.X = -_cameraMatrix.M31;
                viewDirection.Y = -_cameraMatrix.M32;
                viewDirection.Z = -_cameraMatrix.M33;
                changeIndex = _cameraMatrixIndex;
                return true;
            }
            return false;
        }
        
        bool ICullPrimitive.TestWorldBox(Vector3 min, Vector3 max, ref Matrix world)
        {
            return FrustumCull.BoxInFrustum(GetCullingPlanes(), ref min, ref max, ref world);
        }
        bool ICullPrimitive.TestWorldBox(ref Vector3 min, ref Vector3 max, ref Matrix world)
        {
            return FrustumCull.BoxInFrustum(GetCullingPlanes(), ref min, ref max, ref world);
        }

        bool ICullPrimitive.TestWorldSphere(float radius, Vector3 position)
        {
            return FrustumCull.SphereInFrustum(GetCullingPlanes(), radius, ref position);
        }
        bool ICullPrimitive.TestWorldSphere(float radius, ref Vector3 position)
        {
            return FrustumCull.SphereInFrustum(GetCullingPlanes(), radius, ref position);
        }

        ContainmentType ICullPrimitive.IntersectWorldBox(Vector3 min, Vector3 max, ref Matrix world)
        {
            return FrustumCull.BoxIntersectsFrustum(GetCullingPlanes(), ref min, ref max, ref world);
        }
        ContainmentType ICullPrimitive.IntersectWorldBox(ref Vector3 min, ref Vector3 max, ref Matrix world)
        {
            return FrustumCull.BoxIntersectsFrustum(GetCullingPlanes(), ref min, ref max, ref world);
        }

        ContainmentType ICullPrimitive.IntersectWorldSphere(float radius, Vector3 position)
        {
            return FrustumCull.SphereIntersectsFrustum(GetCullingPlanes(), radius, ref position);
        }

        ContainmentType ICullPrimitive.IntersectWorldSphere(float radius, ref Vector3 position)
        {
            return FrustumCull.SphereIntersectsFrustum(GetCullingPlanes(), radius, ref position);
        }

        bool ICullPrimitive.TestWorldBox(Vector3 min, Vector3 max)
        {
            return FrustumCull.AABBInFrustum(GetCullingPlanes(), ref min, ref max);
        }
        bool ICullPrimitive.TestWorldBox(ref Vector3 min, ref Vector3 max)
        {
            return FrustumCull.AABBInFrustum(GetCullingPlanes(), ref min, ref max);
        }
        ContainmentType ICullPrimitive.IntersectWorldBox(Vector3 min, Vector3 max)
        {
            return FrustumCull.AABBIntersectsFrustum(GetCullingPlanes(), ref min, ref max);
        }
        ContainmentType ICullPrimitive.IntersectWorldBox(ref Vector3 min, ref Vector3 max)
        {
            return FrustumCull.AABBIntersectsFrustum(GetCullingPlanes(), ref min, ref max);
        }
    }
}
