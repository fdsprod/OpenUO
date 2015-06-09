#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   GumpFactory.cs
//  *
//  *   This program is free software; you can redistribute it and/or modify
//  *   it under the terms of the GNU General Public License as published by
//  *   the Free Software Foundation; either version 3 of the License, or
//  *   (at your option) any later version.
//  ***************************************************************************/

#endregion

#region Usings

using System.Threading.Tasks;

using OpenUO.Core.Patterns;
using OpenUO.Ultima.Adapters;

#endregion

namespace OpenUO.Ultima
{
    public class GumpFactory : AdapterFactoryBase
    {
        public GumpFactory(InstallLocation install, IContainer container)
            : base(install, container)
        {
        }

        public T GetGump<T>(int index)
        {
            return GetAdapter<IGumpStorageAdapter<T>>().GetGump(index);
        }

        public int GetLength<T>()
        {
            return GetAdapter<IGumpStorageAdapter<T>>().Length;
        }

        public Task<T> GetGumpAsync<T>(int index)
        {
            return Task.Run(async () =>
            {
                var adapter = await GetAdapterAsync<IGumpStorageAdapter<T>>().ConfigureAwait(false);

                return await adapter.GetGumpAsync(index).ConfigureAwait(false);
            });
        }

        public Task<int> GetLengthAsync<T>()
        {
            return Task.Run(async () =>
            {
                var adapter = await GetAdapterAsync<IGumpStorageAdapter<T>>().ConfigureAwait(false);

                return adapter.Length;
            });
        }
    }
}