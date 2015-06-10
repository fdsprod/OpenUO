using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using OpenUO.Core.Patterns;
using OpenUO.Ultima.Adapters;

using SiliconStudio.Paradox.Graphics;

namespace OpenUO.Ultima.Paradox3d.Adapters
{
    internal class UnicodeFontTextureAdapter : Paradox3dStorageAdapterBase, IUnicodeFontStorageAdapter<Texture>
    {
        private const string FILE_NAME_FORMAT = "unifont{0}.mul";
        private UnicodeFont[] _fonts;

        public UnicodeFontTextureAdapter(IContainer container) : base(container)
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

                return _fonts.Length;
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            var install = Install;

            var fonts = new List<UnicodeFont>();

            var i = 0;
            var path = install.GetPath(FILE_NAME_FORMAT, string.Empty);

            while (File.Exists(path))
            {
                var font = new UnicodeFont();

                var maxHeight = 0;

                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var length = (int)fs.Length;
                    var buffer = new byte[length];

                    fs.Read(buffer, 0, buffer.Length);

                    using (var stream = new MemoryStream(buffer))
                    {
                        using (var bin = new BinaryReader(stream))
                        {
                            for (var c = 0; c < 0x10000; ++c)
                            {
                                font.Chars[c] = new UnicodeChar();
                                stream.Seek(((c) * 4), SeekOrigin.Begin);

                                var index = bin.ReadInt32();

                                if ((index >= fs.Length) || (index <= 0))
                                {
                                    continue;
                                }

                                stream.Seek(index, SeekOrigin.Begin);

                                var xOffset = bin.ReadSByte();
                                var yOffset = bin.ReadSByte();

                                int width = bin.ReadByte();
                                int height = bin.ReadByte();

                                maxHeight = Math.Max(height, maxHeight);

                                font.Chars[c].XOffset = xOffset;
                                font.Chars[c].YOffset = yOffset;
                                font.Chars[c].Width = width;
                                font.Chars[c].Height = height;

                                if (!((width == 0) || (height == 0)))
                                {
                                    font.Chars[c].Bytes = bin.ReadBytes(height * (((width - 1) / 8) + 1));
                                }
                            }
                        }
                    }
                }

                font.Height = maxHeight;
                fonts.Add(font);
                path = install.GetPath(FILE_NAME_FORMAT, ++i);
            }

            _fonts = fonts.ToArray();
        }

        public unsafe Texture GetText(int fontId, string text, short hueId)
        {
            var font = _fonts[fontId];

            var width = font.GetWidth(text);
            var height = font.GetHeight(text);

            var texture = Texture.New2D(GraphicsDevice, width, height, PixelFormat.B5G5R5A1_UNorm);
            var buffer = new ushort[width * height];
            
            fixed (ushort* start = buffer)
            {
                var ptr = start;
                var delta = texture.Width;
                var dx = 2;

                for (var i = 0; i < text.Length; ++i)
                {
                    var c = text[i]%0x10000;
                    var ch = font.Chars[c];

                    var charWidth = ch.Width;
                    var charHeight = ch.Height + ch.YOffset;

                    var data = ch.Bytes;

                    if (c == 32)
                    {
                        dx += 5;
                        continue;
                    }

                    dx += ch.XOffset;

                    for (var dy = 0; dy < charHeight; ++dy)
                    {
                        var dest = (ptr + (delta*(dy + (height - charHeight)))) + (dx);

                        for (var k = 0; k < charWidth; ++k)
                        {
                            var offset = k/8 + dy*((charWidth + 7)/8);

                            if (offset > data.Length)
                            {
                                continue;
                            }

                            if ((data[offset] & (1 << (7 - (k%8)))) != 0)
                            {
                                *dest++ = 0x8000;
                            }
                            else
                            {
                                *dest++ = 0x0000;
                            }
                        }
                    }

                    dx += ch.Width;
                }
            }

            texture.SetData(buffer);

            return texture;
        }

        public Task<Texture> GetTextAsync(int fontId, string text, short hueId)
        {
            return Task.FromResult(GetText(fontId, text, hueId));
        }

        public int GetFontHeight(int fontId)
        {
            return _fonts[fontId].Height;
        }

        public Task<int> GetFontHeightAsync(int fontId)
        {
            return Task.FromResult(GetFontHeight(fontId));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _fonts = null;
        }
    }
}