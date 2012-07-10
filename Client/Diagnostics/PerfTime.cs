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
    public struct PerfTime
    {
        private readonly long _ticks;

        public long Ticks
        {
            get { return _ticks; }
        }

        public static PerfTime Now
        {
            get { return new PerfTime(HighResoltionTimer.Ticks); }
        }

        public PerfTime(long ticks)
        {
            _ticks = ticks;
        }

        public static PerfTimeSpan operator -(PerfTime left, PerfTime right)
        {
            return new PerfTimeSpan(left.Ticks - right.Ticks);
        }

        public static PerfTime operator -(PerfTime left, PerfTimeSpan right)
        {
            return new PerfTime(left.Ticks - right.Ticks);
        }

        public static PerfTime operator +(PerfTime left, PerfTimeSpan right)
        {
            return new PerfTime(left.Ticks + right.Ticks);
        }
    }
}
