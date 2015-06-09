#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// MethodInfoExtensions.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class MethodInfoExtensions
{
    public static MethodInfo GetBaseDefinition(this MethodInfo method)
    {
        var flags = BindingFlags.Instance;

        if (method.IsPublic)
        {
            flags |= BindingFlags.Public;
        }
        else
        {
            flags |= BindingFlags.NonPublic;
        }

        // get...
        var info = method.DeclaringType.GetTypeInfo();
        var found = new List<MethodInfo>();

        while (true)
        {
            // find...
            var inParent = info.AsType().GetMethod(method.Name, flags, method.GetParameters().Select(parameter => parameter.ParameterType).ToArray());
            if (inParent != null)
            {
                found.Add(inParent);
            }

            // up...
            if (info.BaseType == null)
            {
                break;
            }

            info = info.BaseType.GetTypeInfo();
        }

        // return the last one...
        return found.Last();
    }

    public static bool IsAbstract(this MethodBase method)
    {
        return method.IsAbstract;
    }

    public static MemberTypes MemberType()
    {
        return MemberTypes.Method;
    }
}