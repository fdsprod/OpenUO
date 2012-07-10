using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace OpenUO.Ultima.UnitTests
{
    public partial class SelectInstallForm : Form
    {
        public InstallLocation SelectedInstall
        {
            get { return installsComboBox.SelectedValue as InstallLocation; }
        }

        public SelectInstallForm(string testSet)
        {
            InitializeComponent();
            Text = "Select an Installation " + testSet;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            IEnumerable<ComboBoxItem<InstallLocation>> installs =
                InstallationLocator.Locate().Select(il =>
                    new ComboBoxItem<InstallLocation>(
                        string.Format("{0} - {1}", il.Version, il.Directory),
                        il)).ToList();

            installsComboBox.DisplayMember = "Display";
            installsComboBox.ValueMember = "Value";
            installsComboBox.DataSource = installs;
        }

        private class ComboBoxItem<T>
        {
            public string Display { get; private set; }
            public T Value { get; private set; }

            public ComboBoxItem(string display, T value)
            {
                Display = display;
                Value = value;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
