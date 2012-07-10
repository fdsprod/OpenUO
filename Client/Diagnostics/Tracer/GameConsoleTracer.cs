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

using Client.Components;
using OpenUO.Core.Patterns;
using OpenUO.Core.Diagnostics;

namespace Client.Diagnostics.Tracer
{
    public class GameConsoleTracer : TraceListener
    {
        private IConsole _console;

        public GameConsoleTracer(IoCContainer container)
        {
            _console = container.Resolve<IConsole>();
        }

        protected override void OnTraceReceived(TraceMessage message)
        {
            _console.WriteLine("{0}: {1}", message.Type, message.Message);
        }
    }
}
