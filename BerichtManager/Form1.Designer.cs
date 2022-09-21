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
			this.btClose = new System.Windows.Forms.Button();
			this.btSetTemplate = new System.Windows.Forms.Button();
			this.btCreate = new System.Windows.Forms.Button();
			this.btSetNumber = new System.Windows.Forms.Button();
			this.btEdit = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.tvReports = new System.Windows.Forms.TreeView();
			this.btEditExisting = new System.Windows.Forms.Button();
			this.btDelete = new System.Windows.Forms.Button();
			this.btPrint = new System.Windows.Forms.Button();
			this.btPrintAll = new System.Windows.Forms.Button();
			this.btLogin = new System.Windows.Forms.Button();
			this.btEditName = new System.Windows.Forms.Button();
			this.cbVisible = new System.Windows.Forms.CheckBox();
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
			// btSetTemplate
			// 
			this.btSetTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btSetTemplate.Location = new System.Drawing.Point(547, 415);
			this.btSetTemplate.Name = "btSetTemplate";
			this.btSetTemplate.Size = new System.Drawing.Size(79, 23);
			this.btSetTemplate.TabIndex = 1;
			this.btSetTemplate.Text = "Set Template";
			this.btSetTemplate.UseVisualStyleBackColor = true;
			this.btSetTemplate.Click += new System.EventHandler(this.btSetTemplate_Click);
			// 
			// btCreate
			// 
			this.btCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btCreate.Location = new System.Drawing.Point(632, 415);
			this.btCreate.Name = "btCreate";
			this.btCreate.Size = new System.Drawing.Size(75, 23);
			this.btCreate.TabIndex = 2;
			this.btCreate.Text = "Create";
			this.btCreate.UseVisualStyleBackColor = true;
			this.btCreate.Click += new System.EventHandler(this.btCreate_Click);
			// 
			// btSetNumber
			// 
			this.btSetNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btSetNumber.Location = new System.Drawing.Point(466, 415);
			this.btSetNumber.Name = "btSetNumber";
			this.btSetNumber.Size = new System.Drawing.Size(75, 23);
			this.btSetNumber.TabIndex = 3;
			this.btSetNumber.Text = "Set Number";
			this.btSetNumber.UseVisualStyleBackColor = true;
			this.btSetNumber.Click += new System.EventHandler(this.btSetNumber_Click);
			// 
			// btEdit
			// 
			this.btEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btEdit.Location = new System.Drawing.Point(385, 415);
			this.btEdit.Name = "btEdit";
			this.btEdit.Size = new System.Drawing.Size(75, 23);
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
			this.button1.Text = "button1";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Visible = false;
			this.button1.Click += new System.EventHandler(this.btTest_Click);
			// 
			// tvReports
			// 
			this.tvReports.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tvReports.Location = new System.Drawing.Point(12, 12);
			this.tvReports.Name = "tvReports";
			this.tvReports.Size = new System.Drawing.Size(686, 397);
			this.tvReports.TabIndex = 6;
			// 
			// btEditExisting
			// 
			this.btEditExisting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btEditExisting.Location = new System.Drawing.Point(704, 12);
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
			this.btPrint.Location = new System.Drawing.Point(704, 41);
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
			this.btPrintAll.Location = new System.Drawing.Point(704, 70);
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
			this.btEditName.Location = new System.Drawing.Point(304, 415);
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
			// FormManager
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
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
			this.MinimumSize = new System.Drawing.Size(695, 202);
			this.Name = "FormManager";
			this.Text = "Bericht Manager";
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
	}
}

