#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   HuedTileList.cs
//  *
//  *   This program is free software; you can redistribute it and/or modify
//  *   it under the terms of the GNU General Public License as published by
//  *   the Free Software Foundation; either version 3 of the License, or
//  *   (at your option) any later version.
//  ***************************************************************************/

#endregion

namespace OpenUO.Ultima
{
    internal class HuedTileList
    {
        private HuedTile[] _tiles;

        public HuedTileList()
        {
            _tiles = new HuedTile[8];
            Count = 0;
        }

        public int Count
        {
            get;
            private set;
        }

        public void Add(short id, short hue, sbyte z)
        {
            if((Count + 1) > _tiles.Length)
            {
                var old = _tiles;
                _tiles = new HuedTile[old.Length * 2];

                for(var i = 0; i < old.Length; ++i)
                {
                    _tiles[i] = old[i];
                }
            }

            _tiles[Count++].Set(id, hue, z);
        }

        public HuedTile[] ToArray()
        {
            var tiles = new HuedTile[Count];

            for(var i = 0; i < Count; ++i)
            {
                tiles[i] = _tiles[i];
            }

            Count = 0;

            return tiles;
        }
    }
}