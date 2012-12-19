#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   ITraceListener.cs
//  *
//  *   This program is free software; you can redistribute it and/or modify
//  *   it under the terms of the GNU General Public License as published by
//  *   the Free Software Foundation; either version 3 of the License, or
//  *   (at your option) any later version.
//  ***************************************************************************/

#endregion

#region Usings

using System;

#endregion

namespace OpenUO.Core.Diagnostics
{
    public interface ITraceListener : IDisposable
    {
        TraceLevels? TraceLevel
        {
            get;
            set;
        }
    }
}