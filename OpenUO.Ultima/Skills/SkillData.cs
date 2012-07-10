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
    public sealed class SkillData
    {
        //public static SkillData DefaultData { get { return new SkillData(-1, "null", false, 0, 0x0, null); } }

        private readonly int _index = -1;
        private readonly string _name = string.Empty;
        private readonly int _extra;
        private readonly bool _useButton;
        private readonly byte _unknown;
        private readonly SkillCategory _category;

        public int Index
        { 
            get { return _index; } 
        }

        public string Name 
        { 
            get { return _name; }
        }

        public int Extra
        { 
            get { return _extra; }
        }

        public bool UseButton 
        {
            get { return _useButton; }
        }

        public byte Unknown
        {
            get { return _unknown; }
        }

        public SkillCategory Category
        {
            get { return _category; }
        }

        public int NameLength
        {
            get { return _name.Length; }
        }

        public SkillData(int index, string name, bool useButton, int extra, byte unk, SkillCategory category)
        {
            _index = index;
            _category = category;
            _name = name;
            _useButton = useButton;
            _extra = extra;
            _unknown = unk;
        }
    }
}
