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

namespace Client.Graphics
{
    internal struct BoundingFrustum
    {
        private Plane[] _planes;
        private Vector3[] _corners;
        private bool _cornersDirty;

        public Plane Left
        {
            get
            {
                return _planes[0];
            }
        }

        public Plane Right
        {
            get
            {
                return _planes[1];
            }
        }

        public Plane Top
        {
            get
            {
                return _planes[2];
            }
        }

        public Plane Bottom
        {
            get
            {
                return _planes[3];
            }
        }

        public Plane Near
        {
            get
            {
                return _planes[4];
            }
        }

        public Plane Far
        {
            get
            {
                return _planes[5];
            }
        }

        public BoundingFrustum(Matrix viewProjMatrix)
        {
            _planes = new Plane[6];
            _corners = new Vector3[8];
            _cornersDirty = true;
            SetMatrix(ref viewProjMatrix);
        }

        public void SetMatrix(ref Matrix viewProjMatrix)
        {
            Plane left = new Plane(viewProjMatrix.M14 + viewProjMatrix.M11,
                              viewProjMatrix.M24 + viewProjMatrix.M21,
                              viewProjMatrix.M34 + viewProjMatrix.M31,
                              viewProjMatrix.M44 + viewProjMatrix.M41);
            left.Normalize();
            _planes[0] = left;

            Plane right = new Plane(viewProjMatrix.M14 - viewProjMatrix.M11,
                               viewProjMatrix.M24 - viewProjMatrix.M21,
                               viewProjMatrix.M34 - viewProjMatrix.M31,
                               viewProjMatrix.M44 - viewProjMatrix.M41);
            right.Normalize();
            _planes[1] = right;

            Plane top = new Plane(viewProjMatrix.M14 - viewProjMatrix.M12,
                             viewProjMatrix.M24 - viewProjMatrix.M22,
                             viewProjMatrix.M34 - viewProjMatrix.M32,
                             viewProjMatrix.M44 - viewProjMatrix.M42);
            top.Normalize();
            _planes[2] = top;

            Plane bottom = new Plane(viewProjMatrix.M14 + viewProjMatrix.M12,
                                viewProjMatrix.M24 + viewProjMatrix.M22,
                                viewProjMatrix.M34 + viewProjMatrix.M32,
                                viewProjMatrix.M44 + viewProjMatrix.M42);
            bottom.Normalize();
            _planes[3] = bottom;

            Plane near = new Plane(viewProjMatrix.M13, viewProjMatrix.M23, viewProjMatrix.M33, viewProjMatrix.M43);
            near.Normalize();
            _planes[4] = near;

            Plane far = new Plane(viewProjMatrix.M14 - viewProjMatrix.M13,
                             viewProjMatrix.M24 - viewProjMatrix.M23,
                             viewProjMatrix.M34 - viewProjMatrix.M33,
                             viewProjMatrix.M44 - viewProjMatrix.M43);
            far.Normalize();
            _planes[5] = far;

            _cornersDirty = true;
        }

        public void GetPlane(ref int index, out Plane plane)
        {
            plane = _planes[index];
        }

        public void GetCorners(Vector3[] array)
        {
            if (_cornersDirty)
            {
                ComputeCorners();
                _cornersDirty = false;
            }

            _corners.CopyTo(array, 0);
        }

        public Vector3[] GetCorners()
        {
            if (_cornersDirty)
            {
                ComputeCorners();
                _cornersDirty = false;
            }
            return (Vector3[])_corners.Clone();
        }

        private void ComputeCorners()
        {
            Ray ray;

            //Left-Top
            ComputeRay(ref _planes[0], ref _planes[2], out ray);
            ComputeRayIntersection(ref _planes[4], ref ray, 0);
            ComputeRayIntersection(ref _planes[5], ref ray, 3);

            //Left-Bottom
            ComputeRay(ref _planes[3], ref _planes[0], out ray);
            ComputeRayIntersection(ref _planes[4], ref ray, 1);
            ComputeRayIntersection(ref _planes[5], ref ray, 2);

            //Right-Top
            ComputeRay(ref _planes[2], ref _planes[1], out ray);
            ComputeRayIntersection(ref _planes[4], ref ray, 4);
            ComputeRayIntersection(ref _planes[5], ref ray, 7);

            //Right-Bottom
            ComputeRay(ref _planes[1], ref _planes[3], out ray);
            ComputeRayIntersection(ref _planes[4], ref ray, 5);
            ComputeRayIntersection(ref _planes[5], ref ray, 6);
        }

        private static void ComputeRay(ref Plane p1, ref Plane p2, out Ray ray)
        {
            Vector3 dir;
            Vector3.Cross(ref p1.Normal, ref p2.Normal, out dir);
            float invLengthSquared = 1.0f / dir.LengthSquared();
            Vector3 origin = Vector3.Cross((-p1.D * p2.Normal) + (p2.D * p1.Normal), dir) * invLengthSquared;

            ray.Position = origin;
            ray.Direction = dir;
        }

        private void ComputeRayIntersection(ref Plane p, ref Ray ray, int cornerIndex)
        {
            float NPDot;
            Vector3.Dot(ref p.Normal, ref ray.Position, out NPDot);
            float NDirDot;
            Vector3.Dot(ref p.Normal, ref ray.Direction, out NDirDot);

            float scale = (-p.D - NPDot) / NDirDot;

            _corners[cornerIndex] = ray.Position + (ray.Direction * scale);
        }
    }
}
