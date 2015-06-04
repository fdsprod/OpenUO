#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   TileMatrixPatch.cs
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
    public class TileMatrixPatch
    {
        private readonly int _landBlocks, _staticBlocks;

        public TileMatrixPatch(TileMatrix matrix, InstallLocation install, int index)
        {
            var mapDataPath = install.GetPath("mapdif{0}.mul", index);
            var mapIndexPath = install.GetPath("mapdifl{0}.mul", index);

            if(File.Exists(mapDataPath) && File.Exists(mapIndexPath))
            {
                _landBlocks = PatchLand(matrix, mapDataPath, mapIndexPath);
            }

            var staDataPath = install.GetPath("stadif{0}.mul", index);
            var staIndexPath = install.GetPath("stadifl{0}.mul", index);
            var staLookupPath = install.GetPath("stadifi{0}.mul", index);

            if(File.Exists(staDataPath) && File.Exists(staIndexPath) && File.Exists(staLookupPath))
            {
                _staticBlocks = PatchStatics(matrix, staDataPath, staIndexPath, staLookupPath);
            }
        }

        public int LandBlocks
        {
            get { return _landBlocks; }
        }

        public int StaticBlocks
        {
            get { return _staticBlocks; }
        }

        private static unsafe int PatchLand(TileMatrix matrix, string dataPath, string indexPath)
        {
            using(var fsData = new FileStream(dataPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using(var fsIndex = new FileStream(indexPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var indexReader = new BinaryReader(fsIndex);

                    var count = (int)(indexReader.BaseStream.Length / 4);

                    for(var i = 0; i < count; ++i)
                    {
                        var blockID = indexReader.ReadInt32();
                        var x = blockID / matrix.BlockHeight;
                        var y = blockID % matrix.BlockHeight;

                        fsData.Seek(4, SeekOrigin.Current);

                        var tiles = new Tile[64];

                        fixed(Tile* pTiles = tiles)
                        {
                            NativeMethods._lread(fsData.SafeFileHandle, pTiles, 192);
                        }

                        //matrix.SetLandBlock(x, y, tiles);
                    }

                    return count;
                }
            }
        }

        private static unsafe int PatchStatics(TileMatrix matrix, string dataPath, string indexPath, string lookupPath)
        {
            using(var fsData = new FileStream(dataPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using(var fsIndex = new FileStream(indexPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using(var fsLookup = new FileStream(lookupPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        var indexReader = new BinaryReader(fsIndex);
                        var lookupReader = new BinaryReader(fsLookup);

                        var count = (int)(indexReader.BaseStream.Length / 4);

                        var lists = new HuedTileList[8][];

                        for(var x = 0; x < 8; ++x)
                        {
                            lists[x] = new HuedTileList[8];

                            for(var y = 0; y < 8; ++y)
                            {
                                lists[x][y] = new HuedTileList();
                            }
                        }

                        for(var i = 0; i < count; ++i)
                        {
                            var blockID = indexReader.ReadInt32();
                            var blockX = blockID / matrix.BlockHeight;
                            var blockY = blockID % matrix.BlockHeight;

                            var offset = lookupReader.ReadInt32();
                            var length = lookupReader.ReadInt32();
                            lookupReader.ReadInt32(); // Extra

                            if(offset < 0 || length <= 0)
                            {
                                //matrix.SetStaticBlock(blockX, blockY, matrix.EmptyStaticBlock);
                                continue;
                            }

                            fsData.Seek(offset, SeekOrigin.Begin);

                            var tileCount = length / 7;

                            var staTiles = new StaticTileData[tileCount];

                            fixed(StaticTileData* pTiles = staTiles)
                            {
                                NativeMethods._lread(fsData.SafeFileHandle, pTiles, length);

                                StaticTileData* pCur = pTiles, pEnd = pTiles + tileCount;

                                while(pCur < pEnd)
                                {
                                    lists[pCur->X & 0x7][pCur->Y & 0x7].Add((short)((pCur->Id & 0x3FFF) + 0x4000), pCur->Hue, pCur->Z);
                                    ++pCur;
                                }

                                var tiles = new HuedTile[8][][];

                                for(var x = 0; x < 8; ++x)
                                {
                                    tiles[x] = new HuedTile[8][];

                                    for(var y = 0; y < 8; ++y)
                                    {
                                        tiles[x][y] = lists[x][y].ToArray();
                                    }
                                }

                                //matrix.SetStaticBlock(blockX, blockY, tiles);
                            }
                        }

                        return count;
                    }
                }
            }
        }
    }
}