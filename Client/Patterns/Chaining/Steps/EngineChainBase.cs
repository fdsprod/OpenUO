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

using OpenUO.Core.Patterns;

namespace Client.Patterns
{
    public abstract class EngineChainBase<T> : ExecutionChainBase<T>
        where T : class
    {
        private IoCContainer _container;

        protected EngineChainBase(IoCContainer container)
        {
            _container = container;
        }

        protected override TStep CreateStep<TStep>()
        {
            return _container.Resolve<TStep>();
        }
    }
}
