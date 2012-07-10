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


namespace OpenUO.Ultima
{
    public interface IAnimationFrame<T>
    {
    }

    public class Frame<T>
    {
        public int CenterX { get; private set; }
        public int CenterY { get; private set; }
        public T Image { get; private set; }

        public Frame(int centerX, int centerY, T image)
        {
            CenterX = centerX;
            CenterY = centerY;
            Image = image;
        }
    }
}
