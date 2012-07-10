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

namespace OpenUO.Ultima.Windows.Forms.Controls
{
    public class ComboBoxItem<T>
    {
        public string Display { get; private set; }
        public T Value { get; private set; }

        public ComboBoxItem(string display, T value)
        {
            Display = display;
            Value = value;
        }
    }
}
