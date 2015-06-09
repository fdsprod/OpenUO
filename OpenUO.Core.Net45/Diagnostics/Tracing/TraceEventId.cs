#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// TraceEventId.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

namespace OpenUO.Core.Diagnostics.Tracing
{
    public static class TraceEventId
    {
        public const int Verbose = 1;
        public const int Info = 2;
        public const int Warning = 3;
        public const int Error = 4;
        public const int Critical = 6;
    }
}