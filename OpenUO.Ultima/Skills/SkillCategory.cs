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

    public class SkillCategory
    {
        private SkillCategoryData _data;
        private int _index = -1;
        private string _name = string.Empty;

        public SkillCategoryData Data
        {
            get { return _data; } 
        }

        public int Index 
        {
            get { return _index; } 
        }

        public string Name 
        {
            get { return _name; }
        }

        public SkillCategory(SkillCategoryData data)
        {
            _data = data;
            _index = _data.Index;
            _name = _data.Name;
        }

        public void ResetFromData()
        {
            _index = _data.Index;
            _name = _data.Name;
        }

        public void ResetFromData(SkillCategoryData data)
        {
            _data = data;
            _index = _data.Index;
            _name = _data.Name;
        }

        public override string ToString()
        {
            return string.Format("{{SkillCategory: {0}}}", _name);
        }
    }
}
