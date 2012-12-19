#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   NativeMethods.cs
//  *
//  *   This program is free software; you can redistribute it and/or modify
//  *   it under the terms of the GNU General Public License as published by
//  *   the Free Software Foundation; either version 3 of the License, or
//  *   (at your option) any later version.
//  ***************************************************************************/

#endregion

#region Usings

using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

#endregion

namespace OpenUO.Ultima
{
    public static class NativeMethods
    {
        [DllImport("Kernel32")]
        public static extern unsafe int _lread(SafeFileHandle fileHandle, void* buffer, int length);

        [DllImport("container32.dll")]
        public static extern unsafe int _lwrite(SafeFileHandle fileHandle, void* buffer, int length);
    }
}