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

namespace OpenUO.Core
{
    public class ProgressEventArgs : EventArgs
    {
        public int PercentComplete { get; private set; }
        public int Length { get; private set; }
        public int Position { get; private set; }

        public ProgressEventArgs(int position, int length)
        {
            Guard.Assert(length > 0, "length must be greater than 0");

            Length = length;
            Position = position;
            PercentComplete = (int)(((float)position / length) * 100);
        }
    }
}
