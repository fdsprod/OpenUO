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
using System.IO;
using System.Text;
using OpenUO.Core;
using OpenUO.Core.Diagnostics;

namespace OpenUO.Ultima.Network
{
    public class PacketReader
    {
        private readonly byte[] _data;
        private readonly int _size;
        private int _index;

        public int Index
        {
            get { return _index; }
        }

        public byte[] Buffer
        {
            get { return _data; }
        }

        public int Size
        {
            get { return _size; }
        }

        public PacketReader(byte[] data, int size, bool fixedSize)
        {
            _data = data;
            _size = size;
            _index = fixedSize ? 1 : 3;
        }

        public void Trace(NetState state)
        {
            try
            {
                string path = "packets.log";                

                using (StreamWriter sw = new StreamWriter(path, true))
                {
                    sw.BaseStream.Seek(sw.BaseStream.Length, SeekOrigin.Begin);

                    byte[] buffer = _data;

                    if (_data.Length > 0)
                        Tracer.Warn(string.Format("Unhandled packet {0} : 0x{0:X2}", _data[0]));

                    buffer.ToFormattedString(buffer.Length, sw);

                    sw.WriteLine();
                    sw.WriteLine();
                }
            }
            catch (Exception e)
            {
                Tracer.Error(e);
            }
        }

        public int Seek(int offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin: _index = offset; break;
                case SeekOrigin.Current: _index += offset; break;
                case SeekOrigin.End: _index = _size - offset; break;
            }

            return _index;
        }

        public float ReadSingle()
        {
            if ((_index + 4) > _size)
                return 0;

            uint num = (uint)((_data[_index++] << 24) | (_data[_index++] << 16) | (_data[_index++] << 8) | _data[_index++]);
            return num;
        }

        public double ReadDouble()
        {
            if ((_index + 8) > _size)
                return 0;

            ulong num = (((ulong)_data[_index++] << 56) |
                    ((ulong)_data[_index++] << 48) |
                    ((ulong)_data[_index++] << 40) |
                    ((ulong)_data[_index++] << 32) |
                    ((ulong)_data[_index++] << 24) |
                    ((ulong)_data[_index++] << 16) |
                    ((ulong)_data[_index++] << 8) |
                    ((ulong)_data[_index++]));
            return num;
        }

        public long ReadInt64()
        {
            if ((_index + 8) > _size)
                return 0;

            return (((long)_data[_index++] << 56) |
                    ((long)_data[_index++] << 48) |
                    ((long)_data[_index++] << 40) |
                    ((long)_data[_index++] << 32) |
                    ((long)_data[_index++] << 24) |
                    ((long)_data[_index++] << 16) |
                    ((long)_data[_index++] << 8) |
                    ((long)_data[_index++]));
        }

        public int ReadInt32()
        {
            if ((_index + 4) > _size)
                return 0;

            return (_data[_index++] << 24)
                 | (_data[_index++] << 16)
                 | (_data[_index++] << 8)
                 | _data[_index++];
        }

        public short ReadInt16()
        {
            if ((_index + 2) > _size)
                return 0;

            return (short)((_data[_index++] << 8) | _data[_index++]);
        }

        public byte ReadByte()
        {
            if ((_index + 1) > _size)
                return 0;

            return _data[_index++];
        }

        public byte[] ReadBytes(int count)
        {
            if ((_index + count) > _size)
                return new byte[0];

            byte[] outbuffer = new byte[count];
            Array.Copy(_data, _index, outbuffer, 0, count);
            _index += count;
            return outbuffer;
        }

        public ulong ReadUInt64()
        {
            if ((_index + 8) > _size)
                return 0;

            return (((ulong)_data[_index++] << 56) |
                    ((ulong)_data[_index++] << 48) |
                    ((ulong)_data[_index++] << 40) |
                    ((ulong)_data[_index++] << 32) |
                    ((ulong)_data[_index++] << 24) |
                    ((ulong)_data[_index++] << 16) |
                    ((ulong)_data[_index++] << 8) |
                    ((ulong)_data[_index++]));
        }

        public uint ReadUInt32()
        {
            if ((_index + 4) > _size)
                return 0;

            return (uint)((_data[_index++] << 24) | (_data[_index++] << 16) | (_data[_index++] << 8) | _data[_index++]);
        }

        public ushort ReadUInt16()
        {
            if ((_index + 2) > _size)
                return 0;

            return (ushort)((_data[_index++] << 8) | _data[_index++]);
        }

        public sbyte ReadSByte()
        {
            if ((_index + 1) > _size)
                return 0;

            return (sbyte)_data[_index++];
        }

        public bool ReadBoolean()
        {
            if ((_index + 1) > _size)
                return false;

            return (_data[_index++] != 0);
        }

        public string ReadUnicodeStringLE()
        {
            StringBuilder sb = new StringBuilder();

            int c;

            while ((_index + 1) < _size && (c = (_data[_index++] | (_data[_index++] << 8))) != 0)
                sb.Append((char)c);

            return sb.ToString();
        }

        public string ReadUnicodeStringLESafe(int fixedLength)
        {
            int bound = _index + (fixedLength << 1);
            int end = bound;

            if (bound > _size)
                bound = _size;

            StringBuilder sb = new StringBuilder();

            int c;

            while ((_index + 1) < bound && (c = (_data[_index++] | (_data[_index++] << 8))) != 0)
            {
                if (IsSafeChar(c))
                    sb.Append((char)c);
            }

            _index = end;

            return sb.ToString();
        }

