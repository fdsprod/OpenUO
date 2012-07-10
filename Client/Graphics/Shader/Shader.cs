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

using OpenUO.Core;
using SharpDX.Direct3D9;

namespace Client.Graphics
{
    public class Shader : DeviceResource
    {
        public static Shader FromMemory(DeviceContext context, byte[] effectData, ShaderFlags shaderFlags)
        {
            return new Shader(context, effectData);
        }

        internal bool NeedsCommitChanges;
        internal Effect Effect;

        private byte[] _effectData;
        private ShaderTechnique _currentTechnique;
        private ShaderTechniqueCollection _techniques;
        private ShaderParameterCollection _parameters;

        public ShaderParameterCollection Parameters
        {
            get { return _parameters; }
        }

        public ShaderTechniqueCollection Techniques
        {
            get { return _techniques; }
        }

        public ShaderTechnique CurrentTechnique
        {
            get { return _currentTechnique; }
            set
            {
                Guard.AssertIsNotNull(value, "value");
                _currentTechnique = value; 
            }
        }

        internal Shader(DeviceContext context, byte[] effectData)
            : base(context)
        {
            _effectData = effectData;

            Initialize();
        }

        public void CommitChanges()
        {
            Effect.CommitChanges();
        }

        public void Bind(DrawState state)
        {
            for (int i = 0; i < Parameters.Count; i++)
            {
                ShaderParameter parameter = Parameters[i];
                ISemantic semantic;

                if (!string.IsNullOrEmpty(parameter.Semantic) && state.SemanticMappings.TryGetValue(parameter.Semantic, out semantic))
                    semantic.Apply(parameter);
            }

            state.Context.ActivePass = CurrentTechnique.Passes[0];
        }

        private void Initialize()
        {
            Effect = Effect.FromMemory(Context, _effectData, ShaderFlags.None);
            _techniques = new ShaderTechniqueCollection(Context, this);
            _parameters = new ShaderParameterCollection(this);

            CurrentTechnique = _techniques[0];
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing && Effect != null)
            {
                Effect.Dispose();
                Effect = null;
            }
        }

        protected override void OnContextReset(DeviceContext context)
        {
            Initialize();
        }

        protected override void OnContextLost(DeviceContext context)
        {
            Dispose(true);
        }

        internal override int GetAllocatedDeviceBytes()
        {
            return _effectData.Length;
        }

        internal override int GetAllocatedManagedBytes()
        {
            return _effectData.Length;
        }

        internal override bool InUse
        {
            get { return Effect != null; }
        }

        internal override bool IsDisposed
        {
            get { return Effect == null; }
        }

        internal override ResourceType ResourceType
        {
            get { return (ResourceType)8; }
        }

        internal override void WarmOverride(DrawState state)
        {

        }
    }
}
