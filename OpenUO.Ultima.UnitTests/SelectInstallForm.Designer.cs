namespace OpenUO.Ultima.UnitTests
{
    partial class SelectInstallForm
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
            this.installsComboBox = new System.Windows.Forms.ComboBox();
            this.okButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // installsComboBox
            // 
            this.installsComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.installsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.installsComboBox.FormattingEnabled = true;
            this.installsComboBox.Location = new System.Drawing.Point(12, 12);
            this.installsComboBox.Name = "installsComboBox";
            this.installsComboBox.Size = new System.Drawing.Size(363, 21);
            this.installsComboBox.TabIndex = 0;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(300, 39);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // SelectInstallForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(387, 68);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.installsComboBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectInstallForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select an Installation";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox installsComboBox;
        private System.Windows.Forms.Button okButton;
    }
}