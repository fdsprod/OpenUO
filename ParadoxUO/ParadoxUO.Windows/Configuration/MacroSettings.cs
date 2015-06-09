#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// MacroSettings.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region Usings

using System.Collections.Generic;

#endregion

namespace OpenUO.Core.Configuration
{
    public sealed class MacroSettings : SettingsSectionBase
    {
        public const string Sectionname = "Macros";
        private Dictionary<MacroActions, Macro> _macro;

        public Dictionary<MacroActions, Macro> Macro
        {
            get { return _macro; }
            set { SetProperty(ref _macro, value); }
        }
    }
}