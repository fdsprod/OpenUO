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

using System;
using SharpDX;
using SharpDX.Direct3D9;

namespace Client.Graphics
{
    public class Camera3D : ICamera
    {
        private Projection _proj;
        private Matrix _camMatrix = Matrix.Identity;
        private Matrix _viewMatrix = Matrix.Identity;
        private bool _camMatChanged = true;
        private bool _viewMatDirty = true;
        private int _camMatIndex = 1;
        private bool _reverseBFC;

        public bool ReverseBackfaceCulling
        {
            get { return _reverseBFC; }
            set { _reverseBFC = value; }
        }

        public Projection Projection
        {
            get { return _proj; }
        }

        public Matrix CameraMatrix
        {
            get { return _camMatrix; }
            set { SetCameraMatrix(ref value); }
        }

        public Vector3 Position
        {
            get { return new Vector3(_camMatrix.M41, _camMatrix.M42, _camMatrix.M43); }
            set
            {
                if (_camMatrix.M41 != value.X || _camMatrix.M42 != value.Y || _camMatrix.M43 != value.Z)
                {
                    _camMatrix.M41 = value.X;
                    _camMatrix.M42 = value.Y;
                    _camMatrix.M43 = value.Z;

                    _camMatChanged = true; _viewMatDirty = true;

                    _camMatIndex = System.Threading.Interlocked.Increment(ref Camera2D._cameraMatrixBaseIndex);
                }
            }
        }

        public Camera3D(Projection projection, Matrix cameraMatrix)
        {
            _proj = projection;
            _camMatrix = cameraMatrix;
        }

        public Camera3D(Projection projection)
        {
            _proj = projection;
            _camMatrix = Matrix.Identity;
        }

        public Camera3D()
        {
            _proj = new Projection();
            _camMatrix = Matrix.Identity;
        }

        public virtual void LookAt(ref Vector3 lookAtTarget, ref Vector3 cameraPosition, ref Vector3 upVector)
        {
            Vector3 dir = cameraPosition - lookAtTarget;
            if (dir.LengthSquared() == 0)
                throw new ArgumentException("target and position are the same");
            dir.Normalize();
            Vector3 xaxis;

            Vector3.Cross(ref upVector, ref dir, out xaxis);
            xaxis.Normalize();

            Vector3.Cross(ref dir, ref xaxis, out upVector);

            _camMatrix.M11 = xaxis.X;
            _camMatrix.M12 = xaxis.Y;
            _camMatrix.M13 = xaxis.Z;
            _camMatrix.M14 = 0;

            _camMatrix.M21 = upVector.X;
            _camMatrix.M22 = upVector.Y;
            _camMatrix.M23 = upVector.Z;
            _camMatrix.M24 = 0;

            _camMatrix.M31 = dir.X;
            _camMatrix.M32 = dir.Y;
            _camMatrix.M33 = dir.Z;
            _camMatrix.M34 = 0;

            _camMatrix.M41 = cameraPosition.X;
            _camMatrix.M42 = cameraPosition.Y;
            _camMatrix.M43 = cameraPosition.Z;
            _camMatrix.M44 = 1;

            _camMatChanged = true;
            _viewMatDirty = true;
            _camMatIndex = System.Threading.Interlocked.Increment(ref Camera2D._cameraMatrixBaseIndex);
        }

        public void LookAt(Vector3 lookAtTarget, Vector3 cameraPosition, Vector3 upVector)
        {
            LookAt(ref lookAtTarget, ref cameraPosition, ref upVector);
        }

        public void SetCameraMatrix(ref Matrix value)
        {
            if (_camMatrix != value)
            {
                _camMatrix = value;
                _camMatChanged = true;
                _viewMatDirty = true;
                _camMatIndex = System.Threading.Interlocked.Increment(ref Camera2D._cameraMatrixBaseIndex);
            }
        }

        public void GetCameraMatrix(out Matrix matrix)
        {
            matrix = _camMatrix;
        }

        public void GetCameraPosition(out Vector3 viewPoint)
        {
            viewPoint = new Vector3(_camMatrix.M41, _camMatrix.M42, _camMatrix.M43);
        }

