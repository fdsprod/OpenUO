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
    internal static class FrustumCull
    {
        internal static bool SphereInFrustum(Plane[] frustum, float radius, ref Vector3 position)
        {
            foreach (Plane plane in frustum)
            {
                if (plane.Normal.X * position.X + plane.Normal.Y * position.Y + plane.Normal.Z * position.Z + plane.D > radius)
                    return false;
            }
            return true;
        }

        internal static ContainmentType SphereIntersectsFrustum(Plane[] frustum, float radius, ref Vector3 position)
        {
            bool intersects = false;
            foreach (Plane plane in frustum)
            {
                float distance = plane.Normal.X * position.X + plane.Normal.Y * position.Y + plane.Normal.Z * position.Z + plane.D;
                if (distance > radius)
                    return ContainmentType.Disjoint;
                if (distance > -radius)
                    intersects = true;
            }
            return intersects ? ContainmentType.Intersects : ContainmentType.Contains;
        }

        internal static bool BoxInFrustum(Plane[] frustum, ref Vector3 minExtents, ref Vector3 maxExtents, ref Matrix world)
        {
            float dot;
            Vector3 point;

            foreach (Plane plane in frustum)
            {
                dot = plane.Normal.X * world.M11 +
                        plane.Normal.Y * world.M12 +
                        plane.Normal.Z * world.M13;

                if (dot >= 0)
                {
                    point.X = world.M41 + world.M11 * minExtents.X;
                    point.Y = world.M42 + world.M12 * minExtents.X;
                    point.Z = world.M43 + world.M13 * minExtents.X;
                }
                else
                {
                    point.X = world.M41 + world.M11 * maxExtents.X;
                    point.Y = world.M42 + world.M12 * maxExtents.X;
                    point.Z = world.M43 + world.M13 * maxExtents.X;
                }


                dot = plane.Normal.X * world.M21 +
                        plane.Normal.Y * world.M22 +
                        plane.Normal.Z * world.M23;

                if (dot >= 0)
                {
                    point.X += world.M21 * minExtents.Y;
                    point.Y += world.M22 * minExtents.Y;
                    point.Z += world.M23 * minExtents.Y;
                }
                else
                {
                    point.X += world.M21 * maxExtents.Y;
                    point.Y += world.M22 * maxExtents.Y;
                    point.Z += world.M23 * maxExtents.Y;
                }



                dot = plane.Normal.X * world.M31 +
                        plane.Normal.Y * world.M32 +
                        plane.Normal.Z * world.M33;

                if (dot >= 0)
                {
                    point.X += world.M31 * minExtents.Z;
                    point.Y += world.M32 * minExtents.Z;
                    point.Z += world.M33 * minExtents.Z;
                }
                else
                {
                    point.X += world.M31 * maxExtents.Z;
                    point.Y += world.M32 * maxExtents.Z;
                    point.Z += world.M33 * maxExtents.Z;
                }

                if (plane.Normal.X * point.X + plane.Normal.Y * point.Y + plane.Normal.Z * point.Z + plane.D > 0)
                    return false;
            }
            return true;
        }

        internal static ContainmentType BoxIntersectsFrustum(Plane[] frustum, ref Vector3 minExtents, ref Vector3 maxExtents, ref Matrix world)
        {
            float dot;
            Vector3 point, overlapPoint;
            bool overlap = false;

            foreach (Plane plane in frustum)
            {
                dot = plane.Normal.X * world.M11 +
                        plane.Normal.Y * world.M12 +
                        plane.Normal.Z * world.M13;

                if (dot >= 0)
                {
                    point.X = world.M41 + world.M11 * minExtents.X;
                    point.Y = world.M42 + world.M12 * minExtents.X;
                    point.Z = world.M43 + world.M13 * minExtents.X;

                    overlapPoint.X = world.M41 + world.M11 * maxExtents.X;
                    overlapPoint.Y = world.M42 + world.M12 * maxExtents.X;
                    overlapPoint.Z = world.M43 + world.M13 * maxExtents.X;
                }
                else
                {
                    point.X = world.M41 + world.M11 * maxExtents.X;
                    point.Y = world.M42 + world.M12 * maxExtents.X;
                    point.Z = world.M43 + world.M13 * maxExtents.X;

                    overlapPoint.X = world.M41 + world.M11 * minExtents.X;
                    overlapPoint.Y = world.M42 + world.M12 * minExtents.X;
                    overlapPoint.Z = world.M43 + world.M13 * minExtents.X;
                }


                dot = plane.Normal.X * world.M21 +
                        plane.Normal.Y * world.M22 +
                        plane.Normal.Z * world.M23;

                if (dot >= 0)
                {
                    point.X += world.M21 * minExtents.Y;
                    point.Y += world.M22 * minExtents.Y;
                    point.Z += world.M23 * minExtents.Y;

                    overlapPoint.X += world.M21 * maxExtents.Y;
                    overlapPoint.Y += world.M22 * maxExtents.Y;
                    overlapPoint.Z += world.M23 * maxExtents.Y;
                }
                else
                {
                    point.X += world.M21 * maxExtents.Y;
                    point.Y += world.M22 * maxExtents.Y;
                    point.Z += world.M23 * maxExtents.Y;

                    overlapPoint.X += world.M21 * minExtents.Y;
                    overlapPoint.Y += world.M22 * minExtents.Y;
                    overlapPoint.Z += world.M23 * minExtents.Y;
                }



                dot = plane.Normal.X * world.M31 +
                        plane.Normal.Y * world.M32 +
                        plane.Normal.Z * world.M33;

                if (dot >= 0)
                {
                    point.X += world.M31 * minExtents.Z;
                    point.Y += world.M32 * minExtents.Z;
                    point.Z += world.M33 * minExtents.Z;

                    overlapPoint.X += world.M31 * maxExtents.Z;
                    overlapPoint.Y += world.M32 * maxExtents.Z;
                    overlapPoint.Z += world.M33 * maxExtents.Z;
                }
                else
                {
                    point.X += world.M31 * maxExtents.Z;
                    point.Y += world.M32 * maxExtents.Z;
                    point.Z += world.M33 * maxExtents.Z;

                    overlapPoint.X += world.M31 * minExtents.Z;
                    overlapPoint.Y += world.M32 * minExtents.Z;
                    overlapPoint.Z += world.M33 * minExtents.Z;
                }

                if (plane.Normal.X * point.X + plane.Normal.Y * point.Y + plane.Normal.Z * point.Z + plane.D > 0)
                    return ContainmentType.Disjoint;

                if (plane.Normal.X * overlapPoint.X + plane.Normal.Y * overlapPoint.Y + plane.Normal.Z * overlapPoint.Z + plane.D > 0)
                    overlap = true;
            }
            if (overlap)
                return ContainmentType.Intersects;
            return ContainmentType.Contains;
        }

        internal static bool AABBInFrustum(Plane[] frustum, ref Vector3 minExtents, ref Vector3 maxExtents)
        {
            Vector3 point;

            foreach (Plane plane in frustum)
            {
                if (plane.Normal.X >= 0)
                    point.X = minExtents.X;
                else
                    point.X = maxExtents.X;

                if (plane.Normal.Y >= 0)
                    point.Y = minExtents.Y;
                else
                    point.Y = maxExtents.Y;

                if (plane.Normal.Z >= 0)
                    point.Z = minExtents.Z;
                else
                    point.Z = maxExtents.Z;

                if (plane.Normal.X * point.X + plane.Normal.Y * point.Y + plane.Normal.Z * point.Z + plane.D > 0)
                    return false;
            }
            return true;
        }

        internal static ContainmentType AABBIntersectsFrustum(Plane[] frustum, ref Vector3 minExtents, ref Vector3 maxExtents)
        {
            bool overlap = false;

            Vector3 point, overlapPoint;

            foreach (Plane plane in frustum)
            {
                if (plane.Normal.X >= 0)
                {
                    point.X = minExtents.X;
                    overlapPoint.X = maxExtents.X;
                }
                else
                {
                    point.X = maxExtents.X;
                    overlapPoint.X = minExtents.X;
                }

                if (plane.Normal.Y >= 0)
                {
                    point.Y = minExtents.Y;
                    overlapPoint.Y = maxExtents.Y;
                }
                else
                {
                    point.Y = maxExtents.Y;
                    overlapPoint.Y = minExtents.Y;
                }

                if (plane.Normal.Z >= 0)
                {
                    point.Z = minExtents.Z;
                    overlapPoint.Z = maxExtents.Z;
                }
                else
                {
                    point.Z = maxExtents.Z;
                    overlapPoint.Z = minExtents.Z;
                }

                if (plane.Normal.X * point.X + plane.Normal.Y * point.Y + plane.Normal.Z * point.Z + plane.D > 0)
                    return ContainmentType.Disjoint;

                if (plane.Normal.X * overlapPoint.X + plane.Normal.Y * overlapPoint.Y + plane.Normal.Z * overlapPoint.Z + plane.D > 0)
                    overlap = true;
            }
            if (overlap)
                return ContainmentType.Intersects;
            return ContainmentType.Contains;
        }
    }
}
