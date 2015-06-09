#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// NetworkClient.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

using OpenUO.Core;
using OpenUO.Core.Collections;
using OpenUO.Core.Configuration;
using OpenUO.Core.Diagnostics.Performance;
using OpenUO.Core.Diagnostics.Tracing;
using OpenUO.Core.Net;
using OpenUO.Core.Net.Compression;
using OpenUO.Core.Patterns;

using SiliconStudio.Paradox.Games;

#endregion

namespace ParadoxUO.Net
{
    internal sealed class NetworkClient : ClientSystemBase, INetworkClient
    {
        private readonly HuffmanDecompression _decompression;
        private readonly List<PacketHandler>[][] _extendedHandlers;
        private readonly List<PacketHandler>[] _handlers;
        private readonly WorkingQueue<QueuedPacket> _queue = new WorkingQueue<QueuedPacket>();
        private readonly object _syncRoot = new object();
        private byte[] _appendData;
        private bool _appendNextMessage;
        private byte[] _receiveBuffer;
        private int _receiveBufferPosition;
        private IPAddress _serverAddress;
        private IPEndPoint _serverEndPoint;
        private Socket _serverSocket;

        public NetworkClient(IContainer container)
            : base(container)
        {
            _decompression = new HuffmanDecompression();
            IsDecompressionEnabled = false;

            _handlers = new List<PacketHandler>[0x100];
            _extendedHandlers = new List<PacketHandler>[0x100][];

            for (var i = 0; i < _handlers.Length; i++)
            {
                _handlers[i] = new List<PacketHandler>();
            }
        }

        public int ClientAddress
        {
            get
            {
                var localEntry = Dns.GetHostEntry(Dns.GetHostName());
                int address;

                if (localEntry.AddressList.Length > 0)
                {
#pragma warning disable 618
                    address = (int) localEntry.AddressList[0].Address;
#pragma warning restore 618
                }
                else
                {
                    address = 0x100007f;
                }

                return ((((address & 0xff) << 0x18) | ((address & 65280) << 8)) | ((address >> 8) & 65280)) | ((address >> 0x18) & 0xff);
            }
        }

        public IPAddress ServerAddress
        {
            get { return _serverAddress; }
        }

        public bool IsDecompressionEnabled
        {
            get;
            set;
        }

        public bool IsConnected
        {
            get;
            private set;
        }

        public void Register<T>(int id, string name, int length, PacketReceiveHandler<T> onReceive) where T : IRecvPacket
        {
            var type = typeof (T);
            var ctors = type.GetConstructors();

            var valid = false;

            for (var i = 0; i < ctors.Length && !valid; i++)
            {
                var parameters = ctors[i].GetParameters();
                valid = (parameters.Length == 1 && parameters[0].ParameterType == typeof (PacketReader));
            }

            if (!valid)
            {
                throw new NetworkException(string.Format("Unable to register packet type {0} without a public constructor with a {1} parameter", type, typeof (PacketReader)));
            }

            if (id > byte.MaxValue)
            {
                throw new NetworkException(string.Format("Unable to register packet id {0:X2} because it is greater than byte.MaxValue", id));
            }

            var handler = new PacketHandler<T>(id, name, length, onReceive);

            _handlers[id].Add(handler);
        }

        public void RegisterExtended<T>(int extendedId, int subId, string name, int length, PacketReceiveHandler<T> onReceive) where T : IRecvPacket
        {
            var type = typeof (T);
            var ctors = type.GetConstructors();

            var valid = false;

            for (var i = 0; i < ctors.Length && !valid; i++)
            {
                var parameters = ctors[i].GetParameters();
                valid = (parameters.Length == 1 && parameters[0].ParameterType == typeof (PacketReader));
            }

            if (!valid)
            {
                throw new NetworkException(string.Format("Unable to register packet type {0} without a public constructor with a {1} parameter", type, typeof (PacketReader)));
            }

            if (extendedId > byte.MaxValue)
            {
                throw new NetworkException(string.Format("Unable to register packet extendedId {0:X2} because it is greater than byte.MaxValue", extendedId));
            }

            if (subId > byte.MaxValue)
            {
                throw new NetworkException(string.Format("Unable to register packet subId {0:X2} because it is greater than byte.MaxValue", subId));
            }

            if (_extendedHandlers[extendedId] == null)
            {
                _extendedHandlers[extendedId] = new List<PacketHandler>[0x100];

                for (var i = 0; i < _extendedHandlers[extendedId].Length; i++)
                {
                    _extendedHandlers[extendedId][i] = new List<PacketHandler>();
                }
            }

            Tracer.Debug("Registering Extended Command: Id: 0x{0:X2} SubCommand: 0x{1:X2} Name: {2} Length: {3}", extendedId, subId, name, length);

            var handler = new PacketHandler<T>(subId, name, length, onReceive);

            _extendedHandlers[extendedId][subId].Add(handler);
        }

