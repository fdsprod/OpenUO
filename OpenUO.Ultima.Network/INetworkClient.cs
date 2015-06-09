#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// INetworkClient.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region Usings

using System.Net;

#endregion

namespace OpenUO.Core.Net
{
    public interface INetworkClient
    {
        int ClientAddress
        {
            get;
        }

        IPAddress ServerAddress
        {
            get;
        }

        bool IsDecompressionEnabled
        {
            get;
            set;
        }

        bool IsConnected
        {
            get;
        }

        void Register<T>(int id, string name, int length, PacketReceiveHandler<T> handler) where T : IRecvPacket;

        void RegisterExtended<T>(int extendedId, int subId, string name, int length, PacketReceiveHandler<T> handler) where T : IRecvPacket;

        bool Connect(string ipAddressOrHostName, int port);

        void Disconnect();

        void Slice();

        bool Send(ISendPacket packet);

        bool Send(byte[] buffer, int offset, int length, string name);

        void Unregister<T>(int id, PacketReceiveHandler<T> handler);

        void UnregisterExtended<T>(int extendedId, int subId, PacketReceiveHandler<T> handler);
    }
}