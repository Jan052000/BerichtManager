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
			this.components = new System.ComponentModel.Container();
			this.btClose = new System.Windows.Forms.Button();
			this.btSave = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.gbManagerOptions = new BerichtManager.OwnControls.ColoredGroupBox();
			this.laFontSize = new System.Windows.Forms.Label();
			this.nudFontSize = new System.Windows.Forms.NumericUpDown();
			this.laTabStop = new System.Windows.Forms.Label();
			this.nudTabStops = new System.Windows.Forms.NumericUpDown();
			this.laFolder = new System.Windows.Forms.Label();
			this.cbLegacyEdit = new System.Windows.Forms.CheckBox();
			this.coTheme = new BerichtManager.OwnControls.ColoredComboBox();
			this.btCreateTheme = new System.Windows.Forms.Button();
			this.tbUpdate = new System.Windows.Forms.TextBox();
			this.laTheme = new System.Windows.Forms.Label();
			this.tbFolder = new System.Windows.Forms.TextBox();
			this.btEditTheme = new System.Windows.Forms.Button();
			this.laUpdate = new System.Windows.Forms.Label();
			this.gbConfig = new BerichtManager.OwnControls.ColoredGroupBox();
			this.cbEndOfWeek = new System.Windows.Forms.CheckBox();
			this.tbNamingPattern = new System.Windows.Forms.TextBox();
			this.tbName = new System.Windows.Forms.TextBox();
			this.laNumber = new System.Windows.Forms.Label();
			this.laName = new System.Windows.Forms.Label();
			this.nudNumber = new System.Windows.Forms.NumericUpDown();
			this.laNamingPattern = new System.Windows.Forms.Label();
			this.tbTemplate = new System.Windows.Forms.TextBox();
			this.laTemplate = new System.Windows.Forms.Label();
			this.gbWebUntis = new BerichtManager.OwnControls.ColoredGroupBox();
			this.cbShouldUseUntis = new System.Windows.Forms.CheckBox();
			this.tbSchool = new System.Windows.Forms.TextBox();
			this.btLogin = new System.Windows.Forms.Button();
			this.laSchool = new System.Windows.Forms.Label();
			this.tbServer = new System.Windows.Forms.TextBox();
			this.laServer = new System.Windows.Forms.Label();
			this.gbPrefix = new BerichtManager.OwnControls.ColoredGroupBox();
			this.cbUseCustomPrefix = new System.Windows.Forms.CheckBox();
			this.tbCustomPrefix = new System.Windows.Forms.TextBox();
			this.gbManagerOptions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudFontSize)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudTabStops)).BeginInit();
			this.gbConfig.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudNumber)).BeginInit();
			this.gbWebUntis.SuspendLayout();
			this.gbPrefix.SuspendLayout();
			this.SuspendLayout();
			// 
			// btClose
			// 
			this.btClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btClose.Location = new System.Drawing.Point(713, 411);
			this.btClose.Name = "btClose";
			this.btClose.Size = new System.Drawing.Size(75, 23);
			this.btClose.TabIndex = 0;
			this.btClose.Text = "Close";
			this.btClose.UseVisualStyleBackColor = true;
			this.btClose.Click += new System.EventHandler(this.btClose_Click);
			// 
			// btSave
			// 
			this.btSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btSave.Location = new System.Drawing.Point(632, 411);
			this.btSave.Name = "btSave";
			this.btSave.Size = new System.Drawing.Size(75, 23);
			this.btSave.TabIndex = 1;
			this.btSave.Text = "Save";
			this.btSave.UseVisualStyleBackColor = true;
			this.btSave.Click += new System.EventHandler(this.btSave_Click);
			// 
			// toolTip1
			// 
			this.toolTip1.AutoPopDelay = 5000;
			this.toolTip1.InitialDelay = 500;
			this.toolTip1.OwnerDraw = true;
			this.toolTip1.ReshowDelay = 100;
			this.toolTip1.Draw += new System.Windows.Forms.DrawToolTipEventHandler(this.toolTip1_Draw);
			// 
			// gbManagerOptions
			// 
			this.gbManagerOptions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.gbManagerOptions.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
			this.gbManagerOptions.Controls.Add(this.laFontSize);
			this.gbManagerOptions.Controls.Add(this.nudFontSize);
			this.gbManagerOptions.Controls.Add(this.laTabStop);
			this.gbManagerOptions.Controls.Add(this.nudTabStops);
			this.gbManagerOptions.Controls.Add(this.laFolder);
			this.gbManagerOptions.Controls.Add(this.cbLegacyEdit);
			this.gbManagerOptions.Controls.Add(this.coTheme);
			this.gbManagerOptions.Controls.Add(this.btCreateTheme);
			this.gbManagerOptions.Controls.Add(this.tbUpdate);
			this.gbManagerOptions.Controls.Add(this.laTheme);
			this.gbManagerOptions.Controls.Add(this.laUpdate);
			this.gbManagerOptions.Controls.Add(this.btEditTheme);
			this.gbManagerOptions.Controls.Add(this.tbFolder);
			this.gbManagerOptions.Dock = System.Windows.Forms.DockStyle.Top;
			this.gbManagerOptions.Location = new System.Drawing.Point(0, 288);
			this.gbManagerOptions.Name = "gbManagerOptions";
			this.gbManagerOptions.Size = new System.Drawing.Size(800, 117);
			this.gbManagerOptions.TabIndex = 45;
			this.gbManagerOptions.TabStop = false;
			this.gbManagerOptions.Text = "Manager";
			// 
			// laFontSize
			// 
			this.laFontSize.AutoSize = true;
			this.laFontSize.Location = new System.Drawing.Point(391, 15);
			this.laFontSize.Name = "laFontSize";
			this.laFontSize.Size = new System.Drawing.Size(49, 13);
			this.laFontSize.TabIndex = 33;
			this.laFontSize.Text = "Font size";
			// 
			// nudFontSize
			// 
			this.nudFontSize.DecimalPlaces = 2;
			this.nudFontSize.Location = new System.Drawing.Point(446, 12);
			this.nudFontSize.Name = "nudFontSize";
			this.nudFontSize.Size = new System.Drawing.Size(120, 20);
			this.nudFontSize.TabIndex = 32;
			this.nudFontSize.ValueChanged += new System.EventHandler(this.MarkAsDirty);
			// 
			// laTabStop
			// 
			this.laTabStop.AutoSize = true;
			this.laTabStop.Location = new System.Drawing.Point(38, 15);
			this.laTabStop.Name = "laTabStop";
			this.laTabStop.Size = new System.Drawing.Size(54, 13);
			this.laTabStop.TabIndex = 30;
			this.laTabStop.Text = "Tab stops";
			// 
			// nudTabStops
			// 
			this.nudTabStops.Location = new System.Drawing.Point(98, 12);
			this.nudTabStops.Name = "nudTabStops";
			this.nudTabStops.Size = new System.Drawing.Size(120, 20);
			this.nudTabStops.TabIndex = 31;
			this.toolTip1.SetToolTip(this.nudTabStops, "Number of units in each tab");
			this.nudTabStops.ValueChanged += new System.EventHandler(this.MarkAsDirty);
			// 
			// laFolder
			// 
			this.laFolder.AutoSize = true;
			this.laFolder.Location = new System.Drawing.Point(56, 68);
			this.laFolder.Name = "laFolder";
			this.laFolder.Size = new System.Drawing.Size(36, 13);
			this.laFolder.TabIndex = 26;
			this.laFolder.Text = "Folder";
			// 
			// cbLegacyEdit
			// 
			this.cbLegacyEdit.AutoSize = true;
			this.cbLegacyEdit.Location = new System.Drawing.Point(257, 14);
			this.cbLegacyEdit.Name = "cbLegacyEdit";
			this.cbLegacyEdit.Size = new System.Drawing.Size(99, 17);
			this.cbLegacyEdit.TabIndex = 10;
			this.cbLegacyEdit.Text = "Use legacy edit";
			this.toolTip1.SetToolTip(this.cbLegacyEdit, "Should seperate forms be used to edit?");
			this.cbLegacyEdit.UseVisualStyleBackColor = true;
			this.cbLegacyEdit.CheckedChanged += new System.EventHandler(this.MarkAsDirty);
			// 
			// coTheme
			// 
			this.coTheme.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.coTheme.ArrowColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
			this.coTheme.BorderColor = System.Drawing.SystemColors.Window;
			this.coTheme.DisabledColor = System.Drawing.SystemColors.Control;
			this.coTheme.DisabledTextColor = System.Drawing.SystemColors.GrayText;
			this.coTheme.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.coTheme.DropDownButtonColor = System.Drawing.SystemColors.Menu;
			this.coTheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.coTheme.FormattingEnabled = true;
			this.coTheme.HighlightColor = System.Drawing.SystemColors.Highlight;
			this.coTheme.Location = new System.Drawing.Point(98, 38);
			this.coTheme.Name = "coTheme";
			this.coTheme.OutlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
			this.coTheme.Size = new System.Drawing.Size(529, 21);
			this.coTheme.TabIndex = 23;
			this.coTheme.TextColor = System.Drawing.SystemColors.WindowText;
			this.toolTip1.SetToolTip(this.coTheme, "Selected theme");
			this.coTheme.SelectedIndexChanged += new System.EventHandler(this.MarkAsDirty);
			// 
			// btCreateTheme
			// 
			this.btCreateTheme.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btCreateTheme.Location = new System.Drawing.Point(714, 38);
			this.btCreateTheme.Name = "btCreateTheme";
			this.btCreateTheme.Size = new System.Drawing.Size(80, 21);
			this.btCreateTheme.TabIndex = 24;
			this.btCreateTheme.Text = "Create theme";
			this.toolTip1.SetToolTip(this.btCreateTheme, "Create a new theme");
			this.btCreateTheme.UseVisualStyleBackColor = true;
			this.btCreateTheme.Click += new System.EventHandler(this.btCreateTheme_Click);
			// 
			// tbUpdate
			// 
			this.tbUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbUpdate.Location = new System.Drawing.Point(98, 91);
			this.tbUpdate.Name = "tbUpdate";
			this.tbUpdate.ReadOnly = true;
			this.tbUpdate.Size = new System.Drawing.Size(696, 20);
			this.tbUpdate.TabIndex = 29;
			this.toolTip1.SetToolTip(this.tbUpdate, "Path to check if an update is available");
			this.tbUpdate.Click += new System.EventHandler(this.tbUpdate_Click);
			// 
			// laTheme
			// 
			this.laTheme.AutoSize = true;
			this.laTheme.Location = new System.Drawing.Point(52, 41);
			this.laTheme.Name = "laTheme";
			this.laTheme.Size = new System.Drawing.Size(40, 13);
			this.laTheme.TabIndex = 22;
			this.laTheme.Text = "Theme";
			// 
			// tbFolder
			// 
			this.tbFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbFolder.Location = new System.Drawing.Point(98, 65);
			this.tbFolder.Name = "tbFolder";
			this.tbFolder.ReadOnly = true;
			this.tbFolder.Size = new System.Drawing.Size(696, 20);
			this.tbFolder.TabIndex = 27;
			this.toolTip1.SetToolTip(this.tbFolder, "Path to folder containing reports");
			this.tbFolder.Click += new System.EventHandler(this.tbFolder_Click);
			// 
			// btEditTheme
			// 
			this.btEditTheme.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btEditTheme.Location = new System.Drawing.Point(633, 38);
			this.btEditTheme.Name = "btEditTheme";
			this.btEditTheme.Size = new System.Drawing.Size(75, 21);
			this.btEditTheme.TabIndex = 25;
			this.btEditTheme.Text = "Edit theme";
			this.toolTip1.SetToolTip(this.btEditTheme, "Choose a theme to edit");
			this.btEditTheme.UseVisualStyleBackColor = true;
			this.btEditTheme.Click += new System.EventHandler(this.btEditTheme_Click);
			// 
			// laUpdate
			// 
			this.laUpdate.AutoSize = true;
			this.laUpdate.Location = new System.Drawing.Point(26, 94);
			this.laUpdate.Name = "laUpdate";
			this.laUpdate.Size = new System.Drawing.Size(66, 13);
			this.laUpdate.TabIndex = 28;
			this.laUpdate.Text = "Update path";
			// 
			// gbConfig
			// 
			this.gbConfig.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.gbConfig.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
			this.gbConfig.Controls.Add(this.cbEndOfWeek);
			this.gbConfig.Controls.Add(this.tbNamingPattern);
			this.gbConfig.Controls.Add(this.tbName);
			this.gbConfig.Controls.Add(this.laNumber);
			this.gbConfig.Controls.Add(this.laName);
			this.gbConfig.Controls.Add(this.nudNumber);
			this.gbConfig.Controls.Add(this.laNamingPattern);
			this.gbConfig.Controls.Add(this.tbTemplate);
			this.gbConfig.Controls.Add(this.laTemplate);
			this.gbConfig.Dock = System.Windows.Forms.DockStyle.Top;
			this.gbConfig.Location = new System.Drawing.Point(0, 166);
			this.gbConfig.Name = "gbConfig";
			this.gbConfig.Size = new System.Drawing.Size(800, 122);
			this.gbConfig.TabIndex = 44;
			this.gbConfig.TabStop = false;
			this.gbConfig.Text = "Report";
			// 
			// cbEndOfWeek
			// 
			this.cbEndOfWeek.AutoSize = true;
			this.cbEndOfWeek.Location = new System.Drawing.Point(257, 73);
			this.cbEndOfWeek.Name = "cbEndOfWeek";
			this.cbEndOfWeek.Size = new System.Drawing.Size(117, 17);
			this.cbEndOfWeek.TabIndex = 9;
			this.cbEndOfWeek.Text = "End week on friday";
			this.toolTip1.SetToolTip(this.cbEndOfWeek, "Should week end date be set to fridays?");
			this.cbEndOfWeek.UseVisualStyleBackColor = true;
			this.cbEndOfWeek.CheckedChanged += new System.EventHandler(this.MarkAsDirty);
			// 
			// tbNamingPattern
			// 
			this.tbNamingPattern.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbNamingPattern.Location = new System.Drawing.Point(98, 96);
			this.tbNamingPattern.Name = "tbNamingPattern";
			this.tbNamingPattern.Size = new System.Drawing.Size(696, 20);
			this.tbNamingPattern.TabIndex = 32;
			this.toolTip1.SetToolTip(this.tbNamingPattern, "~+CW+~ = Calendar week\r\n~+RN+~ = Report number");
			this.tbNamingPattern.TextChanged += new System.EventHandler(this.MarkAsDirty);
			// 
			// tbName
			// 
			this.tbName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbName.Location = new System.Drawing.Point(98, 19);
			this.tbName.Name = "tbName";
			this.tbName.Size = new System.Drawing.Size(696, 20);
			this.tbName.TabIndex = 19;
			this.toolTip1.SetToolTip(this.tbName, "Your name (Last name, First name)");
			this.tbName.TextChanged += new System.EventHandler(this.MarkAsDirty);
			// 
			// laNumber
			// 
			this.laNumber.AutoSize = true;
			this.laNumber.Location = new System.Drawing.Point(13, 73);
			this.laNumber.Name = "laNumber";
			this.laNumber.Size = new System.Drawing.Size(79, 13);
			this.laNumber.TabIndex = 20;
			this.laNumber.Text = "Report Number";
			// 
			// laName
			// 
			this.laName.AutoSize = true;
			this.laName.Location = new System.Drawing.Point(57, 19);
			this.laName.Name = "laName";
			this.laName.Size = new System.Drawing.Size(35, 13);
			this.laName.TabIndex = 18;
			this.laName.Text = "Name";
			// 
			// nudNumber
			// 
			this.nudNumber.Location = new System.Drawing.Point(98, 70);
			this.nudNumber.Name = "nudNumber";
			this.nudNumber.Size = new System.Drawing.Size(120, 20);
			this.nudNumber.TabIndex = 21;
			this.toolTip1.SetToolTip(this.nudNumber, "Number of the next report that will be created");
			this.nudNumber.ValueChanged += new System.EventHandler(this.MarkAsDirty);
			// 
			// laNamingPattern
			// 
			this.laNamingPattern.AutoSize = true;
			this.laNamingPattern.Location = new System.Drawing.Point(13, 99);
			this.laNamingPattern.Name = "laNamingPattern";
			this.laNamingPattern.Size = new System.Drawing.Size(79, 13);
			this.laNamingPattern.TabIndex = 33;
			this.laNamingPattern.Text = "Naming pattern";
			// 
			// tbTemplate
			// 
			this.tbTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbTemplate.Location = new System.Drawing.Point(98, 44);
			this.tbTemplate.Name = "tbTemplate";
			this.tbTemplate.ReadOnly = true;
			this.tbTemplate.Size = new System.Drawing.Size(696, 20);
			this.tbTemplate.TabIndex = 17;
			this.toolTip1.SetToolTip(this.tbTemplate, "Path to word template");
			this.tbTemplate.Click += new System.EventHandler(this.tbTemplate_Click);
			// 
			// laTemplate
			// 
			this.laTemplate.AutoSize = true;
			this.laTemplate.Location = new System.Drawing.Point(41, 47);
			this.laTemplate.Name = "laTemplate";
			this.laTemplate.Size = new System.Drawing.Size(51, 13);
			this.laTemplate.TabIndex = 16;
			this.laTemplate.Text = "Template";
			// 
			// gbWebUntis
			// 
			this.gbWebUntis.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.gbWebUntis.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
			this.gbWebUntis.Controls.Add(this.cbShouldUseUntis);
			this.gbWebUntis.Controls.Add(this.tbSchool);
			this.gbWebUntis.Controls.Add(this.btLogin);
			this.gbWebUntis.Controls.Add(this.laSchool);
			this.gbWebUntis.Controls.Add(this.tbServer);
			this.gbWebUntis.Controls.Add(this.laServer);
			this.gbWebUntis.Dock = System.Windows.Forms.DockStyle.Top;
			this.gbWebUntis.Location = new System.Drawing.Point(0, 72);
			this.gbWebUntis.Name = "gbWebUntis";
			this.gbWebUntis.Size = new System.Drawing.Size(800, 94);
			this.gbWebUntis.TabIndex = 43;
			this.gbWebUntis.TabStop = false;
			this.gbWebUntis.Text = "WebUntis";
			// 
			// cbShouldUseUntis
			// 
			this.cbShouldUseUntis.AutoSize = true;
			this.cbShouldUseUntis.Location = new System.Drawing.Point(98, 19);
			this.cbShouldUseUntis.Name = "cbShouldUseUntis";
			this.cbShouldUseUntis.Size = new System.Drawing.Size(95, 17);
			this.cbShouldUseUntis.TabIndex = 8;
			this.cbShouldUseUntis.Text = "Use WebUntis";
			this.toolTip1.SetToolTip(this.cbShouldUseUntis, "Should classes be fetched from WebUntis?");
			this.cbShouldUseUntis.UseVisualStyleBackColor = true;
			this.cbShouldUseUntis.CheckedChanged += new System.EventHandler(this.cbShouldUseUntis_CheckedChanged);
			// 
			// tbSchool
			// 
			this.tbSchool.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbSchool.Location = new System.Drawing.Point(98, 68);
			this.tbSchool.Name = "tbSchool";
			this.tbSchool.Size = new System.Drawing.Size(696, 20);
			this.tbSchool.TabIndex = 6;
			this.toolTip1.SetToolTip(this.tbSchool, "WebUntis name of your school");
			this.tbSchool.TextChanged += new System.EventHandler(this.MarkAsDirty);
			// 
			// btLogin
			// 
			this.btLogin.Location = new System.Drawing.Point(199, 16);
			this.btLogin.Name = "btLogin";
			this.btLogin.Size = new System.Drawing.Size(75, 21);
			this.btLogin.TabIndex = 15;
			this.btLogin.Text = "Login";
			this.toolTip1.SetToolTip(this.btLogin, "Log in to WebUntis");
			this.btLogin.UseVisualStyleBackColor = true;
			this.btLogin.Click += new System.EventHandler(this.btLogin_Click);
			// 
			// laSchool
			// 
			this.laSchool.AutoSize = true;
			this.laSchool.Location = new System.Drawing.Point(23, 71);
			this.laSchool.Name = "laSchool";
			this.laSchool.Size = new System.Drawing.Size(69, 13);
			this.laSchool.TabIndex = 7;
			this.laSchool.Text = "School name";
			// 
			// tbServer
			// 
			this.tbServer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbServer.Location = new System.Drawing.Point(98, 42);
			this.tbServer.Name = "tbServer";
			this.tbServer.Size = new System.Drawing.Size(696, 20);
			this.tbServer.TabIndex = 5;
			this.toolTip1.SetToolTip(this.tbServer, "Name of the WebUntis server of your school");
			this.tbServer.TextChanged += new System.EventHandler(this.MarkAsDirty);
			// 
			// laServer
			// 
			this.laServer.AutoSize = true;
			this.laServer.Location = new System.Drawing.Point(6, 45);
			this.laServer.Name = "laServer";
			this.laServer.Size = new System.Drawing.Size(86, 13);
			this.laServer.TabIndex = 4;
			this.laServer.Text = "Webuntis Server";
			// 
			// gbPrefix
			// 
			this.gbPrefix.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.gbPrefix.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
			this.gbPrefix.Controls.Add(this.cbUseCustomPrefix);
			this.gbPrefix.Controls.Add(this.tbCustomPrefix);
			this.gbPrefix.Dock = System.Windows.Forms.DockStyle.Top;
			this.gbPrefix.Location = new System.Drawing.Point(0, 0);
			this.gbPrefix.Name = "gbPrefix";
			this.gbPrefix.Size = new System.Drawing.Size(800, 72);
			this.gbPrefix.TabIndex = 42;
			this.gbPrefix.TabStop = false;
			this.gbPrefix.Text = "Prefix";
			// 
			// cbUseCustomPrefix
			// 
			this.cbUseCustomPrefix.AutoSize = true;
			this.cbUseCustomPrefix.Location = new System.Drawing.Point(6, 23);
			this.cbUseCustomPrefix.Name = "cbUseCustomPrefix";
			this.cbUseCustomPrefix.Size = new System.Drawing.Size(183, 17);
			this.cbUseCustomPrefix.TabIndex = 2;
			this.cbUseCustomPrefix.Text = "Use custom prefix in school field?";
			this.toolTip1.SetToolTip(this.cbUseCustomPrefix, "Should custom prefix be used?");
			this.cbUseCustomPrefix.UseVisualStyleBackColor = true;
			this.cbUseCustomPrefix.CheckedChanged += new System.EventHandler(this.cbUseCustomPrefix_CheckedChanged);
			// 
			// tbCustomPrefix
			// 
			this.tbCustomPrefix.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbCustomPrefix.Enabled = false;
			this.tbCustomPrefix.Location = new System.Drawing.Point(6, 46);
			this.tbCustomPrefix.Name = "tbCustomPrefix";
			this.tbCustomPrefix.Size = new System.Drawing.Size(788, 20);
			this.tbCustomPrefix.TabIndex = 3;
			this.toolTip1.SetToolTip(this.tbCustomPrefix, "Prefix to be used in school field");
			this.tbCustomPrefix.TextChanged += new System.EventHandler(this.MarkAsDirty);
			// 
			// OptionMenu
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 446);
			this.Controls.Add(this.gbManagerOptions);
			this.Controls.Add(this.gbConfig);
			this.Controls.Add(this.gbWebUntis);
			this.Controls.Add(this.gbPrefix);
			this.Controls.Add(this.btSave);
			this.Controls.Add(this.btClose);
			this.MinimumSize = new System.Drawing.Size(495, 485);
			this.Name = "OptionMenu";
			this.Text = "OptionMenu";
			this.gbManagerOptions.ResumeLayout(false);
			this.gbManagerOptions.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudFontSize)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudTabStops)).EndInit();
			this.gbConfig.ResumeLayout(false);
			this.gbConfig.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudNumber)).EndInit();
			this.gbWebUntis.ResumeLayout(false);
			this.gbWebUntis.PerformLayout();
			this.gbPrefix.ResumeLayout(false);
			this.gbPrefix.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btClose;
		private System.Windows.Forms.Button btSave;
		private System.Windows.Forms.ToolTip toolTip1;
		private OwnControls.ColoredGroupBox gbPrefix;
		private System.Windows.Forms.CheckBox cbUseCustomPrefix;
		private System.Windows.Forms.TextBox tbCustomPrefix;
		private OwnControls.ColoredGroupBox gbWebUntis;
		private System.Windows.Forms.CheckBox cbShouldUseUntis;
		private System.Windows.Forms.TextBox tbSchool;
		private System.Windows.Forms.Button btLogin;
		private System.Windows.Forms.Label laSchool;
		private System.Windows.Forms.TextBox tbServer;
		private System.Windows.Forms.Label laServer;
		private OwnControls.ColoredGroupBox gbConfig;
		private System.Windows.Forms.CheckBox cbEndOfWeek;
		private System.Windows.Forms.TextBox tbNamingPattern;
		private System.Windows.Forms.TextBox tbName;
		private System.Windows.Forms.Label laNumber;
		private System.Windows.Forms.Label laName;
		private System.Windows.Forms.NumericUpDown nudNumber;
		private System.Windows.Forms.Label laNamingPattern;
		private System.Windows.Forms.Button btCreateTheme;
		private System.Windows.Forms.TextBox tbTemplate;
		private System.Windows.Forms.Button btEditTheme;
		private System.Windows.Forms.Label laTheme;
		private System.Windows.Forms.Label laTemplate;
		private OwnControls.ColoredComboBox coTheme;
		private OwnControls.ColoredGroupBox gbManagerOptions;
		private System.Windows.Forms.Label laTabStop;
		private System.Windows.Forms.NumericUpDown nudTabStops;
		private System.Windows.Forms.Label laFolder;
		private System.Windows.Forms.CheckBox cbLegacyEdit;
		private System.Windows.Forms.TextBox tbUpdate;
		private System.Windows.Forms.TextBox tbFolder;
		private System.Windows.Forms.Label laUpdate;
		private System.Windows.Forms.Label laFontSize;
		private System.Windows.Forms.NumericUpDown nudFontSize;
	}
}