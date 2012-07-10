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
    public struct UopIndexEntry
    {
        public long Lookup;
        public int Length;
        public int Extra;
        public int CompressedSize; // UOP 
        public int DecompressedSize; // UOP 
        public long EntryHash; // UOP Entry Lookup.
        public int BlockHash; // UOP Block Lookup.
        public bool IsCompressed; // UOP Entry Compressed.
    }
}