        public bool Connect(string ipAddressOrHostName, int port)
        {
            if (IsConnected)
            {
                Disconnect();
            }

            var success = true;

            try
            {
                if (!IPAddress.TryParse(ipAddressOrHostName, out _serverAddress))
                {
                    var ipAddresses = Dns.GetHostAddresses(ipAddressOrHostName);

                    if (ipAddresses.Length == 0)
                    {
                        throw new NetworkException("Host address was unreachable or invalid, unable to obtain an ip address.");
                    }

                    // On Vista and later, the first ip address is an empty one '::1'.
                    // This makes sure we choose the first valid ip address.
                    foreach (var address in ipAddresses)
                    {
                        if (address.ToString().Length > 7)
                        {
                            _serverAddress = address;
                            break;
                        }
                    }
                }

                _serverEndPoint = new IPEndPoint(_serverAddress, port);

                Tracer.Debug("Connecting...");

                _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _serverSocket.Connect(_serverEndPoint);

                if (_serverSocket.Connected)
                {
                    Tracer.Debug("Connected.");

                    var state = new SocketState(_serverSocket, ushort.MaxValue);
                    _serverSocket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, OnReceive, state);
                }
            }
            catch
            {
                success = false;
            }

            IsConnected = success;
            return success;
        }

        public void Disconnect()
        {
            if (_serverSocket != null && _serverSocket.Connected)
            {
                try
                {
                    _serverSocket.Shutdown(SocketShutdown.Both);
                    _serverSocket.Close();
                }

                    // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                }

                _serverSocket = null;
                _serverEndPoint = null;
                IsDecompressionEnabled = false;
                Tracer.Debug("Disconnected.");
                IsConnected = false;
            }
        }

        public bool Send(ISendPacket packet)
        {
            var buffer = packet.Compile();

            if (IsConnected)
            {
                var success = Send(buffer, 0, packet.Length, packet.Name);
                if (!success)
                {
                    Disconnect();
                }
                return success;
            }

            return false;
        }

        public bool Send(byte[] buffer, int offset, int length, string name)
        {
            var success = true;

            if (buffer == null || buffer.Length == 0)
            {
                throw new NetworkException("Unable to send, buffer was null or empty");
            }

            TracePacket(buffer, name, length, false);

            try
            {
                lock (_serverSocket)
                {
                    _serverSocket.Send(buffer, offset, length, SocketFlags.None);
                }
            }
            catch (Exception e)
            {
                Tracer.Debug(e.ToString());
                success = false;
            }

            return success;
        }

        public void Unregister<T>(int id, PacketReceiveHandler<T> onRecieve)
        {
            for (var i = 0; i < _handlers[id].Count; i++)
            {
                var handler = _handlers[id][i] as PacketHandler<T>;

                if (handler != null)
                {
                    var receiveHandler = handler.GetHandler();

                    if (receiveHandler != null && receiveHandler.Method.Equals(onRecieve.Method))
                    {
                        _handlers[id].RemoveAt(i);
                        break;
                    }
                }
                else
                {
                    Tracer.Critical("Unable to unregister this onReceive method.");
                }
            }
        }

        public void UnregisterExtended<T>(int id, int subId, PacketReceiveHandler<T> onRecieve)
        {
            for (var i = 0; i < _extendedHandlers[id].Length; i++)
            {
                for (var j = 0; j < _extendedHandlers[id][i].Count; j++)
                {
                    var handler = _extendedHandlers[id][i][j] as PacketHandler<T>;

                    if (handler != null)
                    {
                        var receiveHandler = handler.GetHandler();

                        if (receiveHandler != null && receiveHandler.Method.Equals(onRecieve.Method))
                        {
                            _extendedHandlers[id][i].RemoveAt(j);
                            break;
                        }
                    }
                    else
                    {
                        Tracer.Critical("Unable to unregister this onReceive method.");
                    }
                }
            }
        }

