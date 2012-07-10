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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenUO.Ultima.Adapters;

namespace OpenUO.Ultima.Windows.Forms.Adapters
{
    internal class UnicodeFontBitmapAdapter : StorageAdapterBase, IUnicodeFontStorageAdapter<Bitmap>
    {
        private const string FILE_NAME_FORMAT = "unifont{0}.mul";

        private UnicodeFont[] _fonts;
                
        public override void Initialize()
        {
            base.Initialize();

            var install = Install;

            List<UnicodeFont> fonts = new List<UnicodeFont>();

            int i = 0;
            string path = install.GetPath(FILE_NAME_FORMAT, string.Empty);

            while (File.Exists(path))
            {
                var font = new UnicodeFont();

                int maxHeight = 0;

                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    int length = (int)fs.Length;
                    byte[] buffer = new byte[length];

                    int read = fs.Read(buffer, 0, buffer.Length);

                    using (MemoryStream stream = new MemoryStream(buffer))
                    {
                        using (BinaryReader bin = new BinaryReader(stream))
                        {
                            for (int c = 0; c < 0x10000; ++c)
                            {
                                font.Chars[c] = new UnicodeChar();
                                stream.Seek((long)((c) * 4), SeekOrigin.Begin);

                                int index = bin.ReadInt32();

                                if ((index >= fs.Length) || (index <= 0))
                                    continue;

                                stream.Seek((long)index, SeekOrigin.Begin);

                                sbyte xOffset = bin.ReadSByte();
                                sbyte yOffset = bin.ReadSByte();

                                int width = bin.ReadByte();
                                int height = bin.ReadByte();

                                maxHeight = Math.Max(height, maxHeight);

                                font.Chars[c].XOffset = xOffset;
                                font.Chars[c].YOffset = yOffset;
                                font.Chars[c].Width = width;
                                font.Chars[c].Height = height;

                                if (!((width == 0) || (height == 0)))
                                    font.Chars[c].Bytes = bin.ReadBytes(height * (((width - 1) / 8) + 1));
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

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
                           
            _fonts = null;
        }
        
        public unsafe Bitmap GetText(int fontId, string text, short hueId)
        {
            UnicodeFont font = _fonts[fontId];

            int width = font.GetWidth(text);
            int height = font.GetHeight(text);

            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format16bppArgb1555);
            BitmapData bd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format16bppArgb1555);

            ushort* line = (ushort*)bd.Scan0;
            int delta = bd.Stride >> 1;

            int dx = 2;
            
            for (int i = 0; i < text.Length; ++i)
            {
                int c = (int)text[i] % 0x10000;
                UnicodeChar ch = font.Chars[c];

                int charWidth = ch.Width;
                int charHeight = ch.Height;

                byte[] data = ch.Bytes;

                if (c == 32)
                {
                    dx += 5;
                    continue;
                }
                else
                {
                    dx += ch.XOffset;
                }

                for (int dy = 0; dy < charHeight; ++dy)
                {
                    ushort* dest = (line + (delta * (dy + (height - charHeight)))) + (dx);

                    for (int k = 0; k < charWidth; ++k)
                    {
                        int offset = k / 8 + dy * ((charWidth + 7) / 8);

                        if (offset > data.Length)
                            continue;

                        if ((data[offset] & (1 << (7 - (k % 8)))) != 0)
                            *dest++ = 0x8000;
                        else
                            *dest++ = 0x0000;
                    }
                }

                dx += ch.Width;
            }
            
            bmp.UnlockBits(bd);

            return bmp;
        }

        public int GetFontHeight(int fontId)
        {
            return _fonts[fontId].Height;
        }
    }
}
