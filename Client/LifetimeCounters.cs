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


namespace Client
{
    public static class LifetimeCounters
    {
        public const string DeviceResets = "DeviceResets";
        public const string TextureCount = "TextureCount";
        public const string RenderStateStencilTestChanged = "RenderStateStencilTestChanged";
        public const string RenderStateAlphaBlendChanged = "RenderStateAlphaBlendChanged";
        public const string RenderStateAlphaTestChanged = "RenderStateAlphaTestChanged";
        public const string RenderStateDepthColourCullChanged = "RenderStateDepthColourCullChanged";
    }
}
