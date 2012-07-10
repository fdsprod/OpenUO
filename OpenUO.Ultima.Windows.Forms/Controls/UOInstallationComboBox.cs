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
using System.Windows.Forms;
using System.Diagnostics;

namespace OpenUO.Ultima.Windows.Forms.Controls
{
    public sealed class UOInstallationComboBox : ComboBox
    {
        public new ComboBoxStyle DropDownStyle
        {
            get { return base.DropDownStyle; }
            set { ; }
        }

        public new object DataSource
        {
            get { return base.DataSource; }
            set { ; }
        }

        public new string DisplayMember
        {
            get { return base.DisplayMember; }
            set { ; }
        }

        public new string ValueMember
        {
            get { return base.ValueMember; }
            set { ; }
        }

        public InstallLocation SelectedInstallation
        {
            get { return SelectedValue as InstallLocation; }
        }

        public event EventHandler SelectedInstallationChanged
        {
            add { SelectedIndexChanged += value; }
            remove { SelectedIndexChanged -= value; }
        }

        public UOInstallationComboBox()
        {
            base.DisplayMember = "Display";
            base.ValueMember = "Value";
            base.DropDownStyle = ComboBoxStyle.DropDownList;

            if (!DesignModeUtility.IsDesignMode)
            {
                IEnumerable<ComboBoxItem<InstallLocation>> installs =
                    InstallationLocator.Locate().Select(il =>
                        new ComboBoxItem<InstallLocation>(
                            string.Format("{0} - {1}", il.Version, il.Directory),
                            il)).ToList();

                base.DataSource = installs;
            }
        }
    }
}
