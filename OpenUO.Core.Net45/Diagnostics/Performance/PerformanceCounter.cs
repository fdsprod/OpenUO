#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// PerformanceCounter.cs
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
using System.Linq;

#endregion

namespace OpenUO.Core.Diagnostics.Performance
{
    public static class PerformanceTracer
    {
        private static readonly Dictionary<string, PerformanceTrace> m_PerformanceTraces = new Dictionary<string, PerformanceTrace>();

        public static IEnumerable<PerformanceTrace> GetTraces()
        {
            return m_PerformanceTraces.Values.ToArray();
        }

        public static void Clear()
        {
            m_PerformanceTraces.Clear();
        }

        public static IDisposable BeginPerformanceTrace(string name)
        {
            PerformanceTrace trace;

            if (!m_PerformanceTraces.TryGetValue(name, out trace))
            {
                trace = new PerformanceTrace(name);
            }

            return trace;
        }

        private class Disposable : IDisposable
        {
            public static Disposable Empty = new Disposable();

            public void Dispose()
            {
            }
        }

        public class PerformanceTrace : IDisposable
        {
            private readonly List<TimeSpan> m_ElapsedTimes;
            private readonly Stopwatch m_Stopwatch;

            public PerformanceTrace(string name)
            {
                Name = name;
                m_ElapsedTimes = new List<TimeSpan>();

                if (m_Stopwatch != null)
                {
                    m_Stopwatch.Reset();
                    m_Stopwatch.Start();
                }
                else
                {
                    m_Stopwatch = Stopwatch.StartNew();
                }
            }

            public string Name
            {
                get;
            }

            public TimeSpan Elapsed
            {
                get { return TimeSpan.FromTicks((long) m_ElapsedTimes.Average(t => t.Ticks)); }
            }

            public void Dispose()
            {
                m_Stopwatch.Stop();
                m_ElapsedTimes.Add(m_Stopwatch.Elapsed);

                if (!m_PerformanceTraces.ContainsKey(Name))
                {
                    m_PerformanceTraces.Add(Name, this);
                }
            }
        }
    }
}