        public void GetCameraViewDirection(out Vector3 viewDirection)
        {
            viewDirection = new Vector3(-_camMatrix.M31, -_camMatrix.M32, -_camMatrix.M33);
        }

        public bool ProjectToTarget(ref Vector3 position, out Vector2 coordinate, Viewport viewport)
        {
            Vector2 size = new Vector2(viewport.Width, viewport.Height);

            bool result = ProjectToCoordinate(this, ref position, out coordinate, ref size);

            coordinate.X = size.X * (coordinate.X * 0.5f + 0.5f);
            coordinate.Y = size.Y * (coordinate.Y * 0.5f + 0.5f);

            return result;
        }

        public void ProjectFromTarget(ref Vector2 coordinate, float projectDepth, out Vector3 position, Viewport viewport)
        {
            Vector2 size = new Vector2(viewport.Width, viewport.Height);
            ProjectFromCoordinate(this, false, ref coordinate, projectDepth, out position, ref size);
        }
        
        public bool ProjectToCoordinate(ref Vector3 position, out Vector2 coordinate)
        {
            Vector2 size = Vector2.One;
            return ProjectToCoordinate(this, ref position, out coordinate, ref size);
        }
        
        public void ProjectFromCoordinate(ref Vector2 coordinate, float projectDepth, out Vector3 position)
        {
            Vector2 size = Vector2.One;
            ProjectFromCoordinate(this, false, ref coordinate, projectDepth, out position, ref size);
        }

        internal static bool ProjectToCoordinate(ICamera camera, ref Vector3 position, out Vector2 coordinate, ref Vector2 targetSize)
        {
            Vector4 worldPositionW = new Vector4(position, 1.0f);

            Matrix mat;
            camera.GetViewMatrix(out mat);
            Vector4.Transform(ref worldPositionW, ref mat, out worldPositionW);

            camera.GetProjectionMatrix(out mat, ref targetSize);
            Vector4.Transform(ref worldPositionW, ref mat, out worldPositionW);

            if (worldPositionW.W != 0)
                worldPositionW.W = 1.0f / worldPositionW.W;

            coordinate = new Vector2(worldPositionW.X * worldPositionW.W, worldPositionW.Y * worldPositionW.W);

            return worldPositionW.Z > 0;
        }
        
        internal static void ProjectFromCoordinate(ICamera camera, bool is2D, ref Vector2 screenPosition, float projectDepth, out Vector3 position, ref Vector2 targetSize)
        {
            Vector4 coordinate = new Vector4(0, 0, 0.5f, 1);
            if (targetSize.X != 0)
                coordinate.X = ((screenPosition.X / targetSize.X) - 0.5f) * 2;
            if (targetSize.Y != 0)
                coordinate.Y = ((screenPosition.Y / targetSize.Y) - 0.5f) * 2;

            Matrix mat;

            if (is2D)
            {
                camera.GetCameraMatrix(out mat);
            }
            else
            {
                Matrix pm;
                camera.GetProjectionMatrix(out pm, ref targetSize);
                camera.GetViewMatrix(out mat);

                Matrix.Multiply(ref mat, ref pm, out mat);
                Matrix.Invert(ref mat, out mat);
            }

            Vector4.Transform(ref coordinate, ref mat, out coordinate);

            if (coordinate.W != 0)
            {
                coordinate.W = 1.0f / coordinate.W;
                coordinate.X *= coordinate.W;
                coordinate.Y *= coordinate.W;
                coordinate.Z *= coordinate.W;
                coordinate.W = 1;
            }

            Vector3 cameraPos;
            camera.GetCameraPosition(out cameraPos);

            Vector3 difference = new Vector3();
            difference.X = coordinate.X - cameraPos.X;
            difference.Y = coordinate.Y - cameraPos.Y;
            difference.Z = coordinate.Z - cameraPos.Z;

            if (difference.X != 0 || difference.Y != 0 || difference.Y != 0)
                difference.Normalize();

            difference.X *= projectDepth;
            difference.Y *= projectDepth;
            difference.Z *= projectDepth;

            position = new Vector3();
            position.X = difference.X + cameraPos.X;
            position.Y = difference.Y + cameraPos.Y;
            position.Z = difference.Z + cameraPos.Z;
        }

