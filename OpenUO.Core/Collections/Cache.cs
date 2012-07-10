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

namespace OpenUO.Core.Collections
{
    public class Cache<T, U> : IDisposable, IEnumerable<U>
    {
        protected readonly Dictionary<T, CacheItem<U>> InternalCache;
        protected readonly TimeSpan TimeToExpire;

        public Cache(TimeSpan timeToExpire, int capacity)
        {
            TimeToExpire = timeToExpire;
            InternalCache = new Dictionary<T, CacheItem<U>>(capacity);
        }

        public void Clean()
        {
            var keys = InternalCache.Keys.ToArray();

            for (int i = 0; i < keys.Length; i++)
            {
                T key = keys[i];
                var cacheItem = InternalCache[key];

                if (cacheItem.IsExpired)
                    InternalCache.Remove(key);
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

        protected virtual void OnItemDisposing(CacheItem<U> cacheItem)
        {

        }

        public virtual U this[T index]
        {
            get
            {
                U item = default(U);
                CacheItem<U> cacheItem;

                if (InternalCache.TryGetValue(index, out cacheItem))
                    item = cacheItem.Value;

                return item;
            }
            set
            {
                CacheItem<U> cacheItem;

                if (!InternalCache.TryGetValue(index, out cacheItem))
                {
                    cacheItem = new CacheItem<U>(value, TimeToExpire);
                    InternalCache.Add(index, cacheItem);
                }

                cacheItem.Value = value;
            }
        }

        public IEnumerator<U> GetEnumerator()
        {
            return InternalCache.Values.Select(ci => ci.Value).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return InternalCache.GetEnumerator();
        }

        protected class CacheItem<TValueType> : IDisposable
        {
            private TValueType _value;
            private DateTime _lastAccess;
            private readonly TimeSpan _timeToExpire;

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

            public CacheItem(TValueType value, TimeSpan timeToExpire)
            {
                _value = value;
                _lastAccess = DateTime.Now;
                _timeToExpire = timeToExpire;
            }

            public bool IsExpired
            {
                get { return DateTime.Now >= _lastAccess + _timeToExpire; }
            }

            public void Dispose()
            {
                if (_value is IDisposable)
                    ((IDisposable)_value).Dispose();
            }
        }
    }
}
