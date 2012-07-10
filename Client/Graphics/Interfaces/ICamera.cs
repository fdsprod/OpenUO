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
    public interface ICamera : ICullPrimitive
    {
        bool ReverseBackfaceCulling { get; }

        Plane[] GetCullingPlanes();

        void GetViewMatrix(out Matrix matrix);
        void GetCameraMatrix(out Matrix matrix);
        bool GetProjectionMatrix(ref Matrix matrix, ref Vector2 drawTargetSize, ref int changeIndex);
        void GetProjectionMatrix(out Matrix matrix, ref Vector2 drawTargetSize);
        void GetProjectionMatrix(out Matrix matrix, Vector2 drawTargetSize);
        bool GetViewMatrix(ref Matrix matrix, ref int changeIndex);
        bool GetCameraMatrix(ref Matrix matrix, ref int changeIndex);
        void GetCameraPosition(out Vector3 viewPoint);
        void GetCameraViewDirection(out Vector3 viewDirection);
        bool GetCameraPosition(ref Vector3 viewPoint, ref int changeIndex);
        bool GetCameraViewDirection(ref Vector3 viewDirection, ref int changeIndex);
        void GetCameraNearFarClip(out Vector2 nearFarClip);
        bool GetCameraNearFarClip(ref Vector2 nearFarClip, ref int changeIndex);
        void GetCameraHorizontalVerticalFov(out Vector2 hvFov);
        bool GetCameraHorizontalVerticalFov(ref Vector2 hvFov, ref int changeIndex);
        void GetCameraHorizontalVerticalFovTangent(out Vector2 hvFovTan);
        bool GetCameraHorizontalVerticalFovTangent(ref Vector2 hvFovTan, ref int changeIndex);
        bool ProjectToTarget(ref Vector3 position, out Vector2 coordinate, Viewport target);
        void ProjectFromTarget(ref Vector2 coordinate, float projectDepth, out Vector3 position, Viewport target);
        bool ProjectToCoordinate(ref Vector3 position, out Vector2 coordinate);
        void ProjectFromCoordinate(ref Vector2 coordinate, float projectDepth, out Vector3 position);
    }
}
