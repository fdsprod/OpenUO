#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// SendRecvPacket.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region Usings

using System.IO;

using OpenUO.Core.Diagnostics.Tracing;

#endregion

namespace OpenUO.Core.Net.Packets
{
    public abstract class SendRecvPacket : ISendPacket, IRecvPacket
    {
        private const int BufferSize = 4096;
        protected PacketWriter Stream;

        public SendRecvPacket(int id, string name)
        {
            Id = id;
            Name = name;
            Stream = PacketWriter.CreateInstance(Length);
            Stream.Write(id);
            Stream.Write((short) 0);
        }

        public SendRecvPacket(int id, string name, int length)
        {
            Id = id;
            Name = name;
            Length = length;

            Stream = PacketWriter.CreateInstance(length);
            Stream.Write((byte) id);
        }

        public int Id
        {
            get;
        }

        public string Name
        {
            get;
        }

        public int Length
        {
            get;
        }

        public void EnsureCapacity(int length)
        {
            Stream = PacketWriter.CreateInstance(length);
            Stream.Write((byte) Id);
            Stream.Write((short) length);
        }

        public byte[] Compile()
        {
            Stream.Flush();

            if (Length == 0)
            {
                var length = Stream.Length;
                Stream.Seek(1, SeekOrigin.Begin);
                Stream.Write((ushort) length);
                Stream.Flush();
            }

            return Stream.Compile();
        }

        public override string ToString()
        {
            return string.Format("Id: {0:X2} Name: {1} Length: {2}", Id, Name, Length);
        }

        public void NotImplementedWarning()
        {
            Tracer.Warn(string.Format("Client: unhandled packet 0x{0:X2} - {1}", Id, Name));
        }
    }
}