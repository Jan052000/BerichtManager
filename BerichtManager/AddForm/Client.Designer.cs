namespace BerichtManager.AddForm
{
	partial class Client
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
			this.btTest = new System.Windows.Forms.Button();
			this.webBrowser1 = new System.Windows.Forms.WebBrowser();
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
			// btTest
			// 
			this.btTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btTest.Location = new System.Drawing.Point(632, 415);
			this.btTest.Name = "btTest";
			this.btTest.Size = new System.Drawing.Size(75, 23);
			this.btTest.TabIndex = 1;
			this.btTest.Text = "Test";
			this.btTest.UseVisualStyleBackColor = true;
			this.btTest.Click += new System.EventHandler(this.btTest_Click);
			// 
			// webBrowser1
			// 
			this.webBrowser1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.webBrowser1.Location = new System.Drawing.Point(12, 12);
			this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowser1.Name = "webBrowser1";
			this.webBrowser1.Size = new System.Drawing.Size(776, 397);
			this.webBrowser1.TabIndex = 2;
			this.webBrowser1.Url = new System.Uri("", System.UriKind.Relative);
			// 
			// Client
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.webBrowser1);
			this.Controls.Add(this.btTest);
			this.Controls.Add(this.btClose);
			this.Name = "Client";
			this.Text = "Client";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btClose;
		private System.Windows.Forms.Button btTest;
		private System.Windows.Forms.WebBrowser webBrowser1;
	}
}