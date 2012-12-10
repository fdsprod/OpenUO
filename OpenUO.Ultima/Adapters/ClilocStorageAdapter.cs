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
	public class ClientLocalizationStorageAdapter : StorageAdapterBase, IClilocStorageAdapter<ClilocInfo>
	{
		private Dictionary<ClientLocalizationLanguage, ClientLocalizations> _tables = new Dictionary<ClientLocalizationLanguage, ClientLocalizations>
		{
			{ ClientLocalizationLanguage.ENU, new ClientLocalizations() },
			{ ClientLocalizationLanguage.DEU, new ClientLocalizations() },
			{ ClientLocalizationLanguage.ESP, new ClientLocalizations() },
			{ ClientLocalizationLanguage.FRA, new ClientLocalizations() },
			{ ClientLocalizationLanguage.JPN, new ClientLocalizations() },
			{ ClientLocalizationLanguage.KOR, new ClientLocalizations() }
		};

		public Dictionary<ClientLocalizationLanguage, ClientLocalizations> Tables 
        { 
            get { return _tables; } 
        }

        public override int Length
        {
            get
            { 
                if(!IsInitialized)
                    Initialize();
                
                return _tables[0].Count;
            }
        }

		public override void Initialize()
		{
			base.Initialize();

			List<ClientLocalizations> tables = new List<ClientLocalizations>(_tables.Values);
            var loaded = tables.TrueForAll(t => t.Loaded);

            if (loaded || (Install == null || String.IsNullOrWhiteSpace(Install.Directory)))
            {
                return; 
            }

			foreach (var kvp in _tables)
			{
				if (kvp.Value.Loaded)
				{ 
                    continue; 
                }

				string stub = Path.Combine(Install.Directory, "/Cliloc." + kvp.Key.ToString().ToLower());

				if (File.Exists(stub))
				{ 
                    kvp.Value.Load(new FileInfo(stub)); 
                }
			}
		}

		public unsafe ClilocInfo GetCliloc(ClientLocalizationLanguage lng, int index)
		{
			if (_tables.ContainsKey(lng) && _tables[lng] != null)
			{ 
                return _tables[lng].Lookup(index); 
            }

			return null;
		}

		public unsafe string GetRawString(ClientLocalizationLanguage lng, int index)
		{
			if (_tables.ContainsKey(lng) && _tables[lng] != null && !_tables[lng].IsNullOrEmpty(index))
			{ 
                return _tables[lng][index].Text;
            }

			return String.Empty;
		}

		public unsafe string GetString(ClientLocalizationLanguage lng, int index, string args)
		{
			ClilocInfo info = GetCliloc(lng, index);

			if (info == null)
			{ 
                return String.Empty; 
            }

			return info.ToString(args);
		}

		public unsafe string GetString(ClientLocalizationLanguage lng, int index, params string[] args)
		{
			ClilocInfo info = GetCliloc(lng, index);

			if (info == null)
			{ 
                return String.Empty; 
            }

			return info.ToString(args);
		}
	}
}
