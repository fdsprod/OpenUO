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
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenUO.Ultima.Adapters;

namespace OpenUO.Ultima.PresentationFramework.Adapters
{
    internal class ArtworkImageSourceAdapter : StorageAdapterBase, IArtworkStorageAdapter<ImageSource>
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

        public unsafe ImageSource GetLand(int index)
        {
            index &= 0x3FFF;

            int length, extra;
            Stream stream = _fileIndex.Seek(index, out length, out extra);

            BinaryReader bin = new BinaryReader(stream);
            WriteableBitmap bmp = new WriteableBitmap(44, 44, 96, 96, PixelFormats.Bgr555, null); 
            bmp.Lock();

            if (_fileIndex.IsUopFormat)
            {
                bin.ReadInt32(); // Unknown
                bin.ReadInt32(); // Unknown
                bin.ReadInt32(); // Unknown
            }

            int xOffset = 21;
            int xRun = 2;

            ushort* line = (ushort*)bmp.BackBuffer;
            int delta = bmp.BackBufferStride >> 1;

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

            bmp.AddDirtyRect(new Int32Rect(0, 0, 44, 44));
            bmp.Unlock();

            return bmp;
        }

        public unsafe ImageSource GetStatic(int index)
        {
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

            WriteableBitmap bmp = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr555, null);
            bmp.Lock();

            ushort* line = (ushort*)bmp.BackBuffer;
            int delta = bmp.BackBufferStride >> 1;

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

            bmp.AddDirtyRect(new Int32Rect(0, 0, width, height));
            bmp.Unlock();

            return bmp;
        }
    }
}
