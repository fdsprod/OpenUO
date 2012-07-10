#region License Header
/***************************************************************************
 *   Copyright (c) 2011 OpenUO Software Team.
 *   All Right Reserved.
 *
 *   $Id: VertexPositionColorTexture.cs 14 2011-10-31 07:03:12Z fdsprod@gmail.com $:
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 ***************************************************************************/
#endregion

using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D9;

namespace Client.Graphics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexPositionColorTexture
    {
        [VertexElement(DeclarationUsage.Position, DeclarationType.Float3, 0)]
        public Vector3 Position;

        [VertexElement(DeclarationUsage.TextureCoordinate, DeclarationType.Float2, 0)]
        public Vector2 TextureCoordinate;

        [VertexElement(DeclarationUsage.Color, DeclarationType.Color, 0)]
        public Color Color;
    }
}
