#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// Resolution.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region Usings

using OpenUO.Core.ComponentModel;

#endregion

namespace OpenUO.Core.Configuration
{
    public class Resolution : NotifiableBase
    {
        private int _height;
        private int _width;

        public Resolution(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Height
        {
            get { return _height; }
            set { SetProperty(ref _height, value); }
        }

        public int Width
        {
            get { return _width; }
            set { SetProperty(ref _width, value); }
        }
    }
}