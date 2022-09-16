namespace BerichtManager.AddForm
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
			this.btClose = new System.Windows.Forms.Button();
			this.btLogin = new System.Windows.Forms.Button();
			this.laLogin = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// laUsername
			// 
			this.laUsername.AutoSize = true;
			this.laUsername.Location = new System.Drawing.Point(28, 44);
			this.laUsername.Name = "laUsername";
			this.laUsername.Size = new System.Drawing.Size(55, 13);
			this.laUsername.TabIndex = 0;
			this.laUsername.Text = "Username";
			// 
			// tbUsername
			// 
			this.tbUsername.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbUsername.Location = new System.Drawing.Point(89, 41);
			this.tbUsername.Name = "tbUsername";
			this.tbUsername.Size = new System.Drawing.Size(411, 20);
			this.tbUsername.TabIndex = 1;
			// 
			// tbPassword
			// 
			this.tbPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbPassword.Location = new System.Drawing.Point(89, 67);
			this.tbPassword.Name = "tbPassword";
			this.tbPassword.Size = new System.Drawing.Size(411, 20);
			this.tbPassword.TabIndex = 2;
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
			this.btClose.Location = new System.Drawing.Point(444, 190);
			this.btClose.Name = "btClose";
			this.btClose.Size = new System.Drawing.Size(75, 23);
			this.btClose.TabIndex = 4;
			this.btClose.Text = "Close";
			this.btClose.UseVisualStyleBackColor = true;
			this.btClose.Click += new System.EventHandler(this.btClose_Click);
			// 
			// btLogin
			// 
			this.btLogin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btLogin.Location = new System.Drawing.Point(363, 190);
			this.btLogin.Name = "btLogin";
			this.btLogin.Size = new System.Drawing.Size(75, 23);
			this.btLogin.TabIndex = 5;
			this.btLogin.Text = "Login";
			this.btLogin.UseVisualStyleBackColor = true;
			this.btLogin.Click += new System.EventHandler(this.btLogin_Click);
			// 
			// laLogin
			// 
			this.laLogin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.laLogin.AutoSize = true;
			this.laLogin.Location = new System.Drawing.Point(220, 9);
			this.laLogin.Name = "laLogin";
			this.laLogin.Size = new System.Drawing.Size(93, 13);
			this.laLogin.TabIndex = 6;
			this.laLogin.Text = "Login to Webuntis";
			// 
			// Login
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(531, 225);
			this.Controls.Add(this.laLogin);
			this.Controls.Add(this.btLogin);
			this.Controls.Add(this.btClose);
			this.Controls.Add(this.laPassword);
			this.Controls.Add(this.tbPassword);
			this.Controls.Add(this.tbUsername);
			this.Controls.Add(this.laUsername);
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
		private System.Windows.Forms.Button btClose;
		private System.Windows.Forms.Button btLogin;
		private System.Windows.Forms.Label laLogin;
	}
}