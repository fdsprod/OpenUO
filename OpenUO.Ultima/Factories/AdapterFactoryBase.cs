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
using OpenUO.Core;
using OpenUO.Core.Patterns;
using OpenUO.Ultima.Adapters;

namespace OpenUO.Ultima
{
    public abstract class AdapterFactoryBase : IDisposable
    {
        private IoCContainer _container;
        private IStorageAdapter _adapter;
        private InstallLocation _install;

        protected AdapterFactoryBase(InstallLocation install, IoCContainer container)
        {
            Guard.AssertIsNotNull(install, "install");
            Guard.AssertIsNotNull(container, "container");

            _container = container;
            _install = install;
        }

        protected TStorageAdapter GetAdapter<TStorageAdapter>()
            where TStorageAdapter : class, IStorageAdapter
        {
            if (_adapter == null)
            {
                _adapter = (TStorageAdapter)_container.Resolve<TStorageAdapter>();
                _adapter.Install = _install;
                _adapter.Initialize();
            }

            return (TStorageAdapter)_adapter;
        }

        public void Dispose()
        {
            if (_adapter != null)
            {
                _adapter.Dispose();
                _adapter = null;
            }

            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
