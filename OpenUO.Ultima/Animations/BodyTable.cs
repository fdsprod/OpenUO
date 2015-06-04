#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   BodyTable.cs
//  *
//  *   This program is free software; you can redistribute it and/or modify
//  *   it under the terms of the GNU General Public License as published by
//  *   the Free Software Foundation; either version 3 of the License, or
//  *   (at your option) any later version.
//  ***************************************************************************/

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using OpenUO.Core.Diagnostics;

#endregion

namespace OpenUO.Ultima
{
    public sealed class BodyTable
    {
        private readonly Dictionary<int, BodyTableEntry> _entries;

        public BodyTable(string filePath)
        {
            _entries = new Dictionary<int, BodyTableEntry>();

            if(string.IsNullOrEmpty(filePath))
            {
                return;
            }

            var def = new StreamReader(filePath);

            string line;
            var lineNumber = 0;

            while((line = def.ReadLine()) != null)
            {
                lineNumber++;

                if((line = line.Trim()).Length == 0 || line.StartsWith("#"))
                {
                    continue;
                }

                try
                {
                    var index1 = line.IndexOf(" {");
                    var index2 = line.IndexOf("} ");

                    var newIdString = line.Substring(0, index1);
                    var oldIdString = line.Substring(index1 + 2, index2 - index1 - 2);
                    var newHueString = line.Substring(index2 + 2);

                    var indexOf = oldIdString.IndexOf(',');

                    if(indexOf > -1)
                    {
                        oldIdString = oldIdString.Substring(0, indexOf).Trim();
                    }

                    int newId;
                    int oldId;
                    int newHue;

                    if(!int.TryParse(newIdString, out newId))
                    {
                        Tracer.Warn("New ID '{0}' at line number {1} is not a valid integer", newIdString, lineNumber);
                        continue;
                    }

                    if(!int.TryParse(oldIdString, out oldId))
                    {
                        Tracer.Warn("Old ID '{0}' at line number {1} is not a valid integer", oldIdString, lineNumber);
                        continue;
                    }

                    if(!int.TryParse(newHueString, out newHue))
                    {
                        Tracer.Warn("Hue ID '{0}' at line number {1} is not a valid integer", newHueString, lineNumber);
                        continue;
                    }

                    _entries[newId] = new BodyTableEntry(oldId, newId, newHue);
                }
                catch(Exception e)
                {
                    Tracer.Error(e);
                }
            }
        }

        public Dictionary<int, BodyTableEntry> Entries
        {
            get { return _entries; }
        }

        public BodyTableEntry this[int key]
        {
            get
            {
                BodyTableEntry entry;
                _entries.TryGetValue(key, out entry);
                return entry;
            }
        }
    }
}