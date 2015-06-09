#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// PacketWriter.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

#endregion

namespace OpenUO.Core.Net
{
    public class PacketWriter : IDisposable
    {
        private static readonly Stack<PacketWriter> m_pool = new Stack<PacketWriter>();
        private static readonly byte[] m_buffer = new byte[4];
        private int m_capacity;

        /// <summary>
        /// Instantiates a new PacketWriter instance with a 32 byte capacity.
        /// </summary>
        public PacketWriter()
            : this(32)
        {
        }

        /// <summary>
        /// Instantiates a new PacketWriter instance with a given capacity.
        /// </summary>
        /// <param name="capacity">Initial capacity for the internal stream.</param>
        public PacketWriter(int capacity)
        {
            BaseStream = new MemoryStream(capacity);
            m_capacity = capacity;
        }

        /// <summary>
        /// Gets the total stream length.
        /// </summary>
        public long Length
        {
            get { return BaseStream.Length; }
        }

        /// <summary>
        /// Gets or sets the current stream position.
        /// </summary>
        public long Position
        {
            get { return BaseStream.Position; }
            set { BaseStream.Position = value; }
        }

        /// <summary>
        /// The internal stream used by this PacketWriter instance.
        /// </summary>
        public MemoryStream BaseStream
        {
            get;
            private set;
        }

        #region IDisposable Members

        public void Dispose()
        {
            m_capacity = 0;

            if (BaseStream != null)
            {
                BaseStream.Close();
                BaseStream.Dispose();
            }

            ReleaseInstance(this);
        }

        #endregion

        public static PacketWriter CreateInstance()
        {
            return CreateInstance(32);
        }

        public static PacketWriter CreateInstance(int capacity)
        {
            PacketWriter writer = null;

            lock (m_pool)
            {
                if (m_pool.Count > 0)
                {
                    writer = m_pool.Pop();

                    if (writer != null)
                    {
                        writer.m_capacity = capacity;
                        writer.BaseStream = new MemoryStream(capacity);
                        writer.BaseStream.Seek(0, SeekOrigin.Begin);
                    }
                }
            }

            if (writer == null)
            {
                writer = new PacketWriter(capacity);
            }

            return writer;
        }

        public static void ReleaseInstance(PacketWriter writer)
        {
            lock (m_pool)
            {
                if (!m_pool.Contains(writer))
                {
                    m_pool.Push(writer);
                }
            }
        }

        /// <summary>
        /// Writes a 1-byte boolean value to the underlying stream. False is represented by 0, true by 1.
        /// </summary>
        public void Write(bool value)
        {
            BaseStream.WriteByte((byte) (value ? 1 : 0));
        }

        /// <summary>
        /// Writes a 1-byte unsigned integer value to the underlying stream.
        /// </summary>
        public void Write(byte value)
        {
            BaseStream.WriteByte(value);
        }

        /// <summary>
        /// Writes a 1-byte signed integer value to the underlying stream.
        /// </summary>
        public void Write(sbyte value)
        {
            BaseStream.WriteByte((byte) value);
        }

        /// <summary>
        /// Writes a 2-byte signed integer value to the underlying stream.
        /// </summary>
        public void Write(short value)
        {
            m_buffer[0] = (byte) (value >> 8);
            m_buffer[1] = (byte) value;

            BaseStream.Write(m_buffer, 0, 2);
        }

        /// <summary>
        /// Writes a 2-byte unsigned integer value to the underlying stream.
        /// </summary>
        public void Write(ushort value)
        {
            m_buffer[0] = (byte) (value >> 8);
            m_buffer[1] = (byte) value;

            BaseStream.Write(m_buffer, 0, 2);
        }

        /// <summary>
        /// Writes a 4-byte signed integer value to the underlying stream.
        /// </summary>
        public void Write(int value)
        {
            m_buffer[0] = (byte) (value >> 24);
            m_buffer[1] = (byte) (value >> 16);
            m_buffer[2] = (byte) (value >> 8);
            m_buffer[3] = (byte) value;

            BaseStream.Write(m_buffer, 0, 4);
        }

        /// <summary>
        /// Writes a 4-byte unsigned integer value to the underlying stream.
        /// </summary>
        public void Write(uint value)
        {
            m_buffer[0] = (byte) (value >> 24);
            m_buffer[1] = (byte) (value >> 16);
            m_buffer[2] = (byte) (value >> 8);
            m_buffer[3] = (byte) value;

            BaseStream.Write(m_buffer, 0, 4);
        }

        /// <summary>
        /// Writes a sequence of bytes to the underlying stream
        /// </summary>
        public void Write(byte[] buffer, int offset, int size)
        {
            BaseStream.Write(buffer, offset, size);
        }

        /// <summary>
        /// Writes a fixed-length ASCII-encoded string value to the underlying stream. To fit (size), the string content is either truncated or padded with null characters.
        /// </summary>
        public void WriteAsciiFixed(string value, int size)
        {
            if (value == null)
            {
                ////log.Warn("Attempted to WriteAsciiFixed() with null value");
                value = String.Empty;
            }

            var length = value.Length;

            BaseStream.SetLength(BaseStream.Length + size);

            if (length >= size)
            {
                BaseStream.Position += Encoding.ASCII.GetBytes(value, 0, size, BaseStream.GetBuffer(), (int) BaseStream.Position);
            }
            else
            {
                Encoding.ASCII.GetBytes(value, 0, length, BaseStream.GetBuffer(), (int) BaseStream.Position);
                BaseStream.Position += size;
            }
        }

