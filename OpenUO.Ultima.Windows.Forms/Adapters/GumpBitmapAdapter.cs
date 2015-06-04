#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   GumpBitmapAdapter.cs
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
using OpenUO.Ultima.Adapters;

#endregion

namespace OpenUO.Ultima.Windows.Forms.Adapters
{
    internal class GumpBitmapAdapter : StorageAdapterBase, IGumpStorageAdapter<Bitmap>
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
                    ? install.CreateFileIndex("gumpartLegacyMUL.uop", 0xFFFF, true, ".tga")
                    : install.CreateFileIndex("gumpidx.mul", "gumpart.mul");
        }

        public unsafe Bitmap GetGump(int index)
        {
            int length, extra;

            using(var stream = _fileIndex.Seek(index, out length, out extra))
            {
                if(stream == null)
                {
                    return null;
                }

                using(var bin = new BinaryReader(stream))
                {
                    var width = (extra >> 16) & 0xFFFF;
                    var height = extra & 0xFFFF;

                    var bmp = new Bitmap(width, height, PixelFormat.Format16bppArgb1555);
                    var bd = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format16bppArgb1555);

                    var lookups = new int[height];
                    var start = (int)bin.BaseStream.Position;

                    for(var i = 0; i < height; ++i)
                    {
                        lookups[i] = start + (bin.ReadInt32() * 4);
                    }

                    var line = (ushort*)bd.Scan0;
                    var delta = bd.Stride >> 1;

                    for(var y = 0; y < height; ++y, line += delta)
                    {
                        bin.BaseStream.Seek(lookups[y], SeekOrigin.Begin);

                        var cur = line;
                        var end = line + bd.Width;

                        while(cur < end)
                        {
                            var color = bin.ReadUInt16();
                            var next = cur + bin.ReadUInt16();

                            if(color == 0)
                            {
                                cur = next;
                            }
                            else
                            {
                                color ^= 0x8000;

                                while(cur < next)
                                {
                                    *cur++ = color;
                                }
                            }
                        }
                    }

                    bmp.UnlockBits(bd);

                    return bmp;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if(_fileIndex != null)
            {
                _fileIndex.Close();
                _fileIndex = null;
            }
        }
    }
}