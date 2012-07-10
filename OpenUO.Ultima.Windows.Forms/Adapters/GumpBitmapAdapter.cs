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
    internal class GumpBitmapAdapter : StorageAdapterBase, IGumpStorageAdapter<Bitmap>
    {
        private FileIndex _fileIndex;

        public override void Initialize()
        {
            base.Initialize();

            var install = Install;

            _fileIndex =
                install.IsUOPFormat
                    ? install.CreateFileIndex("gumpartLegacyMUL.uop")
                    : install.CreateFileIndex("gumpidx.mul", "gumpart.mul");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (_fileIndex != null)
            {
                _fileIndex.Close();
                _fileIndex = null;
            }
        }

        public unsafe Bitmap GetGump(int index)
        {
            int length, extra;
            Stream stream = _fileIndex.Seek(index, out length, out extra);

            if (stream == null)
                return null;

            BinaryReader bin = new BinaryReader(stream);

            if (_fileIndex.IsUopFormat)
            {
                bin.ReadInt32(); // Unknown
                bin.ReadInt32(); // Unknown
                bin.ReadInt32(); // Unknown

                extra = (bin.ReadInt32() << 16) | bin.ReadInt32();
            }

            int width = (extra >> 16) & 0xFFFF;
            int height = extra & 0xFFFF;

            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format16bppArgb1555);
            BitmapData bd = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format16bppArgb1555);

            int[] lookups = new int[height];
            int start = (int)bin.BaseStream.Position;

            for (int i = 0; i < height; ++i)
                lookups[i] = start + (bin.ReadInt32() * 4);

            ushort* line = (ushort*)bd.Scan0;
            int delta = bd.Stride >> 1;

            for (int y = 0; y < height; ++y, line += delta)
            {
                bin.BaseStream.Seek(lookups[y], SeekOrigin.Begin);

                ushort* cur = line;
                ushort* end = line + bd.Width;

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

            bmp.UnlockBits(bd);

            return bmp;
        }
    }
}
