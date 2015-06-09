#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// SocketState.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region Usings

using System.Net.Sockets;

#endregion

namespace OpenUO.Core.Net
{
    public class SocketState
    {
        public SocketState(Socket socket, int bufferSize)
        {
            Socket = socket;
            Buffer = new byte[bufferSize];
            DataLength = 0;
        }

        public Socket Socket
        {
            get;
        }

        public byte[] Buffer
        {
            get;
            set;
        }

        public int DataLength
        {
            get;
            set;
        }
    }
}