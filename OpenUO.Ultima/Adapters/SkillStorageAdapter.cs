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

using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OpenUO.Ultima.Adapters
{
    public class SkillStorageAdapter : StorageAdapterBase, ISkillStorageAdapter<Skill>
    {
        private Skill[] _skills;
        private SkillCategory[] _categories;
        private int[] _categoryLookup;
        
        public override void Initialize()
        {
            base.Initialize();

            var install = Install;

            ReadCategories(install);
            ReadSkills(install);
        }
        private void ReadCategories(InstallLocation install)
        {
            List<SkillCategory> categories = new List<SkillCategory>();

            string grpPath = install.GetPath("skillgrp.mul");

            if (grpPath == null)
            {
                _categories = categories.ToArray();
                return;
            }

            using (FileStream stream = new FileStream(grpPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BinaryReader bin = new BinaryReader(stream);

                int categoryCount = bin.ReadInt32();

                categories.Add(new SkillCategory(new SkillCategoryData(-17, 0, "Misc.")));

                for (int i = 1; i < categoryCount; i++)
                {
                    BinaryReader nameReader = new BinaryReader(stream);
                    StringBuilder nameBuilder = new StringBuilder();
                    byte[] nameBuffer = bin.ReadBytes(17);

                    for (int j = 0; j < 17; j++)
                    {
                        char ch = (char)nameBuffer[j];

                        if (char.IsLetterOrDigit(ch) || char.IsWhiteSpace(ch) || char.IsPunctuation(ch))
                            nameBuilder.Append(ch);
                    }

                    string name = nameBuilder.ToString();
                    long fileIndex = stream.Position - name.Length;
                    categories.Add(new SkillCategory(new SkillCategoryData(fileIndex, i, name)));
                }

                _categoryLookup = new int[(stream.Length - stream.Position) / 4];

                for (int i = 0; i < _categoryLookup.Length; i++)
                    _categoryLookup[i] = bin.ReadInt32();
            }

            _categories = categories.ToArray();
        }

        private void ReadSkills(InstallLocation install)
        {
            var fileIndex = install.CreateFileIndex("skills.idx", "skills.mul");

            _skills = new Skill[fileIndex.Length];

            for (int i = 0; i < _skills.Length; i++)
            {
                Skill skill = ReadSkill(fileIndex, i);

                _skills[i] = skill;
            }
        }

        private Skill ReadSkill(FileIndex fileIndex, int index)
        {
            int length, extra;
            Stream stream = fileIndex.Seek(index, out length, out extra);

            if (stream == null)
                return null;

            BinaryReader bin = new BinaryReader(stream);
            int nameLength = length - 2;

            byte useBtn = bin.ReadByte();
            byte[] nameBuffer = new byte[nameLength];
            bin.Read(nameBuffer, 0, nameLength);
            byte unk = bin.ReadByte();

            StringBuilder sb = new StringBuilder(nameBuffer.Length);

            for (int i = 0; i < nameBuffer.Length; i++)
                sb.Append((char)nameBuffer[i]);

            SkillCategory category = _categories[0];

            if (index < _categoryLookup.Length)
                category = _categories[_categoryLookup[index]];

            Skill skill = new Skill(new SkillData(index, sb.ToString(), useBtn > 0, extra, unk, category));

            return skill;
        }

        public unsafe Skill GetSkill(int index)
        {
            if (index < _skills.Length)
                return _skills[index];

            return null;
        }
    }
}
