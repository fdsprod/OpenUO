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
using OpenUO.Core.Configuration;

namespace Client.Configuration
{
    internal sealed class ConfigurationManager : IConfiguration
    {
        private readonly ConfigFile _configFile;

        public bool FileExists
        {
            get { return _configFile.Exists; }
        }

        public event EventHandler RestoreDefaultsInvoked;

        public ConfigurationManager()
        {
            string file = "config.xml";
            _configFile = new ConfigFile(file);
        }

        public void RestoreDefaults()
        {
            var handler = RestoreDefaultsInvoked;

            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public T GetValue<T>(string section, string key)
        {
            return _configFile.GetValue<T>(section, key);
        }

        public T GetValue<T>(string section, string key, T defaultValue)
        {
            return _configFile.GetValue<T>(section, key, defaultValue);
        }

        public void SetValue<T>(string section, string key, T value)
        {
            _configFile.SetValue(section, key, value);
        }
    }
}
