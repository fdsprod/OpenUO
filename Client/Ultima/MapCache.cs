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

using System;
using Client.Graphics;
using OpenUO.Core.Collections;
using OpenUO.Ultima;
using SharpDX;

namespace Client.Ultima
{
    public class MapNodeCache : Cache<int, MapNode>
    {
        public MapNodeCache(int width, int height)
            : base(TimeSpan.FromMinutes(1), width * height)
        {

        }

        public MapNode this[int x, int y]
        {
            get
            {
                int index = x << 16 | y;
                return this[index];
            }
            set
            {
                int index = x << 16 | y;
                this[index] = value;
            }
        }
    }

    public class MapNode
    {
        private bool _needsSorting;
        private readonly Tile[] Tiles;
        private HuedTile[][][] _huedTiles;
        private World _world;
        private Vector2 _position;

        private MapNode _eastNode;
        private MapNode _southNode;

        public MapNode(World world, Vector2 position, Tile[] tiles, HuedTile[][][] huedTiles)
        {
            _world = world;
            _position = position;
            Tiles = tiles;
            _huedTiles = huedTiles;
        }

        public unsafe void Draw(DrawState state)
        {
            //float centerZ = state.Camera.AttachedTo.Position.Z;

            //const float zStep = 1.0f / 44.0f;

            //for (int y = 0; y < 8; y++)
            //    for (int x = 0; x < 8; x++)
            //    {
                    //Tile tile = Tiles[x + (y * 8)];
                    //Tile eastTile;
                    //Tile southTile;
                    //Tile southEastTile;

                    //if (x == 7)
                    //    eastTile = _eastNode.Tiles[(y * 8)];
                    //else
                    //    eastTile = Tiles[x + 1 + (y * 8)];

                    //if (y == 7)
                    //    southTile = _eastNode.Tiles[x];
                    //else
                    //    southTile = Tiles[x + ((y + 1) * 8)];

                    //if (y == 7)
                    //    southEastTile = _eastNode.Tiles[x];
                    //else
                    //    southEastTile = Tiles[x + ((y + 1) * 8)];

                    //float tileZ = ((tile.Z * 4) * zStep);// -centerZ;
                    //int eastTileZ = (eastTile._z * 4) - centerZ;
                    //int southTileZ = (southTile._z * 4) - centerZ;
                    //int southEastTileZ = (southEastTile._z * 4) - centerZ;

                    //FragmentLocation location;
                    //state.CurrentStream.Allocate(VertexFragment.Quad, out location);

                    //VertexPositionNormalTexture* vPtr = (VertexPositionNormalTexture*)location.Vertices;
                    //ushort* iPtr = (ushort*)location.Vertices;

                    //*iPtr++ = (ushort)(location.BaseIndex + 0);
                    //*iPtr++ = (ushort)(location.BaseIndex + 1);
                    //*iPtr++ = (ushort)(location.BaseIndex + 2);
                    //*iPtr++ = (ushort)(location.BaseIndex + 2);
                    //*iPtr++ = (ushort)(location.BaseIndex + 1);
                    //*iPtr = (ushort)(location.BaseIndex + 3);

                    //vPtr->Position.X = _position.X;
                    //vPtr->Position.Y = _position.Y - tileZ;
                    //vPtr++;
                    //vPtr->Position.X = _position.X + 1;
                    //vPtr->Position.Y = _position.Y - tileZ;
                    //vPtr++;
                    //vPtr->Position.X = _position.X;
                    //vPtr->Position.Y = _position.Y + 1 - tileZ;
                    //vPtr++;
                    //vPtr->Position.X = _position.X + 1;
                    //vPtr->Position.Y = _position.Y + 1 - tileZ;
                //}
        }
    }
}
