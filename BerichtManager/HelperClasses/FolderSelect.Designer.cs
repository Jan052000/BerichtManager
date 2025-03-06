namespace BerichtManager.HelperClasses
{
	partial class FolderSelect
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
			btClose = new OwnControls.FocusColoredFlatButton();
			btConfirm = new OwnControls.FocusColoredFlatButton();
			tvFolders = new OwnControls.OwnTreeView.CustomTreeView();
			SuspendLayout();
			// 
			// btClose
			// 
			btClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			btClose.Location = new Point(832, 479);
			btClose.Margin = new Padding(4, 3, 4, 3);
			btClose.Name = "btClose";
			btClose.Size = new Size(88, 27);
			btClose.TabIndex = 2;
			btClose.Text = "Close";
			btClose.UseVisualStyleBackColor = true;
			btClose.Click += btClose_Click;
			// 
			// btConfirm
			// 
			btConfirm.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			btConfirm.Location = new Point(737, 479);
			btConfirm.Margin = new Padding(4, 3, 4, 3);
			btConfirm.Name = "btConfirm";
			btConfirm.Size = new Size(88, 27);
			btConfirm.TabIndex = 1;
			btConfirm.Text = "Confirm";
			btConfirm.UseVisualStyleBackColor = true;
			btConfirm.Click += btConfirm_Click;
			// 
			// tvFolders
			// 
			tvFolders.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			tvFolders.CheckBoxes = true;
			tvFolders.DrawMode = TreeViewDrawMode.OwnerDrawAll;
			tvFolders.Location = new Point(14, 14);
			tvFolders.Margin = new Padding(4, 3, 4, 3);
			tvFolders.Name = "tvFolders";
			tvFolders.Size = new Size(905, 457);
			tvFolders.SuppressWindowsWarnOnKeyDown = true;
			tvFolders.TabIndex = 0;
			// 
			// FolderSelect
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(933, 519);
			Controls.Add(tvFolders);
			Controls.Add(btConfirm);
			Controls.Add(btClose);
			Margin = new Padding(4, 3, 4, 3);
			Name = "FolderSelect";
			Text = "FolderSelect";
			ResumeLayout(false);
		}

		#endregion

		private OwnControls.FocusColoredFlatButton btClose;
		private OwnControls.FocusColoredFlatButton btConfirm;
		private OwnControls.OwnTreeView.CustomTreeView tvFolders;
	}
}