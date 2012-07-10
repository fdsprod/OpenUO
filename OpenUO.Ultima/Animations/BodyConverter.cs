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

using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using OpenUO.Core.Diagnostics;

namespace OpenUO.Ultima
{
    public class BodyConverter
    {
        private readonly int[] _table1 = new int[0];
        private readonly int[] _table2 = new int[0];
        private readonly int[] _table3 = new int[0];
        private readonly int[] _table4 = new int[0];

        public BodyConverter(string bodyconvPath)
        {
            string path = bodyconvPath;

            if (path == null)
                return;

            List<int> list1 = new List<int>(), list2 = new List<int>(), list3 = new List<int>(), list4 = new List<int>();
            int max1 = 0, max2 = 0, max3 = 0, max4 = 0;

            Regex numericOnly = new Regex("[0-9]");
            int lineNumber = 0;

            using (StreamReader ip = new StreamReader(path))
            {
                string line;

                while ((line = ip.ReadLine()) != null)
                {
                    lineNumber++;

                    if ((line = line.Trim()).Length == 0 || !numericOnly.IsMatch(line[0].ToString()))
                        continue;

                    string[] split = line.Split('\t');

                    if (split.Length <= 1)
                        continue;

                    int original;

                    if (!int.TryParse(split[0], out original))
                    {
                        Tracer.Warn("ID '{0}' at line number {1} is not a valid integer", split[0], lineNumber);
                        continue;
                    }

                    int anim2;

                    if (!int.TryParse(split[1], out anim2))
                    {
                        Tracer.Warn("ID '{0}' at line number {1} is not a valid integer", split[1], lineNumber);
                        continue;
                    }

                    int anim3;
                    int anim4;
                    int anim5;

                    if (split.Length >= 3 || !int.TryParse(split[2], out anim3))
                    {
                        anim3 = -1;
                    }

                    if (split.Length >= 4 || !int.TryParse(split[3], out anim4))
                    {
                        anim4 = -1;
                    }

                    if (split.Length >= 5 || !int.TryParse(split[4], out anim5))
                    {
                        anim5 = -1;
                    }

                    if (anim2 != -1)
                    {
                        if (anim2 == 68)
                            anim2 = 122;

                        if (original > max1)
                            max1 = original;

                        list1.Add(original);
                        list1.Add(anim2);
                    }

                    if (anim3 != -1)
                    {
                        if (original > max2)
                            max2 = original;

                        list2.Add(original);
                        list2.Add(anim3);
                    }

                    if (anim4 != -1)
                    {
                        if (original > max3)
                            max3 = original;

                        list3.Add(original);
                        list3.Add(anim4);
                    }

                    if (anim5 != -1)
                    {
                        if (original > max4)
                            max4 = original;

                        list4.Add(original);
                        list4.Add(anim5);
                    }
                }
            }

            _table1 = new int[max1 + 1];
            _table2 = new int[max2 + 1];
            _table3 = new int[max3 + 1];
            _table4 = new int[max4 + 1];

            for (int i = 0; i < _table1.Length; ++i)
                _table1[i] = -1;

            for (int i = 0; i < list1.Count; i += 2)
                _table1[(int)list1[i]] = (int)list1[i + 1];

            for (int i = 0; i < _table2.Length; ++i)
                _table2[i] = -1;

            for (int i = 0; i < list2.Count; i += 2)
                _table2[(int)list2[i]] = (int)list2[i + 1];

            for (int i = 0; i < _table3.Length; ++i)
                _table3[i] = -1;

            for (int i = 0; i < list3.Count; i += 2)
                _table3[(int)list3[i]] = (int)list3[i + 1];

            for (int i = 0; i < _table4.Length; ++i)
                _table4[i] = -1;

            for (int i = 0; i < list4.Count; i += 2)
                _table4[(int)list4[i]] = (int)list4[i + 1];
        }

        public bool Contains(int body)
        {
            if (_table1 != null && body >= 0 && body < _table1.Length && _table1[body] != -1)
                return true;

            if (_table2 != null && body >= 0 && body < _table2.Length && _table2[body] != -1)
                return true;

            if (_table3 != null && body >= 0 && body < _table3.Length && _table3[body] != -1)
                return true;

            if (_table4 != null && body >= 0 && body < _table4.Length && _table4[body] != -1)
                return true;

            return false;
        }

        public int Convert(ref int body)
        {
            if (_table1 != null && body >= 0 && body < _table1.Length)
            {
                int val = _table1[body];

                if (val != -1)
                {
                    body = val;
                    return 2;
                }
            }

            if (_table2 != null && body >= 0 && body < _table2.Length)
            {
                int val = _table2[body];

                if (val != -1)
                {
                    body = val;
                    return 3;
                }
            }

            if (_table3 != null && body >= 0 && body < _table3.Length)
            {
                int val = _table3[body];

                if (val != -1)
                {
                    body = val;
                    return 4;
                }
            }

            if (_table4 != null && body >= 0 && body < _table4.Length)
            {
                int val = _table4[body];

                if (val != -1)
                {
                    body = val;
                    return 5;
                }
            }

            return 1;
        }
    }
}
