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
        private readonly BufferPool _recvBuffer;
        private readonly MessagePump _messagePump;

        private Socket _socket;
        private IPAddress _address;
        private IPEndPoint _hostEndPoint;
        private ByteQueue _buffer;
        private DateTime _connectedOn;

        private bool _isCompressionEnabled;
        private bool _isConnected;
        private bool _isDisposing;

        public ByteQueue Buffer
        {
            get { return _buffer; }
        }

        public bool IsConnected
        {
            get { return _isConnected; }
        }

        public DateTime ConnectedOn
        {
            get { return _connectedOn; }
        }

        public TimeSpan ConnectedFor
        {
            get { return (DateTime.Now - _connectedOn); }
        }

        public IPAddress Address
        {
            get { return _address; }
        }

        public bool IsCompressionEnabled
        {
            get { return _isCompressionEnabled; }
            set { _isCompressionEnabled = value; }
        }

        public event EventHandler Connected;

        public NetState()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _recvBuffer = new BufferPool("Receive Buffer", 16, 4096);
            _messagePump = new MessagePump();
            _buffer = new ByteQueue();
        }

        public void BeginReceive()
        {
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();

            byte[] buffer = _recvBuffer.AcquireBuffer();

            args.SetBuffer(buffer, 0, buffer.Length);
            args.UserToken = this;
            args.RemoteEndPoint = _hostEndPoint;
            args.Completed += OnReceive;

            _socket.ReceiveAsync(args);
        }

        public void Connect(string ip, int port)
        {
            if (!IPAddress.TryParse(ip, out _address))
                throw new Exception("Invalid IP, the ip must be a valid ip address and cannot be a host name.");

            _hostEndPoint = new IPEndPoint(_address, port);

            SocketAsyncEventArgs connectArgs = new SocketAsyncEventArgs();

            connectArgs.UserToken = this;
            connectArgs.RemoteEndPoint = _hostEndPoint;
            connectArgs.Completed += OnConnected;

            _socket.ConnectAsync(connectArgs);
        }

        void OnConnected(object sender, SocketAsyncEventArgs e)
        {
            e.Completed -= OnConnected;

            SocketError errorCode = e.SocketError;

            if (errorCode != SocketError.Success)
                throw new SocketException((Int32)errorCode);

            _isConnected = true;
            _connectedOn = DateTime.Now;

            var handler = Connected;

            if (handler != null)
                handler(this, EventArgs.Empty);

            BeginReceive();
        }

        public virtual void Send(Packet p)
        {
            if (_socket == null)
            {
                p.OnSend();
                return;
            }

            int length;
            byte[] buffer = p.Compile(_isCompressionEnabled, out length);

            if (buffer != null)
            {
                if (buffer.Length <= 0 || length <= 0)
                {
                    p.OnSend();
                    return;
                }

                try
                {
                    SocketAsyncEventArgs args = new SocketAsyncEventArgs();

                    args.SetBuffer(buffer, 0, length);
                    args.RemoteEndPoint = _hostEndPoint;
                    args.UserToken = this;

                    _socket.SendAsync(args);
                }
                catch (CapacityExceededException)
                {
                    Tracer.Error("Too much data pending, disconnecting...");
                    Dispose();
                }
                catch (Exception ex)
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

            SocketError errorCode = e.SocketError;

            if (errorCode != SocketError.Success)
                throw new SocketException((Int32)errorCode);

            try
            {
                int byteCount = e.BytesTransferred;

                if (byteCount > 0)
                {
                    byte[] buffer = e.Buffer;

                    try
                    {
                        BeginReceive();
                    }
                    catch (Exception ex)
                    {
                        Tracer.Error(ex);
                        Dispose();
                    }

                    if (_isCompressionEnabled)
                    {
                        byte[] unpackBuffer = _recvBuffer.AcquireBuffer();

                        int unpackSize;
                        int sourceIndex = 0;
                        int sourceSize = byteCount;

                        while (HuffmanCompression.Decompress(buffer, ref sourceIndex, sourceSize, unpackBuffer, out unpackSize))
                        {                            
                            lock (_buffer)
                                _buffer.Enqueue(unpackBuffer, 0, unpackSize);

                            _messagePump.OnReceive(this);
                        }
                        
                        _recvBuffer.ReleaseBuffer(buffer);
                        _recvBuffer.ReleaseBuffer(unpackBuffer);
                    }
                    else
                    {
                        lock (_buffer)
                            _buffer.Enqueue(buffer, 0, byteCount);
                        
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
            if (_socket == null || _isDisposing)
                return;

            _isDisposing = true;

            try
            {
                _socket.Shutdown(SocketShutdown.Both);
            }
            catch (SocketException ex)
            {
                Tracer.Error(ex);
            }

            try
            {
                _socket.Close();
            }
            catch (SocketException ex)
            {
                Tracer.Error(ex);
            }

            _socket = null;
            _buffer = null;
        }

        internal PacketHandler GetHandler(int packetID)
        {
            return PacketHandlers.GetHandler(packetID);
        }
    }
}
