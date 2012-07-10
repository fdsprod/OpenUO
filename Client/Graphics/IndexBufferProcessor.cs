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

namespace Client.Graphics
{
    static class IndexBufferProcessor
    {
        internal delegate void ProcessMinMax(object o_values, int count, object o_ib);

        internal static ProcessMinMax Method<T>()
        {
            if (typeof(T) == typeof(int))
                return ProcessMinMaxInt;
            if (typeof(T) == typeof(uint))
                return ProcessMinMaxUInt;
            if (typeof(T) == typeof(short))
                return ProcessMinMaxShort;
            if (typeof(T) == typeof(ushort))
                return ProcessMinMaxUShort;

            throw new ArgumentException();
        }

        public static void Init(object item)
        {
            if (item is Indices<int>.Implementation)
            {
                (item as Indices<int>.Implementation).min = int.MaxValue;
                return;
            }
            if (item is Indices<uint>.Implementation)
            {
                (item as Indices<uint>.Implementation).min = uint.MaxValue;
                return;
            }
            if (item is Indices<short>.Implementation)
            {
                (item as Indices<short>.Implementation).min = short.MaxValue;
                return;
            }
            if (item is Indices<ushort>.Implementation)
            {
                (item as Indices<ushort>.Implementation).min = ushort.MaxValue;
                return;
            }
            throw new ArgumentException();
        }

        public static void Update(object item, out int min, out int max)
        {
            if (item is Indices<int>.Implementation)
            {
                min = (int)(item as Indices<int>.Implementation).min;
                max = (int)(item as Indices<int>.Implementation).max;
                return;
            }
            if (item is Indices<uint>.Implementation)
            {
                min = (int)(item as Indices<uint>.Implementation).min;
                max = (int)(item as Indices<uint>.Implementation).max;
                return;
            }
            if (item is Indices<short>.Implementation)
            {
                min = (int)(item as Indices<short>.Implementation).min;
                max = (int)(item as Indices<short>.Implementation).max;
                return;
            }
            if (item is Indices<ushort>.Implementation)
            {
                min = (int)(item as Indices<ushort>.Implementation).min;
                max = (int)(item as Indices<ushort>.Implementation).max;
                return;
            }
            throw new ArgumentException();
        }

        static void ProcessMinMaxInt(object o_values, int count, object o_ib)
        {
            int[] values = (int[])o_values;
            Indices<int>.Implementation ib = (Indices<int>.Implementation)o_ib;
            if (ib.max == ib.min && ib.min == 0 && count != 0)
                ib.min = int.MaxValue;
            for (int i = 0; i < count; i++)
            {
                ib.min = Math.Min(values[i], ib.min);
                ib.max = Math.Max(values[i], ib.max);
            }
        }
        static void ProcessMinMaxUInt(object o_values, int count, object o_ib)
        {
            uint[] values = (uint[])o_values;
            Indices<uint>.Implementation ib = (Indices<uint>.Implementation)o_ib;
            if (ib.max == ib.min && ib.min == 0 && count != 0)
                ib.min = uint.MaxValue;
            for (int i = 0; i < count; i++)
            {
                ib.min = Math.Min(values[i], ib.min);
                ib.max = Math.Max(values[i], ib.max);
            }
        }
        static void ProcessMinMaxShort(object o_values, int count, object o_ib)
        {
            short[] values = (short[])o_values;
            Indices<short>.Implementation ib = (Indices<short>.Implementation)o_ib;
            if (ib.max == ib.min && ib.min == 0 && count != 0)
                ib.min = short.MaxValue;
            for (int i = 0; i < count; i++)
            {
                ib.min = Math.Min(values[i], ib.min);
                ib.max = Math.Max(values[i], ib.max);
            }
        }
        static void ProcessMinMaxUShort(object o_values, int count, object o_ib)
        {
            ushort[] values = (ushort[])o_values;
            Indices<ushort>.Implementation ib = (Indices<ushort>.Implementation)o_ib;
            if (ib.max == ib.min && ib.min == 0 && count != 0)
                ib.min = ushort.MaxValue;
            for (int i = 0; i < count; i++)
            {
                ib.min = Math.Min(values[i], ib.min);
                ib.max = Math.Max(values[i], ib.max);
            }
        }
    }
}
