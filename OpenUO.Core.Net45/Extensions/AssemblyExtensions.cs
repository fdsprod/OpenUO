#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// AssemblyExtensions.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

public static class AssemblyExtensions
{
    public static Type[] GetTypes(this Assembly asm)
    {
        return asm.DefinedTypes.Select(type => type.AsType()).ToArray();
    }

    public static IEnumerable<Type> SafeGetTypes(this Assembly assembly)
    {
        Type[] assemblies;

        try
        {
            assemblies = assembly.GetTypes();
        }
        catch (FileNotFoundException)
        {
            assemblies = new Type[] {};
        }
        catch (NotSupportedException)
        {
            assemblies = new Type[] {};
        }

        return assemblies;
    }
}