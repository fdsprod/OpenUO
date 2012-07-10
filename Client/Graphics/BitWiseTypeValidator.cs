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
using SharpDX.Direct3D9;

namespace Client.Graphics
{
    static class BitWiseTypeValidator
    {
        public static void Validate<T>() where T : new()
        {
            object instance = new T();
            System.Reflection.PropertyInfo[] props = instance.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            object[] defaults = new object[props.Length];
            for (int i = 0; i < props.Length; i++)
            {
                if (props[i].CanRead && props[i].CanWrite)
                    defaults[i] = props[i].GetValue(instance, null);
            }


            for (int i = 0; i < props.Length; i++)
            {
                if (!props[i].CanRead || !props[i].CanWrite)
                    continue;

                System.Collections.ArrayList values = new System.Collections.ArrayList();
                if (typeof(Enum).IsAssignableFrom(props[i].PropertyType))
                {
                    System.Reflection.FieldInfo[] enums = props[i].PropertyType.GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                    foreach (System.Reflection.FieldInfo field in enums)
                    {
                        values.Add(field.GetValue(null));
                    }
                }
                else
                {
                    if (typeof(bool) == props[i].PropertyType)
                    {
                        values.AddRange(new object[] { true, false });
                    }
                    else
                    {
                        if (typeof(byte) == props[i].PropertyType)
                        {
                            for (int b = 0; b < 256; b++)
                                values.Add((byte)b);
                        }
                        else
                        {
                            if (typeof(int) == props[i].PropertyType)
                                continue;
                            throw new ArgumentException();
                        }
                    }
                }

                foreach (object value in values)
                {
                    props[i].SetValue(instance, value, null);

                    if (value.Equals(TextureFilter.PyramidalQuad) ||
                        value.Equals(TextureFilter.GaussianQuad) ||
                        (value.Equals(TextureFilter.Anisotropic) && props[i].Name == "MagFilter") ||
                        (value.Equals(TextureFilter.Anisotropic) && props[i].Name == "MipFilter"))
                        continue;//special cases :-)

                    if (props[i].GetValue(instance, null).Equals(value) == false)
                        throw new ArgumentException();

                    for (int p = 0; p < props.Length; p++)
                    {
                        if (!props[p].CanRead || !props[p].CanWrite)
                            continue;
                        if (p != i)
                        {
                            if (props[p].GetValue(instance, null).Equals(defaults[p]) == false)
                                throw new ArgumentException();
                        }
                    }
                }

                props[i].SetValue(instance, defaults[i], null);
            }
        }
    }
}
