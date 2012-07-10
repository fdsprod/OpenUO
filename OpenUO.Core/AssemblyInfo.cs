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
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace OpenUO.Core
{
    public sealed class AssemblyInfo
    {
        public AssemblyInfo(Assembly assembly)
        {
            Assembly = assembly;
        }

        public Assembly Assembly
        {
            get;
            private set;
        }

        public string BaseDirectory
        {
            get { return Path.GetDirectoryName(AssemblyPath); }
        }

        public string AssemblyPath
        {
            get { return Assembly.Location; }
        }

        public bool Is64Bit
        {
            get { return (IntPtr.Size == 8); }
        }

        public Process Process
        {
            get { return Process.GetCurrentProcess(); }
        }

        public Version Version
        {
            get { return Assembly.GetName().Version; }
        }
    }
}
