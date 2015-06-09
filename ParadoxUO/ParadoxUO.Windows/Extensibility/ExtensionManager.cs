#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// PluginManager.cs
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
using System.Linq;
using System.Reflection;

using OpenUO.Core.Diagnostics.Tracing;
using OpenUO.Core.Patterns;

#endregion

namespace ParadoxUO.Extensibility
{
    internal static class ExtensionManager
    {
        internal static IReadOnlyCollection<IExtension> Extensions
        {
            get;
            private set;
        }

        public static void Initialize(IContainer container, string applicationPath)
        {
            var directory = new DirectoryInfo(Path.Combine(applicationPath, "extensions"));

            if (!directory.Exists)
            {
                return;
            }

            var assemblies = directory.GetFiles("*.dll");
            var extensions = new List<IExtension>();

            foreach (var file in assemblies)
            {
                var assembly = Assembly.LoadFile(file.FullName);
                var types = assembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeof (IModule))).ToArray();

                Tracer.Info("Found {0} extension.", types.Length);

                foreach (var type in types)
                {
                    try
                    {
                        var extension = (IExtension) Activator.CreateInstance(type);

                        Tracer.Info("Initializing extension {0}.", extension.Name);

                        extension.Initialize(container);
                        extensions.Add(extension);
                    }
                    catch (Exception e)
                    {
                        Tracer.Warn(e, "An error occurred while trying to load extension. [{0}]", file.FullName);
                    }
                }
            }

            Extensions = extensions.AsReadOnly();
        }
    }
}