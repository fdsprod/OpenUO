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

using OpenUO.Core.Diagnostics;

namespace OpenUO.Ultima.Adapters
{
	public class ClilocStorageAdapter : StorageAdapterBase, IClilocStorageAdapter<ClilocInfo>
	{
		private Dictionary<ClilocLNG, ClilocTable> _Tables = new Dictionary<ClilocLNG, ClilocTable>
		{
			{ ClilocLNG.ENU, new ClilocTable() },
			{ ClilocLNG.DEU, new ClilocTable() },
			{ ClilocLNG.ESP, new ClilocTable() },
			{ ClilocLNG.FRA, new ClilocTable() },
			{ ClilocLNG.JPN, new ClilocTable() },
			{ ClilocLNG.KOR, new ClilocTable() }
		};
		public Dictionary<ClilocLNG, ClilocTable> Tables { get { return _Tables; } }

		public override void Initialize()
		{
			base.Initialize();

			List<ClilocTable> tables = new List<ClilocTable>(_Tables.Values);

			if (tables.TrueForAll((ClilocTable t) => { return t.Loaded; }) || (Install == null || String.IsNullOrWhiteSpace(Install.Directory)))
			{ return; }

			foreach (var kvp in _Tables)
			{
				if (kvp.Value.Loaded)
				{ continue; }

				string stub = Path.Combine(Install.Directory, "/Cliloc." + kvp.Key.ToString().ToLower());

				if (File.Exists(stub))
				{ kvp.Value.Load(new FileInfo(stub)); }
			}
		}

		public unsafe ClilocInfo GetCliloc(ClilocLNG lng, int index)
		{
			if (_Tables.ContainsKey(lng) && _Tables[lng] != null)
			{ return _Tables[lng].Lookup(index); }

			return null;
		}

		public unsafe string GetRawString(ClilocLNG lng, int index)
		{
			if (_Tables.ContainsKey(lng) && _Tables[lng] != null && !_Tables[lng].IsNullOrEmpty(index))
			{ return _Tables[lng][index].Text; }

			return String.Empty;
		}

		public unsafe string GetString(ClilocLNG lng, int index, string args)
		{
			ClilocInfo info = GetCliloc(lng, index);

			if (info == null)
			{ return String.Empty; }

			return info.ToString(args);
		}

		public unsafe string GetString(ClilocLNG lng, int index, params string[] args)
		{
			ClilocInfo info = GetCliloc(lng, index);

			if (info == null)
			{ return String.Empty; }

			return info.ToString(args);
		}
	}
}
