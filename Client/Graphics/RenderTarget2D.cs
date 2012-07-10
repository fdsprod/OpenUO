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

using SharpDX.Direct3D9;

namespace Client.Graphics
{
    public class RenderTarget2D : Texture2D
    {
        private Surface _surface;

        public Surface Surface
        {
            get { return _surface; }
        }

        public RenderTarget2D(DeviceContext context, int width, int height, Format format)
            : base(context, width, height, 0, SharpDX.Direct3D9.Usage.RenderTarget, format, Pool.Default)
        {
            _surface = GetSurfaceLevel(0);
            RecreateOnReset = false;
        }

        protected override void OnContextLost(DeviceContext context)
        {
            base.OnContextLost(context);

            if (_surface != null)
            {
                _surface.Dispose();
                _surface = null;
            }
        }

        protected override void OnContextReset(DeviceContext context)
        {
            base.OnContextReset(context);

            if(RecreateOnReset)
                _surface = GetSurfaceLevel(0);
        }
    }
}
