#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// GeneralExceptionHandler.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

using System;

using OpenUO.Core.Diagnostics.Tracing;

namespace OpenUO.Core.Diagnostics
{
    public class GeneralExceptionHandler
    {
        private static GeneralExceptionHandler _instance;

        public static GeneralExceptionHandler Instance
        {
            get { return _instance ?? (_instance = new GeneralExceptionHandler()); }
            set { _instance = value; }
        }

        public void OnError(Exception e)
        {
            Tracer.Error(e);
            OnErrorOverride(e);
        }

        protected virtual void OnErrorOverride(Exception e)
        {
        }
    }
}