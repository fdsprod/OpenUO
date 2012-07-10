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

namespace Client.Graphics
{
    [Flags]
    public enum StateFlag
    {
        None = 0,
        AlphaBlend = 1,
        StencilTest = 2,
        AlphaTest = 4,
        DepthColourCull = 8,
        All = AlphaBlend | StencilTest | AlphaTest | DepthColourCull
    }
}