        public void Slice()
        {
            using (PerformanceTracer.BeginPerformanceTrace("NetworkClient.Slice"))
            {
                _queue.Slice();

                while (_queue.Count > 0)
                {
                    var packet = _queue.Dequeue();

                    TracePacket(packet.PacketBuffer, packet.Name, packet.RealLength);
                    InvokeHandlers(packet.PacketHandlers, packet.PacketBuffer, packet.RealLength);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Slice();
        }

        private void OnReceive(IAsyncResult result)
        {
            var state = result.AsyncState as SocketState;

            if (state == null)
            {
                Tracer.Warn("Socket state was null.");
                return;
            }

            try
            {
                var socket = state.Socket;
                if (socket.Connected == false)
                {
                    Disconnect();
                    return;
                }
                var length = socket.EndReceive(result);

                if (length > 0)
                {
                    var buffer = state.Buffer;

                    if (_receiveBuffer == null)
                    {
                        _receiveBuffer = new byte[0x10000];
                    }

                    if (IsDecompressionEnabled)
                    {
                        var outsize = 0;
                        byte[] data;

                        if (_appendNextMessage)
                        {
                            _appendNextMessage = false;
                            data = new byte[_appendData.Length + length];

                            Buffer.BlockCopy(_appendData, 0, data, 0, _appendData.Length);
                            Buffer.BlockCopy(buffer, 0, data, _appendData.Length, length);
                        }
                        else
                        {
                            data = new byte[length];
                            Buffer.BlockCopy(buffer, 0, data, 0, length);
                        }

                        while (_decompression.DecompressOnePacket(ref data, data.Length, ref _receiveBuffer, ref outsize))
                        {
                            int realLength;
                            var subId = _receiveBuffer.Length >= 5 ? (short) (_receiveBuffer[4] | (_receiveBuffer[3] << 8)) : (short) -1;
                            var packetHandlers = GetHandlers(_receiveBuffer[0]);
                            List<PacketHandler> extendedHandlers = null;

                            if (_receiveBuffer.Length >= 5)
                            {
                                extendedHandlers = GetExtendedHandlers(_receiveBuffer[0], subId);
                            }

                            if (packetHandlers.Count == 0 && (extendedHandlers == null || extendedHandlers.Count == 0))
                            {
                                if (subId != -1)
                                {
                                    Tracer.Warn("Unhandled packet with id: 0x{0:x2}, possible subid: 0x{1:x2}", _receiveBuffer[0], subId);
                                }
                                else
                                {
                                    Tracer.Warn("Unhandled packet with id: 0x{0:x2}", _receiveBuffer[0]);
                                }

                                _receiveBufferPosition = 0;
                                break;
                            }

                            var isExtendedHandler = packetHandlers.Count == 0;

                            if (extendedHandlers != null)
                            {
                                packetHandlers.AddRange(extendedHandlers);
                            }

                            GetPacketSize(packetHandlers, out realLength);

                            if (realLength != outsize)
                            {
                                throw new Exception("Bad packet size!");
                            }

                            var name = packetHandlers[0].Name;
                            var packetBuffer = new byte[realLength];

                            Buffer.BlockCopy(_receiveBuffer, isExtendedHandler ? 5 : 0, packetBuffer, 0, realLength);

                            AddPacket(name, packetHandlers, packetBuffer, realLength);
                        }

                        // We've run out of data to parse, or the packet was incomplete. If the packet was incomplete,
                        // we should save what's left for socket receive event.
                        if (data.Length > 0)
                        {
                            _appendNextMessage = true;
                            _appendData = data;
                        }
                    }
                    else
                    {
                        Buffer.BlockCopy(buffer, 0, _receiveBuffer, _receiveBufferPosition, length);

                        _receiveBufferPosition += length;

                        var currentIndex = 0;

                        while (currentIndex < _receiveBufferPosition)
                        {
                            int realLength;
                            var subId = _receiveBuffer.Length >= 5 ? (short) (_receiveBuffer[4] | (_receiveBuffer[3] << 8)) : (short) -1;
                            var packetHandlers = GetHandlers(_receiveBuffer[0]);
                            List<PacketHandler> extendedHandlers = null;

                            if (_receiveBuffer.Length >= 5)
                            {
                                extendedHandlers = GetExtendedHandlers(_receiveBuffer[0], subId);
                            }

                            if (packetHandlers.Count == 0 && extendedHandlers == null)
                            {
                                if (subId != -1)
                                {
                                    Tracer.Warn("Unhandled packet with id: 0x{0:x2}, possible subid: 0x{1:x2}", _receiveBuffer[0], subId);
                                }
                                else
                                {
                                    Tracer.Warn("Unhandled packet with id: 0x{0:x2}", _receiveBuffer[0]);
                                }

                                _receiveBufferPosition = 0;
                                break;
                            }

                            var isExtendedHandler = packetHandlers.Count == 0;

                            if (extendedHandlers != null)
                            {
                                packetHandlers.AddRange(extendedHandlers);
                            }

                            GetPacketSize(packetHandlers, out realLength);

                            if ((_receiveBufferPosition - currentIndex) >= realLength)
                            {
                                var name = packetHandlers[0].Name;
                                var packetBuffer = new byte[realLength];

                                Buffer.BlockCopy(_receiveBuffer, currentIndex + (isExtendedHandler ? 4 : 0), packetBuffer, 0, realLength);

                                AddPacket(name, packetHandlers, packetBuffer, realLength);

                                currentIndex += realLength;
                            }
                            else
                            {
                                //Need more data
                                break;
                            }
                        }

                        _receiveBufferPosition -= currentIndex;

                        if (_receiveBufferPosition > 0)
                        {
                            Buffer.BlockCopy(_receiveBuffer, currentIndex, _receiveBuffer, 0, _receiveBufferPosition);
                        }
                    }
                }

                if (_serverSocket != null)
                {
                    _serverSocket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, OnReceive, state);
                }
            }
            catch (Exception e)
            {
                Tracer.Debug(e.ToString());
                Disconnect();
            }
        }

        private void AddPacket(string name, List<PacketHandler> packetHandlers, byte[] packetBuffer, int realLength)
        {
            lock (_syncRoot)
            {
                _queue.Enqueue(new QueuedPacket(name, packetHandlers, packetBuffer, realLength));
            }
        }

        private void GetPacketSize(List<PacketHandler> packetHandlers, out int realLength)
        {
            realLength = 0;

            if (packetHandlers.Count > 0)
            {
                if (packetHandlers[0].Length == -1)
                {
                    realLength = _receiveBuffer[2] | (_receiveBuffer[1] << 8);
                }
                else
                {
                    realLength = packetHandlers[0].Length;
                }
            }
        }

        private void TracePacket(byte[] buffer, string name, int length, bool servertoclient = true)
        {
            if (Settings.Debug.LogPackets)
            {
                var builder = new StringBuilder();

                builder.AppendLine();
                builder.AppendLine(servertoclient ? "Server - > Client" : "Client - > Server");
                builder.AppendLineFormat("Id: 0x{0:X2} Name: {1} Length: {2}", buffer[0], name, length);
                builder.AppendLineFormat("{1}{0}", buffer.ToFormattedString(length), Environment.NewLine);

                Tracer.Debug(builder.ToString());
            }
        }

        private void InvokeHandlers(List<PacketHandler> packetHandlers, byte[] buffer, int length)
        {
            if (packetHandlers == null)
            {
                return;
            }

            var count = packetHandlers.Count;

            for (var i = 0; i < count; i++)
            {
                var handler = packetHandlers[i];

                try
                {
                    if (handler.CanCreatePacket)
                    {
                        var reader = PacketReader.CreateInstance(buffer, length, handler.Length != -1);
                        var recvPacket = handler.CreatePacket(reader);

                        handler.Invoke(recvPacket);
                    }
                    else
                    {
                        var reader = PacketReader.CreateInstance(buffer, length, packetHandlers[i].Length != -1);
                        handler.Invoke(reader);
                    }
                }
                catch (Exception e)
                {
                    Tracer.Error(e, "An error occurred while handling a packet for Packet { Id: {0}, Name: {1} }.", handler.Id, handler.Name);
                }
            }
        }

        private List<PacketHandler> GetHandlers(byte cmd)
        {
            var packetHandlers = new List<PacketHandler>();

            packetHandlers.AddRange(_handlers[cmd]);

            return packetHandlers;
        }

        private List<PacketHandler> GetExtendedHandlers(byte cmd, short subCommand)
        {
            var packetHandlers = new List<PacketHandler>();

            if (_extendedHandlers[cmd] != null)
            {
                packetHandlers.AddRange(_extendedHandlers[cmd][subCommand]);
            }

            return packetHandlers;
        }
    }
}