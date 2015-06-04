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

        public PacketReader(byte[] data, int size, bool fixedSize)
        {
            _data = data;
            _size = size;
            Index = fixedSize ? 1 : 3;
        }

        public int Index
        {
            get;
            private set;
        }

        public byte[] Buffer
        {
            get { return _data; }
        }

        public int Size
        {
            get { return _size; }
        }

        public void Trace(NetState state)
        {
            try
            {
                var path = "packets.log";

                using(var sw = new StreamWriter(path, true))
                {
                    sw.BaseStream.Seek(sw.BaseStream.Length, SeekOrigin.Begin);

                    var buffer = _data;

                    if(_data.Length > 0)
                    {
                        Tracer.Warn(string.Format("Unhandled packet {0} : 0x{0:X2}", _data[0]));
                    }

                    buffer.ToFormattedString(buffer.Length, sw);

                    sw.WriteLine();
                    sw.WriteLine();
                }
            }
            catch(Exception e)
            {
                Tracer.Error(e);
            }
        }

        public int Seek(int offset, SeekOrigin origin)
        {
            switch(origin)
            {
                case SeekOrigin.Begin:
                    Index = offset;
                    break;
                case SeekOrigin.Current:
                    Index += offset;
                    break;
                case SeekOrigin.End:
                    Index = _size - offset;
                    break;
            }

            return Index;
        }

        public float ReadSingle()
        {
            if((Index + 4) > _size)
            {
                return 0;
            }

            var num = (uint)((_data[Index++] << 24) | (_data[Index++] << 16) | (_data[Index++] << 8) | _data[Index++]);
            return num;
        }

        public double ReadDouble()
        {
            if((Index + 8) > _size)
            {
                return 0;
            }

            var num = (((ulong)_data[Index++] << 56) |
                       ((ulong)_data[Index++] << 48) |
                       ((ulong)_data[Index++] << 40) |
                       ((ulong)_data[Index++] << 32) |
                       ((ulong)_data[Index++] << 24) |
                       ((ulong)_data[Index++] << 16) |
                       ((ulong)_data[Index++] << 8) |
                       _data[Index++]);
            return num;
        }

        public long ReadInt64()
        {
            if((Index + 8) > _size)
            {
                return 0;
            }

            return (((long)_data[Index++] << 56) |
                    ((long)_data[Index++] << 48) |
                    ((long)_data[Index++] << 40) |
                    ((long)_data[Index++] << 32) |
                    ((long)_data[Index++] << 24) |
                    ((long)_data[Index++] << 16) |
                    ((long)_data[Index++] << 8) |
                    _data[Index++]);
        }

        public int ReadInt32()
        {
            if((Index + 4) > _size)
            {
                return 0;
            }

            return (_data[Index++] << 24)
                   | (_data[Index++] << 16)
                   | (_data[Index++] << 8)
                   | _data[Index++];
        }

        public short ReadInt16()
        {
            if((Index + 2) > _size)
            {
                return 0;
            }

            return (short)((_data[Index++] << 8) | _data[Index++]);
        }

        public byte ReadByte()
        {
            if((Index + 1) > _size)
            {
                return 0;
            }

            return _data[Index++];
        }

        public byte[] ReadBytes(int count)
        {
            if((Index + count) > _size)
            {
                return new byte[0];
            }

            var outbuffer = new byte[count];
            Array.Copy(_data, Index, outbuffer, 0, count);
            Index += count;
            return outbuffer;
        }

        public ulong ReadUInt64()
        {
            if((Index + 8) > _size)
            {
                return 0;
            }

            return (((ulong)_data[Index++] << 56) |
                    ((ulong)_data[Index++] << 48) |
                    ((ulong)_data[Index++] << 40) |
                    ((ulong)_data[Index++] << 32) |
                    ((ulong)_data[Index++] << 24) |
                    ((ulong)_data[Index++] << 16) |
                    ((ulong)_data[Index++] << 8) |
                    _data[Index++]);
        }

        public uint ReadUInt32()
        {
            if((Index + 4) > _size)
            {
                return 0;
            }

            return (uint)((_data[Index++] << 24) | (_data[Index++] << 16) | (_data[Index++] << 8) | _data[Index++]);
        }

        public ushort ReadUInt16()
        {
            if((Index + 2) > _size)
            {
                return 0;
            }

            return (ushort)((_data[Index++] << 8) | _data[Index++]);
        }

        public sbyte ReadSByte()
        {
            if((Index + 1) > _size)
            {
                return 0;
            }

            return (sbyte)_data[Index++];
        }

        public bool ReadBoolean()
        {
            if((Index + 1) > _size)
            {
                return false;
            }

            return (_data[Index++] != 0);
        }

        public string ReadUnicodeStringLE()
        {
            var sb = new StringBuilder();

            int c;

            while((Index + 1) < _size && (c = (_data[Index++] | (_data[Index++] << 8))) != 0)
            {
                sb.Append((char)c);
            }

            return sb.ToString();
        }

        public string ReadUnicodeStringLESafe(int fixedLength)
        {
            var bound = Index + (fixedLength << 1);
            var end = bound;

            if(bound > _size)
            {
                bound = _size;
            }

            var sb = new StringBuilder();

            int c;

            while((Index + 1) < bound && (c = (_data[Index++] | (_data[Index++] << 8))) != 0)
            {
                if(IsSafeChar(c))
                {
                    sb.Append((char)c);
                }
            }

            Index = end;

            return sb.ToString();
        }

        public string ReadUnicodeStringLESafe()
        {
            var sb = new StringBuilder();

            int c;

            while((Index + 1) < _size && (c = (_data[Index++] | (_data[Index++] << 8))) != 0)
            {
                if(IsSafeChar(c))
                {
                    sb.Append((char)c);
                }
            }

            return sb.ToString();
        }

        public string ReadUnicodeStringSafe()
        {
            var sb = new StringBuilder();

            int c;

            while((Index + 1) < _size && (c = ((_data[Index++] << 8) | _data[Index++])) != 0)
            {
                if(IsSafeChar(c))
                {
                    sb.Append((char)c);
                }
            }

            return sb.ToString();
        }

        public string ReadUnicodeString()
        {
            var sb = new StringBuilder();

            int c;

            while((Index + 1) < _size && (c = ((_data[Index++] << 8) | _data[Index++])) != 0)
            {
                sb.Append((char)c);
            }

            return sb.ToString();
        }

        public bool IsSafeChar(int c)
        {
            return (c >= 0x20 && c < 0xFFFE);
        }

        public string ReadUTF8StringSafe(int fixedLength)
        {
            if(Index >= _size)
            {
                Index += fixedLength;
                return String.Empty;
            }

            var bound = Index + fixedLength;
            //int end   = bound;

            if(bound > _size)
            {
                bound = _size;
            }

            var count = 0;
            var index = Index;
            var start = Index;

            while(index < bound && _data[index++] != 0)
            {
                ++count;
            }

            index = 0;

            var buffer = new byte[count];
            int value;

            while(Index < bound && (value = _data[Index++]) != 0)
            {
                buffer[index++] = (byte)value;
            }

            var s = Encoding.UTF8.GetString(buffer, 0, buffer.Length);

            var isSafe = true;

            for(var i = 0; isSafe && i < s.Length; ++i)
            {
                isSafe = IsSafeChar(s[i]);
            }

            Index = start + fixedLength;

            if(isSafe)
            {
                return s;
            }

            var sb = new StringBuilder(s.Length);

            for(var i = 0; i < s.Length; ++i)
            {
                if(IsSafeChar(s[i]))
                {
                    sb.Append(s[i]);
                }
            }

            return sb.ToString();
        }

        public string ReadUTF8StringSafe()
        {
            if(Index >= _size)
            {
                return String.Empty;
            }

            var count = 0;
            var index = Index;

            while(index < _size && _data[index++] != 0)
            {
                ++count;
            }

            index = 0;

            var buffer = new byte[count];
            int value;

            while(Index < _size && (value = _data[Index++]) != 0)
            {
                buffer[index++] = (byte)value;
            }

            var s = Encoding.UTF8.GetString(buffer, 0, buffer.Length);

            var isSafe = true;

            for(var i = 0; isSafe && i < s.Length; ++i)
            {
                isSafe = IsSafeChar(s[i]);
            }

            if(isSafe)
            {
                return s;
            }

            var sb = new StringBuilder(s.Length);

            for(var i = 0; i < s.Length; ++i)
            {
                if(IsSafeChar(s[i]))
                {
                    sb.Append(s[i]);
                }
            }

            return sb.ToString();
        }

        public string ReadUTF8String()
        {
            if(Index >= _size)
            {
                return String.Empty;
            }

            var count = 0;
            var index = Index;

            while(index < _size && _data[index++] != 0)
            {
                ++count;
            }

            index = 0;

            var buffer = new byte[count];
            int value;

            while(Index < _size && (value = _data[Index++]) != 0)
            {
                buffer[index++] = (byte)value;
            }

            return Encoding.UTF8.GetString(buffer, 0, buffer.Length);
        }

        public string ReadString()
        {
            var sb = new StringBuilder();

            int c;

            while(Index < _size && (c = _data[Index++]) != 0)
            {
                sb.Append((char)c);
            }

            return sb.ToString();
        }

        public string ReadStringSafe()
        {
            var sb = new StringBuilder();

            int c;

            while(Index < _size && (c = _data[Index++]) != 0)
            {
                if(IsSafeChar(c))
                {
                    sb.Append((char)c);
                }
            }

            return sb.ToString();
        }

        public string ReadUnicodeStringSafe(int fixedLength)
        {
            var bound = Index + (fixedLength << 1);
            var end = bound;

            if(bound > _size)
            {
                bound = _size;
            }

            var sb = new StringBuilder();

            int c;

            while((Index + 1) < bound && (c = ((_data[Index++] << 8) | _data[Index++])) != 0)
            {
                if(IsSafeChar(c))
                {
                    sb.Append((char)c);
                }
            }

            Index = end;

            return sb.ToString();
        }

        public string ReadUnicodeString(int fixedLength)
        {
            var bound = Index + (fixedLength << 1);
            var end = bound;

            if(bound > _size)
            {
                bound = _size;
            }

            var sb = new StringBuilder();

            int c;

            while((Index + 1) < bound && (c = ((_data[Index++] << 8) | _data[Index++])) != 0)
            {
                sb.Append((char)c);
            }

            Index = end;

            return sb.ToString();
        }

        public string ReadStringSafe(int fixedLength)
        {
            var bound = Index + fixedLength;
            var end = bound;

            if(bound > _size)
            {
                bound = _size;
            }

            var sb = new StringBuilder();

            int c;

            while(Index < bound && (c = _data[Index++]) != 0)
            {
                if(IsSafeChar(c))
                {
                    sb.Append((char)c);
                }
            }

            Index = end;

            return sb.ToString();
        }

        public string ReadString(int fixedLength)
        {
            var bound = Index + fixedLength;
            var end = bound;

            if(bound > _size)
            {
                bound = _size;
            }

            var sb = new StringBuilder();

            int c;

            while(Index < bound && (c = _data[Index++]) != 0)
            {
                sb.Append((char)c);
            }

            Index = end;

            return sb.ToString();
        }
    }
}