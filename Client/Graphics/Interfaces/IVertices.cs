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
using SharpDX.Direct3D9;

namespace Client.Graphics
{
    public interface IVertices : IDisposable
    {
        int Count { get; }
        int Stride { get; }
        Usage ResourceUsage { get; }
        Type VertexType { get; }

        void Draw(DrawState state, IIndices indices, PrimitiveType primitiveType);
        void Draw(DrawState state, IIndices indices, PrimitiveType primitiveType, int primitveCount, int startIndex, int vertexOffset);
        void SetDirty();
        void SetDirtyRange(int startIndex, int count);
    }
}
