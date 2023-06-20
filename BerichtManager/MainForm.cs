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
using BerichtManager.ThemeManagement.DefaultThemes;
using System.Diagnostics;
using BerichtManager.HelperClasses;
using BerichtManager.WebUntisClient;
using System.Threading.Tasks;

namespace BerichtManager
{
	public partial class MainForm : Form
	{
		private Word.Document doc = null;
		private Word.Application wordApp;
		private readonly ConfigHandler configHandler = new ConfigHandler(themeManager);
		private readonly Client client;

		/// <summary>
		/// Directory containing all reports
		/// </summary>
		private DirectoryInfo info;
		private readonly CultureInfo culture = new CultureInfo("de-DE");
		private int tvReportsMaxWidth = 50;
		private bool editMode = false;
		private bool wasEdited = false;
		private CustomNodeDrawer nodeDrawer;
		private static readonly ThemeManager themeManager = new ThemeManager();
		private ITheme activeTheme;

		/// <summary>
		/// Value if word has a visible window or not
		/// </summary>
		private bool WordVisible { get; set; } = false;

		/// <summary>
		/// Version number
		/// Major.Minor.Build.Revision
		/// </summary>
		public const string VersionNumber = "1.10.7";

		/// <summary>
		/// String to be printed
		/// </summary>
		private string VersionString = "v" + VersionNumber;

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
			activeTheme = themeManager.GetTheme(configHandler.ActiveTheme());
			if (activeTheme == null)
				activeTheme = new DarkMode();
			ThemeSetter.SetThemes(this, activeTheme);
			ThemeSetter.SetThemes(toRightClickMenu, activeTheme);
			nodeDrawer = new CustomNodeDrawer(activeTheme);
			foreach (Control control in this.Controls)
				control.KeyDown += DetectKeys;
			this.Icon = Icon.ExtractAssociatedIcon(Path.GetFullPath(".\\BerichtManager.exe"));
			tvReports.TreeViewNodeSorter = new TreeNodeSorter();
			info = new DirectoryInfo(configHandler.ReportPath());
			ActivePath = configHandler.ReportPath();
			UpdateTree();
			if (configHandler.LastCreated() == "")
			{
				miEditLatest.Enabled = false;
			}
			SetComponentPositions();
			UpdateTabStops(this, configHandler.TabStops());
			client = new Client(configHandler);
			if (File.Exists(configHandler.PublishPath()))
				if (CompareVersionNumbers(VersionNumber, FileVersionInfo.GetVersionInfo(configHandler.PublishPath()).FileVersion) > 0)
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
				TvReportsMaxWidth(node);
			tvReports.CollapseAll();
			splitterTreeBoxes.SplitPosition = tvReportsMaxWidth + 5;
			Rectangle bounds = scTextBoxes.Bounds;
			bounds.X = paFileTree.Bounds.Right + 1;
			bounds.Width = Width - 1 - paFileTree.Bounds.Width;
			scTextBoxes.Bounds = bounds;
			rtbSchool.Font = new Font(rtbSchool.Font.FontFamily, configHandler.EditorFontSize());
			rtbWork.Font = new Font(rtbWork.Font.FontFamily, configHandler.EditorFontSize());
		}

		/// <summary>
		/// Calculates the max size the tree view should have
		/// </summary>
		/// <param name="treeNode">root nodes of tree</param>
		private void TvReportsMaxWidth(TreeNode treeNode)
		{
			foreach (TreeNode node in treeNode.Nodes)
			{
				if (node.Nodes.Count > 0)
				{
					TvReportsMaxWidth(node);
				}
				else
				{
					if (tvReportsMaxWidth < node.Bounds.Right + 1)
						tvReportsMaxWidth = node.Bounds.Right + 20;
				}
			}
		}

		/**
		 *<summary>Fills the TreeView of the main form with nodes of the upper directory</summary> 
		*/
		private void UpdateTree()
		{
			tvReports.Nodes.Clear();
			tvReports.Nodes.Add(CreateDirectoryNode(info));
			tvReports.Sort();
		}

