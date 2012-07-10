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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace OpenUO.Core
{
    public static class EnumExtensions
    {
        public static T GetAttribute<T>(this Enum en)
            where T : Attribute
        {
            Type type = en.GetType();

            MemberInfo[] memInfo = type.GetMember(en.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(T), false);

                foreach (object attr in attrs)
                {
                    if (attr is T)
                    {
                        return (T)attr;
                    }
                }

            }

            return default(T);
        }

        public static string GetDescription(this Enum en)
        {
            DescriptionAttribute descAttr = en.GetAttribute<DescriptionAttribute>();

            if (descAttr == null)
                return string.Empty;

            return descAttr.Description;
        }
    }
}
