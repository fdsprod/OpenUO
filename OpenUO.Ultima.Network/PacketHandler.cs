#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// PacketHandler.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region Usings

using System;

#endregion

namespace OpenUO.Core.Net
{
    public delegate void PacketReceiveHandler(PacketReader reader);

    public delegate void PacketReceiveHandler<in TPacket>(TPacket packet);

    public class PacketHandler
    {
        private readonly PacketReceiveHandler m_Handler;

        public PacketHandler(int id, string name, int length, PacketReceiveHandler handler)
        {
            Id = id;
            Name = name;
            Length = length;

            m_Handler = handler;
        }

        public int Id
        {
            get;
            private set;
        }

        public int Length
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        protected virtual Type PacketType
        {
            get { return null; }
        }

        public bool CanCreatePacket
        {
            get { return PacketType != null; }
        }

        public IRecvPacket CreatePacket(PacketReader reader)
        {
            return (IRecvPacket) Activator.CreateInstance(PacketType, reader);
        }

        public virtual void Invoke(IRecvPacket recvPacket)
        {
            throw new NotImplementedException();
        }

        public void Invoke(PacketReader reader)
        {
            m_Handler.Invoke(reader);
        }

        public virtual Delegate GetHandler()
        {
            return m_Handler;
        }
    }

    public class PacketHandler<TPacket> : PacketHandler
    {
        private readonly PacketReceiveHandler<TPacket> m_Handler;

        public PacketHandler(int id, string name, int length, PacketReceiveHandler<TPacket> handler)
            : base(id, name, length, null)
        {
            m_Handler = handler;
        }

        protected override Type PacketType
        {
            get { return typeof (TPacket); }
        }

        public override Delegate GetHandler()
        {
            return m_Handler;
        }

        public override void Invoke(IRecvPacket recvPacket)
        {
            m_Handler.Invoke((TPacket) recvPacket);
        }
    }
}