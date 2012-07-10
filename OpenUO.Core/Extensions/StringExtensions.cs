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

namespace OpenUO.Core
{
    public static class StringExtensions
    {
        public static string Remove(this string str, string phrase)
        {
            int index = -1;
            while ((index = str.IndexOf(phrase)) != -1)
            {
                str = str.Remove(index, phrase.Length);
            }

            return str;
        }
    }
}
