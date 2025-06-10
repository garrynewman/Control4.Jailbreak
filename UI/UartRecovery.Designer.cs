namespace Garry.Control4.Jailbreak.UI
{
    partial class UartRecovery
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
            try
            {
                if (disposing)
                {
                    if (components != null)
                    {
                        components.Dispose();
                    }
                    if (_deviceWatcher != null)
                    {
                        _deviceWatcher.Stop();
                        _deviceWatcher.Dispose();
                        _deviceWatcher = null;
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UartRecovery));
            this.labelUartRecovery = new System.Windows.Forms.Label();
            this.comboBoxComPort = new System.Windows.Forms.ComboBox();
            this.labelUartRecoveryHelpText = new System.Windows.Forms.Label();
            this.labelComPort = new System.Windows.Forms.Label();
            this.buttonRestoreSSHAccess = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelUartRecovery
            // 
            this.labelUartRecovery.AutoSize = true;
            this.labelUartRecovery.Font = new System.Drawing.Font("Microsoft YaHei", 16F, System.Drawing.FontStyle.Bold);
            this.labelUartRecovery.ForeColor = System.Drawing.SystemColors.Highlight;
            this.labelUartRecovery.Location = new System.Drawing.Point(24, 31);
            this.labelUartRecovery.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelUartRecovery.Name = "labelUartRecovery";
            this.labelUartRecovery.Size = new System.Drawing.Size(393, 57);
            this.labelUartRecovery.TabIndex = 17;
            this.labelUartRecovery.Text = "UART RECOVERY";
            // 
            // comboBoxComPort
            // 
            this.comboBoxComPort.FormattingEnabled = true;
            this.comboBoxComPort.Items.AddRange(new object[] {
            "Select COM port"});
            this.comboBoxComPort.Location = new System.Drawing.Point(32, 346);
            this.comboBoxComPort.Name = "comboBoxComPort";
            this.comboBoxComPort.Size = new System.Drawing.Size(932, 33);
            this.comboBoxComPort.TabIndex = 1;
            // 
            // labelUartRecoveryHelpText
            // 
            this.labelUartRecoveryHelpText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelUartRecoveryHelpText.Location = new System.Drawing.Point(32, 142);
            this.labelUartRecoveryHelpText.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelUartRecoveryHelpText.Name = "labelUartRecoveryHelpText";
            this.labelUartRecoveryHelpText.Size = new System.Drawing.Size(932, 134);
            this.labelUartRecoveryHelpText.TabIndex = 19;
            this.labelUartRecoveryHelpText.Text = resources.GetString("labelUartRecoveryHelpText.Text");
            // 
            // labelComPort
            // 
            this.labelComPort.AutoSize = true;
            this.labelComPort.Location = new System.Drawing.Point(34, 315);
            this.labelComPort.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelComPort.Name = "labelComPort";
            this.labelComPort.Size = new System.Drawing.Size(106, 25);
            this.labelComPort.TabIndex = 26;
            this.labelComPort.Text = "COM Port";
            // 
            // buttonRestoreSSHAccess
            // 
            this.buttonRestoreSSHAccess.Image = global::Garry.Control4.Jailbreak.Properties.Resources.com_port;
            this.buttonRestoreSSHAccess.Location = new System.Drawing.Point(594, 396);
            this.buttonRestoreSSHAccess.Margin = new System.Windows.Forms.Padding(6);
            this.buttonRestoreSSHAccess.Name = "buttonRestoreSSHAccess";
            this.buttonRestoreSSHAccess.Size = new System.Drawing.Size(370, 65);
            this.buttonRestoreSSHAccess.TabIndex = 2;
            this.buttonRestoreSSHAccess.Text = "Restore SSH Access";
            this.buttonRestoreSSHAccess.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonRestoreSSHAccess.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonRestoreSSHAccess.UseVisualStyleBackColor = true;
            this.buttonRestoreSSHAccess.Click += new System.EventHandler(this.UartRecoveryButton_Click);
            // 
            // UartRecovery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonRestoreSSHAccess);
            this.Controls.Add(this.labelComPort);
            this.Controls.Add(this.labelUartRecoveryHelpText);
            this.Controls.Add(this.comboBoxComPort);
            this.Controls.Add(this.labelUartRecovery);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "UartRecovery";
            this.Size = new System.Drawing.Size(1000, 852);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelUartRecovery;
        private System.Windows.Forms.ComboBox comboBoxComPort;
        private System.Windows.Forms.Label labelUartRecoveryHelpText;
        private System.Windows.Forms.Label labelComPort;
        private System.Windows.Forms.Button buttonRestoreSSHAccess;
    }
}
