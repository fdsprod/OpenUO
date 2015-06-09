#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// NotifiableBase.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region Usings

using System.ComponentModel;
using System.Runtime.CompilerServices;

using JetBrains.Annotations;

#endregion

namespace OpenUO.Core.ComponentModel
{
    public abstract class NotifiableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityHelper.IsEqual(storage, value))
            {
                return false;
            }

            SetPropertyOverride(ref storage, value, propertyName);

            storage = value;
            OnPropertyChanged(propertyName);

            return true;
        }

        protected virtual void SetPropertyOverride<T>(ref T storage, object value, string propertyName)
        {
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}