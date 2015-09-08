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
        private string _clientVersion;
        private string _dataDirectory;

        public UltimaOnlineSettings()
        {
            ClientVersion = "6.0.6.2";
        }

        public string ClientVersion
        {
            get { return _clientVersion; }
            set { SetProperty(ref _clientVersion, value); }
        }

        public string DataDirectory
        {
            get { return _dataDirectory; }
            set { SetProperty(ref _dataDirectory, value); }
        }

        protected override void UpdateVersionValues()
        {
            base.UpdateVersionValues();

            if (ClientVersion == null)
            {
                ClientVersion = "6.0.6.2";
            }
        }
    }
}