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

using System.Collections.Generic;

namespace OpenUO.Ultima.Network
{
    public static class PacketHandlers
    {
        private static readonly PacketHandler[] _extendedHandlersLow;
        private static readonly Dictionary<int, PacketHandler> _extendedHandlersHigh;

        static PacketHandlers()
        {
            Handlers = new PacketHandler[0x100];
            _extendedHandlersLow = new PacketHandler[0x100];
            _extendedHandlersHigh = new Dictionary<int, PacketHandler>();
        }

        public static PacketHandler[] Handlers
        {
            get;
            private set;
        }

        public static void Register(int packetID, int length, OnPacketReceive onReceive)
        {
            Handlers[packetID] = new PacketHandler(packetID, length, onReceive);
        }

        public static PacketHandler GetHandler(int packetID)
        {
            return Handlers[packetID];
        }

        public static void RegisterExtended(int packetID, OnPacketReceive onReceive)
        {
            if(packetID >= 0 && packetID < 0x100)
            {
                _extendedHandlersLow[packetID] = new PacketHandler(packetID, 0, onReceive);
            }
            else
            {
                _extendedHandlersHigh[packetID] = new PacketHandler(packetID, 0, onReceive);
            }
        }

        public static PacketHandler GetExtendedHandler(int packetID)
        {
            if(packetID >= 0 && packetID < 0x100)
            {
                return _extendedHandlersLow[packetID];
            }

            PacketHandler handler;
            _extendedHandlersHigh.TryGetValue(packetID, out handler);
            return handler;
        }

        public static void RemoveExtendedHandler(int packetID)
        {
            if(packetID >= 0 && packetID < 0x100)
            {
                _extendedHandlersLow[packetID] = null;
            }
            else
            {
                _extendedHandlersHigh.Remove(packetID);
            }
        }
    }
}