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

namespace OpenUO.Ultima
{
	public class ClilocTable : IEnumerable<ClilocData>, IDisposable
	{
		private Dictionary<int, ClilocData> _Table = new Dictionary<int, ClilocData>();

		public virtual ClilocLNG Language { get; protected set; }
		public virtual FileInfo InputFile { get; protected set; }

		public int Count { get { return _Table.Count; } }

		public bool Loaded { get; private set; }

		public ClilocTable()
		{ }

		public void Clear()
		{
			foreach (var d in _Table.Values)
				d.Clear();
		}

		public void Dispose()
		{
			Unload();
		}

		public void Unload()
		{
			if (!Loaded)
				return;

			Language = ClilocLNG.NULL;
			InputFile = null;
			_Table.Clear();

			Loaded = false;
		}

		public void Load(FileInfo file)
		{
			if (Loaded)
				return;

			try
			{
				ClilocLNG lng = ClilocLNG.NULL;

				if (!Enum.TryParse<ClilocLNG>(file.Extension.TrimStart('.'), true, out lng))
					throw new FileLoadException("Could not detect language for: " + file.FullName);

				Language = lng;
				InputFile = file;

				using (BinaryReader reader = new BinaryReader(InputFile.OpenRead(), Encoding.UTF8))
				{
					long size = reader.BaseStream.Seek(0, SeekOrigin.End);
					reader.BaseStream.Seek(0, SeekOrigin.Begin);

					reader.ReadInt32();
					reader.ReadInt16();

					while (reader.BaseStream.Position < size)
					{
						int index = reader.ReadInt32();
						reader.ReadByte();
						int length = reader.ReadInt16();
						long offset = reader.BaseStream.Position;
						reader.BaseStream.Seek(length, SeekOrigin.Current);

						if (_Table.ContainsKey(index))
							_Table[index] = new ClilocData(Language, index, offset, length);
						else
							_Table.Add(index, new ClilocData(Language, index, offset, length));
					}
				}

				Loaded = true;
			}
			catch (Exception e)
			{
				Tracer.Error(e);
			}
		}

		public bool Contains(int index)
		{
			return _Table.ContainsKey(index);
		}

		public bool IsNullOrEmpty(int index)
		{
			if (!Contains(index) || _Table[index] == null)
				return true;

			ClilocInfo info = _Table[index].Lookup(InputFile);

			if (!String.IsNullOrWhiteSpace(info.Text))
				return false;

			return true;
		}

		public ClilocInfo Update(int index)
		{
			if (!Contains(index) || _Table[index] == null)
				return null;

			return _Table[index].Lookup(InputFile, true);
		}

		public ClilocInfo Lookup(int index)
		{
			if (!Contains(index) || _Table[index] == null)
				return null;

			return _Table[index].Lookup(InputFile);
		}

		public ClilocInfo this[int index] { get { return Lookup(index); } }

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _Table.Values.GetEnumerator();
		}

		public virtual IEnumerator<ClilocData> GetEnumerator()
		{
			return _Table.Values.GetEnumerator();
		}

		public override string ToString()
		{
			return ((Language == ClilocLNG.NULL) ? "Not Loaded" : "Cliloc." + Language);
		}
	}
}
