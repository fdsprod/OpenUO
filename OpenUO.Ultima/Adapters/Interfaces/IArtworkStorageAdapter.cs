#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   IArtworkStorageAdapter.cs
//  *
//  *   This program is free software; you can redistribute it and/or modify
//  *   it under the terms of the GNU General Public License as published by
//  *   the Free Software Foundation; either version 3 of the License, or
//  *   (at your option) any later version.
//  ***************************************************************************/

#endregion

using System.Threading.Tasks;

namespace OpenUO.Ultima.Adapters
{
    public interface IArtworkStorageAdapter<T> : IStorageAdapter
    {
        T GetLand(int index);
        Task<T> GetLandAsync(int index);
        T GetStatic(int index);
        Task<T> GetStaticAsync(int index);
    }
}