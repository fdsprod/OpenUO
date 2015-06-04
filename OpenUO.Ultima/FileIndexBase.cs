#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   FileIndexBase.cs
//  *
//  *   This program is free software; you can redistribute it and/or modify
//  *   it under the terms of the GNU General Public License as published by
//  *   the Free Software Foundation; either version 3 of the License, or
//  *   (at your option) any later version.
//  ***************************************************************************/

#endregion

#region Usings

using System.IO;

#endregion

namespace OpenUO.Ultima
{
    public abstract class FileIndexBase
    {
        private readonly string _dataPath;

        protected FileIndexBase(string dataPath)
        {
            _dataPath = dataPath;
        }

        protected FileIndexBase(string dataPath, int length)
        {
            Length = length;
            _dataPath = dataPath;
        }

        public int Length
        {
            get;
            private set;
        }

        public bool IsOpen
        {
            get { return Stream != null && Stream.CanRead; }
        }

        public FileIndexEntry[] Entries
        {
            get;
            private set;
        }

        public Stream Stream
        {
            get;
            private set;
        }

        protected string DataPath
        {
            get { return _dataPath; }
        }

        public virtual bool FilesExist
        {
            get { return File.Exists(_dataPath); }
        }

        public Stream Seek(int index, out int length, out int extra)
        {
            if(!FilesExist || index < 0 || index >= Length)
            {
                length = extra = 0;
                return null;
            }

            var e = Entries[index];

            if(e.Lookup < 0 || e.Length <= 0)
            {
                length = extra = 0;
                return null;
            }

            length = e.Length & 0x7FFFFFFF;
            extra = e.Extra;

            if(Stream != null && (!Stream.CanRead || !Stream.CanSeek))
            {
                Stream.Dispose();
                Stream = null;
            }

            if(Stream == null)
            {
                Stream = new FileStream(_dataPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            }

            Stream.Seek(e.Lookup, SeekOrigin.Begin);

            return Stream;
        }

        public void Close()
        {
            if(Stream == null)
            {
                return;
            }

            Stream.Close();
            Stream = null;
        }

        public void Open()
        {
            if(Stream != null)
            {
                return;
            }

            if(!FilesExist)
            {
                return;
            }

            Entries = ReadEntries();

            Length = Entries.Length;
        }

        protected abstract FileIndexEntry[] ReadEntries();
    }
}