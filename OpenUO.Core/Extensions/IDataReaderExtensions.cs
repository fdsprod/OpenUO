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

using System.Data;

namespace OpenUO.Core
{
    public static class IDataReaderExtensions
    {
        public static T GetData<T>(this IDataReader reader, int index) where T : struct
        {
            return reader.GetData<T>(index, default(T));
        }

        public static T GetData<T>(this IDataReader reader, string columnName) where T : struct
        {
            return reader.GetData<T>(reader.GetOrdinal(columnName), default(T));
        }

        public static T GetData<T>(this IDataReader reader, int index, T defaultValue) where T : struct
        {
            if (reader.IsDBNull(index))
                return defaultValue;

            return (T)reader[index];
        }

        public static T GetData<T>(this IDataReader reader, string columnName, T defaultValue) where T : struct
        {
            return reader.GetData<T>(reader.GetOrdinal(columnName), default(T));
        }
    }
}
