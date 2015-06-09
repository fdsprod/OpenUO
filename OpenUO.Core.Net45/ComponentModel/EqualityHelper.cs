#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// EqualityHelper.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

namespace OpenUO.Core.ComponentModel
{
    public static class EqualityHelper
    {
        public static bool IsEqual<T>(T oldValue, T newValue)
        {
            return typeof (T).IsValueType() ? oldValue.Equals(newValue) : Equals(oldValue, newValue);
        }
    }
}