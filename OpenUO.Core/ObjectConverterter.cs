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
using System.ComponentModel;

namespace OpenUO.Core
{
    public static class ObjectConverterter
    {
        public static bool TryConvert<TConvertFrom, UConvertTo>(TConvertFrom convertFrom, out UConvertTo convertTo)
        {
            object to;
            bool converted = TryConvert(typeof(TConvertFrom), convertFrom, typeof(UConvertTo), out to);

            convertTo = (UConvertTo)to;

            return converted;
        }

        public static bool TryConvert(Type convertFrom, object from, Type convertTo, out object to)
        {
            to = null;
            bool converted = false;

            if(convertFrom == convertTo)
            {
                to = from;
                return true;
            }

            if (from != null && convertTo.IsEnum)
            {
                to = Enum.Parse(convertTo, from.ToString(), true);
                return true;
            }

            TypeConverter converter = TypeDescriptor.GetConverter(convertFrom);

            if (converter.CanConvertTo(convertTo))
            {
                to = converter.ConvertTo(from, convertTo);
                converted = true;
            }
            else
            {
                converter = TypeDescriptor.GetConverter(convertTo);

                if (converter.CanConvertFrom(convertFrom))
                {
                    to = converter.ConvertFrom(from);
                    converted = true;
                }
            }

            return converted;
        }
    }
}
