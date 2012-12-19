#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   SelectInstallForm.cs
//  *
//  *   This program is free software; you can redistribute it and/or modify
//  *   it under the terms of the GNU General Public License as published by
//  *   the Free Software Foundation; either version 3 of the License, or
//  *   (at your option) any later version.
//  ***************************************************************************/

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

#endregion

namespace OpenUO.Ultima.UnitTests
{
    public partial class SelectInstallForm : Form
    {
        public SelectInstallForm(string testSet)
        {
            InitializeComponent();
            Text = "Select an Installation " + testSet;
        }

        public InstallLocation SelectedInstall
        {
            get { return installsComboBox.SelectedValue as InstallLocation; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            IEnumerable<ComboBoxItem<InstallLocation>> installs =
                InstallationLocator.Locate().Select(
                    il =>
                    new ComboBoxItem<InstallLocation>(
                        string.Format("{0} - {1}", il.Version, il.Directory),
                        il)).ToList();

            installsComboBox.DisplayMember = "Display";
            installsComboBox.ValueMember = "Value";
            installsComboBox.DataSource = installs;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (SelectedInstall == null)
            {
                MessageBox.Show(
                    this, "You must choose a valid Ultima Online installation", "Invalid Install", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            Close();
        }

        private void findDirectoryButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select your Ultima Online directory";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    InstallLocation install = new InstallLocation(dialog.SelectedPath);
                    installsComboBox.SelectedItem = new ComboBoxItem<InstallLocation>(
                        string.Format("{0} - {1}", install.Version, install.Directory),
                        install);
                }
            }
        }

        private class ComboBoxItem<T>
        {
            public ComboBoxItem(string display, T value)
            {
                Display = display;
                Value = value;
            }

            public string Display
            {
                get;
                private set;
            }

            public T Value
            {
                get;
                private set;
            }
        }
    }
}