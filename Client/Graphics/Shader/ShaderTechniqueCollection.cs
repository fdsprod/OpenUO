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
    public class ShaderTechniqueCollection
    {
        private ShaderTechnique[] _techniques;

        public int Count
        {
            get { return _techniques.Length; }
        }

        internal ShaderTechniqueCollection(DeviceContext context, Shader shader)
        {
            int count = shader.Effect.Description.Techniques;

            _techniques = new ShaderTechnique[count];

            for (int i = 0; i < _techniques.Length; i++)
                _techniques[i] = new ShaderTechnique(context, shader, shader.Effect.GetTechnique(i));
        }

        public ShaderTechnique this[int index]
        {
            get { return _techniques[index]; }
                
        }

        public ShaderTechnique this[string name]
        {
            get
            {
                foreach (var param in _techniques)
                    if (string.Equals(param.Name, name))
                        return param;

                return null;
            }
        }
    }
}
