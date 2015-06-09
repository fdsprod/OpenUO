#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   ArtworkImageSourceAdapter.cs
//  *
//  *   This program is free software; you can redistribute it and/or modify
//  *   it under the terms of the GNU General Public License as published by
//  *   the Free Software Foundation; either version 3 of the License, or
//  *   (at your option) any later version.
//  ***************************************************************************/

#endregion

#region Usings

using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenUO.Ultima.Adapters;

#endregion

namespace OpenUO.Ultima.PresentationOpenUO.Core.Adapters
{
    internal class ArtworkImageSourceAdapter : StorageAdapterBase, IArtworkStorageAdapter<ImageSource>
    {
        private FileIndexBase _fileIndex;

        public override int Length
        {
            get
            {
                if(!IsInitialized)
                {
                    Initialize();
                }

                return _fileIndex.Length;
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            var install = Install;

            _fileIndex =
                install.IsUopFormat
                    ? install.CreateFileIndex("artLegacyMUL.uop", 0x10000, false, ".tga")
                    : install.CreateFileIndex("artidx.mul", "art.mul");
        }

        public unsafe ImageSource GetLand(int index)
        {
            index &= 0x3FFF;

            int length, extra;
            using(var stream = _fileIndex.Seek(index, out length, out extra))
            {
                using(var bin = new BinaryReader(stream))
                {
                    var bmp = new WriteableBitmap(44, 44, 96, 96, PixelFormats.Bgr555, null);

                    bmp.Lock();

                    var xOffset = 21;
                    var xRun = 2;

                    var line = (ushort*)bmp.BackBuffer;
                    var delta = bmp.BackBufferStride >> 1;

                    for(var y = 0; y < 22; ++y, --xOffset, xRun += 2, line += delta)
                    {
                        var cur = line + xOffset;
                        var end = cur + xRun;

                        while(cur < end)
                        {
                            *cur++ = (ushort)(bin.ReadUInt16() | 0x8000);
                        }
                    }

                    xOffset = 0;
                    xRun = 44;

                    for(var y = 0; y < 22; ++y, ++xOffset, xRun -= 2, line += delta)
                    {
                        var cur = line + xOffset;
                        var end = cur + xRun;

                        while(cur < end)
                        {
                            *cur++ = (ushort)(bin.ReadUInt16() | 0x8000);
                        }
                    }

                    bmp.AddDirtyRect(new Int32Rect(0, 0, 44, 44));
                    bmp.Unlock();

                    return bmp;
                }
            }
        }

        public Task<ImageSource> GetLandAsync(int index)
        {
            return Task.FromResult(GetLand(index));
        }

        public unsafe ImageSource GetStatic(int index)
        {
            index += 0x4000;
            index &= 0xFFFF;

            int length, extra;
            using(var stream = _fileIndex.Seek(index, out length, out extra))
            {
                using(var bin = new BinaryReader(stream))
                {
                    bin.ReadInt32(); // Unknown

                    int width = bin.ReadInt16();
                    int height = bin.ReadInt16();

                    if(width <= 0 || height <= 0)
                    {
                        return null;
                    }

                    var lookups = new int[height];

                    var start = (int)bin.BaseStream.Position + (height * 2);

                    for(var i = 0; i < height; ++i)
                    {
                        lookups[i] = (start + (bin.ReadUInt16() * 2));
                    }

                    var bmp = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr555, null);

                    bmp.Lock();

                    var line = (ushort*)bmp.BackBuffer;
                    var delta = bmp.BackBufferStride >> 1;

                    for(var y = 0; y < height; ++y, line += delta)
                    {
                        bin.BaseStream.Seek(lookups[y], SeekOrigin.Begin);

                        var cur = line;
                        ushort* end;

                        int xOffset, xRun;

                        while(((xOffset = bin.ReadUInt16()) + (xRun = bin.ReadUInt16())) != 0)
                        {
                            cur += xOffset;
                            end = cur + xRun;

                            while(cur < end)
                            {
                                *cur++ = (ushort)(bin.ReadUInt16() ^ 0x8000);
                            }
                        }
                    }

                    bmp.AddDirtyRect(new Int32Rect(0, 0, width, height));
                    bmp.Unlock();

                    return bmp;
                }
            }
        }

        public Task<ImageSource> GetStaticAsync(int index)
        {
            return Task.FromResult(GetStatic(index));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _fileIndex.Close();
        }
    }
}