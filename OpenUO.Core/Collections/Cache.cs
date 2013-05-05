#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   Cache.cs
//  *
//  *   This program is free software; you can redistribute it and/or modify
//  *   it under the terms of the GNU General Public License as published by
//  *   the Free Software Foundation; either version 3 of the License, or
//  *   (at your option) any later version.
//  ***************************************************************************/

#endregion

#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace OpenUO.Core.Collections
{
    public class Cache<TKey, TValue> : IDisposable, IEnumerable<TValue>
    {
        protected readonly Dictionary<TKey, CacheItem<TValue>> InternalCache;
        protected readonly TimeSpan TimeToExpire;

        public Cache(TimeSpan timeToExpire, int capacity)
        {
            TimeToExpire = timeToExpire;
            InternalCache = new Dictionary<TKey, CacheItem<TValue>>(capacity);
        }

        public virtual TValue this[TKey index]
        {
            get
            {
                TValue item = default(TValue);
                CacheItem<TValue> cacheItem;

                if (InternalCache.TryGetValue(index, out cacheItem))
                {
                    item = cacheItem.Value;
                }

                return item;
            }
            set
            {
                CacheItem<TValue> cacheItem;

                if (!InternalCache.TryGetValue(index, out cacheItem))
                {
                    cacheItem = new CacheItem<TValue>(value, TimeToExpire);
                    InternalCache.Add(index, cacheItem);
                }

                cacheItem.Value = value;
            }
        }

        public virtual void Dispose()
        {
            foreach (var cacheItem in InternalCache.Values)
            {
                OnItemDisposing(cacheItem);
                cacheItem.Dispose();
            }

            InternalCache.Clear();
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return InternalCache.Values.Select(ci => ci.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return InternalCache.GetEnumerator();
        }

        public void Clean()
        {
            TKey[] keys = InternalCache.Keys.ToArray();

            for (int i = 0; i < keys.Length; i++)
            {
                TKey key = keys[i];
                CacheItem<TValue> cacheItem = InternalCache[key];

                if (cacheItem.IsExpired)
                {
                    InternalCache.Remove(key);
                }
            }
        }

        protected virtual void OnItemDisposing(CacheItem<TValue> cacheItem)
        {
        }

        protected class CacheItem<TValueType> : IDisposable
        {
            private readonly TimeSpan _timeToExpire;
            private DateTime _lastAccess;
            private TValueType _value;

            public CacheItem(TValueType value, TimeSpan timeToExpire)
            {
                _value = value;
                _lastAccess = DateTime.Now;
                _timeToExpire = timeToExpire;
            }

            public TValueType Value
            {
                get
                {
                    _lastAccess = DateTime.Now;
                    return _value;
                }
                set
                {
                    _lastAccess = DateTime.Now;
                    _value = value;
                }
            }

            public bool IsExpired
            {
                get { return DateTime.Now >= _lastAccess + _timeToExpire; }
            }

            public void Dispose()
            {
                IDisposable disposable = _value as IDisposable;

                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}