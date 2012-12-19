#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   DesignModeUtility.cs
//  *
//  *   This program is free software; you can redistribute it and/or modify
//  *   it under the terms of the GNU General Public License as published by
//  *   the Free Software Foundation; either version 3 of the License, or
//  *   (at your option) any later version.
//  ***************************************************************************/

#endregion

#region Usings

using System.ComponentModel;
using System.Diagnostics;

#endregion

namespace OpenUO.Ultima.Windows.Forms
{
    public static class DesignModeUtility
    {
        public static bool IsDesignMode
        {
            get
            {
                if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                {
                    return true;
                }

                if (Process.GetCurrentProcess().ProcessName.ToUpper().Equals("DEVENV"))
                {
                    return true;
                }

                return false;
            }
        }
    }
}