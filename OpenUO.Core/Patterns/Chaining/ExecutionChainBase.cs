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

namespace OpenUO.Core.Patterns
{
    public abstract class ExecutionChainBase<T> : IChain<T>
        where T : class
    {
        private readonly object _syncRoot = new object();

        private Dictionary<string, IChainStep<T>> _steps;
        private IChainStep<T> _head;
        private bool _isFrozen;
        private bool _isExecuting;

        public string Name
        {
            get { return GetType().Name; }
        }

        public bool IsFrozen
        {
            get { return _isFrozen; }
        }

        public void Freeze()
        {
            _isFrozen = true;
        }

        protected ExecutionChainBase()
        {
            _steps = new Dictionary<string, IChainStep<T>>();
        }

        public void Execute(T state)
        {
            try
            {
                lock (_syncRoot)
                {
                    Guard.Assert(!_isExecuting, string.Format("Chain {0} was executed while already executing.", Name));

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

                if (head != null)
                    head.Execute(state);
            }
            finally
            {
                lock (_syncRoot)
                    _isExecuting = false;
            }
        }

        public IChain<T> RegisterStep<TStep>(TStep step) where TStep : class, IChainStep<T>
        {
            lock (_syncRoot)
            {
                if (_isExecuting)
                    throw new Exception("Cannot add chainsteps while the chain is executing.");

                if (_isFrozen)
                    throw new Exception("Chainsteps must be registered before the chain is frozen.");

                step.Chain = this;
                _steps.Add(step.Name, step);
            }

            return this;
        }

        public IChain<T> RegisterStep<TStep>() where TStep : class, IChainStep<T>
        {
            lock (_syncRoot)
            {
                if (_isExecuting)
                    throw new Exception("Cannot add chainsteps while the chain is executing.");

                if (_isFrozen)
                    throw new Exception("Chainsteps must be registered before the chain is frozen.");

                IChainStep<T> step = CreateStep<TStep>();
                step.Chain = this;
                _steps.Add(step.Name, step);
            }

            return this;
        }

        public bool UnregisterStep<TStep>() where TStep : class, IChainStep<T>
        {
            lock (_syncRoot)
            {
                if (_isExecuting)
                    throw new Exception("Cannot remove chainsteps while the chain is executing.");

                if (_isFrozen)
                    throw new Exception("Cannot unregister a chainstep after the chain has been frozen.");

                var remove = _steps.Values.Where(step => step.GetType() == typeof(TStep)).FirstOrDefault();
                return _steps.Remove(remove.Name);
            }
        }

        protected abstract TStep CreateStep<TStep>() where TStep : class, IChainStep<T>;

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
                        Guard.Assert(
                            _steps.ContainsKey(dependency.Name),
                            string.Format(
                                "Cannot execute chain '{0}' because step '{1}' has a mandatory dependency on step '{2}' and '{2}' cannot be found in the {0} chain.",
                                Name, step.Name, dependency.Name));
                    }

                    var dependentNode = graph.GetNode(dependency.Name);
                    node.AddDependent(dependentNode);
                }
            }

            var ordered = graph.ComputeDependencyOrderedList().Select(node => node.Item).ToArray();

            if (ordered.Length == 0)
                return null;

            if (ordered.Length == 1)
                return ordered[0];

            for (int i = 0; i < ordered.Length - 1; i++)
                ordered[i + 1].Successor = ordered[i];

            return ordered[ordered.Length - 1];
        }
    }
}
