#region License Header
/***************************************************************************
 *   Copyright (c) 2011 OpenUO Software Team.
 *   All Right Reserved.
 *
 *   $Id: Asserter.cs 14 2011-10-31 07:03:12Z fdsprod@gmail.com $:
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 ***************************************************************************/
 #endregion

using System;
using System.IO;

namespace OpenUO.Core
{
    public static class Guard
    {
        private const string ErrorInvalidRange = "Cannot perform range assertion on ({0}) because minimum value {1} is greater than maximum value {2}.";

        public static void Assert(bool logicalExpression, string message)
        {
            if (!logicalExpression)
                throw new ArgumentException(message);
        }

        public static void Assert(string parameterName, bool logicalExpression, object actualValue, string message)
        {
            if (!logicalExpression)
            {
                if (actualValue == null)
                    throw (new ArgumentNullException(parameterName, message));

                throw (new ArgumentOutOfRangeException(parameterName, message));
            }
        }

        public static void AssertIsNotNull(object obj, string message)
        {
            if (obj == null)
                throw new ArgumentNullException(message);
        }

        public static void AssertIsNotNullOrEmpty(string value, string paramName)
        {
#if Framework_4_0
            if (string.IsNullOrWhiteSpace(value))
#else
            if (string.IsNullOrEmpty(value))
#endif
                throw new ArgumentException(string.Format("'{0}' cannot be a not be null or empty string.", paramName));
        }

        public static void AssertIsNotLessThan(string parameterName, int minimumValue, int actualValue)
        {
            if (actualValue < minimumValue)
                throw new ArgumentOutOfRangeException(parameterName);
        }

        public static void AssertIsNotLessThan(string parameterName, long minimumValue, long actualValue)
        {
            if (actualValue < minimumValue)
                throw new ArgumentOutOfRangeException(parameterName);
        }

        public static void AssertIsNotLessThan(string parameterName, decimal minimumValue, decimal actualValue)
        {
            if (actualValue < minimumValue)
                throw new ArgumentOutOfRangeException(parameterName);
        }

        public static void AssertIsNotLessThan(string parameterName, float minimumValue, float actualValue)
        {
            if (actualValue < minimumValue)
                throw new ArgumentOutOfRangeException(parameterName);
        }

        public static void AssertIsNotLessThan(string parameterName, TimeSpan minimumValue, TimeSpan actualValue)
        {
            if (actualValue < minimumValue)
                throw new ArgumentOutOfRangeException(parameterName);
        }

        public static void AssertIsNotLessThan(string parameterName, double minimumValue, double actualValue)
        {
            if (actualValue < minimumValue)
                throw new ArgumentOutOfRangeException(parameterName);
        }

        public static void AssertIsNotGreaterThan(string parameterName, int maximumValue, int actualValue)
        {
            if (actualValue > maximumValue)
                throw new ArgumentOutOfRangeException(parameterName);
        }

        public static void AssertIsNotGreaterThan(string parameterName, long maximumValue, long actualValue)
        {
            if (actualValue > maximumValue)
                throw new ArgumentOutOfRangeException(parameterName);
        }

        public static void AssertIsNotGreaterThan(string parameterName, decimal maximumValue, decimal actualValue)
        {
            if (actualValue > maximumValue)
                throw new ArgumentOutOfRangeException(parameterName);
        }

        public static void AssertIsNotGreaterThan(string parameterName, float maximumValue, float actualValue)
        {
            if (actualValue > maximumValue)
                throw new ArgumentOutOfRangeException(parameterName);
        }

        public static void AssertIsNotGreaterThan(string parameterName, double maximumValue, double actualValue)
        {
            if (actualValue > maximumValue)
                throw new ArgumentOutOfRangeException(parameterName);
        }

        public static void AssertInRange(string parameterName, int minimumValue, int actualValue)
        {
            if (actualValue < minimumValue)
                throw new ArgumentOutOfRangeException(parameterName);
        }

        public static void AssertInRange(string parameterName, decimal minimumValue, decimal actualValue)
        {
            if (actualValue < minimumValue)
                throw new ArgumentOutOfRangeException(parameterName);
        }

