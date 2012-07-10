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
using System.Text;

namespace Client.Diagnostics
{
    public sealed class PerformanceMonitor
    {
        private readonly Dictionary<string, MonitorTimer> _monitorTimers;
        private readonly Dictionary<string, int> _counters;
        private readonly Dictionary<string, int> _lifetimeCounters;
        private readonly object _syncRoot = new object();

        public Dictionary<string, MonitorTimer> MonitorTimers
        {
            get { return _monitorTimers; }
        }

        public Dictionary<string, int> Counters
        {
            get { return _counters; }
        }

        public Dictionary<string, int> LifetimeCounters
        {
            get { return _lifetimeCounters; }
        }

        public PerformanceMonitor()
        {
            _monitorTimers = new Dictionary<string, MonitorTimer>();
            _counters = new Dictionary<string, int>();
            _lifetimeCounters = new Dictionary<string, int>();
        }

        public void Reset()
        {
            lock (_syncRoot)
            {
                _monitorTimers.Clear();
                _counters.Clear();
            }
        }

        public void DecreaseLifetimeCounter(string counterId)
        {
            DecreaseLifetimeCounter(counterId, 1);
        }

        public void DecreaseLifetimeCounter(string counterId, int value)
        {
            lock (_syncRoot)
            {
                int counter;

                if (!_lifetimeCounters.TryGetValue(counterId, out counter))
                    _lifetimeCounters.Add(counterId, counter);

                _lifetimeCounters[counterId] = counter - value;
            }
        }

        public void IncreaseLifetimeCounter(string counterId)
        {
            IncreaseLifetimeCounter(counterId, 1);
        }

        public void IncreaseLifetimeCounter(string counterId, int value)
        {
            lock (_syncRoot)
            {
                int counter;

                if (!_lifetimeCounters.TryGetValue(counterId, out counter))
                    _lifetimeCounters.Add(counterId, counter);

                _lifetimeCounters[counterId] = counter + value;
            }
        }

        public void IncreaseCounter(string counterId)
        {
            IncreaseCounter(counterId, 1);
        }

        public void IncreaseCounter(string counterId, int value)
        {
            lock (_syncRoot)
            {
                int counter;

                if (!_counters.TryGetValue(counterId, out counter))
                    _counters.Add(counterId, counter);

                _counters[counterId] = counter + value;
            }
        }

        public void StartTimer(string monitorId)
        {
            MonitorTimer timer;
            lock (_syncRoot)
            {
                if (!_monitorTimers.TryGetValue(monitorId, out timer))
                {
                    timer = new MonitorTimer();
                    _monitorTimers.Add(monitorId, timer);
                }

                timer.Start();
            }
        }

        public void StopTimer(string monitorId)
        {
            MonitorTimer timer;

            lock (_syncRoot)
            {
                if (!_monitorTimers.TryGetValue(monitorId, out timer))
                    throw new Exception(string.Format("Monitor Id : {0} has not started.  StartTimer must be called before StopTimer can be called.", monitorId));

                timer.Stop();
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            if (_monitorTimers.Count > 0)
            {
                builder.AppendLine("Timers");
                builder.AppendLine("--------------------------");
                builder.AppendLine();

                foreach (var key in _monitorTimers.Keys)
                {
                    MonitorTimer timer = _monitorTimers[key];

                    builder.AppendFormat("{0}: {1}{2}", key, TimeSpan.FromSeconds(timer.TotalTime.Seconds), timer.Active ? " Active:True" : string.Empty);
                    builder.AppendLine();
                }
            }

            if (_counters.Count > 0)
            {
                builder.AppendLine();
                builder.AppendLine("Counters");
                builder.AppendLine("--------------------------");
                builder.AppendLine();

                foreach (var key in _counters.Keys)
                {
                    int count = _counters[key];

                    builder.AppendFormat("{0}: {1}", key, count);
                    builder.AppendLine();
                }
            }

            if (_lifetimeCounters.Count > 0)
            {
                builder.AppendLine();
                builder.AppendLine("Lifetime Counters");
                builder.AppendLine("--------------------------");
                builder.AppendLine();

                foreach (var key in _lifetimeCounters.Keys)
                {
                    int count = _lifetimeCounters[key];

                    builder.AppendFormat("{0}: {1}", key, count);
                    builder.AppendLine();
                }
            }

            if (builder.Length == 0)
                builder.Append("No timers or counters exist.");

            return builder.ToString();
        }

        public class MonitorTimer
        {
            public PerfTime StartTime
            {
                get;
                internal set;
            }

            public PerfTimeSpan TotalTime
            {
                get;
                internal set;
            }

            public bool Active
            {
                get;
                internal set;
            }

            internal void Start()
            {
                if (Active)
                    return;

                StartTime = PerfTime.Now;
                Active = true;
            }

            internal void Stop()
            {
                if (!Active)
                    return;

                TotalTime += PerfTime.Now - StartTime;
                Active = false;
            }

            internal void Flush()
            {
                if (Active)
                {
                    PerfTime now = PerfTime.Now;

                    TotalTime += (now - StartTime);
                    StartTime = now;
                }
            }
        }
    }
}
