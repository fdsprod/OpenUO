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


namespace Client.Diagnostics
{
    public struct PerfTimeSpan
    {
        private long _ticks;

        public long Ticks
        {
            get { return _ticks; }
        }

        public double Seconds
        {
            get { return (double)_ticks * HighResoltionTimer.Rate; }
        }

        public double Milliseconds
        {
            get { return Seconds * 1000; }
        }

        public double Microseconds
        {
            get { return Seconds * 1000000; }
        }

        public double Nanoseconds
        {
            get { return Seconds * 1000000000; }
        }

        public PerfTimeSpan(long ticks)
        {
            _ticks = ticks;
        }

        public static PerfTimeSpan operator +(PerfTimeSpan left, PerfTimeSpan right)
        {
            return new PerfTimeSpan(left.Ticks + right.Ticks);
        }

        public static PerfTimeSpan operator -(PerfTimeSpan left, PerfTimeSpan right)
        {
            return new PerfTimeSpan(left.Ticks - right.Ticks);
        }
    }
}
