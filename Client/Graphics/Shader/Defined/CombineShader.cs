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
    public sealed class CombineShader : Shader
    {
        private Texture2D _worldTexture;
        private Texture2D _uiTexture;

        public Texture2D UITexture
        {
            get { return _uiTexture; }
            set
            {
                if (_uiTexture != value)
                {
                    _uiTexture = value;
                    Parameters["UITexture"].SetTexture(value);
                }
            }
        }

        public Texture2D WorldTexture
        {
            get { return _worldTexture; }
            set
            {
                if (_worldTexture != value)
                {
                    _worldTexture = value;
                    Parameters["WorldTexture"].SetTexture(value);
                }
            }
        }

        public CombineShader(DeviceContext context)
            : base(context, Resources.CombineEffect)
        {

        }
    }
}
