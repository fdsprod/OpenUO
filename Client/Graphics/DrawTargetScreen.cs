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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D9;

namespace Client.Graphics
{
    public sealed class DrawTargetScreen : DrawTarget
    {
        private Vector2 windowSize;
        private int windowSizeChangeIndex = 1;
        private 

        /// <summary>
        /// Construct the draw target
        /// </summary>
        /// <param name="application"></param>
        /// <param name="camera"></param>
        public DrawTargetScreen(int width, int height)
        {
            Vector2 ws = new Vector2((float)width, (float)height);

            if (ws.X != windowSize.X || ws.Y != windowSize.Y)
            {
                windowSize = ws;
                windowSizeChangeIndex = System.Threading.Interlocked.Increment(ref DrawTarget.baseSizeIndex);
            }
        }

        protected internal override void Begin(DrawState state)
        {
            DeviceContext device = state.Context;

            state.ResetTextures();

            device.SetRenderTarget(0, null);

            Vector2 ws = new Vector2((float)Width, (float)Height);
            if (ws.X != windowSize.X || ws.Y != windowSize.Y)
            {
                windowSize = ws;
                windowSizeChangeIndex = System.Threading.Interlocked.Increment(ref DrawTarget.baseSizeIndex);
            }
        }

        protected internal override void End(DrawState state)
        {
        }

        public override Format SurfaceFormat
        {
            get { return application.GetScreenFormat(); }
        }

        public override Format? SurfaceDepthFormat
        {
            get { return application.GetScreenDepthFormat(); }
        }

        public override MultisampleType MultiSampleType
        {
            get { return application.GetScreenMultisample(); }
        }

        public override int Width
        {
            get
            {
                return application.WindowWidth;
            }
        }

        public override int Height
        {
            get
            {
                return application.WindowHeight;
            }
        }

        internal protected override bool GetWidthHeightAsVector(out Vector2 size, ref int ci)
        {
            size = windowSize;
            if (windowSizeChangeIndex != ci)
            {
                ci = windowSizeChangeIndex;
                return true;
            }
            return false;
        }

        protected internal override bool HasDepthBuffer
        {
            get { return hasDepth; }
        }

        protected internal override bool HasStencilBuffer
        {
            get { return hasStencil; }
        }

        internal override void Warm(DeviceContext context)
        {
        }

        internal override int GetAllocatedDeviceBytes()
        {
            //approximate
            return 0;// 2 * (4 + hasDepth ? 2 : 0) * Width * Height;
        }
        internal override int GetAllocatedManagedBytes()
        {
            return 0;
        }
        internal override bool InUse
        {
            get { return true; }
        }
        internal override bool IsDisposed
        {
            get { return false; }
        }
    }
}
