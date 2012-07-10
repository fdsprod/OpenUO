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
    public sealed class SimpleTextureEffect : Shader
    {
        private Texture2D _texture;

        public Texture2D Texture
        {
            get { return _texture; }
            set
            {
                if (_texture != value)
                {
                    _texture = value;
                    Parameters["Texture"].SetTexture(value);
                }
            }
        }

        public SimpleTextureEffect(DeviceContext context)
            : base(context, Resources.SimpleTextureEffect)
        {

        }
    }
}
