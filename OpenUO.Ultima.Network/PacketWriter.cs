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
using System.Diagnostics;
using System.IO;
using System.Text;

namespace OpenUO.Ultima.Network
{
    public class PacketWriter
    {
        private static readonly Stack<PacketWriter> _pool = new Stack<PacketWriter>();
        private static readonly byte[] _buffer = new byte[8];

        public static PacketWriter CreateInstance()
        {
            return CreateInstance(32);
        }

        public static PacketWriter CreateInstance(int capacity)
        {
            PacketWriter pw = null;

            lock (_pool)
            {
                if (_pool.Count > 0)
                {
                    pw = _pool.Pop();

                    if (pw != null)
                    {
                        pw._capacity = capacity;
                        pw._stream.SetLength(0);
                    }
                }
            }

            return pw ?? new PacketWriter(capacity);
        }

        public static void ReleaseInstance(PacketWriter pw)
        {
            lock (_pool)
            {
                if (!_pool.Contains(pw))
                {
                    _pool.Push(pw);
                }
                else
                {
                    try
                    {
                        using (StreamWriter op = new StreamWriter("neterr.log"))
                        {
                            op.WriteLine("{0}\tInstance pool contains writer", DateTime.Now);
                        }
                    }
                    catch
                    {
                        Console.WriteLine("net error");
                    }
                }
            }
        }

        private readonly MemoryStream _stream;

        private int _capacity;

        public long Length
        {
            get { return _stream.Length; }
        }

        public long Position
        {
            get { return _stream.Position; }
            set { _stream.Position = value; }
        }

        public MemoryStream UnderlyingStream
        {
            get { return _stream; }
        }

        public PacketWriter()
            : this(32)
        {
        }

        public PacketWriter(int capacity)
        {
            _stream = new MemoryStream(capacity);
            _capacity = capacity;
        }

        public PacketWriter(byte[] buffer)
        {
            _stream = new MemoryStream(buffer);
            _capacity = buffer.Length;
        }

        public void Write(bool value)
        {
            _stream.WriteByte((byte)(value ? 1 : 0));
        }

        public void Write(byte value)
        {
            _stream.WriteByte(value);
        }

        public void Write(sbyte value)
        {
            _stream.WriteByte((byte)value);
        }

        public void Write(short value)
        {
            _buffer[0] = (byte)(value >> 8);
            _buffer[1] = (byte)value;

            _stream.Write(_buffer, 0, 2);
        }

        public void Write(ushort value)
        {
            _buffer[0] = (byte)(value >> 8);
            _buffer[1] = (byte)value;

            _stream.Write(_buffer, 0, 2);
        }

        public void Write(int value)
        {
            _buffer[0] = (byte)(value >> 24);
            _buffer[1] = (byte)(value >> 16);
            _buffer[2] = (byte)(value >> 8);
            _buffer[3] = (byte)value;

            _stream.Write(_buffer, 0, 4);
        }

        public void Write(uint value)
        {
            _buffer[0] = (byte)(value >> 24);
            _buffer[1] = (byte)(value >> 16);
            _buffer[2] = (byte)(value >> 8);
            _buffer[3] = (byte)value;

            _stream.Write(_buffer, 0, 4);
        }

        public void Write(long value)
        {
            _buffer[0] = (byte)(value >> 56);
            _buffer[1] = (byte)(value >> 48);
            _buffer[2] = (byte)(value >> 40);
            _buffer[3] = (byte)(value >> 32);
            _buffer[4] = (byte)(value >> 24);
            _buffer[5] = (byte)(value >> 16);
            _buffer[6] = (byte)(value >> 8);
            _buffer[7] = (byte)value;

            _stream.Write(_buffer, 0, 8);
        }

        public void Write(ulong value)
        {
            _buffer[0] = (byte)(value >> 56);
            _buffer[1] = (byte)(value >> 48);
            _buffer[2] = (byte)(value >> 40);
            _buffer[3] = (byte)(value >> 32);
            _buffer[4] = (byte)(value >> 24);
            _buffer[5] = (byte)(value >> 16);
            _buffer[6] = (byte)(value >> 8);
            _buffer[7] = (byte)value;

            _stream.Write(_buffer, 0, 8);
        }

        public void Write(float value)
        {
            uint num = (uint)value;
            _buffer[0] = (byte)(num >> 24);
            _buffer[1] = (byte)(num >> 16);
            _buffer[2] = (byte)(num >> 8);
            _buffer[3] = (byte)num;

            _stream.Write(_buffer, 0, 4);
        }

