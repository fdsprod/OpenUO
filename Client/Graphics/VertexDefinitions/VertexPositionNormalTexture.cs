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

using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D9;

namespace Client.Graphics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexPositionNormalTexture
    {
        [VertexElement(DeclarationUsage.Position, DeclarationType.Float3, 0)]
        public Vector3 Position;

        [VertexElement(DeclarationUsage.Normal, DeclarationType.Float3, 0)]
        public Vector3 Normal;

        [VertexElement(DeclarationUsage.TextureCoordinate, DeclarationType.Float3, 0)]
        public Vector2 TextureCoordinate;
    }
}
