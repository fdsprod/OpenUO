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
    public static class ClientUtility
    {
        public static void CalculateLoginKeys(uint major, uint minor, uint revision, uint build, out uint key1, out uint key2)
        {
            key1 = (major << 23) | (minor << 14) | (revision << 4);
            key1 ^= (revision * revision) << 9;
            key1 ^= (minor * minor);
            key1 ^= (minor * 11) << 24;
            key1 ^= (revision * 7) << 19;
            key1 ^= 0x2C13A5FD;

            key2 = (major << 22) | (revision << 13) | (minor << 3);
            key2 ^= (revision * revision * 3) << 10;
            key2 ^= (minor * minor);
            key2 ^= (minor * 13) << 23;
            key2 ^= (revision * 7) << 18;
            key2 ^= 0xA31D527F;
        }
    }
}
