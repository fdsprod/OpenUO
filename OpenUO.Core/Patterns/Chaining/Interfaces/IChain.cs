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


namespace OpenUO.Core.Patterns
{
    public interface IChain<T>
        where T : class
    {
        string Name { get; }

        void Execute(T state);

        IChain<T> RegisterStep<TStep>() where TStep : class, IChainStep<T>;
        IChain<T> RegisterStep<TStep>(TStep step) where TStep : class, IChainStep<T>;

        void Freeze();
    }
}
