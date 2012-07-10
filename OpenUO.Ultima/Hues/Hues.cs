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
    public class Hues
    {
        private readonly Hue[] _hues;

        public Hue[] Table { get { return _hues; } }

        public Hues(InstallLocation install)
        {
            string path = install.GetPath("hues.mul");
            int index = 0;

            _hues = new Hue[3000];

            if (path != null)
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    BinaryReader bin = new BinaryReader(fs);

                    int blockCount = (int)fs.Length / 708;

                    if (blockCount > 375)
                        blockCount = 375;

                    for (int i = 0; i < blockCount; ++i)
                    {
                        bin.ReadInt32();

                        for (int j = 0; j < 8; ++j, ++index)
                            _hues[index] = new Hue(index, bin);
                    }
                }
            }

            for (; index < 3000; ++index)
                _hues[index] = new Hue(index);
        }

        public Hue GetHue(int index)
        {
            index &= 0x3FFF;

            if (index >= 0 && index < 3000)
                return _hues[index];

            return _hues[0];
        }
    }
}
