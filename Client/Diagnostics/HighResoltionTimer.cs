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

namespace Client.Diagnostics
{
    public static class HighResoltionTimer
    {
        public static readonly double Frequency;
        public static readonly double Rate;

        public static long Ticks
        {
            get
            {
                long ticks;
                QueryPerformanceCounter(out ticks);
                return ticks;
            }
        }

        static HighResoltionTimer()
        {
            long frequency;
            QueryPerformanceFrequency(out frequency);
            Frequency = (double)frequency;
            Rate = 1 / Frequency;
        }

        [DllImport("Kernel32.dll")]
        public static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        public static extern bool QueryPerformanceFrequency(out long lpFrequency);
    }
}
