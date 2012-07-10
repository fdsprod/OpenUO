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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Client.Graphics;
using OpenUO.Core.Patterns;
using SharpDX.Windows;

namespace Client.Input
{
    internal class InputManager : IInput
    {
        private readonly Dictionary<Keys, InputBinding> _keyBindings;
        private readonly Dictionary<MouseButtons, InputBinding> _mouseBindings;

        public static Keys ModifierKeys
        {
            get
            {
                Keys none = Keys.None;

                if (GetKeyState(0x10) < 0)
                    none |= Keys.Shift;

                if (GetKeyState(0x11) < 0)
                    none |= Keys.Control;

                if (GetKeyState(0x12) < 0)
                    none |= Keys.Alt;

                return none;
            }
        }

        public static MouseButtons MouseButtons
        {
            get
            {
                MouseButtons none = MouseButtons.None;

                if (GetKeyState(1) < 0)
                    none |= MouseButtons.Left;

                if (GetKeyState(2) < 0)
                    none |= MouseButtons.Right;

                if (GetKeyState(4) < 0)
                    none |= MouseButtons.Middle;

                if (GetKeyState(5) < 0)
                    none |= MouseButtons.XButton1;

                if (GetKeyState(6) < 0)
                    none |= MouseButtons.XButton2;

                return none;
            }
        }

        public Dictionary<Keys, InputBinding> KeyBindings
        {
            get { return _keyBindings; }
        }

        public Dictionary<MouseButtons, InputBinding> MouseBindings
        {
            get { return _mouseBindings; }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern short GetKeyState(int keyCode);

        public event EventHandler<HandledKeyEventArgs> KeyDown;
        public event EventHandler<HandledKeyEventArgs> KeyUp;

        public event EventHandler<HandledMouseEventArgs> MouseClick;
        public event EventHandler<HandledMouseEventArgs> MouseDoubleClick;
        public event EventHandler<HandledMouseEventArgs> MouseDown;
        public event EventHandler<HandledMouseEventArgs> MouseUp;
        public event EventHandler<HandledMouseEventArgs> MouseMove;
        public event EventHandler<HandledMouseEventArgs> MouseWheel;

        public InputManager(IoCContainer container)
        {
            IDeviceContextService deviceContextManager = container.Resolve<IDeviceContextService>();

            RenderForm form = deviceContextManager.Form;

            form.KeyDown += OnKeyDown;
            form.KeyUp += OnKeyUp;
            form.MouseClick += OnMouseClick;
            form.MouseDoubleClick += OnMouseDoubleClick;
            form.MouseDown += OnMouseDown;
            form.MouseMove += OnMouseMove;
            form.MouseUp += OnMouseUp;
            form.MouseWheel += OnMouseWheel;

            _keyBindings = new Dictionary<Keys, InputBinding>();
            _mouseBindings = new Dictionary<MouseButtons, InputBinding>();
        }

        public InputBinding AddBinding(string name, bool shift, bool control, bool alt, Keys key, EventHandler handler)
        {
            return AddBinding(name, shift, control, alt, key, handler, null);
        }

        public InputBinding AddBinding(string name, bool shift, bool control, bool alt, Keys key, EventHandler beginHandler, EventHandler endHandler)
        {
            InputBinding binding = new InputBinding(name, shift, control, alt);

            binding.BeginExecution = beginHandler;
            binding.EndExecution = endHandler;

            key |= shift ? Keys.Shift : Keys.None;
            key |= control ? Keys.Control : Keys.None;
            key |= alt ? Keys.Alt : Keys.None;

            KeyBindings.Add(key, binding);

            return binding;
        }

        public InputBinding AddBinding(string name, MouseButtons buttons, EventHandler handler)
        {
            return AddBinding(name, buttons, handler, null);
        }

        public InputBinding AddBinding(string name, MouseButtons buttons, EventHandler beginHandler, EventHandler endHandler)
        {
            InputBinding binding = new InputBinding(name, false, false, false);

            binding.BeginExecution = beginHandler;
            binding.EndExecution = endHandler;

            MouseBindings.Add(buttons, binding);

            return binding;
        }

        private void HandleKeyBindings()
        {
            foreach (Keys keys in KeyBindings.Keys)
            {
                Keys key = keys;
                InputBinding binding = KeyBindings[keys];

                Keys modifiers = binding.ModifierKeys;

                //Remove any modifiers so we can 
                //get the exact key...
                key = key & ~Keys.Shift;
                key = key & ~Keys.Alt;
                key = key & ~Keys.Control;

                binding.IsExecuting = ((GetKeyState((int)key) < 0) && ((ModifierKeys & modifiers) == modifiers));
            }
        }

        private void HandleMouseBindings()
        {
            foreach (MouseButtons button in MouseBindings.Keys)
                MouseBindings[button].IsExecuting = ((MouseButtons & button) == button);
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            HandledMouseEventArgs args = CreateArgs(e);

            var handler = MouseWheel;

            if (handler != null)
                handler(sender, args);
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            HandledMouseEventArgs args = CreateArgs(e);

            var handler = MouseUp;

            if (handler != null)
                handler(sender, args);

            HandleMouseBindings();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            HandledMouseEventArgs args = CreateArgs(e);

            var handler = MouseMove;

            if (handler != null)
                handler(sender, args);
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            HandledMouseEventArgs args = CreateArgs(e);

            var handler = MouseDown;

            if (handler != null)
                handler(sender, args);

            HandleMouseBindings();
        }

        private void OnMouseDoubleClick(object sender, MouseEventArgs e)
        {
            HandledMouseEventArgs args = CreateArgs(e);

            var handler = MouseDoubleClick;

            if (handler != null)
                handler(sender, args);
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            HandledMouseEventArgs args = CreateArgs(e);

            var handler = MouseClick;

            if (handler != null)
                handler(sender, args);
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            HandledKeyEventArgs args = CreateArgs(e);

            var handler = KeyUp;

            if (handler != null)
                handler(sender, args);

            HandleKeyBindings();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            HandledKeyEventArgs args = CreateArgs(e);

            var handler = KeyDown;

            if (handler != null)
                handler(sender, args);

            HandleKeyBindings();
        }

        private static HandledMouseEventArgs CreateArgs(MouseEventArgs e)
        {
            return new HandledMouseEventArgs(e.Button, e.Clicks, e.X, e.Y, e.Delta, false);
        }

        private static HandledKeyEventArgs CreateArgs(KeyEventArgs e)
        {
            return new HandledKeyEventArgs(e.KeyData);
        }
    }
}
