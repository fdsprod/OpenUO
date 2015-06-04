#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   ASCIIFont.cs
//  *
//  *   This program is free software; you can redistribute it and/or modify
//  *   it under the terms of the GNU General Public License as published by
//  *   the Free Software Foundation; either version 3 of the License, or
//  *   (at your option) any later version.
//  ***************************************************************************/

#endregion

namespace OpenUO.Ultima
{
    public class ASCIIFont
    {
        public ASCIIFont(int height, ASCIIChar[] chars)
        {
            Height = height;
            Chars = chars;
        }

        public int Height
        {
            get;
            set;
        }

        public ASCIIChar[] Chars
        {
            get;
            set;
        }

        public int GetWidth(string text)
        {
            if(string.IsNullOrEmpty(text))
            {
                return 0;
            }

            var width = 0;

            for(var i = 0; i < text.Length; ++i)
            {
                var c = ((((text[i]) - 0x20) & 0x7FFFFFFF) % 224);

                if(c >= Chars.Length)
                {
                    continue;
                }

                width += Chars[c].Width;
            }

            return width;
        }
    }
}