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
    public class ShaderPassCollection
    {
        private ShaderPass[] _passes;

        public int Count
        {
            get { return _passes.Length; }
        }

        internal ShaderPassCollection(DeviceContext context, Shader shader, ShaderTechnique technique)
        {
            _passes = new ShaderPass[technique.PassCount];

            for (int i = 0; i < _passes.Length; i++)
                _passes[i] = new ShaderPass(context, shader, shader.Effect.GetPass(technique.Handle, i), i);
        }

        public ShaderPass this[int index]
        {
            get { return _passes[index]; }
                
        }

        public ShaderPass this[string name]
        {
            get
            {
                foreach (var param in _passes)
                    if (string.Equals(param.Name, name))
                        return param;

                return null;
            }
        }
    }
}
