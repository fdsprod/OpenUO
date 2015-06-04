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
using System.Net;
using System.Net.Sockets;
using OpenUO.Core.Diagnostics;

namespace OpenUO.Ultima.Network
{
    public class NetState
    {
        private readonly MessagePump _messagePump;
        private readonly BufferPool _recvBuffer;
        private IPAddress _address;
        private IPEndPoint _hostEndPoint;
        private bool _isDisposing;
        private Socket _socket;

        public NetState()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _recvBuffer = new BufferPool("Receive Buffer", 16, 4096);
            _messagePump = new MessagePump();
            Buffer = new ByteQueue();
        }

        public event EventHandler Connected;

        public ByteQueue Buffer
        {
            get;
            private set;
        }

        public bool IsConnected
        {
            get;
            private set;
        }

        public DateTime ConnectedOn
        {
            get;
            private set;
        }

        public TimeSpan ConnectedFor
        {
            get { return (DateTime.Now - ConnectedOn); }
        }

        public IPAddress Address
        {
            get { return _address; }
        }

        public bool IsCompressionEnabled
        {
            get;
            set;
        }

        public void BeginReceive()
        {
            var args = new SocketAsyncEventArgs();

            var buffer = _recvBuffer.AcquireBuffer();

            args.SetBuffer(buffer, 0, buffer.Length);
            args.UserToken = this;
            args.RemoteEndPoint = _hostEndPoint;
            args.Completed += OnReceive;

            _socket.ReceiveAsync(args);
        }

        public void Connect(string ip, int port)
        {
            if(!IPAddress.TryParse(ip, out _address))
            {
                throw new Exception("Invalid IP, the ip must be a valid ip address and cannot be a host name.");
            }

            _hostEndPoint = new IPEndPoint(_address, port);

            var connectArgs = new SocketAsyncEventArgs();

            connectArgs.UserToken = this;
            connectArgs.RemoteEndPoint = _hostEndPoint;
            connectArgs.Completed += OnConnected;

            _socket.ConnectAsync(connectArgs);
        }

        private void OnConnected(object sender, SocketAsyncEventArgs e)
        {
            e.Completed -= OnConnected;

            var errorCode = e.SocketError;

            if(errorCode != SocketError.Success)
            {
                throw new SocketException((Int32)errorCode);
            }

            IsConnected = true;
            ConnectedOn = DateTime.Now;

            var handler = Connected;

            if(handler != null)
            {
                handler(this, EventArgs.Empty);
            }

            BeginReceive();
        }

        public virtual void Send(Packet p)
        {
            if(_socket == null)
            {
                p.OnSend();
                return;
            }

            int length;
            var buffer = p.Compile(IsCompressionEnabled, out length);

            if(buffer != null)
            {
                if(buffer.Length <= 0 || length <= 0)
                {
                    p.OnSend();
                    return;
                }

                try
                {
                    var args = new SocketAsyncEventArgs();

                    args.SetBuffer(buffer, 0, length);
                    args.RemoteEndPoint = _hostEndPoint;
                    args.UserToken = this;

                    _socket.SendAsync(args);
                }
                catch(CapacityExceededException)
                {
                    Tracer.Error("Too much data pending, disconnecting...");
                    Dispose();
                }
                catch(Exception ex)
                {
                    Tracer.Error(ex);
                    Dispose();
                }

                p.OnSend();
            }
            else
            {
                Tracer.Error("Null buffer send, disconnecting...");
                Tracer.Error(new StackTrace());

                Dispose();
            }
        }

        private void OnReceive(object sender, SocketAsyncEventArgs e)
        {
            e.Completed -= OnConnected;

            var errorCode = e.SocketError;

            if(errorCode != SocketError.Success)
            {
                throw new SocketException((Int32)errorCode);
            }

            try
            {
                var byteCount = e.BytesTransferred;

                if(byteCount > 0)
                {
                    var buffer = e.Buffer;

                    try
                    {
                        BeginReceive();
                    }
                    catch(Exception ex)
                    {
                        Tracer.Error(ex);
                        Dispose();
                    }

                    if(IsCompressionEnabled)
                    {
                        var unpackBuffer = _recvBuffer.AcquireBuffer();

                        int unpackSize;
                        var sourceIndex = 0;
                        var sourceSize = byteCount;

                        while(HuffmanCompression.Decompress(buffer, ref sourceIndex, sourceSize, unpackBuffer, out unpackSize))
                        {
                            lock(Buffer)
                                Buffer.Enqueue(unpackBuffer, 0, unpackSize);

                            _messagePump.OnReceive(this);
                        }

                        _recvBuffer.ReleaseBuffer(buffer);
                        _recvBuffer.ReleaseBuffer(unpackBuffer);
                    }
                    else
                    {
                        lock(Buffer)
                            Buffer.Enqueue(buffer, 0, byteCount);

                        _recvBuffer.ReleaseBuffer(buffer);
                        _messagePump.OnReceive(this);
                    }
                }
                else
                {
                    Dispose();
                }
            }
            catch
            {
                Dispose();
            }
        }

        public void Dispose()
        {
            if(_socket == null || _isDisposing)
            {
                return;
            }

            _isDisposing = true;

            try
            {
                _socket.Shutdown(SocketShutdown.Both);
            }
            catch(SocketException ex)
            {
                Tracer.Error(ex);
            }

            try
            {
                _socket.Close();
            }
            catch(SocketException ex)
            {
                Tracer.Error(ex);
            }

            _socket = null;
            Buffer = null;
        }

        internal PacketHandler GetHandler(int packetID)
        {
            return PacketHandlers.GetHandler(packetID);
        }
    }
}