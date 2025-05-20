using Word = Microsoft.Office.Interop.Word;
using BerichtManager.Config;
using BerichtManager.Forms;
using System.Globalization;
using BerichtManager.ThemeManagement;
using System.Diagnostics;
using BerichtManager.HelperClasses;
using BerichtManager.WebUntisClient;
using BerichtManager.OwnControls;
using BerichtManager.ReportChecking;
using System.Text;
using BerichtManager.ReportChecking.Discrepancies;
using BerichtManager.UploadChecking;
using BerichtManager.IHKClient;
using BerichtManager.IHKClient.Exceptions;
using BerichtManager.IHKClient.ReportContents;
using BerichtManager.WordTemplate;
using BerichtManager.OwnControls.OwnTreeView;
using BerichtManager.Extensions;
using BerichtManager.OwnControls.CustomToolTipControls;
using BerichtManager.ReportQuickInfo;

namespace BerichtManager
{
	public partial class MainForm : Form
	{
		/// <summary>
		/// The currently open word document
		/// </summary>
		private Word.Document? Doc { get; set; }
		/// <summary>
		/// Global instance of Word
		/// </summary>
		private Word.Application? WordApp { get; set; }
		private ConfigHandler ConfigHandler { get; } = ConfigHandler.Instance;
		private Client Client { get; } = new Client();
		/// <summary>
		/// Client which is used to interact with IHK services
		/// </summary>
		private IHKClient.IHKClient IHKClient { get; } = new IHKClient.IHKClient();

		/// <summary>
		/// Directory containing all reports
		/// </summary>
		private DirectoryInfo Info { get; set; }
		public static CultureInfo Culture { get; } = new CultureInfo("de-DE");
		public static DateTimeFormatInfo DateTimeFormatInfo => Culture.DateTimeFormat;
		private int TvReportsMaxWidth { get; set; } = 50;
		/// <summary>
		/// If a word document is opened
		/// </summary>
		private bool EditMode { get; set; } = false;
		/// <summary>
		/// If <see cref="rtbSchool"/> or <see cref="rtbWork"/> have been edited
		/// </summary>
		private bool WasEdited { get; set; } = false;

		/// <summary>
		/// Value if word has a visible window or not
		/// </summary>
		private bool WordVisible { get; set; } = false;

		/// <summary>
		/// Version number
		/// Major.Minor.Build.Revision
		/// </summary>
		public const string VersionNumber = "1.24.1";

		/// <summary>
		/// String to be printed
		/// </summary>
		private string VersionString { get; } = "v" + VersionNumber;

		/// <summary>
		/// Full path to report folder
		/// </summary>
		private string ActivePath { get => ConfigHandler.ReportPath; }

		/// <summary>
		/// Status if the word app is running
		/// </summary>
		private bool WordIsOpen = false;

		/// <summary>
		/// Factory for creating tasks that start word
		/// </summary>
		private TaskFactory WordTaskFactory { get; } = Task.Factory;

		/// <summary>
		/// Generates path to file from selected node
		/// </summary>
		private string FullSelectedPath
		{
			get
			{
				if (tvReports.SelectedNode == null)
					return "";
				return Path.GetFullPath(ActivePath + "\\..\\" + tvReports.SelectedNode.FullPath);
			}
		}

		/// <summary>
		/// <see cref="List{Fields}"/> of <see cref="Fields"/> that have manually created <see cref="ToolStripMenuItem"/>s
		/// </summary>
		private List<Fields> DefaultQuickEditActions => new List<Fields>() { Fields.Work, Fields.School, Fields.Number };

		private ReportNode? _OpenedReportNode { get; set; }
		/// <summary>
		/// <see cref="ReportNode"/> of report opened for edit in <see cref="WordApp"/>
		/// </summary>
		private ReportNode? OpenedReportNode
		{
			get => _OpenedReportNode;
			set
			{
				if (OpenedReportNode == value)
					return;
				if (OpenedReportNode != null)
				{
					OpenedReportNode.IsOpened = false;
					tvReports.ExecuteWithInvoke(() => tvReports.Invalidate(OpenedReportNode.Bounds));
				}
				_OpenedReportNode = value;
				if (OpenedReportNode != null)
					OpenedReportNode.IsOpened = true;
			}
		}

		private CustomTreeNode? rootNode;
		private CustomTreeNode RootNode
		{
			get
			{
				if (rootNode == null)
					rootNode = CreateDirectoryNode(Info);
				return rootNode;
			}
			set
			{
				if (rootNode != value)
					rootNode = value;
			}
		}

		public MainForm()
		{
			InitializeComponent();
			tvReports.TreeViewNodeSorter = new TreeNodeSorter();
			tvReports.CustomNodeDrawer = new CustomNodeDrawer();
			tvReports.ShowNodeToolTips = ConfigHandler.ShowReportToolTip;
			tvReports.Select();
			AddQuickEdits();
			ThemeSetter.SetThemes(this);
			ThemeSetter.SetThemes(ttTips);
			Info = new DirectoryInfo(ConfigHandler.ReportPath);
			rtbSchool.WordWrap = ConfigHandler.UseWordWrap;
			rtbWork.WordWrap = ConfigHandler.UseWordWrap;
			UpdateTree();
			if (ConfigHandler.LastCreated == "")
			{
				miEditLatest.Enabled = false;
			}
			miIHK.Visible = ConfigHandler.UseIHK;
			miIHKOptions.Visible = ConfigHandler.UseIHK;
			SetComponentPositions();
			UpdateTabStops(this, ConfigHandler.TabStops);
			if (File.Exists(ConfigHandler.PublishPath) && CompareVersionNumbers(VersionNumber, FileVersionInfo.GetVersionInfo(ConfigHandler.PublishPath).FileVersion) > 0)
				VersionString += "*";
			WordTaskFactory.StartNew(RestartWordIfNeeded);
		}

		/// <summary>
		/// Compares two version numbers
		/// </summary>
		/// <param name="version1">Version number 1</param>
		/// <param name="version2">Version number 2</param>
		/// <returns>0 if versions are equal, positive if version2 is greater and negative if version2 is smaller</returns>
		private int CompareVersionNumbers(string? version1, string? version2)
		{
			if (version1 == null || version2 == null)
				return string.Compare(version1, version2);
			string[] splitv1 = version1.Split('.');
			string[] splitv2 = version2.Split('.');
			if (splitv1.Length == splitv2.Length)
			{
				for (int i = 0; i < splitv1.Length; i++)
				{
					if (splitv1[i] == splitv2[i])
						continue;
					if (int.TryParse(splitv1[i], out int v1) && int.TryParse(splitv2[i], out int v2))
					{
						return v2 - v1;
					}
					else
					{
						return splitv2[i].CompareTo(splitv1[i]);
					}
				}
			}
			else
			{
				for (int i = 0; i < Math.Min(splitv1.Length, splitv2.Length); i++)
				{
					if (splitv1[i] == splitv2[i])
						continue;
					if (int.TryParse(splitv1[i], out int v1) && int.TryParse(splitv2[i], out int v2))
					{
						return v2 - v1;
					}
					else
					{
						return splitv2[i].CompareTo(splitv1[i]);
					}
				}
				return splitv2.Length - splitv1.Length;
			}
			return 0;
		}

		/// <summary>
		/// Sets sizes and width for components
		/// </summary>
		public void SetComponentPositions()
		{
			tvReports.ExpandAll();
			foreach (TreeNode node in tvReports.Nodes)
				TvReportsCalcMaxWidth(node);
			tvReports.CollapseAll();
			splitterTreeBoxes.SplitPosition = TvReportsMaxWidth + 5;
			Rectangle bounds = scTextBoxes.Bounds;
			bounds.X = paFileTree.Bounds.Right + 1;
			bounds.Width = Width - 1 - paFileTree.Bounds.Width;
			scTextBoxes.Bounds = bounds;
			rtbSchool.Font = new Font(rtbSchool.Font.FontFamily, ConfigHandler.EditorFontSize);
			rtbWork.Font = new Font(rtbWork.Font.FontFamily, ConfigHandler.EditorFontSize);
		}

		/// <summary>
		/// Calculates the max size the tree view should have
		/// </summary>
		/// <param name="treeNode">root nodes of tree</param>
		private void TvReportsCalcMaxWidth(TreeNode treeNode)
		{
			foreach (TreeNode node in treeNode.Nodes)
			{
				if (node.Nodes.Count > 0)
				{
					TvReportsCalcMaxWidth(node);
				}
				else
				{
					if (TvReportsMaxWidth < node.Bounds.Right + 1)
						TvReportsMaxWidth = node.Bounds.Right + 20;
				}
			}
		}

		/// <summary>
		/// Fills the TreeView of the main form with nodes of the upper directory</summary>
		/// </summary>
		private void UpdateTree()
		{
			void update()
			{
				string? openedNodePath = GetFullNodePath(OpenedReportNode);
				List<TreeNode> expanded = new List<TreeNode>();
				if (tvReports.Nodes.Count > 0)
					expanded = GetExpandedNodes(RootNode);

				tvReports.Nodes.Clear();
				RootNode = CreateDirectoryNode(Info);
				tvReports.Nodes.Add(RootNode);

				expanded.ForEach(node =>
				{
					string? path = GetFullNodePath(node);
					GetNodeFromPath(path)?.Expand();
				});

				FillReportNodes(RootNode);

				if (openedNodePath is string)
					OpenedReportNode = GetNodeFromPath(openedNodePath) as ReportNode;
				tvReports.Sort();
			}

			this.ExecuteWithInvoke(update);
		}

		/// <summary>
		/// Searches <paramref name="root"/> for all expanded nodes
		/// </summary>
		/// <param name="root">Root <see cref="TreeNode"/> to search</param>
		/// <returns><see cref="List{TreeNode}"/> of <see cref="TreeNode"/>s which are expanded</returns>
		private List<TreeNode> GetExpandedNodes(TreeNode root)
		{
			List<TreeNode> result = new List<TreeNode>();

			if (root.IsExpanded)
				result.Add(root);
			foreach (TreeNode child in root.Nodes)
			{
				if (child.IsExpanded)
					result.Add(child);
				result.AddRange(GetExpandedNodes(child));
			}

			return result;
		}

		/// <summary>
		/// Generates TreeNodes from files and directorys contained in the upper directory
		/// </summary>
		/// <param name="directoryInfo">The target directory</param>
		/// <returns>A Treenode representing the contents of <paramref name="directoryInfo"/></returns>
		//https://stackoverflow.com/questions/6239544/populate-treeview-with-file-system-directory-structure
		private CustomTreeNode CreateDirectoryNode(DirectoryInfo directoryInfo)
		{
			CustomTreeNode directoryNode = new CustomTreeNode(directoryInfo.Name);
			foreach (var directory in directoryInfo.GetDirectories())
				directoryNode.Nodes.Add(CreateDirectoryNode(directory));
			foreach (var file in directoryInfo.GetFiles())
				if (ReportUtils.IsNameValid(file.Name))
					directoryNode.Nodes.Add(new ReportNode(file.Name, file.LastWriteTime));
				else
					directoryNode.Nodes.Add(new CustomTreeNode(file.Name));
			return directoryNode;
		}

		/// <summary>
		/// Sets the <see cref="UploadedReport"/> properties in all <see cref="ReportNode"/>s contained in <paramref name="node"/>
		/// </summary>
		/// <param name="node"><see cref="TreeNode"/> to set properties of child <see cref="ReportNode"/>s for</param>
		private void FillReportNodes(TreeNode node)
		{
			if (node is ReportNode reportNode)
			{
				if (UploadedReports.GetUploadedReport(GetFullNodePath(reportNode), out UploadedReport? report))
					reportNode.SetReportProperties(report);
				if (QuickInfos.GetQuickInfo(GetFullNodePath(reportNode), out QuickInfo? info))
					reportNode.SetReportProperties(info);
				if (ConfigHandler.ShowReportToolTip)
					reportNode.SetToolTip();
			}
			foreach (TreeNode child in node.Nodes)
				FillReportNodes(child);
		}

		/// <summary>
		/// Sets the global font in a document and fits document to pages
		/// </summary>
		/// <param name="doc">The Document which needs a font change</param>
		/// <param name="app">The Wordapp containing the document</param>
		private void SetFontInDoc(Word.Document doc, Word.Application app)
		{
			doc.Content.Select();
			if (app.Selection.Font.Name != ConfigHandler.EditorFont)
			{
				app.Selection.Font.Name = ConfigHandler.EditorFont;
				ThemedMessageBox.Show(text: "Changed report Font to: " + ConfigHandler.EditorFont, title: "Font changed!");
			}
			FitToPage(doc);
		}

		/// <summary>
		/// Fits <paramref name="doc"/> to pages
		/// </summary>
		/// <param name="doc"></param>
		private void FitToPage(Word.Document? doc = null)
		{
			try
			{
				doc?.FitToPages();
			}
			catch
			{

			}
		}

		/// <summary>
		/// Shows and handles the dialogresult of the passed <paramref name="form"/>
		/// </summary>
		/// <param name="doc">Reference of the opened <see cref="Word.Document"/></param>
		/// <param name="form"><see cref="EditForm"/> to show and handle</param>
		/// <param name="field">Form field to set value for</param>
		/// <returns><see langword="true"/> if document was closed and creation should be aborted, <see langword="false"/> otherwise</returns>
		private static bool ExitOnDialogResult(Word.Document doc, EditForm form, Fields field)
		{
			switch (form.ShowDialog())
			{
				case DialogResult.Abort:
					doc.Close(Word.WdSaveOptions.wdDoNotSaveChanges);
					return true;
				//Skip
				case DialogResult.Cancel:
					break;
				default:
					FormFieldHandler.SetValueInDoc(field, doc, form.Result);
					break;
			}
			return false;
		}

