#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   TileMatrix.cs
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

#endregion

namespace OpenUO.Ultima
{
    public class TileMatrix
    {
        private static HuedTileList[][] _hueTileLists;
        private readonly int _blockHeight;
        private readonly int _blockWidth;

        private readonly HuedTile[][][] _emptyStaticBlock;

        private readonly FileStream _fileIndex;
        private readonly int _height;
        private readonly Tile[] _invalidLandBlock;
        private readonly FileStream _map;
        private readonly UOPIndex _mapIndex;
        private readonly BinaryReader _reader;

        private readonly FileStream _staticsStream;

        private readonly int _width;

        public TileMatrix(InstallLocation install, int fileIndex, int mapID, int width, int height)
        {
            _width = width;
            _height = height;
            _blockWidth = width >> 3;
            _blockHeight = height >> 3;

            if (fileIndex != 0x7F)
            {
                string mapPath = install.GetPath("map{0}.mul", fileIndex);

                if (File.Exists(mapPath))
                {
                    _map = new FileStream(mapPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                }
                else
                {
                    mapPath = install.GetPath("map{0}LegacyMUL.uop", fileIndex);

                    if (File.Exists(mapPath))
                    {
                        _map = new FileStream(mapPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        _mapIndex = new UOPIndex(_map);
                    }
                }

                string indexPath = install.GetPath("staidx{0}.mul", fileIndex);

                if (File.Exists(indexPath))
                {
                    _fileIndex = new FileStream(indexPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    _reader = new BinaryReader(_fileIndex);
                }

                string staticsPath = install.GetPath("statics{0}.mul", fileIndex);

                if (File.Exists(staticsPath))
                {
                    _staticsStream = new FileStream(staticsPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                }
            }

            _emptyStaticBlock = new HuedTile[8][][];

            for (int i = 0; i < 8; ++i)
            {
                _emptyStaticBlock[i] = new HuedTile[8][];

                for (int j = 0; j < 8; ++j)
                {
                    _emptyStaticBlock[i][j] = new HuedTile[0];
                }
            }

            _invalidLandBlock = new Tile[196];
        }

        public int BlockWidth
        {
            get { return _blockWidth; }
        }

        public int BlockHeight
        {
            get { return _blockHeight; }
        }

        public int Width
        {
            get { return _width; }
        }

        public int Height
        {
            get { return _height; }
        }

        public HuedTile[][][] EmptyStaticBlock
        {
            get { return _emptyStaticBlock; }
        }

        public HuedTile[][][] GetStaticBlock(int x, int y)
        {
            if (x < 0 || y < 0 || x >= _blockWidth || y >= _blockHeight || _staticsStream == null || _fileIndex == null)
            {
                return _emptyStaticBlock;
            }

            return ReadStaticBlock(x, y);
        }

        public HuedTile[] GetStaticTiles(int x, int y)
        {
            HuedTile[][][] tiles = GetStaticBlock(x >> 3, y >> 3);

            return tiles[x & 0x7][y & 0x7];
        }

        public Tile[] GetLandBlock(int x, int y)
        {
            if (x < 0 || y < 0 || x >= _blockWidth || y >= _blockHeight || _map == null)
            {
                return _invalidLandBlock;
            }

            return ReadLandBlock(x, y);
        }

        public Tile GetLandTile(int x, int y)
        {
            Tile[] tiles = GetLandBlock(x >> 3, y >> 3);

            return tiles[((y & 0x7) << 3) + (x & 0x7)];
        }

        private unsafe HuedTile[][][] ReadStaticBlock(int x, int y)
        {
            _reader.BaseStream.Seek(((x * _blockHeight) + y) * 12, SeekOrigin.Begin);

            int lookup = _reader.ReadInt32();
            int length = _reader.ReadInt32();

            if (lookup < 0 || length <= 0)
            {
                return _emptyStaticBlock;
            }

            int count = length / 7;

            _staticsStream.Seek(lookup, SeekOrigin.Begin);

            StaticTileData[] staTiles = new StaticTileData[count];

            fixed (StaticTileData* pTiles = staTiles)
            {
                NativeMethods._lread(_staticsStream.SafeFileHandle, pTiles, length);

                if (_hueTileLists == null)
                {
                    _hueTileLists = new HuedTileList[8][];

                    for (int i = 0; i < 8; ++i)
                    {
                        _hueTileLists[i] = new HuedTileList[8];

                        for (int j = 0; j < 8; ++j)
                        {
                            _hueTileLists[i][j] = new HuedTileList();
                        }
                    }
                }

                HuedTileList[][] lists = _hueTileLists;

                StaticTileData* pCur = pTiles, pEnd = pTiles + count;

                while (pCur < pEnd)
                {
                    lists[pCur->X & 0x7][pCur->Y & 0x7].Add((short)((pCur->Id & 0x3FFF) + 0x4000), pCur->Hue, pCur->Z);
                    ++pCur;
                }

                HuedTile[][][] tiles = new HuedTile[8][][];

                for (int i = 0; i < 8; ++i)
                {
                    tiles[i] = new HuedTile[8][];

                    for (int j = 0; j < 8; ++j)
                    {
                        tiles[i][j] = lists[i][j].ToArray();
                    }
                }

                return tiles;
            }
        }

        private unsafe Tile[] ReadLandBlock(int x, int y)
        {
            int offset = ((x * _blockHeight) + y) * 196 + 4;

            if (_mapIndex != null)
            {
                offset = _mapIndex.Lookup(offset);
            }

            _map.Seek(offset, SeekOrigin.Begin);

            Tile[] tiles = new Tile[64];

            fixed (Tile* pTiles = tiles)
            {
                NativeMethods._lread(_map.SafeFileHandle, pTiles, 192);
            }

            return tiles;
        }

        public void Dispose()
        {
            if (_map != null)
            {
                _map.Close();
            }

            if (_staticsStream != null)
            {
                _staticsStream.Close();
            }

            if (_reader != null)
            {
                _reader.Close();
            }
        }

        public class UOPIndex
        {
            private readonly UOPEntry[] _entries;
            private readonly int _length;
            private readonly BinaryReader _reader;
            private readonly int _version;

            public UOPIndex(FileStream stream)
            {
                _reader = new BinaryReader(stream);
                _length = (int)stream.Length;

                if (_reader.ReadInt32() != 0x50594D)
                {
                    throw new ArgumentException("Invalid UOP file.");
                }

                _version = _reader.ReadInt32();
                _reader.ReadInt32();
                int nextTable = _reader.ReadInt32();

                List<UOPEntry> entries = new List<UOPEntry>();

                do
                {
                    stream.Seek(nextTable, SeekOrigin.Begin);
                    int count = _reader.ReadInt32();
                    nextTable = _reader.ReadInt32();
                    _reader.ReadInt32();

                    for (int i = 0; i < count; ++i)
                    {
                        int offset = _reader.ReadInt32();

                        if (offset == 0)
                        {
                            stream.Seek(30, SeekOrigin.Current);
                            continue;
                        }

                        _reader.ReadInt64();
                        int length = _reader.ReadInt32();

                        entries.Add(new UOPEntry(offset, length));

                        stream.Seek(18, SeekOrigin.Current);
                    }
                }
                while (nextTable != 0 && nextTable < _length);

                entries.Sort(OffsetComparer.Instance);

                for (int i = 0; i < entries.Count; ++i)
                {
                    stream.Seek(entries[i].Offset + 2, SeekOrigin.Begin);

                    int dataOffset = _reader.ReadInt16();
                    entries[i].Offset += 4 + dataOffset;

                    stream.Seek(dataOffset, SeekOrigin.Current);
                    entries[i].Order = _reader.ReadInt32();
                }

                entries.Sort();
                _entries = entries.ToArray();
            }

            public int Version
            {
                get { return _version; }
            }

            public int Lookup(int offset)
            {
                int total = 0;

                for (int i = 0; i < _entries.Length; ++i)
                {
                    int newTotal = total + _entries[i].Length;

                    if (offset < newTotal)
                    {
                        return _entries[i].Offset + (offset - total);
                    }

                    total = newTotal;
                }

                return _length;
            }

            public void Close()
            {
                _reader.Close();
            }

            private class OffsetComparer : IComparer<UOPEntry>
            {
                public static readonly IComparer<UOPEntry> Instance = new OffsetComparer();

                public int Compare(UOPEntry x, UOPEntry y)
                {
                    return x.Offset.CompareTo(y.Offset);
                }
            }

            private class UOPEntry : IComparable<UOPEntry>
            {
                public readonly int Length;
                public int Offset;
                public int Order;

                public UOPEntry(int offset, int length)
                {
                    Offset = offset;
                    Length = length;
                    Order = 0;
                }

                public int CompareTo(UOPEntry other)
                {
                    return Order.CompareTo(other.Order);
                }
            }
        }
    }
}