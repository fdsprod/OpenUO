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
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Size = 4)]
    public struct AlphaBlendState
    {
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal int _mode;

        public AlphaBlendState(Blend sourceBlend, Blend destinationBlend)
        {
            _mode = 1;
            SourceBlend = sourceBlend;
            DestinationBlend = destinationBlend;
        }

#if DEBUG
        static AlphaBlendState()
        {
            BitWiseTypeValidator.Validate<AlphaBlendState>();
        }
#endif
        public bool Enabled
        {
            get { return (_mode & 1) == 1; }
            set { _mode = (_mode & ~1) | (value ? 1 : 0); }
        }

        public bool SeparateAlphaBlendEnabled
        {
            get { return (_mode & 2) == 2; }
            set { _mode = (_mode & ~2) | (value ? 2 : 0); }
        }

        public BlendOperation BlendOperation
        {
            get
            {
                //1-5
                return (BlendOperation)(((_mode >> 2) & 7) + 1);
            }
            set
            {
                _mode = (_mode & ~(7 << 2)) | (7 & ((int)value - 1)) << 2;
            }
        }

        public BlendOperation BlendOperationAlpha
        {
            get
            {
                return (BlendOperation)(((_mode >> 5) & 7) + 1);
            }
            set
            {
                _mode = (_mode & ~(7 << 5)) | (7 & ((int)value - 1)) << 5;
            }
        }

        public Blend SourceBlend
        {
            get
            {
                return (Blend)((((_mode >> 8) & 15) ^ 1) + 1);
            }
            set
            {
                _mode = (_mode & ~(15 << 8)) | (15 & ((int)value - 1) ^ 1) << 8;
            }
        }

        public Blend DestinationBlend
        {
            get
            {
                return (Blend)(((_mode >> 12) & 15) + 1);
            }
            set
            {
                _mode = (_mode & ~(15 << 12)) | (15 & ((int)value - 1)) << 12;
            }
        }

        public Blend SourceBlendAlpha
        {
            get
            {
                return (Blend)((((_mode >> 16) & 15) ^ 1) + 1);
            }
            set
            {
                _mode = (_mode & ~(15 << 16)) | (15 & ((int)value - 1) ^ 1) << 16;
            }
        }

        public Blend DestinationBlendAlpha
        {
            get
            {
                return (Blend)(((_mode >> 20) & 15) + 1);
            }
            set
            {
                _mode = (_mode & ~(15 << 20)) | (15 & ((int)value - 1)) << 20;
            }
        }

        internal void ResetState(ref AlphaBlendState current, DeviceContext device)
        {
            device.SetRenderState(RenderState.AlphaBlendEnable, Enabled);
            device.SetRenderState(RenderState.SeparateAlphaBlendEnable, SeparateAlphaBlendEnabled);
            device.SetRenderState(RenderState.BlendOperation, BlendOperation);
            device.SetRenderState(RenderState.BlendOperationAlpha, BlendOperationAlpha);
            device.SetRenderState(RenderState.SourceBlend, SourceBlend);
            device.SetRenderState(RenderState.DestinationBlend, DestinationBlend);
            device.SetRenderState(RenderState.SourceBlendAlpha, SourceBlendAlpha);
            device.SetRenderState(RenderState.DestinationBlendAlpha, DestinationBlendAlpha);

            current._mode = _mode;
        }

        internal bool ApplyState(ref AlphaBlendState current, DeviceContext device)
        {
            bool changed = false;

            if (Enabled)
            {
#if DEBUG
                if (_mode != current._mode)
                    changed = true;
#endif
                if (!current.Enabled)
                    device.SetRenderState(RenderState.AlphaBlendEnable, true);

                if (SeparateAlphaBlendEnabled != current.SeparateAlphaBlendEnabled)
                    device.SetRenderState(RenderState.SeparateAlphaBlendEnable, SeparateAlphaBlendEnabled);

                if (BlendOperation != current.BlendOperation)
                    device.SetRenderState(RenderState.BlendOperation, BlendOperation);

                if (BlendOperationAlpha != current.BlendOperationAlpha)
                    device.SetRenderState(RenderState.BlendOperationAlpha, BlendOperationAlpha);

                if (SourceBlend != current.SourceBlend)
                    device.SetRenderState(RenderState.SourceBlend, SourceBlend);

                if (DestinationBlend != current.DestinationBlend)
                    device.SetRenderState(RenderState.DestinationBlend, DestinationBlend);

                if (SourceBlendAlpha != current.SourceBlendAlpha)
                    device.SetRenderState(RenderState.SourceBlendAlpha, SourceBlendAlpha);

                if (DestinationBlendAlpha != current.DestinationBlendAlpha)
                    device.SetRenderState(RenderState.DestinationBlendAlpha, DestinationBlendAlpha);

                current._mode = _mode;
            }
            else
            {
                if (current.Enabled)
                {
                    device.SetRenderState(RenderState.AlphaBlendEnable, false);
                    current.Enabled = false;
#if DEBUG
                    changed = true;
#endif
                }
            }
            return changed;
        }

        private static readonly AlphaBlendState _none = new AlphaBlendState();
        private static readonly AlphaBlendState _alpha = new AlphaBlendState(Blend.SourceAlpha, Blend.InverseSourceAlpha);
        private static readonly AlphaBlendState _premodulatedAlpha = new AlphaBlendState(Blend.One, Blend.InverseSourceAlpha);
        private static readonly AlphaBlendState _alphaAdditive = new AlphaBlendState(Blend.SourceAlpha, Blend.One);
        private static readonly AlphaBlendState _additive = new AlphaBlendState(Blend.One, Blend.One);
        private static readonly AlphaBlendState _additiveSaturate = new AlphaBlendState(Blend.InverseDestinationColor, Blend.One);
        private static readonly AlphaBlendState _modulate = new AlphaBlendState(Blend.DestinationColor, Blend.Zero);
        private static readonly AlphaBlendState _modulateAdd = new AlphaBlendState(Blend.DestinationColor, Blend.One);
        private static readonly AlphaBlendState _modulateX2 = new AlphaBlendState(Blend.DestinationColor, Blend.SourceColor);

        /// <summary>
        /// State that disables Alpha Blending
        /// </summary>
        public static AlphaBlendState None { get { return _none; } }

        /// <summary>
        /// State that enables standard Alpha Blending (blending based on the alpha value of the source component,
        /// desitination colour is interpolated to the source colour based on source alpha)
        /// </summary>
        public static AlphaBlendState Alpha { get { return _alpha; } }

        /// <summary>
        /// State that enables Premodulated Alpha Blending (Assumes the source colour data has been premodulated
        /// with the source alpha value, useful for reducing colour bleeding and accuracy problems at alpha edges)
        /// </summary>
        public static AlphaBlendState PremodulatedAlpha { get { return _premodulatedAlpha; } }

        /// <summary>
        /// State that enables Additive Alpha Blending (blending based on the alpha value of the source component, 
        /// the desitination colour is added to the source colour modulated by alpha)
        /// </summary>
        public static AlphaBlendState AlphaAdditive { get { return _alphaAdditive; } }

        /// <summary>
        /// State that enables standard Additive Blending (the alpha value is ignored, the desitination colour is
        /// added to the source colour)
        /// </summary>
        public static AlphaBlendState Additive { get { return _additive; } }

        /// <summary>
        /// State that enables Additive Saturate Blending (the alpha value is ignored, the desitination colour 
        /// is added to the source colour, however the source colour is multipled by the inverse of the destination
        /// colour, preventing the addition from blowing out to pure white (eg, 0.75 + 0.75 * (1-0.75) = 0.9375))
        /// </summary>
        public static AlphaBlendState AdditiveSaturate { get { return _additiveSaturate; } }

        /// <summary>
        /// State that enables Modulate (multiply) Blending (the alpha value is ignored, the desitination colour 
        /// is multipled with the source colour)
        /// </summary>
        public static AlphaBlendState Modulate { get { return _modulate; } }

        /// <summary>
        /// State that enables Modulate Add (multiply+add) Blending (the alpha value is ignored, the desitination 
        /// colour multipled with the source colour is added to the desitnation colour)
        /// </summary>
        public static AlphaBlendState ModulateAdd { get { return _modulateAdd; } }

        /// <summary>
        /// State that enables Modulate (multiply) Blending, scaled by 2 (the alpha value is ignored, the desitination
        /// colour is multipled with the source colour, scaled by two)
        /// </summary>
        public static AlphaBlendState ModulateX2 { get { return _modulateX2; } }

        /// <summary>
        /// Set the render state to no Alpha Blending, resetting all states (This is not equivalent to setting
        /// <see cref="Enabled"/> to false, however it has the same effect)
        /// </summary>
        public void SetToNoBlending() { _mode = 0; }

        /// <summary>
        /// Set the render state to standard Alpha Blending (blending based on the alpha value of the source component,
        /// desitination colour is interpolated to the source colour based on source alpha)
        /// </summary>
        public void SetToAlphaBlending() { _mode = _alpha._mode; }

        /// <summary>
        /// Set the render state to Additive Alpha Blending (blending based on the alpha value of the source component,
        /// the desitination colour is added to the source colour modulated by alpha)
        /// </summary>
        public void SetToAdditiveBlending() { _mode = _additive._mode; }

        /// <summary>
        /// Set the render state to Premodulated Alpha Blending (Assumes the source colour data has been premodulated 
        /// with the inverse of the alpha value, useful for reducing colour bleeding and accuracy problems at alpha edges)
        /// </summary>
        public void SetToPremodulatedAlphaBlending() { _mode = _premodulatedAlpha._mode; }

        /// <summary>
        /// Set the render state to Additive Alpha Blending (blending based on the alpha value of the source component,
        /// the desitination colour is added to the source colour modulated by alpha)
        /// </summary>
        public void SetToAlphaAdditiveBlending() { _mode = _alphaAdditive._mode; }

        /// <summary>
        /// Set the render state to Additive Saturate Blending (the alpha value is ignored, the desitination colour is 
        /// added to the source colour, however the source colour is multipled by the inverse of the destination colour, 
        /// preventing the addition from blowing out to pure white (eg, 0.75 + 0.75 * (1-0.75) = 0.9375))
        /// </summary>
        public void SetToAdditiveSaturateBlending() { _mode = _additiveSaturate._mode; }

        /// <summary>
        /// Set the render state to Modulate (multiply) Blending (the alpha value is ignored, the desitination colour 
        /// is multipled with the source colour)
        /// </summary>
        public void SetToModulateBlending() { _mode = _modulate._mode; }

        /// <summary>
        /// Set the render state to Modulate Add (multiply+add) Blending (the alpha value is ignored, the desitination
        /// colour multipled with the source colour is added to the desitnation colour)
        /// </summary>
        public void SetToModulateAddBlending() { _mode = _modulateAdd._mode; }

        /// <summary>
        /// Set the render state to Modulate (multiply) Blending, scaled by 2 (the alpha value is ignored, the desitination 
        /// colour is multipled with the source colour, scaled by two)
        /// </summary>
        public void SetToModulateX2Blending() { _mode = _modulateX2._mode; }


        public static explicit operator AlphaBlendState(int state)
        {
            AlphaBlendState value;
            value._mode = state;
            return value;
        }

        public static implicit operator int(AlphaBlendState state)
        {
            return state._mode;
        }

        public static bool operator ==(AlphaBlendState a, AlphaBlendState b)
        {
            return a._mode == b._mode;
        }

        public static bool operator !=(AlphaBlendState a, AlphaBlendState b)
        {
            return a._mode != b._mode;
        }

        public override bool Equals(object obj)
        {
            if (obj is AlphaBlendState)
                return ((AlphaBlendState)obj)._mode == _mode;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _mode;
        }
    }
}
