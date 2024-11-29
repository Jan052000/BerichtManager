namespace BerichtManager.Forms
{
	partial class SelectEditFrom
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
			this.cbEditName = new System.Windows.Forms.CheckBox();
			this.cbEditNumber = new System.Windows.Forms.CheckBox();
			this.cbEditStartDate = new System.Windows.Forms.CheckBox();
			this.cbEditEndDate = new System.Windows.Forms.CheckBox();
			this.cbEditYear = new System.Windows.Forms.CheckBox();
			this.cbEditWork = new System.Windows.Forms.CheckBox();
			this.cbEditSeminars = new System.Windows.Forms.CheckBox();
			this.cbEditSchool = new System.Windows.Forms.CheckBox();
			this.cbEditSignY = new System.Windows.Forms.CheckBox();
			this.cbEditSign = new System.Windows.Forms.CheckBox();
			this.btConfirm = new BerichtManager.OwnControls.FocusColoredFlatButton();
			this.laInstructions = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// cbEditName
			// 
			this.cbEditName.AutoSize = true;
			this.cbEditName.Location = new System.Drawing.Point(15, 25);
			this.cbEditName.Name = "cbEditName";
			this.cbEditName.Size = new System.Drawing.Size(75, 17);
			this.cbEditName.TabIndex = 1;
			this.cbEditName.Text = "Edit Name";
			this.cbEditName.UseVisualStyleBackColor = true;
			this.cbEditName.CheckedChanged += new System.EventHandler(this.cbChecked_Changed);
			// 
			// cbEditNumber
			// 
			this.cbEditNumber.AutoSize = true;
			this.cbEditNumber.Location = new System.Drawing.Point(15, 48);
			this.cbEditNumber.Name = "cbEditNumber";
			this.cbEditNumber.Size = new System.Drawing.Size(112, 17);
			this.cbEditNumber.TabIndex = 2;
			this.cbEditNumber.Text = "Edit report number";
			this.cbEditNumber.UseVisualStyleBackColor = true;
			this.cbEditNumber.CheckedChanged += new System.EventHandler(this.cbChecked_Changed);
			// 
			// cbEditStartDate
			// 
			this.cbEditStartDate.AutoSize = true;
			this.cbEditStartDate.Location = new System.Drawing.Point(15, 71);
			this.cbEditStartDate.Name = "cbEditStartDate";
			this.cbEditStartDate.Size = new System.Drawing.Size(108, 17);
			this.cbEditStartDate.TabIndex = 3;
			this.cbEditStartDate.Text = "Edit start of week";
			this.cbEditStartDate.UseVisualStyleBackColor = true;
			this.cbEditStartDate.CheckedChanged += new System.EventHandler(this.cbChecked_Changed);
			// 
			// cbEditEndDate
			// 
			this.cbEditEndDate.AutoSize = true;
			this.cbEditEndDate.Location = new System.Drawing.Point(15, 94);
			this.cbEditEndDate.Name = "cbEditEndDate";
			this.cbEditEndDate.Size = new System.Drawing.Size(106, 17);
			this.cbEditEndDate.TabIndex = 4;
			this.cbEditEndDate.Text = "Edit end of week";
			this.cbEditEndDate.UseVisualStyleBackColor = true;
			this.cbEditEndDate.CheckedChanged += new System.EventHandler(this.cbChecked_Changed);
			// 
			// cbEditYear
			// 
			this.cbEditYear.AutoSize = true;
			this.cbEditYear.Location = new System.Drawing.Point(15, 117);
			this.cbEditYear.Name = "cbEditYear";
			this.cbEditYear.Size = new System.Drawing.Size(67, 17);
			this.cbEditYear.TabIndex = 5;
			this.cbEditYear.Text = "Edit year";
			this.cbEditYear.UseVisualStyleBackColor = true;
			this.cbEditYear.CheckedChanged += new System.EventHandler(this.cbChecked_Changed);
			// 
			// cbEditWork
			// 
			this.cbEditWork.AutoSize = true;
			this.cbEditWork.Location = new System.Drawing.Point(15, 140);
			this.cbEditWork.Name = "cbEditWork";
			this.cbEditWork.Size = new System.Drawing.Size(70, 17);
			this.cbEditWork.TabIndex = 6;
			this.cbEditWork.Text = "Edit work";
			this.cbEditWork.UseVisualStyleBackColor = true;
			this.cbEditWork.CheckedChanged += new System.EventHandler(this.cbChecked_Changed);
			// 
			// cbEditSeminars
			// 
			this.cbEditSeminars.AutoSize = true;
			this.cbEditSeminars.Location = new System.Drawing.Point(15, 163);
			this.cbEditSeminars.Name = "cbEditSeminars";
			this.cbEditSeminars.Size = new System.Drawing.Size(88, 17);
			this.cbEditSeminars.TabIndex = 7;
			this.cbEditSeminars.Text = "Edit seminars";
			this.cbEditSeminars.UseVisualStyleBackColor = true;
			this.cbEditSeminars.CheckedChanged += new System.EventHandler(this.cbChecked_Changed);
			// 
			// cbEditSchool
			// 
			this.cbEditSchool.AutoSize = true;
			this.cbEditSchool.Location = new System.Drawing.Point(15, 186);
			this.cbEditSchool.Name = "cbEditSchool";
			this.cbEditSchool.Size = new System.Drawing.Size(78, 17);
			this.cbEditSchool.TabIndex = 8;
			this.cbEditSchool.Text = "Edit school";
			this.cbEditSchool.UseVisualStyleBackColor = true;
			this.cbEditSchool.CheckedChanged += new System.EventHandler(this.cbChecked_Changed);
			// 
			// cbEditSignY
			// 
			this.cbEditSignY.AutoSize = true;
			this.cbEditSignY.Location = new System.Drawing.Point(15, 209);
			this.cbEditSignY.Name = "cbEditSignY";
			this.cbEditSignY.Size = new System.Drawing.Size(113, 17);
			this.cbEditSignY.TabIndex = 9;
			this.cbEditSignY.Text = "Edit your sign date";
			this.cbEditSignY.UseVisualStyleBackColor = true;
			this.cbEditSignY.CheckedChanged += new System.EventHandler(this.cbChecked_Changed);
			// 
			// cbEditSign
			// 
			this.cbEditSign.AutoSize = true;
			this.cbEditSign.Location = new System.Drawing.Point(15, 232);
			this.cbEditSign.Name = "cbEditSign";
			this.cbEditSign.Size = new System.Drawing.Size(148, 17);
			this.cbEditSign.TabIndex = 10;
			this.cbEditSign.Text = "Edit sign date of instructor";
			this.cbEditSign.UseVisualStyleBackColor = true;
			this.cbEditSign.CheckedChanged += new System.EventHandler(this.cbChecked_Changed);
			// 
			// btConfirm
			// 
			this.btConfirm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btConfirm.Location = new System.Drawing.Point(264, 255);
			this.btConfirm.Name = "btConfirm";
			this.btConfirm.Size = new System.Drawing.Size(75, 23);
			this.btConfirm.TabIndex = 11;
			this.btConfirm.Text = "Start Edit";
			this.btConfirm.UseVisualStyleBackColor = true;
			this.btConfirm.Click += new System.EventHandler(this.btConfirm_Click);
			// 
			// laInstructions
			// 
			this.laInstructions.AutoSize = true;
			this.laInstructions.Location = new System.Drawing.Point(12, 9);
			this.laInstructions.Name = "laInstructions";
			this.laInstructions.Size = new System.Drawing.Size(327, 13);
			this.laInstructions.TabIndex = 0;
			this.laInstructions.Text = "Please select which fields in the selected report you want to change";
			// 
			// SelectEditFrom
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(351, 290);
			this.Controls.Add(this.laInstructions);
			this.Controls.Add(this.btConfirm);
			this.Controls.Add(this.cbEditSign);
			this.Controls.Add(this.cbEditSignY);
			this.Controls.Add(this.cbEditSchool);
			this.Controls.Add(this.cbEditSeminars);
			this.Controls.Add(this.cbEditWork);
			this.Controls.Add(this.cbEditYear);
			this.Controls.Add(this.cbEditEndDate);
			this.Controls.Add(this.cbEditStartDate);
			this.Controls.Add(this.cbEditNumber);
			this.Controls.Add(this.cbEditName);
			this.MinimumSize = new System.Drawing.Size(367, 329);
			this.Name = "SelectEditFrom";
			this.Text = "Select which Fields to edit";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox cbEditName;
		private System.Windows.Forms.CheckBox cbEditNumber;
		private System.Windows.Forms.CheckBox cbEditStartDate;
		private System.Windows.Forms.CheckBox cbEditEndDate;
		private System.Windows.Forms.CheckBox cbEditYear;
		private System.Windows.Forms.CheckBox cbEditWork;
		private System.Windows.Forms.CheckBox cbEditSeminars;
		private System.Windows.Forms.CheckBox cbEditSchool;
		private System.Windows.Forms.CheckBox cbEditSignY;
		private System.Windows.Forms.CheckBox cbEditSign;
		private OwnControls.FocusColoredFlatButton btConfirm;
		private System.Windows.Forms.Label laInstructions;
	}
}