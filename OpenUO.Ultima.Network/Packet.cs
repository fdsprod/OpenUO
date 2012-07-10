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
using System.Diagnostics;
using System.IO;

namespace OpenUO.Ultima.Network
{
    public abstract class Packet
    {
        [Flags]
        private enum State
        {
            Inactive = 0x00,
            Static = 0x01,
            Acquired = 0x02,
            Accessed = 0x04,
            Buffered = 0x08,
            Warned = 0x10
        }

        private const int BufferSize = 4096;

        private static readonly BufferPool _buffers = new BufferPool("Compressed", 16, BufferSize);

        protected PacketWriter Stream;

        private readonly int _packetID;
        private readonly int _length;
        private State _state;
        private readonly string _name;

        public string Name
        {
            get { return _name; }
        } 

        public PacketWriter UnderlyingStream
        {
            get { return Stream; }
        }

        public int PacketID
        {
            get { return _packetID; }
        }

        protected Packet(int packetID, string name)
        {
            _packetID = packetID;
        }

        protected Packet(int packetID, string name, int length)
        {
            _packetID = packetID;
            _length = length;

            Stream = PacketWriter.CreateInstance(length);
            Stream.Write((byte)packetID);
        }

        public void EnsureCapacity(int length)
        {
            Stream = PacketWriter.CreateInstance(length);
            Stream.Write((byte)_packetID);
            Stream.Write((short)0);
        }

        public static Packet SetStatic(Packet p)
        {
            p.SetStatic();
            return p;
        }

        public static Packet Acquire(Packet p)
        {
            p.Acquire();
            return p;
        }

        public static void Release(ref Packet p)
        {
            if (p != null)
                p.Release();

            p = null;
        }

        public static void Release(Packet p)
        {
            if (p != null)
                p.Release();
        }

        public void SetStatic()
        {
            _state |= State.Static | State.Acquired;
        }

        public void Acquire()
        {
            _state |= State.Acquired;
        }

        public void OnSend()
        {
            if ((_state & (State.Acquired | State.Static)) == 0)
                Free();
        }

        private void Free()
        {
            if (_compiledBuffer == null)
                return;

            if ((_state & State.Buffered) != 0)
                _buffers.ReleaseBuffer(_compiledBuffer);

            _state &= ~(State.Static | State.Acquired | State.Buffered);

            _compiledBuffer = null;
        }

        public void Release()
        {
            if ((_state & State.Acquired) != 0)
                Free();
        }

        private byte[] _compiledBuffer;
        private int _compiledLength;

        public byte[] Compile(bool compress, out int length)
        {
            if (_compiledBuffer == null)
            {
                if ((_state & State.Accessed) == 0)
                {
                    _state |= State.Accessed;
                }
                else
                {
                    if ((_state & State.Warned) == 0)
                    {
                        _state |= State.Warned;

                        try
                        {
                            //using ( StreamWriter op = new StreamWriter( "net_opt.log", true ) )
                            //{
                            Debug.WriteLine(string.Format("Redundant compile for packet {0}, use Acquire() and Release()", this.GetType()));
                            Debug.WriteLine(new System.Diagnostics.StackTrace());
                            //}
                        }
                        catch
                        {
                        }
                    }

                    _compiledBuffer = new byte[0];
                    _compiledLength = 0;

                    length = _compiledLength;
                    return _compiledBuffer;
                }

                InternalCompile(compress);
            }

            length = _compiledLength;
            return _compiledBuffer;
        }

        private void InternalCompile(bool compress)
        {
            if (_length == 0)
            {
                long streamLen = Stream.Length;

                Stream.Seek(1, SeekOrigin.Begin);
                Stream.Write((ushort)streamLen);
            }
            else if (Stream.Length != _length)
            {
                int diff = (int)Stream.Length - _length;

                Debug.WriteLine("Packet: 0x{0:X2}: Bad packet length! ({1}{2} bytes)", _packetID, diff >= 0 ? "+" : "", diff);
            }

            MemoryStream ms = Stream.UnderlyingStream;

            _compiledBuffer = ms.GetBuffer();
            int length = (int)ms.Length;

            if (_compiledBuffer != null)
            {
                _compiledLength = length;

                byte[] old = _compiledBuffer;

                if (length > BufferSize || (_state & State.Static) != 0)
                {
                    _compiledBuffer = new byte[length];
                }
                else
                {
                    _compiledBuffer = _buffers.AcquireBuffer();
                    _state |= State.Buffered;
                }

                Buffer.BlockCopy(old, 0, _compiledBuffer, 0, length);
            }

            PacketWriter.ReleaseInstance(Stream);
            Stream = null;
        }
    }
}
