#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// FileIndexBase.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region Usings

using System;
using System.IO;
using System.Threading.Tasks;

using OpenUO.Core.Threading.Tasks;

#endregion

namespace OpenUO.Ultima
{
    public abstract class FileIndexBase
    {
        private readonly AsyncLock _asyncLock = new AsyncLock();
        private readonly byte[] _copyBuffer = new byte[2*1024*1024];
        private readonly object _syncRoot = new object();

        protected FileIndexBase(string dataPath)
        {
            DataPath = dataPath;
        }

        protected FileIndexBase(string dataPath, int length)
        {
            Length = length;
            DataPath = dataPath;
        }

        public int Length
        {
            get;
            private set;
        }

        public bool IsOpen
        {
            get
            {
                lock (_syncRoot)
                {
                    return Stream != null && Stream.CanRead;
                }
            }
        }

        public FileIndexEntry[] Entries
        {
            get;
            private set;
        }

        public Stream Stream
        {
            get;
            private set;
        }

        protected string DataPath
        {
            get;
        }

        public virtual bool FilesExist => File.Exists(DataPath);

        public async Task<FileIndexSeekResult> SeekAsync(int index)
        {
            var result = new FileIndexSeekResult();

            if (!FilesExist || index < 0 || index >= Length)
            {
                return FileIndexSeekResult.None;
            }

            var e = Entries[index];

            if (e.Lookup < 0 || e.Length <= 0)
            {
                return FileIndexSeekResult.None;
            }

            var length = result.Length = e.Length & 0x7FFFFFFF;

            result.Extra = e.Extra;

            using (await _asyncLock.LockAsync())
            {
                if (Stream != null && (!Stream.CanRead || !Stream.CanSeek))
                {
                    Stream.Dispose();
                    Stream = null;
                }

                if (Stream == null)
                {
                    Stream = new FileStream(DataPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                }

                Stream.Seek(e.Lookup, SeekOrigin.Begin);

                result.Stream = new MemoryStream();

                for (var offset = 0; offset < length; offset += _copyBuffer.Length)
                {
                    var count = Math.Min(_copyBuffer.Length, length);
                    var numBytesRead = await Stream.ReadAsync(_copyBuffer, offset, count).ConfigureAwait(false);

                    await result.Stream.WriteAsync(_copyBuffer, offset, numBytesRead).ConfigureAwait(false);
                }

                result.Stream.Position = 0;

                return result;
            }
        }

        public Stream Seek(int index, out int length, out int extra)
        {
            if (!FilesExist || index < 0 || index >= Length)
            {
                length = extra = 0;
                return null;
            }

            var e = Entries[index];

            if (e.Lookup < 0 || e.Length <= 0)
            {
                length = extra = 0;
                return null;
            }

            length = e.Length & 0x7FFFFFFF;
            extra = e.Extra;

            lock (_syncRoot)
            {
                if (Stream != null && (!Stream.CanRead || !Stream.CanSeek))
                {
                    Stream.Dispose();
                    Stream = null;
                }

                if (Stream == null)
                {
                    Stream = new FileStream(DataPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                }

                Stream.Seek(e.Lookup, SeekOrigin.Begin);

                var resultStream = new MemoryStream();

                for (var offset = 0; offset < length; offset += _copyBuffer.Length)
                {
                    var count = Math.Min(_copyBuffer.Length, length);
                    var numBytesRead = Stream.Read(_copyBuffer, offset, count);

                    resultStream.Write(_copyBuffer, offset, numBytesRead);
                }

                resultStream.Position = 0;

                return resultStream;
            }
        }

        public void Close()
        {
            lock (_syncRoot)
            {
                if (Stream == null)
                {
                    return;
                }

                Stream.Close();
                Stream = null;
            }
        }

        public void Open()
        {
            lock (_syncRoot)
            {
                if (Stream != null)
                {
                    return;
                }

                if (!FilesExist)
                {
                    return;
                }

                Entries = ReadEntries();

                Length = Entries.Length;
            }
        }

        protected abstract FileIndexEntry[] ReadEntries();

        public sealed class FileIndexSeekResult : IDisposable
        {
            public static readonly FileIndexSeekResult None = new FileIndexSeekResult();
            public int Extra;
            public int Length;
            public MemoryStream Stream;

            public void Dispose()
            {
                if (Stream != null)
                {
                    Stream.Dispose();
                    Stream = null;
                }
            }
        }
    }
}