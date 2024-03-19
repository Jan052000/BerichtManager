namespace BerichtManager.HelperClasses.ReportChecking
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
			this.btClose = new System.Windows.Forms.Button();
			this.btConfirm = new System.Windows.Forms.Button();
			this.tvFolders = new System.Windows.Forms.TreeView();
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
			// tvFolders
			// 
			this.tvFolders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tvFolders.CheckBoxes = true;
			this.tvFolders.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawAll;
			this.tvFolders.Location = new System.Drawing.Point(12, 12);
			this.tvFolders.Name = "tvFolders";
			this.tvFolders.Size = new System.Drawing.Size(776, 397);
			this.tvFolders.TabIndex = 2;
			this.tvFolders.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tvFolders_AfterCheck);
			this.tvFolders.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.tvFolders_DrawNode);
			// 
			// FolderSelect
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.tvFolders);
			this.Controls.Add(this.btConfirm);
			this.Controls.Add(this.btClose);
			this.Name = "FolderSelect";
			this.Text = "FolderSelect";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btClose;
		private System.Windows.Forms.Button btConfirm;
		private System.Windows.Forms.TreeView tvFolders;
	}
}