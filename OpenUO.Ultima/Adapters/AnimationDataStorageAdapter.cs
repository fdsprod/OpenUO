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
using System.Collections.Generic;
using System.IO;

namespace OpenUO.Ultima.Adapters
{
    public class AnimationDataStorageAdapter : StorageAdapterBase, IAnimationDataStorageAdapter<AnimationData>
    {
        private AnimationData[] _animationData;

        public override void Initialize()
        {
            base.Initialize();

            var install = Install;

            List<AnimationData> animationData = new List<AnimationData>();

            using (FileStream stream = File.Open(install.GetPath("animdata.mul"), FileMode.Open))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                int totalBlocks = (int)(reader.BaseStream.Length / 548);

                for (int i = 0; i < totalBlocks; i++)
                {
                    int header = reader.ReadInt32();
                    byte[] frameData = reader.ReadBytes(64);

                    AnimationData animData = new AnimationData
                    {
                        FrameData = new sbyte[64],
                        Unknown = reader.ReadByte(),
                        FrameCount = reader.ReadByte(),
                        FrameInterval = reader.ReadByte(),
                        FrameStart = reader.ReadByte()
                    };

                    Buffer.BlockCopy(frameData, 0, animData.FrameData, 0, 64);
                    animationData.Add(animData);
                }
            }

            _animationData = animationData.ToArray();
        }
        
        public AnimationData GetAnimationData(int index)
        {
            if (index < _animationData.Length)
                return _animationData[index];

            return AnimationData.Empty;
        }
    }
}
