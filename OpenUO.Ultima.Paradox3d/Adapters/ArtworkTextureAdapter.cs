#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// ArtworkTextureAdapter.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region Usings

using System.IO;
using System.Threading.Tasks;

using OpenUO.Core.Patterns;
using OpenUO.Ultima.Adapters;

using SiliconStudio.Paradox.Graphics;

#endregion

namespace OpenUO.Ultima.Paradox3d.Adapters
{
    internal class ArtworkTextureAdapter : Paradox3dStorageAdapterBase, IArtworkStorageAdapter<Texture>
    {
        private FileIndexBase _fileIndex;

        public ArtworkTextureAdapter(IContainer container)
            : base(container)
        {
        }

        public override int Length
        {
            get
            {
                if (!IsInitialized)
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

        public unsafe Texture GetLand(int index)
        {
            index &= 0x3FFF;

            int length, extra;

            using (var stream = _fileIndex.Seek(index, out length, out extra))
            {
                using (var bin = new BinaryReader(stream))
                {
                    var texture = Texture.New2D(GraphicsDevice, 44, 44, PixelFormat.B5G5R5A1_UNorm);
                    var buffer = new ushort[44*44];

                    var xOffset = 21;
                    var xRun = 2;

                    fixed (ushort* start = buffer)
                    {
                        var ptr = start;
                        var delta = texture.Width;

                        for (var y = 0; y < 22; ++y, --xOffset, xRun += 2, ptr += delta)
                        {
                            var cur = ptr + xOffset;
                            var end = cur + xRun;

                            while (cur < end)
                            {
                                *cur++ = (ushort) (bin.ReadUInt16() | 0x8000);
                            }
                        }

                        xOffset = 0;
                        xRun = 44;

                        for (var y = 0; y < 22; ++y, ++xOffset, xRun -= 2, ptr += delta)
                        {
                            var cur = ptr + xOffset;
                            var end = cur + xRun;

                            while (cur < end)
                            {
                                *cur++ = (ushort) (bin.ReadUInt16() | 0x8000);
                            }
                        }
                    }

                    texture.SetData(buffer);

                    return texture;
                }
            }
        }

        public Task<Texture> GetLandAsync(int index)
        {
            return Task.FromResult(GetLand(index));
        }

        public unsafe Texture GetStatic(int index)
        {
            index += 0x4000;
            index &= 0xFFFF;

            int length, extra;
            using (var stream = _fileIndex.Seek(index, out length, out extra))
            {
                using (var bin = new BinaryReader(stream))
                {
                    bin.ReadInt32(); // Unknown

                    int width = bin.ReadInt16();
                    int height = bin.ReadInt16();

                    if (width <= 0 || height <= 0)
                    {
                        return null;
                    }

                    var lookups = new int[height];
                    var lookupStart = (int) bin.BaseStream.Position + (height*2);

                    for (var i = 0; i < height; ++i)
                    {
                        lookups[i] = (lookupStart + (bin.ReadUInt16()*2));
                    }

                    var texture = Texture.New2D(GraphicsDevice, width, height, PixelFormat.B5G5R5A1_UNorm);
                    var buffer = new ushort[width*height];

                    fixed (ushort* start = buffer)
                    {
                        var ptr = start;
                        var delta = texture.Width;

                        for (var y = 0; y < height; ++y, ptr += delta)
                        {
                            bin.BaseStream.Seek(lookups[y], SeekOrigin.Begin);

                            var cur = ptr;

                            int xOffset, xRun;

                            while (((xOffset = bin.ReadUInt16()) + (xRun = bin.ReadUInt16())) != 0)
                            {
                                cur += xOffset;
                                var end = cur + xRun;

                                while (cur < end)
                                {
                                    *cur++ = (ushort) (bin.ReadUInt16() ^ 0x8000);
                                }
                            }
                        }
                    }

                    texture.SetData(buffer);

                    return texture;
                }
            }
        }

        public Task<Texture> GetStaticAsync(int index)
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