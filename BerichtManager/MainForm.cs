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
using BerichtManager.UploadChecking;
using System.Linq;
using BerichtManager.IHKClient;
using System.Net.Http;
using BerichtManager.IHKClient.Exceptions;

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
		private ITheme ActiveTheme { get => ThemeManager.Instance.ActiveTheme; }

		/// <summary>
		/// Value if word has a visible window or not
		/// </summary>
		private bool WordVisible { get; set; } = false;

		/// <summary>
		/// Version number
		/// Major.Minor.Build.Revision
		/// </summary>
		public const string VersionNumber = "1.15.2";

		/// <summary>
		/// String to be printed
		/// </summary>
		private string VersionString { get; } = "v" + VersionNumber;

		/// <summary>
		/// Full path to report folder
		/// </summary>
		private string ActivePath { get; set; } = Path.GetFullPath(".\\..");

		/// <summary>
		/// Status if the word app has finished loading
		/// </summary>
		private bool WordInitialized { get; set; } = false;

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

		public MainForm()
		{
			InitializeComponent();
			ThemeSetter.SetThemes(this, ActiveTheme);
			ThemeSetter.SetThemes(toRightClickMenu, ActiveTheme);
			NodeDrawer = new CustomNodeDrawer();
			foreach (Control control in this.Controls)
				control.KeyDown += DetectKeys;
			this.Icon = Icon.ExtractAssociatedIcon(Path.GetFullPath(".\\BerichtManager.exe"));
			tvReports.TreeViewNodeSorter = new TreeNodeSorter();
			Info = new DirectoryInfo(ConfigHandler.ReportPath());
			ActivePath = ConfigHandler.ReportPath();
			UpdateTree();
			if (ConfigHandler.LastCreated() == "")
			{
				miEditLatest.Enabled = false;
			}
			SetComponentPositions();
			UpdateTabStops(this, ConfigHandler.TabStops());
			if (File.Exists(ConfigHandler.PublishPath()) && CompareVersionNumbers(VersionNumber, FileVersionInfo.GetVersionInfo(ConfigHandler.PublishPath()).FileVersion) > 0)
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
					if (splitv1[i] != splitv2[i])
					{
						if (int.TryParse(splitv1[i], out int v1) && int.TryParse(splitv2[i], out int v2))
						{
							return v2 - v1;
						}
					}
				}
			}
			else
			{
				for (int i = 0; i < Math.Min(splitv1.Length, splitv2.Length); i++)
				{
					if (splitv1[i] != splitv2[i])
					{
						if (int.TryParse(splitv1[i], out int v1) && int.TryParse(splitv2[i], out int v2))
						{
							return v2 - v1;
						}
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
			rtbSchool.Font = new Font(rtbSchool.Font.FontFamily, ConfigHandler.EditorFontSize());
			rtbWork.Font = new Font(rtbWork.Font.FontFamily, ConfigHandler.EditorFontSize());
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
				tvReports.Nodes.Clear();
				TreeNode root = CreateDirectoryNode(Info);
				tvReports.Nodes.Add(root);
				FillStatuses(root);
				MarkEdited(root);
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
				if (ReportFinder.IsReportNameValid(file.Name))
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
				app.Selection.Text = text.Replace("\n", "\v").Substring(0, 200);
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
				field.Result = text.Replace("\n", "\v");
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
			if (app.Selection.Font.Name != ConfigHandler.EditorFont())
			{
				app.Selection.Font.Name = ConfigHandler.EditorFont();
				ThemedMessageBox.Show(ActiveTheme, "Changed report Font to: " + ConfigHandler.EditorFont(), "Font changed!");
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
				ThemedMessageBox.Show(ActiveTheme, ConfigHandler.TemplatePath() + " was not found was it moved or deleted?", "Template not found");
				return;
			}
			try
			{
				int weekOfYear = Culture.Calendar.GetWeekOfYear(baseDate, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
				ldoc = app.Documents.Add(Template: templatePath);

				if (ldoc.FormFields.Count != 10)
				{
					ThemedMessageBox.Show(ActiveTheme, "Invalid template");
					ldoc.Close(SaveChanges: false);
					ldoc = null;
					return;
				}

				EditForm form;
				//Fill name
				IEnumerator enumerator = ldoc.FormFields.GetEnumerator();
				enumerator.MoveNext();
				if (!string.IsNullOrEmpty(ConfigHandler.ReportUserName()))
				{
					((Word.FormField)enumerator.Current).Result = ConfigHandler.ReportUserName();
				}
				else
				{
					form = new EditForm(title: "Enter your name", text: "Name Vorname");
					form.RefreshConfigs += RefreshConfig;
					if (form.ShowDialog() == DialogResult.OK)
					{
						ConfigHandler.ReportUserName(form.Result);
						ConfigHandler.SaveConfig();
						((Word.FormField)enumerator.Current).Result = ConfigHandler.ReportUserName();
					}
					else
					{
						ThemedMessageBox.Show(ActiveTheme, "Cannot proceed without a name!", "Name required!");
						return;
					}
					form.RefreshConfigs -= RefreshConfig;
				}
				enumerator.MoveNext();

				//Enter report nr.
				FillText(app, ((Word.FormField)enumerator.Current), (ConfigHandler.ReportNumber() - reportDifference).ToString());

				//Enter week start and end
				DateTime today = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day);
				DateTime thisWeekStart = today.AddDays(-(int)baseDate.DayOfWeek + 1);
				DateTime thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
				enumerator.MoveNext();
				((Word.FormField)enumerator.Current).Result = thisWeekStart.ToString("dd.MM.yyyy");
				enumerator.MoveNext();
				if (ConfigHandler.EndWeekOnFriday())
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
					if (ConfigHandler.UseUserPrefix())
						FillText(app, (Word.FormField)enumerator.Current, ConfigHandler.CustomPrefix() + "Urlaub");
					else
						FillText(app, (Word.FormField)enumerator.Current, "-Urlaub");
				}
				else
				{
					form = new EditForm(title: "Betriebliche Tätigkeiten" + "(KW " + weekOfYear + ")", isCreate: true);
					form.RefreshConfigs += RefreshConfig;
					form.ShowDialog();
					form.RefreshConfigs -= RefreshConfig;
					if (form.DialogResult == DialogResult.OK)
					{
						FillText(app, (Word.FormField)enumerator.Current, form.Result);
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							ldoc.Close(Word.WdSaveOptions.wdDoNotSaveChanges);
							ldoc = null;
							return;
						}
						else
						{
							((Word.FormField)enumerator.Current).Result = "-Keine-";
						}
					}
				}

				//Enter work seminars
				enumerator.MoveNext();
				if (vacation)
				{
					if (ConfigHandler.UseUserPrefix())
						FillText(app, (Word.FormField)enumerator.Current, ConfigHandler.CustomPrefix() + "Urlaub");
					else
						FillText(app, (Word.FormField)enumerator.Current, "-Urlaub");
				}
				else
				{
					form = new EditForm(title: "Unterweisungen, betrieblicher Unterricht, sonstige Schulungen" + "(KW " + weekOfYear + ")", text: "-Keine-", isCreate: true);
					form.RefreshConfigs += RefreshConfig;
					form.ShowDialog();
					form.RefreshConfigs -= RefreshConfig;
					if (form.DialogResult == DialogResult.OK)
					{
						FillText(app, (Word.FormField)enumerator.Current, form.Result);
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							ldoc.Close(Word.WdSaveOptions.wdDoNotSaveChanges);
							ldoc = null;
							return;
						}
						else
						{
							((Word.FormField)enumerator.Current).Result = "-Keine-";
						}
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
						ThemedMessageBox.Show(ActiveTheme, "Unable to process classes from web\n(try to cancel the creation process and start again)");
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
				if (form.DialogResult == DialogResult.OK)
				{
					FillText(app, (Word.FormField)enumerator.Current, form.Result);
				}
				else
				{
					if (form.DialogResult == DialogResult.Abort)
					{
						ldoc.Close(Word.WdSaveOptions.wdDoNotSaveChanges);
						ldoc = null;
						return;
					}
					else
					{
						((Word.FormField)enumerator.Current).Result = "-Keine-";
					}
				}

				//Fridy of week
				enumerator.MoveNext();
				((Word.FormField)enumerator.Current).Result = thisWeekEnd.AddDays(-2).ToString("dd.MM.yyyy");

				//Sign date 2
				enumerator.MoveNext();
				((Word.FormField)enumerator.Current).Result = thisWeekEnd.AddDays(-2).ToString("dd.MM.yyyy");


				Directory.CreateDirectory(ActivePath + "\\" + today.Year);
				string name = ConfigHandler.NamingPattern().Replace(NamingPatternResolver.CalendarWeek, weekOfYear.ToString()).Replace(NamingPatternResolver.ReportNumber, ConfigHandler.ReportNumber().ToString());
				string path = ActivePath + "\\" + today.Year + "\\" + name + ".docx";
				FitToPage(ldoc);
				ldoc.SaveAs2(FileName: path);
				UpdateTree();

				ConfigHandler.ReportNumber(ConfigHandler.ReportNumber() + 1);
				ConfigHandler.LastReportKW(Culture.Calendar.GetWeekOfYear(today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday));
				ConfigHandler.LastCreated(path);
				ConfigHandler.SaveConfig();
				miEditLatest.Enabled = true;
				ThemedMessageBox.Show(ActiveTheme, "Created Document at: " + path);
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
						ThemedMessageBox.Show(ActiveTheme, "Word closed unexpectedly and is restarting please wait while it restarts");
						RestartWord();
						if (ldocWasSaved)
						{
							ThemedMessageBox.Show(ActiveTheme, text: "Unable to automatically open report, Word was closed unexpectedly", "Loading was cancelled because word closed");
							return;
						}
						CreateDocument(templatePath, baseDate, WordApp, vacation: vacation, reportDifference: reportDifference, isSingle: isSingle);
						break;
					case -2146822750:
						//Document already fit on page
						try
						{
							ldoc.Close(SaveChanges: true);
						}
						catch
						{

						}
						break;
					case -2146233088:
						ThemedMessageBox.Show(ActiveTheme, "Connection refused by remotehost");
						break;
					default:
						Logger.LogError(ex);
						ThemedMessageBox.Show(ActiveTheme, ex.StackTrace);
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
			int reportNr = ConfigHandler.LastReportKW();

			if (ConfigHandler.LastReportKW() < weekOfYear)
			{
				//Missing reports in current year
				DateTime today = DateTime.Today.AddDays(-(weekOfYear - reportNr) * 7);
				for (int i = 1; i < weekOfYear - reportNr; i++)
				{
					CreateDocument(ConfigHandler.TemplatePath(), today.AddDays(i * 7), WordApp, vacation: vacation);
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
					CreateDocument(ConfigHandler.TemplatePath(), today.AddDays(i * 7), WordApp, vacation: vacation);
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
				ThemedMessageBox.Show(ActiveTheme, "A report has already been created for this week");
				return;
			}
			//Check if a report was created
			if (ConfigHandler.LastReportKW() > 0)
			{
				//Check if report for last week was created
				if (GetDistanceToToday() > 1)
				{
					if (ThemedMessageBox.Show(ActiveTheme, "You missed some reports were you on vacation?", "Vacation?", MessageBoxButtons.YesNo) == DialogResult.Yes)
					{
						CreateMissing(vacation: true);
					}
					else
					{
						if (ThemedMessageBox.Show(ActiveTheme, "Do you want to create empty reports then?", "Create?", MessageBoxButtons.YesNo) == DialogResult.Yes)
						{
							CreateMissing();
						}
					}
				}
			}

			CreateDocument(ConfigHandler.TemplatePath(), baseDate: DateTime.Today, WordApp, isSingle: true);
		}

		/// <summary>
		/// Calculates the number of weeks since last report creation
		/// </summary>
		/// <returns>The number of weeks since last report creation</returns>
		private int GetDistanceToToday()
		{
			int lastReportKW = ConfigHandler.LastReportKW();
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
			if (ConfigHandler.LegacyEdit())
				Edit(ConfigHandler.LastCreated());
			else
				EditInTb(ConfigHandler.LastCreated());
		}

		private void btPrint_Click(object sender, EventArgs e)
		{
			if (!HasWordStarted()) return;

			if (tvReports.SelectedNode == null)
			{
				ThemedMessageBox.Show(ActiveTheme, "No report selected");
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
				ThemedMessageBox.Show(ActiveTheme, "No unprinted reports found");
				return;
			}

			PrintDialog printDialog = new PrintDialog();
			if (printDialog.ShowDialog() != DialogResult.OK) return;
			foreach (string key in unPrintedFiles.Keys)
			{
				if (unPrintedFiles[key].Contains(ConfigHandler.LastCreated()))
				{
					if (ThemedMessageBox.Show(ActiveTheme, "Do you want to also print the last created report?\n(" + ConfigHandler.LastCreated() + ")", "Print last created?", MessageBoxButtons.YesNo) == DialogResult.No)
					{
						unPrintedFiles[key].Remove(ConfigHandler.LastCreated());
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
						ThemedMessageBox.Show(ActiveTheme, ex.StackTrace, "Error while printing" + filePath);
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
				if (File.Exists(path))
				{
					if (Path.GetExtension(path) != ".docx" || Path.GetFileName(path).StartsWith("~$"))
						return;
					if (!DocIsSamePathAsSelected())
						Doc = WordApp.Documents.Open(path);

					if (Doc.FormFields.Count != 10)
					{
						ThemedMessageBox.Show(ActiveTheme, "Invalid document (you will have to manually edit)");
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
						switch (edit.DialogResult)
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
						if (selectEdit.ShowDialog() == DialogResult.OK)
						{
							if (selectEdit.SelectedItems.Count == 0)
							{
								SaveOrExit();
								return;
							}
							IEnumerator enumerator = Doc.FormFields.GetEnumerator();
							EditForm edit;
							foreach (EditState si in selectEdit.SelectedItems)
							{
								if (enumerator.MoveNext() && si.ShouldEdit)
								{
									edit = new EditForm(title: si.EditorTitle, text: ((Word.FormField)enumerator.Current).Result);
									edit.RefreshConfigs += RefreshConfig;
									edit.ShowDialog();
									edit.RefreshConfigs -= RefreshConfig;
									switch (edit.DialogResult)
									{
										case DialogResult.OK:
										case DialogResult.Ignore:
											markAsEdited |= edit.Result != (enumerator.Current as Word.FormField)?.Result;
											FillText(WordApp, (Word.FormField)enumerator.Current, edit.Result);
											break;
										default:
											break;
									}
								}
							}
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
					ThemedMessageBox.Show(ActiveTheme, "Saved changes", "Saved");
				}
				else
				{
					ThemedMessageBox.Show(ActiveTheme, path + " not found was it deleted or moved?");
				}
			}
			catch (Exception ex)
			{
				switch (ex.HResult)
				{
					case -2147023174:
						ThemedMessageBox.Show(ActiveTheme, "an unexpected problem occured this progam will now close!");
						break;
					case -2147467262:
					case -2146823679:
						ThemedMessageBox.Show(ActiveTheme, "Word closed unexpectedly and is restarting please try again shortly");
						RestartWord();
						break;
					case -2146822750:
						//Document is only one page long
						/*doc.Save();
						doc.Close();
						doc = null;*/
						break;
					case -2146233088:
						ThemedMessageBox.Show(ActiveTheme, "Connection refused by remotehost");
						break;
					default:
						Logger.LogError(ex);
						ThemedMessageBox.Show(ActiveTheme, ex.StackTrace);
						Console.Write(ex.StackTrace);
						break;
				}
			}
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
				if (Path.GetExtension(path) != ".docx" || Path.GetFileName(path).StartsWith("~$"))
					return;
				Doc = WordApp.Documents.Open(path);
				if (Doc.FormFields.Count != 10)
				{
					ThemedMessageBox.Show(ActiveTheme, "Invalid document (you will have to manually edit)");
					Doc.Close(SaveChanges: false);
					Doc = null;
					EditMode = false;
					WasEdited = false;
					return;
				}
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
						ThemedMessageBox.Show(ActiveTheme, "an unexpected problem occured this progam will now close!");
						break;
					case -2147467262:
					case -2146823679:
						ThemedMessageBox.Show(ActiveTheme, "Word closed unexpectedly and is restarting please try again shortly");
						RestartWord();
						break;
					case -2146822750:
						//Document is only one page long
						/*doc.Save();
						doc.Close();
						doc = null;*/
						break;
					case -2146233088:
						ThemedMessageBox.Show(ActiveTheme, "Connection refused by remotehost");
						break;
					default:
						Logger.LogError(ex);
						ThemedMessageBox.Show(ActiveTheme, ex.StackTrace);
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
				if (UploadedReports.GetUploadedReport(Doc.FullName, out UploadedReport report) && report.Status == ReportNode.UploadStatuses.Accepted)
				{
					rtbWork.Text = Doc.FormFields[6].Result;
					rtbSchool.Text = Doc.FormFields[8].Result;
					WasEdited = false;
					ThemedMessageBox.Show(ActiveTheme, text: "Can not change accepted report", title: "Save not possible");
					return;
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
				ThemedMessageBox.Show(ActiveTheme, "Saved changes", "Saved");
				WasEdited = false;
			}
			catch (Exception ex)
			{
				switch (ex.HResult)
				{
					case -2147023174:
						ThemedMessageBox.Show(ActiveTheme, "an unexpected problem occured this progam will now close!");
						break;
					case -2147467262:
					case -2146823679:
						ThemedMessageBox.Show(ActiveTheme, "Word closed unexpectedly and is restarting please try again shortly");
						RestartWord();
						break;
					case -2146822750:
						//Document is one page already
						break;
					case -2146233088:
						ThemedMessageBox.Show(ActiveTheme, "Connection refused by remotehost");
						break;
					default:
						Logger.LogError(ex);
						ThemedMessageBox.Show(ActiveTheme, ex.StackTrace);
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

			if (ThemedMessageBox.Show(ActiveTheme, "Save unsaved changes?", "Save?", MessageBoxButtons.YesNo) == DialogResult.Yes)
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
				ThemedMessageBox.Show(ActiveTheme, "You may only print Documents(*.docx) files");
				return;
			}
			DirectoryInfo printed = new DirectoryInfo(Path.GetDirectoryName(path));
			if (printed.Name == "Gedruckt")
			{
				if (ThemedMessageBox.Show(ActiveTheme, "Report was already printed do you want to print it again?", "Reprint?", MessageBoxButtons.YesNo) != DialogResult.Yes)
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
				ThemedMessageBox.Show(ActiveTheme, ex.StackTrace);
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
				ThemedMessageBox.Show(ActiveTheme, path + " not Found (was it moved or deleted?)");
				return;
			}
			if (File.GetAttributes(path) == FileAttributes.Directory)
			{
				ThemedMessageBox.Show(ActiveTheme, "You may not delete folders using the manager");
				return;
			}
			if (Path.GetExtension(path) != ".docx" && !path.Contains("\\Logs") && !Path.GetFileName(path).StartsWith("~$"))
			{
				ThemedMessageBox.Show(ActiveTheme, "You may only delete Word documents (*.docx) or their temporary files");
				return;
			}
			if (ThemedMessageBox.Show(ActiveTheme, "Are you sure you want to delete the selected file?", "Delete?", MessageBoxButtons.YesNo) != DialogResult.Yes)
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
			if (path == ConfigHandler.LastCreated())
			{
				if (ConfigHandler.LastReportKW() == Culture.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday))
				{
					if (ConfigHandler.ReportNumber() > 1)
						ConfigHandler.ReportNumber(ConfigHandler.ReportNumber() - 1);
					ConfigHandler.LastReportKW(Culture.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday) - 1);
					ConfigHandler.SaveConfig();
				}
			}
			File.Delete(path);
			UpdateTree();
			ThemedMessageBox.Show(ActiveTheme, "File deleted successfully");
		}

		private void tvReports_DoubleClick(object sender, EventArgs e)
		{
			if ((File.GetAttributes(FullSelectedPath) & FileAttributes.Directory) == FileAttributes.Directory) return;
			if (Path.GetExtension(FullSelectedPath) != ".docx" || Path.GetFileName(FullSelectedPath).StartsWith("~$"))
				return;
			if (!HasWordStarted()) return;
			if (tvReports.SelectedNode == null)
				return;
			if (DocIsSamePathAsSelected())
				return;
			SaveOrExit();
			if (ConfigHandler.LegacyEdit())
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
			bool isNameValid = ReportFinder.IsReportNameValid(tvReports.SelectedNode.Text);
			bool isUploaded = UploadedReports.GetUploadStatus(tvReports.SelectedNode.FullPath, out ReportNode.UploadStatuses status);
			miEdit.Enabled = !isInLogs && isNameValid;
			//miEdit.Visible = !isInLogs && tvReports.SelectedNode.Text.EndsWith(".docx") && !tvReports.SelectedNode.Text.StartsWith("~$");
			miPrint.Enabled = !isInLogs && isNameValid;
			//miPrint.Visible = !isInLogs && tvReports.SelectedNode.Text.EndsWith(".docx") && !tvReports.SelectedNode.Text.StartsWith("~$");
			miDelete.Enabled = isInLogs || tvReports.SelectedNode.Text.EndsWith(".docx") || tvReports.SelectedNode.Text.StartsWith("~$");
			//miDelete.Visible = isInLogs || tvReports.SelectedNode.Text.EndsWith(".docx") || tvReports.SelectedNode.Text.StartsWith("~$");
			miQuickEditOptions.Enabled = !isInLogs && isNameValid;
			//miQuickEditOptions.Visible = !isInLogs && tvReports.SelectedNode.Text.EndsWith(".docx") && !tvReports.SelectedNode.Text.StartsWith("~$");
			miUploadAsNext.Enabled = !isInLogs && isNameValid && !isUploaded;
			miHandInSingle.Enabled = isNameValid && isUploaded && (status == ReportNode.UploadStatuses.Uploaded);
			miUpdateReport.Enabled = isNameValid && isUploaded && (status == ReportNode.UploadStatuses.Uploaded || status == ReportNode.UploadStatuses.Rejected);
		}

		private void btOptions_Click(object sender, EventArgs e)
		{
			int tabStops = ConfigHandler.TabStops();
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
			ThemeSetter.SetThemes(this, theme);
			tvReports.Refresh();
		}

		private void DetectKeys(object sender, KeyEventArgs e)
		{
			if (e.Control && e.KeyCode == Keys.S && EditMode)
			{
				SaveFromTb();
			}
		}

		private void EditRichTextBox(object sender, EventArgs e)
		{
			WasEdited = true;
		}

		private void miWordVisible_Click(object sender, EventArgs e)
		{
			if (WordInitialized)
				WordApp.Visible = miWordVisible.Checked;
			WordVisible = miWordVisible.Checked;
		}

		private void FormManager_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				if (EditMode && WasEdited && ThemedMessageBox.Show(ActiveTheme, "Do you want to save unsaved changes?", "Save changes?", MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					SaveFromTb();
				}
				if (Doc != null)
				{
					Doc.Close(SaveChanges: false);
					Doc = null;
				}
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
					if (Path.GetExtension(FullSelectedPath) != ".docx" || Path.GetFileName(FullSelectedPath).StartsWith("~$")) return;
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
						ThemedMessageBox.Show(ActiveTheme, "No opened report to close", "Could not close");
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
			if (!WordInitialized)
			{
				ThemedMessageBox.Show(ActiveTheme, "Word is still starting, please try again", "Please try again");
			}
			return WordInitialized;
		}

		private void miRevealInExplorer_Click(object sender, EventArgs e)
		{
			if (Directory.Exists(ActivePath))
				Process.Start(ActivePath);
			else
				ThemedMessageBox.Show(ActiveTheme, "The working directory has been deleted from an external source", "You may have a problem");
		}

		/// <summary>
		/// Reloads <see cref="ConfigHandler"/>
		/// </summary>
		public void RefreshConfig()
		{
			ConfigHandler.ReloadConfig();
		}

		/// <summary>
		/// Checks wether or not the word app is still open by comparing types of open and closed word app
		/// </summary>
		/// <returns><see langword="true"/> if word is open and <see langword="false"/> otherwise</returns>
		private bool CheckIfWordOpen()
		{
			return !typeof(Word.Application).IsAssignableFrom(WordApp.GetType());
		}

		/// <summary>
		/// Restarts word if it has been closed
		/// </summary>
		private void RestartWord()
		{
			if (WordApp == null)
			{
				WordApp = new Word.Application();
				WordInitialized = true;
				return;
			}

			//Check if word is still open
			if (CheckIfWordOpen())
				return;
			WordInitialized = false;
			WordApp = new Word.Application();
			WordInitialized = true;
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
				ThemedMessageBox.Show(ActiveTheme, "No opened report to close", "Could not close");
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
		/// Uploads a single report to IHK, handles exceptions
		/// </summary>
		/// <param name="doc">Report to upload</param>
		/// <returns><see cref="UploadResult"/> of creation or <see langword="null"/> if an error occurred</returns>
		private async Task<UploadResult> TryUploadReportToIHK(Word.Document doc)
		{
			try
			{
				return await IHKClient.CreateReport(doc, ConfigHandler.IHKCheckMatchingStartDates());
			}
			catch (HttpRequestException ex)
			{
				Logger.LogError(ex);
				ThemedMessageBox.Show(ActiveTheme, text: "A network error has occurred, please check your connection", title: "Network error");
				return null;
			}
			catch (StartDateMismatchException ex)
			{
				ThemedMessageBox.Show(ActiveTheme, text: ex.Message, title: ex.GetType().Name);
				return null;
			}
			catch (Exception ex)
			{
				ThemedMessageBox.Show(ActiveTheme, text: $"An unexpected exception has occurred, a complete log has been saved to\n{Logger.LogError(ex)}:\n{ex.StackTrace}", title: ex.GetType().Name);
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
				ThemedMessageBox.Show(ActiveTheme, text: "Report was already uploaded", title: "Report already uploaded");
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
				ThemedMessageBox.Show(ActiveTheme, text: "Invalid document, please upload manually", title: "Invalid document");
				doc.Close(SaveChanges: false);
				return;
			}
			UploadResult result = await TryUploadReportToIHK(doc);
			if (result == null)
				return;
			switch (result.Result)
			{
				case CreateResults.Success:
					ThemedMessageBox.Show(ActiveTheme, text: "Report uploaded successfully", title: "Upload successful");
					UploadedReports.Instance.AddReport(tvReports.SelectedNode.FullPath, new UploadedReport(result.StartDate, lfdNr: result.LfdNR));
					break;
				case CreateResults.Unauthorized:
					ThemedMessageBox.Show(ActiveTheme, text: "Session has expired please try again", "Session expired");
					break;
				default:
					ThemedMessageBox.Show(ActiveTheme, text: "Unable to upload report, please try again in a bit", title: "Unable to upload");
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

				List<string> files = new List<string>();
				if (UploadedReports.Instance.TryGetValue(ActivePath, out Dictionary<string, UploadedReport> uploadedPaths))
					files = uploadedPaths.Keys.ToList();

				FolderSelect fs = new FolderSelect(tvReports.Nodes[0], node =>
				{
					return files.Contains(GetFullNodePath(node)) || (!ReportFinder.IsReportNameValid(node.Text) && node.Nodes.Count == 0);
				});
				if (fs.ShowDialog() != DialogResult.OK)
				{
					progressForm.Done();
					return;
				}
				ReportFinder.FindReports(fs.FilteredNode, out List<TreeNode> reports);
				progressForm.Status = $"Uploading {reports.Count} reports";

				string activePath = "";
				try
				{
					activePath = Path.Combine(Doc?.Path, Doc?.Name);
				}
				catch { }
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
						ThemedMessageBox.Show(ActiveTheme, text: $"Invalid document, please add missing form fields to {path}.\nUploading is stopped", title: "Invalid document");
						doc.Close(SaveChanges: false);
						OpenAllDocuments(openReports, activePath);
						progressForm.Done();
						return;
					}
					UploadResult result = await TryUploadReportToIHK(doc);
					if (result == null)
					{
						progressForm.Status = $"Uploading aborted: upload failed";
						ThemedMessageBox.Show(ActiveTheme, text: $"Upload of {path} failed, upload was canceled!", title: "Upload failed");
						doc.Close(SaveChanges: false);
						OpenAllDocuments(openReports, activePath);
						progressForm.Done();
						return;
					}
					switch (result.Result)
					{
						case CreateResults.Success:
							UploadedReports.Instance.AddReport(nodePath, new UploadedReport(result.StartDate, lfdNr: result.LfdNR));
							break;
						case CreateResults.Unauthorized:
							ThemedMessageBox.Show(ActiveTheme, text: "Session has expired please try again", "Session expired");
							doc.Close(SaveChanges: false);
							OpenAllDocuments(openReports, activePath);
							progressForm.Status = $"Abort: Unauthorized";
							progressForm.Done();
							return;
						default:
							ThemedMessageBox.Show(ActiveTheme, text: $"Upload of {path} failed, upload was canceled!", title: "Upload failed");
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

					await Task.Delay(ConfigHandler.IHKUploadDelay());
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
				ThemedMessageBox.Show(ActiveTheme, text: text, title: "Upload finished");
				UpdateTree();
			});
		}

		private async void UploadSelectionClick(object sender, EventArgs e)
		{
			if (!HasWordStarted())
				return;
			if (ThemedMessageBox.Show(ActiveTheme, text: "Warning, this will upload all reports selected in the next window in the order they appear!\nDo you want to proceed?", title: "Caution", buttons: MessageBoxButtons.YesNo) != DialogResult.Yes)
				return;
			EventProgressForm progressForm = new EventProgressForm("Upload progress");
			progressForm.Show();
			await UploadSelection(progressForm);
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
				result.Add(doc.Path);
				if (doc == Doc)
					SaveOrExit();
				else
					doc.Close();
			}
			return result;
		}

		/// <summary>
		/// Generates the full path of the <paramref name="node"/>
		/// </summary>
		/// <param name="node"><see cref="TreeNode"/> to get path for</param>
		/// <param name="separator">String to separate path elements with</param>
		/// <returns>Full path separated by <paramref name="separator"/></returns>
		private string GetFullNodePath(TreeNode node, string separator = "\\")
		{
			TreeNode current = node;

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
				ThemedMessageBox.Show(ActiveTheme, text: "A network error has occurred, please check your connection", title: "Network error");
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
				List<UploadedReport> reportList = await IHKClient.GetReportStatuses();
				reportList.ForEach(report => result |= UploadedReports.UpdateReportStatus(report.StartDate, report.Status, lfdnr: report.LfdNR));
				if (result)
				{
					UpdateTree();
					ThemedMessageBox.Show(ActiveTheme, text: "Update complete.", title: "Update complete");
				}
				else
					ThemedMessageBox.Show(ActiveTheme, text: "Already up to date", title: "Update complete");
			}
			catch (HttpRequestException ex)
			{
				Logger.LogError(ex);
				ThemedMessageBox.Show(ActiveTheme, text: "A network error has occurred, please check your connection", title: "Network error");
			}
			return result;
		}

		private async void MainForm_Load(object sender, EventArgs e)
		{
			if (ConfigHandler.AutoSyncStatusesWithIHK() && await UpdateStatuses())
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
				ThemedMessageBox.Show(ActiveTheme, text: "A network error has occurred, please check your connection", title: "Network error");
			}
			catch (Exception ex)
			{
				ThemedMessageBox.Show(ActiveTheme, text: $"An unexpected exception has occurred, a complete log has been saved to\n{Logger.LogError(ex)}:\n{ex.StackTrace}", title: ex.GetType().Name);
			}
			return false;
		}

		private async void miHandInSingle_Click(object sender, EventArgs e)
		{
			if (!UploadedReports.Instance.TryGetValue(ActivePath, out Dictionary<string, UploadedReport> reports))
			{
				//Should never happen as menu item should be diabled
				ThemedMessageBox.Show(ActiveTheme, text: $"No reports in {ActivePath} have been uploaded yet", title: "Hand in failed");
				return;
			}
			if (!reports.TryGetValue(tvReports.SelectedNode.FullPath, out UploadedReport report))
			{
				//Should never happen as menu item should be diabled
				ThemedMessageBox.Show(ActiveTheme, text: $"Report {FullSelectedPath} was not uploaded yet", title: "Hand in failed");
				return;
			}
			if (!(report.LfdNR is int lfdnr))
			{
				ThemedMessageBox.Show(ActiveTheme, text: $"Lfdnr of {FullSelectedPath} could not be read", title: "Hand in failed");
				return;
			}
			//Prevent unsaved changes from being left locally
			if (report.WasEditedLocally)
			{
				if (ThemedMessageBox.Show(ActiveTheme, text: $"Report {FullSelectedPath} has local changes, do you want to upload them now?", title: "Upload changes?", buttons: MessageBoxButtons.YesNo) != DialogResult.Yes)
				{
					ThemedMessageBox.Show(ActiveTheme, text: "Hand in was canceled", title: "Hand in canceled");
					return;
				}
				if (!WordInitialized)
				{
					ThemedMessageBox.Show(ActiveTheme, text: "Word has not started yet, hand in was canceled", title: "Hand in canceled");
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
					ThemedMessageBox.Show(ActiveTheme, text: "Update of report failed, hand in was canceled", title: "Hand in canceled");
					return;
				}

				if (!HandleUpdateResults(result.Result, report.StartDate))
					return;
			}

			if (!await TryHandIn(lfdnr))
			{
				ThemedMessageBox.Show(ActiveTheme, text: $"Report {FullSelectedPath} could not be handed in", title: "Hand in failed");
				return;
			}

			UploadedReports.UpdateReportStatus(report.StartDate, ReportNode.UploadStatuses.HandedIn, lfdnr);
			UpdateTree();
			ThemedMessageBox.Show(ActiveTheme, text: "Hand in successful", title: "Report handed in");
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
					ThemedMessageBox.Show(ActiveTheme, text: "Login session has expired, try restarting report manager, hand in was skipped", title: "Login session expired");
					break;
				case CreateResults.CreationFailed:
					ThemedMessageBox.Show(ActiveTheme, text: "Report could not be loaded from IHK server, hand in was skipped", title: "Unable to edit report");
					break;
				case CreateResults.UploadFailed:
					ThemedMessageBox.Show(ActiveTheme, text: "Report could not be updated, hand in was skipped", title: "Handin failed");
					break;
				default:
					ThemedMessageBox.Show(ActiveTheme, text: "Unknown upload result, hand in was skipped", title: "Unknown result");
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

				List<string> files = new List<string>();
				if (UploadedReports.Instance.TryGetValue(ActivePath, out Dictionary<string, UploadedReport> uploadedPaths))
					files = uploadedPaths.Keys.ToList();

				FolderSelect fs = new FolderSelect(tvReports.Nodes[0], node =>
				{
					return (node is ReportNode reportNode) && (!files.Contains(GetFullNodePath(node)) || (uploadedPaths.TryGetValue(GetFullNodePath(node), out UploadedReport report) && report.Status != ReportNode.UploadStatuses.Uploaded));
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
					if (!uploadedPaths.TryGetValue(nodePath, out UploadedReport report))
					{
						ThemedMessageBox.Show(ActiveTheme, text: $"Report {fullPath} was not uploaded and could not be handed in as a result", title: "Hand in failed");
						progressForm.Status = "\t- skipped: Not uploaded";
						continue;
					}
					if (report.Status != ReportNode.UploadStatuses.Uploaded)
					{
						ThemedMessageBox.Show(ActiveTheme, text: $"Report {fullPath} could not be handed in due to its upload status", title: "Hand in failed");
						progressForm.Status = "\t- skipped: Can not be handed in";
						continue;
					}
					if (!(report.LfdNR is int lfdnr))
					{
						ThemedMessageBox.Show(ActiveTheme, text: $"Lfdnr of {fullPath} could not be read and report could not be handed in as a result", title: "Hand in failed");
						progressForm.Status = "\t- skipped: Unable to read lfdnr";
						continue;
					}
					//Prevent unsaved changes from being left locally
					if (report.WasEditedLocally)
					{
						if (!WordInitialized)
						{
							ThemedMessageBox.Show(ActiveTheme, text: "Word has not started yet, hand in was canceled", title: "Hand in canceled");
							return;
						}
						//Check if changes should be uploaded or not
						if (!updateSet)
						{
							autoUpdateAllChanges = ThemedMessageBox.Show(ActiveTheme, text: "Should all local changes be uploaded to IHK?", title: "Automatically upload changes?", buttons: MessageBoxButtons.YesNo) == DialogResult.Yes;
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
							ThemedMessageBox.Show(ActiveTheme, text: "Update of report failed, hand in was skipped", title: "Hand in skipped");
							progressForm.Status = "\t- skipped: Uploading changes failed";
							return;
						}

						if (result.Result != CreateResults.Success)
						{
							if (ThemedMessageBox.Show(ActiveTheme, text: $"Update of report {fullPath} failed, do you want to continue trying to hand in reports?", title: "Update failed", buttons: MessageBoxButtons.YesNo) != DialogResult.Yes)
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
						if (ThemedMessageBox.Show(ActiveTheme, text: $"Hand in of report {fullPath} failed, do you want to continue trying to hand in reports?", title: "Hand in failed", buttons: MessageBoxButtons.YesNo) != DialogResult.Yes)
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

					await Task.Delay(ConfigHandler.IHKUploadDelay());
					progressForm.Status = "\t- Success";
				}
				progressForm.Status = "Done";
				progressForm.EventsText += "\nDone";

				if (reports.Count == 0)
				{
					ThemedMessageBox.Show(ActiveTheme, text: "All reports were already handed in", title: "Hand in complete");
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
				ThemedMessageBox.Show(ActiveTheme, text: text, title: "Hand in complete");
			});
		}

		private async void HandInSelectionClick(object sender, EventArgs e)
		{
			if (ThemedMessageBox.Show(ActiveTheme, text: "Warning, this will hand in all reports selected in the next window in the order they appear!\nDo you want to proceed?", title: "Caution", buttons: MessageBoxButtons.YesNo) != DialogResult.Yes)
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
						ThemedMessageBox.Show(ActiveTheme, text: "Invalid document, please upload manually", title: "Invalid document");
						return null;
					case HttpRequestException _:
						Logger.LogError(ex);
						ThemedMessageBox.Show(ActiveTheme, text: "A network error has occurred, please check your connection", title: "Network error");
						return null;
					default:
						ThemedMessageBox.Show(ActiveTheme, text: $"An unexpected exception has occurred, a complete log has been saved to\n{Logger.LogError(ex)}:\n{ex.StackTrace}", title: ex.GetType().Name);
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
				ThemedMessageBox.Show(ActiveTheme, text: "No network connection", title: "No connection");
				return;
			}
			if (!UploadedReports.GetUploadedReport(tvReports.SelectedNode.FullPath, out UploadedReport report))
			{
				ThemedMessageBox.Show(ActiveTheme, text: $"Could not find report {FullSelectedPath} in uploaded list, please add {tvReports.SelectedNode.FullPath} if it is uploaded", title: "Report not found");
				return;
			}
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
				ThemedMessageBox.Show(ActiveTheme, text: "Invalid document, please upload manually", title: "Invalid document");
				doc.Close(SaveChanges: false);
				return;
			}
			if (!(report.LfdNR is int lfdnr))
			{
				ThemedMessageBox.Show(ActiveTheme, text: $"Unable to load lfdnr from {FullSelectedPath}, verify that it is correct", title: "Unable to edit");
				return;
			}

			UploadResult result = await TryUpdateReport(doc, lfdnr);
			if (result == null)
				return;
			if (close)
				doc.Close();

			UploadedReports.UpdateReportStatus(report.StartDate, ReportNode.UploadStatuses.Uploaded, report.LfdNR);
			UploadedReports.SetEdited(tvReports.SelectedNode.FullPath, false);
			UpdateTree();
			ThemedMessageBox.Show(ActiveTheme, text: "Report was successfully updated", title: "Update complete");
		}

		/// <summary>
		/// Checks
		/// </summary>
		/// <returns></returns>
		private bool CheckNetwork()
		{
			return System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
		}
	}
}
