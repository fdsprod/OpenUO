#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   Skill.cs
//  *
//  *   This program is free software; you can redistribute it and/or modify
//  *   it under the terms of the GNU General Public License as published by
//  *   the Free Software Foundation; either version 3 of the License, or
//  *   (at your option) any later version.
//  ***************************************************************************/

#endregion

#region Usings

using System;

#endregion

namespace OpenUO.Ultima
{
    public class Skill
    {
        private int _index = -1;
        private string _name = string.Empty;

        public Skill(SkillData data)
        {
            Data = data;
            ResetFromData();
        }

        public SkillData Data
        {
            get;
            private set;
        }

        public int Index
        {
            get { return _index; }
        }

        public bool HasUseButton
        {
            get;
            set;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public SkillCategory Category
        {
            get;
            set;
        }

        public byte Unknown
        {
            get;
            private set;
        }

        public int ID
        {
            get { return _index + 1; }
        }

        public void ResetFromData()
        {
            _index = Data.Index;
            HasUseButton = Data.UseButton;
            _name = Data.Name;
            Category = Data.Category;
            Unknown = Data.Unknown;
        }

        public void ResetFromData(SkillData data)
        {
            Data = data;
            _index = Data.Index;
            HasUseButton = Data.UseButton;
            _name = Data.Name;
            Category = Data.Category;
            Unknown = Data.Unknown;
        }

        public override string ToString()
        {
            return String.Format("{0} ({1:X4}) {2} {3} Category: {4}", _index, _index, HasUseButton ? "[x]" : "[ ]", _name, Category.Name);
        }
    }
}