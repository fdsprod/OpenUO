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
using System.IO;
using Client.Graphics;
using Client.Ultima;
using SharpDX;
using SharpDX.Direct3D9;

namespace OpenUO.Ultima.Adapters
{
    public class DirectX9ArtworkAdapter : IndexedStorageAdapterBase, IArtworkStorageAdapter<Texture2D>
    {
        private readonly DeviceContext _context;

        public DirectX9ArtworkAdapter(DirectX9IndexedStorageAdapterCreationParameters parameters)
            : base(parameters)
        {
            _context = parameters.Context;
        }

        public unsafe Texture2D GetLand(int index)
        {
            index &= 0x3FFF;

            int length, extra;
            Stream stream = FileIndex.Seek(index, out length, out extra);

            Texture2D texture = new Texture2D(_context, 44, 44, 0, Usage.None, Format.A1R5G5B5, Pool.Managed);
            DataRectangle rect = texture.LockRectangle(0, LockFlags.None);
            BinaryReader bin = new BinaryReader(stream);

            int xOffset = 21;
            int xRun = 2;

            ushort* line = (ushort*)rect.DataPointer;
            int delta = rect.Pitch >> 1;

            for (int y = 0; y < 22; ++y, --xOffset, xRun += 2, line += delta)
            {
                ushort* cur = line + xOffset;
                ushort* end = cur + xRun;

                while (cur < end)
                    *cur++ = (ushort)(bin.ReadUInt16() | 0x8000);
            }

            xOffset = 0;
            xRun = 44;

            for (int y = 0; y < 22; ++y, ++xOffset, xRun -= 2, line += delta)
            {
                ushort* cur = line + xOffset;
                ushort* end = cur + xRun;

                while (cur < end)
                    *cur++ = (ushort)(bin.ReadUInt16() | 0x8000);
            }

            texture.UnlockRectangle(0);

            return texture;
        }

        public unsafe Texture2D GetStatic(int index)
        {
            if (index + 0x4000 > int.MaxValue)
            {
                throw new ArithmeticException("The index must not excede (int.MaxValue - 0x4000)");
            }

            index &= 0x3FFF;
            index += 0x4000;

            int length, extra;
            Stream stream = FileIndex.Seek(index, out length, out extra);

            if (stream == null)
                return null;

            BinaryReader bin = new BinaryReader(stream);

            bin.ReadInt32(); // Unknown
            int width = bin.ReadInt16();
            int height = bin.ReadInt16();

            if (width <= 0 || height <= 0)
                return null;

            int[] lookups = new int[height];

            int start = (int)bin.BaseStream.Position + (height * 2);

            for (int i = 0; i < height; ++i)
                lookups[i] = (start + (bin.ReadUInt16() * 2));

            Texture2D texture = new Texture2D(_context, width, height, 0, Usage.None, Format.A1R5G5B5, Pool.Managed);
            DataRectangle rect = texture.LockRectangle(0, LockFlags.None);

            ushort* line = (ushort*)rect.DataPointer;
            int delta = rect.Pitch >> 1;

            for (int y = 0; y < height; ++y, line += delta)
            {
                bin.BaseStream.Seek(lookups[y], SeekOrigin.Begin);

                ushort* cur = line;

                int xOffset, xRun;

                while (((xOffset = bin.ReadUInt16()) + (xRun = bin.ReadUInt16())) != 0)
                {
                    cur += xOffset;
                    ushort* end = cur + xRun;

                    while (cur < end)
                        *cur++ = (ushort)(bin.ReadUInt16() ^ 0x8000);
                }
            }

            texture.UnlockRectangle(0);

            return texture;
        }
    }
}
