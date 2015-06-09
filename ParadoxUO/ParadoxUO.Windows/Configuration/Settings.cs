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
        private static readonly Settings m_Instance;
        private static readonly SettingsFile m_File;
        private DebugSettings m_Debug;
        private GameSettings m_Game;
        private ServerSettings m_Server;
        private UltimaOnlineSettings m_UltimaOnline;

        static Settings()
        {
            m_File = new SettingsFile("settings.json");
            m_Instance = new Settings();

            m_Instance.m_Debug = CreateOrOpenSection<DebugSettings>(DebugSettings.SectionName);
            m_Instance.m_Server = CreateOrOpenSection<ServerSettings>(ServerSettings.SectionName);
            m_Instance.m_UltimaOnline = CreateOrOpenSection<UltimaOnlineSettings>(UltimaOnlineSettings.SectionName);
            m_Instance.m_Game = CreateOrOpenSection<GameSettings>(GameSettings.SectionName);
        }

        public static bool IsSettingsFileCreated
        {
            get { return m_File.Exists; }
        }

        public static DebugSettings Debug
        {
            get { return m_Instance.m_Debug; }
        }

        public static ServerSettings Server
        {
            get { return m_Instance.m_Server; }
        }

        public static UltimaOnlineSettings UltimaOnline
        {
            get { return m_Instance.m_UltimaOnline; }
        }

        public static GameSettings Game
        {
            get { return m_Instance.m_Game; }
        }

        internal static void Save()
        {
            m_File.Save();
        }

        public static T CreateOrOpenSection<T>(string sectionName)
            where T : SettingsSectionBase, new()
        {
            var section = m_File.CreateOrOpenSection<T>(sectionName);

            // Resubscribe incase this is called for a section 2 times.
            section.Invalidated -= OnSectionInvalidated;
            section.Invalidated += OnSectionInvalidated;
            section.PropertyChanged -= OnSectionPropertyChanged;
            section.PropertyChanged += OnSectionPropertyChanged;

            return section;
        }

        private static void OnSectionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            m_File.InvalidateDirty();
        }

        private static void OnSectionInvalidated(object sender, EventArgs e)
        {
            m_File.InvalidateDirty();
        }
    }
}