        public void Write(double value)
        {
            uint num = (uint)value;
            _buffer[0] = (byte)(num >> 56);
            _buffer[1] = (byte)(num >> 48);
            _buffer[2] = (byte)(num >> 40);
            _buffer[3] = (byte)(num >> 32);
            _buffer[4] = (byte)(num >> 24);
            _buffer[5] = (byte)(num >> 16);
            _buffer[6] = (byte)(num >> 8);
            _buffer[7] = (byte)num;

            _stream.Write(_buffer, 0, 8);
        }

        public void Write(byte[] buffer, int offset, int size)
        {
            _stream.Write(buffer, offset, size);
        }

        public void WriteAsciiFixed(string value, int size)
        {
            if (value == null)
            {
                Debug.WriteLine("Network: Attempted to WriteAsciiFixed() with null value");
                value = String.Empty;
            }

            int length = value.Length;

            _stream.SetLength(_stream.Length + size);

            if (length >= size)
                _stream.Position += Encoding.ASCII.GetBytes(value, 0, size, _stream.GetBuffer(), (int)_stream.Position);
            else
            {
                Encoding.ASCII.GetBytes(value, 0, length, _stream.GetBuffer(), (int)_stream.Position);
                _stream.Position += size;
            }
        }

        public void WriteAsciiNull(string value)
        {
            if (value == null)
            {
                Debug.WriteLine("Network: Attempted to WriteAsciiNull() with null value");
                value = String.Empty;
            }

            int length = value.Length;

            _stream.SetLength(_stream.Length + length + 1);

            Encoding.ASCII.GetBytes(value, 0, length, _stream.GetBuffer(), (int)_stream.Position);
            _stream.Position += length + 1;
        }

        public void WriteLittleUniNull(string value)
        {
            if (value == null)
            {
                Debug.WriteLine("Network: Attempted to WriteLittleUniNull() with null value");
                value = String.Empty;
            }

            int length = value.Length;

            _stream.SetLength(_stream.Length + ((length + 1) * 2));

            _stream.Position += Encoding.Unicode.GetBytes(value, 0, length, _stream.GetBuffer(), (int)_stream.Position);
            _stream.Position += 2;
        }

        public void WriteLittleUniFixed(string value, int size)
        {
            if (value == null)
            {
                Debug.WriteLine("Network: Attempted to WriteLittleUniFixed() with null value");
                value = String.Empty;
            }

            size *= 2;

            int length = value.Length;

            _stream.SetLength(_stream.Length + size);

            if ((length * 2) >= size)
                _stream.Position += Encoding.Unicode.GetBytes(value, 0, length, _stream.GetBuffer(), (int)_stream.Position);
            else
            {
                Encoding.Unicode.GetBytes(value, 0, length, _stream.GetBuffer(), (int)_stream.Position);
                _stream.Position += size;
            }
        }

        public void WriteBigUniNull(string value)
        {
            if (value == null)
            {
                Debug.WriteLine("Network: Attempted to WriteBigUniNull() with null value");
                value = String.Empty;
            }

            int length = value.Length;

            _stream.SetLength(_stream.Length + ((length + 1) * 2));

            _stream.Position += Encoding.BigEndianUnicode.GetBytes(value, 0, length, _stream.GetBuffer(), (int)_stream.Position);
            _stream.Position += 2;
        }

        public void WriteBigUniFixed(string value, int size)
        {
            if (value == null)
            {
                Debug.WriteLine("Network: Attempted to WriteBigUniFixed() with null value");
                value = String.Empty;
            }

            size *= 2;

            int length = value.Length;

            _stream.SetLength(_stream.Length + size);

            if ((length * 2) >= size)
                _stream.Position += Encoding.BigEndianUnicode.GetBytes(value, 0, length, _stream.GetBuffer(), (int)_stream.Position);
            else
            {
                Encoding.BigEndianUnicode.GetBytes(value, 0, length, _stream.GetBuffer(), (int)_stream.Position);
                _stream.Position += size;
            }
        }

        public void Fill()
        {
            Fill((int)(_capacity - _stream.Length));
        }

        public void Fill(int length)
        {
            if (_stream.Position == _stream.Length)
            {
                _stream.SetLength(_stream.Length + length);
                _stream.Seek(0, SeekOrigin.End);
            }
            else
            {
                _stream.Write(new byte[length], 0, length);
            }
        }

        public long Seek(long offset, SeekOrigin origin)
        {
            return _stream.Seek(offset, origin);
        }

        public byte[] ToArray()
        {
            return _stream.ToArray();
        }
    }
}