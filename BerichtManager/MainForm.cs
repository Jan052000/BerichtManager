using System;
using System.IO;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;
using BerichtManager.Config;
using BerichtManager.Forms;
using System.Globalization;
using System.Collections.Generic;
using System.Drawing;
using System.Collections;
using BerichtManager.ThemeManagement;
using System.Diagnostics;
using BerichtManager.HelperClasses;
using BerichtManager.WebUntisClient;
using System.Threading.Tasks;
using BerichtManager.OwnControls;
using BerichtManager.ReportChecking;
using System.Text;
using BerichtManager.ReportChecking.Discrepancies;
using BerichtManager.UploadChecking;
using System.Linq;
using BerichtManager.IHKClient;
using System.Net.Http;
using BerichtManager.IHKClient.Exceptions;
using BerichtManager.IHKClient.ReportContents;

namespace BerichtManager
{
	public partial class MainForm : Form
	{
		/// <summary>
		/// The currently open word document
		/// </summary>
		private Word.Document Doc { get; set; } = null;
		/// <summary>
		/// Global instance of Word
		/// </summary>
		private Word.Application WordApp { get; set; }
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
		private CultureInfo Culture { get; } = new CultureInfo("de-DE");
		private int TvReportsMaxWidth { get; set; } = 50;
		/// <summary>
		/// If a word document is opened
		/// </summary>
		private bool EditMode { get; set; } = false;
		/// <summary>
		/// If <see cref="rtbSchool"/> or <see cref="rtbWork"/> have been edited
		/// </summary>
		private bool WasEdited { get; set; } = false;
		private CustomNodeDrawer NodeDrawer { get; set; }

		/// <summary>
		/// Value if word has a visible window or not
		/// </summary>
		private bool WordVisible { get; set; } = false;

		/// <summary>
		/// Version number
		/// Major.Minor.Build.Revision
		/// </summary>
		public const string VersionNumber = "1.18.2";

		/// <summary>
		/// String to be printed
		/// </summary>
		private string VersionString { get; } = "v" + VersionNumber;

		/// <summary>
		/// Full path to report folder
		/// </summary>
		private string ActivePath { get; set; } = Path.GetFullPath(".\\..");

		/// <summary>
		/// Status if the word app is running
		/// </summary>
		private bool WordIsOpen { get; set; } = false;

		/// <summary>
		/// Factory for creating tasks that start word
		/// </summary>
		private TaskFactory WordTaskFactory { get; } = Task.Factory;

		/// <summary>
		/// Generates path to file from selected node
		/// </summary>
		private string FullSelectedPath
		{
			get => Path.GetFullPath(ActivePath + "\\..\\" + tvReports.SelectedNode.FullPath);
		}

		private ReportNode _OpenedReportNode { get; set; } = null;
		/// <summary>
		/// <see cref="ReportNode"/> of report opened for edit in <see cref="WordApp"/>
		/// </summary>
		private ReportNode OpenedReportNode
		{
			get => _OpenedReportNode;
			set
			{
				if (OpenedReportNode == value)
					return;
				if (OpenedReportNode != null)
				{
					OpenedReportNode.IsOpened = false;
					tvReports.Invalidate(OpenedReportNode.Bounds);
				}
				_OpenedReportNode = value;
				if (OpenedReportNode != null)
					OpenedReportNode.IsOpened = true;
			}
		}

		public MainForm()
		{
			InitializeComponent();
			ThemeSetter.SetThemes(this);
			ThemeSetter.SetThemes(toRightClickMenu);
			ThemeSetter.SetThemes(ttTips);
			NodeDrawer = new CustomNodeDrawer();
			foreach (Control control in this.Controls)
				control.KeyDown += DetectKeys;
			tvReports.TreeViewNodeSorter = new TreeNodeSorter();
			Info = new DirectoryInfo(ConfigHandler.ReportPath);
			ActivePath = ConfigHandler.ReportPath;
			UpdateTree();
			if (ConfigHandler.LastCreated == "")
			{
				miEditLatest.Enabled = false;
			}
			SetComponentPositions();
			UpdateTabStops(this, ConfigHandler.TabStops);
			if (File.Exists(ConfigHandler.PublishPath) && CompareVersionNumbers(VersionNumber, FileVersionInfo.GetVersionInfo(ConfigHandler.PublishPath).FileVersion) > 0)
				VersionString += "*";
			WordTaskFactory.StartNew(RestartWord);
		}

		/// <summary>
		/// Compares two version numbers
		/// </summary>
		/// <param name="version1">Version number 1</param>
		/// <param name="version2">Version number 2</param>
		/// <returns>0 if versions are equal, positive if version2 is greater and negative if version2 is smaller</returns>
		private int CompareVersionNumbers(string version1, string version2)
		{
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
				string openedNodePath = GetFullNodePath(OpenedReportNode);
				List<TreeNode> expanded = new List<TreeNode>();
				if (tvReports.Nodes.Count > 0)
					expanded = GetExpandedNodes(tvReports.Nodes[0]);

				tvReports.Nodes.Clear();
				TreeNode root = CreateDirectoryNode(Info);
				tvReports.Nodes.Add(root);

				expanded.ForEach(node =>
				{
					string path = GetFullNodePath(node);
					GetNodeFromPath(path)?.Expand();
				});

				FillStatuses(root);
				MarkEdited(root);
				if (openedNodePath is string)
					OpenedReportNode = GetNodeFromPath(openedNodePath) as ReportNode;
				tvReports.Sort();
			}

			if (InvokeRequired)
				BeginInvoke(new MethodInvoker(() =>
				{
					update();
				}));
			else
				update();
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
		private TreeNode CreateDirectoryNode(DirectoryInfo directoryInfo)
		{
			TreeNode directoryNode = new TreeNode(directoryInfo.Name);
			foreach (var directory in directoryInfo.GetDirectories())
				directoryNode.Nodes.Add(CreateDirectoryNode(directory));
			foreach (var file in directoryInfo.GetFiles())
				if (ReportUtils.IsNameValid(file.Name))
					directoryNode.Nodes.Add(new ReportNode(file.Name));
				else
					directoryNode.Nodes.Add(new TreeNode(file.Name));
			return directoryNode;
		}

		/// <summary>
		/// Fills the <see cref="ReportNode"/>s in <paramref name="root"/> with their <see cref="ReportNode.UploadStatuses"/>
		/// </summary>
		/// <param name="root">Root <see cref="TreeNode"/></param>
		private void FillStatuses(TreeNode root)
		{
			if (root is ReportNode report)
			{
				if (UploadedReports.GetUploadStatus(GetFullNodePath(root), out ReportNode.UploadStatuses status))
					report.UploadStatus = status;
			}
			foreach (TreeNode node in root.Nodes)
			{
				FillStatuses(node);
			}
		}

		/// <summary>
		/// Sets edit statuses of <see cref="ReportNode"/>s in <paramref name="root"/>
		/// </summary>
		/// <param name="root"></param>
		private void MarkEdited(TreeNode root)
		{
			if (root is ReportNode reportNode && UploadedReports.GetUploadedReport(reportNode.FullPath, out UploadedReport report))
				reportNode.WasEditedLocally = report.WasEditedLocally;
			foreach (TreeNode node in root.Nodes)
			{
				MarkEdited(node);
			}
		}

		/// <summary>
		/// Fills a WordInterop TextField with text
		/// </summary>
		/// <param name="app">The Word Application containing the documents with FormFields to fill</param>
		/// <param name="field">The FormField to fill with Text</param>
		/// <param name="text">The Text to Fill</param>
		private void FillText(Word.Application app, Word.FormField field, string text)
		{
			text = ReportUtils.TransformTextToWord(text);
			field.Select();
			for (int i = 1; i < 6; i++)
			{
				field.Range.Paragraphs.TabStops.Add(i * 14);
			}
			app.Selection.MoveLeft(Word.WdUnits.wdCharacter, 1);
			app.Selection.MoveRight(Word.WdUnits.wdCharacter, 1);
			if (text.Length > 254)
			{
				field.Result = " ";
				app.Selection.Text = text.Substring(0, 200);
				field.Result = field.Result.TrimEnd() + " ";
				app.Selection.MoveLeft(Word.WdUnits.wdCharacter, 1);
				app.Selection.TypeText(text.Substring(200));
				//Remove first space before text
				field.Select();
				app.Selection.MoveLeft(Word.WdUnits.wdCharacter, 1);
				app.Selection.MoveRight(Word.WdUnits.wdCharacter, 1);
				app.Selection.TypeBackspace();
			}
			else
			{
				field.Result = text;
			}
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
			try
			{
				doc.FitToPages();
			}
			catch
			{

			}
		}

		/// <summary>
		/// Fits <paramref name="doc"/> to pages
		/// </summary>
		/// <param name="doc"></param>
		private void FitToPage(Word.Document doc = null)
		{
			if (doc == null)
				doc = Doc;
			try
			{
				doc.FitToPages();
			}
			catch
			{

			}
		}

