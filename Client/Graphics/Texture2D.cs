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

using System.IO;
using SharpDX.Direct3D9;

namespace Client.Graphics
{
    public class Texture2D : TextureBase
    {
        public static Texture2D FromMemory(DeviceContext context, byte[] buffer, Usage usage, Pool pool)
        {
            return new Texture2D(context, Texture.FromMemory(context, buffer, usage, pool));
        }

        public static Texture2D FromStream(DeviceContext context, Stream stream, Usage usage, Pool pool)
        {
            return new Texture2D(context, Texture.FromStream(context, stream, usage, pool));
        }

        public static Texture2D FromFile(DeviceContext context, string file, Usage usage, Pool pool)
        {
            using (var stream = File.OpenRead(file))
            {
                return FromStream(context, stream, usage, pool);
            }
        }

        public Texture2D(DeviceContext context, int width, int height, int levelCount, Usage usage, Format format, Pool pool)
            : base(context, width, height, levelCount, usage, format, pool)
        {
            RecreateOnReset = true;
        }

        internal Texture2D(DeviceContext context, Texture texture)
            : base(context, texture)
        {
            RecreateOnReset = true;
        }
    }
}
