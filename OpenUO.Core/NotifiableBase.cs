#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   TrackableBase.cs
//  *
//  *   This program is free software; you can redistribute it and/or modify
//  *   it under the terms of the GNU General Public License as published by
//  *   the Free Software Foundation; either version 3 of the License, or
//  *   (at your option) any later version.
//  ***************************************************************************/

#endregion

#region Usings

using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#endregion

namespace OpenUO.Core
{
    public abstract class NotifiableBase : IDisposable, INotifyPropertyChanged
    {
        public void Dispose()
        {
            Dispose(true);
        }

#if NET_45
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
#else
        protected virtual bool SetProperty<T>(ref T storage, T value, string propertyName = null)
#endif
        {
            if (Equals(storage, value))
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);

            return true;
        }

#if NET_45
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
#else
        protected virtual void OnPropertyChanged(string propertyName = null)
#endif
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}