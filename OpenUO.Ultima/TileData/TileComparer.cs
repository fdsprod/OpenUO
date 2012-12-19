#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   TileComparer.cs
//  *
//  *   This program is free software; you can redistribute it and/or modify
//  *   it under the terms of the GNU General Public License as published by
//  *   the Free Software Foundation; either version 3 of the License, or
//  *   (at your option) any later version.
//  ***************************************************************************/

#endregion

#region Usings

using System.Collections.Generic;

#endregion

namespace OpenUO.Ultima
{
    public class TileComparer : IComparer<Tile>
    {
        private readonly TileData _tileData;

        public TileComparer(TileData tileData)
        {
            _tileData = tileData;
        }

        public int Compare(Tile x, Tile y)
        {
            if (x._z != y._z)
            {
                return x._z.CompareTo(y._z);
            }

            ItemData xData = _tileData.ItemTable[x._id & 0x3FFF];
            ItemData yData = _tileData.ItemTable[y._id & 0x3FFF];

            if (xData._height != yData._height)
            {
                return xData._height.CompareTo(yData._height);
            }

            if (xData._height != yData._height)
            {
                return xData._height.CompareTo(yData._height);
            }

            if (xData.Background != yData.Background)
            {
                xData.Background.CompareTo(yData.Background);
            }

            return 0;
        }
    }
}