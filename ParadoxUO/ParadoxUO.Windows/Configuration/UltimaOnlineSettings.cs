#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// UltimaOnlineSettings.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

namespace OpenUO.Core.Configuration
{
    public sealed class UltimaOnlineSettings : SettingsSectionBase
    {
        public const string SectionName = "UltimaOnline";
        private byte[] m_ClientVersion;
        private string m_DataDirectory;

        public UltimaOnlineSettings()
        {
            ClientVersion = new byte[] {6, 0, 6, 2};
        }

        public byte[] ClientVersion
        {
            get { return m_ClientVersion; }
            set { SetProperty(ref m_ClientVersion, value); }
        }

        public string DataDirectory
        {
            get { return m_DataDirectory; }
            set { SetProperty(ref m_DataDirectory, value); }
        }

        protected override void UpdateVersionValues()
        {
            base.UpdateVersionValues();

            if (ClientVersion == null)
            {
                ClientVersion = new byte[] {6, 0, 6, 2};
            }
        }
    }
}