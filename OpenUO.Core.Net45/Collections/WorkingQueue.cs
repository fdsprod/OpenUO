#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// WorkingQueue.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region Usings

using System.Collections.Generic;

#endregion

namespace OpenUO.Core.Collections
{
    public class WorkingQueue<T>
    {
        private readonly object _syncRoot = new object();
        private Queue<T> _queue = new Queue<T>();
        private Queue<T> _workingQueue = new Queue<T>();

        public int Count
        {
            get
            {
                lock (_syncRoot)
                {
                    return _queue.Count;
                }
            }
        }

        public void Enqueue(T item)
        {
            lock (_syncRoot)
            {
                _workingQueue.Enqueue(item);
            }
        }

        public T Dequeue()
        {
            lock (_syncRoot)
            {
                return _queue.Dequeue();
            }
        }

        public void Slice()
        {
            lock (_syncRoot)
            {
                var temp = _workingQueue;
                _workingQueue = _queue;
                _queue = temp;
            }
        }
    }
}