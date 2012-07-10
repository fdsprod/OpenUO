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


namespace OpenUO.Ultima
{
    public class ASCIIFont
    {
        private int _height;
        private ASCIIChar[] _chars;

        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        public ASCIIChar[] Chars
        {
            get { return _chars; }
            set { _chars = value; }
        }

        public ASCIIFont(int height, ASCIIChar[] chars)
        {
            _height = height;
            _chars = chars;
        }

        public int GetWidth(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            int width = 0;

            for (int i = 0; i < text.Length; ++i)
            {
                int c = ((((text[i]) - 0x20) & 0x7FFFFFFF) % 224);

                if (c >= _chars.Length)
                    continue;

                width += _chars[c].Width;
            }

            return width;
        }
    }
}
