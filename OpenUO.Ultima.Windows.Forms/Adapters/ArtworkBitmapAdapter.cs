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

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using OpenUO.Ultima.Adapters;

namespace OpenUO.Ultima.Windows.Forms.Adapters
{
    internal class ArtworkBitmapAdapter : StorageAdapterBase, IArtworkStorageAdapter<Bitmap>
    {
        private FileIndex _fileIndex;
        
        public override void Initialize()
        {
            base.Initialize();

            var install = Install;

            _fileIndex =
                install.IsUOPFormat
                    ? install.CreateFileIndex("artLegacyMUL.uop")
                    : install.CreateFileIndex("artidx.mul", "art.mul");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _fileIndex.Close();
        }

        public unsafe Bitmap GetLand(int index)
        {
            index &= 0x3FFF;

            int length, extra;
            Stream stream = _fileIndex.Seek(index, out length, out extra);

            if(stream == null)
                return null;

            Bitmap bmp = new Bitmap(44, 44, PixelFormat.Format16bppArgb1555);
            BitmapData bd = bmp.LockBits(new Rectangle(0, 0, 44, 44), ImageLockMode.WriteOnly, PixelFormat.Format16bppArgb1555);
            BinaryReader bin = new BinaryReader(stream);

            if (_fileIndex.IsUopFormat)
            {
                bin.ReadInt32(); // Unknown
                bin.ReadInt32(); // Unknown
                bin.ReadInt32(); // Unknown
            }

            int xOffset = 21;
            int xRun = 2;

            ushort* line = (ushort*)bd.Scan0;
            int delta = bd.Stride >> 1;

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

            bmp.UnlockBits(bd);

            return bmp;
        }

        public unsafe Bitmap GetStatic(int index)
        {
            if(_fileIndex.IsUopFormat)
                index += 2855;
            else
                index += 0x4000;

            index &= 0xFFFF;

            int length, extra;
            Stream stream = _fileIndex.Seek(index, out length, out extra);
            BinaryReader bin = new BinaryReader(stream);

            if (_fileIndex.IsUopFormat)
            {
                bin.ReadInt32(); // Unknown
                bin.ReadInt32(); // Unknown
                bin.ReadInt32(); // Unknown
            }

            bin.ReadInt32(); // Unknown

            int width = bin.ReadInt16();
            int height = bin.ReadInt16();

            if (width <= 0 || height <= 0)
                return null;

            int[] lookups = new int[height];

            int start = (int)bin.BaseStream.Position + (height * 2);

            for (int i = 0; i < height; ++i)
                lookups[i] = (int)(start + (bin.ReadUInt16() * 2));

            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format16bppArgb1555);
            BitmapData bd = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format16bppArgb1555);

            ushort* line = (ushort*)bd.Scan0;
            int delta = bd.Stride >> 1;

            for (int y = 0; y < height; ++y, line += delta)
            {
                bin.BaseStream.Seek(lookups[y], SeekOrigin.Begin);

                ushort* cur = line;
                ushort* end;

                int xOffset, xRun;

                while (((xOffset = bin.ReadUInt16()) + (xRun = bin.ReadUInt16())) != 0)
                {
                    cur += xOffset;
                    end = cur + xRun;

                    while (cur < end)
                        *cur++ = (ushort)(bin.ReadUInt16() ^ 0x8000);
                }
            }

            bmp.UnlockBits(bd);

            return bmp;
        }
    }
}
