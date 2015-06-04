#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   SkillStorageAdapter.cs
//  *
//  *   This program is free software; you can redistribute it and/or modify
//  *   it under the terms of the GNU General Public License as published by
//  *   the Free Software Foundation; either version 3 of the License, or
//  *   (at your option) any later version.
//  ***************************************************************************/

#endregion

#region Usings

using System.Collections.Generic;
using System.IO;
using System.Text;

#endregion

namespace OpenUO.Ultima.Adapters
{
    public class SkillStorageAdapter : StorageAdapterBase, ISkillStorageAdapter<Skill>
    {
        private SkillCategory[] _categories;
        private int[] _categoryLookup;
        private Skill[] _skills;

        public override int Length
        {
            get
            {
                if(!IsInitialized)
                {
                    Initialize();
                }

                return _skills.Length;
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            var install = Install;

            ReadCategories(install);
            ReadSkills(install);
        }

        public Skill GetSkill(int index)
        {
            if(index < _skills.Length)
            {
                return _skills[index];
            }

            return null;
        }

        private void ReadCategories(InstallLocation install)
        {
            var categories = new List<SkillCategory>();

            var grpPath = install.GetPath("skillgrp.mul");

            if(grpPath == null)
            {
                _categories = categories.ToArray();
                return;
            }

            using(var stream = new FileStream(grpPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var bin = new BinaryReader(stream);

                var categoryCount = bin.ReadInt32();

                categories.Add(new SkillCategory(new SkillCategoryData(-17, 0, "Misc.")));

                for(var i = 1; i < categoryCount; i++)
                {
                    var nameReader = new BinaryReader(stream);
                    var nameBuilder = new StringBuilder();
                    var nameBuffer = bin.ReadBytes(17);

                    for(var j = 0; j < 17; j++)
                    {
                        var ch = (char)nameBuffer[j];

                        if(char.IsLetterOrDigit(ch) || char.IsWhiteSpace(ch) || char.IsPunctuation(ch))
                        {
                            nameBuilder.Append(ch);
                        }
                    }

                    var name = nameBuilder.ToString();
                    var fileIndex = stream.Position - name.Length;
                    categories.Add(new SkillCategory(new SkillCategoryData(fileIndex, i, name)));
                }

                _categoryLookup = new int[(stream.Length - stream.Position) / 4];

                for(var i = 0; i < _categoryLookup.Length; i++)
                {
                    _categoryLookup[i] = bin.ReadInt32();
                }
            }

            _categories = categories.ToArray();
        }

        private void ReadSkills(InstallLocation install)
        {
            var fileIndex = install.CreateFileIndex("skills.idx", "skills.mul");

            _skills = new Skill[fileIndex.Length];

            for(var i = 0; i < _skills.Length; i++)
            {
                var skill = ReadSkill(fileIndex, i);

                _skills[i] = skill;
            }
        }

        private Skill ReadSkill(FileIndexBase fileIndex, int index)
        {
            int length, extra;
            var stream = fileIndex.Seek(index, out length, out extra);

            if(stream == null)
            {
                return null;
            }

            var bin = new BinaryReader(stream);
            var nameLength = length - 2;

            var useBtn = bin.ReadByte();
            var nameBuffer = new byte[nameLength];
            bin.Read(nameBuffer, 0, nameLength);
            var unk = bin.ReadByte();

            var sb = new StringBuilder(nameBuffer.Length);

            for(var i = 0; i < nameBuffer.Length; i++)
            {
                sb.Append((char)nameBuffer[i]);
            }

            var category = _categories[0];

            if(index < _categoryLookup.Length)
            {
                category = _categories[_categoryLookup[index]];
            }

            var skill = new Skill(new SkillData(index, sb.ToString(), useBtn > 0, extra, unk, category));

            return skill;
        }
    }
}