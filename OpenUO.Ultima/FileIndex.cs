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
    public class FileIndex
    {
        const int UOPMagicNumber = 0x0050594D;

        private readonly string _indexPath;
        private readonly string _mulPath;

        private int _length;
        private FileIndexEntry[] _entries;
        private bool _filesExist;
        private Stream _stream;
        private bool _isUopFormat;

        public bool IsUopFormat
        {
            get { return _isUopFormat; }
        }

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

        public string IndexPath
        {
            get { return _indexPath; }
        }

        public string MulPath
        {
            get { return _mulPath; }
        }

        public bool FilesExist
        {
            get { return _filesExist; }
        }

        public Stream Seek(int index, out int length, out int extra)
        {
            if (!_filesExist || index < 0 || index >= _length)
            {
                length = extra = 0;
                return null;
            }

            FileIndexEntry e = _entries[index];

            if (e.Lookup < 0)
            {
                length = extra = 0;
                return null;
            }

            length = e.Length & 0x7FFFFFFF;
            extra = e.Extra;

            if (_stream == null)
            {
                length = extra = 0;
                return null;
            }

            if (_stream == null || !_stream.CanRead || !_stream.CanSeek)
                _stream = new FileStream(_mulPath, FileMode.Open, FileAccess.Read, FileShare.Read);

            _stream.Seek(e.Lookup, SeekOrigin.Begin);

            return _stream;
        }

        public FileIndex(string uopFile)
        {
            _indexPath = uopFile;
            _mulPath = uopFile;

            _isUopFormat = true;
            
            if (!File.Exists(uopFile))
            {
                _filesExist = false;
                return;
            }

            _filesExist = true;

            Open();
        }

        public FileIndex(string idxFile, string mulFile)
        {
            _indexPath = idxFile;
            _mulPath = mulFile;
            
            if (!File.Exists(_indexPath) || !File.Exists(_mulPath))
            {
                _filesExist = false;
                return;
            }
            
            _filesExist = true;
            
            Open();
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

            if (!_filesExist)
                return;

            if (_isUopFormat)
                _entries = ReadUOPEntries();
            else
                _entries = ReadLegacyEntries();

            _length = _entries.Length;
        }

        private FileIndexEntry[] ReadUOPEntries()
        {
            List<FileIndexEntry> entries = new List<FileIndexEntry>();

            _stream = new FileStream(_indexPath, FileMode.Open);

            using (BinaryReader reader = new BinaryReader(_stream))
            {
                UopHeader header = new UopHeader()
                {
                    MagicNumber = reader.ReadInt32(),
                    Version = reader.ReadInt32(),
                    Misc = reader.ReadInt32(),
                    StartAddress = reader.ReadInt64(),
                    BlockSize = reader.ReadInt32(),
                    FileCount = reader.ReadInt32(),
                };

                if (header.MagicNumber != UOPMagicNumber)
                {
                    Tracer.Error("{0} is not a UOP file, magic number not found!", _indexPath);
                    return new FileIndexEntry[0];
                }

                long nextBlockAddress = header.StartAddress;

                List<UopIndexEntry> uopEntries = new List<UopIndexEntry>();

                while (nextBlockAddress != 0L)
                {
                    _stream.Seek(nextBlockAddress, SeekOrigin.Begin);

                    UopBlock block = new UopBlock()
                    {
                        FileCount = reader.ReadInt32(),
                        NextBlockAddress = reader.ReadInt64()
                    };

                    for (int i = 0; i < block.FileCount; i++)
                    {
                        UopIndexEntry uopEntry = new UopIndexEntry()
                        {
                            Lookup = reader.ReadInt64(),
                            Length = reader.ReadInt32(),
                            CompressedSize = reader.ReadInt32(),
                            DecompressedSize = reader.ReadInt32(),
                            EntryHash = reader.ReadInt64(),
                            BlockHash = reader.ReadInt32(),
                            IsCompressed = reader.ReadInt16() == 1,
                        };

                        uopEntries.Add(uopEntry);
                    }

                    nextBlockAddress = block.NextBlockAddress;
                }

                foreach (var uopEntry in uopEntries)
                {
                    FileIndexEntry entry = new FileIndexEntry()
                    {
                        Lookup = (int)uopEntry.Lookup,
                        Length = uopEntry.DecompressedSize
                    };

                    entries.Add(entry);
                }
            }

            return entries.ToArray();
        }

        private FileIndexEntry[] ReadLegacyEntries()
        {
            List<FileIndexEntry> entries = new List<FileIndexEntry>();

            int length = (int)((new FileInfo(_indexPath).Length / 3) / 4);

            using (FileStream index = new FileStream(_indexPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BinaryReader bin = new BinaryReader(index);
                _stream = new FileStream(_mulPath, FileMode.Open, FileAccess.Read, FileShare.Read);

                int count = (int)(index.Length / 12);

                for (int i = 0; i < count && i < length; ++i)
                {
                    FileIndexEntry entry = new FileIndexEntry()
                    {
                        Lookup = bin.ReadInt32(),
                        Length = bin.ReadInt32(),
                        Extra = bin.ReadInt32(),
                    };

                    entries.Add(entry);
                }

                for (int i = count; i < length; ++i)
                {
                    FileIndexEntry entry = new FileIndexEntry()
                    {
                        Lookup = -1,
                        Length = -1,
                        Extra = -1,
                    };

                    entries.Add(entry);
                }
            }

            return entries.ToArray();
        }
    }

    public struct UopHeader
    {
        public int MagicNumber;
        public int Version;
        public int Misc;
        public long StartAddress;
        public int BlockSize;
        public int FileCount;
    }

    public struct UopBlock
    {
        public int FileCount;
        public long NextBlockAddress;
    }
}