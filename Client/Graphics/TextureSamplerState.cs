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
using SharpDX.Direct3D9;

namespace Client.Graphics
{
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Size = 4)]
    [System.Diagnostics.DebuggerStepThrough]
    public struct TextureSamplerState
    {
        private static TextureSamplerState _point = new TextureSamplerState(TextureAddress.Wrap, TextureFilter.Point, TextureFilter.Point, TextureFilter.Point, 0);
        private static TextureSamplerState _bilinear = new TextureSamplerState(TextureAddress.Wrap, TextureFilter.Linear, TextureFilter.Linear, TextureFilter.Point, 0);
        private static TextureSamplerState _trilinear = new TextureSamplerState(TextureAddress.Wrap, TextureFilter.Linear, TextureFilter.Linear, TextureFilter.Linear, 0);
        private static TextureSamplerState _aniLow = new TextureSamplerState(TextureAddress.Wrap, TextureFilter.Anisotropic, TextureFilter.Linear, TextureFilter.Linear, 2);
        private static TextureSamplerState _aniMed = new TextureSamplerState(TextureAddress.Wrap, TextureFilter.Anisotropic, TextureFilter.Linear, TextureFilter.Linear, 4);
        private static TextureSamplerState _aniHigh = new TextureSamplerState(TextureAddress.Wrap, TextureFilter.Anisotropic, TextureFilter.Linear, TextureFilter.Linear, 8);

        [System.Runtime.InteropServices.FieldOffset(0)]
        internal int Mode;

        public static TextureSamplerState PointFiltering
        {
            get { return _point; }
        }

        public static TextureSamplerState BilinearFiltering
        {
            get { return _bilinear; }
        }

        public static TextureSamplerState TrilinearFiltering
        {
            get { return _trilinear; }
        }

        public static TextureSamplerState AnisotropicLowFiltering
        {
            get { return _aniLow; }
        }

        public static TextureSamplerState AnisotropicMediumFiltering
        {
            get { return _aniMed; }
        }

        public static TextureSamplerState AnisotropicHighFiltering
        {
            get { return _aniHigh; }
        }

        public TextureAddress AddressUV
        {
            set
            {
                {
                    const int offset = 0; Mode = ((Mode & ~(7 << offset)) | (7 & ((int)value - 1)) << offset);
                }
                {
                    const int offset = 3; Mode = ((Mode & ~(7 << offset)) | (7 & ((int)value - 1)) << offset);
                }
            }
        }

        public TextureAddress AddressU
        {
            get { const int offset = 0; return (TextureAddress)(((Mode >> offset) & 7) + 1); }
            set { const int offset = 0; Mode = ((Mode & ~(7 << offset)) | (7 & ((int)value - 1)) << offset); }
        }

        public TextureAddress AddressV
        {
            get { const int offset = 3; return (TextureAddress)(((Mode >> offset) & 7) + 1); }
            set { const int offset = 3; Mode = ((Mode & ~(7 << offset)) | (7 & ((int)value - 1)) << offset); }
        }

        public TextureAddress AddressW
        {
            get { const int offset = 6; return (TextureAddress)(((Mode >> offset) & 7) + 1); }
            set { const int offset = 6; Mode = ((Mode & ~(7 << offset)) | (7 & ((int)value - 1)) << offset); }
        }

        public TextureFilter MinFilter
        {
            get { const int offset = 9; return (TextureFilter)(((Mode >> offset) & 3)); }
            set { const int offset = 9; Mode = ((Mode & ~(3 << offset)) | (3 & (Math.Min(3, (int)value))) << offset); }
        }

        public TextureFilter MagFilter
        {
            get { const int offset = 11; return (TextureFilter)(((Mode >> offset) & 3)); }
            set { const int offset = 11; Mode = ((Mode & ~(3 << offset)) | (3 & (Math.Min(2, (int)value))) << offset); }
        }

        public TextureFilter MipFilter
        {
            get { const int offset = 13; return (TextureFilter)(((Mode >> offset) & 3)); }
            set { const int offset = 13; Mode = ((Mode & ~(3 << offset)) | (3 & (Math.Min(2, (int)value))) << offset); }
        }

        public int MaxAnisotropy
        {
            get { const int offset = 16; return (((Mode >> offset) & 15) + 1); }
            set { const int offset = 16; Mode = ((Mode & ~(15 << offset)) | (15 & (Math.Max(0, Math.Min(16, value) - 1))) << offset); }
        }

        public int MaxMipmapLevel
        {
            get { const int offset = 20; return ((((Mode >> offset)) & 255)); }
            set { const int offset = 20; Mode = ((Mode & ~(255 << offset)) | (255 & ((Math.Min(255, value)))) << offset); }
        }

        internal TextureSamplerState(TextureAddress uv, TextureFilter min, TextureFilter mag, TextureFilter mip, int maxAni)
        {
            Mode = 0;
            this.AddressUV = uv;
            this.MinFilter = min;
            this.MagFilter = mag;
            this.MipFilter = mip;
            this.MaxAnisotropy = maxAni;
        }

        internal TextureSamplerState(int mode)
        {
            this.Mode = mode;
        }

        internal void ResetState(DeviceContext context, int index, ref TextureSamplerState current)
        {
            context.SetSamplerState(index, SamplerState.AddressU, AddressU);
            context.SetSamplerState(index, SamplerState.AddressV, AddressV);
            context.SetSamplerState(index, SamplerState.AddressW, AddressW);
            context.SetSamplerState(index, SamplerState.MinFilter, MinFilter);
            context.SetSamplerState(index, SamplerState.MagFilter, MagFilter);
            context.SetSamplerState(index, SamplerState.MipFilter, MipFilter);
            context.SetSamplerState(index, SamplerState.MaxAnisotropy, MaxAnisotropy);
            context.SetSamplerState(index, SamplerState.MaxMipLevel, MaxMipmapLevel);

            current = this;
        }

        public override bool Equals(object obj)
        {
            if (obj is TextureSamplerState)
                return ((TextureSamplerState)obj).Mode == this.Mode;

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Mode;
        }

        public static bool operator ==(TextureSamplerState a, TextureSamplerState b)
        {
            return a.Mode == b.Mode;
        }

        public static bool operator !=(TextureSamplerState a, TextureSamplerState b)
        {
            return a.Mode != b.Mode;
        }

        public static implicit operator int(TextureSamplerState state)
        {
            return state.Mode;
        }

        public static explicit operator TextureSamplerState(int state)
        {
            return new TextureSamplerState(state);
        }
    }
}