        public string ReadUnicodeStringLESafe()
        {
            StringBuilder sb = new StringBuilder();

            int c;

            while ((_index + 1) < _size && (c = (_data[_index++] | (_data[_index++] << 8))) != 0)
            {
                if (IsSafeChar(c))
                    sb.Append((char)c);
            }

            return sb.ToString();
        }

        public string ReadUnicodeStringSafe()
        {
            StringBuilder sb = new StringBuilder();

            int c;

            while ((_index + 1) < _size && (c = ((_data[_index++] << 8) | _data[_index++])) != 0)
            {
                if (IsSafeChar(c))
                    sb.Append((char)c);
            }

            return sb.ToString();
        }

        public string ReadUnicodeString()
        {
            StringBuilder sb = new StringBuilder();

            int c;

            while ((_index + 1) < _size && (c = ((_data[_index++] << 8) | _data[_index++])) != 0)
                sb.Append((char)c);

            return sb.ToString();
        }

        public bool IsSafeChar(int c)
        {
            return (c >= 0x20 && c < 0xFFFE);
        }

        public string ReadUTF8StringSafe(int fixedLength)
        {
            if (_index >= _size)
            {
                _index += fixedLength;
                return String.Empty;
            }

            int bound = _index + fixedLength;
            //int end   = bound;

            if (bound > _size)
                bound = _size;

            int count = 0;
            int index = _index;
            int start = _index;

            while (index < bound && _data[index++] != 0)
                ++count;

            index = 0;

            byte[] buffer = new byte[count];
            int value;

            while (_index < bound && (value = _data[_index++]) != 0)
                buffer[index++] = (byte)value;

            string s = Encoding.UTF8.GetString(buffer, 0, buffer.Length);

            bool isSafe = true;

            for (int i = 0; isSafe && i < s.Length; ++i)
                isSafe = IsSafeChar(s[i]);

            _index = start + fixedLength;

            if (isSafe)
                return s;

            StringBuilder sb = new StringBuilder(s.Length);

            for (int i = 0; i < s.Length; ++i)
                if (IsSafeChar(s[i]))
                    sb.Append(s[i]);

            return sb.ToString();
        }

        public string ReadUTF8StringSafe()
        {
            if (_index >= _size)
                return String.Empty;

            int count = 0;
            int index = _index;

            while (index < _size && _data[index++] != 0)
                ++count;

            index = 0;

            byte[] buffer = new byte[count];
            int value;

            while (_index < _size && (value = _data[_index++]) != 0)
                buffer[index++] = (byte)value;

            string s = Encoding.UTF8.GetString(buffer, 0, buffer.Length);

            bool isSafe = true;

            for (int i = 0; isSafe && i < s.Length; ++i)
                isSafe = IsSafeChar(s[i]);

            if (isSafe)
                return s;

            StringBuilder sb = new StringBuilder(s.Length);

            for (int i = 0; i < s.Length; ++i)
            {
                if (IsSafeChar(s[i]))
                    sb.Append(s[i]);
            }

            return sb.ToString();
        }

        public string ReadUTF8String()
        {
            if (_index >= _size)
                return String.Empty;

            int count = 0;
            int index = _index;

            while (index < _size && _data[index++] != 0)
                ++count;

            index = 0;

            byte[] buffer = new byte[count];
            int value;

            while (_index < _size && (value = _data[_index++]) != 0)
                buffer[index++] = (byte)value;

            return Encoding.UTF8.GetString(buffer, 0, buffer.Length);
        }

        public string ReadString()
        {
            StringBuilder sb = new StringBuilder();

            int c;

            while (_index < _size && (c = _data[_index++]) != 0)
                sb.Append((char)c);

            return sb.ToString();
        }

        public string ReadStringSafe()
        {
            StringBuilder sb = new StringBuilder();

            int c;

            while (_index < _size && (c = _data[_index++]) != 0)
            {
                if (IsSafeChar(c))
                    sb.Append((char)c);
            }

            return sb.ToString();
        }

        public string ReadUnicodeStringSafe(int fixedLength)
        {
            int bound = _index + (fixedLength << 1);
            int end = bound;

            if (bound > _size)
                bound = _size;

            StringBuilder sb = new StringBuilder();

            int c;

            while ((_index + 1) < bound && (c = ((_data[_index++] << 8) | _data[_index++])) != 0)
            {
                if (IsSafeChar(c))
                    sb.Append((char)c);
            }

            _index = end;

            return sb.ToString();
        }

        public string ReadUnicodeString(int fixedLength)
        {
            int bound = _index + (fixedLength << 1);
            int end = bound;

            if (bound > _size)
                bound = _size;

            StringBuilder sb = new StringBuilder();

            int c;

            while ((_index + 1) < bound && (c = ((_data[_index++] << 8) | _data[_index++])) != 0)
                sb.Append((char)c);

            _index = end;

            return sb.ToString();
        }

        public string ReadStringSafe(int fixedLength)
        {
            int bound = _index + fixedLength;
            int end = bound;

            if (bound > _size)
                bound = _size;

            StringBuilder sb = new StringBuilder();

            int c;

            while (_index < bound && (c = _data[_index++]) != 0)
            {
                if (IsSafeChar(c))
                    sb.Append((char)c);
            }

            _index = end;

            return sb.ToString();
        }

        public string ReadString(int fixedLength)
        {
            int bound = _index + fixedLength;
            int end = bound;

            if (bound > _size)
                bound = _size;

            StringBuilder sb = new StringBuilder();

            int c;

            while (_index < bound && (c = _data[_index++]) != 0)
                sb.Append((char)c);

            _index = end;

            return sb.ToString();
        }
    }
}