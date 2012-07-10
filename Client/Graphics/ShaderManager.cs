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

using System.Collections.Generic;
using OpenUO.Core.Patterns;
using SharpDX.Direct3D9;

namespace Client.Graphics
{
    internal class ShaderManager : IShaderManager
    {
        private readonly Dictionary<string, Shader> _shaderCache;
        private readonly DeviceContext _context;

        public ShaderManager(IoCContainer container)
        {
            IDeviceContextService contextService = container.Resolve<IDeviceContextService>();

            _context = contextService.Context;
            _shaderCache = new Dictionary<string, Shader>();
        }

        public TShader GetShader<TShader>() where TShader : Shader
        {
            return (TShader)_shaderCache[typeof(TShader).FullName];
        }

        public Shader GetShader(string effectName)
        {
            return _shaderCache[effectName];
        }

        public void Register<TShader>(TShader shader) where TShader : Shader
        {
            Register(typeof(TShader).FullName, shader);
        }
        
        public void Register(string shaderName, byte[] shaderData)
        {            
#if DEBUG
            Shader shader = Shader.FromMemory(_context, shaderData, ShaderFlags.Debug);
#else
            Shader shader = Shader.FromMemory(_context, shaderData, ShaderFlags.None);
#endif          
            Register(shaderName, shader);
        }

        public void Register(string shaderName, Shader shader)
        {
            if (_shaderCache.ContainsKey(shaderName))
                _shaderCache[shaderName] = shader;
            else
                _shaderCache.Add(shaderName, shader);  
        }
    }
}
