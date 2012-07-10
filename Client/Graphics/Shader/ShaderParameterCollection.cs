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
    public class ShaderParameterCollection
    {
        private ShaderParameter[] _parameters;

        public int Count
        {
            get { return _parameters.Length; }
        }

        internal ShaderParameterCollection(Shader shader)
        {
            int count = shader.Effect.Description.Parameters;

            _parameters = new ShaderParameter[count];

            for (int i = 0; i < count; i++)
                _parameters[i] = new ShaderParameter(shader, shader.Effect.GetParameter(null, i));
        }

        public ShaderParameter this[int index]
        {
            get
            {
                return _parameters[index];
            }
        }

        public ShaderParameter this[string name]
        {
            get
            {
                foreach (var param in _parameters)
                    if (string.Equals(param.Name, name))
                        return param;

                return null;
            }
        }
    }
}
