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
    public sealed class BodyTableEntry
    {
        public int m_OldID;
        public int m_NewID;
        public int m_NewHue;

        public BodyTableEntry(int oldID, int newID, int newHue)
        {
            m_OldID = oldID;
            m_NewID = newID;
            m_NewHue = newHue;
        }
    }
}
