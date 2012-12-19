#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   Map.cs
//  *
//  *   This program is free software; you can redistribute it and/or modify
//  *   it under the terms of the GNU General Public License as published by
//  *   the Free Software Foundation; either version 3 of the License, or
//  *   (at your option) any later version.
//  ***************************************************************************/

#endregion

namespace OpenUO.Ultima
{
    public class Map
    {
        private readonly int m_FileIndex;
        private readonly int m_Height;
        private readonly int m_MapID;
        private readonly TileMatrix m_Tiles;
        private readonly int m_Width;

        public Map(InstallLocation install, int fileIndex, int mapID, int width, int height)
        {
            m_FileIndex = fileIndex;
            m_MapID = mapID;
            m_Width = width;
            m_Height = height;
            m_Tiles = new TileMatrix(install, m_FileIndex, m_MapID, m_Width, m_Height);
        }

        public bool LoadedMatrix
        {
            get { return (m_Tiles != null); }
        }

        public TileMatrix Tiles
        {
            get { return m_Tiles; }
        }

        public int Width
        {
            get { return m_Width; }
        }

        public int Height
        {
            get { return m_Height; }
        }

        public int NumberOfSectorsX
        {
            get { return m_Width / m_Tiles.BlockWidth; }
        }

        public int NumberOfSectorsY
        {
            get { return m_Height / m_Tiles.BlockHeight; }
        }
    }
}