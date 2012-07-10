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
    public class TileMatrixPatch
    {
        private readonly int _landBlocks, _staticBlocks;

        public int LandBlocks
        {
            get { return _landBlocks; }
        }

        public int StaticBlocks
        {
            get { return _staticBlocks; }
        }

        public TileMatrixPatch(TileMatrix matrix, InstallLocation install, int index)
        {
            string mapDataPath = install.GetPath("mapdif{0}.mul", index);
            string mapIndexPath = install.GetPath("mapdifl{0}.mul", index);

            if (File.Exists(mapDataPath) && File.Exists(mapIndexPath))
                _landBlocks = PatchLand(matrix, mapDataPath, mapIndexPath);

            string staDataPath = install.GetPath("stadif{0}.mul", index);
            string staIndexPath = install.GetPath("stadifl{0}.mul", index);
            string staLookupPath = install.GetPath("stadifi{0}.mul", index);

            if (File.Exists(staDataPath) && File.Exists(staIndexPath) && File.Exists(staLookupPath))
                _staticBlocks = PatchStatics(matrix, staDataPath, staIndexPath, staLookupPath);
        }

        private static unsafe int PatchLand(TileMatrix matrix, string dataPath, string indexPath)
        {
            using (FileStream fsData = new FileStream(dataPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (FileStream fsIndex = new FileStream(indexPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BinaryReader indexReader = new BinaryReader(fsIndex);

                int count = (int)(indexReader.BaseStream.Length / 4);

                for (int i = 0; i < count; ++i)
                {
                    int blockID = indexReader.ReadInt32();
                    int x = blockID / matrix.BlockHeight;
                    int y = blockID % matrix.BlockHeight;

                    fsData.Seek(4, SeekOrigin.Current);

                    Tile[] tiles = new Tile[64];

                    fixed (Tile* pTiles = tiles)
                        NativeMethods._lread(fsData.SafeFileHandle, pTiles, 192);

                    matrix.SetLandBlock(x, y, tiles);
                }

                return count;
            }
        }

        private static unsafe int PatchStatics(TileMatrix matrix, string dataPath, string indexPath, string lookupPath)
        {
            using (FileStream fsData = new FileStream(dataPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (FileStream fsIndex = new FileStream(indexPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (FileStream fsLookup = new FileStream(lookupPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BinaryReader indexReader = new BinaryReader(fsIndex);
                BinaryReader lookupReader = new BinaryReader(fsLookup);

                int count = (int)(indexReader.BaseStream.Length / 4);

                HuedTileList[][] lists = new HuedTileList[8][];

                for (int x = 0; x < 8; ++x)
                {
                    lists[x] = new HuedTileList[8];

                    for (int y = 0; y < 8; ++y)
                        lists[x][y] = new HuedTileList();
                }

                for (int i = 0; i < count; ++i)
                {
                    int blockID = indexReader.ReadInt32();
                    int blockX = blockID / matrix.BlockHeight;
                    int blockY = blockID % matrix.BlockHeight;

                    int offset = lookupReader.ReadInt32();
                    int length = lookupReader.ReadInt32();
                    lookupReader.ReadInt32(); // Extra

                    if (offset < 0 || length <= 0)
                    {
                        matrix.SetStaticBlock(blockX, blockY, matrix.EmptyStaticBlock);
                        continue;
                    }

                    fsData.Seek(offset, SeekOrigin.Begin);

                    int tileCount = length / 7;

                    StaticTile[] staTiles = new StaticTile[tileCount];

                    fixed (StaticTile* pTiles = staTiles)
                    {
                        NativeMethods._lread(fsData.SafeFileHandle, pTiles, length);

                        StaticTile* pCur = pTiles, pEnd = pTiles + tileCount;

                        while (pCur < pEnd)
                        {
                            lists[pCur->X & 0x7][pCur->Y & 0x7].Add((short)((pCur->Id & 0x3FFF) + 0x4000), pCur->Hue, pCur->Z);
                            ++pCur;
                        }

                        HuedTile[][][] tiles = new HuedTile[8][][];

                        for (int x = 0; x < 8; ++x)
                        {
                            tiles[x] = new HuedTile[8][];

                            for (int y = 0; y < 8; ++y)
                                tiles[x][y] = lists[x][y].ToArray();
                        }

                        matrix.SetStaticBlock(blockX, blockY, tiles);
                    }
                }

                return count;
            }
        }
    }
}
