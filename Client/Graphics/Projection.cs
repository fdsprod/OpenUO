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

namespace Client.Graphics
{
    public class Projection
    {
        private static int _changeBaseIndex = 2;

        private readonly BoundingFrustum frustum = new BoundingFrustum(Matrix.Identity);
        private readonly Plane[] frustumPlanes = new Plane[6];
        private Matrix _matrix;
        private Vector4 _region = new Vector4(0, 0, 1, 1);
        private float _near = 1;
        private float _far = 1000.0f;
        private float _fov = MathHelper.PiOver2;
        private float _computedAspect;
        private float? _aspect = null;
        private int _changeIndex = 1;
        private bool frustumDirty = true;
        private bool _set = false;
        private bool _orthographic;
        private bool _isExtendedClass;
        private bool _leftHandedProjection;
        private bool _debugPauseCullPlaneUpdates;
        
        public bool UseLeftHandedProjection
        {
            get { return _leftHandedProjection; }
            set { if (_leftHandedProjection != value) { _leftHandedProjection = value; _set = false; _changeIndex = System.Threading.Interlocked.Increment(ref _changeBaseIndex); } }
        }

        public bool PauseFrustumCullPlaneUpdates
        {
            get { return _debugPauseCullPlaneUpdates; }
            set { if (value != _debugPauseCullPlaneUpdates) { _debugPauseCullPlaneUpdates = value; _set = false; _changeIndex = System.Threading.Interlocked.Increment(ref _changeBaseIndex); } }
        }

        public bool Orthographic
        {
            get { return _orthographic; }
            set { if (_orthographic != value) { _orthographic = value; _set = false; _changeIndex = System.Threading.Interlocked.Increment(ref _changeBaseIndex); } }
        }

        public Vector4 Region
        {
            get { return _region; }
            set { if (_region != value) { _region = value; _set = false; _changeIndex = System.Threading.Interlocked.Increment(ref _changeBaseIndex); } }
        }

        public Matrix ProjectionMatrix
        {
            get
            {
                if (!_set)
                {
                    SetProjection();
                }
                return _matrix;
            }
        }

        public float FieldOfView
        {
            get { return _fov; }
            set { if (_fov != value) { _fov = value; _set = false; _changeIndex = System.Threading.Interlocked.Increment(ref _changeBaseIndex); } }
        }

        public float? Aspect
        {
            get { return _aspect; }
            set { if (_aspect != value) { _aspect = value; _set = false; _changeIndex = System.Threading.Interlocked.Increment(ref _changeBaseIndex); } }
        }

        public float FarClip
        {
            get { return _far; }
            set { if (_far != value) { _far = value; _set = false; _changeIndex = System.Threading.Interlocked.Increment(ref _changeBaseIndex); } }
        }

        public float NearClip
        {
            get { return _near; }
            set { if (_near != value) { _near = value; _set = false; _changeIndex = System.Threading.Interlocked.Increment(ref _changeBaseIndex); } }
        }

        public bool GetProjectionMatrix(ref Matrix matrix, ref Vector2 drawTargetSize, ref int changeIndex)
        {
            if (!_aspect.HasValue && !_orthographic && drawTargetSize.Y != 0)
            {
                float value = drawTargetSize.X / drawTargetSize.Y;
                if (_computedAspect != value)
                {
                    _computedAspect = value;
                    changeIndex = _changeIndex - 1;
                    _set = false;
                }
            }

            if (changeIndex != _changeIndex)
            {
                changeIndex = _changeIndex;
                GetProjectionMatrix(out matrix, ref drawTargetSize);
                return true;
            }

            return false;
        }

        public void CopyFrom(Projection projection)
        {
            if (projection == null)
                throw new ArgumentNullException();

            _aspect = projection._aspect;
            _changeIndex = projection._changeIndex;
            _computedAspect = projection._computedAspect;
            _far = projection._far;
            _fov = projection._fov;
            _leftHandedProjection = projection._leftHandedProjection;
            _matrix = projection._matrix;
            _near = projection._near;
            _orthographic = projection._orthographic;
            _region = projection._region;

            _set = false;
            _changeIndex = System.Threading.Interlocked.Increment(ref _changeBaseIndex);
        }

        public Projection()
        {
            _isExtendedClass = (GetType() != typeof(Projection));
        }

        public Projection(float fieldOfView, float nearPlane, float farPlane, float aspectRatio)
        {
            FieldOfView = fieldOfView;
            NearClip = nearPlane;
            FarClip = farPlane;
            Aspect = aspectRatio;
        }

