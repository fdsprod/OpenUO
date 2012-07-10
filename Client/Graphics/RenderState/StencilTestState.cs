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
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Size = 8)]
    public struct StencilTestState
    {
        [System.Runtime.InteropServices.FieldOffset(0)]
        private long _long;
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal int _op;
        [System.Runtime.InteropServices.FieldOffset(4)]
        internal int _mode;

#if DEBUG
        static StencilTestState()
        {
            BitWiseTypeValidator.Validate<StencilTestState>();
        }
#endif
        public bool Enabled
        {
            get { return (_mode & 1) == 1; }
            set { _mode = ((_mode & ~1) | (value ? 1 : 0)); }
        }

        public bool TwoSidedStencilModeEnabled
        {
            get { return (_mode & 2) == 2; }
            set { _mode = ((_mode & ~2) | (value ? 2 : 0)); }
        }

        public StencilOperation StencilPassZFailOperation
        {
            get { const int offset = 0; return (StencilOperation)(((_op >> offset) & 7) + 1); }
            set { const int offset = 0; _op = (((int)_op & ~(7 << offset)) | (7 & ((int)value - 1)) << offset); }
        }

        public StencilOperation StencilPassOperation
        {
            get { const int offset = 4; return (StencilOperation)(((_op >> offset) & 7) + 1); }
            set { const int offset = 4; _op = (((int)_op & ~(7 << offset)) | (7 & ((int)value - 1)) << offset); }
        }

        public StencilOperation StencilFailOperation
        {
            get { const int offset = 8; return (StencilOperation)(((_op >> offset) & 7) + 1); }
            set { const int offset = 8; _op = (((int)_op & ~(7 << offset)) | (7 & ((int)value - 1)) << offset); }
        }

        public StencilOperation BackfaceStencilPassZFailOperation
        {
            get { const int offset = 12; return (StencilOperation)(((_op >> offset) & 7) + 1); }
            set { const int offset = 12; _op = (((int)_op & ~(7 << offset)) | (7 & ((int)value - 1)) << offset); }
        }

        public StencilOperation BackfaceStencilPassOperation
        {
            get { const int offset = 16; return (StencilOperation)(((_op >> offset) & 7) + 1); }
            set { const int offset = 16; _op = (((int)_op & ~(7 << offset)) | (7 & ((int)value - 1)) << offset); }
        }

        public StencilOperation BackfaceStencilFailOperation
        {
            get { const int offset = 20; return (StencilOperation)(((_op >> offset) & 7) + 1); }
            set { const int offset = 20; _op = (((int)_op & ~(7 << offset)) | (7 & ((int)value - 1)) << offset); }
        }

        public Compare StencilFunction
        {
            get { const int offset = 24; return (Compare)(((~(_op >> offset)) & 7) + 1); }
            set { const int offset = 24; _op = (((int)_op & ~(7 << offset)) | (7 & (~((int)value - 1))) << offset); }
        }

        public Compare BackfaceStencilFunction
        {
            get { const int offset = 28; return (Compare)(((~(_op >> offset)) & 7) + 1); }
            set { const int offset = 28; _op = (((int)_op & ~(7 << offset)) | (7 & (~((int)value - 1))) << offset); }
        }

        public byte ReferenceValue
        {
            get { const int offset = 8; return (byte)(~((_mode >> offset) & 255)); }
            set { const int offset = 8; _mode = (((int)_mode & ~(255 << offset)) | (255 & (~value)) << offset); }
        }

        public byte StencilReadMask
        {
            get { const int offset = 16; return (byte)(~((_mode >> offset) & 255)); }
            set { const int offset = 16; _mode = (((int)_mode & ~(255 << offset)) | (255 & (~value)) << offset); }
        }

        public byte StencilWriteMask
        {
            get { const int offset = 24; return (byte)(~((_mode >> offset) & 255)); }
            set { const int offset = 24; _mode = (((int)_mode & ~(255 << offset)) | (255 & (~value)) << offset); }
        }

        internal void ResetState(ref StencilTestState current, DeviceContext device)
        {
            device.SetRenderState(RenderState.StencilZFail, StencilPassZFailOperation);
            device.SetRenderState(RenderState.StencilPass, StencilPassOperation);
            device.SetRenderState(RenderState.StencilFail, StencilFailOperation);
            device.SetRenderState(RenderState.CcwStencilZFail, BackfaceStencilPassZFailOperation);
            device.SetRenderState(RenderState.CcwStencilPass, BackfaceStencilPassOperation);
            device.SetRenderState(RenderState.CcwStencilFail, BackfaceStencilFailOperation);
            device.SetRenderState(RenderState.StencilFunc, StencilFunction);
            device.SetRenderState(RenderState.CcwStencilFunc, BackfaceStencilFunction);

            device.SetRenderState(RenderState.StencilEnable, Enabled);
            device.SetRenderState(RenderState.TwoSidedStencilMode, TwoSidedStencilModeEnabled);

            device.SetRenderState(RenderState.StencilRef, ReferenceValue);
            device.SetRenderState(RenderState.StencilMask, StencilReadMask);
            device.SetRenderState(RenderState.StencilWriteMask, StencilWriteMask);

            current._op = _op;
            current._mode = _mode;
        }

        internal bool ApplyState(ref StencilTestState current, DeviceContext device)
        {
            bool changed = false;
            if (Enabled)
            {
#if DEBUG
                changed = _mode != current._mode ||
                    _op != current._op;
#endif
                if (!current.Enabled)
                    device.SetRenderState(RenderState.StencilEnable, true);

                if (_op != current._op)
                {
                    if (current.StencilPassZFailOperation != StencilPassZFailOperation)
                        device.SetRenderState(RenderState.StencilZFail, StencilPassZFailOperation);

                    if (current.StencilPassOperation != StencilPassOperation)
                        device.SetRenderState(RenderState.StencilPass, StencilPassOperation);

                    if (current.StencilFailOperation != StencilFailOperation)
                        device.SetRenderState(RenderState.StencilFail, StencilFailOperation);
                    
                    if (current.BackfaceStencilPassZFailOperation != BackfaceStencilPassZFailOperation)
                        device.SetRenderState(RenderState.CcwStencilZFail, BackfaceStencilPassZFailOperation);

                    if (current.BackfaceStencilPassOperation != BackfaceStencilPassOperation)
                        device.SetRenderState(RenderState.CcwStencilPass, BackfaceStencilPassOperation);

                    if (current.BackfaceStencilFailOperation != BackfaceStencilFailOperation)
                        device.SetRenderState(RenderState.CcwStencilFail, BackfaceStencilFailOperation);
                    
                    if (current.StencilFunction != StencilFunction)
                        device.SetRenderState(RenderState.StencilFunc, StencilFunction);

                    if (current.BackfaceStencilFunction != BackfaceStencilFunction)
                        device.SetRenderState(RenderState.CcwStencilFunc, BackfaceStencilFunction);

                    current._op = _op;
                }

                if (current._mode != _mode)
                {
                    if (current.TwoSidedStencilModeEnabled != TwoSidedStencilModeEnabled)
                        device.SetRenderState(RenderState.TwoSidedStencilMode, TwoSidedStencilModeEnabled);

                    if (current.ReferenceValue != ReferenceValue)
                        device.SetRenderState(RenderState.StencilRef, ReferenceValue);

                    if (current.StencilReadMask != StencilReadMask)
                        device.SetRenderState(RenderState.StencilMask, StencilReadMask);

                    if (current.StencilWriteMask != StencilWriteMask)
                        device.SetRenderState(RenderState.StencilWriteMask, StencilWriteMask);
                }

                current._mode = _mode;
            }
            else
            {
                if (current.Enabled)
                {
#if DEBUG
                    changed = true;
#endif
                    device.SetRenderState(RenderState.StencilEnable, false);
                    current.Enabled = false;
                }
            }
            return changed;
        }

        public static explicit operator StencilTestState(long state)
        {
            StencilTestState value = new StencilTestState();
            value._long = state;
            return value;
        }

        public static implicit operator long(StencilTestState state)
        {
            return state._long;
        }

        public static bool operator ==(StencilTestState a, StencilTestState b)
        {
            return a._op == b._op && a._mode == b._mode;
        }

        public static bool operator !=(StencilTestState a, StencilTestState b)
        {
            return a._op != b._op || a._mode != b._mode;
        }

        public override bool Equals(object obj)
        {
            if (obj is StencilTestState)
                return ((StencilTestState)obj) == this;

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _op ^ _mode;
        }
    }
}
