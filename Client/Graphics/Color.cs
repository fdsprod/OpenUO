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

using System.Runtime.InteropServices;

namespace Client.Graphics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Color
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public Color(byte a, byte b, byte g, byte r)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public static implicit operator System.Drawing.Color(Color a)
        {
            return System.Drawing.Color.FromArgb(a.A, a.B, a.G, a.G);
        }

        public static implicit operator Color(System.Drawing.Color a)
        {
            return new Color(a.A, a.B, a.G, a.R);
        }
    }
}
