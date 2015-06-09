#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   TexmapImageSourceAdapter.cs
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
    internal class TexmapImageSourceAdapter : StorageAdapterBase, ITexmapStorageAdapter<ImageSource>
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

            _fileIndex = install.CreateFileIndex("texidx.mul", "texmaps.mul");
        }

        public unsafe ImageSource GetTexmap(int index)
        {
            int length, extra;
            using(var stream = _fileIndex.Seek(index, out length, out extra))
            {
                if(stream == null)
                {
                    return null;
                }

                var size = extra == 0 ? 64 : 128;

                using(var bin = new BinaryReader(stream))
                {
                    var bmp = new WriteableBitmap(size, size, 96, 96, PixelFormats.Bgr555, null);

                    bmp.Lock();

                    var line = (ushort*)bmp.BackBuffer;
                    var delta = bmp.BackBufferStride >> 1;

                    for(var y = 0; y < size; ++y, line += delta)
                    {
                        var cur = line;
                        var end = cur + size;

                        while(cur < end)
                        {
                            *cur++ = (ushort)(bin.ReadUInt16() ^ 0x8000);
                        }
                    }

                    bmp.AddDirtyRect(new Int32Rect(0, 0, size, size));
                    bmp.Unlock();

                    return bmp;
                }
            }
        }

        public Task<ImageSource> GetTexmapAsync(int index)
        {
            return Task.FromResult(GetTexmap(index));
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