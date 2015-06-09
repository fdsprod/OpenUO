#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// GraphNode.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace OpenUO.Core.Patterns
{
    public sealed class GraphNode<T> : IComparable<GraphNode<T>>
    {
        private readonly List<GraphNode<T>> _dependents;

        public GraphNode(string identifier, T item)
        {
            Guard.RequireIsNotNull(identifier, "identifier");
            Guard.RequireIsNotNull(item, "item");

            _dependents = new List<GraphNode<T>>();
            Identifier = identifier;
            Item = item;
        }

        public string Identifier
        {
            get;
        }

        public T Item
        {
            get;
        }

        public ICollection<GraphNode<T>> DependsOn
        {
            get { return _dependents; }
        }

        public int CompareTo(GraphNode<T> other)
        {
            if (other == null)
            {
                return -1;
            }

            return Identifier.CompareTo(other.Identifier);
        }

        public void AddDependent(GraphNode<T> dependency)
        {
            if (_dependents.Contains(dependency))
            {
                return;
            }

            if (Equals(dependency))
            {
                throw (new Exception(string.Format("Node named '{0}' cannot have a self-referencing dependency.", Identifier)));
            }

            _dependents.Add(dependency);
        }
    }
}