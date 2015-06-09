#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   ASCIIFontBitmapAdapter.cs
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
    internal class ASCIIFontBitmapAdapter : StorageAdapterBase, IASCIIFontStorageAdapter<Bitmap>
    {
        private ASCIIFont[] _fonts;

        public override int Length
        {
            get
            {
                if(!IsInitialized)
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

            _fonts = new ASCIIFont[10];

            using(var reader = new BinaryReader(File.Open(install.GetPath("fonts.mul"), FileMode.Open)))
            {
                for(var i = 0; i < 10; ++i)
                {
                    reader.ReadByte(); //header

                    var chars = new ASCIIChar[224];
                    var fontHeight = 0;

                    for(var k = 0; k < 224; ++k)
                    {
                        var width = reader.ReadByte();
                        var height = reader.ReadByte();

                        reader.ReadByte(); // delimeter?

                        if(width > 0 && height > 0)
                        {
                            if(height > fontHeight && k < 96)
                            {
                                fontHeight = height;
                            }

                            var pixels = new ushort[width * height];

                            for(var y = 0; y < height; ++y)
                            {
                                for(var x = 0; x < width; ++x)
                                {
                                    pixels[y * width + x] = (ushort)(reader.ReadByte() | (reader.ReadByte() << 8));
                                }
                            }

                            chars[k] = new ASCIIChar
                                       {
                                           Pixels = pixels,
                                           Width = width,
                                           Height = height
                                       };
                        }
                    }

                    _fonts[i] = new ASCIIFont(fontHeight, chars);
                }
            }
        }

        public unsafe Bitmap GetText(int fontId, string text, short hueId)
        {
            var font = _fonts[fontId];

            var width = font.GetWidth(text);
            var height = font.Height;

            var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            var bd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            var line = (int*)bd.Scan0;
            var delta = bd.Stride >> 2;

            var dx = 0;

            for(var i = 0; i < text.Length; ++i)
            {
                var c = ((((text[i]) - 0x20) & 0x7FFFFFFF) % 224);
                var ch = font.Chars[c];

                var charWidth = ch.Width;
                var charHeight = ch.Height;

                var pixels = ch.Pixels;

                for(var dy = 0; dy < charHeight; ++dy)
                {
                    var dest = (line + (delta * (dy + (font.Height - charHeight)))) + (dx);

                    for(var k = 0; k < charWidth; ++k)
                    {
                        var pixel = pixels[charWidth * dy + k];
                        *dest++ = Color.FromArgb(pixel == 0 ? 0 : 255, (pixel & 0x7C00) >> 7, (pixel & 0x3E0) >> 2, (pixel & 0x1F) << 3).ToArgb();
                    }
                }

                dx += charWidth;
            }

            bmp.UnlockBits(bd);

            return bmp;
        }

        public Task<Bitmap> GetTextAsync(int fontId, string text, short hueId)
        {
            return Task.FromResult(GetText(fontId, text, hueId));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _fonts = null;
        }
    }
}