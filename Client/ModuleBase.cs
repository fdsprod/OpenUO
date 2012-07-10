#region License Header
/***************************************************************************
 *   Copyright (c) 2011 OpenUO Software Team.
 *   All Right Reserved.
 *
 *   $Id: ModuleBase.cs 14 2011-10-31 07:03:12Z fdsprod@gmail.com $:
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 ***************************************************************************/
 #endregion

using OpenUO.Core.Patterns;

namespace Client
{
    public abstract class ModuleBase : IModule
    {
        private IoCContainer _container;

        public abstract string Name { get; }

        public IoCContainer Container
        {
            get { return _container; }
        }

        public void OnLoad(IoCContainer container)
        {
            _container = container;
            Initialize();
        }

        protected virtual void Initialize()
        {

        }

        public virtual void OnUnload(IoCContainer container)
        {

        }
    }
}
