#region License Header
/***************************************************************************
 *   Copyright (c) 2011 OpenUO Software Team.
 *   All Right Reserved.
 *
 *   $Id: $:
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 ***************************************************************************/
 #endregion

using System;
using System.Net;
using OpenUO.Ultima.Network;

namespace Client.Network
{
    public interface INetwork : IDisposable
    {
        bool IsConnected { get; }
        bool CompressionEnabled { get; set; }
        IPAddress Address { get; }
        ByteQueue Buffer { get; }
        TimeSpan ConnectedFor { get; }
        DateTime ConnectedOn { get; }

        event EventHandler Connected;

        void Connect(string ip, int port);
        void Send(Packet p);
    }
}
