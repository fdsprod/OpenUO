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

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenUO.Ultima.Adapters;

namespace OpenUO.Ultima.Windows.Forms.Adapters
{
    internal class ASCIIFontBitmapAdapter : StorageAdapterBase, IASCIIFontStorageAdapter<Bitmap>
    {
        private ASCIIFont[] _fonts;

        public override void Initialize()
        {
            base.Initialize();

            var install = Install;

            _fonts = new ASCIIFont[10];

            using (BinaryReader reader = new BinaryReader(File.Open(install.GetPath("fonts.mul"), FileMode.Open)))
            {
                for (int i = 0; i < 10; ++i)
                {
                    reader.ReadByte(); //header

                    ASCIIChar[] chars = new ASCIIChar[224];
                    int fontHeight = 0;

                    for (int k = 0; k < 224; ++k)
                    {
                        byte width = reader.ReadByte();
                        byte height = reader.ReadByte();

                        reader.ReadByte(); // delimeter?

                        if (width > 0 && height > 0)
                        {
                            if (height > fontHeight && k < 96)
                                fontHeight = height;

                            ushort[] pixels = new ushort[width * height];

                            for (int y = 0; y < height; ++y)
                                for (int x = 0; x < width; ++x)
                                    pixels[y * width + x] = (ushort)(reader.ReadByte() | (reader.ReadByte() << 8));

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

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _fonts = null;
        }

        public unsafe Bitmap GetText(int fontId, string text, short hueId)
        {
            ASCIIFont font = _fonts[fontId];

            int width = font.GetWidth(text);
            int height = font.Height;

            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData bd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int* line = (int*)bd.Scan0;
            int delta = bd.Stride >> 2;

            int dx = 0;

            for (int i = 0; i < text.Length; ++i)
            {
                int c = ((((text[i]) - 0x20) & 0x7FFFFFFF) % 224);
                ASCIIChar ch = font.Chars[c];

                int charWidth = ch.Width;
                int charHeight = ch.Height;

                ushort[] pixels = ch.Pixels;

                for (int dy = 0; dy < charHeight; ++dy)
                {
                    int* dest = (line + (delta * (dy + (font.Height - charHeight)))) + (dx);

                    for (int k = 0; k < charWidth; ++k)
                    {
                        ushort pixel = pixels[charWidth * dy + k];
                        *dest++ = Color.FromArgb(pixel == 0 ? 0 : 255, (pixel & 0x7C00) >> 7, (pixel & 0x3E0) >> 2, (pixel & 0x1F) << 3).ToArgb();
                    }
                }

                dx += charWidth;
            }
            
            bmp.UnlockBits(bd);

            return bmp;
        }
    }
}
