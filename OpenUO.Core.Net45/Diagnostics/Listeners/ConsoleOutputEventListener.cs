#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// ConsoleOutputEventListener.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region Usings

using System;
using System.Diagnostics.Tracing;

#endregion

namespace OpenUO.Core.Diagnostics.Tracing.Listeners
{
    public class ConsoleOutputEventListener : EventListener
    {
        protected override void OnEventWritten(EventWrittenEventArgs e)
        {
            var color = ConsoleColor.Gray;

            switch (e.Level)
            {
                case EventLevel.Informational:
                    color = ConsoleColor.White;
                    break;
                case EventLevel.Warning:
                    color = ConsoleColor.Yellow;
                    break;
                case EventLevel.Error:
                case EventLevel.Critical:
                    color = ConsoleColor.Red;
                    break;
            }

            ConsoleManager.PushColor(color);
            Console.WriteLine(e.Payload[0]);
            ConsoleManager.PopColor();
        }
    }
}