#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// SettingsSectionBase.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using OpenUO.Core.ComponentModel;
using OpenUO.Core.Diagnostics;

#endregion

namespace OpenUO.Core.Configuration
{
    public abstract class SettingsSectionBase : NotifiableBase
    {
        public event EventHandler Invalidated;

        internal void OnDeserialized()
        {
            UpdateVersionValues();
        }

        protected virtual void UpdateVersionValues()
        {
        }

        protected override void SetPropertyOverride<T>(ref T storage, object value, string propertyName)
        {
            base.SetPropertyOverride(ref storage, value, propertyName);

            var notifier = storage as INotifyPropertyChanged;

            if (notifier != null)
            {
                // Stop listening to the old value since it is no longer part of
                // the settings section.
                notifier.PropertyChanged -= onSectionPropertyChanged;
            }

            notifier = value as INotifyPropertyChanged;

            if (notifier != null)
            {
                // Start listening to the new value 
                notifier.PropertyChanged += onSectionPropertyChanged;
            }
        }

        private void onSectionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            onInvalidated();
        }

        private void onInvalidated()
        {
            var handler = Invalidated;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}