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


namespace OpenUO.Ultima.Adapters
{
    public abstract class StorageAdapterBase : IStorageAdapter
    {
        public InstallLocation Install
        {
            get;
            set;
        }

        public virtual void Initialize() { }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {

        }
    }
}