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

namespace Client.Patterns.Chaining
{
    public sealed class UpdateDelegateChainStep : DelegateChainStepBase<UpdateState>
    {
        public UpdateDelegateChainStep(string name, ExecuteChainStepHandler<UpdateState> handler, DependencyHandler dependencyHandler)
            : base(name, handler, dependencyHandler) { }

        public UpdateDelegateChainStep(string name, ExecuteChainStepHandler<UpdateState> handler, params string[] dependencies)
            : base(name, handler, dependencies) { }

        protected override sealed void ExecuteOverride(UpdateState state)
        {
            base.ExecuteOverride(state);
        }
    }
}