		/// <summary>
		/// Creates a new Word document from a given template for a given time.
		/// </summary>
		/// <param name="templatePath">The full path of the template to be used</param>
		/// <param name="baseDate">The date of the report to be created</param>
		/// <param name="app">The Wordapp that is used to create the document</param>
		/// <param name="vacation">If you missed reports due to vacation</param>
		/// <param name="reportDifference">The difference between the next report number and the one for the created report</param>
		/// <param name="isSingle">Used to tell the method that this is a regular create job</param>
		private void CreateDocument(string templatePath, DateTime baseDate, Word.Application app, bool vacation = false, int reportDifference = 0, bool isSingle = false)
		{
			Word.Document ldoc = null;
			bool ldocWasSaved = false;
			if (!File.Exists(templatePath))
			{
				ThemedMessageBox.Show(text: ConfigHandler.TemplatePath + " was not found was it moved or deleted?", title: "Template not found");
				return;
			}
			try
			{
				int weekOfYear = Culture.Calendar.GetWeekOfYear(baseDate, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
				ldoc = app.Documents.Add(Template: templatePath);

				if (ldoc.FormFields.Count != 10)
				{
					ThemedMessageBox.Show(text: "Invalid template");
					ldoc.Close(SaveChanges: false);
					ldoc = null;
					return;
				}

				EditForm form;
				//Fill name
				IEnumerator enumerator = ldoc.FormFields.GetEnumerator();
				enumerator.MoveNext();
				if (!string.IsNullOrEmpty(ConfigHandler.ReportUserName))
				{
					((Word.FormField)enumerator.Current).Result = ConfigHandler.ReportUserName;
				}
				else
				{
					form = new EditForm(title: "Enter your name", text: "Name Vorname");
					form.RefreshConfigs += RefreshConfig;
					if (form.ShowDialog() == DialogResult.OK)
					{
						ConfigHandler.ReportUserName = form.Result;
						ConfigHandler.SaveConfig();
						((Word.FormField)enumerator.Current).Result = ConfigHandler.ReportUserName;
					}
					else
					{
						ThemedMessageBox.Show(text: "Cannot proceed without a name!", title: "Name required!");
						return;
					}
					form.RefreshConfigs -= RefreshConfig;
				}
				enumerator.MoveNext();

				//Enter report nr.
				FillText(app, ((Word.FormField)enumerator.Current), (ConfigHandler.ReportNumber - reportDifference).ToString());

				//Enter week start and end
				DateTime today = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day);
				DateTime thisWeekStart = today.AddDays(-(int)baseDate.DayOfWeek + 1);
				DateTime thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
				enumerator.MoveNext();
				((Word.FormField)enumerator.Current).Result = thisWeekStart.ToString("dd.MM.yyyy");
				enumerator.MoveNext();
				if (ConfigHandler.EndWeekOnFriday)
				{
					((Word.FormField)enumerator.Current).Result = thisWeekEnd.AddDays(-2).ToString("dd.MM.yyyy");
				}
				else
				{
					((Word.FormField)enumerator.Current).Result = thisWeekEnd.ToString("dd.MM.yyyy");
				}

				//Enter Year
				enumerator.MoveNext();
				((Word.FormField)enumerator.Current).Result = today.Year.ToString();

				//Enter work field
				enumerator.MoveNext();
				if (vacation)
				{
					if (ConfigHandler.UseCustomPrefix)
						FillText(app, (Word.FormField)enumerator.Current, ConfigHandler.CustomPrefix + "Urlaub");
					else
						FillText(app, (Word.FormField)enumerator.Current, "-Urlaub");
				}
				else
				{
					form = new EditForm(title: "Betriebliche Tätigkeiten" + "(KW " + weekOfYear + ")", isCreate: true);
					form.RefreshConfigs += RefreshConfig;
					form.ShowDialog();
					form.RefreshConfigs -= RefreshConfig;
					switch (form.DialogResult)
					{
						case DialogResult.Abort:
							ldoc.Close(Word.WdSaveOptions.wdDoNotSaveChanges);
							ldoc = null;
							return;
						default:
							FillText(app, (Word.FormField)enumerator.Current, form.Result);
							break;
					}
				}

				//Enter work seminars
				enumerator.MoveNext();
				if (vacation)
				{
					if (ConfigHandler.UseCustomPrefix)
						FillText(app, (Word.FormField)enumerator.Current, ConfigHandler.CustomPrefix + "Urlaub");
					else
						FillText(app, (Word.FormField)enumerator.Current, "-Urlaub");
				}
				else
				{
					string text = "Keine";
					form = new EditForm(title: "Unterweisungen, betrieblicher Unterricht, sonstige Schulungen" + "(KW " + weekOfYear + ")", text: $"{(ConfigHandler.UseCustomPrefix ? ConfigHandler.CustomPrefix : "-")}{text}", isCreate: true);
					form.RefreshConfigs += RefreshConfig;
					form.ShowDialog();
					form.RefreshConfigs -= RefreshConfig;
					switch (form.DialogResult)
					{
						case DialogResult.Abort:
							ldoc.Close(Word.WdSaveOptions.wdDoNotSaveChanges);
							ldoc = null;
							return;
						default:
							FillText(app, (Word.FormField)enumerator.Current, form.Result);
							break;
					}
				}

				//Shool stuff
				enumerator.MoveNext();
				if (isSingle)
				{
					string classes = "";
					try
					{
						Client.GetClassesFromWebUntis().ForEach(c => classes += c);
					}
					catch (AggregateException e)
					{
						ThemedMessageBox.Show(text: "Unable to process classes from web\n(try to cancel the creation process and start again)");
						Logger.LogError(e);
					}
					form = new EditForm(title: "Berufsschule (Unterrichtsthemen)" + "(KW " + weekOfYear + ")", isCreate: true, text: classes);
				}
				else
				{
					form = new EditForm(title: "Berufsschule (Unterrichtsthemen)" + "(KW " + weekOfYear + ")", text: Client.GetHolidaysForDate(baseDate), isCreate: true);
				}
				form.RefreshConfigs += RefreshConfig;
				form.ShowDialog();
				form.RefreshConfigs -= RefreshConfig;
				switch (form.DialogResult)
				{
					case DialogResult.Abort:
						ldoc.Close(Word.WdSaveOptions.wdDoNotSaveChanges);
						ldoc = null;
						return;
					default:
						FillText(app, (Word.FormField)enumerator.Current, form.Result);
						break;
				}

				//Fridy of week
				enumerator.MoveNext();
				((Word.FormField)enumerator.Current).Result = thisWeekEnd.AddDays(-2).ToString("dd.MM.yyyy");

				//Sign date 2
				enumerator.MoveNext();
				((Word.FormField)enumerator.Current).Result = thisWeekEnd.AddDays(-2).ToString("dd.MM.yyyy");


				Directory.CreateDirectory(ActivePath + "\\" + today.Year);
				string name = NamingPatternResolver.ResolveName(weekOfYear.ToString(), ConfigHandler.ReportNumber.ToString());
				string path = ActivePath + "\\" + today.Year + "\\" + name + ".docx";
				FitToPage(ldoc);
				ldoc.SaveAs2(FileName: path);
				UpdateTree();

				ConfigHandler.ReportNumber++;
				ConfigHandler.LastReportWeekOfYear = weekOfYear;
				ConfigHandler.LastCreated = path;
				ConfigHandler.SaveConfig();
				miEditLatest.Enabled = true;
				ThemedMessageBox.Show(text: "Created Document at: " + path, title: "Document saved", allowMessageHighlight: true);
				ldocWasSaved = true;

				ldoc.Close();
				SaveOrExit();
				Doc = WordApp.Documents.Open(path);
				rtbWork.Text = Doc.FormFields[6].Result;
				rtbSchool.Text = Doc.FormFields[8].Result;
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
						ThemedMessageBox.Show(text: "Word closed unexpectedly and is restarting please wait while it restarts");
						RestartWord();
						if (ldocWasSaved)
						{
							ThemedMessageBox.Show(text: "Unable to automatically open report, Word was closed unexpectedly", title: "Loading was cancelled because word closed");
							return;
						}
						CreateDocument(templatePath, baseDate, WordApp, vacation: vacation, reportDifference: reportDifference, isSingle: isSingle);
						break;
					//case -2146822750:
					//	//Document already fit on page
					//	try
					//	{
					//		ldoc.Close(SaveChanges: true);
					//	}
					//	catch
					//	{

					//	}
					//	break;
					case -2146233088:
						ThemedMessageBox.Show(text: "Connection refused by remotehost");
						break;
					//Default file format in word is not compatible with .docx
					case -2146821994:
						ThemedMessageBox.Show(text: "Please change default document format in your Word app under File>Options>Save>DefaultFileFormat, then try again.", title: "Please change Word settingd");
						ldoc.Close(SaveChanges: false);
						break;
					default:
						Logger.LogError(ex);
						ThemedMessageBox.Show(text: ex.StackTrace);
						try
						{
							ldoc.Close(SaveChanges: false);
						}
						catch
						{

						}
						break;
				}
			}
		}