        /// <summary>
        /// Writes a dynamic-length ASCII-encoded string value to the underlying stream, followed by a 1-byte null character.
        /// </summary>
        public void WriteAsciiNull(string value)
        {
            if (value == null)
            {
                ////log.Warn("Attempted to WriteAsciiNull() with null value");
                value = String.Empty;
            }

            var length = value.Length;

            BaseStream.SetLength(BaseStream.Length + length + 1);

            Encoding.ASCII.GetBytes(value, 0, length, BaseStream.GetBuffer(), (int) BaseStream.Position);
            BaseStream.Position += length + 1;
        }

        /// <summary>
        /// Writes a dynamic-length little-endian unicode string value to the underlying stream, followed by a 2-byte null character.
        /// </summary>
        public void WriteLittleUniNull(string value)
        {
            if (value == null)
            {
                //log.Warn("Attempted to WriteLittleUniNull() with null value");
                value = String.Empty;
            }

            var length = value.Length;

            BaseStream.SetLength(BaseStream.Length + ((length + 1)*2));

            BaseStream.Position += Encoding.Unicode.GetBytes(value, 0, length, BaseStream.GetBuffer(), (int) BaseStream.Position);
            BaseStream.Position += 2;
        }

        /// <summary>
        /// Writes a fixed-length little-endian unicode string value to the underlying stream. To fit (size), the string content is either truncated or padded with null characters.
        /// </summary>
        public void WriteLittleUniFixed(string value, int size)
        {
            if (value == null)
            {
                //log.Warn("Attempted to WriteLittleUniFixed() with null value");
                value = String.Empty;
            }

            size *= 2;

            var length = value.Length;

            BaseStream.SetLength(BaseStream.Length + size);

            if ((length*2) >= size)
            {
                BaseStream.Position += Encoding.Unicode.GetBytes(value, 0, length, BaseStream.GetBuffer(), (int) BaseStream.Position);
            }
            else
            {
                Encoding.Unicode.GetBytes(value, 0, length, BaseStream.GetBuffer(), (int) BaseStream.Position);
                BaseStream.Position += size;
            }
        }

        /// <summary>
        /// Writes a dynamic-length big-endian unicode string value to the underlying stream, followed by a 2-byte null character.
        /// </summary>
        public void WriteBigUniNull(string value)
        {
            if (value == null)
            {
                //log.Warn("Attempted to WriteBigUniNull() with null value");
                value = String.Empty;
            }

            var length = value.Length;

            BaseStream.SetLength(BaseStream.Length + ((length + 1)*2));

            BaseStream.Position += Encoding.BigEndianUnicode.GetBytes(value, 0, length, BaseStream.GetBuffer(), (int) BaseStream.Position);
            BaseStream.Position += 2;
        }

        /// <summary>
        /// Writes a fixed-length big-endian unicode string value to the underlying stream. To fit (size), the string content is either truncated or padded with null characters.
        /// </summary>
        public void WriteBigUniFixed(string value, int size)
        {
            if (value == null)
            {
                //log.Warn("Attempted to WriteBigUniFixed() with null value");
                value = String.Empty;
            }

            size *= 2;

            var length = value.Length;

            BaseStream.SetLength(BaseStream.Length + size);

            if ((length*2) >= size)
            {
                BaseStream.Position += Encoding.BigEndianUnicode.GetBytes(value, 0, length, BaseStream.GetBuffer(), (int) BaseStream.Position);
            }
            else
            {
                Encoding.BigEndianUnicode.GetBytes(value, 0, length, BaseStream.GetBuffer(), (int) BaseStream.Position);
                BaseStream.Position += size;
            }
        }

        /// <summary>
        /// Fills the stream from the current position up to (capacity) with 0x00's
        /// </summary>
        public void Fill()
        {
            Fill((int) (m_capacity - BaseStream.Length));
        }

        /// <summary>
        /// Writes a number of 0x00 byte values to the underlying stream.
        /// </summary>
        public void Fill(int length)
        {
            if (BaseStream.Position == BaseStream.Length)
            {
                BaseStream.SetLength(BaseStream.Length + length);
                BaseStream.Seek(0, SeekOrigin.End);
            }
            else
            {
                BaseStream.Write(new byte[length], 0, length);
            }
        }

        /// <summary>
        /// Offsets the current position from an origin.
        /// </summary>
        public long Seek(long offset, SeekOrigin origin)
        {
            return BaseStream.Seek(offset, origin);
        }

        /// <summary>
        /// Gets the entire stream content as a byte array.
        /// </summary>
        public byte[] ToArray()
        {
            return BaseStream.ToArray();
        }

        public void Flush()
        {
            BaseStream.Flush();
        }

        public byte[] Compile()
        {
            Flush();
            return BaseStream.ToArray();
        }
    }
}