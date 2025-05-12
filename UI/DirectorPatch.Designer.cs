namespace  Garry.Control4.Jailbreak.UI
{
	partial class DirectorPatch
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if ( disposing && (components != null) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DirectorPatch));
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.Password = new System.Windows.Forms.TextBox();
            this.Username = new System.Windows.Forms.TextBox();
            this.IpAddress = new System.Windows.Forms.TextBox();
            this.labelIpAddress = new System.Windows.Forms.Label();
            this.labelUsername = new System.Windows.Forms.Label();
            this.labelPassword = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.labelMacAddress = new System.Windows.Forms.Label();
            this.MacAddress = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft YaHei", 16F, System.Drawing.FontStyle.Bold);
            this.label6.ForeColor = System.Drawing.SystemColors.Highlight;
            this.label6.Location = new System.Drawing.Point(24, 31);
            this.label6.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(538, 57);
            this.label6.TabIndex = 16;
            this.label6.Text = "DIRECTOR CERTIFICATE";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.Location = new System.Drawing.Point(32, 142);
            this.label7.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(932, 121);
            this.label7.TabIndex = 17;
            this.label7.Text = "If you created a user we\'ll be able to patch the director with the new certificat" +
    "e.\r\n\r\nWe can work out the password to your director, just make sure the Address " +
    "is correct!\r\n";
            // 
            // button4
            // 
            this.button4.Image = global::Garry.Control4.Jailbreak.Properties.Resources.cup_cake;
            this.button4.Location = new System.Drawing.Point(594, 344);
            this.button4.Margin = new System.Windows.Forms.Padding(6);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(370, 65);
            this.button4.TabIndex = 18;
            this.button4.Text = "Patch Director Certificates";
            this.button4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button4.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.PatchDirectorCertificates);
            // 
            // Password
            // 
            this.Password.Location = new System.Drawing.Point(490, 294);
            this.Password.Margin = new System.Windows.Forms.Padding(6);
            this.Password.Name = "Password";
            this.Password.Size = new System.Drawing.Size(470, 31);
            this.Password.TabIndex = 22;
            // 
            // Username
            // 
            this.Username.Location = new System.Drawing.Point(304, 294);
            this.Username.Margin = new System.Windows.Forms.Padding(6);
            this.Username.Name = "Username";
            this.Username.Size = new System.Drawing.Size(170, 31);
            this.Username.TabIndex = 23;
            this.Username.Text = "root";
            // 
            // IpAddress
            // 
            this.IpAddress.Location = new System.Drawing.Point(32, 294);
            this.IpAddress.Margin = new System.Windows.Forms.Padding(6);
            this.IpAddress.Name = "IpAddress";
            this.IpAddress.Size = new System.Drawing.Size(256, 31);
            this.IpAddress.TabIndex = 24;
            this.IpAddress.Text = "127.0.0.1";
            this.IpAddress.TextChanged += new System.EventHandler(this.OnIpAddressChanged);
            // 
            // labelIpAddress
            // 
            this.labelIpAddress.AutoSize = true;
            this.labelIpAddress.Location = new System.Drawing.Point(34, 263);
            this.labelIpAddress.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelIpAddress.Name = "labelIpAddress";
            this.labelIpAddress.Size = new System.Drawing.Size(116, 25);
            this.labelIpAddress.TabIndex = 25;
            this.labelIpAddress.Text = "IP Address";
            // 
            // labelUsername
            // 
            this.labelUsername.AutoSize = true;
            this.labelUsername.Location = new System.Drawing.Point(298, 263);
            this.labelUsername.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelUsername.Name = "labelUsername";
            this.labelUsername.Size = new System.Drawing.Size(110, 25);
            this.labelUsername.TabIndex = 26;
            this.labelUsername.Text = "Username";
            // 
            // labelPassword
            // 
            this.labelPassword.AutoSize = true;
            this.labelPassword.Location = new System.Drawing.Point(484, 263);
            this.labelPassword.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(218, 25);
            this.labelPassword.TabIndex = 27;
            this.labelPassword.Text = "Password (OS3 Only)";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft YaHei", 16F, System.Drawing.FontStyle.Bold);
            this.label13.ForeColor = System.Drawing.SystemColors.Highlight;
            this.label13.Location = new System.Drawing.Point(24, 467);
            this.label13.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(459, 57);
            this.label13.TabIndex = 28;
            this.label13.Text = "RESTART DIRECTOR";
            // 
            // label14
            // 
            this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label14.Location = new System.Drawing.Point(34, 544);
            this.label14.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(932, 137);
            this.label14.TabIndex = 29;
            this.label14.Text = resources.GetString("label14.Text");
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(594, 731);
            this.button1.Margin = new System.Windows.Forms.Padding(6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(370, 65);
            this.button1.TabIndex = 30;
            this.button1.Text = "Reboot Director";
            this.button1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.RebootDirector);
            // 
            // labelMacAddress
            // 
            this.labelMacAddress.AutoSize = true;
            this.labelMacAddress.Location = new System.Drawing.Point(32, 344);
            this.labelMacAddress.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelMacAddress.Name = "labelMacAddress";
            this.labelMacAddress.Size = new System.Drawing.Size(144, 25);
            this.labelMacAddress.TabIndex = 31;
            this.labelMacAddress.Text = "MAC Address";
            // 
            // MacAddress
            // 
            this.MacAddress.Location = new System.Drawing.Point(34, 375);
            this.MacAddress.Margin = new System.Windows.Forms.Padding(6);
            this.MacAddress.MaxLength = 17;
            this.MacAddress.Name = "MacAddress";
            this.MacAddress.Size = new System.Drawing.Size(256, 31);
            this.MacAddress.TabIndex = 32;
            this.MacAddress.TextChanged += new System.EventHandler(this.OnMacAddressChanged);
            // 
            // DirectorPatch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.MacAddress);
            this.Controls.Add(this.labelMacAddress);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.labelPassword);
            this.Controls.Add(this.labelUsername);
            this.Controls.Add(this.labelIpAddress);
            this.Controls.Add(this.IpAddress);
            this.Controls.Add(this.Username);
            this.Controls.Add(this.Password);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "DirectorPatch";
            this.Size = new System.Drawing.Size(1000, 852);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TextBox Password;
        private System.Windows.Forms.TextBox Username;
        private System.Windows.Forms.Label labelIpAddress;
        private System.Windows.Forms.Label labelUsername;
        private System.Windows.Forms.Label labelPassword;
        public System.Windows.Forms.TextBox IpAddress;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label labelMacAddress;
        public System.Windows.Forms.TextBox MacAddress;
    }
}
