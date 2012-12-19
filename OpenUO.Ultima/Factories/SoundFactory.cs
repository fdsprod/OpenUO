#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   SoundFactory.cs
//  *
//  *   This program is free software; you can redistribute it and/or modify
//  *   it under the terms of the GNU General Public License as published by
//  *   the Free Software Foundation; either version 3 of the License, or
//  *   (at your option) any later version.
//  ***************************************************************************/

#endregion

#region Usings

using OpenUO.Core.Patterns;
using OpenUO.Ultima.Adapters;

#endregion

namespace OpenUO.Ultima
{
    public class SoundFactory : AdapterFactoryBase
    {
        public SoundFactory(InstallLocation install, IContainer container)
            : base(install, container)
        {
        }

        public T GetSound<T>(int index)
        {
            return GetAdapter<ISoundStorageAdapter<T>>().GetSound(index);
        }

        public int GetLength<T>()
        {
            return GetAdapter<ISoundStorageAdapter<T>>().Length;
        }
    }
}