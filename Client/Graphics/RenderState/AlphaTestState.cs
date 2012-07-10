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
    public struct AlphaTestState
    {
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal ushort _mode;

#if DEBUG
        static AlphaTestState()
        {
            BitWiseTypeValidator.Validate<AlphaTestState>();
        }
#endif
        public bool Enabled
        {
            get { return (_mode & 1) == 1; }
            set { _mode = (ushort)((_mode & ~1) | (value ? 1 : 0)); }
        }

        public Compare AlphaTestFunction
        {
            get
            {
                return (Compare)((~((_mode >> 1)) & 7) + 1);
            }
            set
            {
                _mode = (ushort)(((int)_mode & ~(7 << 1)) | (7 & (~((int)value - 1))) << 1);
            }
        }

        public byte ReferenceAlpha
        {
            get
            {
                return (byte)(((_mode & (255 << 8)) >> 8));
            }
            set
            {
                _mode = (ushort)((_mode & ~(255 << 8)) | (((int)value) << 8));
            }
        }


        internal void ResetState(ref AlphaTestState current, DeviceContext device)
        {
            device.SetRenderState(RenderState.AlphaTestEnable, Enabled);
            device.SetRenderState(RenderState.AlphaFunc, AlphaTestFunction);
            device.SetRenderState(RenderState.AlphaRef, ReferenceAlpha);

            current._mode = _mode;
        }

        internal bool ApplyState(ref AlphaTestState current, DeviceContext device)
        {
            bool changed = false;
            if (Enabled)
            {

#if DEBUG
                changed = _mode != current._mode;
#endif

                if (!current.Enabled)
                    device.SetRenderState(RenderState.AlphaTestEnable, true);

                if (AlphaTestFunction != current.AlphaTestFunction)
                    device.SetRenderState(RenderState.AlphaFunc, AlphaTestFunction);

                if (ReferenceAlpha != current.ReferenceAlpha)
                    device.SetRenderState(RenderState.AlphaRef, ReferenceAlpha);

                current._mode = _mode;
            }
            else
            {
                if (current.Enabled)
                {
#if DEBUG
                    changed = true;
#endif
                    device.SetRenderState(RenderState.AlphaTestEnable, false);
                    current.Enabled = false;
                }
            }
            return changed;
        }

        public static explicit operator AlphaTestState(ushort state)
        {
            AlphaTestState value;
            value._mode = state;
            return value;
        }

        public static implicit operator ushort(AlphaTestState state)
        {
            return state._mode;
        }

        public static bool operator ==(AlphaTestState a, AlphaTestState b)
        {
            return a._mode == b._mode;
        }

        public static bool operator !=(AlphaTestState a, AlphaTestState b)
        {
            return a._mode != b._mode;
        }

        public override bool Equals(object obj)
        {
            if (obj is AlphaTestState)
                return ((AlphaTestState)obj)._mode == _mode;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _mode;
        }
    }
}