        void ICamera.GetCameraHorizontalVerticalFov(out Vector2 v)
        {
            v = new Vector2(_proj.GetHorizontalFov(), _proj.GetVerticalFov());
        }

        bool ICamera.GetCameraHorizontalVerticalFov(ref Vector2 v, ref int changeIndex)
        {
            return _proj.GetCameraHorizontalVerticalFov(ref v, ref changeIndex);
        }

        void ICamera.GetCameraHorizontalVerticalFovTangent(out Vector2 v)
        {
            v = new Vector2(_proj.GetHorizontalFovTangent(), _proj.GetVerticalFovTangent());
        }

        bool ICamera.GetCameraHorizontalVerticalFovTangent(ref Vector2 v, ref int changeIndex)
        {
            return _proj.GetCameraHorizontalVerticalFovTangent(ref v, ref changeIndex);
        }

        void ICamera.GetCameraNearFarClip(out Vector2 v)
        {
            v = new Vector2(_proj.NearClip, _proj.FarClip);
        }

        bool ICamera.GetCameraNearFarClip(ref Vector2 v, ref int changeIndex)
        {
            return _proj.GetCameraNearFarClip(ref v, ref changeIndex);
        }

        bool ICamera.ReverseBackfaceCulling { get { return _reverseBFC ^ _proj.UseLeftHandedProjection; } }

        bool ICamera.GetProjectionMatrix(ref Matrix matrix, ref Vector2 drawTargetSize, ref int changeIndex)
        {
            return _proj.GetProjectionMatrix(ref matrix, ref drawTargetSize, ref changeIndex);
        }

        bool ICamera.GetViewMatrix(ref Matrix matrix, ref int changeIndex)
        {
            if (changeIndex != _camMatIndex)
            {
                changeIndex = _camMatIndex;
                ((ICamera)this).GetViewMatrix(out matrix);
                return true;
            }
            return false;
        }

        bool ICamera.GetCameraMatrix(ref Matrix matrix, ref int changeIndex)
        {
            if (changeIndex != _camMatIndex)
            {
                changeIndex = _camMatIndex;
                ((ICamera)this).GetCameraMatrix(out matrix);
                return true;
            }
            return false;
        }

        void ICamera.GetProjectionMatrix(out Matrix matrix, ref Vector2 drawTargetSize)
        {
            _proj.GetProjectionMatrix(out matrix, ref drawTargetSize);
        }

        void ICamera.GetProjectionMatrix(out Matrix matrix, Vector2 drawTargetSize)
        {
            _proj.GetProjectionMatrix(out matrix, ref drawTargetSize);
        }

        bool ICamera.GetCameraPosition(ref Vector3 viewPoint, ref int changeIndex)
        {
            if (changeIndex != _camMatIndex)
            {
                viewPoint.X = _camMatrix.M41;
                viewPoint.Y = _camMatrix.M42;
                viewPoint.Z = _camMatrix.M43;
                changeIndex = _camMatIndex;
                return true;
            }
            return false;
        }

        bool ICamera.GetCameraViewDirection(ref Vector3 viewDirection, ref int changeIndex)
        {
            if (changeIndex != _camMatIndex)
            {
                viewDirection.X = -_camMatrix.M31;
                viewDirection.Y = -_camMatrix.M32;
                viewDirection.Z = -_camMatrix.M33;
                changeIndex = _camMatIndex;
                return true;
            }
            return false;
        }

        void ICamera.GetViewMatrix(out Matrix matrix)
        {
            if (_viewMatDirty)
            {
                Matrix.Invert(ref _camMatrix, out _viewMatrix);
                _viewMatDirty = false;
            }
            matrix = _viewMatrix;
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


        /// <summary>
        /// Get the cull planes for this camera
        /// </summary>
        /// <returns></returns>
        public Plane[] GetCullingPlanes()
        {
            if (_viewMatDirty)
            {
                Matrix.Invert(ref _camMatrix, out _viewMatrix);
                _viewMatDirty = false;
            }
            Plane[] planes = _proj.GetFrustumPlanes(ref _viewMatrix, _camMatChanged);
            _camMatChanged = false;
            return planes;
        }
    }
}
