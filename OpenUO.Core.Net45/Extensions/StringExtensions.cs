#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// StringExtensions.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

using System;
using System.Text;

public static class StringExtensions
{
    public static string Wrap(this string sentence, int limit, int indentationCount, char indentationCharacter)
    {
        var words = sentence.Replace("\n", " ").Replace("\r", " ").Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
        var counter = 0;
        var builder = new StringBuilder();

        for (var index = 0; index < words.Length; index++)
        {
            var word = words[index];

            if ((builder.Length + word.Length)/limit > counter)
            {
                counter++;
                builder.AppendLine();

                for (var i = 0; i < indentationCount; i++)
                {
                    builder.Append(indentationCharacter);
                }
            }

            builder.Append(word);

            if (index < words.Length)
            {
                builder.Append(" ");
            }
        }

        return builder.ToString();
    }
}