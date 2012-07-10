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

using System.IO;
using System.Text;

namespace OpenUO.Ultima
{
    public class Hue
    {
        private readonly int _index;
        private readonly ushort[] _colors;
        private readonly string _name;

        public int Index { get { return _index; } }
        public ushort[] Colors { get { return _colors; } }
        public string Name { get { return _name; } }

        public Hue(int index)
        {
            _name = "Null";
            _index = index;
            _colors = new ushort[34];
        }

        //public Color GetColor(int index)
        //{
        //    int c16 = m_Colors[index];

        //    return Color.FromArgb((c16 & 0x7C00) >> 7, (c16 & 0x3E0) >> 2, (c16 & 0x1F) << 3);
        //}

        public Hue(int index, BinaryReader bin)
        {
            _index = index;
            _colors = new ushort[34];

            for (int i = 0; i < 34; ++i)
                _colors[i] = (ushort)(bin.ReadUInt16() | 0x8000);

            bool nulled = false;

            StringBuilder sb = new StringBuilder(20, 20);

            for (int i = 0; i < 20; ++i)
            {
                char c = (char)bin.ReadByte();

                if (c == 0)
                    nulled = true;
                else if (!nulled)
                    sb.Append(c);
            }

            _name = sb.ToString();
        }
    }
}
