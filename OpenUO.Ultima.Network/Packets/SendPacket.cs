#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// SendPacket.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region Usings

using System.IO;

#endregion

namespace OpenUO.Core.Net.Packets
{
    /// <summary>
    /// A formatted unit of data used in point to point communications.  
    /// </summary>
    public abstract class SendPacket : ISendPacket
    {
        private const int BufferSize = 4096;

        /// <summary>
        /// Used to create the a buffered datablock to be sent
        /// </summary>
        protected PacketWriter Stream;

        /// <summary>
        /// Creates an instance of a packet
        /// </summary>
        /// <param name="id">the Id, or Command that identifies the packet</param>
        /// <param name="name">The name of the packet</param>
        public SendPacket(int id, string name)
        {
            Id = id;
            Name = name;
            Stream = PacketWriter.CreateInstance(Length);
            Stream.Write((byte) id);
            Stream.Write((short) 0);
        }

        /// <summary>
        /// Creates an instance of a packet
        /// </summary>
        /// <param name="id">the Id, or Command that identifies the packet</param>
        /// <param name="name">The name of the packet</param>
        /// <param name="length">The size in bytes of the packet</param>
        public SendPacket(int id, string name, int length)
        {
            Id = id;
            Name = name;
            Length = length;

            Stream = PacketWriter.CreateInstance(length);
            Stream.Write((byte) id);
        }

        /// <summary>
        /// Gets the name of the packet
        /// </summary>
        public string Name
        {
            get;
        }

        /// <summary>
        /// Gets the size in bytes of the packet
        /// </summary>
        public int Length
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the Id, or Command that identifies the packet.
        /// </summary>
        public int Id
        {
            get;
        }

        /// <summary>
        /// Resets the Packet Writer and ensures the packet's 2nd and 3rd bytes are used to store the length
        /// </summary>
        /// <param name="length"></param>
        public void EnsureCapacity(int length)
        {
            Stream = PacketWriter.CreateInstance(length);
            Stream.Write((byte) Id);
            Stream.Write((short) length);
        }

        /// <summary>
        /// Compiles the packet into a System.Byte[] and Disposes the underlying Stream
        /// </summary>
        /// <returns></returns>
        public byte[] Compile()
        {
            Stream.Flush();

            if (Length == 0)
            {
                Length = (int) Stream.Length;
                Stream.Seek(1, SeekOrigin.Begin);
                Stream.Write((ushort) Length);
            }

            return Stream.Compile();
        }

        public override string ToString()
        {
            return string.Format("Id: {0:X2} Name: {1} Length: {2}", Id, Name, Length);
        }
    }
}