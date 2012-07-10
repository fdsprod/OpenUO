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
    public class SeedPacket : Packet
    {
        public SeedPacket(int seed, int major, int minor, int revision, int prototype)
            : base(0xEF, "Seed", 21)
        {
            Stream.Write(seed);
            Stream.Write(major);
            Stream.Write(minor);
            Stream.Write(revision);
            Stream.Write(prototype);
        }
    }

    public class LoginPacket : Packet
    {
        public LoginPacket(string username, string password)
            : base(0x80, "Account Login", 62)
        {
            this.Stream.WriteAsciiFixed(username, 30);
            this.Stream.WriteAsciiFixed(password, 30);
            this.Stream.Write((byte)0x5D);
        }
    }

    public class SelectServerPacket : Packet
    {
        public SelectServerPacket(short id) :
            base(0xA0, "Select Server", 3)
        {
            Stream.Write((short)id);
        }
    }

    public class GameLoginPacket : Packet
    {
        public GameLoginPacket(int authId, string username, string password)
            : base(0x91, "Game Server Login", 0x41)
        {
            Stream.Write(authId);
            Stream.WriteAsciiFixed(username, 30);
            Stream.WriteAsciiFixed(password, 30);
        }
    }

    public class LoginCharacterPacket : Packet
    {
        public LoginCharacterPacket(string characterName, int slotIndex, int ipAddress)
            : base(0x5D, "Character Select", 0x49)
        {
            Stream.Write((uint)0xEDEDEDED); 
            Stream.WriteAsciiFixed(characterName, 30);
            Stream.Write((short)0); // unknown
            Stream.Write((int)0x00); //TODO: this is client flags, need to define and use.
            Stream.Write((int)1); // unknown
            Stream.Write((int)0); //  logincount
            Stream.WriteAsciiFixed("", 16);
            Stream.Write(slotIndex); 
            Stream.Write(ipAddress);
        }
    }

    public class ClientVersionPacket : Packet
    {
        public ClientVersionPacket(string version)
            : base(0xBD, "Client Version")
        {
            Stream.WriteAsciiNull(version);
        }
    }
}
