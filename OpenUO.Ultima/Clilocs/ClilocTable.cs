#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   ClilocTable.cs
//  *
//  *   This program is free software; you can redistribute it and/or modify
//  *   it under the terms of the GNU General Public License as published by
//  *   the Free Software Foundation; either version 3 of the License, or
//  *   (at your option) any later version.
//  ***************************************************************************/

#endregion

#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OpenUO.Core.Diagnostics;

#endregion

namespace OpenUO.Ultima
{
    public class ClientLocalizations : IEnumerable<LocalizationEntry>, IDisposable
    {
        private readonly Dictionary<int, LocalizationEntry> _table = new Dictionary<int, LocalizationEntry>();

        public virtual ClientLocalizationLanguage Language
        {
            get;
            protected set;
        }

        public virtual FileInfo InputFile
        {
            get;
            protected set;
        }

        public int Count
        {
            get { return _table.Count; }
        }

        public bool Loaded
        {
            get;
            private set;
        }

        public ClilocInfo this[int index]
        {
            get { return Lookup(index); }
        }

        public void Dispose()
        {
            Unload();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _table.Values.GetEnumerator();
        }

        public virtual IEnumerator<LocalizationEntry> GetEnumerator()
        {
            return _table.Values.GetEnumerator();
        }

        public void Clear()
        {
            foreach (LocalizationEntry d in _table.Values)
            {
                d.Clear();
            }
        }

        public void Unload()
        {
            if (!Loaded)
            {
                return;
            }

            Language = ClientLocalizationLanguage.NULL;
            InputFile = null;

            _table.Clear();

            Loaded = false;
        }

        public void Load(FileInfo file)
        {
            if (Loaded)
            {
                return;
            }

            try
            {
                ClientLocalizationLanguage lng = ClientLocalizationLanguage.NULL;

                if (!Enum.TryParse(file.Extension.TrimStart('.'), true, out lng))
                {
                    throw new FileLoadException("Could not detect language for: " + file.FullName);
                }

                Language = lng;
                InputFile = file;

                using (BinaryReader reader = new BinaryReader(InputFile.OpenRead(), Encoding.UTF8))
                {
                    long size = reader.BaseStream.Seek(0, SeekOrigin.End);
                    reader.BaseStream.Seek(0, SeekOrigin.Begin);

                    reader.ReadInt32();
                    reader.ReadInt16();

                    while (reader.BaseStream.Position < size)
                    {
                        int index = reader.ReadInt32();
                        reader.ReadByte();
                        int length = reader.ReadInt16();
                        long offset = reader.BaseStream.Position;
                        reader.BaseStream.Seek(length, SeekOrigin.Current);

                        if (_table.ContainsKey(index))
                        {
                            _table[index] = new LocalizationEntry(Language, index, offset, length);
                        }
                        else
                        {
                            _table.Add(index, new LocalizationEntry(Language, index, offset, length));
                        }
                    }
                }

                Loaded = true;
            }
            catch (Exception e)
            {
                Tracer.Error(e);
            }
        }

        public bool Contains(int index)
        {
            return _table.ContainsKey(index);
        }

        public bool IsNullOrEmpty(int index)
        {
            if (!Contains(index) || _table[index] == null)
            {
                return true;
            }

            ClilocInfo info = _table[index].Lookup(InputFile);

            if (!String.IsNullOrWhiteSpace(info.Text))
            {
                return false;
            }

            return true;
        }

        public ClilocInfo Update(int index)
        {
            if (!Contains(index) || _table[index] == null)
            {
                return null;
            }

            return _table[index].Lookup(InputFile, true);
        }

        public ClilocInfo Lookup(int index)
        {
            if (!Contains(index) || _table[index] == null)
            {
                return null;
            }

            return _table[index].Lookup(InputFile);
        }

        public override string ToString()
        {
            return ((Language == ClientLocalizationLanguage.NULL) ? "Not Loaded" : "Cliloc." + Language);
        }
    }
}