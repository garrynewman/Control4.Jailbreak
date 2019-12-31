namespace  Garry.Control4.Jailbreak
{
	partial class Restore
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
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.backupList = new System.Windows.Forms.ListBox();
			this.label3 = new System.Windows.Forms.Label();
			this.restoreUsingFile = new System.Windows.Forms.Button();
			this.restoreFromDirector = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(24, 53);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(390, 44);
			this.label2.TabIndex = 2;
			this.label2.Text = "This will restore your director back to the original certificate and reboot it. T" +
    "here\'s no real reason to do this but it\'s here for your peace of mind.";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.ForeColor = System.Drawing.SystemColors.Highlight;
			this.label1.Location = new System.Drawing.Point(24, 26);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(68, 18);
			this.label1.TabIndex = 1;
			this.label1.Text = "Restore";
			// 
			// backupList
			// 
			this.backupList.FormattingEnabled = true;
			this.backupList.Location = new System.Drawing.Point(27, 165);
			this.backupList.Name = "backupList";
			this.backupList.Size = new System.Drawing.Size(387, 134);
			this.backupList.TabIndex = 3;
			this.backupList.SelectedIndexChanged += new System.EventHandler(this.SelectedIndexChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(24, 149);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(125, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "Backups On Director";
			// 
			// restoreUsingFile
			// 
			this.restoreUsingFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.restoreUsingFile.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.restoreUsingFile.Image = global:: Garry.Control4.Jailbreak.Properties.Resources.folder;
			this.restoreUsingFile.Location = new System.Drawing.Point(238, 89);
			this.restoreUsingFile.Name = "restoreUsingFile";
			this.restoreUsingFile.Padding = new System.Windows.Forms.Padding(2);
			this.restoreUsingFile.Size = new System.Drawing.Size(176, 29);
			this.restoreUsingFile.TabIndex = 4;
			this.restoreUsingFile.Text = " Restore Using File...";
			this.restoreUsingFile.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.restoreUsingFile.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.restoreUsingFile.UseVisualStyleBackColor = true;
			this.restoreUsingFile.Click += new System.EventHandler(this.RestoreFromFile);
			// 
			// restoreFromDirector
			// 
			this.restoreFromDirector.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.restoreFromDirector.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.restoreFromDirector.Image = global:: Garry.Control4.Jailbreak.Properties.Resources.database;
			this.restoreFromDirector.Location = new System.Drawing.Point(238, 305);
			this.restoreFromDirector.Name = "restoreFromDirector";
			this.restoreFromDirector.Padding = new System.Windows.Forms.Padding(2);
			this.restoreFromDirector.Size = new System.Drawing.Size(176, 29);
			this.restoreFromDirector.TabIndex = 0;
			this.restoreFromDirector.Text = "Restore Backup";
			this.restoreFromDirector.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.restoreFromDirector.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.restoreFromDirector.UseVisualStyleBackColor = true;
			this.restoreFromDirector.Click += new System.EventHandler(this.RestoreFromBackup);
			// 
			// Restore
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label3);
			this.Controls.Add(this.restoreUsingFile);
			this.Controls.Add(this.backupList);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.restoreFromDirector);
			this.Name = "Restore";
			this.Size = new System.Drawing.Size(436, 355);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button restoreFromDirector;
		private System.Windows.Forms.ListBox backupList;
		private System.Windows.Forms.Button restoreUsingFile;
		private System.Windows.Forms.Label label3;
	}
}
