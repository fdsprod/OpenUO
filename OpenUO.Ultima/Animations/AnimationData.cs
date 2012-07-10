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


namespace OpenUO.Ultima
{
    public struct AnimationData
    {
        public static readonly AnimationData Empty = new AnimationData() { FrameData = new sbyte[0], FrameStart = 0, FrameInterval = 0, FrameCount = 0 };

        public sbyte[] FrameData;
        public byte Unknown;
        public byte FrameCount;
        public byte FrameInterval;
        public byte FrameStart;
    }
}
