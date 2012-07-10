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
    public static class SingleExtensions
    {
        public static float Epsilon = 0.0001f;

        public static bool NearlyZero(this float val)
        {
            return val < float.Epsilon &&
                   val > -float.Epsilon;
        }

        public static float Cos(this float angle)
        {
            return (float)Math.Cos(angle);
        }

        public static float CosABS(this float angle)
        {
            return (float)Math.Abs(Math.Cos(angle));
        }

        public static float Sin(this float angle)
        {
            return (float)Math.Sin(angle);
        }
    }
}
