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
using System.Windows.Forms;

namespace Client.Input
{
    public class InputBinding
    {
        private static object _syncRoot = new object();

        private string _name;
        private bool _shift;
        private bool _alt;
        private bool _control;
        private bool _isExecuting;

        public Keys ModifierKeys
        {
            get
            {
                Keys modifiers = Keys.None;

                modifiers |= _shift ? Keys.Shift : modifiers;
                modifiers |= _alt ? Keys.Alt : modifiers;
                modifiers |= _control ? Keys.Control : modifiers;

                return modifiers;
            }
        }

        public bool Shift
        {
            get { return _shift; }
            set { _shift = value; }
        }

        public bool Alt
        {
            get { return _alt; }
            set { _alt = value; }
        }

        public bool Control
        {
            get { return _control; }
            set { _control = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public bool IsExecuting
        {
            get { return _isExecuting; }
            internal set
            {
                if (value && !_isExecuting)
                {
                    OnBeginExecution();
                }
                else if (value && _isExecuting && EndExecution == null)
                {
                    OnBeginExecution();
                }
                else if (!value && _isExecuting)
                {
                    OnEndExecution();
                }

                _isExecuting = value;
            }
        }

        public EventHandler BeginExecution;
        public EventHandler EndExecution;

        public InputBinding(string name, bool shift, bool control, bool alt)
        {
            _name = name;
            _shift = shift;
            _alt = alt;
            _control = control;
        }

        protected virtual void OnBeginExecution()
        {
            if (BeginExecution != null)
            {
                BeginExecution(this, EventArgs.Empty);
            }
        }

        protected virtual void OnEndExecution()
        {
            if (EndExecution != null)
            {
                EndExecution(this, EventArgs.Empty);
            }
        }
    }
}
