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
    public sealed class DirectX9GumpAdapter : IndexedStorageAdapterBase, IGumpStorageAdapter<Texture2D>
    {
        private readonly DeviceContext _context;

        public DirectX9GumpAdapter(DirectX9IndexedStorageAdapterCreationParameters parameters)
            : base(parameters)
        {
            _context = parameters.Context;
        }

        public unsafe Texture2D GetGump(int index)
        {
            int length, extra;
            Stream stream = FileIndex.Seek(index, out length, out extra);

            if (stream == null)
                return null;

            int width = (extra >> 16) & 0xFFFF;
            int height = extra & 0xFFFF;

            if (width == 0 || height == 0)
                return null;

            Texture2D texture = new Texture2D(_context, width, height, 0, Usage.None, Format.A1R5G5B5, Pool.Managed);
            DataRectangle rect = texture.LockRectangle(0, LockFlags.None);
            BinaryReader bin = new BinaryReader(stream);

            int[] lookups = new int[height];
            int start = (int)bin.BaseStream.Position;

            for (int i = 0; i < height; ++i)
                lookups[i] = start + (bin.ReadInt32() * 4);

            ushort* line = (ushort*)rect.DataPointer;
            int delta = rect.Pitch >> 1;

            for (int y = 0; y < height; ++y, line += delta)
            {
                bin.BaseStream.Seek(lookups[y], SeekOrigin.Begin);

                ushort* cur = line;
                ushort* end = line + width;

                while (cur < end)
                {
                    ushort color = bin.ReadUInt16();
                    ushort* next = cur + bin.ReadUInt16();

                    if (color == 0)
                    {
                        cur = next;
                    }
                    else
                    {
                        color ^= 0x8000;

                        while (cur < next)
                            *cur++ = color;
                    }
                }
            }

            texture.UnlockRectangle(0);
            return texture;
        }
    }
}
