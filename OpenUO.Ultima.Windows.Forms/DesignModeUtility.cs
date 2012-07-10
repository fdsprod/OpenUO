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
using System.Diagnostics;

namespace OpenUO.Ultima.Windows.Forms
{
    public static class DesignModeUtility
    {
        public static bool IsDesignMode
        {
            get
            {
                if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
                    return true;

                if (Process.GetCurrentProcess().ProcessName.ToUpper().Equals("DEVENV"))
                    return true;

                return false;
            }
        }
    }
}
