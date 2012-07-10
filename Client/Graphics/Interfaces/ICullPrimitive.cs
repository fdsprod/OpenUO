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
    public interface ICullPrimitive
    {
        /// <summary>
        /// FrustumCull test a world space box.
        /// </summary>
        /// <param name="min">box minimum point (in local space)</param>
        /// <param name="max">box maximum point (in local space)</param>
        /// <param name="world">Absolute world-space world matrix of the box (current world matrix is ignored)</param>
        /// <returns>True if the test passes (eg, box is on screen, box intersects shape, etc)</returns>
        bool TestWorldBox(ref Vector3 min, ref Vector3 max, ref Matrix world);
        /// <summary>
        /// FrustumCull test a world space box.
        /// </summary>
        /// <param name="min">box minimum point (in local space)</param>
        /// <param name="max">box maximum point (in local space)</param>
        /// <param name="world">Absolute world-space world matrix of the box (current world matrix is ignored)</param>
        /// <returns>True if the test passes (eg, box is on screen, box intersects shape, etc)</returns>
        bool TestWorldBox(Vector3 min, Vector3 max, ref Matrix world);

        /// <summary>
        /// FrustumCull test an axis-aligned bounding box.
        /// </summary>
        /// <param name="min">box minimum point</param>
        /// <param name="max">box maximum point</param>
        /// <returns>True if the test passes (eg, box is on screen, box intersects shape, etc)</returns>
        bool TestWorldBox(ref Vector3 min, ref Vector3 max);
        /// <summary>
        /// FrustumCull test an axis-aligned bounding box.
        /// </summary>
        /// <param name="min">box minimum point</param>
        /// <param name="max">box maximum point</param>
        /// <returns>True if the test passes (eg, box is on screen, box intersects shape, etc)</returns>
        bool TestWorldBox(Vector3 min, Vector3 max);

        /// <summary>
        /// FrustumCull test a sphere.
        /// </summary>
        /// <param name="position">Absolute world-space position of the sphere (current world matrix is ignored)</param>
        /// <param name="radius">Radius of the sphere</param>
        /// <returns>True if the test passes (eg, sphere is on screen, sphere intersects shape, etc)</returns>
        bool TestWorldSphere(float radius, ref Vector3 position);
        /// <summary>
        /// FrustumCull test a sphere.
        /// </summary>
        /// <param name="position">Absolute world-space position of the sphere (current world matrix is ignored)</param>
        /// <param name="radius">Radius of the sphere</param>
        /// <returns>True if the test passes (eg, sphere is on screen, sphere intersects shape, etc)</returns>
        bool TestWorldSphere(float radius, Vector3 position);


        /// <summary>
        /// <para>Intersect test a world space box.</para>
        /// <para>Note: Intersection tests may be less efficient than boolean TestWorldBox</para>
        /// </summary>
        /// <param name="min">box minimum point (in local space)</param>
        /// <param name="max">box maximum point (in local space)</param>
        /// <param name="world">Absolute world-space world matrix of the box (current world matrix is ignored)</param>
        /// <returns>Intersetction test result</returns>
        ContainmentType IntersectWorldBox(ref Vector3 min, ref Vector3 max, ref Matrix world);
        /// <summary>
        /// <para>Intersect test a world space box.</para>
        /// <para>Note: Intersection tests may be less efficient than boolean TestWorldBox</para>
        /// </summary>
        /// <param name="min">box minimum point (in local space)</param>
        /// <param name="max">box maximum point (in local space)</param>
        /// <param name="world">Absolute world-space world matrix of the box (current world matrix is ignored)</param>
        /// <returns>Intersetction test result</returns>
        ContainmentType IntersectWorldBox(Vector3 min, Vector3 max, ref Matrix world);

        /// <summary>
        /// <para>Intersect test an axis aligned bounding box.</para>
        /// <para>Note: Intersection tests may be less efficient than boolean TestWorldBox</para>
        /// </summary>
        /// <param name="min">box minimum point</param>
        /// <param name="max">box maximum point</param>
        /// <returns>Intersetction test result</returns>
        ContainmentType IntersectWorldBox(ref Vector3 min, ref Vector3 max);
        /// <summary>
        /// <para>Intersect test an axis aligned bounding box.</para>
        /// <para>Note: Intersection tests may be less efficient than boolean TestWorldBox</para>
        /// </summary>
        /// <param name="min">box minimum point</param>
        /// <param name="max">box maximum point</param>
        /// <returns>Intersetction test result</returns>
        ContainmentType IntersectWorldBox(Vector3 min, Vector3 max);

        /// <summary>
        /// Intersect test a sphere.
        /// </summary>
        /// <param name="position">Absolute world-space position of the sphere (current world matrix is ignored)</param>
        /// <param name="radius">Radius of the sphere</param>
        /// <returns>Intersetction test result</returns>
        ContainmentType IntersectWorldSphere(float radius, ref Vector3 position);
        /// <summary>
        /// Intersect test a sphere.
        /// </summary>
        /// <param name="position">Absolute world-space position of the sphere (current world matrix is ignored)</param>
        /// <param name="radius">Radius of the sphere</param>
        /// <returns>Intersetction test result</returns>
        ContainmentType IntersectWorldSphere(float radius, Vector3 position);
    }
}
