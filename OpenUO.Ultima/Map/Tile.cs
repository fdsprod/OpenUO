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

using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace OpenUO.Ultima
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Tile
    {
        internal short _id;
        internal sbyte _z;

        public int Id
        {
            get { return _id; }
        }

        public int Z
        {
            get { return _z; }
            set { _z = (sbyte)value; }
        }

        public bool Ignored
        {
            get { return (_id == 2 || _id == 0x1DB || (_id >= 0x1AE && _id <= 0x1B5)); }
        }

        public Tile(short id, sbyte z)
        {
            _id = id;
            _z = z;
        }
    }
}
