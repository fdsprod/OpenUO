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
using SharpDX;
using SharpDX.Direct3D9;

namespace Client.Graphics.Elements
{
    public class TexturedElement : ElementRect
    {
        private static TextureSamplerState _pointFilter, _bilinearFilter;

        private Rectangle? _crop;
        private Vector2 _pixelSize;
        private Texture2D _texture;

        private bool _usePointFilter = false;
                
        public Rectangle? TextureCrop
        {
            get { return _crop; }
            set
            {
                if (_crop != value)
                {
                    _crop = value;
                    SetDirty();
                }
            }
        }

        public bool UsePointFiltering
        {
            get { return _usePointFilter; }
            set { _usePointFilter = value; }
        }

        public Texture2D Texture
        {
            get { return _texture; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                if (value != _texture)
                {
                    _texture = value;
                    SetDirty();
                }
            }
        }

        public TexturedElement(Vector2 sizeInPixels)
            : base(sizeInPixels) { }

        public TexturedElement(Texture2D texture, Vector2 sizeInPixels)
            : base(sizeInPixels)
        {
            _texture = texture;
        }

        public TexturedElement(Texture2D texture, Vector2 size, bool normalised)
            : base(size, normalised)
        {
            _texture = texture;
        }

        static TexturedElement()
        {
            _bilinearFilter = TextureSamplerState.BilinearFiltering;
            _bilinearFilter.AddressUV = TextureAddress.Clamp;

            _pointFilter = TextureSamplerState.PointFiltering;
            _pointFilter.AddressUV = TextureAddress.Clamp;
        }

        protected override void PreDraw(DeviceContext context, Vector2 size)
        {
            if (size != _pixelSize)
            {
                SetDirty();
                _pixelSize = size;
            }

            base.PreDraw(context, size);
        }

        protected override void WriteTextureCoords(ref Vector2 topLeft, ref Vector2 topRight, ref Vector2 bottomLeft, ref Vector2 bottomRight)
        {
            float x0 = 0, x1 = 1;
            float y0 = 0, y1 = 1;
            
            if (_pixelSize.X != 0 && _pixelSize.Y != 0)
            {
                x0 = 0.5f / _pixelSize.X;
                x1 = 1 + x0;
                y0 = 0.5f / _pixelSize.Y;
                y1 = 1 + y0;

                if (_crop != null && _texture != null)
                {
                    Vector2 size = new Vector2(1.0f / _texture.Size.X, 1.0f / _texture.Size.Y);
                    Rectangle r = _crop.Value;

                    x0 *= r.Width * size.X;
                    y0 *= r.Height * size.Y;

                    x1 = (r.Left + r.Width) * size.X + x0;
                    y1 = (r.Top + r.Height) * size.Y + y0;

                    x0 += r.Left * size.X;
                    y0 += r.Top * size.Y;
                }
            }


            topLeft = new Vector2(x0, y0);
            topRight = new Vector2(x1, y0);
            bottomLeft = new Vector2(x0, y1);
            bottomRight = new Vector2(x1, y1);
        }

        protected override void BindShader(DrawState state, bool maskOnly)
        {
            SimpleTextureEffect shader = state.GetShader<SimpleTextureEffect>();

            shader.Texture = _texture;
            shader.Bind(state);
        }
    }
}
