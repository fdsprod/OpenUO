#region License Header
/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
 #endregion

using System;
using System.Collections.Generic;

namespace OpenUO.Ultima.Network
{
    public class SendQueue
    {
        private const int PendingCap = 2048 * 1024;

        private static int _coalesceBufferSize = 512;
        private static BufferPool _unusedBuffers = new BufferPool("Coalesced", 2048, _coalesceBufferSize);

        public static int CoalesceBufferSize
        {
            get { return _coalesceBufferSize; }
            set
            {
                if (_coalesceBufferSize == value)
                    return;

                if (_unusedBuffers != null)
                    _unusedBuffers.Free();

                _coalesceBufferSize = value;
                _unusedBuffers = new BufferPool("Coalesced", 2048, _coalesceBufferSize);
            }
        }

        public static byte[] AcquireBuffer()
        {
            return _unusedBuffers.AcquireBuffer();
        }

        public static void ReleaseBuffer(byte[] buffer)
        {
            if (buffer != null && buffer.Length == _coalesceBufferSize)
            {
                _unusedBuffers.ReleaseBuffer(buffer);
            }
        }

        private readonly Queue<Gram> _pending;
        private Gram _buffered;

        public bool IsFlushReady
        {
            get { return (_pending.Count == 0 && _buffered != null); }
        }

        public bool IsEmpty
        {
            get { return (_pending.Count == 0 && _buffered == null); }
        }

        public SendQueue()
        {
            _pending = new Queue<Gram>();
        }

        public Gram CheckFlushReady()
        {
            Gram gram = null;

            if (_pending.Count == 0 && _buffered != null)
            {
                gram = _buffered;

                _pending.Enqueue(_buffered);
                _buffered = null;
            }

            return gram;
        }

        public Gram Dequeue()
        {
            Gram gram = null;

            if (_pending.Count > 0)
            {
                _pending.Dequeue().Release();

                if (_pending.Count > 0)
                {
                    gram = _pending.Peek();
                }
            }

            return gram;
        }

        public Gram Enqueue(byte[] buffer, int length)
        {
            return Enqueue(buffer, 0, length);
        }

        public Gram Enqueue(byte[] buffer, int offset, int length)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (!(offset >= 0 && offset < buffer.Length))
            {
                throw new ArgumentOutOfRangeException("offset", "Offset must be greater than or equal to zero and less than the size of the buffer.");
            }

            if (length < 0 || length > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("length", "Length cannot be less than zero or greater than the size of the buffer.");
            }

            if ((buffer.Length - offset) < length)
            {
                throw new ArgumentException("Offset and length do not point to a valid segment within the buffer.");
            }

            int existingBytes = (_pending.Count * _coalesceBufferSize) + (_buffered == null ? 0 : _buffered.Length);

            if ((existingBytes + length) > PendingCap)
            {
                throw new CapacityExceededException();
            }

            Gram gram = null;

            while (length > 0)
            {
                if (_buffered == null)
                {
                    // nothing yet buffered
                    _buffered = Gram.Acquire();
                }

                int bytesWritten = _buffered.Write(buffer, offset, length);

                offset += bytesWritten;
                length -= bytesWritten;

                // If it is Server it'll check the IsFull, if it is not then we'll expect that they are a ClientNetState Send
                if (_buffered.IsFull)
                {
                    if (_pending.Count == 0)
                    {
                        gram = _buffered;
                    }

                    _pending.Enqueue(_buffered);
                    _buffered = null;
                }
            }

            return gram;
        }

        public void Clear()
        {
            if (_buffered != null)
            {
                _buffered.Release();
                _buffered = null;
            }

            while (_pending.Count > 0)
            {
                _pending.Dequeue().Release();
            }
        }
        public class Gram
        {
            private static readonly Stack<Gram> _pool = new Stack<Gram>();

            public static Gram Acquire()
            {
                lock (_pool)
                {
                    Gram gram = _pool.Count > 0 ? _pool.Pop() : new Gram();

                    gram._buffer = AcquireBuffer();
                    gram._length = 0;

                    return gram;
                }
            }

            private byte[] _buffer;
            private int _length;

            public byte[] Buffer
            {
                get { return _buffer; }
            }

            public int Length
            {
                get { return _length; }
            }

            public int Available
            {
                get { return (_buffer.Length - _length); }
            }

            public bool IsFull
            {
                get { return (_length == _buffer.Length); }
            }

            private Gram()
            {
            }

            public int Write(byte[] buffer, int offset, int length)
            {
                int write = Math.Min(length, Available);

                System.Buffer.BlockCopy(buffer, offset, _buffer, _length, write);

                _length += write;

                return write;
            }

            public void Release()
            {
                lock (_pool)
                {
                    _pool.Push(this);
                    ReleaseBuffer(_buffer);
                }
            }
        }
    }

    public sealed class CapacityExceededException : Exception
    {
        public CapacityExceededException() : base("Too much data pending.") { }
    }
}