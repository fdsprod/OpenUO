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
    public struct StaticTile
    {
        public short Id;
        public byte X;
        public byte Y;
        public sbyte Z;
        public short Hue;
    }
}
