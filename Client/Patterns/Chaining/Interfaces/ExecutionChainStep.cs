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
using OpenUO.Core;

namespace Client.Patterns
{
    public abstract class ExecutionChainStep<T> : IChainStep<T>
    {
        private List<ChainDependency> _dependencies;
        private string _name;
        private bool _cancelExecution;
        private IChainStep<T> _successor;

        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                    _name = GetType().Name;

                return _name;
            }
        }

        public bool CancelExecution
        {
            get { return _cancelExecution; }
            set { _cancelExecution = value; }
        }

        public IChainStep<T> Successor
        {
            get { return _successor; }
            set 
            {
                Asserter.AssertIsNull(value, "value");

                if (_successor == null)
                    _successor = value;
                else
                {
                    var successor = _successor;
                    _successor = value;
                    value.Successor = successor;
                }
            }
        }

        public ChainDependency[] Dependencies
        {
            get { return _dependencies.ToArray(); }
        }

        public ExecutionChainStep()
            : this(string.Empty) { }

        public ExecutionChainStep(string name)
        {
            _name = name;
            _dependencies = new List<ChainDependency>();
        }

        public void Execute(T state)
        {
            ExecuteOverride(state);

            if (!CancelExecution && _successor != null)
                _successor.Execute(state);
        }

        protected void AddDepenency(string name, bool mustExist)
        {
            _dependencies.Add(new ChainDependency(name, mustExist));
        }

        protected abstract void ExecuteOverride(T state);
    }
}
