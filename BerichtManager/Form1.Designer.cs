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
			this.btCreate = new System.Windows.Forms.Button();
			this.btSetNumber = new System.Windows.Forms.Button();
			this.btEdit = new System.Windows.Forms.Button();
			this.tvReports = new System.Windows.Forms.TreeView();
			this.toRightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.miDelete = new System.Windows.Forms.ToolStripMenuItem();
			this.miEdit = new System.Windows.Forms.ToolStripMenuItem();
			this.miQuickEditOptions = new System.Windows.Forms.ToolStripMenuItem();
			this.tiQuickEditWork = new System.Windows.Forms.ToolStripMenuItem();
			this.miQuickEditSchool = new System.Windows.Forms.ToolStripMenuItem();
			this.miPrint = new System.Windows.Forms.ToolStripMenuItem();
			this.btEditExisting = new System.Windows.Forms.Button();
			this.btDelete = new System.Windows.Forms.Button();
			this.btPrint = new System.Windows.Forms.Button();
			this.btPrintAll = new System.Windows.Forms.Button();
			this.btLogin = new System.Windows.Forms.Button();
			this.btEditName = new System.Windows.Forms.Button();
			this.cbVisible = new System.Windows.Forms.CheckBox();
			this.btOptions = new System.Windows.Forms.Button();
			this.ttTips = new System.Windows.Forms.ToolTip(this.components);
			this.rtbWork = new System.Windows.Forms.RichTextBox();
			this.rtbSchool = new System.Windows.Forms.RichTextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.paMainView = new System.Windows.Forms.Panel();
			this.splitterTreeBoxes = new System.Windows.Forms.Splitter();
			this.paFileTree = new System.Windows.Forms.Panel();
			this.scTextBoxes = new System.Windows.Forms.SplitContainer();
			this.toRightClickMenu.SuspendLayout();
			this.paMainView.SuspendLayout();
			this.paFileTree.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.scTextBoxes)).BeginInit();
			this.scTextBoxes.Panel1.SuspendLayout();
			this.scTextBoxes.Panel2.SuspendLayout();
			this.scTextBoxes.SuspendLayout();
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
			// btCreate
			// 
			this.btCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btCreate.Location = new System.Drawing.Point(704, 12);
			this.btCreate.Name = "btCreate";
			this.btCreate.Size = new System.Drawing.Size(84, 23);
			this.btCreate.TabIndex = 2;
			this.btCreate.Text = "Create";
			this.ttTips.SetToolTip(this.btCreate, "Creates a new report");
			this.btCreate.UseVisualStyleBackColor = true;
			this.btCreate.Click += new System.EventHandler(this.btCreate_Click);
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
			// btEdit
			// 
			this.btEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btEdit.Location = new System.Drawing.Point(704, 41);
			this.btEdit.Name = "btEdit";
			this.btEdit.Size = new System.Drawing.Size(84, 23);
			this.btEdit.TabIndex = 4;
			this.btEdit.Text = "Edit Latest";
			this.ttTips.SetToolTip(this.btEdit, "Edits last created report");
			this.btEdit.UseVisualStyleBackColor = true;
			this.btEdit.Click += new System.EventHandler(this.btEdit_Click);
			// 
			// tvReports
			// 
			this.tvReports.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tvReports.ContextMenuStrip = this.toRightClickMenu;
			this.tvReports.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tvReports.Location = new System.Drawing.Point(0, 0);
			this.tvReports.Margin = new System.Windows.Forms.Padding(0);
			this.tvReports.Name = "tvReports";
			this.tvReports.Size = new System.Drawing.Size(235, 409);
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
			// btEditExisting
			// 
			this.btEditExisting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btEditExisting.Location = new System.Drawing.Point(704, 70);
			this.btEditExisting.Name = "btEditExisting";
			this.btEditExisting.Size = new System.Drawing.Size(84, 23);
			this.btEditExisting.TabIndex = 7;
			this.btEditExisting.Text = "Edit";
			this.ttTips.SetToolTip(this.btEditExisting, "Edits selected report");
			this.btEditExisting.UseVisualStyleBackColor = true;
			this.btEditExisting.Click += new System.EventHandler(this.btEditExisting_Click);
			// 
			// btDelete
			// 
			this.btDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btDelete.Location = new System.Drawing.Point(704, 99);
			this.btDelete.Name = "btDelete";
			this.btDelete.Size = new System.Drawing.Size(84, 23);
			this.btDelete.TabIndex = 8;
			this.btDelete.Text = "Delete";
			this.ttTips.SetToolTip(this.btDelete, "Deletes selected report");
			this.btDelete.UseVisualStyleBackColor = true;
			this.btDelete.Click += new System.EventHandler(this.btDelete_Click);
			// 
			// btPrint
			// 
			this.btPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btPrint.Location = new System.Drawing.Point(704, 128);
			this.btPrint.Name = "btPrint";
			this.btPrint.Size = new System.Drawing.Size(84, 23);
			this.btPrint.TabIndex = 9;
			this.btPrint.Text = "Print Selected";
			this.ttTips.SetToolTip(this.btPrint, "Prints the selected report");
			this.btPrint.UseVisualStyleBackColor = true;
			this.btPrint.Click += new System.EventHandler(this.btPrint_Click);
			// 
			// btPrintAll
			// 
			this.btPrintAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btPrintAll.Location = new System.Drawing.Point(704, 157);
			this.btPrintAll.Name = "btPrintAll";
			this.btPrintAll.Size = new System.Drawing.Size(84, 23);
			this.btPrintAll.TabIndex = 10;
			this.btPrintAll.Text = "Print All";
			this.ttTips.SetToolTip(this.btPrintAll, "Prints all unprinted reports");
			this.btPrintAll.UseVisualStyleBackColor = true;
			this.btPrintAll.Click += new System.EventHandler(this.btPrintAll_Click);
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
			// cbVisible
			// 
			this.cbVisible.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cbVisible.AutoSize = true;
			this.cbVisible.Location = new System.Drawing.Point(93, 419);
			this.cbVisible.Name = "cbVisible";
			this.cbVisible.Size = new System.Drawing.Size(90, 17);
			this.cbVisible.TabIndex = 13;
			this.cbVisible.Text = "Word visible?";
			this.ttTips.SetToolTip(this.cbVisible, "Toggles if word should open a window");
			this.cbVisible.UseVisualStyleBackColor = true;
			this.cbVisible.CheckedChanged += new System.EventHandler(this.cbVisible_CheckedChanged);
			// 
			// btOptions
			// 
			this.btOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btOptions.Location = new System.Drawing.Point(704, 386);
			this.btOptions.Name = "btOptions";
			this.btOptions.Size = new System.Drawing.Size(84, 23);
			this.btOptions.TabIndex = 14;
			this.btOptions.Text = "Options";
			this.ttTips.SetToolTip(this.btOptions, "Opens the option menu");
			this.btOptions.UseVisualStyleBackColor = true;
			this.btOptions.Click += new System.EventHandler(this.btOptions_Click);
			// 
			// rtbWork
			// 
			this.rtbWork.AcceptsTab = true;
			this.rtbWork.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtbWork.Location = new System.Drawing.Point(0, 0);
			this.rtbWork.Margin = new System.Windows.Forms.Padding(0);
			this.rtbWork.Name = "rtbWork";
			this.rtbWork.Size = new System.Drawing.Size(460, 204);
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
			this.rtbSchool.Size = new System.Drawing.Size(460, 204);
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
			this.paMainView.Location = new System.Drawing.Point(0, 0);
			this.paMainView.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
			this.paMainView.Name = "paMainView";
			this.paMainView.Size = new System.Drawing.Size(698, 409);
			this.paMainView.TabIndex = 15;
			// 
			// splitterTreeBoxes
			// 
			this.splitterTreeBoxes.Location = new System.Drawing.Point(235, 0);
			this.splitterTreeBoxes.Name = "splitterTreeBoxes";
			this.splitterTreeBoxes.Size = new System.Drawing.Size(3, 409);
			this.splitterTreeBoxes.TabIndex = 1;
			this.splitterTreeBoxes.TabStop = false;
			// 
			// paFileTree
			// 
			this.paFileTree.Controls.Add(this.tvReports);
			this.paFileTree.Dock = System.Windows.Forms.DockStyle.Left;
			this.paFileTree.Location = new System.Drawing.Point(0, 0);
			this.paFileTree.Name = "paFileTree";
			this.paFileTree.Size = new System.Drawing.Size(235, 409);
			this.paFileTree.TabIndex = 0;
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
			this.scTextBoxes.Size = new System.Drawing.Size(460, 409);
			this.scTextBoxes.SplitterDistance = 204;
			this.scTextBoxes.SplitterWidth = 1;
			this.scTextBoxes.TabIndex = 2;
			// 
			// FormManager
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.paMainView);
			this.Controls.Add(this.btOptions);
			this.Controls.Add(this.cbVisible);
			this.Controls.Add(this.btEditName);
			this.Controls.Add(this.btLogin);
			this.Controls.Add(this.btPrintAll);
			this.Controls.Add(this.btPrint);
			this.Controls.Add(this.btDelete);
			this.Controls.Add(this.btEditExisting);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.btEdit);
			this.Controls.Add(this.btSetNumber);
			this.Controls.Add(this.btCreate);
			this.Controls.Add(this.btSetTemplate);
			this.Controls.Add(this.btClose);
			this.MinimumSize = new System.Drawing.Size(545, 289);
			this.Name = "FormManager";
			this.Text = "Bericht Manager";
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DetectKeys);
			this.toRightClickMenu.ResumeLayout(false);
			this.paMainView.ResumeLayout(false);
			this.paFileTree.ResumeLayout(false);
			this.scTextBoxes.Panel1.ResumeLayout(false);
			this.scTextBoxes.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.scTextBoxes)).EndInit();
			this.scTextBoxes.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btClose;
		private System.Windows.Forms.Button btSetTemplate;
		private System.Windows.Forms.Button btCreate;
		private System.Windows.Forms.Button btSetNumber;
		private System.Windows.Forms.Button btEdit;
		private System.Windows.Forms.TreeView tvReports;
		private System.Windows.Forms.Button btEditExisting;
		private System.Windows.Forms.Button btDelete;
		private System.Windows.Forms.Button btPrint;
		private System.Windows.Forms.Button btPrintAll;
		private System.Windows.Forms.Button btLogin;
		private System.Windows.Forms.Button btEditName;
		private System.Windows.Forms.CheckBox cbVisible;
		private System.Windows.Forms.ContextMenuStrip toRightClickMenu;
		private System.Windows.Forms.ToolStripMenuItem miDelete;
		private System.Windows.Forms.ToolStripMenuItem miEdit;
		private System.Windows.Forms.ToolStripMenuItem miPrint;
		private System.Windows.Forms.Button btOptions;
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
	}
}

