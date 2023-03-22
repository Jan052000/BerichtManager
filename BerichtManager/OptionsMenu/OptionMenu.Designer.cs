namespace BerichtManager.OptionsMenu
{
	partial class OptionMenu
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
			this.btClose = new System.Windows.Forms.Button();
			this.btSave = new System.Windows.Forms.Button();
			this.cbUseCustomPrefix = new System.Windows.Forms.CheckBox();
			this.tbCustomPrefix = new System.Windows.Forms.TextBox();
			this.laServer = new System.Windows.Forms.Label();
			this.tbServer = new System.Windows.Forms.TextBox();
			this.tbSchool = new System.Windows.Forms.TextBox();
			this.laSchool = new System.Windows.Forms.Label();
			this.cbShouldUseUntis = new System.Windows.Forms.CheckBox();
			this.cbEndOfWeek = new System.Windows.Forms.CheckBox();
			this.cbLegacyEdit = new System.Windows.Forms.CheckBox();
			this.cbUseDarkMode = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// btClose
			// 
			this.btClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btClose.Location = new System.Drawing.Point(713, 415);
			this.btClose.Name = "btClose";
			this.btClose.Size = new System.Drawing.Size(75, 23);
			this.btClose.TabIndex = 0;
			this.btClose.Text = "Close";
			this.btClose.UseVisualStyleBackColor = true;
			this.btClose.Click += new System.EventHandler(this.btClose_Click);
			// 
			// btSave
			// 
			this.btSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btSave.Location = new System.Drawing.Point(632, 415);
			this.btSave.Name = "btSave";
			this.btSave.Size = new System.Drawing.Size(75, 23);
			this.btSave.TabIndex = 1;
			this.btSave.Text = "Save";
			this.btSave.UseVisualStyleBackColor = true;
			this.btSave.Click += new System.EventHandler(this.btSave_Click);
			// 
			// cbUseCustomPrefix
			// 
			this.cbUseCustomPrefix.AutoSize = true;
			this.cbUseCustomPrefix.Location = new System.Drawing.Point(12, 12);
			this.cbUseCustomPrefix.Name = "cbUseCustomPrefix";
			this.cbUseCustomPrefix.Size = new System.Drawing.Size(110, 17);
			this.cbUseCustomPrefix.TabIndex = 2;
			this.cbUseCustomPrefix.Text = "Use custom prefix";
			this.cbUseCustomPrefix.UseVisualStyleBackColor = true;
			this.cbUseCustomPrefix.CheckedChanged += new System.EventHandler(this.cbUseCustomPrefix_CheckedChanged);
			// 
			// tbCustomPrefix
			// 
			this.tbCustomPrefix.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbCustomPrefix.Enabled = false;
			this.tbCustomPrefix.Location = new System.Drawing.Point(12, 35);
			this.tbCustomPrefix.Name = "tbCustomPrefix";
			this.tbCustomPrefix.Size = new System.Drawing.Size(776, 20);
			this.tbCustomPrefix.TabIndex = 3;
			this.tbCustomPrefix.TextChanged += new System.EventHandler(this.MarkAsDirty);
			// 
			// laServer
			// 
			this.laServer.AutoSize = true;
			this.laServer.Location = new System.Drawing.Point(12, 87);
			this.laServer.Name = "laServer";
			this.laServer.Size = new System.Drawing.Size(86, 13);
			this.laServer.TabIndex = 4;
			this.laServer.Text = "Webuntis Server";
			// 
			// tbServer
			// 
			this.tbServer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbServer.Location = new System.Drawing.Point(104, 84);
			this.tbServer.Name = "tbServer";
			this.tbServer.Size = new System.Drawing.Size(684, 20);
			this.tbServer.TabIndex = 5;
			this.tbServer.TextChanged += new System.EventHandler(this.MarkAsDirty);
			// 
			// tbSchool
			// 
			this.tbSchool.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbSchool.Location = new System.Drawing.Point(104, 110);
			this.tbSchool.Name = "tbSchool";
			this.tbSchool.Size = new System.Drawing.Size(684, 20);
			this.tbSchool.TabIndex = 6;
			this.tbSchool.TextChanged += new System.EventHandler(this.MarkAsDirty);
			// 
			// laSchool
			// 
			this.laSchool.AutoSize = true;
			this.laSchool.Location = new System.Drawing.Point(29, 113);
			this.laSchool.Name = "laSchool";
			this.laSchool.Size = new System.Drawing.Size(69, 13);
			this.laSchool.TabIndex = 7;
			this.laSchool.Text = "School name";
			// 
			// cbShouldUseUntis
			// 
			this.cbShouldUseUntis.AutoSize = true;
			this.cbShouldUseUntis.Location = new System.Drawing.Point(12, 61);
			this.cbShouldUseUntis.Name = "cbShouldUseUntis";
			this.cbShouldUseUntis.Size = new System.Drawing.Size(95, 17);
			this.cbShouldUseUntis.TabIndex = 8;
			this.cbShouldUseUntis.Text = "Use WebUntis";
			this.cbShouldUseUntis.UseVisualStyleBackColor = true;
			this.cbShouldUseUntis.CheckedChanged += new System.EventHandler(this.cbShouldUseUntis_CheckedChanged);
			// 
			// cbEndOfWeek
			// 
			this.cbEndOfWeek.AutoSize = true;
			this.cbEndOfWeek.Location = new System.Drawing.Point(12, 136);
			this.cbEndOfWeek.Name = "cbEndOfWeek";
			this.cbEndOfWeek.Size = new System.Drawing.Size(117, 17);
			this.cbEndOfWeek.TabIndex = 9;
			this.cbEndOfWeek.Text = "End week on friday";
			this.cbEndOfWeek.UseVisualStyleBackColor = true;
			this.cbEndOfWeek.CheckedChanged += new System.EventHandler(this.MarkAsDirty);
			// 
			// cbLegacyEdit
			// 
			this.cbLegacyEdit.AutoSize = true;
			this.cbLegacyEdit.Location = new System.Drawing.Point(12, 159);
			this.cbLegacyEdit.Name = "cbLegacyEdit";
			this.cbLegacyEdit.Size = new System.Drawing.Size(99, 17);
			this.cbLegacyEdit.TabIndex = 10;
			this.cbLegacyEdit.Text = "Use legacy edit";
			this.cbLegacyEdit.UseVisualStyleBackColor = true;
			this.cbLegacyEdit.CheckedChanged += new System.EventHandler(this.MarkAsDirty);
			// 
			// cbUseDarkMode
			// 
			this.cbUseDarkMode.AutoSize = true;
			this.cbUseDarkMode.Location = new System.Drawing.Point(12, 182);
			this.cbUseDarkMode.Name = "cbUseDarkMode";
			this.cbUseDarkMode.Size = new System.Drawing.Size(97, 17);
			this.cbUseDarkMode.TabIndex = 11;
			this.cbUseDarkMode.Text = "Use Darkmode";
			this.cbUseDarkMode.UseVisualStyleBackColor = true;
			this.cbUseDarkMode.CheckedChanged += new System.EventHandler(this.MarkAsDirty);
			// 
			// OptionMenu
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.cbUseDarkMode);
			this.Controls.Add(this.cbLegacyEdit);
			this.Controls.Add(this.cbEndOfWeek);
			this.Controls.Add(this.cbShouldUseUntis);
			this.Controls.Add(this.laSchool);
			this.Controls.Add(this.tbSchool);
			this.Controls.Add(this.tbServer);
			this.Controls.Add(this.laServer);
			this.Controls.Add(this.tbCustomPrefix);
			this.Controls.Add(this.cbUseCustomPrefix);
			this.Controls.Add(this.btSave);
			this.Controls.Add(this.btClose);
			this.MinimumSize = new System.Drawing.Size(195, 232);
			this.Name = "OptionMenu";
			this.Text = "OptionMenu";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btClose;
		private System.Windows.Forms.Button btSave;
		private System.Windows.Forms.CheckBox cbUseCustomPrefix;
		private System.Windows.Forms.TextBox tbCustomPrefix;
		private System.Windows.Forms.Label laServer;
		private System.Windows.Forms.TextBox tbServer;
		private System.Windows.Forms.TextBox tbSchool;
		private System.Windows.Forms.Label laSchool;
		private System.Windows.Forms.CheckBox cbShouldUseUntis;
		private System.Windows.Forms.CheckBox cbEndOfWeek;
		private System.Windows.Forms.CheckBox cbLegacyEdit;
		private System.Windows.Forms.CheckBox cbUseDarkMode;
	}
}