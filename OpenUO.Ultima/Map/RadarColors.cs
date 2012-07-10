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
    public unsafe class RadarColors
    {
        private InstallLocation _install;
        private short[] _colors;

        public int Length
        {
            get { return _colors.Length; }
        }

        public RadarColors(InstallLocation install)
        {
            _install = install;
            Load();
        }

        private void Load()
        {
            string path = _install.GetPath("radarcol.mul");

            if (!File.Exists(path))
                return;

            FileInfo file = new FileInfo(path);
            _colors = new short[file.Length / 2];

            fixed (short* ptr = _colors)
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    NativeMethods._lread(fs.SafeFileHandle, ptr, 0x10000);
            }
        }

        private void Save()
        {
            string path = _install.GetPath("radarcol.mul");

            fixed (short* ptr = _colors)
            {
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                    NativeMethods._lwrite(fs.SafeFileHandle, ptr, _colors.Length);
            }
        }

        public short this[int index]
        {
            get { return _colors[index]; }
            set { _colors[index] = value; }
        }
    }
}
