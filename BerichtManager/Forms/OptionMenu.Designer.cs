namespace BerichtManager.Forms
{
	partial class OptionMenu
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
			components = new System.ComponentModel.Container();
			toolTip1 = new ToolTip(components);
			cbIHKCheckMatchingStartDates = new CheckBox();
			tbSupervisorMail = new TextBox();
			tbJobField = new TextBox();
			btIHKLogin = new OwnControls.FocusColoredFlatButton();
			nudUploadDelay = new NumericUpDown();
			nudTabStops = new NumericUpDown();
			coTheme = new OwnControls.ColoredComboBox();
			btCreateTheme = new OwnControls.FocusColoredFlatButton();
			tbUpdate = new TextBox();
			btEditTheme = new OwnControls.FocusColoredFlatButton();
			tbFolder = new TextBox();
			cbEndOfWeek = new CheckBox();
			tbNamingPattern = new TextBox();
			tbName = new TextBox();
			nudNumber = new NumericUpDown();
			tbTemplate = new TextBox();
			cbShouldUseUntis = new CheckBox();
			tbSchool = new TextBox();
			btLogin = new OwnControls.FocusColoredFlatButton();
			tbServer = new TextBox();
			cbUseCustomPrefix = new CheckBox();
			tbCustomPrefix = new TextBox();
			cbLegacyEdit = new CheckBox();
			ttErrors = new ToolTip(components);
			cbIHKAutoGetComment = new CheckBox();
			laIHKBaseUrl = new Label();
			tbIHKBaseUrl = new TextBox();
			cbAutoSyncStatusesWithIHK = new CheckBox();
			laSupervisorMail = new Label();
			laUploadDelay = new Label();
			laJobField = new Label();
			cbShowReportToolTip = new CheckBox();
			cbUseWordWrap = new CheckBox();
			laFontSize = new Label();
			nudFontSize = new NumericUpDown();
			laTabSize = new Label();
			laReportFolder = new Label();
			laTheme = new Label();
			laUpdate = new Label();
			btFieldsConfig = new OwnControls.FocusColoredFlatButton();
			laNumber = new Label();
			laName = new Label();
			laNamingPattern = new Label();
			laTemplate = new Label();
			gbWebUntisMisc = new OwnControls.ColoredGroupBox();
			laSchool = new Label();
			laServer = new Label();
			gbReportPrefix = new OwnControls.ColoredGroupBox();
			laCustomPrefix = new Label();
			btSave = new OwnControls.FocusColoredFlatButton();
			btClose = new OwnControls.FocusColoredFlatButton();
			tcOptions = new OwnControls.CustomTabControl.ColoredTabControl();
			tpGeneral = new OwnControls.CustomTabControl.ColoredTabPage();
			cbGeneralOptional = new OwnControls.ColoredGroupBox();
			cbUseIHK = new CheckBox();
			gbGeneralEdit = new OwnControls.ColoredGroupBox();
			gbGeneralMisc = new OwnControls.ColoredGroupBox();
			gbGeneralAppearance = new OwnControls.ColoredGroupBox();
			tpReport = new OwnControls.CustomTabControl.ColoredTabPage();
			gbReportMisc = new OwnControls.ColoredGroupBox();
			cbReportFormFields = new OwnControls.ColoredGroupBox();
			tpWebUntis = new OwnControls.CustomTabControl.ColoredTabPage();
			tpIHK = new OwnControls.CustomTabControl.ColoredTabPage();
			gbIHKMiscellanious = new OwnControls.ColoredGroupBox();
			gbIHKFields = new OwnControls.ColoredGroupBox();
			gbIHKFailSaves = new OwnControls.ColoredGroupBox();
			paButtons = new Panel();
			paContent = new Panel();
			nudMaxReportToolTipWidth = new NumericUpDown();
			laMaxReportToolTipWidth = new Label();
			((System.ComponentModel.ISupportInitialize)nudUploadDelay).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudTabStops).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudNumber).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudFontSize).BeginInit();
			gbWebUntisMisc.SuspendLayout();
			gbReportPrefix.SuspendLayout();
			tcOptions.SuspendLayout();
			tpGeneral.SuspendLayout();
			cbGeneralOptional.SuspendLayout();
			gbGeneralEdit.SuspendLayout();
			gbGeneralMisc.SuspendLayout();
			gbGeneralAppearance.SuspendLayout();
			tpReport.SuspendLayout();
			gbReportMisc.SuspendLayout();
			cbReportFormFields.SuspendLayout();
			tpWebUntis.SuspendLayout();
			tpIHK.SuspendLayout();
			gbIHKMiscellanious.SuspendLayout();
			gbIHKFields.SuspendLayout();
			gbIHKFailSaves.SuspendLayout();
			paButtons.SuspendLayout();
			paContent.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)nudMaxReportToolTipWidth).BeginInit();
			SuspendLayout();
			// 
			// toolTip1
			// 
			toolTip1.AutoPopDelay = 5000;
			toolTip1.InitialDelay = 500;
			toolTip1.OwnerDraw = true;
			toolTip1.ReshowDelay = 100;
			toolTip1.Draw += toolTip1_Draw;
			// 
			// cbIHKCheckMatchingStartDates
			// 
			cbIHKCheckMatchingStartDates.AutoSize = true;
			cbIHKCheckMatchingStartDates.Location = new Point(118, 22);
			cbIHKCheckMatchingStartDates.Margin = new Padding(4, 3, 4, 3);
			cbIHKCheckMatchingStartDates.Name = "cbIHKCheckMatchingStartDates";
			cbIHKCheckMatchingStartDates.Size = new Size(227, 19);
			cbIHKCheckMatchingStartDates.TabIndex = 0;
			cbIHKCheckMatchingStartDates.Text = "Check matching start dates on upload";
			toolTip1.SetToolTip(cbIHKCheckMatchingStartDates, "Wether or not the report manager should check that start date suggested by IHK is the same as the report");
			cbIHKCheckMatchingStartDates.UseVisualStyleBackColor = true;
			cbIHKCheckMatchingStartDates.CheckedChanged += MarkAsDirty;
			// 
			// tbSupervisorMail
			// 
			tbSupervisorMail.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			tbSupervisorMail.Location = new Point(118, 22);
			tbSupervisorMail.Margin = new Padding(4, 3, 4, 3);
			tbSupervisorMail.Name = "tbSupervisorMail";
			tbSupervisorMail.Size = new Size(651, 23);
			tbSupervisorMail.TabIndex = 1;
			toolTip1.SetToolTip(tbSupervisorMail, "E-mail of supervisor");
			tbSupervisorMail.TextChanged += MarkAsDirty;
			// 
			// tbJobField
			// 
			tbJobField.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			tbJobField.Location = new Point(107, 80);
			tbJobField.Margin = new Padding(4, 3, 4, 3);
			tbJobField.Name = "tbJobField";
			tbJobField.Size = new Size(655, 23);
			tbJobField.TabIndex = 5;
			toolTip1.SetToolTip(tbJobField, "Name of job field");
			tbJobField.TextChanged += MarkAsDirty;
			// 
			// btIHKLogin
			// 
			btIHKLogin.Location = new Point(118, 101);
			btIHKLogin.Margin = new Padding(4, 3, 4, 3);
			btIHKLogin.Name = "btIHKLogin";
			btIHKLogin.Size = new Size(140, 24);
			btIHKLogin.TabIndex = 4;
			btIHKLogin.Text = "Login";
			toolTip1.SetToolTip(btIHKLogin, "Log in to IHK");
			btIHKLogin.UseVisualStyleBackColor = true;
			btIHKLogin.Click += btIHKLogin_Click;
			// 
			// nudUploadDelay
			// 
			nudUploadDelay.Increment = new decimal(new int[] { 50, 0, 0, 0 });
			nudUploadDelay.Location = new Point(118, 47);
			nudUploadDelay.Margin = new Padding(4, 3, 4, 3);
			nudUploadDelay.Maximum = new decimal(new int[] { 3600000, 0, 0, 0 });
			nudUploadDelay.Name = "nudUploadDelay";
			nudUploadDelay.Size = new Size(140, 23);
			nudUploadDelay.TabIndex = 1;
			toolTip1.SetToolTip(nudUploadDelay, "Time in ms between report uploads");
			nudUploadDelay.ValueChanged += MarkAsDirty;
			// 
			// nudTabStops
			// 
			nudTabStops.Location = new Point(91, 22);
			nudTabStops.Margin = new Padding(4, 3, 4, 3);
			nudTabStops.Name = "nudTabStops";
			nudTabStops.Size = new Size(178, 23);
			nudTabStops.TabIndex = 1;
			toolTip1.SetToolTip(nudTabStops, "Number of units in each tab");
			nudTabStops.ValueChanged += MarkAsDirty;
			// 
			// coTheme
			// 
			coTheme.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			coTheme.ArrowColor = Color.FromArgb(100, 100, 100);
			coTheme.BorderColor = SystemColors.Window;
			coTheme.DisabledColor = SystemColors.Control;
			coTheme.DisabledTextColor = SystemColors.GrayText;
			coTheme.DrawMode = DrawMode.OwnerDrawFixed;
			coTheme.DropDownButtonColor = SystemColors.Menu;
			coTheme.DropDownStyle = ComboBoxStyle.DropDownList;
			coTheme.FormattingEnabled = true;
			coTheme.HighlightColor = SystemColors.Highlight;
			coTheme.Location = new Point(91, 80);
			coTheme.Margin = new Padding(4, 3, 4, 3);
			coTheme.Name = "coTheme";
			coTheme.OutlineColor = Color.FromArgb(100, 100, 100);
			coTheme.Size = new Size(475, 24);
			coTheme.TabIndex = 5;
			coTheme.TextColor = SystemColors.WindowText;
			toolTip1.SetToolTip(coTheme, "Selected theme");
			coTheme.SelectedIndexChanged += MarkAsDirty;
			// 
			// btCreateTheme
			// 
			btCreateTheme.Anchor = AnchorStyles.Right;
			btCreateTheme.Location = new Point(670, 94);
			btCreateTheme.Margin = new Padding(4, 3, 4, 3);
			btCreateTheme.Name = "btCreateTheme";
			btCreateTheme.Size = new Size(93, 24);
			btCreateTheme.TabIndex = 7;
			btCreateTheme.Text = "Create theme";
			toolTip1.SetToolTip(btCreateTheme, "Create a new theme");
			btCreateTheme.UseVisualStyleBackColor = true;
			btCreateTheme.Click += btCreateTheme_Click;
			// 
			// tbUpdate
			// 
			tbUpdate.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			tbUpdate.Location = new Point(91, 51);
			tbUpdate.Margin = new Padding(4, 3, 4, 3);
			tbUpdate.Name = "tbUpdate";
			tbUpdate.ReadOnly = true;
			tbUpdate.Size = new Size(672, 23);
			tbUpdate.TabIndex = 3;
			toolTip1.SetToolTip(tbUpdate, "Path to check if an update is available");
			tbUpdate.Click += tbUpdate_Click;
			// 
			// btEditTheme
			// 
			btEditTheme.Anchor = AnchorStyles.Right;
			btEditTheme.Location = new Point(574, 94);
			btEditTheme.Margin = new Padding(4, 3, 4, 3);
			btEditTheme.Name = "btEditTheme";
			btEditTheme.Size = new Size(88, 24);
			btEditTheme.TabIndex = 6;
			btEditTheme.Text = "Edit theme";
			toolTip1.SetToolTip(btEditTheme, "Choose a theme to edit");
			btEditTheme.UseVisualStyleBackColor = true;
			btEditTheme.Click += btEditTheme_Click;
			// 
			// tbFolder
			// 
			tbFolder.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			tbFolder.Location = new Point(91, 22);
			tbFolder.Margin = new Padding(4, 3, 4, 3);
			tbFolder.Name = "tbFolder";
			tbFolder.ReadOnly = true;
			tbFolder.Size = new Size(672, 23);
			tbFolder.TabIndex = 1;
			toolTip1.SetToolTip(tbFolder, "Path to folder containing reports");
			tbFolder.Click += tbFolder_Click;
			// 
			// cbEndOfWeek
			// 
			cbEndOfWeek.AutoSize = true;
			cbEndOfWeek.Location = new Point(107, 109);
			cbEndOfWeek.Margin = new Padding(4, 3, 4, 3);
			cbEndOfWeek.Name = "cbEndOfWeek";
			cbEndOfWeek.Size = new Size(130, 19);
			cbEndOfWeek.TabIndex = 6;
			cbEndOfWeek.Text = "Sign dates on friday";
			toolTip1.SetToolTip(cbEndOfWeek, "Should week end date be set to fridays?");
			cbEndOfWeek.UseVisualStyleBackColor = true;
			cbEndOfWeek.CheckedChanged += MarkAsDirty;
			// 
			// tbNamingPattern
			// 
			tbNamingPattern.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			tbNamingPattern.Location = new Point(107, 51);
			tbNamingPattern.Margin = new Padding(4, 3, 4, 3);
			tbNamingPattern.Name = "tbNamingPattern";
			tbNamingPattern.Size = new Size(656, 23);
			tbNamingPattern.TabIndex = 3;
			toolTip1.SetToolTip(tbNamingPattern, "~+CW+~ = Calendar week\r\n~+RN+~ = Report number");
			tbNamingPattern.TextChanged += NamingPatternChanged;
			// 
			// tbName
			// 
			tbName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			tbName.Location = new Point(107, 22);
			tbName.Margin = new Padding(4, 3, 4, 3);
			tbName.Name = "tbName";
			tbName.Size = new Size(655, 23);
			tbName.TabIndex = 1;
			toolTip1.SetToolTip(tbName, "Your name (Last name, First name)");
			tbName.TextChanged += MarkAsDirty;
			// 
			// nudNumber
			// 
			nudNumber.Location = new Point(107, 51);
			nudNumber.Margin = new Padding(4, 3, 4, 3);
			nudNumber.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
			nudNumber.Name = "nudNumber";
			nudNumber.Size = new Size(140, 23);
			nudNumber.TabIndex = 3;
			toolTip1.SetToolTip(nudNumber, "Number of the next report that will be created");
			nudNumber.ValueChanged += MarkAsDirty;
			// 
			// tbTemplate
			// 
			tbTemplate.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			tbTemplate.Location = new Point(107, 22);
			tbTemplate.Margin = new Padding(4, 3, 4, 3);
			tbTemplate.Name = "tbTemplate";
			tbTemplate.ReadOnly = true;
			tbTemplate.Size = new Size(656, 23);
			tbTemplate.TabIndex = 1;
			toolTip1.SetToolTip(tbTemplate, "Path to word template");
			tbTemplate.Click += tbTemplate_Click;
			// 
			// cbShouldUseUntis
			// 
			cbShouldUseUntis.AutoSize = true;
			cbShouldUseUntis.Location = new Point(91, 22);
			cbShouldUseUntis.Margin = new Padding(4, 3, 4, 3);
			cbShouldUseUntis.Name = "cbShouldUseUntis";
			cbShouldUseUntis.Size = new Size(99, 19);
			cbShouldUseUntis.TabIndex = 0;
			cbShouldUseUntis.Text = "Use WebUntis";
			toolTip1.SetToolTip(cbShouldUseUntis, "Should classes be fetched from WebUntis?");
			cbShouldUseUntis.UseVisualStyleBackColor = true;
			cbShouldUseUntis.CheckedChanged += cbShouldUseUntis_CheckedChanged;
			// 
			// tbSchool
			// 
			tbSchool.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			tbSchool.Location = new Point(114, 51);
			tbSchool.Margin = new Padding(4, 3, 4, 3);
			tbSchool.Name = "tbSchool";
			tbSchool.Size = new Size(654, 23);
			tbSchool.TabIndex = 3;
			toolTip1.SetToolTip(tbSchool, "WebUntis name of your school");
			tbSchool.TextChanged += MarkAsDirty;
			// 
			// btLogin
			// 
			btLogin.Location = new Point(114, 80);
			btLogin.Margin = new Padding(4, 3, 4, 3);
			btLogin.Name = "btLogin";
			btLogin.Size = new Size(140, 24);
			btLogin.TabIndex = 4;
			btLogin.Text = "Login";
			toolTip1.SetToolTip(btLogin, "Log in to WebUntis");
			btLogin.UseVisualStyleBackColor = true;
			btLogin.Click += btLogin_Click;
			// 
			// tbServer
			// 
			tbServer.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			tbServer.Location = new Point(114, 22);
			tbServer.Margin = new Padding(4, 3, 4, 3);
			tbServer.Name = "tbServer";
			tbServer.Size = new Size(654, 23);
			tbServer.TabIndex = 1;
			toolTip1.SetToolTip(tbServer, "Name of the WebUntis server of your school");
			tbServer.TextChanged += MarkAsDirty;
			// 
			// cbUseCustomPrefix
			// 
			cbUseCustomPrefix.AutoSize = true;
			cbUseCustomPrefix.Location = new Point(107, 22);
			cbUseCustomPrefix.Margin = new Padding(4, 3, 4, 3);
			cbUseCustomPrefix.Name = "cbUseCustomPrefix";
			cbUseCustomPrefix.Size = new Size(203, 19);
			cbUseCustomPrefix.TabIndex = 0;
			cbUseCustomPrefix.Text = "Use custom prefix in school field?";
			toolTip1.SetToolTip(cbUseCustomPrefix, "Should custom prefix be used?");
			cbUseCustomPrefix.UseVisualStyleBackColor = true;
			cbUseCustomPrefix.CheckedChanged += cbUseCustomPrefix_CheckedChanged;
			// 
			// tbCustomPrefix
			// 
			tbCustomPrefix.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			tbCustomPrefix.Enabled = false;
			tbCustomPrefix.Location = new Point(107, 47);
			tbCustomPrefix.Margin = new Padding(4, 3, 4, 3);
			tbCustomPrefix.Name = "tbCustomPrefix";
			tbCustomPrefix.Size = new Size(655, 23);
			tbCustomPrefix.TabIndex = 2;
			toolTip1.SetToolTip(tbCustomPrefix, "Prefix to be used in school field");
			tbCustomPrefix.TextChanged += MarkAsDirty;
			// 
			// cbLegacyEdit
			// 
			cbLegacyEdit.AutoSize = true;
			cbLegacyEdit.Location = new Point(91, 22);
			cbLegacyEdit.Margin = new Padding(4, 3, 4, 3);
			cbLegacyEdit.Name = "cbLegacyEdit";
			cbLegacyEdit.Size = new Size(105, 19);
			cbLegacyEdit.TabIndex = 0;
			cbLegacyEdit.Text = "Use legacy edit";
			toolTip1.SetToolTip(cbLegacyEdit, "Should seperate forms be used to edit?");
			cbLegacyEdit.UseVisualStyleBackColor = true;
			// 
			// cbIHKAutoGetComment
			// 
			cbIHKAutoGetComment.AutoSize = true;
			cbIHKAutoGetComment.Location = new Point(118, 76);
			cbIHKAutoGetComment.Margin = new Padding(4, 3, 4, 3);
			cbIHKAutoGetComment.Name = "cbIHKAutoGetComment";
			cbIHKAutoGetComment.Size = new Size(217, 19);
			cbIHKAutoGetComment.TabIndex = 3;
			cbIHKAutoGetComment.Text = "Get comment on edit when possible";
			cbIHKAutoGetComment.UseVisualStyleBackColor = true;
			cbIHKAutoGetComment.CheckedChanged += MarkAsDirty;
			// 
			// laIHKBaseUrl
			// 
			laIHKBaseUrl.AutoSize = true;
			laIHKBaseUrl.Location = new Point(66, 25);
			laIHKBaseUrl.Margin = new Padding(4, 0, 4, 0);
			laIHKBaseUrl.Name = "laIHKBaseUrl";
			laIHKBaseUrl.Size = new Size(44, 15);
			laIHKBaseUrl.TabIndex = 0;
			laIHKBaseUrl.Text = "IHK Url";
			// 
			// tbIHKBaseUrl
			// 
			tbIHKBaseUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			tbIHKBaseUrl.Location = new Point(118, 22);
			tbIHKBaseUrl.Margin = new Padding(4, 3, 4, 3);
			tbIHKBaseUrl.Name = "tbIHKBaseUrl";
			tbIHKBaseUrl.Size = new Size(651, 23);
			tbIHKBaseUrl.TabIndex = 1;
			tbIHKBaseUrl.TextChanged += MarkAsDirty;
			// 
			// cbAutoSyncStatusesWithIHK
			// 
			cbAutoSyncStatusesWithIHK.AutoSize = true;
			cbAutoSyncStatusesWithIHK.Location = new Point(118, 51);
			cbAutoSyncStatusesWithIHK.Margin = new Padding(4, 3, 4, 3);
			cbAutoSyncStatusesWithIHK.Name = "cbAutoSyncStatusesWithIHK";
			cbAutoSyncStatusesWithIHK.Size = new Size(188, 19);
			cbAutoSyncStatusesWithIHK.TabIndex = 2;
			cbAutoSyncStatusesWithIHK.Text = "Sync report statuses on startup";
			cbAutoSyncStatusesWithIHK.UseVisualStyleBackColor = true;
			cbAutoSyncStatusesWithIHK.CheckedChanged += MarkAsDirty;
			// 
			// laSupervisorMail
			// 
			laSupervisorMail.AutoSize = true;
			laSupervisorMail.Location = new Point(11, 25);
			laSupervisorMail.Margin = new Padding(4, 0, 4, 0);
			laSupervisorMail.Name = "laSupervisorMail";
			laSupervisorMail.Size = new Size(99, 15);
			laSupervisorMail.TabIndex = 0;
			laSupervisorMail.Text = "Supervisor e-mail";
			// 
			// laUploadDelay
			// 
			laUploadDelay.AutoSize = true;
			laUploadDelay.Location = new Point(7, 49);
			laUploadDelay.Margin = new Padding(4, 0, 4, 0);
			laUploadDelay.Name = "laUploadDelay";
			laUploadDelay.Size = new Size(103, 15);
			laUploadDelay.TabIndex = 0;
			laUploadDelay.Text = "Upload delay (ms)";
			// 
			// laJobField
			// 
			laJobField.AutoSize = true;
			laJobField.Location = new Point(48, 83);
			laJobField.Margin = new Padding(4, 0, 4, 0);
			laJobField.Name = "laJobField";
			laJobField.Size = new Size(51, 15);
			laJobField.TabIndex = 4;
			laJobField.Text = "Job field";
			// 
			// cbShowReportToolTip
			// 
			cbShowReportToolTip.AutoSize = true;
			cbShowReportToolTip.Location = new Point(91, 135);
			cbShowReportToolTip.Margin = new Padding(4, 3, 4, 3);
			cbShowReportToolTip.Name = "cbShowReportToolTip";
			cbShowReportToolTip.Size = new Size(136, 19);
			cbShowReportToolTip.TabIndex = 9;
			cbShowReportToolTip.Text = "Show report tool tips";
			cbShowReportToolTip.UseVisualStyleBackColor = true;
			cbShowReportToolTip.CheckedChanged += MarkAsDirty;
			// 
			// cbUseWordWrap
			// 
			cbUseWordWrap.AutoSize = true;
			cbUseWordWrap.Location = new Point(91, 110);
			cbUseWordWrap.Margin = new Padding(4, 3, 4, 3);
			cbUseWordWrap.Name = "cbUseWordWrap";
			cbUseWordWrap.Size = new Size(175, 19);
			cbUseWordWrap.TabIndex = 8;
			cbUseWordWrap.Text = "Use word wrap on textboxes";
			cbUseWordWrap.UseVisualStyleBackColor = true;
			cbUseWordWrap.CheckedChanged += MarkAsDirty;
			// 
			// laFontSize
			// 
			laFontSize.AutoSize = true;
			laFontSize.Location = new Point(30, 53);
			laFontSize.Margin = new Padding(4, 0, 4, 0);
			laFontSize.Name = "laFontSize";
			laFontSize.Size = new Size(53, 15);
			laFontSize.TabIndex = 2;
			laFontSize.Text = "Font size";
			// 
			// nudFontSize
			// 
			nudFontSize.DecimalPlaces = 2;
			nudFontSize.Location = new Point(91, 51);
			nudFontSize.Margin = new Padding(4, 3, 4, 3);
			nudFontSize.Name = "nudFontSize";
			nudFontSize.Size = new Size(178, 23);
			nudFontSize.TabIndex = 3;
			nudFontSize.ValueChanged += MarkAsDirty;
			// 
			// laTabSize
			// 
			laTabSize.AutoSize = true;
			laTabSize.Location = new Point(36, 24);
			laTabSize.Margin = new Padding(4, 0, 4, 0);
			laTabSize.Name = "laTabSize";
			laTabSize.Size = new Size(47, 15);
			laTabSize.TabIndex = 0;
			laTabSize.Text = "Tab size";
			// 
			// laReportFolder
			// 
			laReportFolder.AutoSize = true;
			laReportFolder.Location = new Point(7, 25);
			laReportFolder.Margin = new Padding(4, 0, 4, 0);
			laReportFolder.Name = "laReportFolder";
			laReportFolder.Size = new Size(76, 15);
			laReportFolder.TabIndex = 0;
			laReportFolder.Text = "Report folder";
			// 
			// laTheme
			// 
			laTheme.AutoSize = true;
			laTheme.Location = new Point(40, 85);
			laTheme.Margin = new Padding(4, 0, 4, 0);
			laTheme.Name = "laTheme";
			laTheme.Size = new Size(43, 15);
			laTheme.TabIndex = 4;
			laTheme.Text = "Theme";
			// 
			// laUpdate
			// 
			laUpdate.AutoSize = true;
			laUpdate.Location = new Point(7, 54);
			laUpdate.Margin = new Padding(4, 0, 4, 0);
			laUpdate.Name = "laUpdate";
			laUpdate.Size = new Size(72, 15);
			laUpdate.TabIndex = 2;
			laUpdate.Text = "Update path";
			// 
			// btFieldsConfig
			// 
			btFieldsConfig.Location = new Point(107, 80);
			btFieldsConfig.Margin = new Padding(4, 3, 4, 3);
			btFieldsConfig.Name = "btFieldsConfig";
			btFieldsConfig.Size = new Size(140, 27);
			btFieldsConfig.TabIndex = 4;
			btFieldsConfig.Text = "Edit fields config";
			btFieldsConfig.UseVisualStyleBackColor = true;
			btFieldsConfig.Click += btFieldsConfig_Click;
			// 
			// laNumber
			// 
			laNumber.AutoSize = true;
			laNumber.Location = new Point(10, 53);
			laNumber.Margin = new Padding(4, 0, 4, 0);
			laNumber.Name = "laNumber";
			laNumber.Size = new Size(89, 15);
			laNumber.TabIndex = 2;
			laNumber.Text = "Report Number";
			// 
			// laName
			// 
			laName.AutoSize = true;
			laName.Location = new Point(35, 25);
			laName.Margin = new Padding(4, 0, 4, 0);
			laName.Name = "laName";
			laName.Size = new Size(64, 15);
			laName.TabIndex = 0;
			laName.Text = "Your name";
			// 
			// laNamingPattern
			// 
			laNamingPattern.AutoSize = true;
			laNamingPattern.Location = new Point(8, 54);
			laNamingPattern.Margin = new Padding(4, 0, 4, 0);
			laNamingPattern.Name = "laNamingPattern";
			laNamingPattern.Size = new Size(91, 15);
			laNamingPattern.TabIndex = 2;
			laNamingPattern.Text = "Naming pattern";
			// 
			// laTemplate
			// 
			laTemplate.AutoSize = true;
			laTemplate.Location = new Point(7, 25);
			laTemplate.Margin = new Padding(4, 0, 4, 0);
			laTemplate.Name = "laTemplate";
			laTemplate.Size = new Size(92, 15);
			laTemplate.TabIndex = 0;
			laTemplate.Text = "Report template";
			// 
			// gbWebUntisMisc
			// 
			gbWebUntisMisc.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			gbWebUntisMisc.BorderColor = Color.FromArgb(220, 220, 220);
			gbWebUntisMisc.Controls.Add(tbSchool);
			gbWebUntisMisc.Controls.Add(btLogin);
			gbWebUntisMisc.Controls.Add(laSchool);
			gbWebUntisMisc.Controls.Add(tbServer);
			gbWebUntisMisc.Controls.Add(laServer);
			gbWebUntisMisc.Dock = DockStyle.Top;
			gbWebUntisMisc.Location = new Point(0, 0);
			gbWebUntisMisc.Margin = new Padding(4, 3, 4, 3);
			gbWebUntisMisc.Name = "gbWebUntisMisc";
			gbWebUntisMisc.Padding = new Padding(4, 3, 4, 3);
			gbWebUntisMisc.Size = new Size(776, 110);
			gbWebUntisMisc.TabIndex = 0;
			gbWebUntisMisc.TabStop = false;
			gbWebUntisMisc.Text = "Miscellanious";
			// 
			// laSchool
			// 
			laSchool.AutoSize = true;
			laSchool.Location = new Point(27, 55);
			laSchool.Margin = new Padding(4, 0, 4, 0);
			laSchool.Name = "laSchool";
			laSchool.Size = new Size(76, 15);
			laSchool.TabIndex = 2;
			laSchool.Text = "School name";
			// 
			// laServer
			// 
			laServer.AutoSize = true;
			laServer.Location = new Point(7, 26);
			laServer.Margin = new Padding(4, 0, 4, 0);
			laServer.Name = "laServer";
			laServer.Size = new Size(92, 15);
			laServer.TabIndex = 0;
			laServer.Text = "Webuntis Server";
			// 
			// gbReportPrefix
			// 
			gbReportPrefix.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			gbReportPrefix.BorderColor = Color.FromArgb(220, 220, 220);
			gbReportPrefix.Controls.Add(laCustomPrefix);
			gbReportPrefix.Controls.Add(cbUseCustomPrefix);
			gbReportPrefix.Controls.Add(tbCustomPrefix);
			gbReportPrefix.Dock = DockStyle.Top;
			gbReportPrefix.Location = new Point(3, 3);
			gbReportPrefix.Margin = new Padding(4, 3, 4, 3);
			gbReportPrefix.Name = "gbReportPrefix";
			gbReportPrefix.Padding = new Padding(4, 3, 4, 3);
			gbReportPrefix.Size = new Size(770, 76);
			gbReportPrefix.TabIndex = 0;
			gbReportPrefix.TabStop = false;
			gbReportPrefix.Text = "Prefix";
			// 
			// laCustomPrefix
			// 
			laCustomPrefix.AutoSize = true;
			laCustomPrefix.Location = new Point(18, 50);
			laCustomPrefix.Name = "laCustomPrefix";
			laCustomPrefix.Size = new Size(82, 15);
			laCustomPrefix.TabIndex = 1;
			laCustomPrefix.Text = "Custom prefix";
			// 
			// btSave
			// 
			btSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			btSave.Location = new Point(596, 3);
			btSave.Margin = new Padding(4, 3, 4, 3);
			btSave.Name = "btSave";
			btSave.Size = new Size(88, 27);
			btSave.TabIndex = 0;
			btSave.Text = "Save";
			btSave.UseVisualStyleBackColor = true;
			btSave.Click += btSave_Click;
			// 
			// btClose
			// 
			btClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			btClose.Location = new Point(692, 3);
			btClose.Margin = new Padding(4, 3, 4, 3);
			btClose.Name = "btClose";
			btClose.Size = new Size(88, 27);
			btClose.TabIndex = 1;
			btClose.Text = "Close";
			btClose.UseVisualStyleBackColor = true;
			btClose.Click += btClose_Click;
			// 
			// tcOptions
			// 
			tcOptions.Controls.Add(tpGeneral);
			tcOptions.Controls.Add(tpReport);
			tcOptions.Controls.Add(tpWebUntis);
			tcOptions.Controls.Add(tpIHK);
			tcOptions.Dock = DockStyle.Fill;
			tcOptions.Location = new Point(0, 0);
			tcOptions.Name = "tcOptions";
			tcOptions.SelectedIndex = 0;
			tcOptions.Size = new Size(784, 421);
			tcOptions.TabIndex = 0;
			// 
			// tpGeneral
			// 
			tpGeneral.Controls.Add(cbGeneralOptional);
			tpGeneral.Controls.Add(gbGeneralEdit);
			tpGeneral.Controls.Add(gbGeneralMisc);
			tpGeneral.Controls.Add(gbGeneralAppearance);
			tpGeneral.Location = new Point(4, 24);
			tpGeneral.Name = "tpGeneral";
			tpGeneral.Padding = new Padding(3);
			tpGeneral.Size = new Size(776, 393);
			tpGeneral.TabIndex = 0;
			tpGeneral.Text = "General";
			tpGeneral.UseVisualStyleBackColor = true;
			// 
			// cbGeneralOptional
			// 
			cbGeneralOptional.BorderColor = Color.FromArgb(220, 220, 220);
			cbGeneralOptional.Controls.Add(cbUseIHK);
			cbGeneralOptional.Controls.Add(cbShouldUseUntis);
			cbGeneralOptional.Dock = DockStyle.Top;
			cbGeneralOptional.Location = new Point(3, 319);
			cbGeneralOptional.Name = "cbGeneralOptional";
			cbGeneralOptional.Size = new Size(770, 72);
			cbGeneralOptional.TabIndex = 3;
			cbGeneralOptional.TabStop = false;
			cbGeneralOptional.Text = "Optional";
			// 
			// cbUseIHK
			// 
			cbUseIHK.AutoSize = true;
			cbUseIHK.Location = new Point(91, 47);
			cbUseIHK.Name = "cbUseIHK";
			cbUseIHK.Size = new Size(67, 19);
			cbUseIHK.TabIndex = 1;
			cbUseIHK.Text = "Use IHK";
			cbUseIHK.UseVisualStyleBackColor = true;
			cbUseIHK.CheckedChanged += cbUseIHK_CheckedChanged;
			// 
			// gbGeneralEdit
			// 
			gbGeneralEdit.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			gbGeneralEdit.BorderColor = Color.FromArgb(220, 220, 220);
			gbGeneralEdit.Controls.Add(cbLegacyEdit);
			gbGeneralEdit.Dock = DockStyle.Top;
			gbGeneralEdit.Location = new Point(3, 272);
			gbGeneralEdit.Margin = new Padding(4, 3, 4, 3);
			gbGeneralEdit.Name = "gbGeneralEdit";
			gbGeneralEdit.Padding = new Padding(4, 3, 4, 3);
			gbGeneralEdit.Size = new Size(770, 47);
			gbGeneralEdit.TabIndex = 2;
			gbGeneralEdit.TabStop = false;
			gbGeneralEdit.Text = "Edit";
			// 
			// gbGeneralMisc
			// 
			gbGeneralMisc.BorderColor = Color.FromArgb(220, 220, 220);
			gbGeneralMisc.Controls.Add(laReportFolder);
			gbGeneralMisc.Controls.Add(laUpdate);
			gbGeneralMisc.Controls.Add(tbFolder);
			gbGeneralMisc.Controls.Add(tbUpdate);
			gbGeneralMisc.Dock = DockStyle.Top;
			gbGeneralMisc.Location = new Point(3, 192);
			gbGeneralMisc.Name = "gbGeneralMisc";
			gbGeneralMisc.Size = new Size(770, 80);
			gbGeneralMisc.TabIndex = 1;
			gbGeneralMisc.TabStop = false;
			gbGeneralMisc.Text = "Miscellanious";
			// 
			// gbGeneralAppearance
			// 
			gbGeneralAppearance.BorderColor = Color.FromArgb(220, 220, 220);
			gbGeneralAppearance.Controls.Add(laMaxReportToolTipWidth);
			gbGeneralAppearance.Controls.Add(nudMaxReportToolTipWidth);
			gbGeneralAppearance.Controls.Add(cbShowReportToolTip);
			gbGeneralAppearance.Controls.Add(nudTabStops);
			gbGeneralAppearance.Controls.Add(cbUseWordWrap);
			gbGeneralAppearance.Controls.Add(laTabSize);
			gbGeneralAppearance.Controls.Add(laFontSize);
			gbGeneralAppearance.Controls.Add(nudFontSize);
			gbGeneralAppearance.Controls.Add(btCreateTheme);
			gbGeneralAppearance.Controls.Add(coTheme);
			gbGeneralAppearance.Controls.Add(laTheme);
			gbGeneralAppearance.Controls.Add(btEditTheme);
			gbGeneralAppearance.Dock = DockStyle.Top;
			gbGeneralAppearance.Location = new Point(3, 3);
			gbGeneralAppearance.Name = "gbGeneralAppearance";
			gbGeneralAppearance.Size = new Size(770, 189);
			gbGeneralAppearance.TabIndex = 0;
			gbGeneralAppearance.TabStop = false;
			gbGeneralAppearance.Text = "Appearance";
			// 
			// tpReport
			// 
			tpReport.Controls.Add(gbReportMisc);
			tpReport.Controls.Add(cbReportFormFields);
			tpReport.Controls.Add(gbReportPrefix);
			tpReport.Location = new Point(4, 24);
			tpReport.Name = "tpReport";
			tpReport.Padding = new Padding(3);
			tpReport.Size = new Size(776, 365);
			tpReport.TabIndex = 1;
			tpReport.Text = "Report";
			tpReport.UseVisualStyleBackColor = true;
			// 
			// gbReportMisc
			// 
			gbReportMisc.BorderColor = Color.FromArgb(220, 220, 220);
			gbReportMisc.Controls.Add(btFieldsConfig);
			gbReportMisc.Controls.Add(laTemplate);
			gbReportMisc.Controls.Add(laNamingPattern);
			gbReportMisc.Controls.Add(tbNamingPattern);
			gbReportMisc.Controls.Add(tbTemplate);
			gbReportMisc.Dock = DockStyle.Top;
			gbReportMisc.Location = new Point(3, 213);
			gbReportMisc.Name = "gbReportMisc";
			gbReportMisc.Size = new Size(770, 113);
			gbReportMisc.TabIndex = 2;
			gbReportMisc.TabStop = false;
			gbReportMisc.Text = "Miscellanious";
			// 
			// cbReportFormFields
			// 
			cbReportFormFields.BorderColor = Color.FromArgb(220, 220, 220);
			cbReportFormFields.Controls.Add(tbName);
			cbReportFormFields.Controls.Add(cbEndOfWeek);
			cbReportFormFields.Controls.Add(laName);
			cbReportFormFields.Controls.Add(laNumber);
			cbReportFormFields.Controls.Add(nudNumber);
			cbReportFormFields.Controls.Add(tbJobField);
			cbReportFormFields.Controls.Add(laJobField);
			cbReportFormFields.Dock = DockStyle.Top;
			cbReportFormFields.Location = new Point(3, 79);
			cbReportFormFields.Name = "cbReportFormFields";
			cbReportFormFields.Size = new Size(770, 134);
			cbReportFormFields.TabIndex = 1;
			cbReportFormFields.TabStop = false;
			cbReportFormFields.Text = "Form fields";
			// 
			// tpWebUntis
			// 
			tpWebUntis.Controls.Add(gbWebUntisMisc);
			tpWebUntis.Location = new Point(4, 24);
			tpWebUntis.Name = "tpWebUntis";
			tpWebUntis.Size = new Size(776, 365);
			tpWebUntis.TabIndex = 2;
			tpWebUntis.Text = "WebUntis";
			tpWebUntis.UseVisualStyleBackColor = true;
			// 
			// tpIHK
			// 
			tpIHK.Controls.Add(gbIHKMiscellanious);
			tpIHK.Controls.Add(gbIHKFields);
			tpIHK.Controls.Add(gbIHKFailSaves);
			tpIHK.Location = new Point(4, 24);
			tpIHK.Name = "tpIHK";
			tpIHK.Size = new Size(776, 365);
			tpIHK.TabIndex = 3;
			tpIHK.Text = "IHK";
			tpIHK.UseVisualStyleBackColor = true;
			// 
			// gbIHKMiscellanious
			// 
			gbIHKMiscellanious.BorderColor = Color.FromArgb(220, 220, 220);
			gbIHKMiscellanious.Controls.Add(btIHKLogin);
			gbIHKMiscellanious.Controls.Add(cbIHKAutoGetComment);
			gbIHKMiscellanious.Controls.Add(tbIHKBaseUrl);
			gbIHKMiscellanious.Controls.Add(cbAutoSyncStatusesWithIHK);
			gbIHKMiscellanious.Controls.Add(laIHKBaseUrl);
			gbIHKMiscellanious.Dock = DockStyle.Top;
			gbIHKMiscellanious.Location = new Point(0, 127);
			gbIHKMiscellanious.Name = "gbIHKMiscellanious";
			gbIHKMiscellanious.Size = new Size(776, 131);
			gbIHKMiscellanious.TabIndex = 2;
			gbIHKMiscellanious.TabStop = false;
			gbIHKMiscellanious.Text = "Miscellanious";
			// 
			// gbIHKFields
			// 
			gbIHKFields.BorderColor = Color.FromArgb(220, 220, 220);
			gbIHKFields.Controls.Add(tbSupervisorMail);
			gbIHKFields.Controls.Add(laSupervisorMail);
			gbIHKFields.Dock = DockStyle.Top;
			gbIHKFields.Location = new Point(0, 76);
			gbIHKFields.Name = "gbIHKFields";
			gbIHKFields.Size = new Size(776, 51);
			gbIHKFields.TabIndex = 1;
			gbIHKFields.TabStop = false;
			gbIHKFields.Text = "Fields";
			// 
			// gbIHKFailSaves
			// 
			gbIHKFailSaves.BorderColor = Color.FromArgb(220, 220, 220);
			gbIHKFailSaves.Controls.Add(cbIHKCheckMatchingStartDates);
			gbIHKFailSaves.Controls.Add(nudUploadDelay);
			gbIHKFailSaves.Controls.Add(laUploadDelay);
			gbIHKFailSaves.Dock = DockStyle.Top;
			gbIHKFailSaves.Location = new Point(0, 0);
			gbIHKFailSaves.Name = "gbIHKFailSaves";
			gbIHKFailSaves.Size = new Size(776, 76);
			gbIHKFailSaves.TabIndex = 0;
			gbIHKFailSaves.TabStop = false;
			gbIHKFailSaves.Text = "Fail saves";
			// 
			// paButtons
			// 
			paButtons.Controls.Add(btClose);
			paButtons.Controls.Add(btSave);
			paButtons.Dock = DockStyle.Bottom;
			paButtons.Location = new Point(0, 421);
			paButtons.Name = "paButtons";
			paButtons.Size = new Size(784, 33);
			paButtons.TabIndex = 0;
			// 
			// paContent
			// 
			paContent.Controls.Add(tcOptions);
			paContent.Dock = DockStyle.Fill;
			paContent.Location = new Point(0, 0);
			paContent.Name = "paContent";
			paContent.Size = new Size(784, 421);
			paContent.TabIndex = 1;
			// 
			// nudMaxReportToolTipWidth
			// 
			nudMaxReportToolTipWidth.Location = new Point(91, 160);
			nudMaxReportToolTipWidth.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
			nudMaxReportToolTipWidth.Name = "nudMaxReportToolTipWidth";
			nudMaxReportToolTipWidth.Size = new Size(178, 23);
			nudMaxReportToolTipWidth.TabIndex = 11;
			nudMaxReportToolTipWidth.ValueChanged += MarkAsDirty;
			// 
			// laMaxReportToolTipWidth
			// 
			laMaxReportToolTipWidth.AutoSize = true;
			laMaxReportToolTipWidth.Location = new Point(18, 165);
			laMaxReportToolTipWidth.Name = "laMaxReportToolTipWidth";
			laMaxReportToolTipWidth.Size = new Size(67, 15);
			laMaxReportToolTipWidth.TabIndex = 12;
			laMaxReportToolTipWidth.Text = "Max length";
			// 
			// OptionMenu
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(784, 454);
			Controls.Add(paContent);
			Controls.Add(paButtons);
			Margin = new Padding(4, 3, 4, 3);
			MinimumSize = new Size(550, 465);
			Name = "OptionMenu";
			Text = "OptionMenu";
			FormClosing += OptionMenu_FormClosing;
			((System.ComponentModel.ISupportInitialize)nudUploadDelay).EndInit();
			((System.ComponentModel.ISupportInitialize)nudTabStops).EndInit();
			((System.ComponentModel.ISupportInitialize)nudNumber).EndInit();
			((System.ComponentModel.ISupportInitialize)nudFontSize).EndInit();
			gbWebUntisMisc.ResumeLayout(false);
			gbWebUntisMisc.PerformLayout();
			gbReportPrefix.ResumeLayout(false);
			gbReportPrefix.PerformLayout();
			tcOptions.ResumeLayout(false);
			tpGeneral.ResumeLayout(false);
			cbGeneralOptional.ResumeLayout(false);
			cbGeneralOptional.PerformLayout();
			gbGeneralEdit.ResumeLayout(false);
			gbGeneralEdit.PerformLayout();
			gbGeneralMisc.ResumeLayout(false);
			gbGeneralMisc.PerformLayout();
			gbGeneralAppearance.ResumeLayout(false);
			gbGeneralAppearance.PerformLayout();
			tpReport.ResumeLayout(false);
			gbReportMisc.ResumeLayout(false);
			gbReportMisc.PerformLayout();
			cbReportFormFields.ResumeLayout(false);
			cbReportFormFields.PerformLayout();
			tpWebUntis.ResumeLayout(false);
			tpIHK.ResumeLayout(false);
			gbIHKMiscellanious.ResumeLayout(false);
			gbIHKMiscellanious.PerformLayout();
			gbIHKFields.ResumeLayout(false);
			gbIHKFields.PerformLayout();
			gbIHKFailSaves.ResumeLayout(false);
			gbIHKFailSaves.PerformLayout();
			paButtons.ResumeLayout(false);
			paContent.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)nudMaxReportToolTipWidth).EndInit();
			ResumeLayout(false);
		}

		#endregion
		private System.Windows.Forms.ToolTip toolTip1;
		private OwnControls.ColoredGroupBox gbReportPrefix;
		private System.Windows.Forms.CheckBox cbUseCustomPrefix;
		private System.Windows.Forms.TextBox tbCustomPrefix;
		private OwnControls.ColoredGroupBox gbWebUntisMisc;
		private System.Windows.Forms.CheckBox cbShouldUseUntis;
		private System.Windows.Forms.TextBox tbSchool;
		private System.Windows.Forms.Label laSchool;
		private System.Windows.Forms.TextBox tbServer;
		private System.Windows.Forms.Label laServer;
		private System.Windows.Forms.CheckBox cbEndOfWeek;
		private System.Windows.Forms.TextBox tbNamingPattern;
		private System.Windows.Forms.TextBox tbName;
		private System.Windows.Forms.Label laNumber;
		private System.Windows.Forms.Label laName;
		private System.Windows.Forms.NumericUpDown nudNumber;
		private System.Windows.Forms.Label laNamingPattern;
		private System.Windows.Forms.TextBox tbTemplate;
		private System.Windows.Forms.Label laTheme;
		private System.Windows.Forms.Label laTemplate;
		private OwnControls.ColoredComboBox coTheme;
		private System.Windows.Forms.Label laTabSize;
		private System.Windows.Forms.NumericUpDown nudTabStops;
		private System.Windows.Forms.Label laReportFolder;
		private System.Windows.Forms.TextBox tbUpdate;
		private System.Windows.Forms.TextBox tbFolder;
		private System.Windows.Forms.Label laUpdate;
		private System.Windows.Forms.Label laFontSize;
		private System.Windows.Forms.NumericUpDown nudFontSize;
		private OwnControls.FocusColoredFlatButton btClose;
		private OwnControls.FocusColoredFlatButton btSave;
		private OwnControls.FocusColoredFlatButton btLogin;
		private OwnControls.FocusColoredFlatButton btCreateTheme;
		private OwnControls.FocusColoredFlatButton btEditTheme;
		private System.Windows.Forms.Label laUploadDelay;
		private System.Windows.Forms.NumericUpDown nudUploadDelay;
		private System.Windows.Forms.TextBox tbJobField;
		private OwnControls.FocusColoredFlatButton btIHKLogin;
		private System.Windows.Forms.Label laJobField;
		private System.Windows.Forms.Label laSupervisorMail;
		private System.Windows.Forms.TextBox tbSupervisorMail;
		private System.Windows.Forms.CheckBox cbAutoSyncStatusesWithIHK;
		private System.Windows.Forms.Label laIHKBaseUrl;
		private System.Windows.Forms.TextBox tbIHKBaseUrl;
		private System.Windows.Forms.CheckBox cbIHKCheckMatchingStartDates;
		private System.Windows.Forms.CheckBox cbIHKAutoGetComment;
		private OwnControls.FocusColoredFlatButton btFieldsConfig;
		private System.Windows.Forms.ToolTip ttErrors;
		private System.Windows.Forms.CheckBox cbUseWordWrap;
		private System.Windows.Forms.CheckBox cbShowReportToolTip;
		private OwnControls.CustomTabControl.ColoredTabControl tcOptions;
		private OwnControls.CustomTabControl.ColoredTabPage tpGeneral;
		private OwnControls.CustomTabControl.ColoredTabPage tpReport;
		private OwnControls.CustomTabControl.ColoredTabPage tpWebUntis;
		private OwnControls.CustomTabControl.ColoredTabPage tpIHK;
		private Panel paButtons;
		private OwnControls.ColoredGroupBox gbGeneralAppearance;
		private OwnControls.ColoredGroupBox gbGeneralMisc;
		private OwnControls.ColoredGroupBox gbGeneralEdit;
		private CheckBox cbLegacyEdit;
		private Label laCustomPrefix;
		private OwnControls.ColoredGroupBox cbReportFormFields;
		private OwnControls.ColoredGroupBox gbReportMisc;
		private OwnControls.ColoredGroupBox cbGeneralOptional;
		private OwnControls.ColoredGroupBox gbIHKFields;
		private OwnControls.ColoredGroupBox gbIHKFailSaves;
		private OwnControls.ColoredGroupBox gbIHKMiscellanious;
		private Panel paContent;
		private CheckBox cbUseIHK;
		private Label laMaxReportToolTipWidth;
		private NumericUpDown nudMaxReportToolTipWidth;
	}
}