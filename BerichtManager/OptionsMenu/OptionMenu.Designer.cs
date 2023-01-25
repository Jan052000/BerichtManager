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
			this.tbCustomPrefix.TextChanged += new System.EventHandler(this.tbCustomPrefix_TextChanged);
			// 
			// OptionMenu
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.tbCustomPrefix);
			this.Controls.Add(this.cbUseCustomPrefix);
			this.Controls.Add(this.btSave);
			this.Controls.Add(this.btClose);
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
	}
}