        public static void AssertInRange(string parameterName, byte minimumValue, byte maximumValue, byte actualValue)
        {
            if (minimumValue > maximumValue)
            {
                string message = String.Format(ErrorInvalidRange, parameterName, minimumValue, maximumValue);
                throw new InvalidOperationException(message);
            }

            if (actualValue < minimumValue || actualValue > maximumValue)
                throw new ArgumentOutOfRangeException(parameterName);
        }

        public static void AssertInRange(string parameterName, short minimumValue, short maximumValue, short actualValue)
        {
            if (minimumValue > maximumValue)
            {
                string message = String.Format(ErrorInvalidRange, parameterName, minimumValue, maximumValue);
                throw new InvalidOperationException(message);
            }

            if (actualValue < minimumValue || actualValue > maximumValue)
                throw new ArgumentOutOfRangeException(parameterName);
        }

        public static void AssertInRange(string parameterName, int minimumValue, int? maximumValue, int actualValue)
        {
            if (minimumValue > maximumValue)
            {
                string message = String.Format(ErrorInvalidRange, parameterName, minimumValue, maximumValue);
                throw new InvalidOperationException(message);
            }

            if (maximumValue.HasValue && (actualValue < minimumValue || actualValue > maximumValue))
                throw new ArgumentOutOfRangeException(parameterName);

            if (actualValue < minimumValue)
                throw new ArgumentOutOfRangeException(parameterName);
        }

        public static void AssertInRange(string parameterName, long minimumValue, long maximumValue, long actualValue)
        {
            if (minimumValue > maximumValue)
            {
                string message = String.Format(ErrorInvalidRange, parameterName, minimumValue, maximumValue);
                throw new InvalidOperationException(message);
            }

            if (actualValue < minimumValue || actualValue > maximumValue)
                throw new ArgumentOutOfRangeException(parameterName);
        }

        public static void AssertInRange(string parameterName, decimal minimumValue, decimal maximumValue, decimal actualValue)
        {
            if (minimumValue > maximumValue)
            {
                string message = String.Format(ErrorInvalidRange, parameterName, minimumValue, maximumValue);
                throw new InvalidOperationException(message);
            }

            if (actualValue < minimumValue || actualValue > maximumValue)
                throw new ArgumentOutOfRangeException(parameterName);
        }

        public static void AssertInRange(string parameterName, double minimumValue, double maximumValue, double actualValue)
        {
            if (minimumValue > maximumValue)
            {
                string message = String.Format(ErrorInvalidRange, parameterName, minimumValue, maximumValue);
                throw new InvalidOperationException(message);
            }

            if (actualValue < minimumValue || actualValue > maximumValue)
                throw new ArgumentOutOfRangeException(parameterName);
        }

        public static void AssertInRange(string parameterName, float minimumValue, float maximumValue, float actualValue)
        {
            if (minimumValue > maximumValue)
            {
                string message = String.Format(ErrorInvalidRange, parameterName, minimumValue, maximumValue);
                throw new InvalidOperationException(message);
            }

            if (actualValue < minimumValue || actualValue > maximumValue)
                throw new ArgumentOutOfRangeException(parameterName);
        }

        public static void AssertInRange(string parameterName, DateTime minimumValue, DateTime maximumValue, DateTime actualValue)
        {
            if (minimumValue > maximumValue)
            {
                string message = String.Format(ErrorInvalidRange, parameterName, minimumValue, maximumValue);
                throw new InvalidOperationException(message);
            }

            if (actualValue < minimumValue || actualValue > maximumValue)
                throw new ArgumentOutOfRangeException(parameterName);
        }

        public static void AssertIsNull(object obj, string message)
        {
            if (obj != null)
                throw new Exception(message);
        }

        public static void AssertAreEqual(object a, object b, string message)
        {
            if (a != b)
                throw new Exception(message);
        }

        public static void AssertFileExists(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException(path);
        }

        public static void AssertDirectoryExists(string directory)
        {
            if (!Directory.Exists(directory))
                throw new DirectoryNotFoundException(directory);
        }
    }
}