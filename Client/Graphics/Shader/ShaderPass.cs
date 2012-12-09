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
    public class ShaderPass
    {
        internal int Index;
        internal DeviceContext Context;
        internal Effect Effect;
        internal EffectHandle Handle;
        internal bool HasBegun;
        internal Shader Shader;

        public readonly string Name;

        internal ShaderPass(DeviceContext context, Shader shader, EffectHandle passHandle, int index)
        {
            Shader = shader;
            Effect = shader.Effect;
			Handle = passHandle; // TODO: Correct assignment, or reserved for a different handle?
            Context = context;
            Index = index;

            PassDescription desc = shader.Effect.GetPassDescription(passHandle);
            Name = desc.Name;
        }

        public void Apply()
        {            
            if (Context.ActivePass != this)
                Context.ActivePass = this;
            
            if (HasBegun)
            {
                if(Shader.NeedsCommitChanges)
                    Effect.CommitChanges();
            }
            else
            {
                Effect.Begin();
                Effect.BeginPass(Index);
            }

            HasBegun = true;
        }

        internal void End()
        {
            Effect.EndPass();
            Effect.End();

            HasBegun = false;
        }
    }
}
