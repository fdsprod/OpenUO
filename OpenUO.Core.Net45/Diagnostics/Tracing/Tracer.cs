#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// Tracer.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region Usings

using System;
using System.Diagnostics.Tracing;

#endregion

namespace OpenUO.Core.Diagnostics.Tracing
{
    public static class Tracer
    {
        public static void RegisterListener<T>(T listener, EventLevel eventLevel = EventLevel.Verbose)
            where T : EventListener
        {
            listener.EnableEvents(TracerEventSource.Instance, eventLevel);
        }

        public static void Critical(string message)
        {
            ConcreteTracer.Instance.Critical(message);
        }

        public static void Critical(Exception ex)
        {
            ConcreteTracer.Instance.Critical(ex);
        }

        public static void Critical(Exception ex, string message, params object[] args)
        {
            ConcreteTracer.Instance.Critical(ex, message, args);
        }

        public static void Error(string message, params object[] args)
        {
            ConcreteTracer.Instance.Error(message, args);
        }

        public static void Error(Exception ex)
        {
            ConcreteTracer.Instance.Error(ex);
        }

        public static void Error(Exception ex, string message, params object[] args)
        {
            ConcreteTracer.Instance.Error(ex, message, args);
        }

        public static void Warn(Exception ex)
        {
            ConcreteTracer.Instance.Warn(ex);
        }

        public static void Warn(Exception ex, string message, params object[] args)
        {
            ConcreteTracer.Instance.Warn(ex, message, args);
        }

        public static void Warn(string message, params object[] args)
        {
            ConcreteTracer.Instance.Warn(message, args);
        }

        public static void Verbose(string message, params object[] args)
        {
            ConcreteTracer.Instance.Verbose(message, args);
        }

        public static void Debug(string message, params object[] args)
        {
            ConcreteTracer.Instance.Debug(message, args);
        }

        public static void Info(string message, params object[] args)
        {
            ConcreteTracer.Instance.Info(message, args);
        }
    }
}