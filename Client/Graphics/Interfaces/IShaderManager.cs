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
    public interface IShaderManager
    {
        TShader GetShader<TShader>() where TShader : Shader;
        Shader GetShader(string effectName);

        void Register<TShader>(TShader shader) where TShader : Shader;
        void Register(string shaderName, byte[] shaderData);
        void Register(string shaderName, Shader shader);
    }
}
