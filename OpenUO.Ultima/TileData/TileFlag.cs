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

using System;

namespace OpenUO.Ultima
{
    [Flags]
    public enum TileFlag
    {
        /// <summary>
        /// Nothing is flagged.
        /// </summary>
        None = 0x00000000,
        /// <summary>
        /// Not yet documented.
        /// </summary>
        Background = 0x00000001,
        /// <summary>
        /// Not yet documented.
        /// </summary>
        Weapon = 0x00000002,
        /// <summary>
        /// Not yet documented.
        /// </summary>
        Transparent = 0x00000004,
        /// <summary>
        /// The tile is rendered with partial alpha-transparency.
        /// </summary>
        Translucent = 0x00000008,
        /// <summary>
        /// The tile is a wall.
        /// </summary>
        Wall = 0x00000010,
        /// <summary>
        /// The tile can cause damage when moved over.
        /// </summary>
        Damaging = 0x00000020,
        /// <summary>
        /// The tile may not be moved over or through.
        /// </summary>
        Impassable = 0x00000040,
        /// <summary>
        /// Not yet documented.
        /// </summary>
        Wet = 0x00000080,
        /// <summary>
        /// Unknown.
        /// </summary>
        Unknown1 = 0x00000100,
        /// <summary>
        /// The tile is a surface. It may be moved over, but not through.
        /// </summary>
        Surface = 0x00000200,
        /// <summary>
        /// The tile is a stair, ramp, or ladder.
        /// </summary>
        Bridge = 0x00000400,
        /// <summary>
        /// Not yet documented.
        /// </summary>
        Generic = 0x00000800,
        /// <summary>
        /// The tile is a window. Like <see cref="TileFlag.NoShoot" />, tiles with this flag block line of sight.
        /// </summary>
        Window = 0x00001000,
        /// <summary>
        /// The tile blocks line of sight.
        /// </summary>
        NoShoot = 0x00002000,
        /// <summary>
        /// For single-amount tiles, the string "a " should be prepended to the tile name.
        /// </summary>
        ArticleA = 0x00004000,
        /// <summary>
        /// For single-amount tiles, the string "an " should be prepended to the tile name.
        /// </summary>
        ArticleAn = 0x00008000,
        /// <summary>
        /// Not yet documented.
        /// </summary>
        Internal = 0x00010000,
        /// <summary>
        /// The tile becomes translucent when walked behind. Boat masts also have this flag.
        /// </summary>
        Foliage = 0x00020000,
        /// <summary>
        /// Not yet documented.
        /// </summary>
        PartialHue = 0x00040000,
        /// <summary>
        /// Unknown.
        /// </summary>
        Unknown2 = 0x00080000,
        /// <summary>
        /// The tile is a map--in the cartography sense. Unknown usage.
        /// </summary>
        Map = 0x00100000,
        /// <summary>
        /// The tile is a container.
        /// </summary>
        Container = 0x00200000,
        /// <summary>
        /// The tile may be equiped.
        /// </summary>
        Wearable = 0x00400000,
        /// <summary>
        /// The tile gives off light.
        /// </summary>
        LightSource = 0x00800000,
        /// <summary>
        /// The tile is animated.
        /// </summary>
        Animation = 0x01000000,
        /// <summary>
        /// Not yet documented.
        /// </summary>
        NoDiagonal = 0x02000000,
        /// <summary>
        /// Unknown.
        /// </summary>
        Unknown3 = 0x04000000,
        /// <summary>
        /// Not yet documented.
        /// </summary>
        Armor = 0x08000000,
        /// <summary>
        /// The tile is a slanted roof.
        /// </summary>
        Roof = 0x10000000,
        /// <summary>
        /// The tile is a door. Tiles with this flag can be moved through by ghosts and GMs.
        /// </summary>
        Door = 0x20000000,
        /// <summary>
        /// Not yet documented.
        /// </summary>
        StairBack = 0x40000000,
        /// <summary>
        /// Not yet documented.
        /// </summary>
        StairRight = unchecked((int)0x80000000)
    }
}
