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
			this.toRightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.miDelete = new System.Windows.Forms.ToolStripMenuItem();
			this.miEdit = new System.Windows.Forms.ToolStripMenuItem();
			this.miQuickEditOptions = new System.Windows.Forms.ToolStripMenuItem();
			this.tiQuickEditWork = new System.Windows.Forms.ToolStripMenuItem();
			this.miQuickEditSchool = new System.Windows.Forms.ToolStripMenuItem();
			this.miQuickEditNumber = new System.Windows.Forms.ToolStripMenuItem();
			this.miPrint = new System.Windows.Forms.ToolStripMenuItem();
			this.miIHKOptions = new System.Windows.Forms.ToolStripMenuItem();
			this.miRcUpdateStatuses = new System.Windows.Forms.ToolStripMenuItem();
			this.miUploadAsNext = new System.Windows.Forms.ToolStripMenuItem();
			this.miUploadAllSelected = new System.Windows.Forms.ToolStripMenuItem();
			this.miHandInSingle = new System.Windows.Forms.ToolStripMenuItem();
			this.miRcHandInSelection = new System.Windows.Forms.ToolStripMenuItem();
			this.miUpdateReport = new System.Windows.Forms.ToolStripMenuItem();
			this.miRCUpdateSelection = new System.Windows.Forms.ToolStripMenuItem();
			this.miRcShowComment = new System.Windows.Forms.ToolStripMenuItem();
			this.miRcCheckFormat = new System.Windows.Forms.ToolStripMenuItem();
			this.miRcDownloadReports = new System.Windows.Forms.ToolStripMenuItem();
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
			this.tvReports = new BerichtManager.OwnControls.OwnTreeView.CustomTreeView();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.miFile = new System.Windows.Forms.ToolStripMenuItem();
			this.miCreate = new System.Windows.Forms.ToolStripMenuItem();
			this.miEditLatest = new System.Windows.Forms.ToolStripMenuItem();
			this.miPrintAll = new System.Windows.Forms.ToolStripMenuItem();
			this.miCheckDiscrepancy = new System.Windows.Forms.ToolStripMenuItem();
			this.miNumbers = new System.Windows.Forms.ToolStripMenuItem();
			this.miDates = new System.Windows.Forms.ToolStripMenuItem();
			this.miFullCheck = new System.Windows.Forms.ToolStripMenuItem();
			this.miIHK = new System.Windows.Forms.ToolStripMenuItem();
			this.miUpdateStatuses = new System.Windows.Forms.ToolStripMenuItem();
			this.miUploadSelection = new System.Windows.Forms.ToolStripMenuItem();
			this.miHandInSelection = new System.Windows.Forms.ToolStripMenuItem();
			this.miUpdateSelection = new System.Windows.Forms.ToolStripMenuItem();
			this.miCheckFormat = new System.Windows.Forms.ToolStripMenuItem();
			this.miDownloadReports = new System.Windows.Forms.ToolStripMenuItem();
			this.miCloseReport = new System.Windows.Forms.ToolStripMenuItem();
			this.miRevealInExplorer = new System.Windows.Forms.ToolStripMenuItem();
			this.miOptions = new System.Windows.Forms.ToolStripMenuItem();
			this.miOptionsMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.miWordVisible = new System.Windows.Forms.ToolStripMenuItem();
			this.miClose = new System.Windows.Forms.ToolStripMenuItem();
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
			// toRightClickMenu
			// 
			this.toRightClickMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miDelete,
            this.miEdit,
            this.miQuickEditOptions,
            this.miPrint,
            this.miIHKOptions,
            this.miRefresh});
			this.toRightClickMenu.Name = "contextMenuStrip1";
			this.toRightClickMenu.Size = new System.Drawing.Size(147, 136);
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
            this.miQuickEditSchool,
            this.miQuickEditNumber});
			this.miQuickEditOptions.Name = "miQuickEditOptions";
			this.miQuickEditOptions.Size = new System.Drawing.Size(146, 22);
			this.miQuickEditOptions.Text = "Quick actions";
			// 
			// tiQuickEditWork
			// 
			this.tiQuickEditWork.Name = "tiQuickEditWork";
			this.tiQuickEditWork.Size = new System.Drawing.Size(174, 22);
			this.tiQuickEditWork.Text = "Edit work";
			this.tiQuickEditWork.Click += new System.EventHandler(this.miQuickEditWork_Click);
			// 
			// miQuickEditSchool
			// 
			this.miQuickEditSchool.Name = "miQuickEditSchool";
			this.miQuickEditSchool.Size = new System.Drawing.Size(174, 22);
			this.miQuickEditSchool.Text = "Edit school";
			this.miQuickEditSchool.Click += new System.EventHandler(this.miQuickEditSchool_Click);
			// 
			// miQuickEditNumber
			// 
			this.miQuickEditNumber.Name = "miQuickEditNumber";
			this.miQuickEditNumber.Size = new System.Drawing.Size(174, 22);
			this.miQuickEditNumber.Text = "Edit report number";
			this.miQuickEditNumber.Click += new System.EventHandler(this.miQuickEditNumber_Click);
			// 
			// miPrint
			// 
			this.miPrint.Name = "miPrint";
			this.miPrint.Size = new System.Drawing.Size(146, 22);
			this.miPrint.Text = "Print";
			this.miPrint.Click += new System.EventHandler(this.miPrint_Click);
			// 
			// miIHKOptions
			// 
			this.miIHKOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miRcUpdateStatuses,
            this.miUploadAsNext,
            this.miUploadAllSelected,
            this.miHandInSingle,
            this.miRcHandInSelection,
            this.miUpdateReport,
            this.miRCUpdateSelection,
            this.miRcShowComment,
            this.miRcCheckFormat,
            this.miRcDownloadReports});
			this.miIHKOptions.Name = "miIHKOptions";
			this.miIHKOptions.Size = new System.Drawing.Size(146, 22);
			this.miIHKOptions.Text = "IHK";
			this.miIHKOptions.ToolTipText = "All options for interactions with IHK";
			// 
			// miRcUpdateStatuses
			// 
			this.miRcUpdateStatuses.Name = "miRcUpdateStatuses";
			this.miRcUpdateStatuses.Size = new System.Drawing.Size(168, 22);
			this.miRcUpdateStatuses.Text = "Update statuses";
			this.miRcUpdateStatuses.ToolTipText = "Update statuses of reports";
			this.miRcUpdateStatuses.Click += new System.EventHandler(this.miUpdateStatuses_Click);
			// 
			// miUploadAsNext
			// 
			this.miUploadAsNext.Name = "miUploadAsNext";
			this.miUploadAsNext.Size = new System.Drawing.Size(168, 22);
			this.miUploadAsNext.Text = "Upload";
			this.miUploadAsNext.ToolTipText = "Upload the selected report as next report";
			this.miUploadAsNext.Click += new System.EventHandler(this.miUploadAsNext_Click);
			// 
			// miUploadAllSelected
			// 
			this.miUploadAllSelected.Name = "miUploadAllSelected";
			this.miUploadAllSelected.Size = new System.Drawing.Size(168, 22);
			this.miUploadAllSelected.Text = "Upload selection";
			this.miUploadAllSelected.ToolTipText = "Upload all reports selected in following form";
			this.miUploadAllSelected.Click += new System.EventHandler(this.UploadSelectionClick);
			// 
			// miHandInSingle
			// 
			this.miHandInSingle.Name = "miHandInSingle";
			this.miHandInSingle.Size = new System.Drawing.Size(168, 22);
			this.miHandInSingle.Text = "Hand in";
			this.miHandInSingle.ToolTipText = "Hand in this report";
			this.miHandInSingle.Click += new System.EventHandler(this.miHandInSingle_Click);
			// 
			// miRcHandInSelection
			// 
			this.miRcHandInSelection.Name = "miRcHandInSelection";
			this.miRcHandInSelection.Size = new System.Drawing.Size(168, 22);
			this.miRcHandInSelection.Text = "Hand in selection";
			this.miRcHandInSelection.ToolTipText = "Hand in all reports selected in following form";
			this.miRcHandInSelection.Click += new System.EventHandler(this.HandInSelectionClick);
			// 
			// miUpdateReport
			// 
			this.miUpdateReport.Name = "miUpdateReport";
			this.miUpdateReport.Size = new System.Drawing.Size(168, 22);
			this.miUpdateReport.Text = "Update report";
			this.miUpdateReport.ToolTipText = "Upload local changes to IHK";
			this.miUpdateReport.Click += new System.EventHandler(this.SendReportToIHK);
			// 
			// miRCUpdateSelection
			// 
			this.miRCUpdateSelection.Name = "miRCUpdateSelection";
			this.miRCUpdateSelection.Size = new System.Drawing.Size(168, 22);
			this.miRCUpdateSelection.Text = "Update selection";
			this.miRCUpdateSelection.Click += new System.EventHandler(this.SendSelectionToIHK);
			// 
			// miRcShowComment
			// 
			this.miRcShowComment.Name = "miRcShowComment";
			this.miRcShowComment.Size = new System.Drawing.Size(168, 22);
			this.miRcShowComment.Text = "Show comment";
			this.miRcShowComment.Click += new System.EventHandler(this.miRcShowComment_Click);
			// 
			// miRcCheckFormat
			// 
			this.miRcCheckFormat.Name = "miRcCheckFormat";
			this.miRcCheckFormat.Size = new System.Drawing.Size(168, 22);
			this.miRcCheckFormat.Text = "Check format";
			this.miRcCheckFormat.Click += new System.EventHandler(this.CheckFormat);
			// 
			// miRcDownloadReports
			// 
			this.miRcDownloadReports.Name = "miRcDownloadReports";
			this.miRcDownloadReports.Size = new System.Drawing.Size(168, 22);
			this.miRcDownloadReports.Text = "Download reports";
			this.miRcDownloadReports.Click += new System.EventHandler(this.DownloadIHKReports);
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
			this.rtbWork.TabIndex = 0;
			this.rtbWork.Text = "";
			this.ttTips.SetToolTip(this.rtbWork, "Work");
			this.rtbWork.WordWrap = false;
			this.rtbWork.TextChanged += new System.EventHandler(this.EditRichTextBox);
			this.rtbWork.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDownDefault);
			// 
			// rtbSchool
			// 
			this.rtbSchool.AcceptsTab = true;
			this.rtbSchool.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtbSchool.Location = new System.Drawing.Point(0, 0);
			this.rtbSchool.Margin = new System.Windows.Forms.Padding(0);
			this.rtbSchool.Name = "rtbSchool";
			this.rtbSchool.Size = new System.Drawing.Size(559, 210);
			this.rtbSchool.TabIndex = 0;
			this.rtbSchool.Text = "";
			this.ttTips.SetToolTip(this.rtbSchool, "School");
			this.rtbSchool.WordWrap = false;
			this.rtbSchool.TextChanged += new System.EventHandler(this.EditRichTextBox);
			this.rtbSchool.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDownDefault);
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
			this.paMainView.TabIndex = 0;
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
			this.scTextBoxes.TabIndex = 1;
			this.scTextBoxes.TabStop = false;
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
			this.tvReports.TabIndex = 0;
			this.tvReports.Click += new System.EventHandler(this.tvReports_Click);
			this.tvReports.DoubleClick += new System.EventHandler(this.tvReports_DoubleClick);
			this.tvReports.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDownDefault);
			this.tvReports.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tvReports_KeyUp);
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
			this.menuStrip1.TabIndex = 1;
			this.menuStrip1.TabStop = true;
			this.menuStrip1.Text = "menuStrip1";
			this.menuStrip1.Paint += new System.Windows.Forms.PaintEventHandler(this.menuStrip1_Paint);
			this.menuStrip1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDownDefault);
			// 
			// miFile
			// 
			this.miFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miCreate,
            this.miEditLatest,
            this.miPrintAll,
            this.miCheckDiscrepancy,
            this.miIHK,
            this.miCloseReport,
            this.miRevealInExplorer});
			this.miFile.Name = "miFile";
			this.miFile.Size = new System.Drawing.Size(37, 20);
			this.miFile.Text = "File";
			// 
			// miCreate
			// 
			this.miCreate.Name = "miCreate";
			this.miCreate.Size = new System.Drawing.Size(191, 22);
			this.miCreate.Text = "Create";
			this.miCreate.Click += new System.EventHandler(this.btCreate_Click);
			// 
			// miEditLatest
			// 
			this.miEditLatest.Name = "miEditLatest";
			this.miEditLatest.Size = new System.Drawing.Size(191, 22);
			this.miEditLatest.Text = "Edit latest";
			this.miEditLatest.Click += new System.EventHandler(this.btEdit_Click);
			// 
			// miPrintAll
			// 
			this.miPrintAll.Name = "miPrintAll";
			this.miPrintAll.Size = new System.Drawing.Size(191, 22);
			this.miPrintAll.Text = "Print all";
			this.miPrintAll.Click += new System.EventHandler(this.btPrintAll_Click);
			// 
			// miCheckDiscrepancy
			// 
			this.miCheckDiscrepancy.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miNumbers,
            this.miDates,
            this.miFullCheck});
			this.miCheckDiscrepancy.Name = "miCheckDiscrepancy";
			this.miCheckDiscrepancy.Size = new System.Drawing.Size(191, 22);
			this.miCheckDiscrepancy.Text = "Check for discrepancy";
			// 
			// miNumbers
			// 
			this.miNumbers.Name = "miNumbers";
			this.miNumbers.Size = new System.Drawing.Size(157, 22);
			this.miNumbers.Text = "Check numbers";
			this.miNumbers.ToolTipText = "Check if report numbers are continuous";
			this.miNumbers.Click += new System.EventHandler(this.CheckNumbers_Click);
			// 
			// miDates
			// 
			this.miDates.Name = "miDates";
			this.miDates.Size = new System.Drawing.Size(157, 22);
			this.miDates.Text = "Check dates";
			this.miDates.Click += new System.EventHandler(this.CheckDates_Click);
			// 
			// miFullCheck
			// 
			this.miFullCheck.Name = "miFullCheck";
			this.miFullCheck.Size = new System.Drawing.Size(157, 22);
			this.miFullCheck.Text = "Full check";
			this.miFullCheck.Click += new System.EventHandler(this.FullCheck_Click);
			// 
			// miIHK
			// 
			this.miIHK.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miUpdateStatuses,
            this.miUploadSelection,
            this.miHandInSelection,
            this.miUpdateSelection,
            this.miCheckFormat,
            this.miDownloadReports});
			this.miIHK.Name = "miIHK";
			this.miIHK.Size = new System.Drawing.Size(191, 22);
			this.miIHK.Text = "IHK";
			this.miIHK.ToolTipText = "All options for interactions with IHK";
			// 
			// miUpdateStatuses
			// 
			this.miUpdateStatuses.Name = "miUpdateStatuses";
			this.miUpdateStatuses.Size = new System.Drawing.Size(168, 22);
			this.miUpdateStatuses.Text = "Update statuses";
			this.miUpdateStatuses.ToolTipText = "Update statuses of reports";
			this.miUpdateStatuses.Click += new System.EventHandler(this.miUpdateStatuses_Click);
			// 
			// miUploadSelection
			// 
			this.miUploadSelection.Name = "miUploadSelection";
			this.miUploadSelection.Size = new System.Drawing.Size(168, 22);
			this.miUploadSelection.Text = "Upload selection";
			this.miUploadSelection.ToolTipText = "Upload all reports selected in following form";
			this.miUploadSelection.Click += new System.EventHandler(this.UploadSelectionClick);
			// 
			// miHandInSelection
			// 
			this.miHandInSelection.Name = "miHandInSelection";
			this.miHandInSelection.Size = new System.Drawing.Size(168, 22);
			this.miHandInSelection.Text = "Hand in selection";
			this.miHandInSelection.ToolTipText = "Hand in all reports selected in following form";
			this.miHandInSelection.Click += new System.EventHandler(this.HandInSelectionClick);
			// 
			// miUpdateSelection
			// 
			this.miUpdateSelection.Name = "miUpdateSelection";
			this.miUpdateSelection.Size = new System.Drawing.Size(168, 22);
			this.miUpdateSelection.Text = "Update selection";
			this.miUpdateSelection.Click += new System.EventHandler(this.SendSelectionToIHK);
			// 
			// miCheckFormat
			// 
			this.miCheckFormat.Name = "miCheckFormat";
			this.miCheckFormat.Size = new System.Drawing.Size(168, 22);
			this.miCheckFormat.Text = "Check format";
			this.miCheckFormat.Click += new System.EventHandler(this.CheckFormat);
			// 
			// miDownloadReports
			// 
			this.miDownloadReports.Name = "miDownloadReports";
			this.miDownloadReports.Size = new System.Drawing.Size(168, 22);
			this.miDownloadReports.Text = "Download reports";
			this.miDownloadReports.Click += new System.EventHandler(this.DownloadIHKReports);
			// 
			// miCloseReport
			// 
			this.miCloseReport.Name = "miCloseReport";
			this.miCloseReport.Size = new System.Drawing.Size(191, 22);
			this.miCloseReport.Text = "Close Report";
			this.miCloseReport.Click += new System.EventHandler(this.miCloseReport_Click);
			// 
			// miRevealInExplorer
			// 
			this.miRevealInExplorer.Name = "miRevealInExplorer";
			this.miRevealInExplorer.Size = new System.Drawing.Size(191, 22);
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
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.Shown += new System.EventHandler(this.MainForm_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDownDefault);
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
		private OwnControls.OwnTreeView.CustomTreeView tvReports;
		private System.Windows.Forms.ToolStripMenuItem miQuickEditNumber;
	}
}

