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
    public class ShaderTechnique
    {
        internal readonly int PassCount;
        internal readonly EffectHandle Handle;

        public readonly string Name;
        public readonly ShaderPassCollection Passes;

        internal ShaderTechnique(DeviceContext context, Shader shader, EffectHandle techniqueHandle)
        {
            TechniqueDescription desc = shader.Effect.GetTechniqueDescription(techniqueHandle);

            Handle = techniqueHandle;
            PassCount = desc.Passes;
            Name = desc.Name;
            Passes = new ShaderPassCollection(context, shader, this);
        }
    }
}