		private void btClose_Click(object sender, EventArgs e)
		{
			Close();
		}

		/// <summary>
		/// Used for detecting missing reports and initiating their creation
		/// </summary>
		/// <param name="vacation">Passed on to <see cref="CreateDocument(string, DateTime, Word.Application, bool, int, bool)"/> for if you were on vacation</param>
		private void CreateMissing(bool vacation = false)
		{
			DateTimeFormatInfo dfi = Culture.DateTimeFormat;
			int weekOfYear = Culture.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
			int reportNr = ConfigHandler.LastReportWeekOfYear;

			if (ConfigHandler.LastReportWeekOfYear < weekOfYear)
			{
				//Missing reports in current year
				DateTime today = DateTime.Today.AddDays(-(weekOfYear - reportNr) * 7);
				for (int i = 1; i < weekOfYear - reportNr; i++)
				{
					CreateDocument(ConfigHandler.TemplatePath, today.AddDays(i * 7), WordApp, vacation: vacation);
				}
			}
			else
			{
				//Missing missing reports over multiple years
				int nrOfWeeksLastYear = Culture.Calendar.GetWeekOfYear(new DateTime(DateTime.Today.Year - 1, 12, 31), dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
				int weekOfCurrentYear = Culture.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);

				int repeats = nrOfWeeksLastYear - reportNr + weekOfCurrentYear;

				DateTime today = DateTime.Today.AddDays(-(repeats * 7));

				//Generate reports for missing reports over 2 years
				for (int i = 1; i < repeats; i++)
				{
					CreateDocument(ConfigHandler.TemplatePath, today.AddDays(i * 7), WordApp, vacation: vacation);
				}
			}
		}

		private void btCreate_Click(object sender, EventArgs e)
		{
			if (!HasWordStarted()) return;

			//Check if report for this week was already created
			int currentWeek = Culture.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
			if (File.Exists(ActivePath + "\\" + DateTime.Today.Year + "\\WochenberichtKW" + currentWeek + ".docx") || File.Exists(ActivePath + "\\" + DateTime.Today.Year + "\\Gedruckt\\WochenberichtKW" + currentWeek + ".docx"))
			{
				ThemedMessageBox.Show(text: "A report has already been created for this week");
				return;
			}
			//Check if a report was created
			if (ConfigHandler.LastReportWeekOfYear > 0)
			{
				//Check if report for last week was created
				if (GetDistanceToToday() > 1)
				{
					if (ThemedMessageBox.Show(text: "You missed some reports were you on vacation?", title: "Vacation?", buttons: MessageBoxButtons.YesNo) == DialogResult.Yes)
					{
						CreateMissing(vacation: true);
					}
					else
					{
						if (ThemedMessageBox.Show(text: "Do you want to create empty reports then?", title: "Create?", buttons: MessageBoxButtons.YesNo) == DialogResult.Yes)
						{
							CreateMissing();
						}
					}
				}
			}

			CreateDocument(ConfigHandler.TemplatePath, baseDate: DateTime.Today, WordApp, isSingle: true);
		}

