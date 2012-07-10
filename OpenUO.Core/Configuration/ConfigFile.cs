#region License Header
/***************************************************************************
 *   Copyright (c) 2011 OpenUO Software Team.
 *   All Right Reserved.
 *
 *   $Id: ConfigFile.cs 14 2011-10-31 07:03:12Z fdsprod@gmail.com $:
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 ***************************************************************************/
 #endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

using OpenUO.Core.Diagnostics;
using OpenUO.Core.IO;

namespace OpenUO.Core.Configuration
{
    public class ConfigFile
    {
        private static object _syncRoot = new object();

        private readonly string _filename;
        private readonly Dictionary<string, Dictionary<string, string>> _sections;

        public bool Exists { get { return File.Exists(_filename); } }

        public ConfigFile(string filename)
        {
            _filename = filename;
            _sections = new Dictionary<string, Dictionary<string, string>>();

            Reload();
        }

        private void LoadSettings()
        {
            Tracer.Info("Loading configuration...");

            _sections.Clear();

            try
            {
                lock (_syncRoot)
                {
                    if (!File.Exists(_filename))
                        return;

                    using (Stream stream = File.Open(_filename, FileMode.Open))
                    {
                        XDocument document = XDocument.Load(stream);

                        foreach (XElement section in document.Root.Descendants("section"))
                        {
                            try
                            {
                                Dictionary<string, string> sectionTable = new Dictionary<string, string>();
                                _sections.Add(section.Attribute("name").Value, sectionTable);

                                foreach (XElement element in section.DescendantNodes())
                                {
                                    try
                                    {
                                        sectionTable.Add(element.Attribute("name").Value, element.Attribute("value").Value);
                                    }
                                    catch (Exception e)
                                    {
                                        Tracer.Error(e);
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Tracer.Error(e);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Tracer.Error(e);
            }
        }

        private void SaveSettings()
        {
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();

                settings.CheckCharacters = false;
                settings.CloseOutput = true;
                settings.Encoding = Encoding.UTF8;
                settings.Indent = true;
                settings.NewLineHandling = NewLineHandling.Entitize;

                XDocument document = new XDocument();
                document.Add(new XElement("configuration"));

                XElement configuration = document.Element("configuration");

                foreach (var section in _sections)
                {
                    XElement xSection = new XElement("section", new XAttribute("name", section.Key));

                    foreach (var setting in section.Value)
                    {
                        xSection.Add(
                            new XElement("setting",
                                new XAttribute("name", setting.Key),
                                new XAttribute("value", setting.Value)));
                    }

                    configuration.Add(xSection);
                }

                lock (_syncRoot)
                {
                    string directory = Path.GetDirectoryName(_filename);

                    if(!string.IsNullOrEmpty(directory))
                        FileSystemHelper.EnsureDirectoryExists(directory);

                    using (Stream stream = File.Open(_filename, FileMode.Create))
                        document.Save(stream);
                }
            }
            catch (Exception e)
            {
                Tracer.Error(e);
            }
        }

        public void SetValue<T>(string section, string key, T value)
        {
            Dictionary<string, string> sectionTable;

            if (!_sections.TryGetValue(section, out sectionTable))
            {
                sectionTable = new Dictionary<string, string>();
                _sections.Add(section, sectionTable);
            }

            if (sectionTable.ContainsKey(key))
                sectionTable[key] = value.ToString();
            else
                sectionTable.Add(key, value.ConvertTo<string>());

            SaveSettings();
        }

        public T GetValue<T>(string section, string key)
        {
            return GetValue(section, key, default(T));
        }

        public T GetValue<T>(string section, string key, T defaultValue)
        {
            if (!_sections.ContainsKey(section))
                return defaultValue;

            Dictionary<string, string> sectionTable = _sections[section];

            return !sectionTable.ContainsKey(key) ? defaultValue : sectionTable[key].ConvertTo<T>();
        }

        public void Reload()
        {
            if (File.Exists(_filename))
                LoadSettings();
        }
    }
}
