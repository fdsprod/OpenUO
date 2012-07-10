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

using System;

namespace OpenUO.Core
{
    public static class ObjectExtensions
    {
        public static object ConvertTo(this object obj, Type convertTo)
        {
            if (obj == null || obj == DBNull.Value)
            {
                if (convertTo.IsNullableType() || convertTo.IsClass)
                    return null;

                throw new InvalidCastException(string.Format("Unable to cast null to {1}'.", obj.GetType(), convertTo));
            }

            object converted = null;

            if (!ObjectConverterter.TryConvert(obj.GetType(), obj, convertTo, out converted))
            {
                throw new InvalidCastException(string.Format("Unable to cast '{0}' to {1}'.", obj.GetType(), convertTo));
            }

            return converted;
        }

        public static bool TryConvertTo(this object obj, Type convertTo, out object outValue)
        {
            outValue = null;

            if (obj == null)
            {
                return convertTo.IsNullableType();
            }

            object converted;

            if (!ObjectConverterter.TryConvert(obj.GetType(), obj, convertTo, out converted))
            {
                return false;
            }

            outValue = converted;
            return true;
        }

        public static T ConvertTo<T>(this object obj)
        {
            return (T)ConvertTo(obj, typeof(T));
        }

        public static bool TryConvertTo<T>(this object obj, out T outValue)
        {
            object o;
            bool success = TryConvertTo(obj, typeof(T), out o);
            outValue = (T)o;

            return success;
        }
    }
}
