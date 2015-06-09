#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// GameSettings.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region Usings

using SiliconStudio.Core.Mathematics;
using SiliconStudio.Paradox.Input;

#endregion

namespace OpenUO.Core.Configuration
{
    public sealed class GameSettings : SettingsSectionBase
    {
        public const string SectionName = "Game";
        private bool _alwaysRun;
        private bool _autoSelectLastCharacter;
        private Resolution _gameResolution;
        private Point _gameWindowPosition;
        private bool _isFixedTimeStep;
        private bool _isVSyncEnabled;
        private string _lastCharacterName;
        private Mouse _mouse;
        private Resolution _resolution;

        public GameSettings()
        {
            Resolution = new Resolution(1024, 768);
            GameResolution = new Resolution(800, 600);
            GameWindowPosition = new Point(0, 0);
            Mouse = new Mouse(MouseButton.Left, MouseButton.Right);
        }

        public Point GameWindowPosition
        {
            get { return _gameWindowPosition; }
            set { SetProperty(ref _gameWindowPosition, value); }
        }

        public bool IsFixedTimeStep
        {
            get { return _isFixedTimeStep; }
            set { SetProperty(ref _isFixedTimeStep, value); }
        }

        public Mouse Mouse
        {
            get { return _mouse; }
            set { SetProperty(ref _mouse, value); }
        }

        public Resolution Resolution
        {
            get { return _resolution; }
            set { SetProperty(ref _resolution, value); }
        }

        public Resolution GameResolution
        {
            get { return _gameResolution; }
            set { SetProperty(ref _gameResolution, value); }
        }

        public bool IsVSyncEnabled
        {
            get { return _isVSyncEnabled; }
            set { SetProperty(ref _isVSyncEnabled, value); }
        }

        public bool AlwaysRun
        {
            get { return _alwaysRun; }
            set { SetProperty(ref _alwaysRun, value); }
        }

        public string LastCharacterName
        {
            get { return _lastCharacterName; }
            set { SetProperty(ref _lastCharacterName, value); }
        }

        public bool AutoSelectLastCharacter
        {
            get { return _autoSelectLastCharacter; }
            set { SetProperty(ref _autoSelectLastCharacter, value); }
        }
    }
}