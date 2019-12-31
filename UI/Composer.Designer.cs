namespace  Garry.Control4.Jailbreak
{
	partial class Composer
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Composer));
			this.label4 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.linkFolder = new System.Windows.Forms.LinkLabel();
			this.linkOpenPatched = new System.Windows.Forms.LinkLabel();
			this.button3 = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.ForeColor = System.Drawing.SystemColors.Highlight;
			this.label4.Location = new System.Drawing.Point(13, 193);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(189, 18);
			this.label4.TabIndex = 3;
			this.label4.Text = "Patching Composer Pro";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.ForeColor = System.Drawing.SystemColors.Highlight;
			this.label2.Location = new System.Drawing.Point(13, 18);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(178, 18);
			this.label2.TabIndex = 7;
			this.label2.Text = "Getting Composer Pro";
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label5.Location = new System.Drawing.Point(13, 48);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(390, 57);
			this.label5.TabIndex = 8;
			this.label5.Text = "The Composer version should match the version of your system. The latest is 3.1.\r" +
    "\n\r\nIt doesn\'t feel right linking to Composer Pro downloads but they\'re quite eas" +
    "y to find if you know the filename. ";
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.Location = new System.Drawing.Point(13, 222);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(405, 54);
			this.label3.TabIndex = 4;
			this.label3.Text = resources.GetString("label3.Text");
			// 
			// linkFolder
			// 
			this.linkFolder.AutoSize = true;
			this.linkFolder.Location = new System.Drawing.Point(13, 337);
			this.linkFolder.Name = "linkFolder";
			this.linkFolder.Size = new System.Drawing.Size(115, 13);
			this.linkFolder.TabIndex = 11;
			this.linkFolder.TabStop = true;
			this.linkFolder.Text = "Open Composer Folder";
			this.linkFolder.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OpenComposerFolder);
			// 
			// linkOpenPatched
			// 
			this.linkOpenPatched.AutoSize = true;
			this.linkOpenPatched.Location = new System.Drawing.Point(303, 337);
			this.linkOpenPatched.Name = "linkOpenPatched";
			this.linkOpenPatched.Size = new System.Drawing.Size(126, 13);
			this.linkOpenPatched.TabIndex = 12;
			this.linkOpenPatched.TabStop = true;
			this.linkOpenPatched.Text = "Open Patched Composer";
			this.linkOpenPatched.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.StartModdedComposer);
			// 
			// button3
			// 
			this.button3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.button3.Image = global:: Garry.Control4.Jailbreak.Properties.Resources.reddit;
			this.button3.Location = new System.Drawing.Point(181, 127);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(93, 37);
			this.button3.TabIndex = 10;
			this.button3.Text = " r/c4diy";
			this.button3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.button3.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.OpenControl4Reddit);
			// 
			// button1
			// 
			this.button1.Image = global:: Garry.Control4.Jailbreak.Properties.Resources.loupe;
			this.button1.Location = new System.Drawing.Point(280, 127);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(123, 37);
			this.button1.TabIndex = 9;
			this.button1.Text = " Search Google";
			this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.button1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.SearchGoogleForComposer);
			// 
			// button2
			// 
			this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button2.AutoSize = true;
			this.button2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.button2.Image = global:: Garry.Control4.Jailbreak.Properties.Resources.patch;
			this.button2.Location = new System.Drawing.Point(250, 289);
			this.button2.Name = "button2";
			this.button2.Padding = new System.Windows.Forms.Padding(8);
			this.button2.Size = new System.Drawing.Size(153, 39);
			this.button2.TabIndex = 5;
			this.button2.Text = "  Patch Composer.exe";
			this.button2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.PatchComposer);
			// 
			// Composer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.linkOpenPatched);
			this.Controls.Add(this.linkFolder);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.label4);
			this.Name = "Composer";
			this.Size = new System.Drawing.Size(436, 361);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.LinkLabel linkFolder;
		private System.Windows.Forms.LinkLabel linkOpenPatched;
	}
}