		/// <summary>
		/// Creates a single <see cref="Word.Document"/>
		/// </summary>
		/// <param name="dayInReport"><see cref="DateTime"/> in span of report</param>
		/// <param name="wasOnVacation">Wether or not manual fields should be filled for vacation</param>
		/// <param name="onlyAutomaticFields">Wether or not manual fields should be left blank</param>
		/// <param name="autofillValues">Cache of values to autofill manual fields with (Used to cache entered values if word closes while creating document)</param>
		private void CreateDocument(DateTime dayInReport, bool wasOnVacation = false, bool onlyAutomaticFields = false, Dictionary<Fields, string?>? autofillValues = null)
		{
			Word.Document? doc = null;
			bool createComplete = false;
			if (string.IsNullOrEmpty(ConfigHandler.TemplatePath))
			{
				ThemedMessageBox.Show(text: "Please open the option menu and select a word template to use.", title: "Please select a word template to use");
				return;
			}
			if (!File.Exists(ConfigHandler.TemplatePath))
			{
				ThemedMessageBox.Show(text: ConfigHandler.TemplatePath + " was not found was it moved or deleted?", title: "Template not found");
				return;
			}
			if (string.IsNullOrEmpty(ConfigHandler.ReportUserName))
			{
				EditForm nameForm = new EditForm(title: "Please enter the name to use in reports.");
				if (nameForm.ShowDialog() != DialogResult.OK)
				{
					ThemedMessageBox.Show(text: "Please provide a name in order to create reports!", title: "No name was provided.");
					return;
				}
				ConfigHandler.ReportUserName = nameForm.Result!;
				ConfigHandler.SaveConfig();
			}
			try
			{
				int weekOfYear = dayInReport.GetIsoWeekOfYear();
				RestartWordIfNeeded();
				doc = WordApp!.Documents.Add(Template: ConfigHandler.TemplatePath);
				int reportNumber = ConfigHandler.ReportNumber;
				FillEssentials(doc, dayInReport, reportNumber, out string startDate);

				if (!onlyAutomaticFields)
				{
					autofillValues ??= new Dictionary<Fields, string?>();

					string? workText;
					if (autofillValues.TryGetValue(Fields.Work, out string? work) && !string.IsNullOrWhiteSpace(work))
						workText = work;
					else
						workText = wasOnVacation ? $"{(ConfigHandler.UseCustomPrefix ? ConfigHandler.CustomPrefix : "-")}Urlaub" : "";

					EditForm workForm = new EditForm(title: "Betriebliche Tätigkeiten" + "(KW " + weekOfYear + ")", isCreate: true, text: workText);
					if (ExitOnDialogResult(doc, workForm, Fields.Work))
						return;
					autofillValues.Add(Fields.Work, workForm.Result);

					string? seminarsText;
					if (autofillValues.TryGetValue(Fields.Seminars, out string? seminars) && !string.IsNullOrWhiteSpace(seminars))
						seminarsText = seminars;
					else
						seminarsText = $"{ConfigHandler.AutoSeminarsValue}";

					if (ConfigHandler.AskForSeminars)
					{
						EditForm seminarsForm = new EditForm(title: "Unterweisungen, betrieblicher Unterricht, sonstige Schulungen" + "(KW " + weekOfYear + ")", text: seminarsText, isCreate: true);
						if (ExitOnDialogResult(doc, seminarsForm, Fields.Seminars))
							return;
						autofillValues.Add(Fields.Seminars, seminarsForm.Result);
					}
					else
					{
						FormFieldHandler.SetValueInDoc(Fields.Seminars, doc, seminarsText);
					}

					string? schoolText;
					if (autofillValues.TryGetValue(Fields.School, out string? school) && !string.IsNullOrWhiteSpace(school))
						schoolText = school;
					else if (DateTime.Today.GetIsoWeekOfYear() - dayInReport.GetIsoWeekOfYear() <= ConfigHandler.WebUntisMaxAllowedWeeksLookback)
					{
						List<string> classes;
						try
						{
							classes = Client.GetClassesFromWebUntis();
						}
						catch (Exception ex)
						{
							ThemedMessageBox.Info(text: $"{ex.GetType().Name} while fetching classes from WebUntis.", title: "Failed to fetch classes!");
							Logger.LogError(ex);
							classes = new List<string>();
						}
						schoolText = string.Join("", classes);
					}
					else
					{
						schoolText = Client.GetHolidaysForDate(dayInReport);
					}
					EditForm schoolForm = new EditForm(title: "Berufsschule (Unterrichtsthemen)" + "(KW " + weekOfYear + ")", isCreate: true, text: schoolText);
					if (ExitOnDialogResult(doc, schoolForm, Fields.School))
						return;
					autofillValues.Add(Fields.School, schoolForm.Result);
				}

				Directory.CreateDirectory(Path.Combine(ActivePath, GetYearOfReport(dayInReport).ToString()));
				string name = NamingPatternResolver.ResolveName(weekOfYear.ToString(), ConfigHandler.ReportNumber.ToString());
				string path = Path.Combine(ActivePath, GetYearOfReport(dayInReport).ToString(), name + ".docx");
				FitToPage(doc);
				doc.SaveAs2(FileName: path);

				QuickInfos.AddOrUpdateQuickInfo(path, new QuickInfo(startDate, reportNumber));
				ConfigHandler.ReportNumber++;
				ConfigHandler.LastReportCreateDate = dayInReport;
				ConfigHandler.LastCreated = path;
				ConfigHandler.SaveConfig();
				createComplete = true;

				miEditLatest.Enabled = true;
				UpdateTree();
				ThemedMessageBox.Info(text: "Created Document at: " + path, title: "Document saved", allowMessageHighlight: true);

				SaveOrExit();
				Doc = doc;
				OpenedReportNode = GetNodeFromPath(path) as ReportNode;
				rtbWork.Text = FormFieldHandler.GetValueFromDoc<string>(Fields.Work, Doc);
				rtbSchool.Text = FormFieldHandler.GetValueFromDoc<string>(Fields.School, Doc);
				EditMode = true;
				WasEdited = false;
			}
			catch (Exception ex)
			{
				switch (ex.HResult)
				{
					case -2147023174:
					case -2147467262:
					case -2146823679:
					case -2147023179:
					//{"Der Remoteprozeduraufruf ist fehlgeschlagen. (Ausnahme von HRESULT: 0x800706BE)"}
					case -2147023170:
						if (createComplete)
						{
							ThemedMessageBox.Show(text: "Unable to automatically open report, Word was closed unexpectedly", title: "Loading was cancelled because word closed");
							return;
						}
						ThemedMessageBox.Show(text: "Word closed unexpectedly and is restarting please wait while it restarts");
						RestartWordIfNeeded();
						CreateDocument(dayInReport, wasOnVacation: wasOnVacation, onlyAutomaticFields: onlyAutomaticFields, autofillValues: autofillValues);
						break;
					case -2146233088:
						ThemedMessageBox.Show(text: "Connection refused by remotehost");
						break;
					//Default file format in word is not compatible with .docx
					case -2146821994:
						ThemedMessageBox.Show(text: "Please change default document format in your Word app under File>Options>Save>DefaultFileFormat, then try again.", title: "Please change Word settingd");
						doc?.Close(SaveChanges: false);
						break;
					default:
						ThemedMessageBox.Error(ex);
						try
						{
							doc?.Close(SaveChanges: false);
						}
						catch
						{

						}
						break;
				}
			}
		}

		/// <summary>
		/// Fills all automatic fields in <paramref name="doc"/>
		/// </summary>
		/// <param name="doc"><see cref="Word.Document"/> to fill with values</param>
		/// <param name="dayInReport"><see cref="DateTime"/> in span of report</param>
		/// <param name="reportNumber">Number of report</param>
		/// <param name="reportStartDate"><see cref="DateTime"/> of start date formatted to <see cref="DateTimeUtils.DATEFORMAT"/></param>
		private void FillEssentials(Word.Document doc, DateTime dayInReport, int reportNumber, out string reportStartDate)
		{
			FormFieldHandler.SetValueInDoc(Fields.Name, doc, ConfigHandler.ReportUserName);
			FormFieldHandler.SetValueInDoc(Fields.Number, doc, reportNumber.ToString());

			DateTime today = new DateTime(dayInReport.Year, dayInReport.Month, dayInReport.Day);
			DateTime thisWeekStart = today.AddDays(-(int)dayInReport.DayOfWeek + 1);
			DateTime thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);

			if (ConfigHandler.EndWeekOnFriday)
				thisWeekEnd = thisWeekEnd.AddDays(-2);
			reportStartDate = thisWeekStart.ToString(DateTimeUtils.DATEFORMAT);

			FormFieldHandler.SetValueInDoc(Fields.Year, doc, GetYearOfReport(dayInReport).ToString());
			FormFieldHandler.SetValueInDoc(Fields.StartDate, doc, reportStartDate);
			FormFieldHandler.SetValueInDoc(Fields.EndDate, doc, thisWeekEnd.ToString(DateTimeUtils.DATEFORMAT));
			FormFieldHandler.SetValueInDoc(Fields.SignDateYou, doc, thisWeekEnd.ToString(DateTimeUtils.DATEFORMAT));
			FormFieldHandler.SetValueInDoc(Fields.SignDateSupervisor, doc, thisWeekEnd.ToString(DateTimeUtils.DATEFORMAT));
		}

		/// <summary>
		/// Gets the correct year for the report
		/// </summary>
		/// <param name="dayInReport"><see cref="DateTime"/> of date in report span</param>
		/// <returns>Year of report</returns>
		private int GetYearOfReport(DateTime dayInReport)
		{
			DateTime today = new DateTime(dayInReport.Year, dayInReport.Month, dayInReport.Day);
			DateTime thisWeekStart = today.AddDays(-(int)dayInReport.DayOfWeek + 1);
			DateTime thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
			if (thisWeekStart.Year == thisWeekEnd.Year)
				return dayInReport.Year;
			else if (thisWeekStart.GetIsoWeekOfYear() == 1)
				return thisWeekEnd.Year;
			else
				return thisWeekStart.Year;
		}

		private void btClose_Click(object sender, EventArgs e)
		{
			Close();
		}

		/// <summary>
		/// Used for detecting missing reports and initiating their creation, only creates past reports
		/// </summary>
		/// <param name="vacation">Passed on to <see cref="CreateDocument(string, DateTime, Word.Application, bool, int, bool)"/> for if you were on vacation</param>
		/// <param name="empty">Indicates that all reports should only contain automatically filled infos</param>
		private void CreateMissing(bool vacation = false, bool empty = false)
		{
			if (ConfigHandler.LastReportCreateDate is not DateTime lastCreateDate)
				return;
			int deltaWeeks = (DateTime.Today - lastCreateDate).Days / 7;

			for (int i = deltaWeeks - 1; i > 0; i--)
			{
				CreateDocument(DateTime.Today.AddDays(-7 * i), wasOnVacation: vacation, onlyAutomaticFields: empty);
			}
		}

		private void btCreate_Click(object sender, EventArgs e)
		{
			//Check if report for this week was already created
			string docName = NamingPatternResolver.ResolveNameWithExtension(DateTime.Today, ConfigHandler.ReportNumber - 1);
			if (File.Exists(Path.Combine(ActivePath, DateTime.Today.Year.ToString(), docName)) || File.Exists(Path.Combine(ActivePath, DateTime.Today.Year.ToString(), PRINTEDFOLDERNAME, docName)))
			{
				ThemedMessageBox.Show(text: "A report has already been created for this week");
				return;
			}
			//Check if a report was created
			if (ConfigHandler.LastReportCreateDate != null)
			{
				//Check if report for last week was created
				if ((DateTime.Today - ConfigHandler.LastReportCreateDate.Value).Days / 7 > 1)
				{
					if (ThemedMessageBox.Show(text: "You missed some reports were you on vacation?", title: "Vacation?", buttons: MessageBoxButtons.YesNo) == DialogResult.Yes)
					{
						CreateMissing(vacation: true);
					}
					else if (ThemedMessageBox.Show(text: "Do you want to write the missing empty reports then?", title: "Create?", buttons: MessageBoxButtons.YesNo) == DialogResult.Yes)
					{
						CreateMissing();
					}
					else if (ThemedMessageBox.Show(text: "Do you want to create all empty reports?", title: "Create all reports?", buttons: MessageBoxButtons.YesNo) == DialogResult.Yes)
					{
						CreateMissing(empty: true);
					}
				}
			}

			CreateDocument(DateTime.Today);
		}

		private void miEditLatest_Click(object sender, EventArgs e)
		{
			if (DocIsSamePathAsSelected())
				return;
			SaveOrExit();
			if (ConfigHandler.UseLegacyEdit)
				Edit(ConfigHandler.LastCreated);
			else
				EditInTb(ConfigHandler.LastCreated);
		}

		private void btPrintAll_Click(object sender, EventArgs e)
		{
			try
			{
				if (!Directory.Exists(ActivePath))
					return;

				StartWaitCursor();
				bool stop = false;
				EventProgressForm progressForm = new EventProgressForm("Print reports");
				progressForm.Stop += () => stop = true;
				progressForm.Show();

				FolderSelect select = new FolderSelect(RootNode, node =>
				{
					bool isReport = (node is ReportNode reportNode);
					bool emptyNonReportNode = !ReportUtils.IsNameValid(node.Text) && node.Nodes.Count == 0;
					return !isReport && emptyNonReportNode;
				}, title: "Select reports to print");
				select.ShowDialog();
				if (select.DialogResult != DialogResult.OK)
				{
					progressForm.Done();
					return;
				}

				if (stop)
					return;

				int printed = 0;
				ReportFinder.FindReports(select.FilteredNode, out List<TreeNode> reports);
				foreach (TreeNode node in reports)
				{
					if (stop)
						break;
					string filePath = GetFullPath(node);
					string message = PrintReport(filePath) ?? $"Printed {filePath}";
					progressForm.Status = message;
					if (message == null)
						printed++;
				}

				string title = stop ? "Print canceled" : "Print finished";
				progressForm.Status = title;
				progressForm.Done();
				ThemedMessageBox.Show(text: $"Printed {printed} / {reports.Count} reports.", title: title);

			}
			catch (Exception ex)
			{
				ThemedMessageBox.Error(ex, title: "Print canceled");
			}
			finally
			{
				UpdateTree();
				EndWaitCursor();
			}
		}

