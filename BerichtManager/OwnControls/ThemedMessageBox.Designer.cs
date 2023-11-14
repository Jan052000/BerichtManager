namespace BerichtManager.OwnControls
{
	partial class ThemedMessageBox
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
			this.btCancel = new System.Windows.Forms.Button();
			this.btNo = new System.Windows.Forms.Button();
			this.paText = new System.Windows.Forms.Panel();
			this.rtbText = new System.Windows.Forms.RichTextBox();
			this.btYes = new System.Windows.Forms.Button();
			this.paText.SuspendLayout();
			this.SuspendLayout();
			// 
			// btCancel
			// 
			this.btCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btCancel.Location = new System.Drawing.Point(281, 159);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(75, 20);
			this.btCancel.TabIndex = 0;
			this.btCancel.Text = "Cancel";
			this.btCancel.UseVisualStyleBackColor = true;
			// 
			// btNo
			// 
			this.btNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btNo.Location = new System.Drawing.Point(200, 159);
			this.btNo.Name = "btNo";
			this.btNo.Size = new System.Drawing.Size(75, 20);
			this.btNo.TabIndex = 1;
			this.btNo.Text = "No";
			this.btNo.UseVisualStyleBackColor = true;
			// 
			// paText
			// 
			this.paText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.paText.Controls.Add(this.rtbText);
			this.paText.Location = new System.Drawing.Point(12, 12);
			this.paText.Name = "paText";
			this.paText.Size = new System.Drawing.Size(344, 138);
			this.paText.TabIndex = 2;
			// 
			// rtbText
			// 
			this.rtbText.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.rtbText.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtbText.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rtbText.Location = new System.Drawing.Point(0, 0);
			this.rtbText.Name = "rtbText";
			this.rtbText.ReadOnly = true;
			this.rtbText.Size = new System.Drawing.Size(344, 138);
			this.rtbText.TabIndex = 2;
			this.rtbText.Text = "";
			// 
			// btYes
			// 
			this.btYes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btYes.Location = new System.Drawing.Point(119, 159);
			this.btYes.Name = "btYes";
			this.btYes.Size = new System.Drawing.Size(75, 20);
			this.btYes.TabIndex = 3;
			this.btYes.Text = "Yes";
			this.btYes.UseVisualStyleBackColor = true;
			// 
			// ThemedMessageBox
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(368, 191);
			this.Controls.Add(this.btYes);
			this.Controls.Add(this.paText);
			this.Controls.Add(this.btNo);
			this.Controls.Add(this.btCancel);
			this.MaximumSize = new System.Drawing.Size(600, 500);
			this.Name = "ThemedMessageBox";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ColoredMessageBox";
			this.paText.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.Button btNo;
		private System.Windows.Forms.Panel paText;
		private System.Windows.Forms.RichTextBox rtbText;
		private System.Windows.Forms.Button btYes;
	}
}