#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// Macro.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region Usings

using System;

using OpenUO.Core.ComponentModel;

using SiliconStudio.Paradox.Input;

#endregion

namespace OpenUO.Core.Configuration
{
    public sealed class Macro : NotifiableBase
    {
        private bool _alt;
        private bool _control;
        private Keys _key;
        private bool _shift;

        public Keys Key
        {
            get { return _key; }
            set { SetProperty(ref _key, value); }
        }

        public bool Shift
        {
            get { return _shift; }
            set { SetProperty(ref _shift, value); }
        }

        public bool Alt
        {
            get { return _alt; }
            set { SetProperty(ref _alt, value); }
        }

        public bool Control
        {
            get { return _control; }
            set { SetProperty(ref _control, value); }
        }

        public override int GetHashCode()
        {
            return Tuple.Create(Control, Alt, Shift, Key).GetHashCode();
        }
    }
}