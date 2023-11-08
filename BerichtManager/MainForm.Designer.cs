namespace BerichtManager
{
	partial class MainForm
	{
		/// <summary>
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Verwendete Ressourcen bereinigen.
		/// </summary>
		/// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
				try
				{
					if (this.WordApp != null) 
					{
						this.WordApp.Quit(SaveChanges: false);
					}
				}
				catch 
				{
					
				}
			}
			base.Dispose(disposing);
		}

		#region Vom Windows Form-Designer generierter Code

		/// <summary>
		/// Erforderliche Methode für die Designerunterstützung.
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.tvReports = new System.Windows.Forms.TreeView();
			this.toRightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.miDelete = new System.Windows.Forms.ToolStripMenuItem();
			this.miEdit = new System.Windows.Forms.ToolStripMenuItem();
			this.miQuickEditOptions = new System.Windows.Forms.ToolStripMenuItem();
			this.tiQuickEditWork = new System.Windows.Forms.ToolStripMenuItem();
			this.miQuickEditSchool = new System.Windows.Forms.ToolStripMenuItem();
			this.miPrint = new System.Windows.Forms.ToolStripMenuItem();
			this.miRefresh = new System.Windows.Forms.ToolStripMenuItem();
			this.ttTips = new System.Windows.Forms.ToolTip(this.components);
			this.rtbWork = new System.Windows.Forms.RichTextBox();
			this.rtbSchool = new System.Windows.Forms.RichTextBox();
			this.paMainView = new System.Windows.Forms.Panel();
			this.scTextBoxes = new System.Windows.Forms.SplitContainer();
			this.paPadding1 = new System.Windows.Forms.Panel();
			this.paPadding2 = new System.Windows.Forms.Panel();
			this.splitterTreeBoxes = new System.Windows.Forms.Splitter();
			this.paFileTree = new System.Windows.Forms.Panel();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.miFile = new System.Windows.Forms.ToolStripMenuItem();
			this.miCreate = new System.Windows.Forms.ToolStripMenuItem();
			this.miEditLatest = new System.Windows.Forms.ToolStripMenuItem();
			this.miPrintAll = new System.Windows.Forms.ToolStripMenuItem();
			this.miRevealInExplorer = new System.Windows.Forms.ToolStripMenuItem();
			this.miOptions = new System.Windows.Forms.ToolStripMenuItem();
			this.miOptionsMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.miWordVisible = new System.Windows.Forms.ToolStripMenuItem();
			this.miClose = new System.Windows.Forms.ToolStripMenuItem();
			this.miCloseReport = new System.Windows.Forms.ToolStripMenuItem();
			this.toRightClickMenu.SuspendLayout();
			this.paMainView.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.scTextBoxes)).BeginInit();
			this.scTextBoxes.Panel1.SuspendLayout();
			this.scTextBoxes.Panel2.SuspendLayout();
			this.scTextBoxes.SuspendLayout();
			this.paPadding1.SuspendLayout();
			this.paPadding2.SuspendLayout();
			this.paFileTree.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tvReports
			// 
			this.tvReports.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tvReports.ContextMenuStrip = this.toRightClickMenu;
			this.tvReports.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tvReports.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawAll;
			this.tvReports.Location = new System.Drawing.Point(0, 0);
			this.tvReports.Margin = new System.Windows.Forms.Padding(0);
			this.tvReports.Name = "tvReports";
			this.tvReports.Size = new System.Drawing.Size(235, 426);
			this.tvReports.TabIndex = 6;
			this.tvReports.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.tvReports_DrawNode);
			this.tvReports.Click += new System.EventHandler(this.tvReports_Click);
			this.tvReports.DoubleClick += new System.EventHandler(this.tvReports_DoubleClick);
			this.tvReports.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DetectKeys);
			this.tvReports.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tvReports_KeyUp);
			// 
			// toRightClickMenu
			// 
			this.toRightClickMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miDelete,
            this.miEdit,
            this.miQuickEditOptions,
            this.miPrint,
            this.miRefresh});
			this.toRightClickMenu.Name = "contextMenuStrip1";
			this.toRightClickMenu.Size = new System.Drawing.Size(147, 114);
			this.toRightClickMenu.Opening += new System.ComponentModel.CancelEventHandler(this.toRightClickMenu_Opening);
			// 
			// miDelete
			// 
			this.miDelete.Name = "miDelete";
			this.miDelete.Size = new System.Drawing.Size(146, 22);
			this.miDelete.Text = "Delete";
			this.miDelete.Click += new System.EventHandler(this.miDelete_Click);
			// 
			// miEdit
			// 
			this.miEdit.Name = "miEdit";
			this.miEdit.Size = new System.Drawing.Size(146, 22);
			this.miEdit.Text = "Edit";
			this.miEdit.Click += new System.EventHandler(this.miEdit_Click);
			// 
			// miQuickEditOptions
			// 
			this.miQuickEditOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tiQuickEditWork,
            this.miQuickEditSchool});
			this.miQuickEditOptions.Name = "miQuickEditOptions";
			this.miQuickEditOptions.Size = new System.Drawing.Size(146, 22);
			this.miQuickEditOptions.Text = "Quick actions";
			// 
			// tiQuickEditWork
			// 
			this.tiQuickEditWork.Name = "tiQuickEditWork";
			this.tiQuickEditWork.Size = new System.Drawing.Size(132, 22);
			this.tiQuickEditWork.Text = "Edit work";
			this.tiQuickEditWork.Click += new System.EventHandler(this.miQuickEditWork_Click);
			// 
			// miQuickEditSchool
			// 
			this.miQuickEditSchool.Name = "miQuickEditSchool";
			this.miQuickEditSchool.Size = new System.Drawing.Size(132, 22);
			this.miQuickEditSchool.Text = "Edit school";
			this.miQuickEditSchool.Click += new System.EventHandler(this.miQuickEditSchool_Click);
			// 
			// miPrint
			// 
			this.miPrint.Name = "miPrint";
			this.miPrint.Size = new System.Drawing.Size(146, 22);
			this.miPrint.Text = "Print";
			this.miPrint.Click += new System.EventHandler(this.miPrint_Click);
			// 
			// miRefresh
			// 
			this.miRefresh.Name = "miRefresh";
			this.miRefresh.Size = new System.Drawing.Size(146, 22);
			this.miRefresh.Text = "Refresh";
			this.miRefresh.Click += new System.EventHandler(this.MiRefresh_Click);
			// 
			// rtbWork
			// 
			this.rtbWork.AcceptsTab = true;
			this.rtbWork.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtbWork.Location = new System.Drawing.Point(0, 3);
			this.rtbWork.Margin = new System.Windows.Forms.Padding(0);
			this.rtbWork.Name = "rtbWork";
			this.rtbWork.Size = new System.Drawing.Size(559, 209);
			this.rtbWork.TabIndex = 4;
			this.rtbWork.Text = "";
			this.ttTips.SetToolTip(this.rtbWork, "Work");
			this.rtbWork.WordWrap = false;
			this.rtbWork.TextChanged += new System.EventHandler(this.EditRichTextBox);
			this.rtbWork.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DetectKeys);
			// 
			// rtbSchool
			// 
			this.rtbSchool.AcceptsTab = true;
			this.rtbSchool.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtbSchool.Location = new System.Drawing.Point(0, 0);
			this.rtbSchool.Margin = new System.Windows.Forms.Padding(0);
			this.rtbSchool.Name = "rtbSchool";
			this.rtbSchool.Size = new System.Drawing.Size(559, 210);
			this.rtbSchool.TabIndex = 4;
			this.rtbSchool.Text = "";
			this.ttTips.SetToolTip(this.rtbSchool, "School");
			this.rtbSchool.WordWrap = false;
			this.rtbSchool.TextChanged += new System.EventHandler(this.EditRichTextBox);
			this.rtbSchool.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DetectKeys);
			// 
			// paMainView
			// 
			this.paMainView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.paMainView.Controls.Add(this.scTextBoxes);
			this.paMainView.Controls.Add(this.splitterTreeBoxes);
			this.paMainView.Controls.Add(this.paFileTree);
			this.paMainView.Location = new System.Drawing.Point(0, 24);
			this.paMainView.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
			this.paMainView.Name = "paMainView";
			this.paMainView.Size = new System.Drawing.Size(800, 426);
			this.paMainView.TabIndex = 15;
			// 
			// scTextBoxes
			// 
			this.scTextBoxes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.scTextBoxes.Location = new System.Drawing.Point(238, 0);
			this.scTextBoxes.Name = "scTextBoxes";
			this.scTextBoxes.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// scTextBoxes.Panel1
			// 
			this.scTextBoxes.Panel1.Controls.Add(this.paPadding1);
			// 
			// scTextBoxes.Panel2
			// 
			this.scTextBoxes.Panel2.Controls.Add(this.paPadding2);
			this.scTextBoxes.Size = new System.Drawing.Size(562, 426);
			this.scTextBoxes.SplitterDistance = 212;
			this.scTextBoxes.SplitterWidth = 1;
			this.scTextBoxes.TabIndex = 2;
			// 
			// paPadding1
			// 
			this.paPadding1.Controls.Add(this.rtbWork);
			this.paPadding1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.paPadding1.Location = new System.Drawing.Point(0, 0);
			this.paPadding1.Name = "paPadding1";
			this.paPadding1.Padding = new System.Windows.Forms.Padding(0, 3, 3, 0);
			this.paPadding1.Size = new System.Drawing.Size(562, 212);
			this.paPadding1.TabIndex = 0;
			// 
			// paPadding2
			// 
			this.paPadding2.Controls.Add(this.rtbSchool);
			this.paPadding2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.paPadding2.Location = new System.Drawing.Point(0, 0);
			this.paPadding2.Name = "paPadding2";
			this.paPadding2.Padding = new System.Windows.Forms.Padding(0, 0, 3, 3);
			this.paPadding2.Size = new System.Drawing.Size(562, 213);
			this.paPadding2.TabIndex = 0;
			// 
			// splitterTreeBoxes
			// 
			this.splitterTreeBoxes.Location = new System.Drawing.Point(235, 0);
			this.splitterTreeBoxes.Name = "splitterTreeBoxes";
			this.splitterTreeBoxes.Size = new System.Drawing.Size(3, 426);
			this.splitterTreeBoxes.TabIndex = 1;
			this.splitterTreeBoxes.TabStop = false;
			// 
			// paFileTree
			// 
			this.paFileTree.Controls.Add(this.tvReports);
			this.paFileTree.Dock = System.Windows.Forms.DockStyle.Left;
			this.paFileTree.Location = new System.Drawing.Point(0, 0);
			this.paFileTree.Name = "paFileTree";
			this.paFileTree.Size = new System.Drawing.Size(235, 426);
			this.paFileTree.TabIndex = 0;
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miFile,
            this.miOptions,
            this.miClose});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(800, 24);
			this.menuStrip1.TabIndex = 16;
			this.menuStrip1.Text = "menuStrip1";
			this.menuStrip1.Paint += new System.Windows.Forms.PaintEventHandler(this.menuStrip1_Paint);
			// 
			// miFile
			// 
			this.miFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miCreate,
            this.miEditLatest,
            this.miPrintAll,
            this.miRevealInExplorer,
            this.miCloseReport});
			this.miFile.Name = "miFile";
			this.miFile.Size = new System.Drawing.Size(37, 20);
			this.miFile.Text = "File";
			// 
			// miCreate
			// 
			this.miCreate.Name = "miCreate";
			this.miCreate.Size = new System.Drawing.Size(180, 22);
			this.miCreate.Text = "Create";
			this.miCreate.Click += new System.EventHandler(this.btCreate_Click);
			// 
			// miEditLatest
			// 
			this.miEditLatest.Name = "miEditLatest";
			this.miEditLatest.Size = new System.Drawing.Size(180, 22);
			this.miEditLatest.Text = "Edit latest";
			this.miEditLatest.Click += new System.EventHandler(this.btEdit_Click);
			// 
			// miPrintAll
			// 
			this.miPrintAll.Name = "miPrintAll";
			this.miPrintAll.Size = new System.Drawing.Size(180, 22);
			this.miPrintAll.Text = "Print all";
			this.miPrintAll.Click += new System.EventHandler(this.btPrintAll_Click);
			// 
			// miRevealInExplorer
			// 
			this.miRevealInExplorer.Name = "miRevealInExplorer";
			this.miRevealInExplorer.Size = new System.Drawing.Size(180, 22);
			this.miRevealInExplorer.Text = "Reveal in explorer";
			this.miRevealInExplorer.Click += new System.EventHandler(this.miRevealInExplorer_Click);
			// 
			// miOptions
			// 
			this.miOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miOptionsMenu,
            this.miWordVisible});
			this.miOptions.Name = "miOptions";
			this.miOptions.Size = new System.Drawing.Size(61, 20);
			this.miOptions.Text = "Options";
			// 
			// miOptionsMenu
			// 
			this.miOptionsMenu.Name = "miOptionsMenu";
			this.miOptionsMenu.Size = new System.Drawing.Size(150, 22);
			this.miOptionsMenu.Text = "Options menu";
			this.miOptionsMenu.Click += new System.EventHandler(this.btOptions_Click);
			// 
			// miWordVisible
			// 
			this.miWordVisible.CheckOnClick = true;
			this.miWordVisible.Name = "miWordVisible";
			this.miWordVisible.Size = new System.Drawing.Size(150, 22);
			this.miWordVisible.Text = "Word visible";
			this.miWordVisible.Click += new System.EventHandler(this.miWordVisible_Click);
			// 
			// miClose
			// 
			this.miClose.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.miClose.Enabled = false;
			this.miClose.Name = "miClose";
			this.miClose.Size = new System.Drawing.Size(48, 20);
			this.miClose.Text = "Close";
			this.miClose.Visible = false;
			this.miClose.Click += new System.EventHandler(this.btClose_Click);
			// 
			// miCloseReport
			// 
			this.miCloseReport.Name = "miCloseReport";
			this.miCloseReport.Size = new System.Drawing.Size(180, 22);
			this.miCloseReport.Text = "Close Report";
			this.miCloseReport.Click += new System.EventHandler(this.miCloseReport_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.menuStrip1);
			this.Controls.Add(this.paMainView);
			this.MainMenuStrip = this.menuStrip1;
			this.MinimumSize = new System.Drawing.Size(545, 289);
			this.Name = "MainForm";
			this.Text = "Bericht Manager";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormManager_FormClosing);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DetectKeys);
			this.toRightClickMenu.ResumeLayout(false);
			this.paMainView.ResumeLayout(false);
			this.scTextBoxes.Panel1.ResumeLayout(false);
			this.scTextBoxes.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.scTextBoxes)).EndInit();
			this.scTextBoxes.ResumeLayout(false);
			this.paPadding1.ResumeLayout(false);
			this.paPadding2.ResumeLayout(false);
			this.paFileTree.ResumeLayout(false);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.TreeView tvReports;
		private System.Windows.Forms.ContextMenuStrip toRightClickMenu;
		private System.Windows.Forms.ToolStripMenuItem miDelete;
		private System.Windows.Forms.ToolStripMenuItem miEdit;
		private System.Windows.Forms.ToolStripMenuItem miPrint;
		private System.Windows.Forms.ToolTip ttTips;
		private System.Windows.Forms.ToolStripMenuItem miQuickEditOptions;
		private System.Windows.Forms.ToolStripMenuItem tiQuickEditWork;
		private System.Windows.Forms.ToolStripMenuItem miQuickEditSchool;
		private System.Windows.Forms.RichTextBox rtbWork;
		private System.Windows.Forms.RichTextBox rtbSchool;
		private System.Windows.Forms.Panel paMainView;
		private System.Windows.Forms.Panel paFileTree;
		private System.Windows.Forms.Splitter splitterTreeBoxes;
		private System.Windows.Forms.SplitContainer scTextBoxes;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem miFile;
		private System.Windows.Forms.ToolStripMenuItem miCreate;
		private System.Windows.Forms.ToolStripMenuItem miEditLatest;
		private System.Windows.Forms.ToolStripMenuItem miOptions;
		private System.Windows.Forms.ToolStripMenuItem miPrintAll;
		private System.Windows.Forms.ToolStripMenuItem miWordVisible;
		private System.Windows.Forms.ToolStripMenuItem miOptionsMenu;
		private System.Windows.Forms.ToolStripMenuItem miClose;
		private System.Windows.Forms.ToolStripMenuItem miRefresh;
		private System.Windows.Forms.Panel paPadding1;
		private System.Windows.Forms.Panel paPadding2;
		private System.Windows.Forms.ToolStripMenuItem miRevealInExplorer;
		private System.Windows.Forms.ToolStripMenuItem miCloseReport;
	}
}

