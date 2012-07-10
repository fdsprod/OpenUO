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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace OpenUO.Core
{
	public static class IPAddressExtensions
	{
		private static Dictionary<IPAddress, IPAddress> _ipAddressTable;

		public static IPAddress Intern( this IPAddress ipAddress )
		{
			if ( _ipAddressTable == null )
			{
				_ipAddressTable = new Dictionary<IPAddress, IPAddress>();
			}

			IPAddress interned;

			if ( !_ipAddressTable.TryGetValue( ipAddress, out interned ) )
			{
				interned = ipAddress;
				_ipAddressTable[ipAddress] = interned;
			}

			return interned;
		}
	}
}
