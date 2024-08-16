namespace BerichtManager.Forms
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
			this.components = new System.ComponentModel.Container();
			this.btSkip = new BerichtManager.OwnControls.FocusColoredFlatButton();
			this.btConfirm = new BerichtManager.OwnControls.FocusColoredFlatButton();
			this.rtInput = new System.Windows.Forms.RichTextBox();
			this.btQuit = new BerichtManager.OwnControls.FocusColoredFlatButton();
			this.nudFontSize = new System.Windows.Forms.NumericUpDown();
			this.cbEditorFont = new System.Windows.Forms.CheckBox();
			this.btSaveAndQuit = new BerichtManager.OwnControls.FocusColoredFlatButton();
			this.ttTips = new System.Windows.Forms.ToolTip(this.components);
			this.cbFontFamily = new BerichtManager.OwnControls.ColoredComboBox();
			((System.ComponentModel.ISupportInitialize)(this.nudFontSize)).BeginInit();
			this.SuspendLayout();
			// 
			// btClose
			// 
			this.btSkip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btSkip.Location = new System.Drawing.Point(713, 415);
			this.btSkip.Name = "btClose";
			this.btSkip.Size = new System.Drawing.Size(75, 23);
			this.btSkip.TabIndex = 0;
			this.btSkip.Text = "Skip";
			this.ttTips.SetToolTip(this.btSkip, "Saves input and closes this window");
			this.btSkip.UseVisualStyleBackColor = true;
			this.btSkip.Click += new System.EventHandler(this.btSkip_Click);
			// 
			// btConfirm
			// 
			this.btConfirm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btConfirm.Location = new System.Drawing.Point(460, 415);
			this.btConfirm.Name = "btConfirm";
			this.btConfirm.Size = new System.Drawing.Size(75, 23);
			this.btConfirm.TabIndex = 1;
			this.btConfirm.Text = "Confirm";
			this.ttTips.SetToolTip(this.btConfirm, "Saves input");
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
			this.btQuit.Location = new System.Drawing.Point(632, 415);
			this.btQuit.Name = "btQuit";
			this.btQuit.Size = new System.Drawing.Size(75, 23);
			this.btQuit.TabIndex = 3;
			this.btQuit.Text = "Quit Edit";
			this.ttTips.SetToolTip(this.btQuit, "Aborts edit");
			this.btQuit.UseVisualStyleBackColor = true;
			this.btQuit.Click += new System.EventHandler(this.btQuit_Click);
			// 
			// nudFontSize
			// 
			this.nudFontSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.nudFontSize.DecimalPlaces = 2;
			this.nudFontSize.Location = new System.Drawing.Point(390, 415);
			this.nudFontSize.Name = "nudFontSize";
			this.nudFontSize.Size = new System.Drawing.Size(64, 20);
			this.nudFontSize.TabIndex = 5;
			this.ttTips.SetToolTip(this.nudFontSize, "Changes local font size");
			this.nudFontSize.ValueChanged += new System.EventHandler(this.nudFontSize_ValueChanged);
			// 
			// cbEditorFont
			// 
			this.cbEditorFont.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cbEditorFont.AutoSize = true;
			this.cbEditorFont.Location = new System.Drawing.Point(12, 417);
			this.cbEditorFont.Name = "cbEditorFont";
			this.cbEditorFont.Size = new System.Drawing.Size(77, 17);
			this.cbEditorFont.TabIndex = 7;
			this.cbEditorFont.Text = "Editor Font";
			this.ttTips.SetToolTip(this.cbEditorFont, "Toggles if the selected font should be used");
			this.cbEditorFont.UseVisualStyleBackColor = true;
			this.cbEditorFont.CheckedChanged += new System.EventHandler(this.cbEditorFont_CheckedChanged);
			// 
			// btSaveAndQuit
			// 
			this.btSaveAndQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btSaveAndQuit.Location = new System.Drawing.Point(541, 415);
			this.btSaveAndQuit.Name = "btSaveAndQuit";
			this.btSaveAndQuit.Size = new System.Drawing.Size(85, 23);
			this.btSaveAndQuit.TabIndex = 8;
			this.btSaveAndQuit.Text = "Save and Quit";
			this.ttTips.SetToolTip(this.btSaveAndQuit, "Saves input and closes this window");
			this.btSaveAndQuit.UseVisualStyleBackColor = true;
			this.btSaveAndQuit.Click += new System.EventHandler(this.btSaveAndQuit_Click);
			// 
			// cbFontFamily
			// 
			this.cbFontFamily.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.cbFontFamily.ArrowColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
			this.cbFontFamily.BorderColor = System.Drawing.SystemColors.Window;
			this.cbFontFamily.DisabledColor = System.Drawing.SystemColors.Control;
			this.cbFontFamily.DisabledTextColor = System.Drawing.SystemColors.GrayText;
			this.cbFontFamily.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.cbFontFamily.DropDownButtonColor = System.Drawing.SystemColors.Menu;
			this.cbFontFamily.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbFontFamily.FormattingEnabled = true;
			this.cbFontFamily.HighlightColor = System.Drawing.SystemColors.Highlight;
			this.cbFontFamily.Location = new System.Drawing.Point(95, 415);
			this.cbFontFamily.Name = "cbFontFamily";
			this.cbFontFamily.OutlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
			this.cbFontFamily.Size = new System.Drawing.Size(289, 21);
			this.cbFontFamily.TabIndex = 6;
			this.cbFontFamily.TextColor = System.Drawing.SystemColors.WindowText;
			this.ttTips.SetToolTip(this.cbFontFamily, "Changes font in editor and report");
			this.cbFontFamily.SelectedValueChanged += new System.EventHandler(this.cbFontFamily_SelectedValueChanged);
			// 
			// EditForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.btSaveAndQuit);
			this.Controls.Add(this.cbEditorFont);
			this.Controls.Add(this.cbFontFamily);
			this.Controls.Add(this.nudFontSize);
			this.Controls.Add(this.btQuit);
			this.Controls.Add(this.rtInput);
			this.Controls.Add(this.btConfirm);
			this.Controls.Add(this.btSkip);
			this.MinimumSize = new System.Drawing.Size(698, 39);
			this.Name = "EditForm";
			this.Text = "EditForm";
			((System.ComponentModel.ISupportInitialize)(this.nudFontSize)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OwnControls.FocusColoredFlatButton btSkip;
		private OwnControls.FocusColoredFlatButton btConfirm;
		private System.Windows.Forms.RichTextBox rtInput;
		private OwnControls.FocusColoredFlatButton btQuit;
		private System.Windows.Forms.NumericUpDown nudFontSize;
		private OwnControls.ColoredComboBox cbFontFamily;
		private System.Windows.Forms.CheckBox cbEditorFont;
		private OwnControls.FocusColoredFlatButton btSaveAndQuit;
		private System.Windows.Forms.ToolTip ttTips;
	}
}