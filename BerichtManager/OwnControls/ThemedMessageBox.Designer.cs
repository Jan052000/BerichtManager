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
			this.components = new System.ComponentModel.Container();
			this.paText = new System.Windows.Forms.Panel();
			this.rtbText = new System.Windows.Forms.RichTextBox();
			this.btYes = new BerichtManager.OwnControls.FocusColoredFlatButton();
			this.btNo = new BerichtManager.OwnControls.FocusColoredFlatButton();
			this.btCancel = new BerichtManager.OwnControls.FocusColoredFlatButton();
			this.btCopyToClipboard = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.paText.SuspendLayout();
			this.SuspendLayout();
			// 
			// paText
			// 
			this.paText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.paText.AutoScroll = true;
			this.paText.Controls.Add(this.rtbText);
			this.paText.Location = new System.Drawing.Point(12, 12);
			this.paText.Name = "paText";
			this.paText.Size = new System.Drawing.Size(410, 173);
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
			this.rtbText.Size = new System.Drawing.Size(410, 173);
			this.rtbText.TabIndex = 2;
			this.rtbText.Text = "";
			// 
			// btYes
			// 
			this.btYes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btYes.Location = new System.Drawing.Point(185, 191);
			this.btYes.Name = "btYes";
			this.btYes.Size = new System.Drawing.Size(75, 23);
			this.btYes.TabIndex = 3;
			this.btYes.Text = "Yes";
			this.btYes.UseVisualStyleBackColor = true;
			// 
			// btNo
			// 
			this.btNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btNo.Location = new System.Drawing.Point(266, 191);
			this.btNo.Name = "btNo";
			this.btNo.Size = new System.Drawing.Size(75, 23);
			this.btNo.TabIndex = 1;
			this.btNo.Text = "No";
			this.btNo.UseVisualStyleBackColor = true;
			// 
			// btCancel
			// 
			this.btCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btCancel.Location = new System.Drawing.Point(347, 191);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(75, 23);
			this.btCancel.TabIndex = 0;
			this.btCancel.Text = "Cancel";
			this.btCancel.UseVisualStyleBackColor = true;
			// 
			// btCopyToClipboard
			// 
			this.btCopyToClipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btCopyToClipboard.Location = new System.Drawing.Point(12, 191);
			this.btCopyToClipboard.Name = "btCopyToClipboard";
			this.btCopyToClipboard.Size = new System.Drawing.Size(75, 23);
			this.btCopyToClipboard.TabIndex = 4;
			this.btCopyToClipboard.Text = "Copy";
			this.btCopyToClipboard.UseVisualStyleBackColor = true;
			this.btCopyToClipboard.Click += new System.EventHandler(this.CopyToClipboard);
			// 
			// ThemedMessageBox
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(434, 226);
			this.Controls.Add(this.btCopyToClipboard);
			this.Controls.Add(this.btYes);
			this.Controls.Add(this.paText);
			this.Controls.Add(this.btNo);
			this.Controls.Add(this.btCancel);
			this.MinimumSize = new System.Drawing.Size(99, 39);
			this.Name = "ThemedMessageBox";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ColoredMessageBox";
			this.Shown += new System.EventHandler(this.FocusButton);
			this.paText.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Panel paText;
		private System.Windows.Forms.RichTextBox rtbText;
		private FocusColoredFlatButton btCancel;
		private FocusColoredFlatButton btNo;
		private FocusColoredFlatButton btYes;
		private System.Windows.Forms.Button btCopyToClipboard;
		private System.Windows.Forms.ToolTip toolTip1;
	}
}