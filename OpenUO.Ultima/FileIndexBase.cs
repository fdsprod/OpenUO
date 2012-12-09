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
using OpenUO.Core.Diagnostics;

namespace OpenUO.Ultima
{
    public abstract class FileIndexBase
    {
        private string _dataPath;
        private int _length;
        private FileIndexEntry[] _entries;
        private Stream _stream;

        public int Length
        {
            get { return _length; }
        } 

        public bool IsOpen
        {
            get { return _stream != null && _stream.CanRead; }
        }

        public FileIndexEntry[] Entries
        {
            get { return _entries; }
        }

        public Stream Stream
        {
            get { return _stream; }
        }

        protected string DataPath
        {
            get { return _dataPath; }
        }

        public virtual bool FilesExist
        {
            get{ return File.Exists(_dataPath); }
        }

        protected FileIndexBase(string dataPath)
        {
            _dataPath = dataPath;
        }

        protected FileIndexBase(string dataPath, int length)
        {
            _length = length;
            _dataPath = dataPath;
        }

        public Stream Seek(int index, out int length, out int extra)
        {
            if (!FilesExist || index < 0 || index >= _length)
            {
                length = extra = 0;
                return null;
            }

            FileIndexEntry e = _entries[index];

            if (e.Lookup < 0 || e.Length <= 0)
            {
                length = extra = 0;
                return null;
            }

            length = e.Length & 0x7FFFFFFF;
            extra = e.Extra;

            if (_stream != null && (!_stream.CanRead || !_stream.CanSeek))
            {
                _stream.Dispose();
                _stream = null;
            }

            if (_stream == null)
            {
                _stream = new FileStream(_dataPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            }

            _stream.Seek(e.Lookup, SeekOrigin.Begin);

            return _stream;
        }
        
        public void Close()
        {
            if (_stream == null)
                return;

            _stream.Close();
            _stream = null;
        }

        public void Open()
        {
            if (_stream != null)
                return;

            if (!FilesExist)
                return;

            _entries = ReadEntries();

            _length = _entries.Length;
        }

        protected abstract FileIndexEntry[] ReadEntries();
    }
}