		/// <summary>
		/// Method for editing a Word document at a path relative to the working directory
		/// </summary>
		/// <param name="path">The path relative to the working directory</param>
		/// <param name="field"><see cref="Fields"/> of report to jump edit to</param>
		/// <param name="quickEditTitle">Title of the editor window</param>
		public void Edit(string path, Fields? field = null, string quickEditTitle = "")
		{
			try
			{
				if (!File.Exists(path))
				{
					ThemedMessageBox.Show(text: path + " not found was it deleted or moved?");
					return;
				}
				if (!ReportUtils.IsNameValid(Path.GetFileName(path)))
					return;
				RestartWordIfNeeded();

				bool readOnly = !UploadedReports.CanBeEdited(path);
				if (!DocIsSamePathAsSelected())
				{
					CloseOpenDocument();
					Doc = WordApp!.Documents.Open(FileName: path, ReadOnly: readOnly);
				}
				if (Doc == null)
				{
					ThemedMessageBox.Show(text: "Unable to open report.", title: "Unable to open report");
					return;
				}

				if (!FormFieldHandler.ValidFormFieldCount(Doc))
				{
					ThemedMessageBox.Show(text: "Invalid document (you will have to manually edit)");
					Doc.Close(SaveChanges: false);
					Doc = null;
					return;
				}

				rtbWork.Text = FormFieldHandler.GetValueFromDoc<string>(Fields.Work, Doc);
				rtbWork.ReadOnly = readOnly;
				rtbSchool.Text = FormFieldHandler.GetValueFromDoc<string>(Fields.School, Doc);
				rtbSchool.ReadOnly = readOnly;

				bool save = false;
				if (field.HasValue)
				{
					string? value = FormFieldHandler.GetValueFromDoc<string>(field.Value, Doc);
					EditForm edit = new EditForm(title: quickEditTitle, text: value, isReadonly: readOnly);
					edit.RefreshConfigs += RefreshConfig;
					switch (edit.ShowDialog())
					{
						//Confirm
						case DialogResult.OK:
						//Save and quit
						case DialogResult.Ignore:
							if (edit.Result == value)
							{
								//No need to save if value has not changed
								break;
							}
							save = true;
							FormFieldHandler.SetValueInDoc(field.Value, Doc, edit.Result);
							break;
						//Skip
						case DialogResult.Cancel:
						//Quit
						case DialogResult.Abort:
						default:
							break;
					}
					edit.RefreshConfigs -= RefreshConfig;
				}
				else
				{
					SelectEditFrom selectEdit = new SelectEditFrom();
					if (selectEdit.ShowDialog() != DialogResult.OK)
						return;
					if (selectEdit.SelectedFields.Count == 0)
					{
						SaveOrExit();
						return;
					}

					bool stopLoop = false;
					foreach (SelectedField selected in selectEdit.SelectedFields)
					{
						if (stopLoop)
							break;
						string? value = FormFieldHandler.GetValueFromDoc<string>(selected.Field, Doc);
						EditForm edit = new EditForm(title: selected.CheckBoxText, text: value, isReadonly: readOnly);
						edit.RefreshConfigs += RefreshConfig;
						switch (edit.ShowDialog())
						{
							//Confirm
							case DialogResult.OK:
								if (edit.Result == value)
									break;
								save = true;
								FormFieldHandler.SetValueInDoc(selected.Field, Doc, edit.Result);
								break;
							//Save and quit
							case DialogResult.Ignore:
								stopLoop = true;
								if (edit.Result == value)
									break;
								save = true;
								FormFieldHandler.SetValueInDoc(selected.Field, Doc, edit.Result);
								break;
							//Quit
							case DialogResult.Abort:
								stopLoop = true;
								save = false;
								break;
							default:
								break;
						}
						edit.RefreshConfigs -= RefreshConfig;
					}

				}

				if (save && Doc.ReadOnly != true)
				{
					FitToPage(Doc);
					Doc.Save();
					UploadedReports.SetEdited(path, true);
					UpdateTree();
					ThemedMessageBox.Show(text: "Saved changes", title: "Saved");
				}

				rtbWork.Text = FormFieldHandler.GetValueFromDoc<string>(Fields.Work, Doc);
				rtbWork.ReadOnly = readOnly;
				rtbSchool.Text = FormFieldHandler.GetValueFromDoc<string>(Fields.School, Doc);
				rtbSchool.ReadOnly = readOnly;
				EditMode = true;
				WasEdited = false;
				OpenedReportNode = GetNodeFromPath(path) as ReportNode;
			}
			catch (Exception ex)
			{
				switch (ex.HResult)
				{
					case -2147023174:
					case -2147467262:
					case -2146823679:
						ThemedMessageBox.Show(text: "Word closed unexpectedly and is restarting please try again shortly");
						RestartWordIfNeeded();
						break;
					//case -2146822750:
					//Document is only one page long
					/*doc.Save();
					doc.Close();
					doc = null;*/
					//	break;
					case -2146233088:
						ThemedMessageBox.Show(text: "Connection refused by remotehost");
						break;
					default:
						ThemedMessageBox.Error(ex);
						break;
				}
			}
		}

		/// <summary>
		/// Retrieves a <see cref="TreeNode"/> from <see cref="tvReports"/> that lies at <paramref name="path"/>
		/// </summary>
		/// <param name="path">Path of report</param>
		/// <returns><see cref="TreeNode"/> that <paramref name="path"/> leads to</returns>
		private TreeNode? GetNodeFromPath(string? path)
		{
			if (path == null)
				return null;
			List<string> segments = path.Replace("/", "\\").Split('\\').ToList();
			segments.RemoveRange(0, segments.IndexOf(RootNode.Text) + 1);
			TreeNode result = RootNode;

			foreach (string segment in segments)
			{
				foreach (TreeNode node in result.Nodes)
				{
					if (node.Text == segment)
					{
						result = node;
						break;
					}
				}
			}
			if (path != RootNode.Text && result == RootNode)
				return null;
			return result;
		}

		/// <summary>
		/// Opens the ontents for work and school form fields in textboxes
		/// </summary>
		/// <param name="path">document to open</param>
		private void EditInTb(string path)
		{
			if (Doc != null)
				if (DocIsSamePathAsSelected())
					return;
				else
					SaveOrExit();
			try
			{
				if ((File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory)
					return;
				if (!File.Exists(path))
					return;
				if (!ReportUtils.IsNameValid(Path.GetFileName(path)))
					return;

				if (ConfigHandler.IHKAutoGetComment && ConfigHandler.UseIHK && UploadedReports.GetUploadedReport(path, out UploadedReport? report) && report.LfdNR.HasValue && report.LfdNR > 0)
				{
					Task<CommentResult> commentTask = Task.Run(async () =>
					{
						try
						{
							return await GetCommentResult(report.LfdNR);
						}
						catch (Exception ex)
						{
							return new CommentResult(CommentResult.ResultStatus.Exception, exception: ex);
						}
					});
					CommentResult result = commentTask.Result;
					HandleCommentResult(result);
				}

				RestartWordIfNeeded();
				bool readOnly = !UploadedReports.CanBeEdited(path);
				Doc = WordApp!.Documents.Open(FileName: path, ReadOnly: readOnly);
				if (!FormFieldHandler.ValidFormFieldCount(Doc))
				{
					ThemedMessageBox.Show(text: "Invalid document (you will have to manually edit)");
					Doc.Close(SaveChanges: false);
					Doc = null;
					EditMode = false;
					WasEdited = false;
					OpenedReportNode = null;
					return;
				}

				if (GetNodeFromPath(path) is ReportNode reportNode)
					OpenedReportNode = reportNode;

				rtbWork.Text = FormFieldHandler.GetValueFromDoc<string>(Fields.Work, Doc);
				rtbWork.ReadOnly = readOnly;
				rtbSchool.Text = FormFieldHandler.GetValueFromDoc<string>(Fields.School, Doc);
				rtbSchool.ReadOnly = readOnly;
				EditMode = true;
				WasEdited = false;
			}
			catch (Exception ex)
			{
				switch (ex.HResult)
				{
					case -2147023174:
					case -2147467262:
					case -2146823679:
						ThemedMessageBox.Show(text: "Word closed unexpectedly and is restarting please try again shortly");
						RestartWordIfNeeded();
						break;
					//case -2146822750:
					//	//Document is only one page long
					//	/*doc.Save();
					//	doc.Close();
					//	doc = null;*/
					//	break;
					case -2146233088:
						ThemedMessageBox.Show(text: "Connection refused by remotehost");
						break;
					default:
						ThemedMessageBox.Error(ex);
						break;
				}
			}
		}

		/// <summary>
		/// Saves active document does not close
		/// </summary>
		private void SaveFromTb()
		{
			try
			{
				if (Doc == null || !CheckIfWordRunning() || !WasEdited)
					return;
				//Stop saving of accepted reports
				if (UploadedReports.GetUploadedReport(Doc.FullName, out UploadedReport? report))
				{
					switch (report.Status)
					{
						case ReportNode.UploadStatuses.Accepted:
							rtbWork.Text = FormFieldHandler.GetValueFromDoc<string>(Fields.Work, Doc);
							rtbSchool.Text = FormFieldHandler.GetValueFromDoc<string>(Fields.School, Doc);
							WasEdited = false;
							ThemedMessageBox.Show(text: "Can not change accepted report", title: "Save not possible");
							return;
						case ReportNode.UploadStatuses.HandedIn:
							rtbWork.Text = FormFieldHandler.GetValueFromDoc<string>(Fields.Work, Doc);
							rtbSchool.Text = FormFieldHandler.GetValueFromDoc<string>(Fields.School, Doc);
							WasEdited = false;
							ThemedMessageBox.Show(text: "Can not change handed in report", title: "Save not possible");
							return;
					}
				}

				FormFieldHandler.SetValueInDoc(Fields.Work, Doc, rtbWork.Text);
				FormFieldHandler.SetValueInDoc(Fields.School, Doc, rtbSchool.Text);

				FitToPage(Doc);
				Doc.Save();
				if (WasEdited)
				{
					UploadedReports.SetEdited(Doc.FullName, true);
					UpdateTree();
				}
				ThemedMessageBox.Show(text: "Saved changes", title: "Saved");
				WasEdited = false;
			}
			catch (Exception ex)
			{
				switch (ex.HResult)
				{
					case -2147023174:
					case -2147467262:
					case -2146823679:
						ThemedMessageBox.Show(text: "Word closed unexpectedly and is restarting please try again shortly");
						RestartWordIfNeeded();
						break;
					//case -2146822750:
					//	//Document is one page already
					//	break;
					case -2146233088:
						ThemedMessageBox.Show(text: "Connection refused by remotehost");
						break;
					default:
						ThemedMessageBox.Error(ex);
						break;
				}
			}
		}

		/// <summary>
		/// Either saves the changes and closes report or just closes report
		/// </summary>
		private void SaveOrExit()
		{
			OpenedReportNode = null;
			if (Doc == null)
				return;
			if (!EditMode)
				return;
			if (!CheckIfWordRunning())
				return;

			if (WasEdited && !Doc.ReadOnly && ThemedMessageBox.Show(text: "Save unsaved changes?", title: "Save?", buttons: MessageBoxButtons.YesNo) == DialogResult.Yes)
				SaveFromTb();
			Doc.Close(SaveChanges: false);
			Doc = null;
			EditMode = false;
			WasEdited = false;
		}

		/// <summary>
		/// Checks if the document path is the same as the selected report path
		/// </summary>
		/// <returns>is document path same as selected report path</returns>
		private bool DocIsSamePathAsSelected()
		{
			if (Doc == null)
				return false;
			try
			{
				return FullSelectedPath == Doc.FullName;
			}
			catch
			{
				return false;
			}
		}

		private const string PRINTEDFOLDERNAME = "Gedruckt";
		/// <summary>
		/// Prints a report and moves it into the folder of printed reports
		/// </summary>
		/// <param name="path">Path of report to print</param>
		/// <returns>Result message or <see langword="null"/> if successful</returns>
		private string? PrintReport(string path)
		{
			if (!Path.IsPathRooted(path))
				path = Path.Combine(ActivePath, path);
			if (!File.Exists(path))
				return "File not found";
			if (Path.GetExtension(path) != ".docx")
				return "File is not a report (.docx)";

			DirectoryInfo reportFolder = new DirectoryInfo(path);
			bool pathAlreadyPrinted = reportFolder.Name == PRINTEDFOLDERNAME;
			if (!pathAlreadyPrinted && !reportFolder.GetDirectories().Any(folder => folder.Name == PRINTEDFOLDERNAME))
				reportFolder.CreateSubdirectory(PRINTEDFOLDERNAME);

			try
			{
				RestartWordIfNeeded();
				bool isSameAsOpened;
				if (Doc != null)
					isSameAsOpened = path == Doc.FullName;
				else
					isSameAsOpened = false;

				Word.Document document;
				if (isSameAsOpened)
					document = Doc!;
				else
					document = WordApp!.Documents.Open(path, ReadOnly: true);
				document.PrintOut(Background: false);
				if (!isSameAsOpened)
					document.Close();
				if (!pathAlreadyPrinted)
				{
					string oldRelPath = path.Substring(ActivePath.LastIndexOf(Path.DirectorySeparatorChar) + 1);
					List<string> splitPath = oldRelPath.Split(Path.DirectorySeparatorChar).ToList();
					splitPath.Insert(splitPath.Count - 1, PRINTEDFOLDERNAME);
					var newRelPath = Path.Combine(splitPath.ToArray());
					File.Move(path, Path.Combine(path.Substring(0, path.Length - Path.GetFileName(path).Length), PRINTEDFOLDERNAME, Path.GetFileName(path)));
					UploadedReports.MoveReport(oldRelPath, newRelPath);
				}
			}
			catch (Exception ex)
			{
				return $"{ex.GetType().Name} while printing {path}";
			}
			return null;
		}

		/// <summary>
		/// Method for deleting a file located at the path
		/// </summary>
		/// <param name="path">Path of file to delete</param>
		private void DeleteDocument(string path)
		{
			if (!File.Exists(path))
			{
				if (File.GetAttributes(path) == FileAttributes.Directory)
				{
					ThemedMessageBox.Show(text: "You may not delete folders using the manager");
					return;
				}
				ThemedMessageBox.Show(text: path + " not Found (was it moved or deleted?)");
				return;
			}
			if (Path.GetExtension(path) != ".docx" && !path.Contains("\\Logs") && !Path.GetFileName(path).StartsWith("~$"))
			{
				ThemedMessageBox.Show(text: "You may only delete Word documents (*.docx), their temporary files or logs");
				return;
			}
			if (ThemedMessageBox.Show(text: $"Are you sure you want to {path}?", title: "Delete?", buttons: MessageBoxButtons.YesNo) != DialogResult.Yes)
				return;
			if (path == Doc?.FullName)
			{
				Doc.Close(SaveChanges: false);
				Doc = null;
				rtbSchool.Text = "";
				rtbWork.Text = "";
				EditMode = false;
				WasEdited = false;
				OpenedReportNode = null;
			}

			//Roll back config if last created report is deleted
			ResolvedValues values = NamingPatternResolver.GetValuesFromName(path);
			if (values.CalendarWeek == ConfigHandler.LastReportCreateDate?.GetWeekOfYear() || values.ReportNumber == ConfigHandler.ReportNumber - 1)
			{
				ConfigHandler.LastReportCreateDate = ConfigHandler.LastReportCreateDate?.AddDays(-7);
				ConfigHandler.ReportNumber--;
				ConfigHandler.SaveConfig();
			}

			File.Delete(path);
			UpdateTree();
			ThemedMessageBox.Show(text: "File deleted successfully");
		}

		private void tvReports_DoubleClick(object sender, EventArgs e)
		{
			if (!File.Exists(FullSelectedPath))
				return;
			if ((File.GetAttributes(FullSelectedPath) & FileAttributes.Directory) == FileAttributes.Directory)
				return;
			if (!ReportUtils.IsNameValid(Path.GetFileName(FullSelectedPath)))
				return;
			if (tvReports.SelectedNode == null)
				return;
			if (DocIsSamePathAsSelected())
				return;
			if (ConfigHandler.UseLegacyEdit)
				Edit(FullSelectedPath);
			else
				EditInTb(FullSelectedPath);
		}

		private void tvReports_Click(object sender, EventArgs e)
		{
			if (e is not MouseEventArgs mea || mea.Button != MouseButtons.Right)
				return;
			tvReports.SelectedNode = tvReports.GetNodeAt(mea.X, mea.Y);
		}

		private void miDelete_Click(object sender, EventArgs e)
		{
			DeleteDocument(FullSelectedPath);
		}

		private void miEdit_Click(object sender, EventArgs e)
		{
			SaveOrExit();
			Edit(FullSelectedPath);
		}

		private void miPrint_Click(object sender, EventArgs e)
		{
			if (DocIsSamePathAsSelected() && WasEdited && ThemedMessageBox.Show(text: $"Report {FullSelectedPath} has unsaved changes!\nSave?", title: "Save unsaved changes", buttons: MessageBoxButtons.YesNo) == DialogResult.Yes)
				SaveFromTb();
			StartWaitCursor();
			string? result = PrintReport(FullSelectedPath);
			ThemedMessageBox.Show(text: result ?? $"Printed {FullSelectedPath}", title: result == null ? "Print finished" : "Print aborted");
			UpdateTree();
			EndWaitCursor();
		}

		private void miQuickEditWork_Click(object sender, EventArgs e)
		{
			QuickEdit(Fields.Work, "Edit work");
		}

		private void miQuickEditSchool_Click(object sender, EventArgs e)
		{
			QuickEdit(Fields.School, "Edit school");
		}

		private void miQuickEditNumber_Click(object sender, EventArgs e)
		{
			QuickEdit(Fields.Number, "Edit report number");
		}

		private void QuickEdit(Fields field, string title)
		{
			SaveOrExit();
			Edit(FullSelectedPath, field: field, quickEditTitle: title);
		}

		private void AddQuickEdits()
		{
			miQuickEditOptions.DropDownItems.Clear();
			List<(Fields Field, ToolStripItem Item)> toAdd = new List<(Fields, ToolStripItem)>();
			foreach (KeyValuePair<Fields, FormField> kvp in FormFieldHandler.GetCurrentFields())
			{
				Fields field = kvp.Key;
				string displayName = kvp.Value.DisplayText;
				toAdd.Add((field, new CustomToolTipStripMenuItem($"Edit {displayName}", null, (s, e) => QuickEdit(field, $"Edit {displayName}")) { Name = $"miQuickEdit{field}" }));
			}
			toAdd.Sort(new Comparison<(Fields Field, ToolStripItem Item)>((i1, i2) =>
			{
				bool i1IsDefault = DefaultQuickEditActions.Contains(i1.Field);
				bool i2IsDefault = DefaultQuickEditActions.Contains(i2.Field);
				if (i1IsDefault && i2IsDefault)
					return DefaultQuickEditActions.IndexOf(i1.Field).CompareTo(DefaultQuickEditActions.IndexOf(i2.Field) * -1);
				else if (i1IsDefault && !i2IsDefault)
					return -1;
				else if (!i1IsDefault && i2IsDefault)
					return 1;
				else
					return 0;
			}));
			miQuickEditOptions.DropDownItems.AddRange(toAdd.Select(item => item.Item).ToArray());
		}

		private void toRightClickMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			bool isInLogs = tvReports.SelectedNode?.Parent?.Text == "Logs";
			bool isNameValid = ReportUtils.IsNameValid(tvReports.SelectedNode?.Text);
			bool isTempFile = tvReports.SelectedNode?.Text.StartsWith("~$") == true;
			bool isReportFile = tvReports.SelectedNode?.Text.EndsWith(".docx") == true;
			bool isTempTemplateFile = tvReports.SelectedNode?.Text.EndsWith(".dotx") == true && isTempFile;
			bool isUploaded = UploadedReports.GetUploadedReport(tvReports.SelectedNode?.FullPath, out UploadedReport? report);
			bool uploaded = report?.Status == ReportNode.UploadStatuses.Uploaded;
			bool rejected = report?.Status == ReportNode.UploadStatuses.Rejected;
			bool wasEdited = report?.WasEditedLocally == true;
			bool wasUpdated = report?.WasUpdated == true;

			miEdit.Enabled = !isInLogs && isNameValid;
			//miEdit.Visible = !isInLogs && tvReports.SelectedNode.Text.EndsWith(".docx") && !tvReports.SelectedNode.Text.StartsWith("~$");
			miPrint.Enabled = !isInLogs && isNameValid;
			//miPrint.Visible = !isInLogs && tvReports.SelectedNode.Text.EndsWith(".docx") && !tvReports.SelectedNode.Text.StartsWith("~$");
			miDelete.Enabled = isInLogs || isReportFile || isTempTemplateFile || isNameValid;
			//miDelete.Visible = isInLogs || tvReports.SelectedNode.Text.EndsWith(".docx") || tvReports.SelectedNode.Text.StartsWith("~$");
			miQuickEditOptions.Enabled = !isInLogs && isNameValid;
			//miQuickEditOptions.Visible = !isInLogs && tvReports.SelectedNode.Text.EndsWith(".docx") && !tvReports.SelectedNode.Text.StartsWith("~$");
			miUploadAsNext.Enabled = !isInLogs && isNameValid && !isUploaded;
			miHandInSingle.Enabled = isNameValid && isUploaded && (uploaded || rejected && (wasEdited || wasUpdated));
			miUpdateReport.Enabled = isNameValid && isUploaded && wasEdited && (uploaded || rejected);
			miRcShowComment.Enabled = isNameValid && isUploaded && report!.LfdNR.HasValue;
			miDownloadReports.Enabled = miRcDownloadReports.Enabled = !string.IsNullOrEmpty(ConfigHandler.IHKUserName) && !string.IsNullOrEmpty(ConfigHandler.IHKPassword);
		}

