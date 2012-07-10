namespace Ultima.Winforms.Sample
{
    partial class Form1
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
            this.uoInstallationComboBox1 = new OpenUO.Ultima.Windows.Forms.Controls.UOInstallationComboBox();
            this.SuspendLayout();
            // 
            // uoInstallationComboBox1
            // 
            this.uoInstallationComboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uoInstallationComboBox1.DisplayMember = "Display";
            this.uoInstallationComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.uoInstallationComboBox1.FormattingEnabled = true;
            this.uoInstallationComboBox1.Location = new System.Drawing.Point(12, 12);
            this.uoInstallationComboBox1.Name = "uoInstallationComboBox1";
            this.uoInstallationComboBox1.Size = new System.Drawing.Size(1386, 21);
            this.uoInstallationComboBox1.TabIndex = 3;
            this.uoInstallationComboBox1.ValueMember = "Value";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1410, 837);
            this.Controls.Add(this.uoInstallationComboBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.ResumeLayout(false);

        }

        #endregion

        private OpenUO.Ultima.Windows.Forms.Controls.UOInstallationComboBox uoInstallationComboBox1;
    }
}