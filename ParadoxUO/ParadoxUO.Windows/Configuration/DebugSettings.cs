#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// DebugSettings.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region Usings

#endregion

namespace OpenUO.Core.Configuration
{
    public sealed class DebugSettings : SettingsSectionBase
    {
        public const string SectionName = "Debug";
        private bool _isConsoleEnabled;
        private bool _logPackets;
        private bool _showDataRead;
        private bool _showDataReadBreakdown;
        private bool _showFps;
        private bool _showUiOutlines;
        private bool _tracePerformance;

        public bool TracePerformance
        {
            get { return _tracePerformance; }
            set { SetProperty(ref _tracePerformance, value); }
        }

        public bool LogPackets
        {
            get { return _logPackets; }
            set { SetProperty(ref _logPackets, value); }
        }

        public bool ShowUiOutlines
        {
            get { return _showUiOutlines; }
            set { SetProperty(ref _showUiOutlines, value); }
        }

        public bool ShowDataReadBreakdown
        {
            get { return _showDataReadBreakdown; }
            set { SetProperty(ref _showDataReadBreakdown, value); }
        }

        public bool ShowDataRead
        {
            get { return _showDataRead; }
            set { SetProperty(ref _showDataRead, value); }
        }

        public bool ShowFps
        {
            get { return _showFps; }
            set { SetProperty(ref _showFps, value); }
        }

        public bool IsConsoleEnabled
        {
            get { return _isConsoleEnabled; }
            set { SetProperty(ref _isConsoleEnabled, value); }
        }
    }
}