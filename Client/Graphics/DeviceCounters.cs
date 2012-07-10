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


namespace Client.Graphics
{
    public static class DeviceCounters
    {
        public const string VerticesDrawn = "VerticesDrawn";
        public const string IndicesDrawn = "IndicesDrawn";
        public const string PrimitivesDrawn = "PrimitivesDrawn";
        public const string SetWorldMatrix = "SetWorldMatrix";
        public const string SetViewMatrix = "SetViewMatrix";
        public const string SetProjectionMatrix = "SetProjectionMatrix";
        public const string PushWorldMatrix = "PushWorldMatrix";
        public const string PushViewMatrix = "PushViewMatrix";
        public const string PushProjectionMatrix = "PushProjectionMatrix";
        public const string PushTranslateWorldMatrix = "PushTranslateWorldMatrix";
        public const string PushTranslateViewMatrix = "PushTranslateViewMatrix";
        public const string PushTranslateProjectionMatrix = "PushTranslateProjectionMatrix";
        public const string PushMultiplyTranslateWorldMatrix = "PushMultiplyTranslateWorldMatrix";
        public const string PushMultiplyTranslateViewMatrix = "PushMultiplyTranslateViewMatrix";
        public const string PushMultiplyTranslateProjectionMatrix = "PushMultiplyTranslateProjectionMatrix";
        public const string PushMultiplyWorldMatrix = "PushMultiplyWorldMatrix";
        public const string PushMultiplyViewMatrix = "PushMultiplyViewMatrix";
        public const string PushMultiplyProjectionMatrix = "PushMultiplyProjectionMatrix";
        public const string MatrixMultiplyCalculate = "MatrixMultiplyCalculate";
        public const string MatrixViewChanged = "MatrixViewChanged";
        public const string MatrixProjectionChanged = "MatrixProjectionChanged";
        public const string SetCamera = "SetCamera";
        public const string DynamicIndexBufferBytesCopied = "DynamicIndexBufferBytesCopied";
        public const string IndexBufferBytesCopied = "IndexBufferBytesCopied";
        public const string VertexBufferByesCopied = "VertexBufferByesCopied";
        public const string DrawIndexedPrimitive = "DrawIndexedPrimitive";
        public const string DrawIndexedUserPrimitivesST = "DrawIndexedUserPrimitivesST";
        public const string DrawUserPrimitivesT = "DrawUserPrimitivesT";
        public const string DrawPrimitives = "DrawPrimitives";
        public const string LinesDrawn = "LinesDrawn";
        public const string PointsDrawn = "PointsDrawn";
        public const string TrianglesDrawn = "TrianglesDrawn";
        public const string SetCameraCount = "SetCameraCount";
        public const string DefaultCullerTestBoxCount = "DefaultCullerTestBoxCount";
        public const string DefaultCullerTestBoxPreCulledCount = "DefaultCullerTestBoxPreCulledCount";
        public const string DefaultCullerTestBoxCulledCount = "DefaultCullerTestBoxCulledCount";
        public const string DefaultCullerTestBoxPostCulledCount = "DefaultCullerTestBoxPostCulledCount";
        public const string DefaultCullerTestSpherePreCulledCount = "DefaultCullerTestSpherePreCulledCount";
        public const string DefaultCullerTestSphereCulledCount = "DefaultCullerTestSphereCulledCount";
        public const string DefaultCullerTestSpherePostCulledCount = "DefaultCullerTestSpherePostCulledCount";
        public const string DefaultCullerTestSphereCount = "DefaultCullerTestSphereCount";
        public const string BufferClearTargetCount = "BufferClearTargetCount";
        public const string BufferClearStencilCount = "BufferClearStencilCount";
        public const string BufferClearDepthCount = "BufferClearDepthCount";
        public const string ShaderConstantMatrixInverseCalculateCount = "ShaderConstantMatrixInverseCalculateCount";
        public const string ShaderConstantMatrixValueSetCount = "ShaderConstantMatrixValueSetCount";
        public const string ShaderConstantMatrixTransposeCalculateCount = "ShaderConstantMatrixTransposeCalculateCount";
        public const string ShaderConstantMatrixMultiplyCalculateCount = "ShaderConstantMatrixMultiplyCalculateCount";
    }    
}
