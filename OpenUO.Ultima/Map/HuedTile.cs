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

using System.Runtime.InteropServices;

namespace OpenUO.Ultima
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct HuedTile
    {
        internal short _id;
        internal short _hue;
        internal sbyte _z;

        public int Id
        {
            get { return _id; }
        }

        public int Hue
        {
            get { return _hue; }
        }

        public int Z
        {
            get { return _z; }
            set { _z = (sbyte)value; }

        }

        public HuedTile(short id, short hue, sbyte z)
        {
            _id = id;
            _hue = hue;
            _z = z;
        }

        public void Set(short id, short hue, sbyte z)
        {
            _id = id;
            _hue = hue;
            _z = z;
        }
    }
}
