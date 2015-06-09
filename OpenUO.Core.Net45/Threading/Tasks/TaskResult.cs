#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// TaskResult.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region Usings

using System.Threading.Tasks;

#endregion

namespace OpenUO.Core.Threading.Tasks
{
    public static class TaskResult
    {
        public static readonly Task Complete = Task.FromResult(true);
        public static readonly Task True = Task.FromResult(true);
        public static readonly Task False = Task.FromResult(false);
    }
}