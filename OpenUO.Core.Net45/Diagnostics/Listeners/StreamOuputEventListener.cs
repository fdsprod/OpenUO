#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// StreamOuputEventListener.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region Usings

using System;
using System.Diagnostics.Tracing;
using System.IO;
using System.Text;

#endregion

namespace OpenUO.Core.Diagnostics.Tracing.Listeners
{
    public class StreamOuputEventListener : EventListener
    {
        private const string Format = "{0} {1:yyyy-MM-dd HH\\:mm\\:ss\\:ffff} {2}";
        private readonly bool m_closeStreamOnDispose;
        private readonly object m_syncRoot = new object();
        private readonly StreamWriter m_writer;
        private Stream m_stream;

        public StreamOuputEventListener(Stream stream, bool closeStreamOnDispose)
        {
            m_stream = stream;
            m_writer = new StreamWriter(m_stream, Encoding.Unicode);
            m_closeStreamOnDispose = closeStreamOnDispose;
        }

        public override void Dispose()
        {
            base.Dispose();

            if (!m_closeStreamOnDispose || m_stream == null)
            {
                return;
            }

            m_stream.Dispose();
            m_stream = null;
        }

        protected override void OnEventWritten(EventWrittenEventArgs e)
        {
            var output = string.Format(Format, e.Level, DateTime.Now, e.Payload[0]);

            lock (m_syncRoot)
            {
                m_writer.WriteLine(output);
                m_writer.Flush();
            }
        }
    }
}