		private void btOptions_Click(object sender, EventArgs e)
		{
			OptionMenu optionMenu = new OptionMenu();
			optionMenu.ActiveThemeChanged += ActiveThemeChanged;
			optionMenu.ReportFolderChanged += ReportFolderChanged;
			optionMenu.TabStopsChanged += UpdateTabStops;
			optionMenu.FontSizeChanged += ChangeFontSize;
			optionMenu.IHKBaseAddressChanged += IHKBaseAddressChanged;
			optionMenu.UseWordWrapChanged += UseWordWrapChanged;
			optionMenu.ShowReportToolTipChanged += ShowReportToolTipChanged;
			optionMenu.UseIHKChanged += UseIHKChanged;
			optionMenu.ShowDialog();
			optionMenu.ActiveThemeChanged -= ActiveThemeChanged;
			optionMenu.ReportFolderChanged -= ReportFolderChanged;
			optionMenu.TabStopsChanged -= UpdateTabStops;
			optionMenu.FontSizeChanged -= ChangeFontSize;
			optionMenu.IHKBaseAddressChanged -= IHKBaseAddressChanged;
			optionMenu.UseWordWrapChanged -= UseWordWrapChanged;
			optionMenu.ShowReportToolTipChanged -= ShowReportToolTipChanged;
			optionMenu.UseIHKChanged -= UseIHKChanged;
		}

		private void UseIHKChanged(bool status)
		{
			miIHK.Visible = status;
			miIHKOptions.Visible = status;
		}

		private void ShowReportToolTipChanged(bool status)
		{
			tvReports.ShowNodeToolTips = status;
			if (status)
				ThemeSetter.SetThemes(tvReports.NodeToolTip);
		}

		private void UseWordWrapChanged(bool useWordWrap)
		{
			rtbSchool.WordWrap = useWordWrap;
			rtbWork.WordWrap = useWordWrap;
		}

		private void IHKBaseAddressChanged()
		{
			IHKClient.UpdateBaseURL();
		}

		private void ReportFolderChanged(object sender, string folderPath)
		{
			Info = new DirectoryInfo(folderPath);
			UpdateTree();
		}

		private void UpdateTabStops(object sender, int tabStops)
		{
			List<int> tabs = new List<int>();
			for (int i = 0; i < 32; i++)
			{
				tabs.Add(i * tabStops);
			}
			string tempBuffer = rtbSchool.Text;
			rtbSchool.Text = "";
			rtbSchool.SelectionTabs = tabs.ToArray();
			rtbSchool.Text = tempBuffer;
			tempBuffer = rtbWork.Text;
			rtbWork.Text = "";
			rtbWork.SelectionTabs = tabs.ToArray();
			rtbWork.Text = tempBuffer;
		}

		private void ActiveThemeChanged()
		{
			ThemeSetter.SetThemes(this);
			ThemeSetter.SetThemes(ttTips);
			tvReports.Refresh();
		}

		private void OnKeyDownDefault(object sender, KeyEventArgs e)
		{
			if (e.Control && e.KeyCode == Keys.S && EditMode)
				SaveFromTb();
			if (e.Control && e.Shift && e.KeyCode == Keys.V)
				PasteOnlyText(sender, e);
			if (e.KeyCode == Keys.Escape)
				CloseOpenDocument();
		}

		/// <summary>
		/// Strips the formatting from text in <see cref="Clipboard"/> and pastes it into <paramref name="sender"/>
		/// </summary>
		/// <param name="sender"><see cref="RichTextBox"/> to paste <see cref="Clipboard"/> content to</param>
		/// <param name="e"><see cref="KeyEventArgs"/> of KeyDown event</param>
		private void PasteOnlyText(object sender, KeyEventArgs e)
		{
			if (sender is not RichTextBox rtb)
				return;
			e.SuppressKeyPress = true;
			rtb.Paste(DataFormats.GetFormat(DataFormats.Text));
		}

		private void EditRichTextBox(object sender, EventArgs e)
		{
			WasEdited = true;
		}

		private void miWordVisible_Click(object sender, EventArgs e)
		{
			if (CheckIfWordRunning())
			{
				WordApp!.Visible = miWordVisible.Checked;
				WordVisible = miWordVisible.Checked;
			}
			else
			{
				miWordVisible.Checked = false;
				WordVisible = false;
			}
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				if (EditMode && WasEdited && ThemedMessageBox.Show(text: "Do you want to save unsaved changes?", title: "Save changes?", buttons: MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					SaveFromTb();
				}
				if (Doc != null)
				{
					Doc.Close(SaveChanges: false);
					Doc = null;
				}
				try
				{
					WordApp?.Quit(SaveChanges: false);
				}
				catch { }
				if (WordApp != null)
					System.Runtime.InteropServices.Marshal.FinalReleaseComObject(WordApp);
			}
			catch (Exception ex)
			{
				Console.Write(ex.StackTrace);
			}
		}

		private void menuStrip1_Paint(object sender, PaintEventArgs e)
		{
			int versionNumberWidth = (int)e.Graphics.MeasureString(VersionNumber, menuStrip1.Font).Width / 2;
			if (sender is not Control control)
				return;
			TextRenderer.DrawText(e.Graphics, VersionString, menuStrip1.Font, new Point(control.Location.X + control.Width / 2 - versionNumberWidth, control.Location.Y + control.Padding.Top + 2), control.ForeColor);
		}

