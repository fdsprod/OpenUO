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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace OpenUO.Ultima
{
	public class ClilocInfo
	{
		public static readonly Regex VarPattern = new Regex(@"(~(\d+)_(\w+)~)", RegexOptions.IgnoreCase);

		public ClilocLNG Language { get; protected set; }
		public int Index { get; protected set; }
		public string Text { get; protected set; }

		public ClilocInfo(ClilocLNG lng, int index, string text)
		{
			Language = lng;
			Index = index;
			Text = text;
		}

		public override string ToString()
		{
			return Text;
		}

		public virtual string ToString(string args)
		{
			if (String.IsNullOrWhiteSpace(args))
				return ToString();

			return ToString(args.Contains("\t") ? Regex.Split(args, "\t") : new string[] { args });
		}

		public virtual string ToString(string[] args)
		{
			if (args == null || args.Length == 0)
				return ToString();

			string text = Text;
			Match match = VarPattern.Match(text);

			while (match.Success)
			{
				int i = Int32.Parse(match.Groups[2].Value);

				if (args.Length < i)
				{
					text = null;
					break;
				}

				text = text.Replace(match.Groups[1].Value, args[i - 1]);
				match = match.NextMatch();
			}

			return text.Trim();
		}
	}
}