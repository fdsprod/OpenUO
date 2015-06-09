#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// PacketReader.cs
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
    public class PacketReader : IDisposable
    {
        private static readonly Stack<PacketReader> m_pool = new Stack<PacketReader>();

        public PacketReader(byte[] data, int size, bool fixedSize)
        {
            Buffer = data;
            Size = size;
            Index = fixedSize ? 1 : 3;
        }

        public int Index
        {
            get;
            private set;
        }

        public byte[] Buffer
        {
            get;
            private set;
        }

        public int Size
        {
            get;
            private set;
        }

        public void Dispose()
        {
            Buffer = null;
            Size = 0;
            Index = 0;

            ReleaseInstance(this);
        }

        public static PacketReader CreateInstance(byte[] buffer, int length, bool fixedSize)
        {
            PacketReader reader = null;

            lock (m_pool)
            {
                if (m_pool.Count > 0)
                {
                    reader = m_pool.Pop();

                    if (reader != null)
                    {
                        reader.Buffer = buffer;
                        reader.Size = length;
                        reader.Index = fixedSize ? 1 : 3;
                    }
                }
            }

            if (reader == null)
            {
                reader = new PacketReader(buffer, length, fixedSize);
            }

            return reader;
        }

        public static void ReleaseInstance(PacketReader reader)
        {
            lock (m_pool)
            {
                if (!m_pool.Contains(reader))
                {
                    m_pool.Push(reader);
                }
            }
        }

        public int Seek(int offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Index = offset;
                    break;
                case SeekOrigin.Current:
                    Index += offset;
                    break;
                case SeekOrigin.End:
                    Index = Size - offset;
                    break;
            }

            return Index;
        }

        public int ReadInt32()
        {
            if ((Index + 4) > Size)
            {
                return 0;
            }

            return (Buffer[Index++] << 24)
                   | (Buffer[Index++] << 16)
                   | (Buffer[Index++] << 8)
                   | Buffer[Index++];
        }

        public short ReadInt16()
        {
            if ((Index + 2) > Size)
            {
                return 0;
            }

            return (short) ((Buffer[Index++] << 8) | Buffer[Index++]);
        }

        public byte ReadByte()
        {
            if ((Index + 1) > Size)
            {
                return 0;
            }

            return Buffer[Index++];
        }

        public byte[] ReadBytes(int length)
        {
            if ((Index + length) > Size)
            {
                return new byte[0];
            }

            var b = new byte[length];

            Array.Copy(Buffer, Index, b, 0, length);
            Index += length;
            return b;
        }

        public uint ReadUInt32()
        {
            if ((Index + 4) > Size)
            {
                return 0;
            }

            return (uint) ((Buffer[Index++] << 24) | (Buffer[Index++] << 16) | (Buffer[Index++] << 8) | Buffer[Index++]);
        }

        public ushort ReadUInt16()
        {
            if ((Index + 2) > Size)
            {
                return 0;
            }

            return (ushort) ((Buffer[Index++] << 8) | Buffer[Index++]);
        }

        public sbyte ReadSByte()
        {
            if ((Index + 1) > Size)
            {
                return 0;
            }

            return (sbyte) Buffer[Index++];
        }

        public bool ReadBoolean()
        {
            if ((Index + 1) > Size)
            {
                return false;
            }

            return (Buffer[Index++] != 0);
        }

        public string ReadUnicodeStringLE()
        {
            var sb = new StringBuilder();

            int c;

            while ((Index + 1) < Size && (c = (Buffer[Index++] | (Buffer[Index++] << 8))) != 0)
            {
                sb.Append((char) c);
            }

            return sb.ToString();
        }

        public string ReadUnicodeStringLESafe(int fixedLength)
        {
            var bound = Index + (fixedLength << 1);
            var end = bound;

            if (bound > Size)
            {
                bound = Size;
            }

            var sb = new StringBuilder();

            int c;

            while ((Index + 1) < bound && (c = (Buffer[Index++] | (Buffer[Index++] << 8))) != 0)
            {
                if (IsSafeChar(c))
                {
                    sb.Append((char) c);
                }
            }

            Index = end;

            return sb.ToString();
        }

        public string ReadUnicodeStringLESafe()
        {
            var sb = new StringBuilder();

            int c;

            while ((Index + 1) < Size && (c = (Buffer[Index++] | (Buffer[Index++] << 8))) != 0)
            {
                if (IsSafeChar(c))
                {
                    sb.Append((char) c);
                }
            }

            return sb.ToString();
        }

        public string ReadUnicodeStringSafe()
        {
            var sb = new StringBuilder();

            int c;

            while ((Index + 1) < Size && (c = ((Buffer[Index++] << 8) | Buffer[Index++])) != 0)
            {
                if (IsSafeChar(c))
                {
                    sb.Append((char) c);
                }
            }

            return sb.ToString();
        }

        public string ReadUnicodeString()
        {
            var sb = new StringBuilder();

            int c;

            while ((Index + 1) < Size && (c = ((Buffer[Index++] << 8) | Buffer[Index++])) != 0)
            {
                sb.Append((char) c);
            }

            return sb.ToString();
        }

        public bool IsSafeChar(int c)
        {
            return ((c >= 0x20 && c < 0xFFFE) || (c == 0x09));
        }

        public string ReadUTF8StringSafe(int fixedLength)
        {
            if (Index >= Size)
            {
                Index += fixedLength;
                return String.Empty;
            }

            var bound = Index + fixedLength;

            //int end   = bound;

            if (bound > Size)
            {
                bound = Size;
            }

            var count = 0;
            var index = Index;
            var start = Index;

            while (index < bound && Buffer[index++] != 0)
            {
                ++count;
            }

            index = 0;

            var buffer = new byte[count];
            var value = 0;

            while (Index < bound && (value = Buffer[Index++]) != 0)
            {
                buffer[index++] = (byte) value;
            }

            var s = Encoding.UTF8.GetString(buffer);

            var isSafe = true;

            for (var i = 0; isSafe && i < s.Length; ++i)
            {
                isSafe = IsSafeChar(s[i]);
            }

            Index = start + fixedLength;

            if (isSafe)
            {
                return s;
            }

            var sb = new StringBuilder(s.Length);

            for (var i = 0; i < s.Length; ++i)
            {
                if (IsSafeChar(s[i]))
                {
                    sb.Append(s[i]);
                }
            }

            return sb.ToString();
        }

        public string ReadUTF8StringSafe()
        {
            if (Index >= Size)
            {
                return String.Empty;
            }

            var count = 0;
            var index = Index;

            while (index < Size && Buffer[index++] != 0)
            {
                ++count;
            }

            index = 0;

            var buffer = new byte[count];
            var value = 0;

            while (Index < Size && (value = Buffer[Index++]) != 0)
            {
                buffer[index++] = (byte) value;
            }

            var s = Encoding.UTF8.GetString(buffer);

            var isSafe = true;

            for (var i = 0; isSafe && i < s.Length; ++i)
            {
                isSafe = IsSafeChar(s[i]);
            }

            if (isSafe)
            {
                return s;
            }

            var sb = new StringBuilder(s.Length);

            for (var i = 0; i < s.Length; ++i)
            {
                if (IsSafeChar(s[i]))
                {
                    sb.Append(s[i]);
                }
            }

            return sb.ToString();
        }

        public string ReadUTF8String()
        {
            if (Index >= Size)
            {
                return String.Empty;
            }

            var count = 0;
            var index = Index;

            while (index < Size && Buffer[index++] != 0)
            {
                ++count;
            }

            index = 0;

            var buffer = new byte[count];
            var value = 0;

            while (Index < Size && (value = Buffer[Index++]) != 0)
            {
                buffer[index++] = (byte) value;
            }

            return Encoding.UTF8.GetString(buffer);
        }

        public string ReadString()
        {
            var sb = new StringBuilder();

            int c;

            while (Index < Size && (c = Buffer[Index++]) != 0)
            {
                sb.Append((char) c);
            }

            return sb.ToString();
        }

        public string ReadStringSafe()
        {
            var sb = new StringBuilder();

            int c;

            while (Index < Size && (c = Buffer[Index++]) != 0)
            {
                if (IsSafeChar(c))
                {
                    sb.Append((char) c);
                }
            }

            return sb.ToString();
        }

        public string ReadUnicodeStringSafe(int fixedLength)
        {
            var bound = Index + (fixedLength << 1);
            var end = bound;

            if (bound > Size)
            {
                bound = Size;
            }

            var sb = new StringBuilder();

            int c;

            while ((Index + 1) < bound && (c = ((Buffer[Index++] << 8) | Buffer[Index++])) != 0)
            {
                if (IsSafeChar(c))
                {
                    sb.Append((char) c);
                }
            }

            Index = end;

            return sb.ToString();
        }

        public string ReadUnicodeStringSafeReverse()
        {
            var sb = new StringBuilder();

            int c;

            while ((Index + 1) < Size && (c = ((Buffer[Index++]) | Buffer[Index++] << 8)) != 0)
            {
                if (IsSafeChar(c))
                {
                    sb.Append((char) c);
                }
            }

            return sb.ToString();
        }

        public string ReadUnicodeStringReverse(int fixedLength)
        {
            var bound = Index + (fixedLength << 1);
            var end = bound;

            if (bound > Size)
            {
                bound = Size;
            }

            var sb = new StringBuilder();

            int c;

            while ((Index + 1) < bound && (c = ((Buffer[Index++]) | Buffer[Index++] << 8)) != 0)
            {
                sb.Append((char) c);
            }

            Index = end;

            return sb.ToString();
        }

        public string ReadUnicodeString(int fixedLength)
        {
            var bound = Index + (fixedLength << 1);
            var end = bound;

            if (bound > Size)
            {
                bound = Size;
            }

            var sb = new StringBuilder();

            int c;

            while ((Index + 1) < bound && (c = ((Buffer[Index++] << 8) | Buffer[Index++])) != 0)
            {
                sb.Append((char) c);
            }

            Index = end;

            return sb.ToString();
        }

        public string ReadStringSafe(int fixedLength)
        {
            var bound = Index + fixedLength;
            var end = bound;

            if (bound > Size)
            {
                bound = Size;
            }

            var sb = new StringBuilder();

            int c;

            while (Index < bound && (c = Buffer[Index++]) != 0)
            {
                if (IsSafeChar(c))
                {
                    sb.Append((char) c);
                }
            }

            Index = end;

            return sb.ToString();
        }

        public string ReadString(int fixedLength)
        {
            var bound = Index + fixedLength;
            var end = bound;

            if (bound > Size)
            {
                bound = Size;
            }

            var sb = new StringBuilder();

            int c;

            while (Index < bound && (c = Buffer[Index++]) != 0)
            {
                sb.Append((char) c);
            }

            Index = end;

            return sb.ToString();
        }
    }
}