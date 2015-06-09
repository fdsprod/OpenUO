#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// QueuedPacket.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region Usings

using System.Collections.Generic;

using OpenUO.Core.Net;

#endregion

namespace ParadoxUO.Net
{
    internal class QueuedPacket
    {
        public string Name;
        public byte[] PacketBuffer;
        public List<PacketHandler> PacketHandlers;
        public int RealLength;

        public QueuedPacket(string name, List<PacketHandler> packetHandlers, byte[] packetBuffer, int realLength)
        {
            Name = name;
            PacketHandlers = packetHandlers;
            PacketBuffer = packetBuffer;
            RealLength = realLength;
        }
    }
}