﻿namespace BerichtManager.Forms
{
	partial class Login
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
			this.laUsername = new System.Windows.Forms.Label();
			this.tbUsername = new System.Windows.Forms.TextBox();
			this.tbPassword = new System.Windows.Forms.TextBox();
			this.laPassword = new System.Windows.Forms.Label();
			this.btClose = new BerichtManager.OwnControls.FocusColoredFlatButton();
			this.btLogin = new BerichtManager.OwnControls.FocusColoredFlatButton();
			this.laLogin = new System.Windows.Forms.Label();
			this.cbKeepLogin = new System.Windows.Forms.CheckBox();
			this.cbShowPassword = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// laUsername
			// 
			this.laUsername.AutoSize = true;
			this.laUsername.Location = new System.Drawing.Point(28, 44);
			this.laUsername.Name = "laUsername";
			this.laUsername.Size = new System.Drawing.Size(55, 13);
			this.laUsername.TabIndex = 1;
			this.laUsername.Text = "Username";
			// 
			// tbUsername
			// 
			this.tbUsername.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbUsername.Location = new System.Drawing.Point(89, 41);
			this.tbUsername.Name = "tbUsername";
			this.tbUsername.Size = new System.Drawing.Size(411, 20);
			this.tbUsername.TabIndex = 2;
			// 
			// tbPassword
			// 
			this.tbPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbPassword.Location = new System.Drawing.Point(89, 67);
			this.tbPassword.Name = "tbPassword";
			this.tbPassword.Size = new System.Drawing.Size(411, 20);
			this.tbPassword.TabIndex = 4;
			this.tbPassword.UseSystemPasswordChar = true;
			// 
			// laPassword
			// 
			this.laPassword.AutoSize = true;
			this.laPassword.Location = new System.Drawing.Point(30, 70);
			this.laPassword.Name = "laPassword";
			this.laPassword.Size = new System.Drawing.Size(53, 13);
			this.laPassword.TabIndex = 3;
			this.laPassword.Text = "Password";
			// 
			// btClose
			// 
			this.btClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btClose.Location = new System.Drawing.Point(444, 116);
			this.btClose.Name = "btClose";
			this.btClose.Size = new System.Drawing.Size(75, 23);
			this.btClose.TabIndex = 8;
			this.btClose.Text = "Close";
			this.btClose.UseVisualStyleBackColor = true;
			this.btClose.Click += new System.EventHandler(this.btClose_Click);
			// 
			// btLogin
			// 
			this.btLogin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btLogin.Location = new System.Drawing.Point(363, 116);
			this.btLogin.Name = "btLogin";
			this.btLogin.Size = new System.Drawing.Size(75, 23);
			this.btLogin.TabIndex = 7;
			this.btLogin.Text = "Login";
			this.btLogin.UseVisualStyleBackColor = true;
			this.btLogin.Click += new System.EventHandler(this.btLogin_Click);
			// 
			// laLogin
			// 
			this.laLogin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.laLogin.AutoSize = true;
			this.laLogin.Location = new System.Drawing.Point(201, 9);
			this.laLogin.Name = "laLogin";
			this.laLogin.Size = new System.Drawing.Size(112, 13);
			this.laLogin.TabIndex = 0;
			this.laLogin.Text = "Login to {Placeholder}";
			// 
			// cbKeepLogin
			// 
			this.cbKeepLogin.AutoSize = true;
			this.cbKeepLogin.Location = new System.Drawing.Point(89, 93);
			this.cbKeepLogin.Name = "cbKeepLogin";
			this.cbKeepLogin.Size = new System.Drawing.Size(93, 17);
			this.cbKeepLogin.TabIndex = 5;
			this.cbKeepLogin.Text = "Stay logged in";
			this.cbKeepLogin.UseVisualStyleBackColor = true;
			// 
			// cbShowPassword
			// 
			this.cbShowPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cbShowPassword.AutoSize = true;
			this.cbShowPassword.Location = new System.Drawing.Point(398, 93);
			this.cbShowPassword.Name = "cbShowPassword";
			this.cbShowPassword.Size = new System.Drawing.Size(102, 17);
			this.cbShowPassword.TabIndex = 6;
			this.cbShowPassword.Text = "Show Password";
			this.cbShowPassword.UseVisualStyleBackColor = true;
			this.cbShowPassword.CheckedChanged += new System.EventHandler(this.cbShowPassword_CheckedChanged);
			// 
			// Login
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(531, 151);
			this.Controls.Add(this.cbShowPassword);
			this.Controls.Add(this.cbKeepLogin);
			this.Controls.Add(this.laLogin);
			this.Controls.Add(this.btLogin);
			this.Controls.Add(this.btClose);
			this.Controls.Add(this.laPassword);
			this.Controls.Add(this.tbPassword);
			this.Controls.Add(this.tbUsername);
			this.Controls.Add(this.laUsername);
			this.MinimumSize = new System.Drawing.Size(333, 186);
			this.Name = "Login";
			this.Text = "Login";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label laUsername;
		private System.Windows.Forms.TextBox tbUsername;
		private System.Windows.Forms.TextBox tbPassword;
		private System.Windows.Forms.Label laPassword;
		private System.Windows.Forms.Label laLogin;
		private System.Windows.Forms.CheckBox cbKeepLogin;
		private System.Windows.Forms.CheckBox cbShowPassword;
		private OwnControls.FocusColoredFlatButton btClose;
		private OwnControls.FocusColoredFlatButton btLogin;
	}
}