#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// SafeDictionary.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace OpenUO.Core.Collections
{
    public class SafeDictionary<TKey, TValue> : IDisposable
    {
        private readonly Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();
        private readonly object _syncRoot = new object();

        public TValue this[TKey key]
        {
            set
            {
                lock (_syncRoot)
                {
                    TValue current;

                    if (_dictionary.TryGetValue(key, out current))
                    {
                        var disposable = current as IDisposable;

                        if (disposable != null)
                        {
                            disposable.Dispose();
                        }
                    }

                    _dictionary[key] = value;
                }
            }
        }

        public IEnumerable<TKey> Keys
        {
            get { return _dictionary.Keys; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Clear()
        {
            lock (_syncRoot)
            {
                _dictionary.Clear();
            }
        }

        public bool Remove(TKey key)
        {
            lock (_syncRoot)
            {
                return _dictionary.Remove(key);
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (_syncRoot)
            {
                return _dictionary.TryGetValue(key, out value);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (_syncRoot)
                {
                    IEnumerable<IDisposable> disposableItems = _dictionary.Values.Where(o => o is IDisposable).Cast<IDisposable>().ToArray();

                    foreach (var item in disposableItems)
                    {
                        item.Dispose();
                    }

                    _dictionary.Clear();
                }
            }
        }
    }
}