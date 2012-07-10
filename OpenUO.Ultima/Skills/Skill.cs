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
    public class Skill
    {
        private SkillData _data;
        private int _index = -1;
        private bool _hasUseButton;
        private string _name = string.Empty;
        private SkillCategory _category;
        private byte _unknown;

        public SkillData Data 
        {
            get { return _data; }
        }

        public int Index
        { 
            get { return _index; } 
        }

        public bool HasUseButton
        { 
            get { return _hasUseButton; } 
            set { _hasUseButton = value; }
        }

        public string Name
        { 
            get { return _name; } 
            set { _name = value; } 
        }

        public SkillCategory Category 
        { 
            get { return _category; } 
            set { _category = value; } 
        }

        public byte Unknown 
        { 
            get { return _unknown; } 
        }

        public int ID 
        {
            get { return _index + 1; } 
        }
        
        public Skill(SkillData data)
        {
            _data = data;
            ResetFromData();
        }

        public void ResetFromData()
        {
            _index = _data.Index;
            _hasUseButton = _data.UseButton;
            _name = _data.Name;
            _category = _data.Category;
            _unknown = _data.Unknown;
        }

        public void ResetFromData(SkillData data)
        {
            _data = data;
            _index = _data.Index;
            _hasUseButton = _data.UseButton;
            _name = _data.Name;
            _category = _data.Category;
            _unknown = _data.Unknown;
        }

        public override string ToString()
        {
            return String.Format("{0} ({1:X4}) {2} {3} Category: {4}", _index, _index, _hasUseButton ? "[x]" : "[ ]", _name, _category.Name);
        }
    }

}
