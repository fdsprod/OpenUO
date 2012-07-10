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


using Client.Graphics;

using OpenUO.Ultima;
using OpenUO.Ultima.Adapters;
using SharpDX;
using SharpDX.Direct3D9;

namespace Client.Ultima.Adapters
{
    public sealed class DirectX9UnicodeFontAdapter :  StorageAdapterBase, IUnicodeFontStorageAdapter<Texture2D>
    {
        private UnicodeFont[] _fonts;
        private DeviceContext _context;

        public DirectX9UnicodeFontAdapter(DirectX9UnicodeFontStorageAdapterCreationParameter parameters)
        {
            _fonts = parameters.Fonts;
            _context = parameters.Context;
        }

        public unsafe Texture2D GetText(int fontId, string text, short hueId)
        {
            UnicodeFont font = _fonts[fontId];

            int width = font.GetWidth(text);
            int height = font.GetHeight(text);

            Texture2D texture = new Texture2D(_context, width, height, 0, Usage.None, Format.A1R5G5B5, Pool.Managed);
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

        protected override OpenUO.Ultima.Transactions.ITransaction CreateWriteTransactionOverride()
        {
            throw new System.NotImplementedException();
        }
    }
}