		/// <summary>
		/// Calculates the number of weeks since last report creation
		/// </summary>
		/// <returns>The number of weeks since last report creation</returns>
		private int GetDistanceToToday()
		{
			int lastReportKW = ConfigHandler.LastReportWeekOfYear;
			int todaysWeek = Culture.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
			//Both weeks are in the same year
			if (lastReportKW <= todaysWeek)
			{
				return todaysWeek - lastReportKW;
			}
			//Both weeks are in different years
			else
			{
				int lastWeekOfLastYear = Culture.Calendar.GetWeekOfYear(new DateTime(DateTime.Today.Year - 1, 12, 31), CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
				return lastWeekOfLastYear - lastReportKW + todaysWeek;
			}
		}

		private void btEdit_Click(object sender, EventArgs e)
		{
			if (!HasWordStarted()) return;

			if (DocIsSamePathAsSelected())
				return;
			SaveOrExit();
			if (ConfigHandler.UseLegacyEdit)
				Edit(ConfigHandler.LastCreated);
			else
				EditInTb(ConfigHandler.LastCreated);
		}

		private void btPrint_Click(object sender, EventArgs e)
		{
			if (!HasWordStarted()) return;

			if (tvReports.SelectedNode == null)
			{
				ThemedMessageBox.Show(text: "No report selected");
				return;
			}
			PrintDocument(FullSelectedPath);
		}

		private void btPrintAll_Click(object sender, EventArgs e)
		{
			if (!HasWordStarted()) return;

			if (!Directory.Exists(ActivePath)) return;
			Dictionary<string, List<string>> unPrintedFiles = new Dictionary<string, List<string>>();
			foreach (string dirName in Directory.GetDirectories(ActivePath))
			{
				foreach (string file in Directory.GetFiles(dirName))
				{
					if (Path.GetExtension(file) == ".docx")
					{
						if (!Directory.Exists(dirName + "\\Gedruckt"))
						{
							Directory.CreateDirectory(dirName + "\\Gedruckt");
						}
						if (!unPrintedFiles.ContainsKey(dirName))
						{
							unPrintedFiles.Add(dirName, new List<string>());
						}
						unPrintedFiles[dirName].Add(file);
					}
				}
			}
			if (unPrintedFiles.Count == 0)
			{
				ThemedMessageBox.Show(text: "No unprinted reports found");
				return;
			}

			PrintDialog printDialog = new PrintDialog();
			if (printDialog.ShowDialog() != DialogResult.OK) return;
			foreach (string key in unPrintedFiles.Keys)
			{
				if (unPrintedFiles[key].Contains(ConfigHandler.LastCreated))
				{
					if (ThemedMessageBox.Show(text: "Do you want to also print the last created report?\n(" + ConfigHandler.LastCreated + ")", title: "Print last created?", buttons: MessageBoxButtons.YesNo) == DialogResult.No)
					{
						unPrintedFiles[key].Remove(ConfigHandler.LastCreated);
					}
				}
			}
			foreach (string key in unPrintedFiles.Keys)
			{
				unPrintedFiles[key].ForEach((filePath) =>
				{

					try
					{
						bool isSameAsOpened;
						if (Doc != null)
							isSameAsOpened = filePath == Doc.Path + "\\" + Doc.Name;
						else isSameAsOpened = false;
						if (isSameAsOpened)
						{
							SaveOrExit();
							rtbSchool.Text = "";
							rtbWork.Text = "";
							WasEdited = false;
						};
						Word.Document document = WordApp.Documents.Open(FileName: filePath, ReadOnly: true);
						WordApp.Visible = WordVisible;
						document.PrintOut(Background: false);
						document.Close();
						File.Move(filePath, key + "\\Gedruckt\\" + Path.GetFileName(filePath));
						string oldRelPath = filePath.Split(new string[] { Path.GetFullPath(ActivePath + "\\..") + Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries).First();
						string newRelPath = (key + "\\Gedruckt\\" + Path.GetFileName(filePath)).Split(new string[] { Path.GetFullPath(ActivePath + "\\..") + Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries).First();
						UploadedReports.MoveReport(oldRelPath, newRelPath);
					}
					catch (Exception ex)
					{
						Logger.LogError(ex);
						ThemedMessageBox.Show(text: ex.StackTrace, title: "Error while printing" + filePath);
						Console.Write(ex.StackTrace);
					}
				});
			}
			UpdateTree();
		}

		/// <summary>
		/// Method for editing a Word document at a path relative to the working directory
		/// </summary>
		/// <param name="path">The path relative to the working directory</param>
		/// <param name="quickEditFieldNr">The number of the field to quick edit</param>
		/// <param name="quickEditTitle">Title of the editor window</param>
		public void Edit(string path, int quickEditFieldNr = -1, string quickEditTitle = "")
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
				if (!DocIsSamePathAsSelected())
				{
					SaveOrExit();
					Doc = WordApp.Documents.Open(path);
				}
				if (UploadedReports.GetUploadStatus(path, out ReportNode.UploadStatuses status) && (status == ReportNode.UploadStatuses.HandedIn || status == ReportNode.UploadStatuses.Accepted))
				{
					ThemedMessageBox.Show(text: "Cannot edit report that was handed in or accepted", title: "Unable to edit");
					return;
				}

				if (Doc.FormFields.Count != 10)
				{
					ThemedMessageBox.Show(text: "Invalid document (you will have to manually edit)");
					Doc.Close(SaveChanges: false);
					Doc = null;
					return;
				}

				bool markAsEdited = false;
				if (quickEditFieldNr > -1)
				{
					IEnumerator enumerator = Doc.FormFields.GetEnumerator();
					for (int i = 0; i < quickEditFieldNr; i++)
					{
						if (enumerator.MoveNext())
						{

						}
					}
					EditForm edit = new EditForm(title: quickEditTitle, text: ((Word.FormField)enumerator.Current).Result);
					edit.RefreshConfigs += RefreshConfig;
					switch (edit.ShowDialog())
					{
						case DialogResult.OK:
						case DialogResult.Ignore:
							markAsEdited |= edit.Result != (enumerator.Current as Word.FormField)?.Result;
							FillText(WordApp, (Word.FormField)enumerator.Current, edit.Result);
							break;
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
					if (selectEdit.SelectedItems.Count == 0)
					{
						SaveOrExit();
						return;
					}
					IEnumerator enumerator = Doc.FormFields.GetEnumerator();
					EditForm edit;
					foreach (EditState si in selectEdit.SelectedItems)
					{
						if (!enumerator.MoveNext() || !si.ShouldEdit)
							continue;
						edit = new EditForm(title: si.EditorTitle, text: ((Word.FormField)enumerator.Current).Result);
						edit.RefreshConfigs += RefreshConfig;
						switch (edit.ShowDialog())
						{
							case DialogResult.OK:
							case DialogResult.Ignore:
								markAsEdited |= edit.Result != (enumerator.Current as Word.FormField)?.Result;
								FillText(WordApp, (Word.FormField)enumerator.Current, edit.Result);
								break;
							default:
								break;
						}
						edit.RefreshConfigs -= RefreshConfig;
					}
				}
				FitToPage(Doc);
				Doc.Save();
				if (markAsEdited)
				{
					UploadedReports.SetEdited(path.Split(new string[] { Path.GetFullPath(ActivePath + "\\..") + Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries).First(), true);
					UpdateTree();
				}
				rtbWork.Text = Doc.FormFields[6].Result;
				rtbSchool.Text = Doc.FormFields[8].Result;
				EditMode = true;
				WasEdited = false;
				ThemedMessageBox.Show(text: "Saved changes", title: "Saved");
			}
			catch (Exception ex)
			{
				switch (ex.HResult)
				{
					case -2147023174:
						ThemedMessageBox.Show(text: "an unexpected problem occured this progam will now close!");
						break;
					case -2147467262:
					case -2146823679:
						ThemedMessageBox.Show(text: "Word closed unexpectedly and is restarting please try again shortly");
						RestartWord();
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
						Logger.LogError(ex);
						ThemedMessageBox.Show(text: ex.StackTrace);
						Console.Write(ex.StackTrace);
						break;
				}
			}
		}

		/// <summary>
		/// Retrieves a <see cref="TreeNode"/> from <see cref="tvReports"/> that lies at <paramref name="path"/>
		/// </summary>
		/// <param name="path">Path of report</param>
		/// <returns><see cref="TreeNode"/> that <paramref name="path"/> leads to</returns>
		private TreeNode GetNodeFromPath(string path)
		{
			List<string> segments = path.Replace("/", "\\").Split('\\').ToList();
			segments.RemoveRange(0, segments.IndexOf(tvReports.Nodes[0].Text) + 1);
			TreeNode result = tvReports.Nodes[0];

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
			if (path != tvReports.Nodes[0].Text && result == tvReports.Nodes[0])
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

				if (ConfigHandler.IHKAutoGetComment && UploadedReports.GetUploadedReport(path, out UploadedReport report) && report.LfdNR.HasValue && report.LfdNR > 0)
				{
					Task<CommentResult> commentTask = Task.Run(async () =>
					{
						try
						{
							return await IHKClient.GetCommentFromReport(report.LfdNR);
						}
						catch (Exception ex)
						{
							return new CommentResult(CommentResult.ResultStatus.Exception, exception: ex);
						}
					});
					CommentResult result = commentTask.Result;
					HandleCommentResult(result);
				}

				Doc = WordApp.Documents.Open(path);
				if (Doc.FormFields.Count != 10)
				{
					ThemedMessageBox.Show(text: "Invalid document (you will have to manually edit)");
					Doc.Close(SaveChanges: false);
					Doc = null;
					EditMode = false;
					WasEdited = false;
					return;
				}

				if (GetNodeFromPath(path) is ReportNode reportNode)
					OpenedReportNode = reportNode;

				rtbWork.Text = Doc.FormFields[6].Result;
				rtbSchool.Text = Doc.FormFields[8].Result;
				EditMode = true;
				WasEdited = false;
			}
			catch (Exception ex)
			{
				switch (ex.HResult)
				{
					case -2147023174:
						ThemedMessageBox.Show(text: "an unexpected problem occured this progam will now close!");
						break;
					case -2147467262:
					case -2146823679:
						ThemedMessageBox.Show(text: "Word closed unexpectedly and is restarting please try again shortly");
						RestartWord();
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
						Logger.LogError(ex);
						ThemedMessageBox.Show(text: ex.StackTrace, title: ex.GetType().Name);
						Console.Write(ex.StackTrace);
						break;
				}
			}
		}

		/// <summary>
		/// Saves active document
		/// </summary>
		private void SaveFromTb()
		{
			try
			{
				if (Doc == null || WordApp == null || !WasEdited)
					return;
				//Stop saving of accepted reports
				if (UploadedReports.GetUploadedReport(Doc.FullName, out UploadedReport report))
				{
					switch (report.Status)
					{
						case ReportNode.UploadStatuses.Accepted:
							rtbWork.Text = Doc.FormFields[6].Result;
							rtbSchool.Text = Doc.FormFields[8].Result;
							WasEdited = false;
							ThemedMessageBox.Show(text: "Can not change accepted report", title: "Save not possible");
							return;
						case ReportNode.UploadStatuses.HandedIn:
							rtbWork.Text = Doc.FormFields[6].Result;
							rtbSchool.Text = Doc.FormFields[8].Result;
							WasEdited = false;
							ThemedMessageBox.Show(text: "Can not change handed in report", title: "Save not possible");
							return;
					}
				}

				FillText(WordApp, Doc.FormFields[6], rtbWork.Text);
				FillText(WordApp, Doc.FormFields[8], rtbSchool.Text);
				FitToPage(Doc);
				Doc.Save();
				if (WasEdited)
				{
					UploadedReports.SetEdited(Path.Combine(Doc.Path, Doc.Name).Split(new string[] { Path.GetFullPath(ActivePath + "\\..") + Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries).First(), true);
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
						ThemedMessageBox.Show(text: "an unexpected problem occured this progam will now close!");
						break;
					case -2147467262:
					case -2146823679:
						ThemedMessageBox.Show(text: "Word closed unexpectedly and is restarting please try again shortly");
						RestartWord();
						break;
					//case -2146822750:
					//	//Document is one page already
					//	break;
					case -2146233088:
						ThemedMessageBox.Show(text: "Connection refused by remotehost");
						break;
					default:
						Logger.LogError(ex);
						ThemedMessageBox.Show(text: ex.StackTrace);
						Console.Write(ex.StackTrace);
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
			if (!WasEdited)
			{
				Doc.Close(SaveChanges: false);
				Doc = null;
				EditMode = false;
				return;
			}

			if (ThemedMessageBox.Show(text: "Save unsaved changes?", title: "Save?", buttons: MessageBoxButtons.YesNo) == DialogResult.Yes)
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
				return FullSelectedPath == Doc.Path + "\\" + Doc.Name;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Method for printing a document located at the path
		/// </summary>
		/// <param name="path">Path of document to print out</param>
		private void PrintDocument(string path)
		{
			if (Path.GetExtension(path) != ".docx")
			{
				ThemedMessageBox.Show(text: "You may only print Documents(*.docx) files");
				return;
			}
			DirectoryInfo printed = new DirectoryInfo(Path.GetDirectoryName(path));
			if (printed.Name == "Gedruckt")
			{
				if (ThemedMessageBox.Show(text: "Report was already printed do you want to print it again?", title: "Reprint?", buttons: MessageBoxButtons.YesNo) != DialogResult.Yes)
				{
					return;
				}
			}
			PrintDialog printDialog = new PrintDialog();
			if (printDialog.ShowDialog() != DialogResult.OK) return;
			if (!File.Exists(path)) return;
			if (!Directory.Exists(path.Substring(0, path.Length - Path.GetFileName(path).Length) + "\\Gedruckt"))
			{
				Directory.CreateDirectory(path.Substring(0, path.Length - Path.GetFileName(path).Length) + "\\Gedruckt");
			}
			try
			{
				bool isSameAsOpened;
				if (Doc != null)
					isSameAsOpened = path == Doc.Path + "\\" + Doc.Name;
				else isSameAsOpened = false;
				if (isSameAsOpened)
				{
					SaveOrExit();
					rtbSchool.Text = "";
					rtbWork.Text = "";
					WasEdited = false;
				};
				Word.Document document = WordApp.Documents.Open(path, ReadOnly: true);
				WordApp.Visible = WordVisible;
				document.PrintOut(Background: false);
				document.Close();
				if (printed.Name != "Gedruckt")
				{
					File.Move(path,
					path.Substring(0, path.Length - Path.GetFileName(path).Length) + "\\Gedruckt\\" + Path.GetFileName(path));
					string oldRelPath = path.Split(new string[] { Path.GetFullPath(ActivePath + "\\..") + Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries).First();
					string newRelPath = path.Substring(0, path.Length - Path.GetFileName(path).Length) + "\\Gedruckt\\" + Path.GetFileName(path).Split(new string[] { Path.GetFullPath(ActivePath + "\\..") + Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries).First();
					UploadedReports.MoveReport(oldRelPath, newRelPath);
					UpdateTree();
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
				ThemedMessageBox.Show(text: ex.StackTrace);
				Console.Write(ex.StackTrace);
			}
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
				ThemedMessageBox.Show(text: "You may only delete Word documents (*.docx) or their temporary files");
				return;
			}
			if (ThemedMessageBox.Show(text: $"Are you sure you want to {path}?", title: "Delete?", buttons: MessageBoxButtons.YesNo) != DialogResult.Yes)
				return;
			if (path == Doc?.Path + "\\" + Doc?.Name)
			{
				Doc.Close(SaveChanges: false);
				Doc = null;
				rtbSchool.Text = "";
				rtbWork.Text = "";
				EditMode = false;
				WasEdited = false;
			}
			if (path == ConfigHandler.LastCreated)
			{
				if (ConfigHandler.LastReportWeekOfYear == Culture.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday))
				{
					if (ConfigHandler.ReportNumber > 1)
						ConfigHandler.ReportNumber--;
					ConfigHandler.LastReportWeekOfYear = Culture.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday) - 1;
					ConfigHandler.SaveConfig();
				}
			}
			File.Delete(path);
			UpdateTree();
			ThemedMessageBox.Show(text: "File deleted successfully");
		}

		private void tvReports_DoubleClick(object sender, EventArgs e)
		{
			if ((File.GetAttributes(FullSelectedPath) & FileAttributes.Directory) == FileAttributes.Directory) return;
			if (!ReportUtils.IsNameValid(Path.GetFileName(FullSelectedPath)))
				return;
			if (!HasWordStarted()) return;
			if (tvReports.SelectedNode == null)
				return;
			if (DocIsSamePathAsSelected())
				return;
			SaveOrExit();
			if (ConfigHandler.UseLegacyEdit)
			{
				Edit(FullSelectedPath);
			}
			else
			{
				EditInTb(FullSelectedPath);
			}
		}

		private void tvReports_Click(object sender, EventArgs e)
		{
			if (((MouseEventArgs)e).Button == MouseButtons.Right)
			{
				tvReports.SelectedNode = tvReports.GetNodeAt(((MouseEventArgs)e).X, ((MouseEventArgs)e).Y);
			}
		}

		private void miDelete_Click(object sender, EventArgs e)
		{
			DeleteDocument(FullSelectedPath);
		}

		private void miEdit_Click(object sender, EventArgs e)
		{
			if (!HasWordStarted()) return;

			SaveOrExit();
			Edit(FullSelectedPath);
		}

		private void miPrint_Click(object sender, EventArgs e)
		{
			if (!HasWordStarted()) return;

			PrintDocument(FullSelectedPath);
		}

		private void miQuickEditWork_Click(object sender, EventArgs e)
		{
			if (!HasWordStarted()) return;

			SaveOrExit();
			Edit(FullSelectedPath, quickEditFieldNr: 6, quickEditTitle: "Edit work");
		}

		private void miQuickEditSchool_Click(object sender, EventArgs e)
		{
			if (!HasWordStarted()) return;

			SaveOrExit();
			Edit(FullSelectedPath, quickEditFieldNr: 8, quickEditTitle: "Edit school");
		}

		private void toRightClickMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			bool isInLogs = false;
			if (tvReports.SelectedNode.Parent != null)
			{
				if (tvReports.SelectedNode.Parent.Text == "Logs")
				{
					isInLogs = true;
				}
			}
			bool isNameValid = ReportUtils.IsNameValid(tvReports.SelectedNode.Text);
			bool isUploaded = UploadedReports.GetUploadedReport(tvReports.SelectedNode.FullPath, out UploadedReport report);
			bool uploaded = report?.Status == ReportNode.UploadStatuses.Uploaded;
			bool rejected = report?.Status == ReportNode.UploadStatuses.Rejected;
			bool wasEdited = report != null && (report?.WasEditedLocally).Value;

			miEdit.Enabled = !isInLogs && isNameValid;
			//miEdit.Visible = !isInLogs && tvReports.SelectedNode.Text.EndsWith(".docx") && !tvReports.SelectedNode.Text.StartsWith("~$");
			miPrint.Enabled = !isInLogs && isNameValid;
			//miPrint.Visible = !isInLogs && tvReports.SelectedNode.Text.EndsWith(".docx") && !tvReports.SelectedNode.Text.StartsWith("~$");
			miDelete.Enabled = isInLogs || tvReports.SelectedNode.Text.EndsWith(".docx") || tvReports.SelectedNode.Text.StartsWith("~$");
			//miDelete.Visible = isInLogs || tvReports.SelectedNode.Text.EndsWith(".docx") || tvReports.SelectedNode.Text.StartsWith("~$");
			miQuickEditOptions.Enabled = !isInLogs && isNameValid;
			//miQuickEditOptions.Visible = !isInLogs && tvReports.SelectedNode.Text.EndsWith(".docx") && !tvReports.SelectedNode.Text.StartsWith("~$");
			miUploadAsNext.Enabled = !isInLogs && isNameValid && !isUploaded;
			miHandInSingle.Enabled = isNameValid && isUploaded && (uploaded || rejected && wasEdited);
			miUpdateReport.Enabled = isNameValid && isUploaded && wasEdited && (uploaded || rejected);
			miRcShowComment.Enabled = isNameValid && isUploaded && report.LfdNR.HasValue;
			miDownloadReports.Enabled = miRcDownloadReports.Enabled = !string.IsNullOrEmpty(ConfigHandler.IHKUserName) && !string.IsNullOrEmpty(ConfigHandler.IHKPassword);
		}

		private void btOptions_Click(object sender, EventArgs e)
		{
			int tabStops = ConfigHandler.TabStops;
			OptionMenu optionMenu = new OptionMenu();
			optionMenu.ActiveThemeChanged += ActiveThemeChanged;
			optionMenu.ReportFolderChanged += ReportFolderChanged;
			optionMenu.TabStopsChanged += UpdateTabStops;
			optionMenu.FontSizeChanged += ChangeFontSize;
			optionMenu.IHKBaseAddressChanged += IHKBaseAddressChanged;
			optionMenu.ShowDialog();
			optionMenu.ActiveThemeChanged -= ActiveThemeChanged;
			optionMenu.ReportFolderChanged -= ReportFolderChanged;
			optionMenu.TabStopsChanged -= UpdateTabStops;
			optionMenu.FontSizeChanged -= ChangeFontSize;
			optionMenu.IHKBaseAddressChanged -= IHKBaseAddressChanged;
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

		private void ActiveThemeChanged(object sender, ITheme theme)
		{
			ThemeSetter.SetThemes(this);
			ThemeSetter.SetThemes(ttTips);
			tvReports.Refresh();
		}

		private void DetectKeys(object sender, KeyEventArgs e)
		{
			if (e.Control && e.KeyCode == Keys.S && EditMode)
			{
				SaveFromTb();
			}
			if (e.Control && e.Shift && e.KeyCode == Keys.V)
				PasteOnlyText(sender, e);
		}

		/// <summary>
		/// Strips the formatting from text in <see cref="Clipboard"/> and pastes it into <paramref name="sender"/>
		/// </summary>
		/// <param name="sender"><see cref="RichTextBox"/> to paste <see cref="Clipboard"/> content to</param>
		/// <param name="e"><see cref="KeyEventArgs"/> of KeyDown event</param>
		private void PasteOnlyText(object sender, KeyEventArgs e)
		{
			if (!(sender is RichTextBox rtb))
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
				WordApp.Visible = miWordVisible.Checked;
			WordVisible = miWordVisible.Checked;
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

		private void tvReports_DrawNode(object sender, DrawTreeNodeEventArgs e)
		{
			NodeDrawer.DrawNode(e);
		}

		private void menuStrip1_Paint(object sender, PaintEventArgs e)
		{
			int versionNumberWidth = (int)e.Graphics.MeasureString(VersionNumber, menuStrip1.Font).Width / 2;
			Control control = sender as Control;
			TextRenderer.DrawText(e.Graphics, VersionString, menuStrip1.Font, new Point(control.Location.X + control.Width / 2 - versionNumberWidth, control.Location.Y + control.Padding.Top + 2), control.ForeColor);
		}

		private void tvReports_KeyUp(object sender, KeyEventArgs e)
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
					if (!HasWordStarted()) return;
					if (DocIsSamePathAsSelected()) return;
					SaveOrExit();
					EditInTb(FullSelectedPath);
					break;
				case Keys.Delete:
					DeleteDocument(FullSelectedPath);
					break;
				case Keys.Escape:
					if (Doc == null || !EditMode)
					{
						ThemedMessageBox.Show(text: "No opened report to close", title: "Could not close");
						return;
					}
					CloseOpenDocument();
					break;
			}
		}

		private void MiRefresh_Click(object sender, EventArgs e)
		{
			UpdateTree();
		}

		/// <summary>
		/// Checks if Word has started
		/// </summary>
		/// <returns>Has Word finished starting</returns>
		private bool HasWordStarted()
		{
			if (!CheckIfWordRunning())
			{
				ThemedMessageBox.Show(text: "Word is still starting, please try again", title: "Please try again");
				return false;
			}
			return true;
		}

		private void miRevealInExplorer_Click(object sender, EventArgs e)
		{
			if (Directory.Exists(ActivePath))
				Process.Start(ActivePath);
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
		private void RestartWord()
		{
			if (CheckIfWordRunning())
				return;
			WordApp = new Word.Application();
			((Word.ApplicationEvents4_Event)WordApp).Quit += OnWordClose;
			WordIsOpen = true;
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
			rtbSchool.Text = "";
			rtbWork.Text = "";
			WasEdited = false;
			EditMode = false;
		}

		/// <summary>
		/// Checks all selected reports for discrepancies and handles output
		/// </summary>
		/// <param name="check">Kind of check to execute</param>
		private void CheckDiscrepancies(ReportChecker.CheckKinds check)
		{
			if (!HasWordStarted())
				return;
			FolderSelect select = new FolderSelect(tvReports.Nodes[0]);
			if (select.ShowDialog() != DialogResult.OK)
				return;
			if (select.FilteredNode == null)
			{
				ThemedMessageBox.Show(text: "No file or folder was selected, check was canceled", title: "No selection was made");
				return;
			}
			string activePath = Doc?.FullName ?? "";
			List<string> openReports = CloseAllReports();
			ReportChecker checker = new ReportChecker(WordApp);
			Cursor = Cursors.WaitCursor;
			if (!checker.Check(select.FilteredNode, out List<IReportDiscrepancy> discrepancies, check: check))
			{
				Cursor = Cursors.Default;
				OpenAllDocuments(openReports, activePath);
				return;
			}
			Cursor = Cursors.Default;
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
		/// Opens a list of <see cref="Word.Document"/>s from <paramref name="paths"/> and opens <paramref name="activePath"/> in text boxes
		/// </summary>
		/// <param name="paths">Paths of previously opened reports</param>
		/// <param name="activePath">Path to open in text box edit</param>
		private void OpenAllDocuments(List<string> paths, string activePath)
		{
			paths.ForEach(path =>
			{
				if (string.IsNullOrWhiteSpace(path))
					return;
				if (path == activePath)
					EditInTb(path);
				else
					WordApp.Documents.Open(FileName: path);
			});
		}

		/// <summary>
		/// Closes all open reports
		/// </summary>
		/// <returns><see cref="List{T}"/> of paths from previously opened reports</returns>
		private List<string> CloseAllReports()
		{
			List<string> result = new List<string>();
			foreach (Word.Document doc in WordApp.Documents)
			{
				result.Add(doc.FullName);
				if (doc == Doc)
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
		/// <returns><see cref="UploadResult"/> of creation or <see langword="null"/> if an error occurred</returns>
		private async Task<UploadResult> TryUploadReportToIHK(Word.Document doc)
		{
			try
			{
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
				ThemedMessageBox.Show(text: $"An unexpected exception has occurred, a complete log has been saved to\n{Logger.LogError(ex)}:\n{ex.StackTrace}", title: ex.GetType().Name);
				return null;
			}
		}

		private async void miUploadAsNext_Click(object sender, EventArgs e)
		{
			if (!HasWordStarted())
				return;
			//should not happen as menu item should be disabled
			if (UploadedReports.GetUploadedReport(tvReports.SelectedNode.FullPath, out _))
			{
				ThemedMessageBox.Show(text: "Report was already uploaded", title: "Report already uploaded");
				return;
			}
			Word.Document doc;
			bool close = true;
			if (DocIsSamePathAsSelected())
			{
				close = false;
				doc = Doc;
			}
			else
				doc = WordApp.Documents.Open(FullSelectedPath);
			if (doc.FormFields.Count < 10)
			{
				ThemedMessageBox.Show(text: "Invalid document, please upload manually", title: "Invalid document");
				doc.Close(SaveChanges: false);
				return;
			}
			UploadResult result = await TryUploadReportToIHK(doc);
			if (result == null)
				return;
			switch (result.Result)
			{
				case CreateResults.Success:
					ThemedMessageBox.Show(text: "Report uploaded successfully", title: "Upload successful");
					UploadedReports.AddReport(tvReports.SelectedNode.FullPath, new UploadedReport(result.StartDate, lfdNr: result.LfdNR));
					break;
				case CreateResults.Unauthorized:
					ThemedMessageBox.Show(text: "Session has expired please try again", title: "Session expired");
					break;
				default:
					ThemedMessageBox.Show(text: "Unable to upload report, please try again in a bit", title: "Unable to upload");
					break;
			}
			if (close)
				doc.Close(SaveChanges: false);
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
				progressForm.Stop += () => shouldStop = true;

				if (!UploadedReports.GetUploadedPaths(out List<string> files))
				{
					//Should never happen as menu item should be diabled
					ThemedMessageBox.Show(text: $"No reports in {ActivePath} have been uploaded yet", title: "Hand in failed");
					return;
				}

				FolderSelect fs = new FolderSelect(tvReports.Nodes[0], node =>
				{
					return files.Contains(GetFullNodePath(node)) || (!ReportUtils.IsNameValid(node.Text) && node.Nodes.Count == 0);
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

				foreach (TreeNode report in reports)
				{
					string nodePath = GetFullNodePath(report);
					string path = Path.Combine(ActivePath, "..", nodePath);

					progressForm.Status = $"Uploading {nodePath}";

					Word.Document doc = WordApp.Documents.Open(path);
					if (doc.FormFields.Count < 10)
					{
						progressForm.Status = $"Uploading aborted: Invalid dcument";
						ThemedMessageBox.Show(text: $"Invalid document, please add missing form fields to {path}.\nUploading is stopped", title: "Invalid document");
						doc.Close(SaveChanges: false);
						OpenAllDocuments(openReports, activePath);
						progressForm.Done();
						return;
					}
					UploadResult result = await TryUploadReportToIHK(doc);
					if (result == null)
					{
						progressForm.Status = $"Uploading aborted: upload failed";
						ThemedMessageBox.Show(text: $"Upload of {path} failed, upload was canceled!", title: "Upload failed");
						doc.Close(SaveChanges: false);
						OpenAllDocuments(openReports, activePath);
						progressForm.Done();
						return;
					}
					switch (result.Result)
					{
						case CreateResults.Success:
							UploadedReports.AddReport(nodePath, new UploadedReport(result.StartDate, lfdNr: result.LfdNR));
							break;
						case CreateResults.Unauthorized:
							ThemedMessageBox.Show(text: "Session has expired please try again", title: "Session expired");
							doc.Close(SaveChanges: false);
							OpenAllDocuments(openReports, activePath);
							progressForm.Status = $"Abort: Unauthorized";
							progressForm.Done();
							return;
						default:
							ThemedMessageBox.Show(text: $"Upload of {path} failed, upload was canceled!", title: "Upload failed");
							doc.Close(SaveChanges: false);
							OpenAllDocuments(openReports, activePath);
							progressForm.Status = $"Abort: Upload failed";
							progressForm.Done();
							return;
					}
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
				progressForm.Status = $"Done";
				progressForm.Done();
				string text = "";
				if (reports.Count == 1)
					text = "Upload of report was succesful";
				else
					text = $"Upload of all {reports.Count} reports was successful";
				ThemedMessageBox.Show(text: text, title: "Upload finished");
				UpdateTree();
			});
		}

		private async void UploadSelectionClick(object sender, EventArgs e)
		{
			if (!HasWordStarted())
				return;
			if (ThemedMessageBox.Show(text: "Warning, this will upload all reports selected in the next window in the order they appear!\nDo you want to proceed?", title: "Caution", buttons: MessageBoxButtons.YesNo) != DialogResult.Yes)
				return;
			EventProgressForm progressForm = new EventProgressForm("Upload progress");
			progressForm.Show();
			await UploadSelection(progressForm);
		}

		/// <summary>
		/// Generates the full path of the <paramref name="node"/>
		/// </summary>
		/// <param name="node"><see cref="TreeNode"/> to get path for</param>
		/// <param name="separator">String to separate path elements with</param>
		/// <returns>Full path separated by <paramref name="separator"/> or <see langword="null"/> if <paramref name="node"/> is <see langword="null"/></returns>
		private string GetFullNodePath(TreeNode node, string separator = "\\")
		{
			TreeNode current = node;
			if (node == null)
				return null;

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
				List<UploadedReport> reportList = await IHKClient.GetIHKReports();
				reportList.ForEach(report => result |= UploadedReports.UpdateReportStatus(report.StartDate, report.Status, lfdnr: report.LfdNR));
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
			return result;
		}

		private async void MainForm_Load(object sender, EventArgs e)
		{
			if (ConfigHandler.AutoSyncStatusesWithIHK && await UpdateStatuses())
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
				ThemedMessageBox.Show(text: $"An unexpected exception has occurred, a complete log has been saved to\n{Logger.LogError(ex)}:\n{ex.StackTrace}", title: ex.GetType().Name);
			}
			return false;
		}

		private async void miHandInSingle_Click(object sender, EventArgs e)
		{
			if (!UploadedReports.GetUploadedPaths(out List<string> paths))
			{
				//Should never happen as menu item should be diabled
				ThemedMessageBox.Show(text: $"No reports in {ActivePath} have been uploaded yet", title: "Hand in failed");
				return;
			}
			if (!UploadedReports.GetUploadedReport(tvReports.SelectedNode.FullPath, out UploadedReport report))
			{
				//Should never happen as menu item should be diabled
				ThemedMessageBox.Show(text: $"Report {FullSelectedPath} was not uploaded yet", title: "Hand in failed");
				return;
			}
			if (!(report.LfdNR is int lfdnr))
			{
				ThemedMessageBox.Show(text: $"Lfdnr of {FullSelectedPath} could not be read", title: "Hand in failed");
				return;
			}
			if (report.Status != ReportNode.UploadStatuses.Uploaded && report.Status != ReportNode.UploadStatuses.Rejected)
			{
				ThemedMessageBox.Show(text: $"Can not update {FullSelectedPath} as it can not be changed on IHK server", title: "Can not update");
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

				Word.Document doc;
				if (DocIsSamePathAsSelected())
					doc = Doc;
				else
					doc = WordApp.Documents.Open(FullSelectedPath);
				UploadResult result = await TryUpdateReport(doc, lfdnr);
				if (!DocIsSamePathAsSelected())
					doc.Close();

				if (result == null)
				{
					ThemedMessageBox.Show(text: "Update of report failed, hand in was canceled", title: "Hand in canceled");
					return;
				}

				if (!HandleUpdateResults(result.Result, report.StartDate))
					return;
			}

			if (!await TryHandIn(lfdnr))
			{
				ThemedMessageBox.Show(text: $"Report {FullSelectedPath} could not be handed in", title: "Hand in failed");
				return;
			}

			UploadedReports.UpdateReportStatus(report.StartDate, ReportNode.UploadStatuses.HandedIn, lfdnr);
			UpdateTree();
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

				if (!UploadedReports.GetUploadedPaths(out List<string> files))
				{
					//Should never happen as menu item should be diabled
					ThemedMessageBox.Show(text: $"No reports in {ActivePath} have been uploaded yet", title: "Hand in failed");
					return;
				}

				FolderSelect fs = new FolderSelect(tvReports.Nodes[0], node =>
				{
					bool isReport = (node is ReportNode reportNode);
					bool isUploaded = UploadedReports.GetUploadedReport(GetFullNodePath(node), out UploadedReport report);
					bool statusIsUploaded = report?.Status == ReportNode.UploadStatuses.Uploaded;
					bool rejectedWasEdited = report?.Status == ReportNode.UploadStatuses.Rejected && (report?.WasEditedLocally).Value;
					bool emptyNonReportNode = !ReportUtils.IsNameValid(node.Text) && node.Nodes.Count == 0;
					return isReport && (!isUploaded || !statusIsUploaded) && !rejectedWasEdited || emptyNonReportNode;
				});
				if (fs.ShowDialog() != DialogResult.OK)
				{
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
					string nodePath = GetFullNodePath(reportNode);
					string fullPath = Path.GetFullPath(Path.Combine(ActivePath, "..", nodePath));
					progressForm.Status = $"Handing in {fullPath}:";
					//Final fail save
					if (!UploadedReports.GetUploadedReport(nodePath, out UploadedReport report))
					{
						ThemedMessageBox.Show(text: $"Report {fullPath} was not uploaded and could not be handed in as a result", title: "Hand in failed");
						progressForm.Status = "\t- skipped: Not uploaded";
						continue;
					}
					if (report.Status != ReportNode.UploadStatuses.Uploaded)
					{
						ThemedMessageBox.Show(text: $"Report {fullPath} could not be handed in due to its upload status", title: "Hand in failed");
						progressForm.Status = "\t- skipped: Can not be handed in";
						continue;
					}
					if (!(report.LfdNR is int lfdnr))
					{
						ThemedMessageBox.Show(text: $"Lfdnr of {fullPath} could not be read and report could not be handed in as a result", title: "Hand in failed");
						progressForm.Status = "\t- skipped: Unable to read lfdnr";
						continue;
					}
					//Prevent unsaved changes from being left locally
					if (report.WasEditedLocally)
					{
						if (!CheckIfWordRunning())
						{
							ThemedMessageBox.Show(text: "Word has not started yet, hand in was canceled", title: "Hand in canceled");
							return;
						}
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
						Word.Document doc = GetDocumentIfOpen(fullPath);
						if (doc == null)
						{
							doc = WordApp.Documents.Open(fullPath);
							shouldClose = true;
						}

						progressForm.Status = "\t- Uploading changes";
						UploadResult result = await TryUpdateReport(doc, lfdnr);

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
					UploadedReports.UpdateReportStatus(report.StartDate, ReportNode.UploadStatuses.HandedIn, report.LfdNR);

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
			await HandInSelection(progressForm);
		}

		/// <summary>
		/// Checks if a document at <paramref name="path"/> is open in <see cref="WordApp"/>
		/// </summary>
		/// <param name="path">Path of <see cref="Word.Document"/> to find</param>
		/// <returns><see cref="Word.Document"/> if document is opened in <see cref="WordApp"/> and <see langword="null"/> otherwise</returns>
		private Word.Document GetDocumentIfOpen(string path)
		{
			if (!HasWordStarted())
				return null;
			Word.Document document = null;
			foreach (Word.Document doc in WordApp.Documents)
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
		private async Task<UploadResult> TryUpdateReport(Word.Document doc, int lfdnr)
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
						ThemedMessageBox.Show(text: $"An unexpected exception has occurred, a complete log has been saved to\n{Logger.LogError(ex)}:\n{ex.StackTrace}", title: ex.GetType().Name);
						return null;
				}
			}
		}

		private async void SendReportToIHK(object sender, EventArgs e)
		{
			if (!HasWordStarted())
				return;
			if (!CheckNetwork())
			{
				ThemedMessageBox.Show(text: "No network connection", title: "No connection");
				return;
			}
			if (!UploadedReports.GetUploadedReport(tvReports.SelectedNode.FullPath, out UploadedReport report))
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
			Word.Document doc;
			//Prevent word app from showing when opening an already open document
			bool close = true;
			if (DocIsSamePathAsSelected())
			{
				close = false;
				doc = Doc;
			}
			else
				doc = WordApp.Documents.Open(FullSelectedPath);
			if (doc.FormFields.Count < 10)
			{
				ThemedMessageBox.Show(text: "Invalid document, please upload manually", title: "Invalid document");
				doc.Close(SaveChanges: false);
				return;
			}
			if (!(report.LfdNR is int lfdnr))
			{
				ThemedMessageBox.Show(text: $"Unable to load lfdnr from {FullSelectedPath}, verify that it is correct", title: "Unable to edit");
				return;
			}

			UploadResult result = await TryUpdateReport(doc, lfdnr);
			if (result == null)
				return;
			if (close)
				doc.Close();

			UploadedReports.SetEdited(tvReports.SelectedNode.FullPath, false);
			UpdateTree();
			ThemedMessageBox.Show(text: "Report was successfully updated", title: "Update complete");
		}

		private async void SendSelectionToIHK(object sender, EventArgs e)
		{
			if (!HasWordStarted())
				return;
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

			FolderSelect select = new FolderSelect(tvReports.Nodes[0], (node) =>
			{
				bool isReport = node is ReportNode;
				bool emptyNonReport = !ReportUtils.IsNameValid(node.Text) && node.Nodes.Count == 0;
				bool uploaded = UploadedReports.GetUploadedReport(GetFullNodePath(node), out UploadedReport report);
				bool validStatus = uploaded && (report.Status == ReportNode.UploadStatuses.Uploaded || report.Status == ReportNode.UploadStatuses.Rejected);
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

			progressForm.Status = "Closing all reports";
			string activePath = Doc?.FullName;
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
				if (!UploadedReports.GetUploadedReport(fullPath, out UploadedReport report))
				{
					progressForm.Status = "Skipped, not a report";
					skipped.Add(fullPath, "not a report");
					continue;
				}
				if (!(report.LfdNR is int lfdnr))
				{
					progressForm.Status = "Skipped, lfdnr not found";
					skipped.Add(fullPath, "lfdnr not found");
					continue;
				}

				Word.Document doc = WordApp.Documents.Open(fullPath);
				if (doc.FormFields.Count < 10)
				{
					progressForm.Status = "Skipped, invalid form field count";
					skipped.Add(fullPath, "invalid form field count");
					continue;
				}

				UploadResult result = null;
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
						UploadedReports.SetEdited(fullPath, false);
						break;
					default:
						progressForm.Status = "Skipped, Upload failed";
						skipped.Add(fullPath, $"Upload failed {result.Result}");
						break;
				}

				doc.Close(SaveChanges: false);
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
				FolderSelect fs = new FolderSelect(tvReports.Nodes[0], node =>
				{
					return !ReportUtils.IsNameValid(node.Text) && node.Nodes.Count == 0;
				});
				if (fs.ShowDialog() != DialogResult.OK)
				{
					progressForm.Status = "File selection was canceled";
					progressForm.Done();
					return new List<ReportNode>();
				}

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
					Word.Document doc = WordApp.Documents.Open(GetFullPath(node));

					if (doc.FormFields.Count < 10)
					{
						progressForm.Status = $"-{doc.FullName}: Skipped, invalid number of form fields";
						continue;
					}

					checkFor.ForEach(newLine =>
					{
						errorFound |= doc.FormFields[6].Result.Contains(newLine);
						errorFound |= doc.FormFields[7].Result.Contains(newLine);
						errorFound |= doc.FormFields[8].Result.Contains(newLine);
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
			return Path.GetFullPath(Path.Combine(ActivePath, "..", GetFullNodePath(node)));
		}

		/// <summary>
		/// Checks the format of word reports and corrects them on both local and IHK reports
		/// </summary>
		/// <param name="sender"><see cref="Control"/> that raised the event</param>
		/// <param name="e"><see cref="EventArgs"/> of event</param>
		private async void CheckFormat(object sender, EventArgs e)
		{
			if (!HasWordStarted())
				return;

			//Create progress form
			bool stop = false;
			EventProgressForm progressForm = new EventProgressForm("Checking formats");
			progressForm.Stop += () => stop = true;
			progressForm.Show();
			progressForm.Status = "Indexing word reports:";


			//Close documents
			progressForm.Status = "Closing open documents";
			string activePath = Doc?.FullName;
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
					Word.Document report = WordApp.Documents.Open(GetFullPath(node));

					if (report.FormFields.Count < 10)
					{
						progressForm.Status = $"Skipped {report.FullName} as it has an invalid number of form fields";
						skipped.Add(report.FullName, "invalid number of form fields");
						continue;
					}

					progressForm.Status = $"Editing {report.FullName}";
					string work = ReportUtils.TransformTextToWord(report.FormFields[6].Result);
					string seminars = ReportUtils.TransformTextToWord(report.FormFields[7].Result);
					string school = ReportUtils.TransformTextToWord(report.FormFields[8].Result);

					FillText(WordApp, report.FormFields[6], work);
					FillText(WordApp, report.FormFields[7], seminars);
					FillText(WordApp, report.FormFields[8], school);

					report.Close(SaveChanges: true);

					UploadedReports.UpdateReport(GetFullNodePath(node), wasEdited: true);
					edited++;
				}
			}

			progressForm.Status = "Opening closed reports";
			OpenAllDocuments(closedDocs, activePath);
			progressForm.Status = "Done";

			progressForm.Done();

			string resultsMessage = $"Found {formatErrors} {(formatErrors.Count == 1 ? "error" : "errors")} and fixed {formatErrors.Count - skipped.Count} / {formatErrors.Count} of them.";
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
			if (!UploadedReports.GetUploadedReport(FullSelectedPath, out UploadedReport report))
			{
				ThemedMessageBox.Show(text: "Report not uploaded", title: "Unable to fetch comment");
				return;
			}
			if (!report.LfdNR.HasValue || report.LfdNR < 0)
			{
				ThemedMessageBox.Show(text: "Unable to read lfdnr, check if it has been save correctly", title: "Unable to read lfdnr");
				return;
			}

			CommentResult result = await IHKClient.GetCommentFromReport(report.LfdNR);
			HandleCommentResult(result);
		}

		private void HandleCommentResult(CommentResult result)
		{
			switch (result.UploadResult)
			{
				case CommentResult.ResultStatus.Success:
					if (string.IsNullOrEmpty(result.Comment))
						ThemedMessageBox.Info(text: "No comment found", title: "Comment");
					else
						ThemedMessageBox.Info(text: $"Comment:\n{result.Comment}", title: "Comment");
					break;
				case CommentResult.ResultStatus.Exception:
					ThemedMessageBox.Info(text: $"A(n) {result.Exception?.GetType().Name} occurred, the comment could not be fetched", title: result.Exception?.GetType().Name);
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

			FolderSelect select = new FolderSelect(tvReports.Nodes[0], (node) =>
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

			//Index local reports
			string activePath = Doc?.FullName ?? "";
			List<string> openReports = CloseAllReports();

			List<string> skipped = new List<string>();
			progressForm.Status = $"Indexing {reportNodes.Count} report{(reportNodes.Count == 1 ? "" : "s")}";

			Dictionary<string, UploadedReport> indexedReports = new Dictionary<string, UploadedReport>();
			foreach (TreeNode node in reportNodes)
			{
				if (stop)
				{
					DoStop();
					OpenAllDocuments(openReports, activePath);
					return;
				}

				string fullNodePath = GetFullPath(node);
				Word.Document doc = WordApp.Documents.Open(fullNodePath, ReadOnly: true);

				progressForm.Status = $"\t- {doc.FullName}:";

				if (doc.FormFields.Count < 10)
				{
					progressForm.Status = "\t\t- Skipped, invalid form field count";
					skipped.Add(doc.FullName);
					continue;
				}

				if (UploadedReports.GetUploadedReport(fullNodePath, out UploadedReport report))
					indexedReports.Add(report.StartDate.ToString("dd.MM.yyyy"), report);
				else
					indexedReports.Add(doc.FormFields[2].Result, null);
				doc.Close(SaveChanges: false);

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

			//Get report infos from IHK
			List<UploadedReport> uploadedReports;
			try
			{
				uploadedReports = await IHKClient.GetIHKReports();
			}
			catch (Exception ex)
			{
				ThemedMessageBox.Show(text: $"A(n) {ex.GetType().Name} occurred, report info could not be fetched");
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
							content = result.ReportContent;
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

				Word.Document doc = WordApp.Documents.Add(Template: ConfigHandler.TemplatePath);
				if (doc.FormFields.Count < 10)
				{
					DoStop("Aborted, template has an invalid field cound");
					OpenAllDocuments(openReports, activePath);
					break;
				}

				ReportTransformer.IHKToWord(WordApp, doc, new Report(kvp.Value.Content) { ReportNr = reportNumber });

				string newReportName = NamingPatternResolver.ResolveNameWithExtension(kvp.Key.StartDate, kvp.Value.Number);
				string folder = Path.Combine(ActivePath, kvp.Key.StartDate.Year.ToString());
				string newPath = Path.Combine(folder, newReportName);
				newLatestPath = newPath;
				FitToPage(doc);
				if (!Directory.Exists(folder))
					Directory.CreateDirectory(folder);
				doc.SaveAs2(FileName: newPath);
				doc.Close();
				UploadedReports.AddReport(newPath, kvp.Key);
				progressForm.Status = $"Saved {newPath}";
			}

			progressForm.Status = "Opening closed reports";
			OpenAllDocuments(openReports, activePath);

			progressForm.Status = "Updating config";
			var last = downloadedContents.LastOrDefault();
			if (downloadedContents.Count > 0 && ConfigHandler.ReportNumber < last.Value.Number)
			{
				ConfigHandler.LastReportWeekOfYear = Culture.Calendar.GetWeekOfYear(last.Key.StartDate, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
				ConfigHandler.ReportNumber = last.Value.Number;
				ConfigHandler.LastCreated = newLatestPath;
			}


			UpdateTree();

			progressForm.Status = "Finished";
			progressForm.Done();
		}
	}
}
