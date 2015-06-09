#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// StringBuilderExtensions.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

using System.Text;

public static class StringBuilderExtensions
{
    public static StringBuilder AppendLineFormat(this StringBuilder builder, string format, params object[] args)
    {
        builder.AppendFormat(format, args);
        builder.AppendLine();

        return builder;
    }
}