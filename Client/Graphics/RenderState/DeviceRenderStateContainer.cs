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


namespace Client.Graphics
{
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
    public sealed class DeviceRenderStateContainer
    {
        internal DeviceRenderStateContainer()
        {
        }

        [System.Runtime.InteropServices.FieldOffset(0)]
        internal DeviceRenderState State;
        /// <summary>
        /// Get/Set/Modify the alpha stencil test render states to be used during rendering
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        public StencilTestState StencilTest;
        /// <summary>
        /// Get/Set/Modify the alpha blending render states to be used during rendering
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(8)]
        public AlphaBlendState AlphaBlend;
        /// <summary>
        /// Get/Set/Modify the alpha test render states to be used during rendering
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(12)]
        public AlphaTestState AlphaTest;
        /// <summary>
        /// Get/Set/Modify the depth test, colour write and face culling render states to be used during rendering
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(14)]
        public DepthColourCullState DepthColourCull;

        /// <summary>
        /// Cast to a <see cref="DeviceRenderState"/> implicitly
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static implicit operator DeviceRenderState(DeviceRenderStateContainer source)
        {
            return source.State;
        }
    }
}
