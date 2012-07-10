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

using Client.Graphics;

using OpenUO.Ultima;
using OpenUO.Ultima.Adapters;
using SharpDX;
using SharpDX.Direct3D9;

namespace Client.Ultima.Adapters
{
    public sealed class DirectX9TexmapsAdapter : IndexedStorageAdapterBase, ITexmapStorageAdapter<Texture2D>
    {
        private readonly DeviceContext _context;

        public DirectX9TexmapsAdapter(DirectX9IndexedStorageAdapterCreationParameters parameters)
            : base(parameters)
        {
            _context = parameters.Context;
        }

        public unsafe Texture2D GetTexmap(int index)
        {
            int length, extra;
            Stream stream = FileIndex.Seek(index, out length, out extra);

            if (stream == null)
                return null;

            int size = extra == 0 ? 64 : 128;

            Texture2D texture = new Texture2D(_context, size, size, 0, Usage.None, Format.A1R5G5B5, Pool.Managed);
            DataRectangle rect = texture.LockRectangle(0, LockFlags.None);
            BinaryReader bin = new BinaryReader(stream);

            ushort* line = (ushort*)rect.DataPointer;
            int delta = rect.Pitch >> 1;

            for (int y = 0; y < size; ++y, line += delta)
            {
                ushort* cur = line;
                ushort* end = cur + size;

                while (cur < end)
                    *cur++ = (ushort)(bin.ReadUInt16() ^ 0x8000);
            }

            texture.UnlockRectangle(0);

            return texture;
        }
    }
}
