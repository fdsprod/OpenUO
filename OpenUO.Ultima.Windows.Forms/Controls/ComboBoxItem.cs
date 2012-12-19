#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   ComboBoxItem.cs
//  *
//  *   This program is free software; you can redistribute it and/or modify
//  *   it under the terms of the GNU General Public License as published by
//  *   the Free Software Foundation; either version 3 of the License, or
//  *   (at your option) any later version.
//  ***************************************************************************/

#endregion

namespace OpenUO.Ultima.Windows.Forms.Controls
{
    public class ComboBoxItem<T>
    {
        public ComboBoxItem(string display, T value)
        {
            Display = display;
            Value = value;
        }

        public string Display
        {
            get;
            private set;
        }

        public T Value
        {
            get;
            private set;
        }
    }
}