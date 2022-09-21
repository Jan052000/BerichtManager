﻿namespace BerichtManager.AddForm
{
	partial class EditForm
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
			this.btConfirm = new System.Windows.Forms.Button();
			this.rtInput = new System.Windows.Forms.RichTextBox();
			this.btQuit = new System.Windows.Forms.Button();
			this.nudFontSize = new System.Windows.Forms.NumericUpDown();
			this.cbFontFamily = new System.Windows.Forms.ComboBox();
			((System.ComponentModel.ISupportInitialize)(this.nudFontSize)).BeginInit();
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
			// btConfirm
			// 
			this.btConfirm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btConfirm.Location = new System.Drawing.Point(632, 415);
			this.btConfirm.Name = "btConfirm";
			this.btConfirm.Size = new System.Drawing.Size(75, 23);
			this.btConfirm.TabIndex = 1;
			this.btConfirm.Text = "Confirm";
			this.btConfirm.UseVisualStyleBackColor = true;
			this.btConfirm.Click += new System.EventHandler(this.btConfirm_Click);
			// 
			// rtInput
			// 
			this.rtInput.AcceptsTab = true;
			this.rtInput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.rtInput.Location = new System.Drawing.Point(12, 12);
			this.rtInput.Name = "rtInput";
			this.rtInput.Size = new System.Drawing.Size(776, 397);
			this.rtInput.TabIndex = 2;
			this.rtInput.Text = "";
			// 
			// btQuit
			// 
			this.btQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btQuit.Location = new System.Drawing.Point(551, 415);
			this.btQuit.Name = "btQuit";
			this.btQuit.Size = new System.Drawing.Size(75, 23);
			this.btQuit.TabIndex = 3;
			this.btQuit.Text = "Quit Edit";
			this.btQuit.UseVisualStyleBackColor = true;
			this.btQuit.Click += new System.EventHandler(this.btQuit_Click);
			// 
			// nudFontSize
			// 
			this.nudFontSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.nudFontSize.DecimalPlaces = 2;
			this.nudFontSize.Location = new System.Drawing.Point(498, 415);
			this.nudFontSize.Name = "nudFontSize";
			this.nudFontSize.Size = new System.Drawing.Size(47, 20);
			this.nudFontSize.TabIndex = 5;
			this.nudFontSize.ValueChanged += new System.EventHandler(this.nudFontSize_ValueChanged);
			// 
			// cbFontFamily
			// 
			this.cbFontFamily.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.cbFontFamily.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbFontFamily.FormattingEnabled = true;
			this.cbFontFamily.Location = new System.Drawing.Point(12, 415);
			this.cbFontFamily.Name = "cbFontFamily";
			this.cbFontFamily.Size = new System.Drawing.Size(480, 21);
			this.cbFontFamily.TabIndex = 6;
			this.cbFontFamily.SelectedValueChanged += new System.EventHandler(this.cbFontFamily_SelectedValueChanged);
			// 
			// EditForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.cbFontFamily);
			this.Controls.Add(this.nudFontSize);
			this.Controls.Add(this.btQuit);
			this.Controls.Add(this.rtInput);
			this.Controls.Add(this.btConfirm);
			this.Controls.Add(this.btClose);
			this.Name = "EditForm";
			this.Text = "EditForm";
			((System.ComponentModel.ISupportInitialize)(this.nudFontSize)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btClose;
		private System.Windows.Forms.Button btConfirm;
		private System.Windows.Forms.RichTextBox rtInput;
		private System.Windows.Forms.Button btQuit;
		private System.Windows.Forms.NumericUpDown nudFontSize;
		private System.Windows.Forms.ComboBox cbFontFamily;
	}
}