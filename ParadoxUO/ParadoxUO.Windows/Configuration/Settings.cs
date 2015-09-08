#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// Settings.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region Usings

using System;
using System.ComponentModel;

#endregion

namespace OpenUO.Core.Configuration
{
    public sealed class Settings
    {
        private static readonly Settings _instance;
        private static readonly SettingsFile _file;
        private DebugSettings _debug;
        private GameSettings _game;
        private ServerSettings _server;
        private UltimaOnlineSettings _ultimaOnline;

        static Settings()
        {
            _file = new SettingsFile("settings.json");
            _instance = new Settings();

            _instance._debug = CreateOrOpenSection<DebugSettings>(DebugSettings.SectionName);
            _instance._server = CreateOrOpenSection<ServerSettings>(ServerSettings.SectionName);
            _instance._ultimaOnline = CreateOrOpenSection<UltimaOnlineSettings>(UltimaOnlineSettings.SectionName);
            _instance._game = CreateOrOpenSection<GameSettings>(GameSettings.SectionName);
        }

        public static bool IsSettingsFileCreated
        {
            get { return _file.Exists; }
        }

        public static DebugSettings Debug
        {
            get { return _instance._debug; }
        }

        public static ServerSettings Server
        {
            get { return _instance._server; }
        }

        public static UltimaOnlineSettings UltimaOnline
        {
            get { return _instance._ultimaOnline; }
        }

        public static GameSettings Game
        {
            get { return _instance._game; }
        }

        internal static void Save()
        {
            _file.Save();
        }

        public static T CreateOrOpenSection<T>(string sectionName)
            where T : SettingsSectionBase, new()
        {
            var section = _file.CreateOrOpenSection<T>(sectionName);

            // Resubscribe incase this is called for a section 2 times.
            section.Invalidated -= OnSectionInvalidated;
            section.Invalidated += OnSectionInvalidated;
            section.PropertyChanged -= OnSectionPropertyChanged;
            section.PropertyChanged += OnSectionPropertyChanged;

            return section;
        }

        private static void OnSectionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _file.InvalidateDirty();
        }

        private static void OnSectionInvalidated(object sender, EventArgs e)
        {
            _file.InvalidateDirty();
        }
    }
}