#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// DebugOutputEventListener.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region Usings

using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;

#endregion

namespace OpenUO.Core.Diagnostics.Tracing.Listeners
{
    public class DebugOutputEventListener : EventListener
    {
        private const string Format = "{0} {1:yyyy-MM-dd HH\\:mm\\:ss\\:ffff} {2}";

        protected override void OnEventWritten(EventWrittenEventArgs e)
        {
            var output = string.Format(Format, e.Level, DateTime.Now, e.Payload[0]);
            Debug.WriteLine(output);
        }
    }
}