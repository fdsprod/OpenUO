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

namespace OpenUO.Ultima
{
    public sealed class UnicodeFont
    {
        public UnicodeChar[] Chars
        {
            get;
            set; 
        }

        public int Height 
        {
            get;
            set;
        }

        public UnicodeFont()
        {
            Chars = new UnicodeChar[0x10000];
        }

        public int GetWidth(string text)
        {
            if (text == null || text.Length == 0)
                return 0;

            int width = 0;

            for (int i = 0; i < text.Length; ++i)
            {
                int c = (int)text[i] % 0x10000;

                if (c == 32)
                    width += 5;

                width += Chars[c].Width;
                width += Chars[c].XOffset;
            }

            return width;
        }

        public int GetHeight(string text)
        {
            if (text == null || text.Length == 0)
                return 0;

            int height = 0;

            for (int i = 0; i < text.Length; ++i)
            {
                int c = (int)text[i] % 0x10000;
                height = Math.Max(height, Chars[c].Height + Chars[c].YOffset);
            }

            return height;
        }
    }
}
