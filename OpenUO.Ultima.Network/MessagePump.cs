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

namespace OpenUO.Ultima.Network
{
    public class MessagePump
    {
        private const int BufferSize = 4096;
        private readonly BufferPool _buffers = new BufferPool("Processor", 4, BufferSize);

        public bool OnReceive(NetState ns)
        {
            ByteQueue buffer = ns.Buffer;

            if (buffer == null || buffer.Length <= 0)
                return true;

            lock (buffer)
            {
                int length = buffer.Length;

                while (length > 0)
                {
                    int packetID = buffer.GetPacketID();

                    PacketHandler handler = ns.GetHandler(packetID);

                    if (handler == null)
                    {
                        byte[] data = new byte[length];
                        length = buffer.Dequeue(data, 0, length);

                        new PacketReader(data, length, false).Trace(ns);

                        break;
                    }

                    int packetLength = handler.Length;

                    if (packetLength <= 0)
                    {
                        if (length >= 3)
                        {
                            packetLength = buffer.GetPacketLength();

                            if (packetLength < 3)
                            {
                                ns.Dispose();
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (length < packetLength)
                        break;

                    byte[] packetBuffer = BufferSize >= packetLength ? _buffers.AcquireBuffer() : new byte[packetLength];
                    packetLength = buffer.Dequeue(packetBuffer, 0, packetLength);

                    PacketReader r = new PacketReader(packetBuffer, packetLength, handler.Length != 0);

                    handler.OnReceive(ns, r);

                    length = buffer.Length;

                    if (BufferSize >= packetLength)
                        _buffers.ReleaseBuffer(packetBuffer);
                }
            }

            return true;
        }
    }
}
