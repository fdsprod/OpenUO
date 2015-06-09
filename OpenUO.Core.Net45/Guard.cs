#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// Guard.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region Usings

using System;
using System.IO;

using JetBrains.Annotations;

#endregion

namespace OpenUO.Core
{
    public static class Guard
    {
        private const string ErrorInvalidRange =
            "Cannot perform range assertion on ({0}) because minimum value {1} is greater than maximum value {2}.";

        [ContractAnnotation("halt <= condition: false")]
        public static void Require(bool condition, string message)
        {
            if (!condition)
            {
                throw new ArgumentException(message);
            }
        }

        [ContractAnnotation("halt <= condition: false")]
        public static void Require(string parameterName, bool condition, object actualValue, string message)
        {
            if (!condition)
            {
                if (actualValue == null)
                {
                    throw (new ArgumentNullException(parameterName, message));
                }

                throw (new ArgumentOutOfRangeException(parameterName, message));
            }
        }

        [ContractAnnotation("halt <= condition: false")]
        public static void Require<TException>(bool condition, string message = null)
            where TException : Exception
        {
            if (!condition)
            {
                if (string.IsNullOrEmpty(message))
                {
                    throw (TException)Activator.CreateInstance(typeof(TException));
                }

                throw (TException)Activator.CreateInstance(typeof(TException), message);
            }
        }

        [ContractAnnotation("halt <= obj: null")]
        public static void RequireIsNotNull(object obj, string message)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(message);
            }
        }

        public static void RequireIsNotNullOrEmpty(string value, string paramName)
        {
#if Framework_4_0
            if (string.IsNullOrWhiteSpace(value))
#else
            if (string.IsNullOrEmpty(value))
            {
#endif
                throw new ArgumentException(string.Format("'{0}' cannot be a not be null or empty string.", paramName));
            }
        }

        public static void RequireIsNotLessThan(string parameterName, int minimumValue, int actualValue)
        {
            if (actualValue < minimumValue)
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }
        }

        public static void RequireIsNotLessThan(string parameterName, long minimumValue, long actualValue)
        {
            if (actualValue < minimumValue)
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }
        }

        public static void RequireIsNotLessThan(string parameterName, decimal minimumValue, decimal actualValue)
        {
            if (actualValue < minimumValue)
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }
        }

        public static void RequireIsNotLessThan(string parameterName, float minimumValue, float actualValue)
        {
            if (actualValue < minimumValue)
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }
        }

        public static void RequireIsNotLessThan(string parameterName, TimeSpan minimumValue, TimeSpan actualValue)
        {
            if (actualValue < minimumValue)
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }
        }

        public static void RequireIsNotLessThan(string parameterName, double minimumValue, double actualValue)
        {
            if (actualValue < minimumValue)
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }
        }

        public static void RequireIsNotGreaterThan(string parameterName, int maximumValue, int actualValue)
        {
            if (actualValue > maximumValue)
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }
        }

        public static void RequireIsNotGreaterThan(string parameterName, long maximumValue, long actualValue)
        {
            if (actualValue > maximumValue)
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }
        }

        public static void RequireIsNotGreaterThan(string parameterName, decimal maximumValue, decimal actualValue)
        {
            if (actualValue > maximumValue)
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }
        }

        public static void RequireIsNotGreaterThan(string parameterName, float maximumValue, float actualValue)
        {
            if (actualValue > maximumValue)
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }
        }

        public static void RequireIsNotGreaterThan(string parameterName, double maximumValue, double actualValue)
        {
            if (actualValue > maximumValue)
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }
        }

        public static void RequireInRange(string parameterName, int minimumValue, int actualValue)
        {
            if (actualValue < minimumValue)
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }
        }

        public static void RequireInRange(string parameterName, decimal minimumValue, decimal actualValue)
        {
            if (actualValue < minimumValue)
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }
        }

        public static void RequireInRange(string parameterName, byte minimumValue, byte maximumValue, byte actualValue)
        {
            if (minimumValue > maximumValue)
            {
                var message = String.Format(ErrorInvalidRange, parameterName, minimumValue, maximumValue);
                throw new InvalidOperationException(message);
            }

            if (actualValue < minimumValue || actualValue > maximumValue)
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }
        }

        public static void RequireInRange(string parameterName, short minimumValue, short maximumValue, short actualValue)
        {
            if (minimumValue > maximumValue)
            {
                var message = String.Format(ErrorInvalidRange, parameterName, minimumValue, maximumValue);
                throw new InvalidOperationException(message);
            }

            if (actualValue < minimumValue || actualValue > maximumValue)
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }
        }

        public static void RequireInRange(string parameterName, int minimumValue, int? maximumValue, int actualValue)
        {
            if (minimumValue > maximumValue)
            {
                var message = String.Format(ErrorInvalidRange, parameterName, minimumValue, maximumValue);
                throw new InvalidOperationException(message);
            }

            if (maximumValue.HasValue && (actualValue < minimumValue || actualValue > maximumValue))
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }

            if (actualValue < minimumValue)
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }
        }

        public static void RequireInRange(string parameterName, long minimumValue, long maximumValue, long actualValue)
        {
            if (minimumValue > maximumValue)
            {
                var message = String.Format(ErrorInvalidRange, parameterName, minimumValue, maximumValue);
                throw new InvalidOperationException(message);
            }

            if (actualValue < minimumValue || actualValue > maximumValue)
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }
        }

        public static void RequireInRange(string parameterName, decimal minimumValue, decimal maximumValue, decimal actualValue)
        {
            if (minimumValue > maximumValue)
            {
                var message = String.Format(ErrorInvalidRange, parameterName, minimumValue, maximumValue);
                throw new InvalidOperationException(message);
            }

            if (actualValue < minimumValue || actualValue > maximumValue)
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }
        }

        public static void RequireInRange(string parameterName, double minimumValue, double maximumValue, double actualValue)
        {
            if (minimumValue > maximumValue)
            {
                var message = String.Format(ErrorInvalidRange, parameterName, minimumValue, maximumValue);
                throw new InvalidOperationException(message);
            }

            if (actualValue < minimumValue || actualValue > maximumValue)
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }
        }

        public static void RequireInRange(string parameterName, float minimumValue, float maximumValue, float actualValue)
        {
            if (minimumValue > maximumValue)
            {
                var message = String.Format(ErrorInvalidRange, parameterName, minimumValue, maximumValue);
                throw new InvalidOperationException(message);
            }

            if (actualValue < minimumValue || actualValue > maximumValue)
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }
        }

        public static void RequireInRange(string parameterName, DateTime minimumValue, DateTime maximumValue, DateTime actualValue)
        {
            if (minimumValue > maximumValue)
            {
                var message = String.Format(ErrorInvalidRange, parameterName, minimumValue, maximumValue);
                throw new InvalidOperationException(message);
            }

            if (actualValue < minimumValue || actualValue > maximumValue)
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }
        }

        public static void RequireIsNull(object obj, string message)
        {
            if (obj != null)
            {
                throw new Exception(message);
            }
        }

        public static void RequireAreEqual(object a, object b, string message)
        {
            if (a != b)
            {
                throw new Exception(message);
            }
        }

        public static void RequireFileExists(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }
        }

        public static void RequireDirectoryExists(string directory)
        {
            if (!Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException(directory);
            }
        }
    }
}