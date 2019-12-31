namespace  Garry.Control4.Jailbreak
{
	partial class Director
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
			this.controllerList = new System.Windows.Forms.ListBox();
			this.refreshLink = new System.Windows.Forms.LinkLabel();
			this.controllerAddress = new System.Windows.Forms.TextBox();
			this.selectButton = new System.Windows.Forms.Button();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.directorInfo = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.SuspendLayout();
			// 
			// controllerList
			// 
			this.controllerList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.controllerList.FormattingEnabled = true;
			this.controllerList.Location = new System.Drawing.Point(0, 0);
			this.controllerList.Name = "controllerList";
			this.controllerList.Size = new System.Drawing.Size(232, 410);
			this.controllerList.TabIndex = 1;
			this.controllerList.SelectedIndexChanged += new System.EventHandler(this.SelectedIndexChanged);
			// 
			// refreshLink
			// 
			this.refreshLink.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.refreshLink.BackColor = System.Drawing.Color.Transparent;
			this.refreshLink.Location = new System.Drawing.Point(0, -1);
			this.refreshLink.Margin = new System.Windows.Forms.Padding(5);
			this.refreshLink.Name = "refreshLink";
			this.refreshLink.Size = new System.Drawing.Size(232, 19);
			this.refreshLink.TabIndex = 6;
			this.refreshLink.TabStop = true;
			this.refreshLink.Text = "Refresh";
			this.refreshLink.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.refreshLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.RefreshLink);
			// 
			// controllerAddress
			// 
			this.controllerAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.controllerAddress.Location = new System.Drawing.Point(0, 26);
			this.controllerAddress.Name = "controllerAddress";
			this.controllerAddress.Size = new System.Drawing.Size(232, 20);
			this.controllerAddress.TabIndex = 5;
			this.controllerAddress.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.controllerAddress.TextChanged += new System.EventHandler(this.ControllerAddressTextChanged);
			// 
			// selectButton
			// 
			this.selectButton.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.selectButton.Enabled = false;
			this.selectButton.Location = new System.Drawing.Point(0, 52);
			this.selectButton.Name = "selectButton";
			this.selectButton.Size = new System.Drawing.Size(232, 25);
			this.selectButton.TabIndex = 4;
			this.selectButton.Text = "Select && View Controller Info";
			this.selectButton.UseVisualStyleBackColor = true;
			this.selectButton.Click += new System.EventHandler(this.ConnectButtonClicked);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.directorInfo);
			this.splitContainer1.Size = new System.Drawing.Size(593, 491);
			this.splitContainer1.SplitterDistance = 232;
			this.splitContainer1.TabIndex = 7;
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer2.IsSplitterFixed = true;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.controllerList);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.selectButton);
			this.splitContainer2.Panel2.Controls.Add(this.refreshLink);
			this.splitContainer2.Panel2.Controls.Add(this.controllerAddress);
			this.splitContainer2.Size = new System.Drawing.Size(232, 491);
			this.splitContainer2.SplitterDistance = 410;
			this.splitContainer2.TabIndex = 7;
			// 
			// directorInfo
			// 
			this.directorInfo.AutoSize = true;
			this.directorInfo.Location = new System.Drawing.Point(15, 21);
			this.directorInfo.Name = "directorInfo";
			this.directorInfo.Size = new System.Drawing.Size(13, 13);
			this.directorInfo.TabIndex = 0;
			this.directorInfo.Text = "..";
			// 
			// Controller
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer1);
			this.Name = "Controller";
			this.Size = new System.Drawing.Size(593, 491);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			this.splitContainer2.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListBox controllerList;
		private System.Windows.Forms.LinkLabel refreshLink;
		private System.Windows.Forms.TextBox controllerAddress;
		private System.Windows.Forms.Button selectButton;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.Label directorInfo;
	}
}
