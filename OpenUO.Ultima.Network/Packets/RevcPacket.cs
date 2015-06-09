#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// RevcPacket.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region usings



#endregion

using OpenUO.Core.Diagnostics.Tracing;

namespace OpenUO.Core.Net.Packets
{
    public abstract class RecvPacket : IRecvPacket
    {
        protected RecvPacket(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id
        {
            get;
        }

        public string Name
        {
            get;
        }

        public void NotImplementedWarning()
        {
            Tracer.Warn(string.Format("Client: unhandled packet 0x{0:X2} - {1}", Id, Name));
        }
    }
}