		private async void tvReports_KeyUp(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Enter:
					if (tvReports.SelectedNode == null) return;
					if ((File.GetAttributes(FullSelectedPath) & FileAttributes.Directory) == FileAttributes.Directory)
					{
						if (tvReports.SelectedNode.Nodes.Count > 0)
						{

							if (tvReports.SelectedNode.IsExpanded)
								tvReports.SelectedNode.Collapse();
							else
								tvReports.SelectedNode.Expand();
						}
						return;
					}
					if (!ReportUtils.IsNameValid(Path.GetFileName(FullSelectedPath)))
						return;
					RestartWordIfNeeded();
					if (DocIsSamePathAsSelected())
						return;
					SaveOrExit();
					EditInTb(FullSelectedPath);
					break;
				case Keys.Delete:
					DeleteDocument(FullSelectedPath);
					break;
				case Keys.C:
					if (!e.Control)
						return;
					int? lfdnr;
					if (OpenedReportNode != null)
						lfdnr = OpenedReportNode.LfdNr;
					else if (tvReports.GetNodeAt(tvReports.PointToClient(new Point(Cursor.Position.X, Cursor.Position.Y))) is ReportNode reportNode)
						lfdnr = reportNode.LfdNr;
					else
					{
						ThemedMessageBox.Info(text: "No report selected to fetch comment for.", title: "No node available");
						return;
					}
					StartWaitCursor();
					HandleCommentResult(await GetCommentResult(lfdnr));
					EndWaitCursor();
					break;
			}
		}

		/// <summary>
		/// Used to start displaying the wait cursor on the form and its children
		/// </summary>
		private void StartWaitCursor()
		{
			UseWaitCursor = true;
		}

		/// <summary>
		/// Used to stop displaying the wait cursor on the form and its children
		/// </summary>
		private void EndWaitCursor()
		{
			UseWaitCursor = false;
		}

		private void MiRefresh_Click(object sender, EventArgs e)
		{
			UpdateTree();
		}

		private void miRevealInExplorer_Click(object sender, EventArgs e)
		{
			if (Directory.Exists(ActivePath))
				Process.Start("explorer.exe", ActivePath);
			else
				ThemedMessageBox.Show(text: "The working directory has been deleted from an external source", title: "You may have a problem");
		}

		/// <summary>
		/// Reloads <see cref="ConfigHandler"/>
		/// </summary>
		public void RefreshConfig()
		{
			ConfigHandler.ReloadConfig();
		}

		/// <summary>
		/// Delegate to execute when word closes
		/// </summary>
		private void OnWordClose()
		{
			Doc = null;
			WordIsOpen = false;
		}

		/// <summary>
		/// Checks if word should be running by catching an exception if it is not
		/// </summary>
		/// <returns><see langword="true"/> if word is still running and <see langword="false"/> if not</returns>
		private bool CheckIfWordRunning()
		{
			if (WordApp == null || !WordIsOpen)
				return false;
			try
			{
				var dummy = WordApp.Version;
			}
			catch
			{
				WordIsOpen = false;
				return false;
			}
			return true;
		}

		/// <summary>
		/// Restarts word if it has been closed
		/// </summary>
		private void RestartWordIfNeeded()
		{
			if (CheckIfWordRunning())
				return;
			StartWaitCursor();
			WordApp = new Word.Application();
			EndWaitCursor();
			((Word.ApplicationEvents4_Event)WordApp).Quit += OnWordClose;
			WordIsOpen = true;
			miWordVisible.Checked = WordApp.Visible;
		}

		/// <summary>
		/// Changes font size of editing text boxes
		/// </summary>
		/// <param name="fontSize">Size to set font to</param>
		private void ChangeFontSize(float fontSize)
		{
			bool wasEdited = this.WasEdited;
			rtbSchool.Font = new Font(rtbSchool.Font.FontFamily, fontSize);
			rtbWork.Font = new Font(rtbWork.Font.FontFamily, fontSize);
			this.WasEdited = wasEdited;
		}

		private void miCloseReport_Click(object sender, EventArgs e)
		{
			if (Doc == null || !EditMode)
			{
				ThemedMessageBox.Show(text: "No opened report to close", title: "Could not close");
				return;
			}
			CloseOpenDocument();
		}

		/// <summary>
		/// Closes the opened report
		/// </summary>
		private void CloseOpenDocument()
		{
			if (Doc == null)
				return;
			SaveOrExit();
			rtbSchool.ExecuteWithInvoke(() =>
			{
				rtbSchool.Text = "";
				rtbSchool.ReadOnly = false;
			});
			rtbWork.ExecuteWithInvoke(() =>
			{
				rtbWork.Text = "";
				rtbWork.ReadOnly = false;
			});
			WasEdited = false;
			EditMode = false;
		}

		/// <summary>
		/// Checks all selected reports for discrepancies and handles output
		/// </summary>
		/// <param name="check">Kind of check to execute</param>
		private void CheckDiscrepancies(ReportChecker.CheckKinds check)
		{
			FolderSelect select = new FolderSelect(RootNode, node => !ReportUtils.IsNameValid(node.Text) && node.Nodes.Count == 0);
			if (select.ShowDialog() != DialogResult.OK)
				return;
			if (select.FilteredNode == null)
			{
				ThemedMessageBox.Show(text: "No file or folder was selected, check was canceled", title: "No selection was made");
				return;
			}
			RestartWordIfNeeded();
			string activePath = Doc?.FullName ?? "";
			List<string> openReports = CloseAllReports();
			ReportChecker checker = new ReportChecker(WordApp!);
			StartWaitCursor();
			if (!checker.Check(select.FilteredNode, out List<IReportDiscrepancy> discrepancies, check: check))
			{
				EndWaitCursor();
				OpenAllDocuments(openReports, activePath);
				return;
			}
			EndWaitCursor();
			OpenAllDocuments(openReports, activePath);
			if (discrepancies == null)
				return;
			if (discrepancies.Count == 0)
			{
				ThemedMessageBox.Show(text: "No discrepancies found", "Check done");
				return;
			}
			StringBuilder message = new StringBuilder();
			message.AppendLine("At least one discrepancy was found:");
			discrepancies.ForEach(d => message.AppendLine(d.ToString()));
			ThemedMessageBox.Show(text: message.ToString(), title: "Discrepancy found");
		}

		/// <summary>
		/// Checks if two <see cref="Word.Document"/>s are equal by comparing the differences in their contents
		/// </summary>
		/// <param name="doc1">First <see cref="Word.Document"/> to compare</param>
		/// <param name="doc2">Second <see cref="Word.Document"/> to compare</param>
		/// <returns><see langword="true"/> if docs contents are the same and both are not <see langword="null"/> or <see langword="false"/> otherwise</returns>
		private bool IsDocSameDoc(Word.Document? doc1, Word.Document? doc2)
		{
			if (doc1 == null || doc2 == null)
				return false;
			RestartWordIfNeeded();
			Word.Document compareDoc = WordApp!.CompareDocuments(doc1, doc2);
			bool docIsSameAsOpened = compareDoc.Revisions.Count == 0;
			compareDoc.Close(SaveChanges: false);
			return docIsSameAsOpened;
		}

		/// <summary>
		/// Opens a list of <see cref="Word.Document"/>s from <paramref name="paths"/> and opens <paramref name="activePath"/> in text boxes
		/// </summary>
		/// <param name="paths">Paths of previously opened reports</param>
		/// <param name="activePath">Path to open in text box edit</param>
		private void OpenAllDocuments(List<string> paths, string? activePath)
		{
			foreach (string path in paths)
			{
				if (string.IsNullOrWhiteSpace(path))
					continue;
				if (path == activePath)
					this.ExecuteWithInvoke(() => EditInTb(path));
				else
					Doc = WordApp!.Documents.Open(FileName: path);
			};
		}

		/// <summary>
		/// Closes all open reports
		/// </summary>
		/// <returns><see cref="List{T}"/> of paths from previously opened reports</returns>
		private List<string> CloseAllReports()
		{
			List<string> result = new List<string>();
			foreach (Word.Document doc in WordApp!.Documents)
			{
				result.Add(doc.FullName);
				if (IsDocSameDoc(Doc, doc))
					CloseOpenDocument();
				else
					doc.Close();
			}
			return result;
		}

		private void CheckNumbers_Click(object sender, EventArgs e)
		{
			CheckDiscrepancies(ReportChecker.CheckKinds.Numbers);
		}

		private void CheckDates_Click(object sender, EventArgs e)
		{
			CheckDiscrepancies(ReportChecker.CheckKinds.Dates);
		}

		private void FullCheck_Click(object sender, EventArgs e)
		{
			CheckDiscrepancies(ReportChecker.CheckKinds.All);
		}

		/// <summary>
		/// Uploads a single report to IHK, handles exceptions
		/// </summary>
		/// <param name="doc">Report to upload</param>
		/// <param name="ihkSiteReportsCache"><see cref="List{UploadedReport}"/> of <see cref="UploadedReport"/>s from IHK site to check if report was already uploaded</param>
		/// <returns><see cref="UploadResult"/> of creation or <see langword="null"/> if an error occurred</returns>
		private async Task<UploadResult?> TryUploadReportToIHK(Word.Document doc, List<UploadedReport>? ihkSiteReportsCache = null)
		{
			try
			{
				Report report = ReportTransformer.WordToIHK(doc);
				if (ihkSiteReportsCache == null)
					ihkSiteReportsCache = await IHKClient.GetIHKReports();
				if (ihkSiteReportsCache == null)
					return null;
				if (ihkSiteReportsCache.FirstOrDefault(r => r.StartDate.ToString("dd.MM.yyyy") == report.ReportContent.StartDate) is UploadedReport uploadedReport)
					return new UploadResult(CreateResults.ReportAlreadyUploaded, uploadedReport.StartDate, uploadedReport.LfdNR);

				return await IHKClient.CreateReport(doc, ConfigHandler.IHKCheckMatchingStartDates);
			}
			catch (HttpRequestException ex)
			{
				Logger.LogError(ex);
				ThemedMessageBox.Show(text: "A network error has occurred, please check your connection", title: "Network error");
				return null;
			}
			catch (StartDateMismatchException ex)
			{
				ThemedMessageBox.Show(text: ex.Message, title: ex.GetType().Name);
				return null;
			}
			catch (Exception ex)
			{
				ThemedMessageBox.Error(ex);
				return null;
			}
		}

		private async void miUploadAsNext_Click(object sender, EventArgs e)
		{
			//should not happen as menu item should be disabled
			if (UploadedReports.GetUploadedReport(tvReports.SelectedNode.FullPath, out _))
			{
				ThemedMessageBox.Show(text: "Report was already uploaded", title: "Report already uploaded");
				return;
			}

			StartWaitCursor();

			Word.Document doc;
			bool close = true;
			if (DocIsSamePathAsSelected())
			{
				close = false;
				doc = Doc!;
			}
			else
				doc = WordApp!.Documents.Open(FullSelectedPath);
			if (!FormFieldHandler.ValidFormFieldCount(doc))
			{
				ThemedMessageBox.Show(text: "Invalid document, please upload manually", title: "Invalid document");
				if (close)
					doc.Close(SaveChanges: false);
				EndWaitCursor();
				return;
			}
			UploadResult? result = await TryUploadReportToIHK(doc);
			if (result == null)
			{
				ThemedMessageBox.Show(text: "Upload of report failed", title: "Upload failed");
				if (close)
					doc.Close(SaveChanges: false);
				EndWaitCursor();
				return;
			}

			//Handle upload result
			HandleUploadResult(result, doc, null, FullSelectedPath, tvReports.SelectedNode.FullPath, new List<string>(), FullSelectedPath, out _, out _, closeDoc: close);

			EndWaitCursor();
			UpdateTree();
		}

		/// <summary>
		/// Uploads a selection of reports to IHK
		/// </summary>
		/// <param name="progressForm"><see cref="EventProgressForm"/> to display progress on</param>
		/// <returns><see cref="Task"/> that runs the hand ins and can be <see langword="await"/>ed</returns>
		private Task UploadSelection(EventProgressForm progressForm)
		{
			return Task.Run(async () =>
			{
				bool shouldStop = false;
				bool aborted = false;
				int uploaded = 0;
				progressForm.Stop += () => shouldStop = true;

				FolderSelect fs = new FolderSelect(RootNode, node =>
				{
					bool isReport = node is ReportNode reportNode;
					bool wasUploaded = UploadedReports.GetUploadedReport(GetFullNodePath(node), out UploadedReport? report);
					bool emptyNonReportNode = !ReportUtils.IsNameValid(node.Text) && node.Nodes.Count == 0;
					return isReport && wasUploaded || emptyNonReportNode;
				});
				if (fs.ShowDialog() != DialogResult.OK)
				{
					progressForm.Done();
					return;
				}
				ReportFinder.FindReports(fs.FilteredNode, out List<TreeNode> reports);
				progressForm.Status = $"Uploading {reports.Count} reports";

				string activePath = Doc?.FullName ?? "";
				progressForm.Status = "Closing open reports";
				List<string> openReports = CloseAllReports();

				//Cache IHK report list
				List<UploadedReport>? ihkReports = await IHKClient.GetIHKReports();
				if (ihkReports == null)
				{
					string status = "Unable to fetch list of uploaded reports from IHK, aborting upload!";
					ThemedMessageBox.Show(text: status, title: "Unable to fetch list of uploaded reports");
					progressForm.Status = status;
					progressForm.Done();
					return;
				}

				foreach (TreeNode report in reports)
				{
					string? nodePath = GetFullNodePath(report);
					//Should not happen
					if (nodePath == null)
					{
						aborted = true;
						progressForm.Status = $"Could not construct file path of a report {report.Parent?.Text}\\{report.Text}";
						break;
					}
					string path = Path.Combine(ActivePath, "..", nodePath);

					progressForm.Status = $"Uploading {nodePath}";

					Word.Document doc = WordApp!.Documents.Open(path);
					if (!FormFieldHandler.ValidFormFieldCount(doc))
					{
						progressForm.Status = $"Uploading aborted: Invalid dcument";
						ThemedMessageBox.Show(text: $"Invalid document, please add missing form fields to {path}.\nAborting upload", title: "Invalid document");
						doc.Close(SaveChanges: false);
						aborted = true;
						break;
					}
					UploadResult? result = await TryUploadReportToIHK(doc, ihkReports);
					if (result == null)
					{
						progressForm.Status = $"Uploading aborted: upload failed";
						ThemedMessageBox.Show(text: $"Upload of {path} failed, aborting upload!", title: "Upload failed");
						doc.Close(SaveChanges: false);
						aborted = true;
						break;
					}

					//Handle upload result
					if (!HandleUploadResult(result, doc, progressForm, path, nodePath, openReports, activePath, out bool uploadFailed, out bool stop, closeDoc: false))
					{
						string status = $"Unexpected upload result: {result.Result}, aborting upload";
						ThemedMessageBox.Show(text: status, title: "Unexpected result");
						progressForm.Status = status;
						doc.Close(SaveChanges: false);
						aborted = true;
						break;
					}
					if (uploadFailed)
					{
						progressForm.Status = $"Uploading aborted: upload failed";
						ThemedMessageBox.Show(text: $"Upload of {path} failed, aborting upload!", title: "Upload failed");
						doc.Close(SaveChanges: false);
						aborted = true;
						break;
					}
					if (stop)
					{
						progressForm.Status = $"Uploaded {path}, but unable to get lfdnr from IHK";
						ThemedMessageBox.Show(text: progressForm.Status, title: "Upload stopped");
						shouldStop = true;
						aborted = true;
					}

					uploaded++;

					doc.Close(SaveChanges: false);

					if (shouldStop)
					{
						progressForm.Status = $"Stopping";
						break;
					}

					await Task.Delay(ConfigHandler.IHKUploadDelay);
				}

				progressForm.Status = "Opening closed reports";
				OpenAllDocuments(openReports, activePath);
				if (aborted)
				{
					ThemedMessageBox.Show(text: $"Upload was aborted, {uploaded} / {reports.Count} reports were uploaded.", title: "Upload was aborted");
					progressForm.Status = $"Upload aborted, {uploaded} / {reports.Count} uploaded";
				}
				else
				{
					string text = "";
					if (reports.Count == 1)
						text = "Upload of report was succesful.";
					else if (uploaded != reports.Count)
						text = $"Upload of {uploaded} / {reports.Count} reports was successful.";
					else
						text = $"Upload of all {reports.Count} reports was successful.";
					ThemedMessageBox.Show(text: text, title: shouldStop ? "Upload stopped" : "Upload finished");
					progressForm.Status = $"Done";
				}
				progressForm.Done();
				UpdateTree();
			});
		}

		private async void UploadSelectionClick(object sender, EventArgs e)
		{
			if (ThemedMessageBox.Show(text: "Warning, this will upload all reports selected in the next window in the order they appear!\nDo you want to proceed?", title: "Caution", buttons: MessageBoxButtons.YesNo) != DialogResult.Yes)
				return;
			RestartWordIfNeeded();
			EventProgressForm progressForm = new EventProgressForm("Upload progress");
			progressForm.Show();
			await UploadSelection(progressForm);
		}

		/// <summary>
		/// Handles incoming <see cref="UploadResult"/>s
		/// </summary>
		/// <param name="doc"><see cref="Word.Document"/> that is being uploaded</param>
		/// <param name="result"><see cref="UploadResult"/> of upload process</param>
		/// <param name="progressForm"><see cref="EventProgressForm"/> to display data on if <see langword="null"/> is provided, a <see cref="ThemedMessageBox"/> will be shown</param>
		/// <param name="reportFilePath">Path of report file</param>
		/// <param name="nodePath">Path of report node</param>
		/// <param name="openReports"><see cref="List{string}"/> of paths of previously open reports</param>
		/// <param name="activePath">Path of last open report</param>
		/// <param name="uploadFailed"><see langword="true"/> if further execution is advised to return as upload has failed and <see langword="false"/> otherwise</param>
		/// <param name="closeDoc">Wether or not <paramref name="doc"/> should be closed if necessary</param>
		/// <param name="shouldStop"><see langword="true"/> if further uploading is disadvised</param>
		/// <returns><see langword="true"/> if <paramref name="result"/> was handled and <see langword="false"/> otherwise</returns>
		private bool HandleUploadResult(UploadResult result, Word.Document doc, EventProgressForm? progressForm, string reportFilePath, string nodePath, List<string> openReports,
			string activePath, out bool uploadFailed, out bool shouldStop, bool closeDoc = true)
		{
			bool res = true;
			uploadFailed = true;
			shouldStop = false;
			string progressFormNewStatus;
			bool shouldCallDone = false;
			switch (result.Result)
			{
				case CreateResults.ReportAlreadyUploaded:
					UploadedReports.AddReport(nodePath, new UploadedReport(result.StartDate, lfdNr: result.LfdNR));
					progressFormNewStatus = $"Report {reportFilePath} was already uploaded, marking it as uploaded";
					uploadFailed = false;
					break;
				case CreateResults.Success:
					UploadedReports.AddReport(nodePath, new UploadedReport(result.StartDate, lfdNr: result.LfdNR));
					progressFormNewStatus = "Upload successful";
					uploadFailed = false;
					break;
				case CreateResults.UnableToFetchLFDNR:
					if (closeDoc)
						doc.Close(SaveChanges: false);
					UploadedReports.AddReport(nodePath, new UploadedReport(result.StartDate));
					progressFormNewStatus = "Unable to read lfdnr from IHK, update manually";
					uploadFailed = false;
					shouldStop = true;
					break;
				case CreateResults.Unauthorized:
					if (closeDoc)
						doc.Close(SaveChanges: false);
					OpenAllDocuments(openReports, activePath);
					progressFormNewStatus = $"Abort: Unauthorized";
					shouldCallDone = true;
					shouldStop = true;
					break;
				case CreateResults.CreationFailed:
				case CreateResults.UploadFailed:
					if (closeDoc)
						doc.Close(SaveChanges: false);
					OpenAllDocuments(openReports, activePath);
					progressFormNewStatus = $"Abort: Upload failed";
					shouldCallDone = true;
					shouldStop = true;
					break;
				default:
					progressFormNewStatus = $"Unknown creation result: {result.Result}";
					res = false;
					shouldStop = true;
					break;
			}
			if (progressForm != null)
			{
				progressForm.Status = progressFormNewStatus;
				if (shouldCallDone)
					progressForm.Done();
			}
			else
			{
				ThemedMessageBox.Show(text: progressFormNewStatus, title: "Upload result");
			}
			return res;
		}

		/// <summary>
		/// Generates the full path of the <paramref name="node"/>
		/// </summary>
		/// <param name="node"><see cref="TreeNode"/> to get path for</param>
		/// <param name="separator">String to separate path elements with</param>
		/// <returns>Full path separated by <paramref name="separator"/> or <see langword="null"/> if <paramref name="node"/> is <see langword="null"/></returns>
		private string? GetFullNodePath(TreeNode? node, string separator = "\\")
		{
			TreeNode? current = node;
			if (node == null || current == null)
				return null;

			if (node.TreeView != null)
				return node.FullPath;

			string path = node.Text;
			while (current.Parent != null)
			{
				current = current.Parent;
				path = current.Text + separator + path;
			}
			return path;
		}

		private async void miUpdateStatuses_Click(object sender, EventArgs e)
		{
			try
			{
				if (await UpdateStatuses())
					UpdateTree();
			}
			catch (HttpRequestException ex)
			{
				Logger.LogError(ex);
				ThemedMessageBox.Show(text: "A network error has occurred, please check your connection", title: "Network error");
			}
		}

		/// <summary>
		/// Fetches and updates <see cref="UploadedReport"/>s in <see cref="UploadedReports"/>
		/// </summary>
		/// <returns><see langword="true"/> if any statuses have been updated and <see langword="false"/> otherwise</returns>
		private async Task<bool> UpdateStatuses()
		{
			bool result = false;
			try
			{
				StartWaitCursor();
				List<UploadedReport>? reportList = await IHKClient.GetIHKReports();
				if (reportList == null)
				{
					ThemedMessageBox.Show(text: "Unable to fetch statuses from IHK.", title: "Status update failed");
					return false;
				}

				foreach (UploadedReport report in reportList)
				{
					result |= UploadedReports.UpdateReportStatus(report.StartDate, report.Status, lfdnr: report.LfdNR);
				}

				if (result)
				{
					UpdateTree();
					ThemedMessageBox.Show(text: "Update complete.", title: "Update complete");
				}
				else
					ThemedMessageBox.Show(text: "Already up to date", title: "Update complete");
			}
			catch (HttpRequestException ex)
			{
				Logger.LogError(ex);
				ThemedMessageBox.Show(text: "A network error has occurred, please check your connection", title: "Network error");
			}
			EndWaitCursor();
			return result;
		}

		private async void MainForm_Load(object sender, EventArgs e)
		{
			if (ConfigHandler.AutoSyncStatusesWithIHK && ConfigHandler.UseIHK && await UpdateStatuses())
				UpdateTree();
		}

		/// <summary>
		/// Tries to hand in a report for <paramref name="lfdnr"/> and handles exceptions
		/// </summary>
		/// <param name="lfdnr">Number of report on IHK servers</param>
		/// <returns><see langword="true"/> if hand in was successful and <see langword="false"/> otherwise</returns>
		private async Task<bool> TryHandIn(int lfdnr)
		{
			try
			{
				return await IHKClient.HandInReport(lfdnr);
			}
			catch (HttpRequestException ex)
			{
				Logger.LogError(ex);
				ThemedMessageBox.Show(text: "A network error has occurred, please check your connection", title: "Network error");
			}
			catch (Exception ex)
			{
				ThemedMessageBox.Error(ex);
			}
			return false;
		}

		private async void miHandInSingle_Click(object sender, EventArgs e)
		{
			if (!UploadedReports.GetUploadedPaths(out _))
			{
				//Should never happen as menu item should be diabled
				ThemedMessageBox.Show(text: $"No reports in {ActivePath} have been uploaded yet", title: "Hand in failed");
				return;
			}
			if (!UploadedReports.GetUploadedReport(tvReports.SelectedNode.FullPath, out UploadedReport? report))
			{
				//Should never happen as menu item should be diabled
				ThemedMessageBox.Show(text: $"Report {FullSelectedPath} was not uploaded yet", title: "Hand in failed");
				return;
			}
			if (report.LfdNR is not int lfdnr)
			{
				ThemedMessageBox.Show(text: $"Lfdnr of {FullSelectedPath} could not be read", title: "Hand in failed");
				return;
			}
			if (report.Status != ReportNode.UploadStatuses.Uploaded && report.Status != ReportNode.UploadStatuses.Rejected)
			{
				ThemedMessageBox.Show(text: $"Can not update {FullSelectedPath} as it can not be changed on IHK server", title: "Can not update");
				return;
			}
			if (report.Status == ReportNode.UploadStatuses.Rejected && !report.WasUpdated && !report.WasEditedLocally)
			{
				ThemedMessageBox.Show(text: $"Can not update {FullSelectedPath} as it was rejected and not changed", title: "Can not update");
				return;
			}

			//Prevent unsaved changes from being left locally
			if (report.WasEditedLocally)
			{
				if (ThemedMessageBox.Show(text: $"Report {FullSelectedPath} has local changes, do you want to upload them now?", title: "Upload changes?", buttons: MessageBoxButtons.YesNo) != DialogResult.Yes)
				{
					ThemedMessageBox.Show(text: "Hand in was canceled", title: "Hand in canceled");
					return;
				}
				if (!CheckIfWordRunning())
				{
					ThemedMessageBox.Show(text: "Word has not started yet, hand in was canceled", title: "Hand in canceled");
					return;
				}

				StartWaitCursor();
				RestartWordIfNeeded();
				Word.Document doc;
				if (DocIsSamePathAsSelected())
					doc = Doc!;
				else
					doc = WordApp!.Documents.Open(FullSelectedPath);
				UploadResult? result = await TryUpdateReport(doc, lfdnr);
				if (!DocIsSamePathAsSelected())
					doc.Close();

				if (result == null)
				{
					ThemedMessageBox.Show(text: "Update of report failed, hand in was canceled", title: "Hand in canceled");
					EndWaitCursor();
					return;
				}

				if (!HandleUpdateResults(result.Result, report.StartDate))
				{
					EndWaitCursor();
					return;
				}
			}

			if (!await TryHandIn(lfdnr))
			{
				ThemedMessageBox.Show(text: $"Report {FullSelectedPath} could not be handed in", title: "Hand in failed");
				EndWaitCursor();
				return;
			}

			UploadedReports.UpdateReport(FullSelectedPath, status: ReportNode.UploadStatuses.HandedIn, wasEdited: false, wasUpdated: false);
			//When report is handed in, a new comment should be generated should it be rejected again
			UploadedReports.SetLastComment(FullSelectedPath, null);
			UpdateTree();
			EndWaitCursor();
			ThemedMessageBox.Show(text: "Hand in successful", title: "Report handed in");
		}

		/// <summary>
		/// Handles success and shows message boxes if not
		/// </summary>
		/// <param name="result"><see cref="CreateResults"/> of update process</param>
		/// <param name="startDate"><see cref="DateTime"/> of report start date</param>
		/// <returns><see langword="true"/> if <paramref name="result"/> is <see cref="CreateResults.Success"/> and <see langword="false"/> otherwise</returns>
		private bool HandleUpdateResults(CreateResults result, DateTime startDate)
		{
			switch (result)
			{
				case CreateResults.Success:
					UploadedReports.SetEdited(startDate, false);
					break;
				case CreateResults.Unauthorized:
					ThemedMessageBox.Show(text: "Login session has expired, try restarting report manager, hand in was skipped", title: "Login session expired");
					break;
				case CreateResults.CreationFailed:
					ThemedMessageBox.Show(text: "Report could not be loaded from IHK server, hand in was skipped", title: "Unable to edit report");
					break;
				case CreateResults.UploadFailed:
					ThemedMessageBox.Show(text: "Report could not be updated, hand in was skipped", title: "Handin failed");
					break;
				default:
					ThemedMessageBox.Show(text: "Unknown upload result, hand in was skipped", title: "Unknown result");
					break;
			}
			return result == CreateResults.Success;
		}

		/// <summary>
		/// Hands in a selection of reports to IHK
		/// </summary>
		/// <param name="progressForm"><see cref="EventProgressForm"/> to display progress on</param>
		/// <returns><see cref="Task"/> that runs the hand ins and can be <see langword="await"/>ed</returns>
		private Task HandInSelection(EventProgressForm progressForm)
		{
			return Task.Run(async () =>
			{
				bool shouldStop = false;
				progressForm.Stop += () => shouldStop = true;

				if (!UploadedReports.GetUploadedPaths(out _))
				{
					//Should never happen as menu item should be diabled
					ThemedMessageBox.Show(text: $"No reports in {ActivePath} have been uploaded yet", title: "Hand in failed");
					return;
				}

				FolderSelect fs = new FolderSelect(RootNode, node =>
				{
					bool isReport = (node is ReportNode reportNode);
					bool isUploaded = UploadedReports.GetUploadedReport(GetFullNodePath(node), out UploadedReport? report);
					bool statusIsUploaded = report?.Status == ReportNode.UploadStatuses.Uploaded;
					bool rejectedWasEdited = report?.Status == ReportNode.UploadStatuses.Rejected && (report.WasEditedLocally);
					bool emptyNonReportNode = !ReportUtils.IsNameValid(node.Text) && node.Nodes.Count == 0;
					bool wasUpdated = isUploaded && report!.WasUpdated && report?.Status == ReportNode.UploadStatuses.Rejected;
					return isReport && (!isUploaded || !statusIsUploaded) && !rejectedWasEdited && !wasUpdated || emptyNonReportNode;
				});
				if (fs.ShowDialog() != DialogResult.OK)
				{
					progressForm.Status = "File selection canceled";
					progressForm.Done();
					return;
				}
				ReportFinder.FindReports(fs.FilteredNode, out List<TreeNode> reports);

				bool updateSet = false;
				bool autoUpdateAllChanges = false;
				List<string> skippedPaths = new List<string>();
				List<string> failedPaths = new List<string>();

				bool needsUpdate = false;
				int handedIn = 0;
				foreach (TreeNode reportNode in reports)
				{
					string nodePath = GetFullNodePath(reportNode)!;
					string fullPath = Path.GetFullPath(Path.Combine(ActivePath, "..", nodePath));
					progressForm.Status = $"Handing in {fullPath}:";
					//Final fail save
					if (!UploadedReports.GetUploadedReport(nodePath, out UploadedReport? report))
					{
						ThemedMessageBox.Show(text: $"Report {fullPath} was not uploaded and could not be handed in as a result", title: "Hand in failed");
						progressForm.Status = "\t- skipped: Not uploaded";
						continue;
					}
					if (report.Status != ReportNode.UploadStatuses.Uploaded && report.Status != ReportNode.UploadStatuses.Rejected)
					{
						ThemedMessageBox.Show(text: $"Report {fullPath} could not be handed in due to its upload status", title: "Hand in failed");
						progressForm.Status = "\t- skipped: Can not be handed in";
						continue;
					}
					if (report.Status == ReportNode.UploadStatuses.Rejected && !report.WasEditedLocally && !report.WasUpdated)
					{
						ThemedMessageBox.Show(text: $"Report {fullPath} could not be handed in because it was rejected and not changed", title: "Hand in failed");
						progressForm.Status = "\t- skipped: Rejected and unchanged";
						continue;
					}
					if (report.LfdNR is not int lfdnr)
					{
						ThemedMessageBox.Show(text: $"Lfdnr of {fullPath} could not be read and report could not be handed in as a result", title: "Hand in failed");
						progressForm.Status = "\t- skipped: Unable to read lfdnr";
						continue;
					}

					//Prevent unsaved changes from being left locally
					if (report.WasEditedLocally)
					{
						RestartWordIfNeeded();
						//Check if changes should be uploaded or not
						if (!updateSet)
						{
							autoUpdateAllChanges = ThemedMessageBox.Show(text: "Should all local changes be uploaded to IHK?", title: "Automatically upload changes?", buttons: MessageBoxButtons.YesNo) == DialogResult.Yes;
							updateSet = true;
						}

						//Check if local changes should be uploaded
						if (!autoUpdateAllChanges)
						{
							skippedPaths.Add(fullPath);
							progressForm.Status = "\t- skipped: Unsaved changes";
							continue;
						}

						//Open document if it is not open
						bool shouldClose = false;
						Word.Document? doc = GetDocumentIfOpen(fullPath);
						if (doc == null)
						{
							doc = WordApp!.Documents.Open(fullPath);
							shouldClose = true;
						}

						progressForm.Status = "\t- Uploading changes";
						UploadResult? result = await TryUpdateReport(doc, lfdnr);

						//Close report if it was not opened before
						if (shouldClose)
							doc.Close();

						if (result == null)
						{
							ThemedMessageBox.Show(text: "Update of report failed, hand in was skipped", title: "Hand in skipped");
							progressForm.Status = "\t- skipped: Uploading changes failed";
							return;
						}

						if (result.Result != CreateResults.Success)
						{
							if (ThemedMessageBox.Show(text: $"Update of report {fullPath} failed, do you want to continue trying to hand in reports?", title: "Update failed", buttons: MessageBoxButtons.YesNo) != DialogResult.Yes)
							{
								progressForm.Status = "\t- skipped: Uploading changes failed";
								break;
							}
							else
							{
								progressForm.Status = "Abort hand in: Update failed";
								continue;
							}
						}

					}

					progressForm.Status = "\t- Handing in";
					if (!await TryHandIn(lfdnr))
					{
						failedPaths.Add(fullPath);
						if (ThemedMessageBox.Show(text: $"Hand in of report {fullPath} failed, do you want to continue trying to hand in reports?", title: "Hand in failed", buttons: MessageBoxButtons.YesNo) != DialogResult.Yes)
						{
							progressForm.Status = "\t- skipped: Hand in failed";
							break;
						}
						else
						{
							progressForm.Status = "Abort hand in: hand in failed";
							continue;
						}
					}
					needsUpdate = true;
					handedIn++;
					progressForm.Status = "\t- Updating status";
					UploadedReports.UpdateReport(nodePath, status: ReportNode.UploadStatuses.HandedIn, wasEdited: false, wasUpdated: false);
					//When report is handed in, a new comment should be generated should it be rejected again
					UploadedReports.SetLastComment(nodePath, null);

					if (shouldStop)
					{
						progressForm.Status = "Stopped";
						break;
					}

					await Task.Delay(ConfigHandler.IHKUploadDelay);
					progressForm.Status = "\t- Success";
				}
				progressForm.Status = "Done";
				progressForm.EventsText += "\nDone";

				if (reports.Count == 0)
				{
					ThemedMessageBox.Show(text: "All reports were already handed in", title: "Hand in complete");
					progressForm.Done();
					return;
				}
				if (needsUpdate)
					UpdateTree();
				string text = $"{handedIn} / {reports.Count} reports were successfully handed in";
				if (handedIn == reports.Count && skippedPaths.Count == 0)
					text = "All " + text;
				if (skippedPaths.Count > 0)
					text += "\nSkipped reports:";
				skippedPaths.ForEach(path => text += $"\n- {path}");
				if (failedPaths.Count > 0)
					text += "\nFailed hand ins:";
				failedPaths.ForEach(path => text += $"\n- {path}");
				progressForm.Done();
				ThemedMessageBox.Show(text: text, title: "Hand in complete");
			});
		}

		private async void HandInSelectionClick(object sender, EventArgs e)
		{
			if (ThemedMessageBox.Show(text: "Warning, this will hand in all reports selected in the next window in the order they appear!\nDo you want to proceed?", title: "Caution", buttons: MessageBoxButtons.YesNo) != DialogResult.Yes)
				return;
			EventProgressForm progressForm = new EventProgressForm("Hand in progress");
			progressForm.Show();
			StartWaitCursor();
			await HandInSelection(progressForm);
			EndWaitCursor();
		}

		/// <summary>
		/// Checks if a document at <paramref name="path"/> is open in <see cref="WordApp"/>
		/// </summary>
		/// <param name="path">Path of <see cref="Word.Document"/> to find</param>
		/// <returns><see cref="Word.Document"/> if document is opened in <see cref="WordApp"/> and <see langword="null"/> otherwise</returns>
		private Word.Document? GetDocumentIfOpen(string path)
		{
			if (!CheckIfWordRunning())
				return null;
			Word.Document? document = null;
			foreach (Word.Document doc in WordApp!.Documents)
			{
				if (doc.Path == path || doc.Path == Path.Combine(ActivePath, "..", path))
				{
					document = doc;
					break;
				}
			}
			return document;
		}

		/// <summary>
		/// Tries to edit the report with <paramref name="lfdnr"/> to have the contents of <paramref name="doc"/>, handles all exceptions
		/// </summary>
		/// <param name="doc"><see cref="Word.Document"/> to update report with</param>
		/// <param name="lfdnr">Lfdnr of report on IHK servers</param>
		/// <returns><see cref="UploadResult"/> of edit process or <see langword="null"/> if an error occurred</returns>
		private async Task<UploadResult?> TryUpdateReport(Word.Document doc, int lfdnr)
		{
			try
			{
				return await IHKClient.EditReport(doc, lfdnr);
			}
			catch (Exception ex)
			{
				switch (ex)
				{
					case InvalidDocumentException _:
						ThemedMessageBox.Show(text: "Invalid document, please upload manually", title: "Invalid document");
						return null;
					case HttpRequestException _:
						Logger.LogError(ex);
						ThemedMessageBox.Show(text: "A network error has occurred, please check your connection", title: "Network error");
						return null;
					default:
						ThemedMessageBox.Error(ex);
						return null;
				}
			}
		}

		private async void SendReportToIHK(object sender, EventArgs e)
		{
			if (!CheckNetwork())
			{
				ThemedMessageBox.Show(text: "No network connection", title: "No connection");
				return;
			}
			if (!UploadedReports.GetUploadedReport(tvReports.SelectedNode.FullPath, out UploadedReport? report))
			{
				ThemedMessageBox.Show(text: $"Could not find report {FullSelectedPath} in uploaded list, please add {tvReports.SelectedNode.FullPath} if it is uploaded", title: "Report not found");
				return;
			}
			if (report.Status != ReportNode.UploadStatuses.Uploaded && report.Status != ReportNode.UploadStatuses.Rejected)
			{
				ThemedMessageBox.Show(text: $"Can not update {FullSelectedPath} as it can not be changed on IHK server", title: "Can not update");
				return;
			}
			if (!report.WasEditedLocally)
				return;

			StartWaitCursor();
			RestartWordIfNeeded();
			Word.Document doc;
			//Prevent word app from showing when opening an already open document
			bool close = true;
			if (DocIsSamePathAsSelected())
			{
				close = false;
				doc = Doc!;
			}
			else
				doc = WordApp!.Documents.Open(FullSelectedPath);
			if (!FormFieldHandler.ValidFormFieldCount(doc))
			{
				ThemedMessageBox.Show(text: "Invalid document, please upload manually", title: "Invalid document");
				doc.Close(SaveChanges: false);
				EndWaitCursor();
				return;
			}
			if (report.LfdNR is not int lfdnr)
			{
				ThemedMessageBox.Show(text: $"Unable to load lfdnr from {FullSelectedPath}, verify that it is correct", title: "Unable to edit");
				EndWaitCursor();
				return;
			}

			UploadResult? result = await TryUpdateReport(doc, lfdnr);
			if (close)
				doc.Close();

			if (result == null || !HandleUpdateResults(result.Result, result.StartDate))
			{
				ThemedMessageBox.Show(text: "Update failed", title: "Update failed");
				EndWaitCursor();
				return;
			}

			UploadedReports.UpdateReport(tvReports.SelectedNode.FullPath, wasEdited: false, wasUpdated: true);
			UpdateTree();
			EndWaitCursor();
			ThemedMessageBox.Show(text: "Report was successfully updated", title: "Update complete");
		}

		private async void SendSelectionToIHK(object sender, EventArgs e)
		{
			if (!CheckNetwork())
			{
				ThemedMessageBox.Show(text: "No network connection", title: "No connection");
				return;
			}

			EventProgressForm progressForm = new EventProgressForm("Updating IHK reports");
			bool stop = false;
			progressForm.Stop += () => stop = true;
			progressForm.Show();
			progressForm.Status = "Selecting reports";

			FolderSelect select = new FolderSelect(RootNode, (node) =>
			{
				bool isReport = node is ReportNode;
				bool emptyNonReport = !ReportUtils.IsNameValid(node.Text) && node.Nodes.Count == 0;
				bool uploaded = UploadedReports.GetUploadedReport(GetFullNodePath(node), out UploadedReport? report);
				bool validStatus = uploaded && (report!.Status == ReportNode.UploadStatuses.Uploaded || report.Status == ReportNode.UploadStatuses.Rejected);
				bool wasEdited = report != null && report.WasEditedLocally;
				return isReport && (!wasEdited || !validStatus) || emptyNonReport;
			});

			if (select.ShowDialog() != DialogResult.OK)
			{
				progressForm.Status = "File selection was canceled";
				progressForm.Done();
				return;
			}

			ReportFinder.FindReports(select.FilteredNode, out List<TreeNode> reportNodes);
			if (reportNodes.Count == 0)
			{
				progressForm.Status = "No reports selected";
				progressForm.Done();
				return;
			}
			else
				progressForm.Status = $"{reportNodes.Count} {(reportNodes.Count == 1 ? "report" : "reports")} were selected";

			RestartWordIfNeeded();
			progressForm.Status = "Closing all reports";
			string? activePath = Doc?.FullName;
			List<string> openReports = CloseAllReports();

			Dictionary<string, string> skipped = new Dictionary<string, string>();
			foreach (TreeNode node in reportNodes)
			{
				if (stop)
				{
					progressForm.Status = "Stopping";
					break;
				}
				string fullPath = GetFullPath(node);
				progressForm.Status = $"Updating {fullPath}:";
				if (!UploadedReports.GetUploadedReport(fullPath, out UploadedReport? report))
				{
					progressForm.Status = "Skipped, not a report";
					skipped.Add(fullPath, "not a report");
					continue;
				}
				if (report.LfdNR is not int lfdnr)
				{
					progressForm.Status = "Skipped, lfdnr not found";
					skipped.Add(fullPath, "lfdnr not found");
					continue;
				}

				Word.Document doc = WordApp!.Documents.Open(fullPath);
				if (!FormFieldHandler.ValidFormFieldCount(doc))
				{
					progressForm.Status = "Skipped, invalid form field count";
					skipped.Add(fullPath, "invalid form field count");
					continue;
				}

				UploadResult? result = null;
				try
				{
					result = await IHKClient.EditReport(doc, lfdnr);
				}
				catch (Exception ex)
				{
					progressForm.Status = $"Skipped, Upload failed with {ex.GetType().Name}";
					skipped.Add(fullPath, $"{ex.GetType().Name}");
					doc.Close(SaveChanges: false);
					continue;
				}
				switch (result.Result)
				{
					case CreateResults.Success:
						progressForm.Status = "\t-Updated";
						UploadedReports.UpdateReport(fullPath, wasEdited: false, wasUpdated: true);
						break;
					default:
						progressForm.Status = "Skipped, Upload failed";
						skipped.Add(fullPath, $"Upload failed {result.Result}");
						break;
				}

				doc.Close(SaveChanges: false);

				await Task.Delay(ConfigHandler.IHKUploadDelay);
			}

			if (stop)
				progressForm.Status = "Stopped";

			progressForm.Status = "Opening closed reports";
			OpenAllDocuments(openReports, activePath);
			progressForm.Status = "Calculating statistics";

			string resultsMessage = $"Uploaded {reportNodes.Count - skipped.Count} / {reportNodes.Count} reports";
			if (skipped.Count > 0)
				resultsMessage += "\nSkipped:";
			foreach (KeyValuePair<string, string> kvp in skipped)
			{
				resultsMessage += $"\n- {kvp.Key}, {kvp.Value}";
			}

			progressForm.Status = "Done";
			progressForm.Done();
			ThemedMessageBox.Show(text: resultsMessage, title: "Update results");
			if (reportNodes.Count > 0)
				UpdateTree();
		}

		/// <summary>
		/// Checks
		/// </summary>
		/// <returns></returns>
		private bool CheckNetwork()
		{
			return System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
		}

		/// <summary>
		/// Checks all word document reports for wrong newlines
		/// </summary>
		/// <param name="progressForm"><see cref="EventProgressForm"/> to show progress on</param>
		/// <returns><see cref="List{}"/> of <see cref="ReportNode"/>s which contain wrong newlines</returns>
		private Task<List<ReportNode>> CheckWordDocuments(EventProgressForm progressForm)
		{
			return Task.Run(() =>
			{
				bool stop = false;
				void HandleStop()
				{
					stop = true;
				}
				progressForm.Stop += HandleStop;
				bool newLineError = false;
				List<ReportNode> result = new List<ReportNode>();
				FolderSelect fs = new FolderSelect(RootNode, node =>
				{
					return !ReportUtils.IsNameValid(node.Text) && node.Nodes.Count == 0;
				});
				if (fs.ShowDialog() != DialogResult.OK)
				{
					progressForm.Status = "File selection was canceled";
					progressForm.Done();
					return new List<ReportNode>();
				}

				progressForm.Status = "Indexing word reports:";
				ReportFinder.FindReports(fs.FilteredNode, out List<TreeNode> reportNodes);
				if (reportNodes.Count == 0)
				{
					progressForm.Status = "No reports selected";
					return new List<ReportNode>();
				}

				//Find all report containing odd new lines not previously replaced
				int rchecked = 0;
				List<string> checkFor = new List<string>() { "\r\n", "\r", "\n" };
				foreach (TreeNode node in reportNodes)
				{
					if (stop)
					{
						progressForm.Status = "Stopping";
						break;
					}
					bool errorFound = false;
					Word.Document doc = WordApp!.Documents.Open(GetFullPath(node));

					if (!FormFieldHandler.ValidFormFieldCount(doc))
					{
						progressForm.Status = $"-{doc.FullName}: Skipped, invalid number of form fields";
						continue;
					}

					checkFor.ForEach(newLine =>
					{
						errorFound |= FormFieldHandler.GetValueFromDoc<string>(Fields.Work, doc)?.Contains(newLine) == true;
						errorFound |= FormFieldHandler.GetValueFromDoc<string>(Fields.Seminars, doc)?.Contains(newLine) == true;
						errorFound |= FormFieldHandler.GetValueFromDoc<string>(Fields.School, doc)?.Contains(newLine) == true;
					});

					bool canBeUpdated = node is ReportNode report && UploadedReports.GetUploadStatus(GetFullNodePath(node), out var status) && (status == ReportNode.UploadStatuses.Uploaded || status == ReportNode.UploadStatuses.Rejected);
					progressForm.Status = $"-{doc.FullName}: {(errorFound ? "flagged" + (canBeUpdated ? "" : " but can not be updated on IHK") : "no misplaced new lines")}";
					doc.Close(SaveChanges: false);
					newLineError |= errorFound;
					//Flag report if error was found, node is report and report is uploaded or rejected
					if (errorFound && node is ReportNode reportNode && (reportNode.UploadStatus == ReportNode.UploadStatuses.Uploaded || reportNode.UploadStatus == ReportNode.UploadStatuses.Rejected))
						result.Add(reportNode);
					rchecked++;
				}

				progressForm.Status = $"Checked {rchecked} {(rchecked != 1 ? "reports" : "report")}";
				progressForm.Stop -= HandleStop;

				return result;
			});
		}

		/// <summary>
		/// Generates a full file path to a file represented by <paramref name="node"/>
		/// </summary>
		/// <param name="node"><see cref="TreeNode"/> to get full file path for</param>
		/// <returns>Full file path to file represented by <paramref name="node"/></returns>
		private string GetFullPath(TreeNode node)
		{
			return Path.GetFullPath(Path.Combine(ActivePath, "..", GetFullNodePath(node)!));
		}

		/// <summary>
		/// Checks the format of word reports and corrects them on both local and IHK reports
		/// </summary>
		/// <param name="sender"><see cref="Control"/> that raised the event</param>
		/// <param name="e"><see cref="EventArgs"/> of event</param>
		private async void CheckFormat(object sender, EventArgs e)
		{
			//Create progress form
			bool stop = false;
			EventProgressForm progressForm = new EventProgressForm("Checking formats");
			progressForm.Stop += () => stop = true;
			progressForm.Show();

			RestartWordIfNeeded();
			//Close documents
			progressForm.Status = "Closing open documents";
			string? activePath = Doc?.FullName;
			List<string> closedDocs = CloseAllReports();

			if (stop)
				return;

			//Check word documents
			List<ReportNode> formatErrors = new List<ReportNode>();
			try
			{
				formatErrors = await CheckWordDocuments(progressForm);
			}
			catch (Exception ex)
			{
				progressForm.Status = $"{ex.GetType().Name} occurred, stopping";
				stop = true;
			}

			if (stop)
			{
				progressForm.Status = "Stopped";
				progressForm.Done();
				return;
			}
			if (formatErrors.Count == 0)
			{
				progressForm.Status = "No formatting errors found";
				progressForm.Done();
				return;
			}

			int edited = 0;

			bool shouldEdit = ThemedMessageBox.Show(text: $"Correct formatting of {formatErrors.Count} {(formatErrors.Count == 1 ? "report" : "reports")}?",
				title: "Correct formatting?", buttons: MessageBoxButtons.YesNo) == DialogResult.Yes;
			Dictionary<string, string> skipped = new Dictionary<string, string>();
			if (shouldEdit)
			{
				foreach (ReportNode node in formatErrors)
				{
					Word.Document report = WordApp!.Documents.Open(GetFullPath(node));

					if (!FormFieldHandler.ValidFormFieldCount(report))
					{
						progressForm.Status = $"Skipped {report.FullName} as it has an invalid number of form fields";
						skipped.Add(report.FullName, "invalid number of form fields");
						continue;
					}

					progressForm.Status = $"Editing {report.FullName}";
					string work = ReportUtils.TransformTextToWord(FormFieldHandler.GetValueFromDoc<string>(Fields.Work, report));
					string seminars = ReportUtils.TransformTextToWord(FormFieldHandler.GetValueFromDoc<string>(Fields.Seminars, report));
					string school = ReportUtils.TransformTextToWord(FormFieldHandler.GetValueFromDoc<string>(Fields.School, report));

					FormFieldHandler.SetValueInDoc(Fields.Work, report, work);
					FormFieldHandler.SetValueInDoc(Fields.Seminars, report, seminars);
					FormFieldHandler.SetValueInDoc(Fields.School, report, school);

					report.Close(SaveChanges: true);

					UploadedReports.UpdateReport(GetFullNodePath(node), wasEdited: true);
					edited++;

					if (stop)
					{
						progressForm.Status = "Stopped";
						progressForm.Done();
						break;
					}
				}
			}

			progressForm.Status = "Opening closed reports";
			OpenAllDocuments(closedDocs, activePath);
			progressForm.Status = "Done";

			progressForm.Done();

			string resultsMessage = $"Found {formatErrors.Count} {(formatErrors.Count == 1 ? "error" : "errors")} and fixed {edited + skipped.Count} / {formatErrors.Count} of them.";
			if (skipped.Count > 0)
				resultsMessage += "\nSkipped:";
			foreach (KeyValuePair<string, string> kvp in skipped)
			{
				resultsMessage += $"\n\t- {kvp.Key}, {kvp.Value}";
			}
			ThemedMessageBox.Show(text: resultsMessage);

			if (formatErrors.Count > 0 && shouldEdit)
				UpdateTree();
		}

		private async void miRcShowComment_Click(object sender, EventArgs e)
		{
			if (!ReportUtils.IsNameValid(tvReports.SelectedNode.Text))
				return;
			if (!UploadedReports.GetUploadedReport(FullSelectedPath, out UploadedReport? report))
			{
				ThemedMessageBox.Show(text: "Report not uploaded", title: "Unable to fetch comment");
				return;
			}
			if (!report.LfdNR.HasValue || report.LfdNR < 0)
			{
				ThemedMessageBox.Show(text: "Unable to read lfdnr, check if it has been save correctly", title: "Unable to read lfdnr");
				return;
			}

			StartWaitCursor();
			CommentResult result = await GetCommentResult(report.LfdNR);
			HandleCommentResult(result);
			EndWaitCursor();
		}

		private async Task<CommentResult> GetCommentResult(int? lfdnr)
		{
			if (UploadedReports.GetLastComment(lfdnr) is string comment)
				return new CommentResult(CommentResult.ResultStatus.Success, comment: comment);
			CommentResult result = await IHKClient.GetCommentFromReport(lfdnr);
			if (result.Comment is string comment1)
			{
				UploadedReports.SetLastComment(lfdnr, comment1);
				UpdateTree();
			}
			return result;
		}

		private void HandleCommentResult(CommentResult result)
		{
			switch (result.UploadResult)
			{
				case CommentResult.ResultStatus.Success:
					if (string.IsNullOrEmpty(result.Comment))
						ThemedMessageBox.Info(text: "No comment found", title: "Comment");
					else
						ThemedMessageBox.Info(text: $"Comment:\n{result.Comment}", title: "Comment", allowMessageHighlight: true);
					break;
				case CommentResult.ResultStatus.Exception:
					ThemedMessageBox.Info(text: $"A(n) {result.Exception?.GetType().Name} occurred, the comment could not be fetched", title: result?.Exception?.GetType().Name ?? "Fetching comment failed");
					break;
				default:
					ThemedMessageBox.Info(text: $"Fetching comment failed: {result.UploadResult}", title: "Failed fetching comment");
					break;
			}
		}

		private async void DownloadIHKReports(object sender, EventArgs e)
		{
			EventProgressForm progressForm = new EventProgressForm("Download of reports from IHK");
			bool stop = false;
			progressForm.Stop += () => stop = true;
			progressForm.Show();
			progressForm.Status = "Fetching report numbers and statuses from IHK";

			//Handles stopping the execution
			void DoStop(string message = "Stopped")
			{
				progressForm.Status = message;
				progressForm.Done();
			}

			FolderSelect select = new FolderSelect(RootNode, (node) =>
			{
				return !ReportUtils.IsNameValid(node.Text) && node.Nodes.Count == 0;
			}, "Select reports to keep");
			if (select.ShowDialog() != DialogResult.OK)
			{
				if (ThemedMessageBox.Show(text: "You are about to replace all local reports with IHK reports, are you sure?", title: "Overwrite all reports?", MessageBoxButtons.YesNo) != DialogResult.Yes)
				{
					DoStop("Not all reports should be overwritten, please start process again.");
					return;
				}
			}
			ReportFinder.FindReports(select.FilteredNode, out List<TreeNode> reportNodes);

			RestartWordIfNeeded();
			//Index local reports
			string activePath = Doc?.FullName ?? "";
			List<string> openReports = CloseAllReports();

			List<string> skipped = new List<string>();
			progressForm.Status = $"Indexing {reportNodes.Count} report{(reportNodes.Count == 1 ? "" : "s")}";

			//Key is StartDate formatted as string
			Dictionary<string, UploadedReport?> indexedReports = new Dictionary<string, UploadedReport?>();
			foreach (TreeNode node in reportNodes)
			{
				if (stop)
				{
					DoStop();
					OpenAllDocuments(openReports, activePath);
					return;
				}

				string fullNodePath = GetFullPath(node);
				progressForm.Status = $"\t- {fullNodePath}:";
				string? startDate;
				if (ConfigHandler.ReportDownloadUseQuickInfo && QuickInfos.GetQuickInfo(fullNodePath, out QuickInfo? info))
					startDate = info.StartDate;
				else
				{
					Word.Document doc = WordApp!.Documents.Open(fullNodePath, ReadOnly: true);
					if (!FormFieldHandler.ValidFormFieldCount(doc))
					{
						progressForm.Status = "\t\t- Skipped, invalid form field count";
						skipped.Add(doc.FullName);
						continue;
					}
					startDate = FormFieldHandler.GetValueFromDoc<string>(Fields.StartDate, doc);
					doc.Close(SaveChanges: false);
				}


				if (UploadedReports.GetUploadedReport(fullNodePath, out UploadedReport? report))
					indexedReports.Add(report.StartDate.ToString(DateTimeUtils.DATEFORMAT), report);
				else if (string.IsNullOrWhiteSpace(startDate))
				{
					stop = true;
					break;
				}
				else if (!indexedReports.TryAdd(startDate, null))
				{
					stop = true;
					break;
				}

				progressForm.Status = "\t\t- Success";
			}
			progressForm.Status = "Finished indexing";

			//Ask for overwirte permission
			if (skipped.Count > 0)
			{
				string skippedMessage = $"Skipped indexing of {skipped.Count} report{(skipped.Count == 1 ? "" : "s")}, they could be overwritten when continuing, do you want to continue?\nSkipped:";
				skipped.ForEach(path => skippedMessage += path + "\n");
				if (ThemedMessageBox.Show(text: skippedMessage, title: "Continue with possible overwrites", buttons: MessageBoxButtons.YesNo, allowMessageHighlight: true) != DialogResult.Yes)
				{
					DoStop("Aborted");
					CloseAllReports();
					OpenAllDocuments(openReports, activePath);
					return;
				}
			}

			if (stop)
			{
				DoStop();
				OpenAllDocuments(openReports, activePath);
				return;
			}

			progressForm.Status = "Fetching reports";

			//Get report infos from IHK
			List<UploadedReport>? uploadedReports;
			try
			{
				uploadedReports = await IHKClient.GetIHKReports();
				progressForm.Status = "Done";
			}
			catch (Exception ex)
			{
				ThemedMessageBox.Error(ex, allowMessageHighlight: true);
				DoStop(ex.GetType().Name);
				OpenAllDocuments(openReports, activePath);
				return;
			}
			//Stop if desired
			if (stop)
			{
				DoStop();
				OpenAllDocuments(openReports, activePath);
				return;
			}

			if (uploadedReports == null)
			{
				string message = "Could not fetch uploaded reports from IHK servers.";
				DoStop(message);
				ThemedMessageBox.Show(text: message, title: "Failed to fetch reports");
				OpenAllDocuments(openReports, activePath);
				return;
			}

			//Reverse report list as reports are listed descending
			uploadedReports.Reverse();
			//Fetch report contents
			int reportNumber = 0;
			List<UploadedReport> skippedDownloads = new List<UploadedReport>();
			Dictionary<UploadedReport, (int Number, ReportContent Content)> downloadedContents = new Dictionary<UploadedReport, (int Number, ReportContent Content)>();
			foreach (UploadedReport report in uploadedReports)
			{
				reportNumber++;
				if (stop)
				{
					DoStop();
					OpenAllDocuments(openReports, activePath);
					return;
				}
				progressForm.Status = $"Downloading report from {report.StartDate:dd.MM.yyyy} to {report.StartDate.AddDays(5):dd.MM.yyyy}:";

				if (indexedReports.ContainsKey(report.StartDate.ToString("dd.MM.yyyy")))
				{
					progressForm.Status = "\t- Skipped, local will be kept";
					continue;
				}

				ReportContent content;
				try
				{
					GetReportResult result = await IHKClient.GetReportContent(report.LfdNR);
					switch (result.Result)
					{
						case GetReportResult.ResultStatuses.Success:
							content = result.ReportContent!;
							break;
						default:
							progressForm.Status = $"\t- Skipped: {result.Result}";
							skippedDownloads.Add(report);
							continue;
					}
				}
				catch (Exception ex)
				{
					progressForm.Status = $"\t- Download aborted: {ex.GetType().Name}";
					break;
				}

				progressForm.Status = "\t- Success";

				downloadedContents.Add(report, (reportNumber, content));
				await Task.Delay(ConfigHandler.IHKUploadDelay);
			}

			string newLatestPath = "";
			//Writing word documents
			progressForm.Status = "Writing reports";
			foreach (KeyValuePair<UploadedReport, (int Number, ReportContent Content)> kvp in downloadedContents)
			{
				if (stop)
				{
					DoStop();
					OpenAllDocuments(openReports, activePath);
					break;
				}

				Word.Document doc = WordApp!.Documents.Add(Template: ConfigHandler.TemplatePath);
				if (!FormFieldHandler.ValidFormFieldCount(doc))
				{
					DoStop("Aborted, template has an invalid field cound");
					OpenAllDocuments(openReports, activePath);
					break;
				}

				ReportTransformer.IHKToWord(doc, new Report(kvp.Value.Content) { ReportNr = reportNumber });

				string newReportName = NamingPatternResolver.ResolveNameWithExtension(kvp.Key.StartDate, kvp.Value.Number);
				string folder = Path.Combine(ActivePath, GetYearOfReport(kvp.Key.StartDate).ToString());
				string newPath = Path.Combine(folder, newReportName);
				newLatestPath = newPath;
				FitToPage(doc);
				if (!Directory.Exists(folder))
					Directory.CreateDirectory(folder);
				doc.SaveAs2(FileName: newPath);
				doc.Close();
				if (UploadedReports.GetUploadedReport(newPath, out UploadedReport? report))
					UploadedReports.UpdateRerport(newPath, kvp.Key);
				else
					UploadedReports.AddReport(newPath, kvp.Key);
				QuickInfos.AddOrUpdateQuickInfo(newPath, new QuickInfo(kvp.Key.StartDate.ToString(DateTimeUtils.DATEFORMAT), reportNumber));
				progressForm.Status = $"Saved {newPath}";
			}

			progressForm.Status = "Opening closed reports";
			OpenAllDocuments(openReports, activePath);

			progressForm.Status = "Updating config";
			var last = downloadedContents.LastOrDefault();
			if (downloadedContents.Count > 0 && ConfigHandler.ReportNumber < last.Value.Number)
			{
				ConfigHandler.LastReportCreateDate = last.Key.StartDate;
				ConfigHandler.ReportNumber = last.Value.Number;
				ConfigHandler.LastCreated = newLatestPath;
			}


			UpdateTree();

			progressForm.Status = "Finished";
			progressForm.Done();
		}

		private void ShowIHKReportStatusStatistics(object? sender, EventArgs e)
		{
			static void countStatuses(TreeNode root, Dictionary<ReportNode.UploadStatuses, int> input)
			{
				if (root is ReportNode reportNode)
					if (!input.TryAdd(reportNode.UploadStatus, 1))
						input[reportNode.UploadStatus]++;

				Dictionary<ReportNode.UploadStatuses, int> result = new();

				foreach (TreeNode child in root.Nodes)
				{
					countStatuses(child, input);
				}
			}

			var counts = new Dictionary<ReportNode.UploadStatuses, int>();
			countStatuses(RootNode, counts);

			StringBuilder message = new StringBuilder();
			message.AppendLine("Values are loaded from disc, IHK status count may vary.");
			foreach (var status in counts)
			{
				message.AppendLine($"{status.Key}: {status.Value}");
			}
			ThemedMessageBox.Show(text: message.ToString(), title: "IHK report status statistics");
		}

		private void miUpdateQuickInfos_Click(object sender, EventArgs e)
		{
			bool stop = false;
			EventProgressForm progress = new EventProgressForm("Fetching report information...");
			progress.Stop += () => stop = true;
			progress.Show();
			progress.Status = "Fetching report information:";

			RestartWordIfNeeded();
			progress.Status = "Closong opened reports";
			string activePath = Doc?.FullName ?? "";
			List<string> openReports = CloseAllReports();
			ReportFinder.FindReports(RootNode, out List<TreeNode> reports);
			foreach (var report in reports)
			{
				if (stop)
				{
					progress.Status = "Aborting";
					progress.Done();
					break;
				}
				Word.Document doc = WordApp!.Documents.Open(FileName: GetFullPath(report), ReadOnly: true);
				progress.Status = $"Reading {doc.FullName}: ";
				string? startDate = FormFieldHandler.GetValueFromDoc<string?>(Fields.StartDate, doc);
				int? reportNr = FormFieldHandler.GetValueFromDoc<int?>(Fields.Number, doc);
				progress.Status = $"\t-StartDate: {startDate}";
				progress.Status = $"\t-ReportNumber: {reportNr}";
				if (reportNr == null || startDate == null)
				{
					ThemedMessageBox.Info(text: $"Could not read StartDate or ReportNumber of report {doc.FullName}!", title: "Unable to read data");
					progress.Status = $"Skipped, could not read StartDare or ReportNumber";
				}
				else
					QuickInfos.AddOrUpdateQuickInfo(doc.FullName, new QuickInfo(startDate, reportNr.Value));
				doc.Close(SaveChanges: false);
			}
			progress.Status = "Opening closed reports";
			OpenAllDocuments(openReports, activePath);
			progress.Status = "Finished fetching report information";
			progress.Done();
		}
	}
}
