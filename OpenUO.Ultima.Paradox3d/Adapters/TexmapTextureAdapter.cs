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

using System.IO;
using OpenUO.Core.Patterns;
using OpenUO.Ultima.Adapters;
using SiliconStudio.Paradox.Graphics;

#endregion

namespace OpenUO.Ultima.Paradox3d.Adapters
{
    internal class TexmapTextureAdapter : StorageAdapterBase, ITexmapStorageAdapter<Texture>
    {
        private readonly GraphicsDevice _graphicsDevice;
        private FileIndexBase _fileIndex;

        public TexmapTextureAdapter(IContainer container)
        {
            _graphicsDevice = container.Resolve<GraphicsDevice>();
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
                    var texture = Texture.New2D(_graphicsDevice, size, size, PixelFormat.B5G5R5A1_UNorm);
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
                                *cur++ = (ushort)(bin.ReadUInt16() ^ 0x8000);
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