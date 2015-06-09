#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// Boostrapper.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region Usings

using System;
using System.Diagnostics;
using System.Threading.Tasks;

using OpenUO.Core.Configuration;
using OpenUO.Core.Diagnostics;
using OpenUO.Core.Diagnostics.Tracing;
using OpenUO.Core.Diagnostics.Tracing.Listeners;
using OpenUO.Core.Patterns;
using OpenUO.Core.Windows.Diagnostics.Tracing.Listeners;
using OpenUO.Ultima;

using ParadoxUO.Extensibility;

using SiliconStudio.Core;
using SiliconStudio.Paradox.Engine;

#endregion

namespace ParadoxUO
{
    public class Boostrapper
    {
        public static string BaseApplicationPath
        {
            get { return AppDomain.CurrentDomain.BaseDirectory; }
        }

        [STAThread]
        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

            GeneralExceptionHandler.Instance = new GeneralExceptionHandler();

            if (Debugger.IsAttached)
            {
                Tracer.RegisterListener(new DebugOutputEventListener());
            }

            Tracer.RegisterListener(new FileLogEventListener("debug.log"));

            if (Settings.Debug.IsConsoleEnabled)
            {
                Tracer.RegisterListener(new ConsoleOutputEventListener());
            }

            try
            {
                if (Settings.Debug.IsConsoleEnabled && !ConsoleManager.HasConsole)
                {
                    ConsoleManager.Show();
                }

                //Profiler.EnableAll();

                using (var client = new Client())
                {
                    client.Container.RegisterModule<ParadoxUOCoreModule>();

                    ExtensionManager.Initialize(client.Container, BaseApplicationPath);

                    client.Run();
                }
            }
            finally
            {
                if (ConsoleManager.HasConsole)
                {
                    ConsoleManager.Hide();
                }
            }
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            GeneralExceptionHandler.Instance.OnError((Exception) e.ExceptionObject);
        }

        private static void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            GeneralExceptionHandler.Instance.OnError(e.Exception);
        }
    }
}