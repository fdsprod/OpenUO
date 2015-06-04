#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   IDataRecordExtensions.cs
//  *
//  *   This program is free software; you can redistribute it and/or modify
//  *   it under the terms of the GNU General Public License as published by
//  *   the Free Software Foundation; either version 3 of the License, or
//  *   (at your option) any later version.
//  ***************************************************************************/

#endregion

#region Usings

using System.Collections.Generic;
using System.Data;
using OpenUO.Core.Data;
using OpenUO.Core.Reflection;

#endregion

namespace OpenUO.Core
{
    public static class IDataRecordExtensions
    {
        private static readonly Dictionary<string, Dictionary<string, PropertyAccessor>> _propertyAccessorsLookup =
            new Dictionary<string, Dictionary<string, PropertyAccessor>>();

        public static void MapTo<T>(this IDataRecord reader, T item)
        {
            var type = typeof(T);
            var properties = type.GetProperties();

            foreach(var property in properties)
            {
                var attributes = property.GetCustomAttributes(typeof(ColumnAttribute), true);

                if(attributes.Length > 0)
                {
                    Dictionary<string, PropertyAccessor> propertyAccessorTable;

                    if(!_propertyAccessorsLookup.TryGetValue(type.FullName, out propertyAccessorTable))
                    {
                        propertyAccessorTable = new Dictionary<string, PropertyAccessor>();
                        _propertyAccessorsLookup.Add(type.FullName, propertyAccessorTable);
                    }

                    PropertyAccessor propertyAccessor;

                    if(!propertyAccessorTable.TryGetValue(property.Name, out propertyAccessor))
                    {
                        propertyAccessor = new PropertyAccessor(typeof(T), property);
                        propertyAccessorTable.Add(property.Name, propertyAccessor);
                    }

                    var dataFieldAttr = (ColumnAttribute)attributes[0];

                    propertyAccessor.Set(item, reader[dataFieldAttr.Name].ConvertTo(property.PropertyType));
                }
            }
        }

        public static T CreateType<T>(this IDataRecord row)
            where T : new()
        {
            var item = new T();

            row.MapTo(item);

            return item;
        }
    }
}