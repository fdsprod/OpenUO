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

using OpenUO.Core;
using OpenUO.Core.Diagnostics;

namespace OpenUO.Ultima.UnitTests
{
    public class TestingBase
    {
        private static bool _configuredInstallForTest;
        private static InstallLocation _install;

        protected static InstallLocation Install
        {
            get
            {
                if (!_configuredInstallForTest)
                {
                    using (SelectInstallForm form = new SelectInstallForm("CoreAdapterTests"))
                    {
                        form.ShowDialog();
                        _install = form.SelectedInstall;
                        _configuredInstallForTest = true;
                    }

                    //Outputs Trace warnings and errors to the Visual Studio Output Console.
                    new DebugTraceListener();
                }

                Guard.AssertIsNotNull(_install, "Ultima Online is not installed");

                return _install;
            }
        }
    }
}
