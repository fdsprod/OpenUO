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

namespace OpenUO.Core.Patterns
{
    public sealed class GraphNode<T> : IComparable<GraphNode<T>>
    {
        private readonly List<GraphNode<T>> _dependents;
        private readonly string _identifier;
        private readonly T _item;

        public GraphNode(string identifier, T item)
        {
            Guard.AssertIsNotNull(identifier, "identifier");
            Guard.AssertIsNotNull(item, "item");

            _dependents = new List<GraphNode<T>>();
            _identifier = identifier;
            _item = item;
        }

        public string Identifier
        {
            get { return _identifier; }
        }

        public T Item
        {
            get { return _item; }
        }

        public void AddDependent(GraphNode<T> dependency)
        {
            if (_dependents.Contains(dependency))
                return;

            if (Equals(dependency))
                throw (new Exception(string.Format("Node named '{0}' cannot have a self-referencing dependency.", _identifier)));

            _dependents.Add(dependency);
        }

        public ICollection<GraphNode<T>> DependsOn
        {
            get { return _dependents; }
        }
        
        public int CompareTo(GraphNode<T> other)
        {
            if (other == null)
                return -1;

            return _identifier.CompareTo(other._identifier);
        }
    }
}
