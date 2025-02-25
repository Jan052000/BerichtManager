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
			components = new System.ComponentModel.Container();
			toRightClickMenu = new ContextMenuStrip(components);
			miDelete = new ToolStripMenuItem();
			miEdit = new ToolStripMenuItem();
			miQuickEditOptions = new ToolStripMenuItem();
			tiQuickEditWork = new ToolStripMenuItem();
			miQuickEditSchool = new ToolStripMenuItem();
			miQuickEditNumber = new ToolStripMenuItem();
			miPrint = new ToolStripMenuItem();
			miIHKOptions = new ToolStripMenuItem();
			miRcUpdateStatuses = new ToolStripMenuItem();
			miUploadAsNext = new ToolStripMenuItem();
			miUploadAllSelected = new ToolStripMenuItem();
			miHandInSingle = new ToolStripMenuItem();
			miRcHandInSelection = new ToolStripMenuItem();
			miUpdateReport = new ToolStripMenuItem();
			miRCUpdateSelection = new ToolStripMenuItem();
			miRcShowComment = new ToolStripMenuItem();
			miRcCheckFormat = new ToolStripMenuItem();
			miRcDownloadReports = new ToolStripMenuItem();
			miRefresh = new ToolStripMenuItem();
			ttTips = new ToolTip(components);
			rtbWork = new RichTextBox();
			rtbSchool = new RichTextBox();
			menuStrip1 = new MenuStrip();
			miFile = new ToolStripMenuItem();
			miCreate = new ToolStripMenuItem();
			miEditLatest = new ToolStripMenuItem();
			miPrintAll = new ToolStripMenuItem();
			miCheckDiscrepancy = new ToolStripMenuItem();
			miNumbers = new ToolStripMenuItem();
			miDates = new ToolStripMenuItem();
			miFullCheck = new ToolStripMenuItem();
			miIHK = new ToolStripMenuItem();
			miUpdateStatuses = new ToolStripMenuItem();
			miUploadSelection = new ToolStripMenuItem();
			miHandInSelection = new ToolStripMenuItem();
			miUpdateSelection = new ToolStripMenuItem();
			miCheckFormat = new ToolStripMenuItem();
			miDownloadReports = new ToolStripMenuItem();
			miCloseReport = new ToolStripMenuItem();
			miRevealInExplorer = new ToolStripMenuItem();
			miOptions = new ToolStripMenuItem();
			miOptionsMenu = new ToolStripMenuItem();
			miWordVisible = new ToolStripMenuItem();
			miClose = new ToolStripMenuItem();
			paMainView = new Panel();
			scTextBoxes = new SplitContainer();
			paPadding1 = new Panel();
			paPadding2 = new Panel();
			splitterTreeBoxes = new Splitter();
			paFileTree = new Panel();
			tvReports = new OwnControls.OwnTreeView.CustomTreeView();
			toRightClickMenu.SuspendLayout();
			menuStrip1.SuspendLayout();
			paMainView.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)scTextBoxes).BeginInit();
			scTextBoxes.Panel1.SuspendLayout();
			scTextBoxes.Panel2.SuspendLayout();
			scTextBoxes.SuspendLayout();
			paPadding1.SuspendLayout();
			paPadding2.SuspendLayout();
			paFileTree.SuspendLayout();
			SuspendLayout();
			// 
			// toRightClickMenu
			// 
			toRightClickMenu.Items.AddRange(new ToolStripItem[] { miDelete, miEdit, miQuickEditOptions, miPrint, miIHKOptions, miRefresh });
			toRightClickMenu.Name = "contextMenuStrip1";
			toRightClickMenu.Size = new Size(147, 136);
			toRightClickMenu.Opening += toRightClickMenu_Opening;
			// 
			// miDelete
			// 
			miDelete.Name = "miDelete";
			miDelete.Size = new Size(146, 22);
			miDelete.Text = "Delete";
			miDelete.Click += miDelete_Click;
			// 
			// miEdit
			// 
			miEdit.Name = "miEdit";
			miEdit.Size = new Size(146, 22);
			miEdit.Text = "Edit";
			miEdit.Click += miEdit_Click;
			// 
			// miQuickEditOptions
			// 
			miQuickEditOptions.DropDownItems.AddRange(new ToolStripItem[] { tiQuickEditWork, miQuickEditSchool, miQuickEditNumber });
			miQuickEditOptions.Name = "miQuickEditOptions";
			miQuickEditOptions.Size = new Size(146, 22);
			miQuickEditOptions.Text = "Quick actions";
			// 
			// tiQuickEditWork
			// 
			tiQuickEditWork.Name = "tiQuickEditWork";
			tiQuickEditWork.Size = new Size(174, 22);
			tiQuickEditWork.Text = "Edit work";
			tiQuickEditWork.Click += miQuickEditWork_Click;
			// 
			// miQuickEditSchool
			// 
			miQuickEditSchool.Name = "miQuickEditSchool";
			miQuickEditSchool.Size = new Size(174, 22);
			miQuickEditSchool.Text = "Edit school";
			miQuickEditSchool.Click += miQuickEditSchool_Click;
			// 
			// miQuickEditNumber
			// 
			miQuickEditNumber.Name = "miQuickEditNumber";
			miQuickEditNumber.Size = new Size(174, 22);
			miQuickEditNumber.Text = "Edit report number";
			miQuickEditNumber.Click += miQuickEditNumber_Click;
			// 
			// miPrint
			// 
			miPrint.Name = "miPrint";
			miPrint.Size = new Size(146, 22);
			miPrint.Text = "Print";
			miPrint.Click += miPrint_Click;
			// 
			// miIHKOptions
			// 
			miIHKOptions.DropDownItems.AddRange(new ToolStripItem[] { miRcUpdateStatuses, miUploadAsNext, miUploadAllSelected, miHandInSingle, miRcHandInSelection, miUpdateReport, miRCUpdateSelection, miRcShowComment, miRcCheckFormat, miRcDownloadReports });
			miIHKOptions.Name = "miIHKOptions";
			miIHKOptions.Size = new Size(146, 22);
			miIHKOptions.Text = "IHK";
			miIHKOptions.ToolTipText = "All options for interactions with IHK";
			// 
			// miRcUpdateStatuses
			// 
			miRcUpdateStatuses.Name = "miRcUpdateStatuses";
			miRcUpdateStatuses.Size = new Size(168, 22);
			miRcUpdateStatuses.Text = "Update statuses";
			miRcUpdateStatuses.ToolTipText = "Update statuses of reports";
			miRcUpdateStatuses.Click += miUpdateStatuses_Click;
			// 
			// miUploadAsNext
			// 
			miUploadAsNext.Name = "miUploadAsNext";
			miUploadAsNext.Size = new Size(168, 22);
			miUploadAsNext.Text = "Upload";
			miUploadAsNext.ToolTipText = "Upload the selected report as next report";
			miUploadAsNext.Click += miUploadAsNext_Click;
			// 
			// miUploadAllSelected
			// 
			miUploadAllSelected.Name = "miUploadAllSelected";
			miUploadAllSelected.Size = new Size(168, 22);
			miUploadAllSelected.Text = "Upload selection";
			miUploadAllSelected.ToolTipText = "Upload all reports selected in following form";
			miUploadAllSelected.Click += UploadSelectionClick;
			// 
			// miHandInSingle
			// 
			miHandInSingle.Name = "miHandInSingle";
			miHandInSingle.Size = new Size(168, 22);
			miHandInSingle.Text = "Hand in";
			miHandInSingle.ToolTipText = "Hand in this report";
			miHandInSingle.Click += miHandInSingle_Click;
			// 
			// miRcHandInSelection
			// 
			miRcHandInSelection.Name = "miRcHandInSelection";
			miRcHandInSelection.Size = new Size(168, 22);
			miRcHandInSelection.Text = "Hand in selection";
			miRcHandInSelection.ToolTipText = "Hand in all reports selected in following form";
			miRcHandInSelection.Click += HandInSelectionClick;
			// 
			// miUpdateReport
			// 
			miUpdateReport.Name = "miUpdateReport";
			miUpdateReport.Size = new Size(168, 22);
			miUpdateReport.Text = "Update report";
			miUpdateReport.ToolTipText = "Upload local changes to IHK";
			miUpdateReport.Click += SendReportToIHK;
			// 
			// miRCUpdateSelection
			// 
			miRCUpdateSelection.Name = "miRCUpdateSelection";
			miRCUpdateSelection.Size = new Size(168, 22);
			miRCUpdateSelection.Text = "Update selection";
			miRCUpdateSelection.Click += SendSelectionToIHK;
			// 
			// miRcShowComment
			// 
			miRcShowComment.Name = "miRcShowComment";
			miRcShowComment.Size = new Size(168, 22);
			miRcShowComment.Text = "Show comment";
			miRcShowComment.Click += miRcShowComment_Click;
			// 
			// miRcCheckFormat
			// 
			miRcCheckFormat.Name = "miRcCheckFormat";
			miRcCheckFormat.Size = new Size(168, 22);
			miRcCheckFormat.Text = "Check format";
			miRcCheckFormat.Click += CheckFormat;
			// 
			// miRcDownloadReports
			// 
			miRcDownloadReports.Name = "miRcDownloadReports";
			miRcDownloadReports.Size = new Size(168, 22);
			miRcDownloadReports.Text = "Download reports";
			miRcDownloadReports.Click += DownloadIHKReports;
			// 
			// miRefresh
			// 
			miRefresh.Name = "miRefresh";
			miRefresh.Size = new Size(146, 22);
			miRefresh.Text = "Refresh";
			miRefresh.Click += MiRefresh_Click;
			// 
			// rtbWork
			// 
			rtbWork.AcceptsTab = true;
			rtbWork.Dock = DockStyle.Fill;
			rtbWork.Location = new Point(0, 3);
			rtbWork.Margin = new Padding(0);
			rtbWork.Name = "rtbWork";
			rtbWork.Size = new Size(651, 242);
			rtbWork.TabIndex = 0;
			rtbWork.Text = "";
			ttTips.SetToolTip(rtbWork, "Work");
			rtbWork.WordWrap = false;
			rtbWork.TextChanged += EditRichTextBox;
			rtbWork.KeyDown += OnKeyDownDefault;
			// 
			// rtbSchool
			// 
			rtbSchool.AcceptsTab = true;
			rtbSchool.Dock = DockStyle.Fill;
			rtbSchool.Location = new Point(0, 0);
			rtbSchool.Margin = new Padding(0);
			rtbSchool.Name = "rtbSchool";
			rtbSchool.Size = new Size(651, 244);
			rtbSchool.TabIndex = 0;
			rtbSchool.Text = "";
			ttTips.SetToolTip(rtbSchool, "School");
			rtbSchool.WordWrap = false;
			rtbSchool.TextChanged += EditRichTextBox;
			rtbSchool.KeyDown += OnKeyDownDefault;
			// 
			// menuStrip1
			// 
			menuStrip1.Items.AddRange(new ToolStripItem[] { miFile, miOptions, miClose });
			menuStrip1.Location = new Point(0, 0);
			menuStrip1.Name = "menuStrip1";
			menuStrip1.Padding = new Padding(7, 2, 0, 2);
			menuStrip1.Size = new Size(933, 24);
			menuStrip1.TabIndex = 1;
			menuStrip1.TabStop = true;
			menuStrip1.Text = "menuStrip1";
			menuStrip1.Paint += menuStrip1_Paint;
			menuStrip1.KeyDown += OnKeyDownDefault;
			// 
			// miFile
			// 
			miFile.DropDownItems.AddRange(new ToolStripItem[] { miCreate, miEditLatest, miPrintAll, miCheckDiscrepancy, miIHK, miCloseReport, miRevealInExplorer });
			miFile.Name = "miFile";
			miFile.Size = new Size(37, 20);
			miFile.Text = "File";
			// 
			// miCreate
			// 
			miCreate.Name = "miCreate";
			miCreate.Size = new Size(191, 22);
			miCreate.Text = "Create";
			miCreate.Click += btCreate_Click;
			// 
			// miEditLatest
			// 
			miEditLatest.Name = "miEditLatest";
			miEditLatest.Size = new Size(191, 22);
			miEditLatest.Text = "Edit latest";
			miEditLatest.Click += miEditLatest_Click;
			// 
			// miPrintAll
			// 
			miPrintAll.Name = "miPrintAll";
			miPrintAll.Size = new Size(191, 22);
			miPrintAll.Text = "Print all";
			miPrintAll.Click += btPrintAll_Click;
			// 
			// miCheckDiscrepancy
			// 
			miCheckDiscrepancy.DropDownItems.AddRange(new ToolStripItem[] { miNumbers, miDates, miFullCheck });
			miCheckDiscrepancy.Name = "miCheckDiscrepancy";
			miCheckDiscrepancy.Size = new Size(191, 22);
			miCheckDiscrepancy.Text = "Check for discrepancy";
			// 
			// miNumbers
			// 
			miNumbers.Name = "miNumbers";
			miNumbers.Size = new Size(157, 22);
			miNumbers.Text = "Check numbers";
			miNumbers.ToolTipText = "Check if report numbers are continuous";
			miNumbers.Click += CheckNumbers_Click;
			// 
			// miDates
			// 
			miDates.Name = "miDates";
			miDates.Size = new Size(157, 22);
			miDates.Text = "Check dates";
			miDates.Click += CheckDates_Click;
			// 
			// miFullCheck
			// 
			miFullCheck.Name = "miFullCheck";
			miFullCheck.Size = new Size(157, 22);
			miFullCheck.Text = "Full check";
			miFullCheck.Click += FullCheck_Click;
			// 
			// miIHK
			// 
			miIHK.DropDownItems.AddRange(new ToolStripItem[] { miUpdateStatuses, miUploadSelection, miHandInSelection, miUpdateSelection, miCheckFormat, miDownloadReports });
			miIHK.Name = "miIHK";
			miIHK.Size = new Size(191, 22);
			miIHK.Text = "IHK";
			miIHK.ToolTipText = "All options for interactions with IHK";
			// 
			// miUpdateStatuses
			// 
			miUpdateStatuses.Name = "miUpdateStatuses";
			miUpdateStatuses.Size = new Size(168, 22);
			miUpdateStatuses.Text = "Update statuses";
			miUpdateStatuses.ToolTipText = "Update statuses of reports";
			miUpdateStatuses.Click += miUpdateStatuses_Click;
			// 
			// miUploadSelection
			// 
			miUploadSelection.Name = "miUploadSelection";
			miUploadSelection.Size = new Size(168, 22);
			miUploadSelection.Text = "Upload selection";
			miUploadSelection.ToolTipText = "Upload all reports selected in following form";
			miUploadSelection.Click += UploadSelectionClick;
			// 
			// miHandInSelection
			// 
			miHandInSelection.Name = "miHandInSelection";
			miHandInSelection.Size = new Size(168, 22);
			miHandInSelection.Text = "Hand in selection";
			miHandInSelection.ToolTipText = "Hand in all reports selected in following form";
			miHandInSelection.Click += HandInSelectionClick;
			// 
			// miUpdateSelection
			// 
			miUpdateSelection.Name = "miUpdateSelection";
			miUpdateSelection.Size = new Size(168, 22);
			miUpdateSelection.Text = "Update selection";
			miUpdateSelection.Click += SendSelectionToIHK;
			// 
			// miCheckFormat
			// 
			miCheckFormat.Name = "miCheckFormat";
			miCheckFormat.Size = new Size(168, 22);
			miCheckFormat.Text = "Check format";
			miCheckFormat.Click += CheckFormat;
			// 
			// miDownloadReports
			// 
			miDownloadReports.Name = "miDownloadReports";
			miDownloadReports.Size = new Size(168, 22);
			miDownloadReports.Text = "Download reports";
			miDownloadReports.Click += DownloadIHKReports;
			// 
			// miCloseReport
			// 
			miCloseReport.Name = "miCloseReport";
			miCloseReport.Size = new Size(191, 22);
			miCloseReport.Text = "Close Report";
			miCloseReport.Click += miCloseReport_Click;
			// 
			// miRevealInExplorer
			// 
			miRevealInExplorer.Name = "miRevealInExplorer";
			miRevealInExplorer.Size = new Size(191, 22);
			miRevealInExplorer.Text = "Reveal in explorer";
			miRevealInExplorer.Click += miRevealInExplorer_Click;
			// 
			// miOptions
			// 
			miOptions.DropDownItems.AddRange(new ToolStripItem[] { miOptionsMenu, miWordVisible });
			miOptions.Name = "miOptions";
			miOptions.Size = new Size(61, 20);
			miOptions.Text = "Options";
			// 
			// miOptionsMenu
			// 
			miOptionsMenu.Name = "miOptionsMenu";
			miOptionsMenu.Size = new Size(150, 22);
			miOptionsMenu.Text = "Options menu";
			miOptionsMenu.Click += btOptions_Click;
			// 
			// miWordVisible
			// 
			miWordVisible.CheckOnClick = true;
			miWordVisible.Name = "miWordVisible";
			miWordVisible.Size = new Size(150, 22);
			miWordVisible.Text = "Word visible";
			miWordVisible.Click += miWordVisible_Click;
			// 
			// miClose
			// 
			miClose.Alignment = ToolStripItemAlignment.Right;
			miClose.Enabled = false;
			miClose.Name = "miClose";
			miClose.Size = new Size(48, 20);
			miClose.Text = "Close";
			miClose.Visible = false;
			miClose.Click += btClose_Click;
			// 
			// paMainView
			// 
			paMainView.Controls.Add(scTextBoxes);
			paMainView.Controls.Add(splitterTreeBoxes);
			paMainView.Controls.Add(paFileTree);
			paMainView.Dock = DockStyle.Fill;
			paMainView.Location = new Point(0, 24);
			paMainView.Margin = new Padding(0, 0, 4, 3);
			paMainView.Name = "paMainView";
			paMainView.Size = new Size(933, 495);
			paMainView.TabIndex = 2;
			// 
			// scTextBoxes
			// 
			scTextBoxes.Dock = DockStyle.Fill;
			scTextBoxes.Location = new Point(278, 0);
			scTextBoxes.Margin = new Padding(4, 3, 4, 3);
			scTextBoxes.Name = "scTextBoxes";
			scTextBoxes.Orientation = Orientation.Horizontal;
			// 
			// scTextBoxes.Panel1
			// 
			scTextBoxes.Panel1.Controls.Add(paPadding1);
			// 
			// scTextBoxes.Panel2
			// 
			scTextBoxes.Panel2.Controls.Add(paPadding2);
			scTextBoxes.Size = new Size(655, 495);
			scTextBoxes.SplitterDistance = 245;
			scTextBoxes.SplitterWidth = 3;
			scTextBoxes.TabIndex = 1;
			scTextBoxes.TabStop = false;
			// 
			// paPadding1
			// 
			paPadding1.Controls.Add(rtbWork);
			paPadding1.Dock = DockStyle.Fill;
			paPadding1.Location = new Point(0, 0);
			paPadding1.Margin = new Padding(4, 3, 4, 3);
			paPadding1.Name = "paPadding1";
			paPadding1.Padding = new Padding(0, 3, 4, 0);
			paPadding1.Size = new Size(655, 245);
			paPadding1.TabIndex = 0;
			// 
			// paPadding2
			// 
			paPadding2.Controls.Add(rtbSchool);
			paPadding2.Dock = DockStyle.Fill;
			paPadding2.Location = new Point(0, 0);
			paPadding2.Margin = new Padding(4, 3, 4, 3);
			paPadding2.Name = "paPadding2";
			paPadding2.Padding = new Padding(0, 0, 4, 3);
			paPadding2.Size = new Size(655, 247);
			paPadding2.TabIndex = 0;
			// 
			// splitterTreeBoxes
			// 
			splitterTreeBoxes.Location = new Point(274, 0);
			splitterTreeBoxes.Margin = new Padding(4, 3, 4, 3);
			splitterTreeBoxes.Name = "splitterTreeBoxes";
			splitterTreeBoxes.Size = new Size(4, 495);
			splitterTreeBoxes.TabIndex = 1;
			splitterTreeBoxes.TabStop = false;
			// 
			// paFileTree
			// 
			paFileTree.Controls.Add(tvReports);
			paFileTree.Dock = DockStyle.Left;
			paFileTree.Location = new Point(0, 0);
			paFileTree.Margin = new Padding(4, 3, 4, 3);
			paFileTree.Name = "paFileTree";
			paFileTree.Size = new Size(274, 495);
			paFileTree.TabIndex = 0;
			// 
			// tvReports
			// 
			tvReports.BorderStyle = BorderStyle.None;
			tvReports.ContextMenuStrip = toRightClickMenu;
			tvReports.Dock = DockStyle.Fill;
			tvReports.DrawMode = TreeViewDrawMode.OwnerDrawAll;
			tvReports.Location = new Point(0, 0);
			tvReports.Margin = new Padding(0);
			tvReports.Name = "tvReports";
			tvReports.Size = new Size(274, 495);
			tvReports.TabIndex = 0;
			tvReports.Click += tvReports_Click;
			tvReports.DoubleClick += tvReports_DoubleClick;
			tvReports.KeyDown += OnKeyDownDefault;
			tvReports.KeyUp += tvReports_KeyUp;
			// 
			// MainForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			BackColor = SystemColors.Window;
			ClientSize = new Size(933, 519);
			Controls.Add(paMainView);
			Controls.Add(menuStrip1);
			MainMenuStrip = menuStrip1;
			Margin = new Padding(4, 3, 4, 3);
			MinimumSize = new Size(633, 327);
			Name = "MainForm";
			Text = "Bericht Manager";
			FormClosing += MainForm_FormClosing;
			Shown += MainForm_Load;
			KeyDown += OnKeyDownDefault;
			toRightClickMenu.ResumeLayout(false);
			menuStrip1.ResumeLayout(false);
			menuStrip1.PerformLayout();
			paMainView.ResumeLayout(false);
			scTextBoxes.Panel1.ResumeLayout(false);
			scTextBoxes.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)scTextBoxes).EndInit();
			scTextBoxes.ResumeLayout(false);
			paPadding1.ResumeLayout(false);
			paPadding2.ResumeLayout(false);
			paFileTree.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion
		private System.Windows.Forms.ContextMenuStrip toRightClickMenu;
		private System.Windows.Forms.ToolStripMenuItem miDelete;
		private System.Windows.Forms.ToolStripMenuItem miEdit;
		private System.Windows.Forms.ToolStripMenuItem miPrint;
		private System.Windows.Forms.ToolTip ttTips;
		private System.Windows.Forms.ToolStripMenuItem miQuickEditOptions;
		private System.Windows.Forms.ToolStripMenuItem tiQuickEditWork;
		private System.Windows.Forms.ToolStripMenuItem miQuickEditSchool;
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
		private System.Windows.Forms.ToolStripMenuItem miRevealInExplorer;
		private System.Windows.Forms.ToolStripMenuItem miCloseReport;
		private System.Windows.Forms.ToolStripMenuItem miCheckDiscrepancy;
		private System.Windows.Forms.ToolStripMenuItem miNumbers;
		private System.Windows.Forms.ToolStripMenuItem miDates;
		private System.Windows.Forms.ToolStripMenuItem miFullCheck;
		private System.Windows.Forms.ToolStripMenuItem miIHKOptions;
		private System.Windows.Forms.ToolStripMenuItem miUploadAsNext;
		private System.Windows.Forms.ToolStripMenuItem miUploadAllSelected;
		private System.Windows.Forms.ToolStripMenuItem miUploadSelection;
		private System.Windows.Forms.ToolStripMenuItem miUpdateStatuses;
		private System.Windows.Forms.ToolStripMenuItem miRcUpdateStatuses;
		private System.Windows.Forms.ToolStripMenuItem miIHK;
		private System.Windows.Forms.ToolStripMenuItem miHandInSingle;
		private System.Windows.Forms.ToolStripMenuItem miHandInSelection;
		private System.Windows.Forms.ToolStripMenuItem miRcHandInSelection;
		private System.Windows.Forms.ToolStripMenuItem miUpdateReport;
		private System.Windows.Forms.ToolStripMenuItem miCheckFormat;
		private System.Windows.Forms.ToolStripMenuItem miRcCheckFormat;
		private System.Windows.Forms.ToolStripMenuItem miRCUpdateSelection;
		private System.Windows.Forms.ToolStripMenuItem miUpdateSelection;
		private System.Windows.Forms.ToolStripMenuItem miRcShowComment;
		private System.Windows.Forms.ToolStripMenuItem miRcDownloadReports;
		private System.Windows.Forms.ToolStripMenuItem miDownloadReports;
		private System.Windows.Forms.ToolStripMenuItem miQuickEditNumber;
		private Panel paMainView;
		private SplitContainer scTextBoxes;
		private Panel paPadding1;
		private RichTextBox rtbWork;
		private Panel paPadding2;
		private RichTextBox rtbSchool;
		private Splitter splitterTreeBoxes;
		private Panel paFileTree;
		private OwnControls.OwnTreeView.CustomTreeView tvReports;
	}
}

