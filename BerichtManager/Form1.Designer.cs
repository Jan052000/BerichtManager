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
			this.button1 = new System.Windows.Forms.Button();
			this.tvReports = new System.Windows.Forms.TreeView();
			this.toRightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.miDelete = new System.Windows.Forms.ToolStripMenuItem();
			this.miEdit = new System.Windows.Forms.ToolStripMenuItem();
			this.miPrint = new System.Windows.Forms.ToolStripMenuItem();
			this.btEditExisting = new System.Windows.Forms.Button();
			this.btDelete = new System.Windows.Forms.Button();
			this.btPrint = new System.Windows.Forms.Button();
			this.btPrintAll = new System.Windows.Forms.Button();
			this.btLogin = new System.Windows.Forms.Button();
			this.btEditName = new System.Windows.Forms.Button();
			this.cbVisible = new System.Windows.Forms.CheckBox();
			this.btOptions = new System.Windows.Forms.Button();
			this.miQuickEditOptions = new System.Windows.Forms.ToolStripMenuItem();
			this.tiQuickEditWork = new System.Windows.Forms.ToolStripMenuItem();
			this.miQuickEditSchool = new System.Windows.Forms.ToolStripMenuItem();
			this.toRightClickMenu.SuspendLayout();
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
			this.btEdit.UseVisualStyleBackColor = true;
			this.btEdit.Click += new System.EventHandler(this.btEdit_Click);
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
			// tvReports
			// 
			this.tvReports.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tvReports.ContextMenuStrip = this.toRightClickMenu;
			this.tvReports.Location = new System.Drawing.Point(12, 12);
			this.tvReports.Name = "tvReports";
			this.tvReports.Size = new System.Drawing.Size(686, 397);
			this.tvReports.TabIndex = 6;
			this.tvReports.Click += new System.EventHandler(this.tvReports_Click);
			this.tvReports.DoubleClick += new System.EventHandler(this.tvReports_DoubleClick);
			// 
			// toRightClickMenu
			// 
			this.toRightClickMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miDelete,
            this.miEdit,
            this.miQuickEditOptions,
            this.miPrint});
			this.toRightClickMenu.Name = "contextMenuStrip1";
			this.toRightClickMenu.Size = new System.Drawing.Size(181, 114);
			this.toRightClickMenu.Opening += new System.ComponentModel.CancelEventHandler(this.toRightClickMenu_Opening);
			// 
			// miDelete
			// 
			this.miDelete.Name = "miDelete";
			this.miDelete.Size = new System.Drawing.Size(180, 22);
			this.miDelete.Text = "Delete";
			this.miDelete.Click += new System.EventHandler(this.miDelete_Click);
			// 
			// miEdit
			// 
			this.miEdit.Name = "miEdit";
			this.miEdit.Size = new System.Drawing.Size(180, 22);
			this.miEdit.Text = "Edit";
			this.miEdit.Click += new System.EventHandler(this.miEdit_Click);
			// 
			// miPrint
			// 
			this.miPrint.Name = "miPrint";
			this.miPrint.Size = new System.Drawing.Size(180, 22);
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
			this.btDelete.UseVisualStyleBackColor = true;
			this.btDelete.Click += new System.EventHandler(this.btDelete_Click);
			// 
			// btPrint
			// 
			this.btPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btPrint.Location = new System.Drawing.Point(704, 157);
			this.btPrint.Name = "btPrint";
			this.btPrint.Size = new System.Drawing.Size(84, 23);
			this.btPrint.TabIndex = 9;
			this.btPrint.Text = "Print Selected";
			this.btPrint.UseVisualStyleBackColor = true;
			this.btPrint.Click += new System.EventHandler(this.btPrint_Click);
			// 
			// btPrintAll
			// 
			this.btPrintAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btPrintAll.Location = new System.Drawing.Point(704, 128);
			this.btPrintAll.Name = "btPrintAll";
			this.btPrintAll.Size = new System.Drawing.Size(84, 23);
			this.btPrintAll.TabIndex = 10;
			this.btPrintAll.Text = "Print All";
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
			this.btOptions.UseVisualStyleBackColor = true;
			this.btOptions.Click += new System.EventHandler(this.btOptions_Click);
			// 
			// miQuickEditOptions
			// 
			this.miQuickEditOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tiQuickEditWork,
            this.miQuickEditSchool});
			this.miQuickEditOptions.Name = "miQuickEditOptions";
			this.miQuickEditOptions.Size = new System.Drawing.Size(180, 22);
			this.miQuickEditOptions.Text = "Quick actions";
			// 
			// tiQuickEditWork
			// 
			this.tiQuickEditWork.Name = "tiQuickEditWork";
			this.tiQuickEditWork.Size = new System.Drawing.Size(180, 22);
			this.tiQuickEditWork.Text = "Edit work";
			this.tiQuickEditWork.Click += new System.EventHandler(this.miQuickEditWork_Click);
			// 
			// miQuickEditSchool
			// 
			this.miQuickEditSchool.Name = "miQuickEditSchool";
			this.miQuickEditSchool.Size = new System.Drawing.Size(180, 22);
			this.miQuickEditSchool.Text = "Edit school";
			this.miQuickEditSchool.Click += new System.EventHandler(this.miQuickEditSchool_Click);
			// 
			// FormManager
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.btOptions);
			this.Controls.Add(this.cbVisible);
			this.Controls.Add(this.btEditName);
			this.Controls.Add(this.btLogin);
			this.Controls.Add(this.btPrintAll);
			this.Controls.Add(this.btPrint);
			this.Controls.Add(this.btDelete);
			this.Controls.Add(this.btEditExisting);
			this.Controls.Add(this.tvReports);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.btEdit);
			this.Controls.Add(this.btSetNumber);
			this.Controls.Add(this.btCreate);
			this.Controls.Add(this.btSetTemplate);
			this.Controls.Add(this.btClose);
			this.MinimumSize = new System.Drawing.Size(545, 289);
			this.Name = "FormManager";
			this.Text = "Bericht Manager";
			this.toRightClickMenu.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btClose;
		private System.Windows.Forms.Button btSetTemplate;
		private System.Windows.Forms.Button btCreate;
		private System.Windows.Forms.Button btSetNumber;
		private System.Windows.Forms.Button btEdit;
		private System.Windows.Forms.Button button1;
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
		private System.Windows.Forms.ToolStripMenuItem miQuickEditOptions;
		private System.Windows.Forms.ToolStripMenuItem tiQuickEditWork;
		private System.Windows.Forms.ToolStripMenuItem miQuickEditSchool;
	}
}

