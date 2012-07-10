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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;
using OpenUO.Core;
using OpenUO.Core.Patterns;

namespace Client.Patterns.Chaining
{
    public class ExecutionChain<T> : IChain<T>
    {
        private readonly object _syncRoot;

        private Dictionary<string, IChainStep<T>> _steps;
        private IChainStep<T> _head;
        private IKernel _kernel;
        private bool _isFrozen;
        private bool _isExecuting;
        private string _name;

        public string Name
        {
            get { return _name; }
        }

        public bool IsFrozen
        {
            get { return _isFrozen; }
        }

        public void Freeze()
        {
            _isFrozen = true;
        }

        protected ExecutionChain(string name)
        {
            _name = name;
            _steps = new Dictionary<string, IChainStep<T>>();
        }

        public void Execute(T state)
        {
            lock(_syncRoot)
            {
                if(_isExecuting)
                    return;

                _isExecuting = true;
            }

            IChainStep<T> head;

            if (_isFrozen && _head != null)
                head = _head;
            else 
            {
                head = ComputeChainSequence();

                if (_isFrozen)
                    _head = head;
            }

            _head.Execute(state);

            lock (_syncRoot)
                _isExecuting = false;

        }

        private IChainStep<T> ComputeChainSequence()
        {
            DirectedAcyclicGraph<IChainStep<T>> graph = new DirectedAcyclicGraph<IChainStep<T>>();

            foreach (var step in _steps.Values)
                graph.AddNode(new GraphNode<IChainStep<T>>(step.Name, step));

            foreach (var step in _steps.Values)
            {
                GraphNode<IChainStep<T>> node = graph.GetNode(step.Name);

                foreach (var dependency in step.Dependencies)
                {
                    if (dependency.MustExist)
                    {
                        Asserter.Assert(
                            _steps.ContainsKey(dependency.Name),
                            string.Format(
                                "Cannot execute chain '{0}' because step '{1}' has a mandatory dependency on step '{2}' and '{2}' cannot be found in the {0} chain.",
                                _name, step.Name, dependency.Name));
                    }

                    var dependentNode = graph.GetNode(dependency.Name);
                    node.AddDependent(dependentNode);
                }
            }

            var ordered = graph.ComputeDependencyOrderedList().Select(node => node.Item).ToArray();

            for (int i = ordered.Length - 2; i > 0; i--)
                ordered[i].Successor = ordered[i + 1];

            return ordered[0];
        }

        public void RegisterStep<TStep>() where TStep : IChainStep<T>
        {
            lock (_syncRoot)
            {
                if (_isExecuting)
                    throw new Exception("Cannot add chainsteps while the chain is executing.");

                IChainStep<T> step = _kernel.Get<TStep>();
                _steps.Add(step.Name, step);
            }
        }
    }
}
