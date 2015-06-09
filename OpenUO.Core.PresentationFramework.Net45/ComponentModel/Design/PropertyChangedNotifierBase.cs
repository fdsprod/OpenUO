#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// PropertyChangedNotifierBase.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region Usings

using System;
using System.Linq.Expressions;

using OpenUO.Core.ComponentModel;

#endregion

namespace OpenUO.Core.PresentationOpenUO.Core.ComponentModel.Design
{
    public abstract class PropertyChangedNotifierBase : NotifiableBase
    {
        protected virtual bool SetProperty<T>(ref T storage, T value, Expression<Func<T>> propertyExpression)
        {
            var propertyName = PropertySupport.ExtractPropertyName(propertyExpression);
            return SetProperty(ref storage, value, propertyName);
        }

        protected void RaisePropertyChanged(params Expression<Func<object>>[] propertyExpressions)
        {
            for (var i = 0; i < propertyExpressions.Length; i++)
            {
                var propertyExpression = propertyExpressions[i];
                OnPropertyChanged(PropertySupport.ExtractPropertyName(propertyExpression));
            }
        }

        protected void RaisePropertyChanged(params string[] propertyNames)
        {
            for (var i = 0; i < propertyNames.Length; i++)
            {
                OnPropertyChanged(propertyNames[i]);
            }
        }
    }
}