		/**
		 * <summary>
		 * Generates TreeNodes from files and directorys contained in the upper directory
		 * </summary>
		 * <param name="directoryInfo">The target directory</param>
		 * <returns>A Treenode representing the contents of <paramref name="directoryInfo"/></returns>
		*/
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

		/**
		 * <summary>
		 * Fills a WordInterop TextField with text
		 * </summary>
		 * <param name="app">The Word Application containing the documents with FormFields to fill</param>
		 * <param name="field">The FormField to fill with Text</param>
		 * <param name="text">The Text to Fill</param>
		*/
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
				field.Result = "";
				app.Selection.Text = text.Replace("\n", "\v").Substring(0, 200);
				field.Result = field.Result.Trim() + " ";
				app.Selection.MoveLeft(Word.WdUnits.wdCharacter, 1);
				app.Selection.TypeText(text.Substring(201));
			}
			else
			{
				field.Result = text.Replace("\n", "\v");
			}
		}

		/**
		 * <summary>
		 * Sets the global font in a document and fits document to pages
		 * </summary>
		 * <param name="app">The Wordapp containing the document</param>
		 * <param name="doc">The Document which needs a font change</param>
		*/
		private void SetFontInDoc(Word.Document doc, Word.Application app)
		{
			doc.Content.Select();
			if (app.Selection.Font.Name != configHandler.EditorFont())
			{
				app.Selection.Font.Name = configHandler.EditorFont();
				MessageBox.Show("Changed report Font to: " + configHandler.EditorFont(), "Font changed!");
			}
			try
			{
				doc.FitToPages();
			}
			catch
			{

			}
		}

		/**
		 * <summary>
		 * Creates a new Word document from a given template for a given time.
		 * </summary>
		 * <param name="templatePath">The full path of the template to be used</param>
		 * <param name="baseDate">The date of the report to be created</param>
		 * <param name="app">The Wordapp that is used to create the document</param>
		 * <param name="vacation">If you missed reports due to vacation</param>
		 * <param name="reportDifference">The reportnumber difference between the report to create and the would be current report number</param>
		 * <param name="isSingle">Used to tell the method that this is a regular create job</param>
		*/
		private void CreateDocument(string templatePath, DateTime baseDate, Word.Application app, bool vacation = false, int reportDifference = 0, bool isSingle = false)
		{
			Word.Document ldoc = null;
			if (!File.Exists(templatePath))
			{
				MessageBox.Show(configHandler.TemplatePath() + " was not found was it moved or deleted?");
				return;
			}
			try
			{
				int weekOfYear = culture.Calendar.GetWeekOfYear(baseDate, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
				ldoc = app.Documents.Add(Template: templatePath);

				if (ldoc.FormFields.Count != 10)
				{
					MessageBox.Show("Invalid template");
					ldoc.Close(SaveChanges: false);
					ldoc = null;
					return;
				}

				EditForm form;
				//Fill name
				IEnumerator enumerator = ldoc.FormFields.GetEnumerator();
				enumerator.MoveNext();
				if (!string.IsNullOrEmpty(configHandler.ReportUserName()))
				{
					((Word.FormField)enumerator.Current).Result = configHandler.ReportUserName();
				}
				else
				{
					form = new EditForm("Enter your name", activeTheme, text: "Name Vorname");
					form.RefreshConfigs += RefreshConfig;
					if (form.ShowDialog() == DialogResult.OK)
					{
						configHandler.ReportUserName(form.Result);
						configHandler.SaveConfig();
						((Word.FormField)enumerator.Current).Result = configHandler.ReportUserName();
					}
					else
					{
						MessageBox.Show("Cannot proceed without a name!", "Name required!");
						return;
					}
					form.RefreshConfigs -= RefreshConfig;
				}
				enumerator.MoveNext();

				//Enter report nr.
				if (int.TryParse(configHandler.ReportNumber(), out int number))
				{
					FillText(app, ((Word.FormField)enumerator.Current), (number + reportDifference).ToString());
				}
				else
				{
					FillText(app, ((Word.FormField)enumerator.Current), "-1");
					MessageBox.Show("Unable to load report number from config", "Error while loading report number");
				}

				//Enter week start and end
				DateTime today = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day);
				DateTime thisWeekStart = today.AddDays(-(int)baseDate.DayOfWeek + 1);
				DateTime thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
				enumerator.MoveNext();
				((Word.FormField)enumerator.Current).Result = thisWeekStart.ToString("dd.MM.yyyy");
				enumerator.MoveNext();
				if (configHandler.EndWeekOnFriday())
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
					if (configHandler.UseUserPrefix())
						FillText(app, (Word.FormField)enumerator.Current, configHandler.CustomPrefix() + "Urlaub");
					else
						FillText(app, (Word.FormField)enumerator.Current, "-Urlaub");
				}
				else
				{
					form = new EditForm("Betriebliche Tätigkeiten" + "(KW " + weekOfYear + ")", activeTheme, isCreate: true);
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
					if (configHandler.UseUserPrefix())
						FillText(app, (Word.FormField)enumerator.Current, configHandler.CustomPrefix() + "Urlaub");
					else
						FillText(app, (Word.FormField)enumerator.Current, "-Urlaub");
				}
				else
				{
					form = new EditForm("Unterweisungen, betrieblicher Unterricht, sonstige Schulungen" + "(KW " + weekOfYear + ")", activeTheme, text: "-Keine-", isCreate: true);
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
						client.GetClassesFromWebUntis().ForEach(c => classes += c);
					}
					catch (AggregateException e)
					{
						MessageBox.Show("Unable to process classes from web\n(try to cancel the creation process and start again)");
						Logger.LogError(e);
					}
					form = new EditForm("Berufsschule (Unterrichtsthemen)" + "(KW " + weekOfYear + ")", activeTheme, school: true, isCreate: true, text: classes);
				}
				else
				{
					form = new EditForm("Berufsschule (Unterrichtsthemen)" + "(KW " + weekOfYear + ")", activeTheme, text: client.getHolidaysForDate(baseDate), isCreate: true);
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
				string name = configHandler.NamingPattern().Replace(NamingPatternResolver.CalendarWeek, weekOfYear.ToString()).Replace(NamingPatternResolver.ReportNumber, configHandler.ReportNumber());
				string path = ActivePath + "\\" + today.Year + "\\" + name + ".docx";
				SetFontInDoc(ldoc, app);
				ldoc.SaveAs2(FileName: path);

				if (int.TryParse(configHandler.ReportNumber(), out int i)) configHandler.ReportNumber("" + (i + 1));
				configHandler.LastReportKW(culture.Calendar.GetWeekOfYear(today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday));
				configHandler.LastCreated(path);
				configHandler.SaveConfig();
				miEditLatest.Enabled = true;
				MessageBox.Show("Created Document at: " + path);

				ldoc.Close();
				UpdateTree();
			}
			catch (Exception ex)
			{
				switch (ex.HResult)
				{
					case -2147023174:
						MessageBox.Show("an unexpected problem occured this progam will now close!");
						break;
					case -2147467262:
					case -2146823679:
						MessageBox.Show("Word closed unexpectedly and is restarting please try again shortly");
						RestartWord();
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
						MessageBox.Show("Connection refused by remotehost");
						break;
					default:
						Logger.LogError(ex);
						MessageBox.Show(ex.StackTrace);
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

		/**
		 * <summary>
		 * Used for detecting missing reports and initiating their creation
		 * </summary>
		 * 
		*/
		private void CreateMissing(bool vacation = false)
		{
			DateTimeFormatInfo dfi = culture.DateTimeFormat;
			DateTime date1 = new DateTime(DateTime.Today.Year, 12, 31);

			int weekOfYear = culture.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
			int reportNr = configHandler.LastReportKW();

			if (configHandler.LastReportKW() < weekOfYear)
			{
				//Missing reports in current year
				DateTime today = DateTime.Today.AddDays(-(weekOfYear - reportNr) * 7);
				for (int i = 1; i < weekOfYear - reportNr; i++)
				{
					CreateDocument(configHandler.TemplatePath(), today.AddDays(i * 7), wordApp, vacation: vacation);
				}
			}
			else
			{
				//Missing missing reports over multiple years
				int nrOfWeeksLastYear = culture.Calendar.GetWeekOfYear(new DateTime(DateTime.Today.Year - 1, 12, 31), dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
				int weekOfCurrentYear = culture.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);

				DateTime endOfLastYear = new DateTime(DateTime.Today.Year - 1, 12, 31);

				int repeats = nrOfWeeksLastYear - reportNr + weekOfCurrentYear;

				DateTime today = DateTime.Today.AddDays(-(repeats * 7));

				//Generate reports for missing reports over 2 years
				for (int i = 1; i < repeats; i++)
				{
					CreateDocument(configHandler.TemplatePath(), today.AddDays(i * 7), wordApp, vacation: vacation);
				}
			}
		}

		private void btCreate_Click(object sender, EventArgs e)
		{
			if (!HasWordStarted()) return;

			//Check if report for this week was already created
			int currentWeek = culture.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
			if (File.Exists(ActivePath + "\\" + DateTime.Today.Year + "\\WochenberichtKW" + currentWeek + ".docx") || File.Exists(ActivePath + "\\" + DateTime.Today.Year + "\\Gedruckt\\WochenberichtKW" + currentWeek + ".docx"))
			{
				MessageBox.Show("A report has already been created for this week");
				return;
			}
			//Check if a report was created
			if (configHandler.LastReportKW() > 0)
			{
				//Check if report for last week was created
				if (getDistanceToToday() > 1)
				{
					if (MessageBox.Show("You missed some reports were you on vacation?", "Vacation?", MessageBoxButtons.YesNo) == DialogResult.Yes)
					{
						CreateMissing(vacation: true);
					}
					else
					{
						if (MessageBox.Show("Do you want to create empty reports then?", "Create?", MessageBoxButtons.YesNo) == DialogResult.Yes)
						{
							CreateMissing();
						}
					}
				}
			}

			CreateDocument(configHandler.TemplatePath(), baseDate: DateTime.Today, wordApp, isSingle: true);
		}

		/// <summary>
		/// Returns the distance between the last created date in the config and today
		/// </summary>
		/// <returns>The number of weeks since last report creation</returns>
		private int getDistanceToToday()
		{
			int lastReportKW = configHandler.LastReportKW();
			int todaysWeek = culture.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
			//Both weeks are in the same year
			if (lastReportKW <= todaysWeek)
			{
				return todaysWeek - lastReportKW;
			}
			//Both weeks are in different years
			else
			{
				int lastWeekOfLastYear = culture.Calendar.GetWeekOfYear(new DateTime(DateTime.Today.Year - 1, 12, 31), CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
				return lastWeekOfLastYear - lastReportKW + todaysWeek;
			}
		}

		private void btEdit_Click(object sender, EventArgs e)
		{
			if (!HasWordStarted()) return;

			if (DocIsSamePathAsSelected())
				return;
			SaveOrExit();
			if (configHandler.LegacyEdit())
				Edit(configHandler.LastCreated());
			else
				EditInTb(configHandler.LastCreated());
		}

		private void btPrint_Click(object sender, EventArgs e)
		{
			if (!HasWordStarted()) return;

			if (tvReports.SelectedNode == null)
			{
				MessageBox.Show("No report selected");
				return;
			}
			PrintDocument(FullSelectedPath);
		}

		private void btPrintAll_Click(object sender, EventArgs e)
		{
			if (!HasWordStarted()) return;

			if (Directory.Exists(ActivePath))
			{
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

				PrintDialog printDialog = new PrintDialog();
				if (printDialog.ShowDialog() == DialogResult.OK)
				{
					if (unPrintedFiles.Count == 0)
					{
						MessageBox.Show("No unprinted reports found");
						return;
					}
					else
					{
						foreach (string key in unPrintedFiles.Keys)
						{
							if (unPrintedFiles[key].Contains(configHandler.LastCreated()))
							{
								if (MessageBox.Show("Do you want to also print the last created report?\n(" + configHandler.LastCreated() + ")", "Print last created?", MessageBoxButtons.YesNo) == DialogResult.No)
								{
									unPrintedFiles[key].Remove(configHandler.LastCreated());
								}
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
								if (doc != null)
									isSameAsOpened = filePath == doc.Path + "\\" + doc.Name;
								else isSameAsOpened = false;
								if (isSameAsOpened)
								{
									SaveOrExit();
									rtbSchool.Text = "";
									rtbWork.Text = "";
									wasEdited = false;
								};
								Word.Document document = wordApp.Documents.Open(FileName: filePath, ReadOnly: true);
								wordApp.Visible = WordVisible;
								document.PrintOut(Background: false);
								document.Close();
								File.Move(filePath, key + "\\Gedruckt\\" + Path.GetFileName(filePath));
							}
							catch (Exception ex)
							{
								Logger.LogError(ex);
								MessageBox.Show(ex.StackTrace, "Error while printing" + filePath);
								Console.Write(ex.StackTrace);
							}
						});
					}
					UpdateTree();
				}
			}
		}

		/**
		<summary>
		Method for editing a Word document at a path relative to the working directory
		</summary> 
		<param name="path">The path relative to the working directory</param>
		<param name="quickEditFieldNr">The number of the field to quick edit</param>
		<param name="quickEditTitle">Title of the editor window</param>
		*/
		public void Edit(string path, int quickEditFieldNr = -1, string quickEditTitle = "")
		{
			try
			{
				if (File.Exists(path))
				{
					if (Path.GetExtension(path) != ".docx" || Path.GetFileName(path).StartsWith("~$"))
						return;
					if (!DocIsSamePathAsSelected())
						doc = wordApp.Documents.Open(path);

					if (doc.FormFields.Count != 10)
					{
						MessageBox.Show("Invalid document (you will have to manually edit)");
						doc.Close(SaveChanges: false);
						doc = null;
						return;
					}

					if (quickEditFieldNr > -1)
					{
						IEnumerator enumerator = doc.FormFields.GetEnumerator();
						for (int i = 0; i < quickEditFieldNr; i++)
						{
							if (enumerator.MoveNext())
							{

							}
						}
						EditForm edit = new EditForm(quickEditTitle, activeTheme, text: ((Word.FormField)enumerator.Current).Result);
						edit.RefreshConfigs += RefreshConfig;
						if (edit.ShowDialog() == DialogResult.OK)
						{
							FillText(wordApp, (Word.FormField)enumerator.Current, edit.Result);
						}
						else
						{
							if (edit.DialogResult == DialogResult.Ignore)
							{
								FillText(wordApp, (Word.FormField)enumerator.Current, edit.Result);
							}
						}
						edit.RefreshConfigs -= RefreshConfig;
					}
					else
					{
						SelectEditFrom selectEdit = new SelectEditFrom(activeTheme);
						if (selectEdit.ShowDialog() == DialogResult.OK)
						{
							IEnumerator enumerator = doc.FormFields.GetEnumerator();
							EditForm edit;
							foreach (EditState si in selectEdit.SelectedItems)
							{
								if (enumerator.MoveNext())
								{
									if (si.ShouldEdit)
									{
										edit = new EditForm(si.EditorTitle, activeTheme, text: ((Word.FormField)enumerator.Current).Result);
										edit.RefreshConfigs += RefreshConfig;
										edit.ShowDialog();
										edit.RefreshConfigs -= RefreshConfig;
										if (edit.DialogResult == DialogResult.OK)
										{
											FillText(wordApp, (Word.FormField)enumerator.Current, edit.Result);
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
													FillText(wordApp, (Word.FormField)enumerator.Current, edit.Result);
													break;
												}
											}
										}
									}
								}
							}
						}
					}
					SetFontInDoc(doc, wordApp);
					doc.Save();
					rtbWork.Text = doc.FormFields[6].Result;
					rtbSchool.Text = doc.FormFields[8].Result;
					editMode = true;
					wasEdited = false;
					MessageBox.Show("Saved changes", "Saved");
				}
				else
				{
					MessageBox.Show(path + " not found was it deleted or moved?");
				}
			}
			catch (Exception ex)
			{
				switch (ex.HResult)
				{
					case -2147023174:
						MessageBox.Show("an unexpected problem occured this progam will now close!");
						break;
					case -2147467262:
					case -2146823679:
						MessageBox.Show("Word closed unexpectedly and is restarting please try again shortly");
						RestartWord();
						break;
					case -2146822750:
						//Document is only one page long
						/*doc.Save();
						doc.Close();
						doc = null;*/
						break;
					case -2146233088:
						MessageBox.Show("Connection refused by remotehost");
						break;
					default:
						Logger.LogError(ex);
						MessageBox.Show(ex.StackTrace);
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
			if (doc != null)
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
				doc = wordApp.Documents.Open(path);
				if (doc.FormFields.Count != 10)
				{
					MessageBox.Show("Invalid document (you will have to manually edit)");
					doc.Close(SaveChanges: false);
					doc = null;
					editMode = false;
					wasEdited = false;
					return;
				}
				rtbWork.Text = doc.FormFields[6].Result;
				rtbSchool.Text = doc.FormFields[8].Result;
				editMode = true;
				wasEdited = false;
			}
			catch (Exception ex)
			{
				switch (ex.HResult)
				{
					case -2147023174:
						MessageBox.Show("an unexpected problem occured this progam will now close!");
						break;
					case -2147467262:
					case -2146823679:
						MessageBox.Show("Word closed unexpectedly and is restarting please try again shortly");
						RestartWord();
						break;
					case -2146822750:
						//Document is only one page long
						/*doc.Save();
						doc.Close();
						doc = null;*/
						break;
					case -2146233088:
						MessageBox.Show("Connection refused by remotehost");
						break;
					default:
						Logger.LogError(ex);
						MessageBox.Show(ex.StackTrace);
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
				if (doc == null || wordApp == null)
					return;
				FillText(wordApp, doc.FormFields[6], rtbWork.Text);
				FillText(wordApp, doc.FormFields[8], rtbSchool.Text);
				SetFontInDoc(doc, wordApp);
				doc.Save();
				MessageBox.Show("Saved changes", "Saved");
				wasEdited = false;
			}
			catch (Exception ex)
			{
				switch (ex.HResult)
				{
					case -2147023174:
						MessageBox.Show("an unexpected problem occured this progam will now close!");
						break;
					case -2147467262:
					case -2146823679:
						MessageBox.Show("Word closed unexpectedly and is restarting please try again shortly");
						RestartWord();
						break;
					case -2146822750:
						//Document is one page already
						break;
					case -2146233088:
						MessageBox.Show("Connection refused by remotehost");
						break;
					default:
						Logger.LogError(ex);
						MessageBox.Show(ex.StackTrace);
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
			if (doc == null)
				return;
			if (!editMode)
				return;
			if (DocIsSamePathAsSelected())
				return;
			if (!wasEdited)
			{
				doc.Close(SaveChanges: false);
				doc = null;
				return;
			}

			if (MessageBox.Show("Save unsaved changes?", "Save?", MessageBoxButtons.YesNo) == DialogResult.Yes)
				SaveFromTb();
			doc.Close(SaveChanges: false);
			doc = null;
			editMode = false;
			wasEdited = false;
		}

		/// <summary>
		/// Checks if the document path is the same as the selected report path
		/// </summary>
		/// <returns>is document path same as selected report path</returns>
		private bool DocIsSamePathAsSelected()
		{
			if (doc == null)
				return false;
			try
			{
				string path = doc.Path;
			}
			catch
			{
				return false;
			}
			return FullSelectedPath == doc.Path + "\\" + doc.Name;
		}

		/**
		<summary>
		Method for printing a document located at the path
		</summary> 
		<param name="path">The path</param>
		*/
		private void PrintDocument(string path)
		{
			if (Path.GetExtension(path) != ".docx")
			{
				MessageBox.Show("You may only print Documents(*.docx) files");
				return;
			}
			DirectoryInfo printed = new DirectoryInfo(Path.GetDirectoryName(path));
			if (printed.Name == "Gedruckt")
			{
				if (MessageBox.Show("Report was already printed do you want to print it again?", "Reprint?", MessageBoxButtons.YesNo) != DialogResult.Yes)
				{
					return;
				}
			}
			PrintDialog printDialog = new PrintDialog();
			if (printDialog.ShowDialog() == DialogResult.OK)
			{
				if (File.Exists(path))
				{
					if (!Directory.Exists(path.Substring(0, path.Length - Path.GetFileName(path).Length) + "\\Gedruckt"))
					{
						Directory.CreateDirectory(path.Substring(0, path.Length - Path.GetFileName(path).Length) + "\\Gedruckt");
					}
					try
					{
						bool isSameAsOpened;
						if (doc != null)
							isSameAsOpened = path == doc.Path + "\\" + doc.Name;
						else isSameAsOpened = false;
						if (isSameAsOpened)
						{
							SaveOrExit();
							rtbSchool.Text = "";
							rtbWork.Text = "";
							wasEdited = false;
						};
						Word.Document document = wordApp.Documents.Open(path, ReadOnly: true);
						wordApp.Visible = WordVisible;
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
						MessageBox.Show(ex.StackTrace);
						Console.Write(ex.StackTrace);
					}
				}
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
				MessageBox.Show(path + " not Found (was it moved or deleted?)");
				return;
			}
			if (File.GetAttributes(path) == FileAttributes.Directory)
			{
				MessageBox.Show("You may not delete folders using the manager");
				return;
			}
			if (Path.GetFileName(path).StartsWith("~$"))
			{
				File.Delete(path);
				UpdateTree();
				MessageBox.Show(path + " was deleted successfully!", "File was deleted");
				return;
			}
			if (Path.GetExtension(path) != ".docx" && !path.Contains("\\Logs"))
			{
				MessageBox.Show("You may only delete Word documents (*.docx) or their temporary files");
				return;
			}
			if (MessageBox.Show("Are you sure you want to delete the selected file?", "Delete?", MessageBoxButtons.YesNo) != DialogResult.Yes)
				return;
			if (path == configHandler.LastCreated())
			{
				if (configHandler.LastReportKW() == culture.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday))
				{
					if (int.TryParse(configHandler.ReportNumber(), out int number))
					{
						configHandler.ReportNumber("" + (number - 1));
						configHandler.LastReportKW(culture.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday) - 1);
						configHandler.SaveConfig();
					}
					else
					{
						MessageBox.Show("Could not reset current number of report");
					}
				}
			}
			File.Delete(path);
			UpdateTree();
			MessageBox.Show("File deleted successfully");
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
			if (configHandler.LegacyEdit())
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
			int tabStops = configHandler.TabStops();
			OptionMenu optionMenu = new OptionMenu(configHandler, activeTheme, themeManager);
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
			info = new DirectoryInfo(folderPath);
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
			activeTheme = theme;
			ThemeSetter.SetThemes(this, theme);
			nodeDrawer.SetTheme(activeTheme);
			tvReports.Refresh();
		}

		private void DetectKeys(object sender, KeyEventArgs e)
		{
			if (e.Control && e.KeyCode == Keys.S && editMode)
			{
				SaveFromTb();
			}
		}

		private void EditRichTextBox(object sender, EventArgs e)
		{
			wasEdited = true;
		}

		private void miWordVisible_Click(object sender, EventArgs e)
		{
			if (WordInitialized)
				wordApp.Visible = miWordVisible.Checked;
			WordVisible = miWordVisible.Checked;
		}

		private void FormManager_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				if (editMode && wasEdited && MessageBox.Show("Do you want to save unsaved changes?", "Save changes?", MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					SaveFromTb();
				}
				if (doc != null)
				{
					doc.Close(SaveChanges: false);
					doc = null;
				}
			}
			catch (Exception ex)
			{
				Console.Write(ex.StackTrace);
			}
		}

		private void tvReports_DrawNode(object sender, DrawTreeNodeEventArgs e)
		{
			if (e.Bounds.Width < 1 || e.Bounds.Height < 1)
				return;
			nodeDrawer.DrawNode(e);
		}

		private void menuStrip1_Paint(object sender, PaintEventArgs e)
		{
			int versionNumberWidth = (int)e.Graphics.MeasureString(VersionNumber, menuStrip1.Font).Width / 2;
			TextRenderer.DrawText(e.Graphics, VersionString, menuStrip1.Font, new Point(e.ClipRectangle.X + e.ClipRectangle.Width / 2 - versionNumberWidth, e.ClipRectangle.Y + menuStrip1.Padding.Top + 2), menuStrip1.ForeColor);
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
					if (doc == null || !editMode)
					{
						MessageBox.Show("No opened report to close", "Could not close");
						return;
					}
					SaveOrExit();
					doc.Close(SaveChanges: false);
					doc = null;
					rtbSchool.Text = "";
					rtbWork.Text = "";
					wasEdited = false;
					editMode = false;
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
				MessageBox.Show("Word is still starting, please try again", "Please try again");
			}
			return WordInitialized;
		}

		private void miRevealInExplorer_Click(object sender, EventArgs e)
		{
			if (Directory.Exists(ActivePath))
				Process.Start(ActivePath);
			else
				MessageBox.Show("The working directory has been deleted from an external source", "You may have a problem");
		}

		/// <summary>
		/// Reloads <see cref="configHandler"/>
		/// </summary>
		public void RefreshConfig()
		{
			configHandler.ReloadConfig();
		}

		/// <summary>
		/// Restarts word if it has been closed
		/// </summary>
		private void RestartWord()
		{
			if (wordApp == null)
			{
				wordApp = new Word.Application();
				WordInitialized = true;
				return;
			}

			//Check if word is still open
			if (!typeof(Word.Application).IsAssignableFrom(wordApp.GetType()))
				return;
			WordInitialized = false;
			//try
			//{
			//	wordApp.Quit(SaveChanges: false);
			//}
			//catch
			//{

			//}
			wordApp = new Word.Application();
			WordInitialized = true;
		}

		/// <summary>
		/// Changes font size of editing text boxes
		/// </summary>
		/// <param name="fontSize">Size to set font to</param>
		private void ChangeFontSize(float fontSize)
		{
			bool wasEdited = this.wasEdited;
			rtbSchool.Font = new Font(rtbSchool.Font.FontFamily, fontSize);
			rtbWork.Font = new Font(rtbWork.Font.FontFamily, fontSize);
			this.wasEdited = wasEdited;
		}

		private void miCloseReport_Click(object sender, EventArgs e)
		{
			if (doc == null || !editMode)
			{
				MessageBox.Show("No opened report to close", "Could not close");
				return;
			}
			SaveOrExit();
			doc.Close(SaveChanges: false);
			doc = null;
			rtbSchool.Text = "";
			rtbWork.Text = "";
			wasEdited = false;
			editMode = false;
		}
	}
}
