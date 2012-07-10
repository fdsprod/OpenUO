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

namespace Client.ChainSteps
{
    public delegate void ExecuteChainStepHandler<T>(T state);
    public delegate void DependencyHandler(List<ChainDependency> dependencies);

    public class DelegateChainStepBase<T> : ExecutionChainStepBase<T>
        where T : class
    {
        private ExecuteChainStepHandler<T> _handler;
        private string _name;

        public override string Name
        {
            get { return _name; }
        }

        public DelegateChainStepBase(string name, ExecuteChainStepHandler<T> handler)
            : this(name, handler, (DependencyHandler)null) { }

        public DelegateChainStepBase(string name, ExecuteChainStepHandler<T> handler, DependencyHandler dependencyHandler)
        {
            _name = name;
            _handler = handler;

            if (dependencyHandler != null)
            {
                List<ChainDependency> dependencies = new List<ChainDependency>();

                foreach (ChainDependency dep in dependencies)
                    AddDepenency(dep.Name, dep.MustExist);
            }
        }

        public DelegateChainStepBase(string name, ExecuteChainStepHandler<T> handler, params string[] requiredDependencies)
        {
            _name = name;
            _handler = handler;

            foreach (string dep in requiredDependencies)
                AddDepenency(dep, true);
        }

        protected override void ExecuteOverride(T state)
        {
            _handler(state);
        }
    }
}
