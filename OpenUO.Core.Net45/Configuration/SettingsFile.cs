#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// SettingsFile.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

using OpenUO.Core.Diagnostics.Tracing;

#endregion

namespace OpenUO.Core.Configuration
{
    public class SettingsFile
    {
        private static readonly object _syncRoot = new object();
        private readonly string _filename;
        private readonly Timer _saveTimer;
        private readonly Dictionary<string, SettingsSectionBase> _sectionCache;
        private Dictionary<string, JToken> _tokenCache;

        public SettingsFile(string filename)
        {
            _sectionCache = new Dictionary<string, SettingsSectionBase>();
            _tokenCache = new Dictionary<string, JToken>();
            _saveTimer = new Timer
            {
                Interval = 300,
                AutoReset = true
            };
            _saveTimer.Elapsed += OnTimerElapsed;
            _filename = filename;

            if (File.Exists(_filename))
            {
                try
                {
                    Load();
                }
                catch (Exception e)
                {
                    Tracer.Error(e, "Unable to load settings file {0}", _filename);
                }
            }
        }

        public bool Exists
        {
            get { return File.Exists(_filename); }
        }

        public void Save()
        {
            try
            {
                lock (_syncRoot)
                {
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        DateFormatHandling = DateFormatHandling.IsoDateFormat,
                        DateParseHandling = DateParseHandling.DateTimeOffset,
                        DateTimeZoneHandling = DateTimeZoneHandling.Local
                    };

                    var result = JsonConvert.SerializeObject(_sectionCache, Formatting.Indented, settings);

                    if (File.Exists(_filename))
                    {
                        File.Copy(_filename, _filename + ".bak", true);
                    }

                    File.WriteAllText(_filename, result);
                }
            }
            catch (Exception e)
            {
                Tracer.Error(e);
            }
        }

        public void InvalidateDirty()
        {
            //Lock the timer so we dont start it while its saving
            lock (_saveTimer)
            {
                _saveTimer.Stop();
                _saveTimer.Start();
            }
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // Lock the timer so we dont start it till its done save.
            lock (_saveTimer)
            {
                _saveTimer.Stop();

                Save();
            }
        }

        private void Load()
        {
            try
            {
                lock (_syncRoot)
                {
                    LoadFromFile(_filename);

                    if (_tokenCache == null && File.Exists(_filename + ".bak"))
                    {
                        Tracer.Error("Unable to read settings file.  Trying backup file");
                        LoadFromFile(_filename + ".bak");
                    }

                    if (_tokenCache == null)
                    {
                        Tracer.Error("Unable to read settings backup file.  Resettings all settings.");
                        _tokenCache = new Dictionary<string, JToken>();
                    }
                }
            }
            catch (Exception e)
            {
                Tracer.Error(e);
            }
        }

        private void LoadFromFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return;
            }
            var contents = File.ReadAllText(fileName);
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Converters = new List<JsonConverter> {new IsoDateTimeConverter()}
            };

            _tokenCache = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(contents, settings);
        }

        public T CreateOrOpenSection<T>(string sectionName)
            where T : SettingsSectionBase, new()
        {
            JToken token;
            SettingsSectionBase section;

            // We've already deserialized the section, so just return it.
            if (_sectionCache.TryGetValue(sectionName, out section))
            {
                return (T) section;
            }

            var isCached = _tokenCache.TryGetValue(sectionName, out token);

            if (isCached)
            {
                // We've haven't deserialized it but it exists, so read it in and save it to the local cache
                section = token.ToObject<T>();
                section.OnDeserialized();
            }
            else
            {
                // New section not saved in the file, create it and save it.
                section = new T();
                _tokenCache[sectionName] = JToken.FromObject(section);
                InvalidateDirty();
            }

            _sectionCache[sectionName] = section;

            return (T) section;
        }
    }
}