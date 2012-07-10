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
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Size = 2)]
    public struct DepthColourCullState
    {
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal ushort _mode;

#if DEBUG
        static DepthColourCullState()
        {
            BitWiseTypeValidator.Validate<DepthColourCullState>();
        }
#endif
        public bool DepthTestEnabled
        {
            get { return (_mode & 1) != 1; }
            set { _mode = (ushort)((_mode & ~1) | (value ? 0 : 1)); }
        }

        public bool DepthWriteEnabled
        {
            get { return (_mode & 2) != 2; }
            set { _mode = (ushort)((_mode & ~2) | (value ? 0 : 2)); }
        }

        public Compare DepthTestFunction
        {
            get
            {
                return (Compare)((((_mode >> 2) & 15) ^ 4));
            }
            set
            {
                _mode = (ushort)((_mode & ~(15 << 2)) | (15 & (((int)value)) ^ 4) << 2);
            }
        }

        public Cull CullMode
        {
            get
            {
                return (Cull)(((((_mode >> 6) & 3) ^ 2)) + 1);
            }
            set
            {
                _mode = (ushort)((_mode & ~(3 << 6)) | (3 & ((((int)value)) - 1) ^ 2) << 6);
            }
        }

        internal Cull GetCullMode(bool reverseCull)
        {
            Cull mode = (Cull)(((((_mode >> 6) & 3) ^ 2)) + 1);

            if (reverseCull)
            {
                switch (mode)
                {
                    case Cull.Clockwise:
                        return Cull.Counterclockwise;
                    case Cull.Counterclockwise:
                        return Cull.Clockwise;
                }
            }

            return mode;
        }

        public FillMode FillMode
        {
            get
            {
                return (FillMode)(((((_mode >> 12) & 3) ^ 2)) + 1);
            }
            set
            {
                _mode = (ushort)((_mode & ~(3 << 12)) | (3 & ((((int)value)) - 1) ^ 2) << 12);
            }
        }

        internal void ResetState(ref DepthColourCullState current, DeviceContext device, bool reverseCull)
        {
            device.SetRenderState(RenderState.ZEnable, DepthTestEnabled);
            device.SetRenderState(RenderState.ZWriteEnable, DepthWriteEnabled);
            device.SetRenderState(RenderState.ZFunc, DepthTestFunction);
            device.SetRenderState(RenderState.CullMode, GetCullMode(reverseCull));
            device.SetRenderState(RenderState.FillMode, FillMode);

            current._mode = _mode;
        }

        internal bool ApplyState(ref DepthColourCullState current, DeviceContext device, bool reverseCull)
        {
            bool changed = false;

            // bits 6 to 14 are used.. so ignore bits 1-5
            if ((current._mode & (~63)) != (_mode & (~63)))
            {
#if DEBUG
                changed = true;
#endif
                Cull cull = CullMode;
                if (cull != current.CullMode)
                {
                    device.SetRenderState(RenderState.CullMode, GetCullMode(reverseCull));
                    current.CullMode = cull;
                }

                FillMode fill = FillMode;
                if (fill != current.FillMode)
                {
                    device.SetRenderState(RenderState.FillMode, FillMode);
                    current.FillMode = fill;
                }
            }

            if (DepthTestEnabled)
            {
                if (!current.DepthTestEnabled)
                {
#if DEBUG
                    changed = true;
#endif
                    device.SetRenderState(RenderState.ZEnable, true);
                }

                if (DepthWriteEnabled != current.DepthWriteEnabled)
                {
#if DEBUG
                    changed = true;
#endif
                    device.SetRenderState(RenderState.ZWriteEnable, DepthWriteEnabled);
                }

                if (DepthTestFunction != current.DepthTestFunction)
                {
#if DEBUG
                    changed = true;
#endif
                    device.SetRenderState(RenderState.ZFunc, DepthTestFunction);
                }

                current._mode = _mode;
            }
            else
            {
                if (current.DepthTestEnabled)
                {
#if DEBUG
                    changed = true;
#endif
                    device.SetRenderState(RenderState.ZEnable, false);
                    current.DepthTestEnabled = false;
                }
            }
            return changed;
        }

        public static explicit operator DepthColourCullState(ushort state)
        {
            DepthColourCullState value;
            value._mode = state;
            return value;
        }

        public static implicit operator ushort(DepthColourCullState state)
        {
            return state._mode;
        }

        public static bool operator ==(DepthColourCullState a, DepthColourCullState b)
        {
            return a._mode == b._mode;
        }

        public static bool operator !=(DepthColourCullState a, DepthColourCullState b)
        {
            return a._mode != b._mode;
        }

        public override bool Equals(object obj)
        {
            if (obj is DepthColourCullState)
                return ((DepthColourCullState)obj)._mode == _mode;

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _mode;
        }

    }
}
