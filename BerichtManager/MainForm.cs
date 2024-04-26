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
		private readonly TaskFactory WordTaskFactory = Task.Factory;

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
			tvReports.Nodes.Clear();
			tvReports.Nodes.Add(CreateDirectoryNode(Info));
			tvReports.Sort();
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
				directoryNode.Nodes.Add(new TreeNode(file.Name));
			return directoryNode;
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
						if (edit.ShowDialog() == DialogResult.OK)
						{
							FillText(WordApp, (Word.FormField)enumerator.Current, edit.Result);
						}
						else
						{
							if (edit.DialogResult == DialogResult.Ignore)
							{
								FillText(WordApp, (Word.FormField)enumerator.Current, edit.Result);
							}
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
								if (enumerator.MoveNext())
								{
									if (si.ShouldEdit)
									{
										edit = new EditForm(title: si.EditorTitle, text: ((Word.FormField)enumerator.Current).Result);
										edit.RefreshConfigs += RefreshConfig;
										edit.ShowDialog();
										edit.RefreshConfigs -= RefreshConfig;
										if (edit.DialogResult == DialogResult.OK)
										{
											FillText(WordApp, (Word.FormField)enumerator.Current, edit.Result);
										}
										else
										{
											if (edit.DialogResult == DialogResult.Abort)
											{
												break;
											}
											else
											{
												if (edit.DialogResult == DialogResult.Ignore)
												{
													FillText(WordApp, (Word.FormField)enumerator.Current, edit.Result);
													break;
												}
											}
										}
									}
								}
							}
						}
					}
					FitToPage(Doc);
					Doc.Save();
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
				if (Doc == null || WordApp == null)
					return;
				FillText(WordApp, Doc.FormFields[6], rtbWork.Text);
				FillText(WordApp, Doc.FormFields[8], rtbSchool.Text);
				FitToPage(Doc);
				Doc.Save();
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
			miEdit.Enabled = !isInLogs && tvReports.SelectedNode.Text.EndsWith(".docx") && !tvReports.SelectedNode.Text.StartsWith("~$");
			//miEdit.Visible = !isInLogs && tvReports.SelectedNode.Text.EndsWith(".docx") && !tvReports.SelectedNode.Text.StartsWith("~$");
			miPrint.Enabled = !isInLogs && tvReports.SelectedNode.Text.EndsWith(".docx") && !tvReports.SelectedNode.Text.StartsWith("~$");
			//miPrint.Visible = !isInLogs && tvReports.SelectedNode.Text.EndsWith(".docx") && !tvReports.SelectedNode.Text.StartsWith("~$");
			miDelete.Enabled = isInLogs || tvReports.SelectedNode.Text.EndsWith(".docx") || tvReports.SelectedNode.Text.StartsWith("~$");
			//miDelete.Visible = isInLogs || tvReports.SelectedNode.Text.EndsWith(".docx") || tvReports.SelectedNode.Text.StartsWith("~$");
			miQuickEditOptions.Enabled = !isInLogs && tvReports.SelectedNode.Text.EndsWith(".docx") && !tvReports.SelectedNode.Text.StartsWith("~$");
			//miQuickEditOptions.Visible = !isInLogs && tvReports.SelectedNode.Text.EndsWith(".docx") && !tvReports.SelectedNode.Text.StartsWith("~$");
			return;
		}

		private void btOptions_Click(object sender, EventArgs e)
		{
			int tabStops = ConfigHandler.TabStops();
			OptionMenu optionMenu = new OptionMenu();
			optionMenu.ActiveThemeChanged += ActiveThemeChanged;
			optionMenu.ReportFolderChanged += ReportFolderChanged;
			optionMenu.TabStopsChanged += UpdateTabStops;
			optionMenu.FontSizeChanged += ChangeFontSize;
			optionMenu.ShowDialog();
			optionMenu.ActiveThemeChanged -= ActiveThemeChanged;
			optionMenu.ReportFolderChanged -= ReportFolderChanged;
			optionMenu.TabStopsChanged -= UpdateTabStops;
			optionMenu.FontSizeChanged -= ChangeFontSize;
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
				ThemedMessageBox.Show(ActiveTheme, text: "No file or folder was selected, check was canceled", title: "No selection was made");
				return;
			}
			string activePath = "";
			try
			{
				activePath = Doc?.Path;
			}
			catch { }
			List<string> openReports = CloseAllReports();
			ReportChecker checker = new ReportChecker(WordApp);
			if (!checker.Check(select.FilteredNode, out List<IReportDiscrepancy> discrepancies, check: check))
			{
				OpenAllDocuments(openReports, activePath);
				return;
			}
			OpenAllDocuments(openReports, activePath);
			if (discrepancies == null)
				return;
			if (discrepancies.Count == 0)
			{
				ThemedMessageBox.Show(ActiveTheme, text: "No discrepancies found", "Check done");
				return;
			}
			StringBuilder message = new StringBuilder();
			message.AppendLine("At least one discrepancy was found:");
			discrepancies.ForEach(d => message.AppendLine(d.ToString()));
			ThemedMessageBox.Show(ActiveTheme, text: message.ToString(), title: "Discrepancy found");
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
	}
}
