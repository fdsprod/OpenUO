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

using SharpDX;
using SharpDX.Direct3D9;

namespace Client.Graphics
{
    public class ShaderParameter
    {
        internal Effect Effect;
        internal EffectHandle Handle;
        internal readonly int ElementCount;
        internal Shader Shader;

        public readonly ShaderElementCollection Elements;
        public readonly ParameterClass Class;
        public readonly string Name;
        public readonly string Semantic;
        public readonly ParameterType Type;

        internal ShaderParameter(Shader shader, EffectHandle parameterHandle)
        {
            Shader = shader;
            Effect = shader.Effect;
            Handle = parameterHandle;

            ParameterDescription desc = shader.Effect.GetParameterDescription(parameterHandle);

            Name = desc.Name;
            Semantic = desc.Semantic;

            ElementCount = desc.Elements;
            Elements = new ShaderElementCollection(shader, this);
        }

        public void SetValue(string str)
        {
            Effect.SetString(Handle, str);
            Shader.NeedsCommitChanges = true;
        }

        public void SetTexture(BaseTexture texture)
        {
            Effect.SetTexture(Handle, texture);
            Shader.NeedsCommitChanges = true;
        }

        public void SetValue(bool value)
        {
            Effect.SetValue(Handle, value);
            Shader.NeedsCommitChanges = true;
        }

        public void SetValue(bool[] values)
        {
            Effect.SetValue(Handle, values);
            Shader.NeedsCommitChanges = true;
        }

        public void SetValue(float value)
        {
            Effect.SetValue(Handle, value);
            Shader.NeedsCommitChanges = true;
        }

        public void SetValue(float[] values)
        {
            Effect.SetValue(Handle, values);
        }

        public void SetValue(int value)
        {
            Effect.SetValue(Handle, value);
            Shader.NeedsCommitChanges = true;
        }

        public void SetValue(int[] values)
        {
            Effect.SetValue(Handle, values);
            Shader.NeedsCommitChanges = true;
        }

        public void SetValue(Matrix value)
        {
            Effect.SetValue(Handle, value);
            Shader.NeedsCommitChanges = true;
        }

        public void SetValue(Matrix[] values)
        {
            Effect.SetValue(Handle, values);
            Shader.NeedsCommitChanges = true;
        }

        public void SetValue<T>(T value) where T : struct
        {
            Effect.SetValue<T>(Handle, value);
            Shader.NeedsCommitChanges = true;
        }

        public void SetValue<T>(T[] values) where T : struct
        {
            Effect.SetValue<T>(Handle, values);
            Shader.NeedsCommitChanges = true;
        }

        public void SetValue(Vector4 value)
        {
            Effect.SetValue(Handle, value);
            Shader.NeedsCommitChanges = true;
        }

        public void SetValue(Vector4[] values)
        {
            Effect.SetValue(Handle, values);
            Shader.NeedsCommitChanges = true;
        }
    }
}
