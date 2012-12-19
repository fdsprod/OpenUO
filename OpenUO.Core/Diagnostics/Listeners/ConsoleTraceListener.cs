#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   ConsoleTraceListener.cs
//  *
//  *   This program is free software; you can redistribute it and/or modify
//  *   it under the terms of the GNU General Public License as published by
//  *   the Free Software Foundation; either version 3 of the License, or
//  *   (at your option) any later version.
//  ***************************************************************************/

#endregion

#region Usings

using System;

#endregion

namespace OpenUO.Core.Diagnostics
{
    public sealed class ConsoleTraceListener : TraceListener
    {
        protected override void OnTraceReceived(TraceMessage message)
        {
            ConsoleColor color = ConsoleColor.Gray;

            switch (message.Type)
            {
                case TraceLevels.Info:
                    color = ConsoleColor.White;
                    break;
                case TraceLevels.Warning:
                    color = ConsoleColor.Yellow;
                    break;
                case TraceLevels.Error:
                case TraceLevels.Fatal:
                    color = ConsoleColor.Red;
                    break;
                default:
                    break;
            }

            ConsoleHelper.PushColor(color);
            Console.WriteLine(message);
            ConsoleHelper.PopColor();
        }
    }
}