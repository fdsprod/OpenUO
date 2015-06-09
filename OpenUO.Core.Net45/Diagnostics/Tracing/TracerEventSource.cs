#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// TracerEventSource.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region Usings

using System.Diagnostics.Tracing;

#endregion

namespace OpenUO.Core.Diagnostics.Tracing
{
    internal sealed class TracerEventSource : EventSource
    {
        public static readonly TracerEventSource Instance = new TracerEventSource();

        [Event(TraceEventId.Critical, Level = EventLevel.Critical)]
        public void Critical(string message)
        {
            WriteEvent(TraceEventId.Critical, message);
        }

        [Event(TraceEventId.Error, Level = EventLevel.Error)]
        public void Error(string message)
        {
            WriteEvent(TraceEventId.Error, message);
        }

        [Event(TraceEventId.Info, Level = EventLevel.Informational)]
        public void Info(string message)
        {
            WriteEvent(TraceEventId.Info, message);
        }

        [Event(TraceEventId.Verbose, Level = EventLevel.Verbose)]
        public void Verbose(string message)
        {
            WriteEvent(TraceEventId.Verbose, message);
        }

        [Event(TraceEventId.Warning, Level = EventLevel.Warning)]
        public void Warn(string message)
        {
            WriteEvent(TraceEventId.Warning, message);
        }
    }
}