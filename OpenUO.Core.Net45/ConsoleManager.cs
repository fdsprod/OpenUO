#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// ConsoleManager.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

#endregion

namespace OpenUO.Core
{
    [SuppressUnmanagedCodeSecurity]
    internal static class ConsoleManager
    {
        private const string Kernel32_DllName = "kernel32.dll";
        private static readonly Stack<ConsoleColor> m_consoleColors = new Stack<ConsoleColor>();

        public static bool HasConsole
        {
            get { return GetConsoleWindow() != IntPtr.Zero; }
        }

        public static void PushColor(ConsoleColor color)
        {
            try
            {
                m_consoleColors.Push(Console.ForegroundColor);
                Console.ForegroundColor = color;
            }
            catch
            {
            }
        }

        public static ConsoleColor PopColor()
        {
            try
            {
                Console.ForegroundColor = m_consoleColors.Pop();
            }
            catch
            {
            }

            return Console.ForegroundColor;
        }

        [DllImport(Kernel32_DllName)]
        private static extern bool AllocConsole();

        [DllImport(Kernel32_DllName)]
        private static extern bool FreeConsole();

        [DllImport(Kernel32_DllName)]
        private static extern IntPtr GetConsoleWindow();

        public static void Show()
        {
            if (!HasConsole)
            {
                AllocConsole();
                InvalidateOutAndError();
            }
        }

        public static void Hide()
        {
            if (HasConsole)
            {
                SetOutAndErrorNull();
                FreeConsole();
            }
        }

        public static void Toggle()
        {
            if (HasConsole)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        private static void InvalidateOutAndError()
        {
            var type = typeof (Console);

            _FieldInfo output = type.GetField("_out",
                BindingFlags.Static | BindingFlags.NonPublic);

            _FieldInfo error = type.GetField("_error",
                BindingFlags.Static | BindingFlags.NonPublic);

            _MethodInfo initializeStdOutError = type.GetMethod("InitializeStdOutError",
                BindingFlags.Static | BindingFlags.NonPublic);

            Debug.Assert(output != null);
            Debug.Assert(error != null);
            Debug.Assert(initializeStdOutError != null);

            output.SetValue(null, null);
            error.SetValue(null, null);
            initializeStdOutError.Invoke(null, new object[] {true});
        }

        private static void SetOutAndErrorNull()
        {
            Console.SetOut(TextWriter.Null);
            Console.SetError(TextWriter.Null);
        }
    }
}