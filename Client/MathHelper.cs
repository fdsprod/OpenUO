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

namespace Client
{
    public static class MathHelper
    {
        public const float E = 2.71828175f;
        public const float Log2E = 1.442695f;
        public const float Log10E = 0.4342945f;
        public const float Pi = 3.14159274f;
        public const float TwoPi = 6.28318548f;
        public const float PiOver2 = 1.57079637f;
        public const float PiOver4 = 0.7853982f;

        public static float ToRadians(float degrees)
        {
            return degrees * 0.0174532924f;
        }
        public static float ToDegrees(float radians)
        {
            return radians * 57.2957764f;
        }

        public static float Distance(float value1, float value2)
        {
            return Math.Abs(value1 - value2);
        }

        public static float Min(float value1, float value2)
        {
            return Math.Min(value1, value2);
        }

        public static float Max(float value1, float value2)
        {
            return Math.Max(value1, value2);
        }

        public static float Clamp(float value, float min, float max)
        {
            value = ((value > max) ? max : value);
            value = ((value < min) ? min : value);

            return value;
        }

        public static float Lerp(float value1, float value2, float amount)
        {
            return value1 + (value2 - value1) * amount;
        }

        public static float WrapAngle(float angle)
        {
            angle = (float)Math.IEEERemainder((double)angle, 6.2831854820251465);

            if (angle <= -3.14159274f)
                angle += 6.28318548f;
            else
            {
                if (angle > 3.14159274f)
                    angle -= 6.28318548f;
            }

            return angle;
        }
    }
}
