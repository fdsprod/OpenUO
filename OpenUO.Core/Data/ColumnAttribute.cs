#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   ColumnAttribute.cs
//  *
//  *   This program is free software; you can redistribute it and/or modify
//  *   it under the terms of the GNU General Public License as published by
//  *   the Free Software Foundation; either version 3 of the License, or
//  *   (at your option) any later version.
//  ***************************************************************************/

#endregion

#region Usings

using System;

#endregion

namespace OpenUO.Core.Data
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ColumnAttribute : Attribute
    {
        private readonly string _name;

        public ColumnAttribute(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }
    }
}