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
			this.laInstructions = new System.Windows.Forms.Label();
			this.flpCheckBoxes = new System.Windows.Forms.FlowLayoutPanel();
			this.btConfirm = new BerichtManager.OwnControls.FocusColoredFlatButton();
			this.SuspendLayout();
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
			// flpCheckBoxes
			// 
			this.flpCheckBoxes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.flpCheckBoxes.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flpCheckBoxes.Location = new System.Drawing.Point(12, 25);
			this.flpCheckBoxes.Name = "flpCheckBoxes";
			this.flpCheckBoxes.Size = new System.Drawing.Size(327, 239);
			this.flpCheckBoxes.TabIndex = 12;
			this.flpCheckBoxes.WrapContents = false;
			// 
			// btConfirm
			// 
			this.btConfirm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btConfirm.Location = new System.Drawing.Point(264, 270);
			this.btConfirm.Name = "btConfirm";
			this.btConfirm.Size = new System.Drawing.Size(75, 23);
			this.btConfirm.TabIndex = 11;
			this.btConfirm.Text = "Start Edit";
			this.btConfirm.UseVisualStyleBackColor = true;
			this.btConfirm.Click += new System.EventHandler(this.btConfirm_Click);
			// 
			// SelectEditFrom
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(351, 305);
			this.Controls.Add(this.flpCheckBoxes);
			this.Controls.Add(this.laInstructions);
			this.Controls.Add(this.btConfirm);
			this.Name = "SelectEditFrom";
			this.Text = "Select which Fields to edit";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private OwnControls.FocusColoredFlatButton btConfirm;
		private System.Windows.Forms.Label laInstructions;
		private System.Windows.Forms.FlowLayoutPanel flpCheckBoxes;
	}
}