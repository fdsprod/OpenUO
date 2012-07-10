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

namespace OpenUO.Ultima
{
    public sealed class UltimaException : Exception
    {
        public UltimaException(string format, params object[] args)
            : base(string.Format(format, args)) { }
    }
}
