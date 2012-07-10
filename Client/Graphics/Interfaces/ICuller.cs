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
    public interface ICuller : ICullPrimitive
    {
        /// <summary>
        /// FrustumCull test a box. World matrix will be inferred (eg, current <see cref="DrawState"/> <see cref="DrawState.GetWorldMatrix(out Matrix)">world matrix</see>)
        /// </summary>
        /// <param name="min">box minimum point (in local space)</param>
        /// <param name="max">box maximum point (in local space)</param>
        /// <returns>True if the test passes (eg, box is on screen, box intersects shape, etc)</returns>
        bool TestBox(ref Vector3 min, ref Vector3 max);
        /// <summary>
        /// FrustumCull test a box. World matrix will be inferred (eg, current <see cref="DrawState"/> <see cref="DrawState.GetWorldMatrix(out Matrix)">world matrix</see>)
        /// </summary>
        /// <param name="min">box minimum point (in local space)</param>
        /// <param name="max">box maximum point (in local space)</param>
        /// <returns>True if the test passes (eg, box is on screen, box intersects shape, etc)</returns>
        bool TestBox(Vector3 min, Vector3 max);

        /// <summary>
        /// FrustumCull test a box with a local matrix. World matrix will be inferred (eg, current <see cref="DrawState"/> <see cref="DrawState.GetWorldMatrix(out Matrix)">world matrix</see>)
        /// </summary>
        /// <param name="min">box minimum point (in local space)</param>
        /// <param name="max">box maximum point (in local space)</param>
        /// <param name="boxMatrix">Local matrix of the box</param>
        /// <returns>True if the test passes (eg, box is on screen, box intersects shape, etc)</returns>
        bool TestBox(ref Vector3 min, ref Vector3 max, ref Matrix boxMatrix);
        /// <summary>
        /// FrustumCull test a box with a local matrix. World matrix will be inferred (eg, current <see cref="DrawState"/> <see cref="DrawState.GetWorldMatrix(out Matrix)">world matrix</see>)
        /// </summary>
        /// <param name="min">box minimum point (in local space)</param>
        /// <param name="max">box maximum point (in local space)</param>
        /// <param name="boxMatrix">Local matrix of the box</param>
        /// <returns>True if the test passes (eg, box is on screen, box intersects shape, etc)</returns>
        bool TestBox(Vector3 min, Vector3 max, ref Matrix boxMatrix);

        /// <summary>
        /// FrustumCull test a sphere. World position will be inferred (eg, current <see cref="DrawState"/> <see cref="DrawState.GetWorldMatrix(out Matrix)">world matrix</see>)
        /// </summary>
        /// <param name="radius">Radius of the sphere</param>
        /// <returns>True if the test passes (eg, sphere is on screen, sphere intersects shape, etc)</returns>
        bool TestSphere(float radius);
        /// <summary>
        /// FrustumCull test a sphere. World position will be inferred (eg, current <see cref="DrawState"/> <see cref="DrawState.GetWorldMatrix(out Matrix)">world matrix</see>)
        /// </summary>
        /// <param name="radius">Radius of the sphere</param>
        /// <param name="position">Position of the sphere in relation to the world matrix</param>
        /// <returns>True if the test passes (eg, sphere is on screen, sphere intersects shape, etc)</returns>
        bool TestSphere(float radius, ref Vector3 position);
        /// <summary>
        /// FrustumCull test a sphere. World position will be inferred (eg, current <see cref="DrawState"/> <see cref="DrawState.GetWorldMatrix(out Matrix)">world matrix</see>)
        /// </summary>
        /// <param name="radius">Radius of the sphere</param>
        /// <param name="position">Position of the sphere in relation to the world matrix</param>
        /// <returns>True if the test passes (eg, sphere is on screen, sphere intersects shape, etc)</returns>
        bool TestSphere(float radius, Vector3 position);


        /// <summary>
        /// <para>Intersect a box. World matrix will be inferred (eg, current <see cref="DrawState"/> <see cref="DrawState.GetWorldMatrix(out Matrix)">world matrix</see>)</para>
        /// <para>Note: Intersection tests may be less efficient than boolean TestBox</para>
        /// </summary>
        /// <param name="min">box minimum point (in local space)</param>
        /// <param name="max">box maximum point (in local space)</param>
        /// <returns>Intersetction test result</returns>
        ContainmentType IntersectBox(ref Vector3 min, ref Vector3 max);
        /// <summary>
        /// <para>Intersect a box. World matrix will be inferred (eg, current <see cref="DrawState"/> <see cref="DrawState.GetWorldMatrix(out Matrix)">world matrix</see>)</para>
        /// <para>Note: Intersection tests may be less efficient than boolean TestBox</para>
        /// </summary>
        /// <param name="min">box minimum point (in local space)</param>
        /// <param name="max">box maximum point (in local space)</param>
        /// <returns>Intersetction test result</returns>
        ContainmentType IntersectBox(Vector3 min, Vector3 max);

        /// <summary>
        /// <para>Intersect a box with a local matrix. World matrix will be inferred (eg, current <see cref="DrawState"/> <see cref="DrawState.GetWorldMatrix(out Matrix)">world matrix</see>)</para>
        /// <para>Note: Intersection tests may be less efficient than boolean TestBox</para>
        /// </summary>
        /// <param name="min">box minimum point (in local space)</param>
        /// <param name="max">box maximum point (in local space)</param>
        /// <param name="boxMatrix">Local matrix of the box</param>
        /// <returns>Intersetction test result</returns>
        ContainmentType IntersectBox(ref Vector3 min, ref Vector3 max, ref Matrix boxMatrix);
        /// <summary>
        /// <para>Intersect a box with a local matrix. World matrix will be inferred (eg, current <see cref="DrawState"/> <see cref="DrawState.GetWorldMatrix(out Matrix)">world matrix</see>)</para>
        /// <para>Note: Intersection tests may be less efficient than boolean TestBox</para>
        /// </summary>
        /// <param name="min">box minimum point (in local space)</param>
        /// <param name="max">box maximum point (in local space)</param>
        /// <param name="boxMatrix">Local matrix of the box</param>
        /// <returns>Intersetction test result</returns>
        ContainmentType IntersectBox(Vector3 min, Vector3 max, ref Matrix boxMatrix);

        /// <summary>
        /// <para>Intersect a sphere. World position will be inferred (eg, current <see cref="DrawState"/> <see cref="DrawState.GetWorldMatrix(out Matrix)">world matrix</see>)</para>
        /// <para>Note: Intersection tests may be less efficient than boolean TestSphere</para>
        /// </summary>
        /// <param name="radius">Radius of the sphere</param>
        /// <returns>Intersetction test result</returns>
        ContainmentType IntersectSphere(float radius);
        /// <summary>
        /// <para>Intersect a sphere. World position will be inferred (eg, current <see cref="DrawState"/> <see cref="DrawState.GetWorldMatrix(out Matrix)">world matrix</see>)</para>
        /// <para>Note: Intersection tests may be less efficient than boolean TestSphere</para>
        /// </summary>
        /// <param name="radius">Radius of the sphere</param>
        /// <param name="position">Position of the sphere in relation to the world matrix</param>
        /// <returns>Intersetction test result</returns>
        ContainmentType IntersectSphere(float radius, ref Vector3 position);
        /// <summary>
        /// <para>Intersect a sphere. World position will be inferred (eg, current <see cref="DrawState"/> <see cref="DrawState.GetWorldMatrix(out Matrix)">world matrix</see>)</para>
        /// <para>Note: Intersection tests may be less efficient than boolean TestSphere</para>
        /// </summary>
        /// <param name="radius">Radius of the sphere</param>
        /// <param name="position">Position of the sphere in relation to the world matrix</param>
        /// <returns>Intersetction test result</returns>
        ContainmentType IntersectSphere(float radius, Vector3 position);


        /// <summary>
        /// Gets the world matrix for the current context, eg the top of the rendering world matrix stack (as stored in the <see cref="DrawState"/> object)
        /// </summary>
        /// <param name="world"></param>
        void GetWorldMatrix(out Matrix world);
        /// <summary>
        /// Gets the world position for the current context, eg the top of the rendering world matrix stack (as stored in the <see cref="DrawState"/> object)
        /// </summary>
        /// <param name="position"></param>
        void GetWorldPosition(out Vector3 position);
        /// <summary>
        /// Gets the current rendering frame index
        /// </summary>
        /// <remarks><para>This value is useful if an object needs to calculate culling data once per frame, but doesn't want to recalculate it if culled more than once during the frame</para>
        /// <para>Implementations should return the same value that <see cref="DrawState.FrameIndex"/> would return for the current frame</para></remarks>
        int FrameIndex { get; }
    }
}
