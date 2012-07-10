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

namespace Client.Patterns
{
    public interface IChainStep<T>
    {
        string Name { get; }
        bool CancelExecution { get; set; }
        IChainStep<T> Successor { get; set; }
        ChainDependency[] Dependencies { get; }

        void Execute(T state);
    }

    public class ChainDependency
    {
        public string Name { get; set; }
        public bool MustExist { get; set; }

        public ChainDependency(string name, bool mustExist)
        {
            Name = name;
            MustExist = mustExist;
        }
    }
}
