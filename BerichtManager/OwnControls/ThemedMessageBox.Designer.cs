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
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.paButtons = new System.Windows.Forms.Panel();
			this.btCopyToClipboard = new BerichtManager.OwnControls.FocusColoredFlatButton();
			this.btYes = new BerichtManager.OwnControls.FocusColoredFlatButton();
			this.btCancel = new BerichtManager.OwnControls.FocusColoredFlatButton();
			this.btNo = new BerichtManager.OwnControls.FocusColoredFlatButton();
			this.paText = new System.Windows.Forms.Panel();
			this.rtbText = new System.Windows.Forms.RichTextBox();
			this.paButtons.SuspendLayout();
			this.paText.SuspendLayout();
			this.SuspendLayout();
			// 
			// paButtons
			// 
			this.paButtons.Controls.Add(this.btCopyToClipboard);
			this.paButtons.Controls.Add(this.btYes);
			this.paButtons.Controls.Add(this.btCancel);
			this.paButtons.Controls.Add(this.btNo);
			this.paButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.paButtons.Location = new System.Drawing.Point(0, 197);
			this.paButtons.Name = "paButtons";
			this.paButtons.Size = new System.Drawing.Size(434, 29);
			this.paButtons.TabIndex = 0;
			// 
			// btCopyToClipboard
			// 
			this.btCopyToClipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btCopyToClipboard.Location = new System.Drawing.Point(12, 3);
			this.btCopyToClipboard.Name = "btCopyToClipboard";
			this.btCopyToClipboard.Size = new System.Drawing.Size(75, 23);
			this.btCopyToClipboard.TabIndex = 0;
			this.btCopyToClipboard.Text = "Copy";
			this.btCopyToClipboard.UseVisualStyleBackColor = true;
			this.btCopyToClipboard.Click += new System.EventHandler(this.CopyToClipboard);
			// 
			// btYes
			// 
			this.btYes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btYes.Location = new System.Drawing.Point(185, 3);
			this.btYes.Name = "btYes";
			this.btYes.Size = new System.Drawing.Size(75, 23);
			this.btYes.TabIndex = 1;
			this.btYes.Text = "Yes";
			this.btYes.UseVisualStyleBackColor = true;
			// 
			// btCancel
			// 
			this.btCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btCancel.Location = new System.Drawing.Point(347, 3);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(75, 23);
			this.btCancel.TabIndex = 3;
			this.btCancel.Text = "Cancel";
			this.btCancel.UseVisualStyleBackColor = true;
			// 
			// btNo
			// 
			this.btNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btNo.Location = new System.Drawing.Point(266, 3);
			this.btNo.Name = "btNo";
			this.btNo.Size = new System.Drawing.Size(75, 23);
			this.btNo.TabIndex = 2;
			this.btNo.Text = "No";
			this.btNo.UseVisualStyleBackColor = true;
			// 
			// paText
			// 
			this.paText.AutoScroll = true;
			this.paText.Controls.Add(this.rtbText);
			this.paText.Dock = System.Windows.Forms.DockStyle.Fill;
			this.paText.Location = new System.Drawing.Point(0, 0);
			this.paText.Name = "paText";
			this.paText.Padding = new System.Windows.Forms.Padding(12, 12, 12, 0);
			this.paText.Size = new System.Drawing.Size(434, 197);
			this.paText.TabIndex = 1;
			// 
			// rtbText
			// 
			this.rtbText.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.rtbText.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtbText.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rtbText.Location = new System.Drawing.Point(12, 12);
			this.rtbText.Name = "rtbText";
			this.rtbText.ReadOnly = true;
			this.rtbText.Size = new System.Drawing.Size(410, 185);
			this.rtbText.TabIndex = 0;
			this.rtbText.TabStop = false;
			this.rtbText.Text = "";
			// 
			// ThemedMessageBox
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(434, 226);
			this.Controls.Add(this.paText);
			this.Controls.Add(this.paButtons);
			this.MinimumSize = new System.Drawing.Size(99, 39);
			this.Name = "ThemedMessageBox";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ColoredMessageBox";
			this.Shown += new System.EventHandler(this.FocusButton);
			this.paButtons.ResumeLayout(false);
			this.paText.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private FocusColoredFlatButton btCancel;
		private FocusColoredFlatButton btNo;
		private FocusColoredFlatButton btYes;
		private OwnControls.FocusColoredFlatButton btCopyToClipboard;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Panel paButtons;
		private System.Windows.Forms.Panel paText;
		private System.Windows.Forms.RichTextBox rtbText;
	}
}