        internal BoundingFrustum GetFrustum(ref Matrix viewMatrix, bool viewChanged)
        {
            if (frustumDirty || !_set || viewChanged)
            {
                if (!_set)
                {
                    SetProjection();
                }

                if (!_debugPauseCullPlaneUpdates)
                {
                    Matrix m;
                    Matrix.Multiply(ref viewMatrix, ref _matrix, out m);
                    frustum.SetMatrix(ref m);

                    frustumPlanes[0] = frustum.Near;
                    frustumPlanes[1] = frustum.Far;
                    frustumPlanes[2] = frustum.Left;
                    frustumPlanes[3] = frustum.Right;
                    frustumPlanes[4] = frustum.Bottom;
                    frustumPlanes[5] = frustum.Top;

                    frustumDirty = false;
                }
            }

            return frustum;
        }

        internal Plane[] GetFrustumPlanes(ref Matrix viewMatrix, bool viewChanged)
        {
            if (frustumDirty || !_set || viewChanged)
            {
                if (!_set)
                {
                    SetProjection();
                }

                if (!_debugPauseCullPlaneUpdates)
                {
                    Matrix m;
                    Matrix.Multiply(ref viewMatrix, ref _matrix, out m);
                    frustum.SetMatrix(ref m);

                    frustumPlanes[0] = frustum.Far;
                    frustumPlanes[1] = frustum.Left;
                    frustumPlanes[2] = frustum.Right;
                    frustumPlanes[3] = frustum.Bottom;
                    frustumPlanes[4] = frustum.Top;
                    frustumPlanes[5] = frustum.Near;

                    frustumDirty = false;
                }
            }

            return frustumPlanes;
        }

        public float GetVerticalFovTangent()
        {
            return (float)Math.Tan(FieldOfView / 2);
        }

        public float GetHorizontalFovTangent()
        {
            return (float)Math.Tan(FieldOfView / 2) * _aspect.GetValueOrDefault(_computedAspect);
        }

        protected virtual void CalculateProjectionMatrix(out Matrix projection)
        {
            if (_orthographic)
                Matrix.OrthoLH(1, 1, _near, _far, out projection);
            else
            {
                Matrix.PerspectiveFovLH(_fov, _aspect.GetValueOrDefault(_computedAspect), _near, _far, out projection);
                if (_leftHandedProjection)
                {
                    projection.M33 *= -1;
                    projection.M34 *= -1;
                }
            }

            if (float.IsInfinity(projection.M11))
                throw new ArgumentException();
        }

        internal void GetProjectionMatrix(out Matrix matrix, ref Vector2 drawTargetSize)
        {
            if (!_aspect.HasValue && !_orthographic && drawTargetSize.Y != 0)
            {
                float value = drawTargetSize.X / drawTargetSize.Y;
                if (_computedAspect != value)
                {
                    _computedAspect = value;
                    _set = false;
                }
            }

            if (!_set)
                SetProjection();

            matrix = _matrix;
        }

        internal float GetVerticalFov()
        {
            return FieldOfView;
        }

        internal float GetHorizontalFov()
        {
            return FieldOfView * _aspect.GetValueOrDefault(_computedAspect);
        }

        internal bool GetCameraNearFarClip(ref Vector2 v, ref int changeIndex)
        {
            if (changeIndex != _changeIndex)
            {
                changeIndex = _changeIndex;
                v.X = _near;
                v.Y = _far;
                return true;
            }
            return false;
        }

        internal bool GetCameraHorizontalVerticalFovTangent(ref Vector2 v, ref int changeIndex)
        {
            if (changeIndex != _changeIndex)
            {
                changeIndex = _changeIndex;
                v.Y = GetVerticalFovTangent();
                v.X = v.Y * _aspect.GetValueOrDefault(_computedAspect);
                return true;
            }
            return false;
        }

        internal bool GetCameraHorizontalVerticalFov(ref Vector2 v, ref int changeIndex)
        {
            if (changeIndex != _changeIndex)
            {
                changeIndex = _changeIndex;
                v.Y = FieldOfView;
                v.X = FieldOfView * _aspect.GetValueOrDefault(_computedAspect);
                return true;
            }
            return false;
        }

        private void SetProjection()
        {
            CalculateProjectionMatrix(out _matrix);

            if (_isExtendedClass)
            {
                Vector4 zero = Vector4.UnitW;
                Vector4 one = new Vector4(0, 0, 1, 1);
                Vector4.Transform(ref one, ref _matrix, out one);
                Vector4.Transform(ref zero, ref _matrix, out zero);
                zero.Z = 0;

                if (zero != Vector4.Zero || one.X != 0 || one.Y != 0)
                    throw new InvalidOperationException("Projection matix must not modify the X/Y of positions on the z-axis. A zero vector multiplication should also have an output W of zero");
            }

            if (_region.X != 0 || _region.Y != 0 || _region.Z != 1 || _region.W != 1)
            {
                _matrix *= Matrix.Translation(1 - (_region.X + _region.Z), (_region.Y + _region.W) - 1, 0) * Matrix.Scaling(1.0f / (_region.Z - _region.X), 1.0f / (_region.W - _region.Y), 1);
            }

            frustumDirty = true;
            _set = true;
        }

    }
}
