namespace BerichtManager
{
	partial class FormManager
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
					if (this.wordApp != null) 
					{
						this.wordApp.Quit(SaveChanges: false);
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
			this.btClose = new System.Windows.Forms.Button();
			this.btSetTemplate = new System.Windows.Forms.Button();
			this.btSetNumber = new System.Windows.Forms.Button();
			this.tvReports = new System.Windows.Forms.TreeView();
			this.toRightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.miDelete = new System.Windows.Forms.ToolStripMenuItem();
			this.miEdit = new System.Windows.Forms.ToolStripMenuItem();
			this.miQuickEditOptions = new System.Windows.Forms.ToolStripMenuItem();
			this.tiQuickEditWork = new System.Windows.Forms.ToolStripMenuItem();
			this.miQuickEditSchool = new System.Windows.Forms.ToolStripMenuItem();
			this.miPrint = new System.Windows.Forms.ToolStripMenuItem();
			this.btLogin = new System.Windows.Forms.Button();
			this.btEditName = new System.Windows.Forms.Button();
			this.ttTips = new System.Windows.Forms.ToolTip(this.components);
			this.rtbWork = new System.Windows.Forms.RichTextBox();
			this.rtbSchool = new System.Windows.Forms.RichTextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.paMainView = new System.Windows.Forms.Panel();
			this.scTextBoxes = new System.Windows.Forms.SplitContainer();
			this.splitterTreeBoxes = new System.Windows.Forms.Splitter();
			this.paFileTree = new System.Windows.Forms.Panel();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.miFile = new System.Windows.Forms.ToolStripMenuItem();
			this.miCreate = new System.Windows.Forms.ToolStripMenuItem();
			this.miEditLatest = new System.Windows.Forms.ToolStripMenuItem();
			this.miPrintAll = new System.Windows.Forms.ToolStripMenuItem();
			this.miOptions = new System.Windows.Forms.ToolStripMenuItem();
			this.miOptionsMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.miWordVisible = new System.Windows.Forms.ToolStripMenuItem();
			this.toRightClickMenu.SuspendLayout();
			this.paMainView.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.scTextBoxes)).BeginInit();
			this.scTextBoxes.Panel1.SuspendLayout();
			this.scTextBoxes.Panel2.SuspendLayout();
			this.scTextBoxes.SuspendLayout();
			this.paFileTree.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btClose
			// 
			this.btClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btClose.Location = new System.Drawing.Point(704, 415);
			this.btClose.Name = "btClose";
			this.btClose.Size = new System.Drawing.Size(84, 23);
			this.btClose.TabIndex = 0;
			this.btClose.Text = "Close";
			this.btClose.UseVisualStyleBackColor = true;
			this.btClose.Click += new System.EventHandler(this.btClose_Click);
			// 
			// btSetTemplate
			// 
			this.btSetTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btSetTemplate.Location = new System.Drawing.Point(619, 415);
			this.btSetTemplate.Name = "btSetTemplate";
			this.btSetTemplate.Size = new System.Drawing.Size(79, 23);
			this.btSetTemplate.TabIndex = 1;
			this.btSetTemplate.Text = "Set Template";
			this.ttTips.SetToolTip(this.btSetTemplate, "Sets the path for the template");
			this.btSetTemplate.UseVisualStyleBackColor = true;
			this.btSetTemplate.Click += new System.EventHandler(this.btSetTemplate_Click);
			// 
			// btSetNumber
			// 
			this.btSetNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btSetNumber.Location = new System.Drawing.Point(538, 415);
			this.btSetNumber.Name = "btSetNumber";
			this.btSetNumber.Size = new System.Drawing.Size(75, 23);
			this.btSetNumber.TabIndex = 3;
			this.btSetNumber.Text = "Set Number";
			this.ttTips.SetToolTip(this.btSetNumber, "Changes the report number to be used next");
			this.btSetNumber.UseVisualStyleBackColor = true;
			this.btSetNumber.Click += new System.EventHandler(this.btSetNumber_Click);
			// 
			// tvReports
			// 
			this.tvReports.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tvReports.ContextMenuStrip = this.toRightClickMenu;
			this.tvReports.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tvReports.Location = new System.Drawing.Point(0, 0);
			this.tvReports.Margin = new System.Windows.Forms.Padding(0);
			this.tvReports.Name = "tvReports";
			this.tvReports.Size = new System.Drawing.Size(235, 385);
			this.tvReports.TabIndex = 6;
			this.tvReports.Click += new System.EventHandler(this.tvReports_Click);
			this.tvReports.DoubleClick += new System.EventHandler(this.tvReports_DoubleClick);
			this.tvReports.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DetectKeys);
			// 
			// toRightClickMenu
			// 
			this.toRightClickMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miDelete,
            this.miEdit,
            this.miQuickEditOptions,
            this.miPrint});
			this.toRightClickMenu.Name = "contextMenuStrip1";
			this.toRightClickMenu.Size = new System.Drawing.Size(147, 92);
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
			// btLogin
			// 
			this.btLogin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btLogin.Location = new System.Drawing.Point(12, 415);
			this.btLogin.Name = "btLogin";
			this.btLogin.Size = new System.Drawing.Size(75, 23);
			this.btLogin.TabIndex = 11;
			this.btLogin.Text = "Login";
			this.ttTips.SetToolTip(this.btLogin, "Opens login window");
			this.btLogin.UseVisualStyleBackColor = true;
			this.btLogin.Click += new System.EventHandler(this.btLogin_Click);
			// 
			// btEditName
			// 
			this.btEditName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btEditName.Location = new System.Drawing.Point(457, 415);
			this.btEditName.Name = "btEditName";
			this.btEditName.Size = new System.Drawing.Size(75, 23);
			this.btEditName.TabIndex = 12;
			this.btEditName.Text = "Edit Name";
			this.ttTips.SetToolTip(this.btEditName, "Sets the name to be used in reports");
			this.btEditName.UseVisualStyleBackColor = true;
			this.btEditName.Click += new System.EventHandler(this.btEditName_Click);
			// 
			// rtbWork
			// 
			this.rtbWork.AcceptsTab = true;
			this.rtbWork.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtbWork.Location = new System.Drawing.Point(0, 0);
			this.rtbWork.Margin = new System.Windows.Forms.Padding(0);
			this.rtbWork.Name = "rtbWork";
			this.rtbWork.Size = new System.Drawing.Size(562, 192);
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
			this.rtbSchool.Size = new System.Drawing.Size(562, 192);
			this.rtbSchool.TabIndex = 4;
			this.rtbSchool.Text = "";
			this.ttTips.SetToolTip(this.rtbSchool, "School");
			this.rtbSchool.WordWrap = false;
			this.rtbSchool.TextChanged += new System.EventHandler(this.EditRichTextBox);
			this.rtbSchool.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DetectKeys);
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.Enabled = false;
			this.button1.Location = new System.Drawing.Point(203, 415);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 5;
			this.button1.Text = "Test";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Visible = false;
			this.button1.Click += new System.EventHandler(this.btTest_Click);
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
			this.paMainView.Size = new System.Drawing.Size(800, 385);
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
			this.scTextBoxes.Panel1.Controls.Add(this.rtbWork);
			// 
			// scTextBoxes.Panel2
			// 
			this.scTextBoxes.Panel2.Controls.Add(this.rtbSchool);
			this.scTextBoxes.Size = new System.Drawing.Size(562, 385);
			this.scTextBoxes.SplitterDistance = 192;
			this.scTextBoxes.SplitterWidth = 1;
			this.scTextBoxes.TabIndex = 2;
			// 
			// splitterTreeBoxes
			// 
			this.splitterTreeBoxes.Location = new System.Drawing.Point(235, 0);
			this.splitterTreeBoxes.Name = "splitterTreeBoxes";
			this.splitterTreeBoxes.Size = new System.Drawing.Size(3, 385);
			this.splitterTreeBoxes.TabIndex = 1;
			this.splitterTreeBoxes.TabStop = false;
			// 
			// paFileTree
			// 
			this.paFileTree.Controls.Add(this.tvReports);
			this.paFileTree.Dock = System.Windows.Forms.DockStyle.Left;
			this.paFileTree.Location = new System.Drawing.Point(0, 0);
			this.paFileTree.Name = "paFileTree";
			this.paFileTree.Size = new System.Drawing.Size(235, 385);
			this.paFileTree.TabIndex = 0;
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miFile,
            this.miOptions});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(800, 24);
			this.menuStrip1.TabIndex = 16;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// miFile
			// 
			this.miFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miCreate,
            this.miEditLatest,
            this.miPrintAll});
			this.miFile.Name = "miFile";
			this.miFile.Size = new System.Drawing.Size(37, 20);
			this.miFile.Text = "File";
			// 
			// miCreate
			// 
			this.miCreate.Name = "miCreate";
			this.miCreate.Size = new System.Drawing.Size(125, 22);
			this.miCreate.Text = "Create";
			this.miCreate.Click += new System.EventHandler(this.btCreate_Click);
			// 
			// miEditLatest
			// 
			this.miEditLatest.Name = "miEditLatest";
			this.miEditLatest.Size = new System.Drawing.Size(125, 22);
			this.miEditLatest.Text = "Edit latest";
			this.miEditLatest.Click += new System.EventHandler(this.btEdit_Click);
			// 
			// miPrintAll
			// 
			this.miPrintAll.Name = "miPrintAll";
			this.miPrintAll.Size = new System.Drawing.Size(125, 22);
			this.miPrintAll.Text = "Print all";
			this.miPrintAll.Click += new System.EventHandler(this.btPrintAll_Click);
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
			this.miOptionsMenu.Size = new System.Drawing.Size(180, 22);
			this.miOptionsMenu.Text = "Options menu";
			this.miOptionsMenu.Click += new System.EventHandler(this.btOptions_Click);
			// 
			// miWordVisible
			// 
			this.miWordVisible.CheckOnClick = true;
			this.miWordVisible.Name = "miWordVisible";
			this.miWordVisible.Size = new System.Drawing.Size(180, 22);
			this.miWordVisible.Text = "Word visible";
			this.miWordVisible.Click += new System.EventHandler(this.miWordVisible_Click);
			// 
			// FormManager
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.menuStrip1);
			this.Controls.Add(this.paMainView);
			this.Controls.Add(this.btEditName);
			this.Controls.Add(this.btLogin);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.btSetNumber);
			this.Controls.Add(this.btSetTemplate);
			this.Controls.Add(this.btClose);
			this.MainMenuStrip = this.menuStrip1;
			this.MinimumSize = new System.Drawing.Size(545, 289);
			this.Name = "FormManager";
			this.Text = "Bericht Manager";
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DetectKeys);
			this.toRightClickMenu.ResumeLayout(false);
			this.paMainView.ResumeLayout(false);
			this.scTextBoxes.Panel1.ResumeLayout(false);
			this.scTextBoxes.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.scTextBoxes)).EndInit();
			this.scTextBoxes.ResumeLayout(false);
			this.paFileTree.ResumeLayout(false);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btClose;
		private System.Windows.Forms.Button btSetTemplate;
		private System.Windows.Forms.Button btSetNumber;
		private System.Windows.Forms.TreeView tvReports;
		private System.Windows.Forms.Button btLogin;
		private System.Windows.Forms.Button btEditName;
		private System.Windows.Forms.ContextMenuStrip toRightClickMenu;
		private System.Windows.Forms.ToolStripMenuItem miDelete;
		private System.Windows.Forms.ToolStripMenuItem miEdit;
		private System.Windows.Forms.ToolStripMenuItem miPrint;
		private System.Windows.Forms.ToolTip ttTips;
		private System.Windows.Forms.ToolStripMenuItem miQuickEditOptions;
		private System.Windows.Forms.ToolStripMenuItem tiQuickEditWork;
		private System.Windows.Forms.ToolStripMenuItem miQuickEditSchool;
		private System.Windows.Forms.Button button1;
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
	}
}

