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
    public sealed class SkillCategoryData
    {
        //public static SkillCategoryData DefaultData { get { return new SkillCategoryData(0, -1, "null"); } }

        private readonly long _fileIndex = -1;
        private readonly int _index = -1;
        private readonly string _name = string.Empty;

        public long FileIndex
        {
            get { return _fileIndex; } 
        }

        public int Index
        { 
            get { return _index; }
        }

        public string Name
        {
            get { return _name; }
        }

        public SkillCategoryData(long fileIndex, int index, string name)
        {
            _fileIndex = fileIndex;
            _index = index;
            _name = name;
        }
    }
}
