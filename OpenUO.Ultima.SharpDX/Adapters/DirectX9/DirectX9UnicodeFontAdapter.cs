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
using System.IO;
using OpenUO.Ultima.Adapters;
using SharpDX;
using SharpDX.Direct3D9;

namespace OpenUO.Ultima.SharpDX.Adapters.DirectX9
{
    public sealed class DirectX9UnicodeFontAdapter : StorageAdapterBase, IUnicodeFontStorageAdapter<Texture>
    {
        private const string FILE_NAME_FORMAT = "unifont{0}.mul";

        private UnicodeFont[] _fonts;
        private Device _device;

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

        public unsafe Texture GetText(int fontId, string text, short hueId)
        {
            UnicodeFont font = _fonts[fontId];

            int width = font.GetWidth(text);
            int height = font.GetHeight(text);

            Texture texture = new Texture(_device, width, height, 0, Usage.None, Format.A1R5G5B5, Pool.Managed);
            DataRectangle rect = texture.LockRectangle(0, LockFlags.None);

            ushort* line = (ushort*)rect.DataPointer;
            int delta = rect.Pitch >> 1;

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

            texture.UnlockRectangle(0);

            return texture;
        }

        public int GetFontHeight(int fontId)
        {
            return _fonts[fontId].Height;
        }
    }
}
