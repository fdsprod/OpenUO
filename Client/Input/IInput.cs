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

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Client.Input
{
    public interface IInput
    {
        Dictionary<Keys, InputBinding> KeyBindings { get; }
        Dictionary<MouseButtons, InputBinding> MouseBindings { get; }

        event EventHandler<HandledKeyEventArgs> KeyDown;
        event EventHandler<HandledKeyEventArgs> KeyUp;
        event EventHandler<HandledMouseEventArgs> MouseClick;
        event EventHandler<HandledMouseEventArgs> MouseDoubleClick;
        event EventHandler<HandledMouseEventArgs> MouseDown;
        event EventHandler<HandledMouseEventArgs> MouseUp;
        event EventHandler<HandledMouseEventArgs> MouseMove;
        event EventHandler<HandledMouseEventArgs> MouseWheel;

        InputBinding AddBinding(string name, bool shift, bool control, bool alt, Keys key, EventHandler handler);
        InputBinding AddBinding(string name, bool shift, bool control, bool alt, Keys key, EventHandler beginHandler, EventHandler endHandler);
        InputBinding AddBinding(string name, MouseButtons buttons, EventHandler handler);
        InputBinding AddBinding(string name, MouseButtons buttons, EventHandler beginHandler, EventHandler endHandler);
    }
}
