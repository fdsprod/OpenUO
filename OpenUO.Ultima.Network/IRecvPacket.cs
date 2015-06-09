#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// IRecvPacket.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region usings

#endregion

namespace OpenUO.Core.Net
{
    public interface IRecvPacket
    {
        int Id
        {
            get;
        }

        string Name
        {
            get;
        }
    }
}