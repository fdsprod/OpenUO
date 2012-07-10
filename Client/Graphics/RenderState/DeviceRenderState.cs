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
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Size = 16)]
    public struct DeviceRenderState : IComparable<DeviceRenderState>
    {
        public DeviceRenderState(AlphaBlendState alphaBlendState, AlphaTestState alphaTestState, DepthColourCullState depthState, StencilTestState stencilState)
        {
            StencilTest = stencilState;
            AlphaBlend = alphaBlendState;
            AlphaTest = alphaTestState;
            DepthColourCull = depthState;
        }

        [System.Runtime.InteropServices.FieldOffset(0)]
        public StencilTestState StencilTest;

        [System.Runtime.InteropServices.FieldOffset(8)]
        public AlphaBlendState AlphaBlend;

        [System.Runtime.InteropServices.FieldOffset(12)]
        public AlphaTestState AlphaTest;

        [System.Runtime.InteropServices.FieldOffset(14)]
        public DepthColourCullState DepthColourCull;

        internal void ApplyState(ref DeviceRenderState current, DeviceContext device, DrawState state)
        {
            if (current.StencilTest._mode != StencilTest._mode ||
                current.StencilTest._op != StencilTest._op)
            {
#if DEBUG
                if (StencilTest.ApplyState(ref current.StencilTest, device))
                    device.PerformanceMonitor.IncreaseLifetimeCounter(LifetimeCounters.RenderStateStencilTestChanged);
#else
				StencilTest.ApplyState(ref current.StencilTest, device);
#endif
            }

            if (current.AlphaBlend._mode != AlphaBlend._mode)
            {
#if DEBUG
                if (AlphaBlend.ApplyState(ref current.AlphaBlend, device))
                    device.PerformanceMonitor.IncreaseLifetimeCounter(LifetimeCounters.RenderStateAlphaBlendChanged);
#else
				AlphaBlend.ApplyState(ref current.AlphaBlend, device);
#endif
            }

            if (current.AlphaTest._mode != AlphaTest._mode)
            {
#if DEBUG
                if (AlphaTest.ApplyState(ref current.AlphaTest, device))
                    device.PerformanceMonitor.IncreaseLifetimeCounter(LifetimeCounters.RenderStateAlphaTestChanged);
#else
				AlphaTest.ApplyState(ref current.AlphaTest, device);
#endif
            }

            if (current.DepthColourCull._mode != DepthColourCull._mode)
            {
#if DEBUG
                if (DepthColourCull.ApplyState(ref current.DepthColourCull, device, false))
                    device.PerformanceMonitor.IncreaseLifetimeCounter(LifetimeCounters.RenderStateDepthColourCullChanged);
#else
				DepthColourCull.ApplyState(ref current.DepthColourCull, device, false);
#endif
            }
        }

        internal void ResetState(StateFlag state, ref DeviceRenderState current, DeviceContext device, bool reverseCull)
        {
            if ((state & StateFlag.StencilTest) != 0)
                StencilTest.ResetState(ref current.StencilTest, device);
            if ((state & StateFlag.AlphaBlend) != 0)
                AlphaBlend.ResetState(ref current.AlphaBlend, device);
            if ((state & StateFlag.AlphaTest) != 0)
                AlphaTest.ResetState(ref current.AlphaTest, device);
            if ((state & StateFlag.DepthColourCull) != 0)
                DepthColourCull.ResetState(ref current.DepthColourCull, device, reverseCull);
        }

        /// <summary>
        /// Fast has code of all the render states
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return StencilTest._op ^ StencilTest._mode ^ AlphaBlend._mode ^ (AlphaTest._mode | (DepthColourCull._mode << 16));
        }

        /// <summary></summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is DeviceRenderState)
                return ((IComparable<DeviceRenderState>)this).CompareTo((DeviceRenderState)obj) == 0;

            return false;
        }

        int IComparable<DeviceRenderState>.CompareTo(DeviceRenderState other)
        {
            if (AlphaBlend._mode > other.AlphaBlend._mode)
                return 1;
            if (AlphaBlend._mode < other.AlphaBlend._mode)
                return -1;
            if (DepthColourCull._mode > other.DepthColourCull._mode)
                return 1;
            if (DepthColourCull._mode < other.DepthColourCull._mode)
                return -1;
            if (AlphaTest._mode > other.AlphaTest._mode)
                return 1;
            if (AlphaTest._mode < other.AlphaTest._mode)
                return -1;
            if (StencilTest._mode > other.StencilTest._mode)
                return 1;
            if (StencilTest._mode < other.StencilTest._mode)
                return -1;
            if (StencilTest._op > other.StencilTest._op)
                return 1;
            if (StencilTest._op < other.StencilTest._op)
                return -1;
            return 0;
        }
    }
}
