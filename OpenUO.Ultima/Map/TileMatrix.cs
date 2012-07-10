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

using System.IO;

namespace OpenUO.Ultima
{
    public class TileMatrix
    {
        private static InstallLocation _install;
        private static HuedTileList[][] _hueTileLists;

        private readonly HuedTile[][][][][] _staticTiles;
        private readonly Tile[][][] _landTiles;

        private readonly Tile[] _invalidLandBlock;
        private readonly HuedTile[][][] _emptyStaticBlock;

        private readonly FileStream _map;

        private readonly FileStream _fileIndex;
        private readonly BinaryReader _reader;

        private readonly FileStream _staticsStream;

        private readonly int _blockWidth, _blockHeight;
        private readonly int _width, _height;

        private readonly TileMatrixPatch _patch;

        public TileMatrixPatch Patch
        {
            get { return _patch; }
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

        public TileMatrix(InstallLocation install, int fileIndex, int mapID, int width, int height)
        {
            _install = install;
            _width = width;
            _height = height;
            _blockWidth = width >> 3;
            _blockHeight = height >> 3;

            if (fileIndex != 0x7F)
            {
                string mapPath = install.GetPath("map{0}.mul", fileIndex);
                string indexPath = install.GetPath("staidx{0}.mul", fileIndex);
                string staticsPath = install.GetPath("statics{0}.mul", fileIndex);   

                if (!File.Exists(mapPath))
                    mapPath = install.GetPath("map{0}LegacyMUL.uop", fileIndex);
                    
                _map = new FileStream(mapPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                _fileIndex = new FileStream(indexPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                _staticsStream = new FileStream(staticsPath, FileMode.Open, FileAccess.Read, FileShare.Read); 

                _reader = new BinaryReader(_fileIndex);   
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

            _landTiles = new Tile[_blockWidth][][];
            _staticTiles = new HuedTile[_blockWidth][][][][];

            _patch = new TileMatrixPatch(this, install, mapID);

            /*for ( int i = 0; i < m_BlockWidth; ++i )
            {
                m_LandTiles[i] = new Tile[m_BlockHeight][];
                m_StaticTiles[i] = new Tile[m_BlockHeight][][][];
            }*/
        }
        
        public void SetStaticBlock(int x, int y, HuedTile[][][] value)
        {
            if (x < 0 || y < 0 || x >= _blockWidth || y >= _blockHeight)
                return;

            if (_staticTiles[x] == null)
                _staticTiles[x] = new HuedTile[_blockHeight][][][];

            _staticTiles[x][y] = value;
        }

        public HuedTile[][][] GetStaticBlock(int x, int y)
        {
            if (x < 0 || y < 0 || x >= _blockWidth || y >= _blockHeight || _staticsStream == null || _fileIndex == null)
                return _emptyStaticBlock;

            if (_staticTiles[x] == null)
                _staticTiles[x] = new HuedTile[_blockHeight][][][];

            HuedTile[][][] tiles = _staticTiles[x][y] ?? (_staticTiles[x][y] = ReadStaticBlock(x, y));

            return tiles;
        }

        public HuedTile[] GetStaticTiles(int x, int y)
        {
            HuedTile[][][] tiles = GetStaticBlock(x >> 3, y >> 3);

            return tiles[x & 0x7][y & 0x7];
        }

        public void SetLandBlock(int x, int y, Tile[] value)
        {
            if (x < 0 || y < 0 || x >= _blockWidth || y >= _blockHeight)
                return;

            if (_landTiles[x] == null)
                _landTiles[x] = new Tile[_blockHeight][];

            _landTiles[x][y] = value;
        }

        public Tile[] GetLandBlock(int x, int y)
        {
            if (x < 0 || y < 0 || x >= _blockWidth || y >= _blockHeight || _map == null) return _invalidLandBlock;

            if (_landTiles[x] == null)
                _landTiles[x] = new Tile[_blockHeight][];

            Tile[] tiles = _landTiles[x][y] ?? (_landTiles[x][y] = ReadLandBlock(x, y));

            return tiles;
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

            StaticTile[] staTiles = new StaticTile[count];

            fixed (StaticTile* pTiles = staTiles)
            {
                NativeMethods._lread(_staticsStream.SafeFileHandle, pTiles, length);

                if (_hueTileLists == null)
                {
                    _hueTileLists = new HuedTileList[8][];

                    for (int i = 0; i < 8; ++i)
                    {
                        _hueTileLists[i] = new HuedTileList[8];

                        for (int j = 0; j < 8; ++j)
                            _hueTileLists[i][j] = new HuedTileList();
                    }
                }

                HuedTileList[][] lists = _hueTileLists;

                StaticTile* pCur = pTiles, pEnd = pTiles + count;

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
                        tiles[i][j] = lists[i][j].ToArray();
                }

                return tiles;
            }
        }

        private unsafe Tile[] ReadLandBlock(int x, int y)
        {
            int offset = ((x * _blockHeight) + y) * 196 + 4;

            if (_install.IsUOPFormat)
            {
                int block = offset / 0xC4000;
                offset += 0xD88 + (0xD54 * (block / 100)) + (12 * block);
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
                _map.Close();

            if (_staticsStream != null)
                _staticsStream.Close();

            if (_reader != null)
                _reader.Close();
        }
    }
}
