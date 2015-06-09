#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   ArtworkBitmapAdapter.cs
//  *
//  *   This program is free software; you can redistribute it and/or modify
//  *   it under the terms of the GNU General Public License as published by
//  *   the Free Software Foundation; either version 3 of the License, or
//  *   (at your option) any later version.
//  ***************************************************************************/

#endregion

#region Usings

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

using OpenUO.Ultima.Adapters;

#endregion

namespace OpenUO.Ultima.Windows.Forms.Adapters
{
    internal class ArtworkBitmapAdapter : StorageAdapterBase, IArtworkStorageAdapter<Bitmap>
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

        public unsafe Bitmap GetLand(int index)
        {
            index &= 0x3FFF;

            int length, extra;
            using(var stream = _fileIndex.Seek(index, out length, out extra))
            {
                if(stream == null)
                {
                    return null;
                }

                using(var bmp = new Bitmap(44, 44, PixelFormat.Format16bppArgb1555))
                {
                    var bd = bmp.LockBits(new Rectangle(0, 0, 44, 44), ImageLockMode.WriteOnly, PixelFormat.Format16bppArgb1555);
                    var bin = new BinaryReader(stream);

                    var xOffset = 21;
                    var xRun = 2;

                    var line = (ushort*)bd.Scan0;
                    var delta = bd.Stride >> 1;

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

                    bmp.UnlockBits(bd);

                    return bmp;
                }
            }
        }

        public Task<Bitmap> GetLandAsync(int index)
        {
            return Task.FromResult(GetLand(index));
        }

        public unsafe Bitmap GetStatic(int index)
        {
            index += 0x4000;

            int length, extra;
            using(var stream = _fileIndex.Seek(index, out length, out extra))
            {
                if(stream == null)
                {
                    return null;
                }

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

                    var bmp = new Bitmap(width, height, PixelFormat.Format16bppArgb1555);
                    var bd = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format16bppArgb1555);

                    var line = (ushort*)bd.Scan0;
                    var delta = bd.Stride >> 1;

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

                    bmp.UnlockBits(bd);

                    return bmp;
                }
            }
        }

        public Task<Bitmap> GetStaticAsync(int index)
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