#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// SecureStringExtensions.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region Usings

using System;
using System.Runtime.InteropServices;
using System.Security;

#endregion

public static class SecureStringExtensions
{
    public static void AppendString(this SecureString secureString, string value)
    {
        if (secureString == null)
        {
            throw new ArgumentNullException("secureString");
        }

        if (value.Length <= 0)
        {
            return;
        }

        foreach (var c in value)
        {
            secureString.AppendChar(c);
        }
    }

    public static string ConvertToUnsecureString(this SecureString secureString)
    {
        if (secureString == null)
        {
            throw new ArgumentNullException("secureString");
        }

        var unmanagedString = IntPtr.Zero;
        try
        {
            unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);
            return Marshal.PtrToStringUni(unmanagedString);
        }
        finally
        {
            Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
        }
    }
}