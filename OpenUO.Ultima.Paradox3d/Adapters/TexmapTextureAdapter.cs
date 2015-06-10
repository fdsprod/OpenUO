#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   UltimaSDKBitmapModule.cs
//  *
//  *   This program is free software; you can redistribute it and/or modify
//  *   it under the terms of the GNU General Public License as published by
//  *   the Free Software Foundation; either version 3 of the License, or
//  *   (at your option) any later version.
//  ***************************************************************************/

#endregion

#region Usings

using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using OpenUO.Core.Patterns;
using OpenUO.Ultima.Adapters;
using SiliconStudio.Paradox.Graphics;

#endregion

namespace OpenUO.Ultima.Paradox3d.Adapters
{
    internal class TexmapTextureAdapter : Paradox3dStorageAdapterBase, ITexmapStorageAdapter<Texture>
    {
        private FileIndexBase _fileIndex;

        public TexmapTextureAdapter(IContainer container)
            : base(container)
        {
        }

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

        public unsafe Texture GetTexmap(int index)
        {
            int extra;
            int length;

            using(var stream = _fileIndex.Seek(index, out length, out extra))
            {
                if(stream == null)
                {
                    return null;
                }

                var size = extra == 0 ? 64 : 128;

                using(var bin = new BinaryReader(stream))
                {
                    var texture = Texture.New2D(GraphicsDevice, size, size, PixelFormat.B5G5R5A1_UNorm);
                    var buffer = new ushort[size * size];

                    fixed(ushort* start = buffer)
                    {
                        var ptr = start;
                        var delta = texture.Width;

                        for(var y = 0; y < size; ++y, ptr += delta)
                        {
                            var cur = ptr;
                            var end = cur + size;

                            while(cur < end)
                            {
                                *cur++ = (bin.ReadUInt16());
                            }
                        }
                    }

                    texture.SetData(buffer);

                    return texture;
                }
            }
        }

        public async Task<Texture> GetTexmapAsync(int index)
        {
            using (var result = await _fileIndex.SeekAsync(index).ConfigureAwait(false))
            {
                var stream = result.Stream;

                if (stream == null)
                {
                    return null;
                }

                var size = result.Extra == 0 ? 64 : 128;

                using (var bin = new BinaryReader(stream))
                {
                    var texture = Texture.New2D(GraphicsDevice, size, size, PixelFormat.B5G5R5A1_UNorm);
                    var buffer = new ushort[size * size];

                    unsafe
                    {
                        fixed (ushort* start = buffer)
                        {
                            var ptr = start;
                            var delta = texture.Width;

                            for (var y = 0; y < size; ++y, ptr += delta)
                            {
                                var cur = ptr;
                                var end = cur + size;

                                while (cur < end)
                                {
                                    *cur++ = (ushort)(bin.ReadUInt16() ^ 0x8000);
                                }
                            }
                        }
                    }

                    texture.SetData(buffer);

                    return texture;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if(_fileIndex == null)
            {
                return;
            }

            _fileIndex.Close();
            _fileIndex = null;
        }
    }
}