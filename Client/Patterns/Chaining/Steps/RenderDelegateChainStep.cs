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

using Client.ChainSteps;
using Client.Graphics;

namespace Client.Patterns.Chaining
{
    public sealed class RenderDelegateChainStep : DelegateChainStepBase<DrawState>
    {
        public RenderDelegateChainStep(string name, ExecuteChainStepHandler<DrawState> handler)
            : base(name, handler) { }

        public RenderDelegateChainStep(string name, ExecuteChainStepHandler<DrawState> handler, DependencyHandler dependencyHandler)
            : base(name, handler, dependencyHandler) { }

        public RenderDelegateChainStep(string name, ExecuteChainStepHandler<DrawState> handler, params string[] requiredDependencies)
            : base(name, handler, requiredDependencies) { }
        
        protected override sealed void ExecuteOverride(DrawState state)
        {
            base.ExecuteOverride(state);
        }
    }
}
