#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// FileLogEventListener.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region Usings

using System.IO;

using OpenUO.Core.Diagnostics.Tracing.Listeners;

#endregion

namespace OpenUO.Core.Windows.Diagnostics.Tracing.Listeners
{
    public sealed class FileLogEventListener : StreamOuputEventListener
    {
        public FileLogEventListener(string filename)
            : base(new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Read), true)
        {
        }
    }
}