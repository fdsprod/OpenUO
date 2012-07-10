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
using SharpDX.Direct3D9;

namespace Client.Graphics
{
    public abstract class Resource
    {
        private static List<WeakReference> _resources = new List<WeakReference>();
        private static List<Resource> _localResourceList = new List<Resource>();
        private static List<WeakReference> _localRefList = new List<WeakReference>();
        private static Stack<WeakReference> _nullReferences = new Stack<WeakReference>();
        private static bool _resourceCreated = false;
        private static bool _monitoringEnabled = false;

        internal static void ClearResourceTracking()
        {
            _resources.Clear();
            _localResourceList.Clear();
            _localRefList.Clear();
            _nullReferences.Clear();
            _monitoringEnabled = false;
            _resourceCreated = false;
        }

        public static void EnableResourceTracking()
        {
            if (_resourceCreated && !_monitoringEnabled)
                throw new InvalidOperationException("EnableResourceTracking can only be called before any resources have been created");
            _monitoringEnabled = true;
        }

        public static bool EnableResourceTracking(bool throwOnError)
        {
            if (_resourceCreated && !_monitoringEnabled)
            {
                if (throwOnError)
                    EnableResourceTracking();
                return false;
            }
            _monitoringEnabled = true;
            return true;
        }

        public static int CountDisposedResourcesStillAlive()
        {
            int count = 0;
            lock (_resources)
            {
                GetResources(_localResourceList);
                foreach (Resource r in _localResourceList)
                {
                    if (r.IsDisposed)
                        count++;
                }
                _localResourceList.Clear();
            }
            return count;
        }

        public static int CountDisposedResourcesStillAlive(ResourceType filter)
        {
            int count = 0;
            lock (_resources)
            {
                GetResources(_localResourceList);
                foreach (Resource r in _localResourceList)
                {
                    if (r.IsDisposed && r.ResourceType == filter)
                        count++;
                }
                _localResourceList.Clear();
            }
            return count;
        }

        public static int CountResourcesNotUsedByDevice()
        {
            int count = 0;
            lock (_resources)
            {
                GetResources(_localResourceList);
                foreach (Resource r in _localResourceList)
                {
                    if (r.IsDisposed == false && r.InUse == false)
                        count++;
                }
                _localResourceList.Clear();
            }
            return count;
        }

        public static int CountResourcesNotUsedByDevice(ResourceType filter)
        {
            int count = 0;
            lock (_resources)
            {
                GetResources(_localResourceList);
                foreach (Resource r in _localResourceList)
                {
                    if (r.IsDisposed == false && r.InUse == false && r.ResourceType == filter)
                        count++;
                }
                _localResourceList.Clear();
            }
            return count;
        }

        public static int GetAllAllocatedManagedBytes()
        {
            return GetAllAllocatedManagedBytes((ResourceType)0);
        }

        public static int GetAllAllocatedDeviceBytes()
        {
            return GetAllAllocatedDeviceBytes((ResourceType)0);
        }
                
        public static int GetAllAllocatedManagedBytes(ResourceType filter)
        {
            int bytes = 0;
            lock (_localResourceList)
            {
                GetResources(_localResourceList);
                foreach (Resource r in _localResourceList)
                {
                    if (r.ResourceType == filter)
                        bytes += r.GetAllocatedManagedBytes();
                }
                _localResourceList.Clear();
            }
            return bytes;
        }

        public static int GetAllAllocatedDeviceBytes(ResourceType filter)
        {
            int bytes = 0;
            lock (_localResourceList)
            {
                GetResources(_localResourceList);
                foreach (Resource r in _localResourceList)
                {
                    if (r.ResourceType == filter || filter == (ResourceType)0)
                        bytes += r.GetAllocatedDeviceBytes();
                }
                _localResourceList.Clear();
            }
            return bytes;
        }

        public static void GetResourceCountByType(IDictionary<Type, int> resourceCounts)
        {
            if (resourceCounts == null)
                throw new ArgumentNullException();

            lock (_localResourceList)
            {
                GetResources(_localResourceList);
                foreach (Resource r in _localResourceList)
                {
                    Type type = r.GetType();
                    int count;
                    if (resourceCounts.TryGetValue(type, out count))
                        resourceCounts[type] = count + 1;
                    else
                        resourceCounts.Add(type, 1);
                }
                _localResourceList.Clear();
            }
        }

        public static int GetResourceCount()
        {
            lock (_localResourceList)
            {
                GetResources(_localResourceList);
                int count = _localResourceList.Count;
                _localResourceList.Clear();

                return count;
            }
        }

        public static int GetResourceCount(ResourceType filter)
        {
            lock (_localResourceList)
            {
                GetResources(_localResourceList);

                int count = 0;

                foreach (Resource res in _localResourceList)
                    if (res.ResourceType == filter)
                        count++;

                _localResourceList.Clear();

                return count;
            }
        }

        private static void GetResources(List<Resource> resourceList)
        {
            if (!_monitoringEnabled)
                throw new ArgumentException("Resource Tracking is not enabled");

            lock (_resources)
            {
                foreach (WeakReference wr in _resources)
                {
                    Resource target = wr.Target as Resource;

                    if (target != null)
                    {
                        resourceList.Add(target);
                        _localRefList.Add(wr);
                    }
                    else
                    {
                        wr.Target = null;
                        _nullReferences.Push(wr);
                    }
                }

                if (_localRefList.Count != _resources.Count)
                {
                    _resources.Clear();
                    _resources.AddRange(_localRefList);
                }

                _localRefList.Clear();
            }
        }

        internal abstract bool InUse { get; }
        internal abstract bool IsDisposed { get; }
        internal abstract ResourceType ResourceType { get; }

        internal Resource()
        {
            _resourceCreated = true;

            if (_monitoringEnabled)
            {
                lock (_resources)
                {
                    WeakReference wr = null;

                    if (_nullReferences.Count > 0)
                        wr = _nullReferences.Pop();
                    else
                        wr = new WeakReference(null);

                    wr.Target = this;
                    _resources.Add(wr);
                }
            }
        }

        public void Warm(DrawState state)
        {
            lock (this)
                WarmOverride(state);
        }

        internal abstract int GetAllocatedManagedBytes();

        internal abstract int GetAllocatedDeviceBytes();

        internal abstract void WarmOverride(DrawState state);
    }
}
