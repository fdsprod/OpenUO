#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// ServerSettings.cs
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
    public sealed class ServerSettings : SettingsSectionBase
    {
        public const string SectionName = "Server";
        private string _serverAddress;
        private int _serverPort;
        private string _userName;

        public ServerSettings()
        {
            ServerAddress = "127.0.0.1";
            ServerPort = 2593;
        }

        public string UserName
        {
            get { return _userName; }
            set { SetProperty(ref _userName, value); }
        }

        public int ServerPort
        {
            get { return _serverPort; }
            set { SetProperty(ref _serverPort, value); }
        }

        public string ServerAddress
        {
            get { return _serverAddress; }
            set { SetProperty(ref _serverAddress, value); }
        }
    }
}