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
    internal class TexmapBitmapAdapter : StorageAdapterBase, ITexmapStorageAdapter<Bitmap>
    {
        private FileIndex _fileIndex;

        public override void Initialize()
        {
            base.Initialize();

            var install = Install;

            _fileIndex = install.CreateFileIndex("texidx.mul", "texmaps.mul");
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

        public unsafe Bitmap GetTexmap(int index)
        {
            int length, extra;
            Stream stream = _fileIndex.Seek(index, out length, out extra);

            if (stream == null)
                return null;

            int size = extra == 0 ? 64 : 128;

            Bitmap bmp = new Bitmap(size, size, PixelFormat.Format16bppArgb1555);
            BitmapData bd = bmp.LockBits(new Rectangle(0, 0, size, size), ImageLockMode.WriteOnly, PixelFormat.Format16bppArgb1555);
            BinaryReader bin = new BinaryReader(stream);

            ushort* line = (ushort*)bd.Scan0;
            int delta = bd.Stride >> 1;

            for (int y = 0; y < size; ++y, line += delta)
            {
                ushort* cur = line;
                ushort* end = cur + size;

                while (cur < end)
                    *cur++ = (ushort)(bin.ReadUInt16() ^ 0x8000);
            }

            bmp.UnlockBits(bd);

            return bmp;
        }
    }
}
