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
			this.btLogin = new System.Windows.Forms.Button();
			this.laTemplate = new System.Windows.Forms.Label();
			this.tbTemplate = new System.Windows.Forms.TextBox();
			this.laName = new System.Windows.Forms.Label();
			this.tbName = new System.Windows.Forms.TextBox();
			this.laNumber = new System.Windows.Forms.Label();
			this.nudNumber = new System.Windows.Forms.NumericUpDown();
			this.laTheme = new System.Windows.Forms.Label();
			this.coTheme = new BerichtManager.OwnControls.ColoredComboBox();
			this.btCreateTheme = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.nudNumber)).BeginInit();
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
			// btLogin
			// 
			this.btLogin.Location = new System.Drawing.Point(230, 263);
			this.btLogin.Name = "btLogin";
			this.btLogin.Size = new System.Drawing.Size(75, 21);
			this.btLogin.TabIndex = 15;
			this.btLogin.Text = "Login";
			this.btLogin.UseVisualStyleBackColor = true;
			this.btLogin.Click += new System.EventHandler(this.btLogin_Click);
			// 
			// laTemplate
			// 
			this.laTemplate.AutoSize = true;
			this.laTemplate.Location = new System.Drawing.Point(47, 212);
			this.laTemplate.Name = "laTemplate";
			this.laTemplate.Size = new System.Drawing.Size(51, 13);
			this.laTemplate.TabIndex = 16;
			this.laTemplate.Text = "Template";
			// 
			// tbTemplate
			// 
			this.tbTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbTemplate.Location = new System.Drawing.Point(104, 209);
			this.tbTemplate.Name = "tbTemplate";
			this.tbTemplate.Size = new System.Drawing.Size(684, 20);
			this.tbTemplate.TabIndex = 17;
			this.tbTemplate.Click += new System.EventHandler(this.tbTemplate_Click);
			// 
			// laName
			// 
			this.laName.AutoSize = true;
			this.laName.Location = new System.Drawing.Point(63, 240);
			this.laName.Name = "laName";
			this.laName.Size = new System.Drawing.Size(35, 13);
			this.laName.TabIndex = 18;
			this.laName.Text = "Name";
			// 
			// tbName
			// 
			this.tbName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbName.Location = new System.Drawing.Point(104, 237);
			this.tbName.Name = "tbName";
			this.tbName.Size = new System.Drawing.Size(684, 20);
			this.tbName.TabIndex = 19;
			this.tbName.TextChanged += new System.EventHandler(this.MarkAsDirty);
			// 
			// laNumber
			// 
			this.laNumber.AutoSize = true;
			this.laNumber.Location = new System.Drawing.Point(54, 267);
			this.laNumber.Name = "laNumber";
			this.laNumber.Size = new System.Drawing.Size(44, 13);
			this.laNumber.TabIndex = 20;
			this.laNumber.Text = "Number";
			// 
			// nudNumber
			// 
			this.nudNumber.Location = new System.Drawing.Point(104, 263);
			this.nudNumber.Name = "nudNumber";
			this.nudNumber.Size = new System.Drawing.Size(120, 20);
			this.nudNumber.TabIndex = 21;
			this.nudNumber.ValueChanged += new System.EventHandler(this.MarkAsDirty);
			// 
			// laTheme
			// 
			this.laTheme.AutoSize = true;
			this.laTheme.Location = new System.Drawing.Point(58, 185);
			this.laTheme.Name = "laTheme";
			this.laTheme.Size = new System.Drawing.Size(40, 13);
			this.laTheme.TabIndex = 22;
			this.laTheme.Text = "Theme";
			// 
			// coTheme
			// 
			this.coTheme.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.coTheme.ArrowColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
			this.coTheme.BorderColor = System.Drawing.SystemColors.Window;
			this.coTheme.DisabledColor = System.Drawing.SystemColors.Control;
			this.coTheme.DisabledTextColor = System.Drawing.SystemColors.GrayText;
			this.coTheme.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.coTheme.DropDownButtonColor = System.Drawing.SystemColors.Menu;
			this.coTheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.coTheme.FormattingEnabled = true;
			this.coTheme.HighlightColor = System.Drawing.SystemColors.Highlight;
			this.coTheme.Location = new System.Drawing.Point(104, 182);
			this.coTheme.Name = "coTheme";
			this.coTheme.OutlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
			this.coTheme.Size = new System.Drawing.Size(598, 21);
			this.coTheme.TabIndex = 23;
			this.coTheme.TextColor = System.Drawing.SystemColors.WindowText;
			this.coTheme.SelectedIndexChanged += new System.EventHandler(this.MarkAsDirty);
			// 
			// btCreateTheme
			// 
			this.btCreateTheme.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btCreateTheme.Location = new System.Drawing.Point(708, 182);
			this.btCreateTheme.Name = "btCreateTheme";
			this.btCreateTheme.Size = new System.Drawing.Size(80, 21);
			this.btCreateTheme.TabIndex = 24;
			this.btCreateTheme.Text = "Create theme";
			this.btCreateTheme.UseVisualStyleBackColor = true;
			this.btCreateTheme.Click += new System.EventHandler(this.btCreateTheme_Click);
			// 
			// OptionMenu
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.btCreateTheme);
			this.Controls.Add(this.coTheme);
			this.Controls.Add(this.laTheme);
			this.Controls.Add(this.nudNumber);
			this.Controls.Add(this.laNumber);
			this.Controls.Add(this.tbName);
			this.Controls.Add(this.laName);
			this.Controls.Add(this.tbTemplate);
			this.Controls.Add(this.laTemplate);
			this.Controls.Add(this.btLogin);
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
			this.MinimumSize = new System.Drawing.Size(495, 321);
			this.Name = "OptionMenu";
			this.Text = "OptionMenu";
			((System.ComponentModel.ISupportInitialize)(this.nudNumber)).EndInit();
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
		private System.Windows.Forms.Button btLogin;
		private System.Windows.Forms.Label laTemplate;
		private System.Windows.Forms.TextBox tbTemplate;
		private System.Windows.Forms.Label laName;
		private System.Windows.Forms.TextBox tbName;
		private System.Windows.Forms.Label laNumber;
		private System.Windows.Forms.NumericUpDown nudNumber;
		private System.Windows.Forms.Label laTheme;
		private OwnControls.ColoredComboBox coTheme;
		private System.Windows.Forms.Button btCreateTheme;
	}
}