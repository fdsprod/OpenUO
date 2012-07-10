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
    public sealed class UnicodeChar
    {
        public byte[] Bytes { get; set; }
        public sbyte XOffset { get; set; }
        public sbyte YOffset { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

        public UnicodeChar()
        {

        }
    }
}
