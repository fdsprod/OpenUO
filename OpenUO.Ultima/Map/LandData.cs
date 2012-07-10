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
    public struct LandData
    {
        private readonly string _name;
        private readonly TileFlag _flags;

        public LandData(string name, TileFlag flags)
        {
            _name = name;
            _flags = flags;
        }

        /// <summary>
        /// Gets the name of this land tile.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets a bitfield representing the 32 individual flags of this land tile.
        /// </summary>
        public TileFlag Flags
        {
            get { return _flags; }
        }
    }
}
