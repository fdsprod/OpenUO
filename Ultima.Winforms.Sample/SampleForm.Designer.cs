namespace Ultima.Winforms.Sample
{
    partial class SampleForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.artworkControl2 = new OpenUO.Ultima.Windows.Forms.Controls.ArtworkControl();
            this.artworkControl1 = new OpenUO.Ultima.Windows.Forms.Controls.ArtworkControl();
            this.uoInstallationComboBox1 = new OpenUO.Ultima.Windows.Forms.Controls.UOInstallationComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Installation:";
            // 
            // artworkControl2
            // 
            this.artworkControl2.ArtworkControlType = OpenUO.Ultima.Windows.Forms.Controls.ArtworkControlType.Statics;
            this.artworkControl2.Location = new System.Drawing.Point(512, 33);
            this.artworkControl2.Name = "artworkControl2";
            this.artworkControl2.Size = new System.Drawing.Size(508, 549);
            this.artworkControl2.TabIndex = 6;
            this.artworkControl2.Text = "artworkControl2";
            // 
            // artworkControl1
            // 
            this.artworkControl1.ArtworkControlType = OpenUO.Ultima.Windows.Forms.Controls.ArtworkControlType.Land;
            this.artworkControl1.Location = new System.Drawing.Point(15, 33);
            this.artworkControl1.Name = "artworkControl1";
            this.artworkControl1.Size = new System.Drawing.Size(491, 549);
            this.artworkControl1.TabIndex = 5;
            this.artworkControl1.Text = "artworkControl1";
            // 
            // uoInstallationComboBox1
            // 
            this.uoInstallationComboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uoInstallationComboBox1.DisplayMember = "Display";
            this.uoInstallationComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.uoInstallationComboBox1.FormattingEnabled = true;
            this.uoInstallationComboBox1.Location = new System.Drawing.Point(78, 6);
            this.uoInstallationComboBox1.Name = "uoInstallationComboBox1";
            this.uoInstallationComboBox1.Size = new System.Drawing.Size(942, 21);
            this.uoInstallationComboBox1.TabIndex = 2;
            this.uoInstallationComboBox1.ValueMember = "Value";
            this.uoInstallationComboBox1.SelectedInstallationChanged += new System.EventHandler(this.uoInstallationComboBox1_SelectedInstallationChanged);
            // 
            // SampleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1032, 594);
            this.Controls.Add(this.artworkControl2);
            this.Controls.Add(this.artworkControl1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.uoInstallationComboBox1);
            this.Name = "SampleForm";
            this.Text = "OpenUO Ultima SDK Winforms Sample";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private OpenUO.Ultima.Windows.Forms.Controls.UOInstallationComboBox uoInstallationComboBox1;
        private System.Windows.Forms.Label label1;
        private OpenUO.Ultima.Windows.Forms.Controls.ArtworkControl artworkControl1;
        private OpenUO.Ultima.Windows.Forms.Controls.ArtworkControl artworkControl2;
    }
}

