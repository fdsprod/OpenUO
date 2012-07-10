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

namespace OpenUO.Core
{
    public static class ArrayExtensions
    {
        public static void Fill<T>(this T[] array) where T : new()
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new T();
            }
        }

        public static void Fill<T>(this T[,] array) where T : new()
        {
            for (int x = 0; x < array.GetLength(0); x++)
            {
                for (int y = 0; y < array.GetLength(1); y++)
                {
                    array[x, y] = new T();
                }
            }
        }
    }
}
