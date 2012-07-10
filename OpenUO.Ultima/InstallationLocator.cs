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
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;
using OpenUO.Core.Diagnostics;

namespace OpenUO.Ultima
{
    public static class InstallationLocator
    {
        public static List<string> KnownInstallationRegistryKeys { get; private set; }

        static InstallationLocator()
        {
            KnownInstallationRegistryKeys = new List<string>() {
                @"Electronic Arts\EA Games\Ultima Online Stygian Abyss Classic",     
                @"Origin Worlds Online\Ultima Online\KR Legacy Beta", 
                @"EA Games\Ultima Online: Mondain's Legacy\1.00.0000", 
                @"Origin Worlds Online\Ultima Online\1.0", 
                @"Origin Worlds Online\Ultima Online Third Dawn\1.0",
                @"EA GAMES\Ultima Online Samurai Empire", 
                @"EA Games\Ultima Online: Mondain's Legacy", 
                @"EA GAMES\Ultima Online Samurai Empire\1.0", 
                @"EA GAMES\Ultima Online Samurai Empire\1.00.0000", 
                @"EA GAMES\Ultima Online: Samurai Empire\1.0", 
                @"EA GAMES\Ultima Online: Samurai Empire\1.00.0000", 
                @"EA Games\Ultima Online: Mondain's Legacy\1.0", 
                @"EA Games\Ultima Online: Mondain's Legacy\1.00.0000", 
                @"Origin Worlds Online\Ultima Online Samurai Empire BETA\2d\1.0", 
                @"Origin Worlds Online\Ultima Online Samurai Empire BETA\3d\1.0", 
                @"Origin Worlds Online\Ultima Online Samurai Empire\2d\1.0", 
                @"Origin Worlds Online\Ultima Online Samurai Empire\3d\1.0"
            };
        }

        public static IEnumerable<InstallLocation> Locate()
        {
            List<InstallLocation> installations = new List<InstallLocation>();

            for (int i = 0; i < KnownInstallationRegistryKeys.Count; i++)
            {
                string exePath = IntPtr.Size == 8 ? GetExePath(@"Wow6432Node\" + KnownInstallationRegistryKeys[i]) : GetExePath(KnownInstallationRegistryKeys[i]);

                if (!string.IsNullOrEmpty(exePath) && !installations.Contains(exePath))
                    installations.Add(exePath);
            }

            return installations;
        }

        private static string GetExePath(string subName)
        {
            try
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey(string.Format(@"SOFTWARE\{0}", subName));

                if (key == null)
                {
                    key = Registry.CurrentUser.OpenSubKey(string.Format(@"SOFTWARE\{0}", subName));

                    if (key == null)
                        return null;
                }

                bool isExePath = true;
                string path = key.GetValue("ExePath") as string;

                if (((path == null) || (path.Length <= 0)) || (!Directory.Exists(path) && !File.Exists(path)))
                {
                    isExePath = false;
                    path = key.GetValue("Install Dir") as string;

                    if (string.IsNullOrEmpty(path) || (!Directory.Exists(path) && !File.Exists(path)))
                    {
                        path = key.GetValue("InstallDir") as string;

                        if (string.IsNullOrEmpty(path) || (!Directory.Exists(path) && !File.Exists(path)))
                            return null;
                    }
                }

                if (isExePath) 
                    path = Path.GetDirectoryName(path);

                if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
                    return null;

                return path;
            }
            catch(Exception e)
            {
                Tracer.Error(e);
                return null;
            }
        }
    }
}
