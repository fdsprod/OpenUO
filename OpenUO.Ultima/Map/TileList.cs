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
    public class TileList
    {
        private Tile[] _tiles;
        private int _count;

        public TileList()
        {
            _tiles = new Tile[8];
            _count = 0;
        }

        public int Count
        {
            get
            {
                return _count;
            }
        }

        public void Add(short id, sbyte z)
        {
            if ((_count + 1) > _tiles.Length)
            {
                Tile[] old = _tiles;
                _tiles = new Tile[old.Length * 2];

                for (int i = 0; i < old.Length; ++i)
                    _tiles[i] = old[i];
            }

            _tiles[_count++].Set(id, z);
        }

        public Tile[] ToArray()
        {
            Tile[] tiles = new Tile[_count];

            for (int i = 0; i < _count; ++i)
                tiles[i] = _tiles[i];

            _count = 0;

            return tiles;
        }
    }
}
