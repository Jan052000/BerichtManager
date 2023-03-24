using System;
using System.IO;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;
using BerichtManager.Config;
using BerichtManager.AddForm;
using System.Globalization;
using System.Collections.Generic;
using System.Drawing;
using System.Collections;
using BerichtManager.OptionsMenu;
using BerichtManager.HelperClasses;
using BerichtManager.ThemeManagement;
using BerichtManager.ThemeManagement.DefaultThemes;

namespace BerichtManager
{
	public partial class FormManager : Form
	{
		private Word.Document doc = null;
		private Word.Application wordApp = null;
		private readonly ConfigHandler configHandler = new ConfigHandler(themeManager);
		private readonly Client client;
		private readonly DirectoryInfo info = new DirectoryInfo(Path.GetFullPath(".\\.."));
		private bool visible = false;
		private readonly CultureInfo culture = new CultureInfo("de-DE");
		private int tvReportsMaxWidth = 50;
		private bool editMode = false;
		private bool wasEdited = false;
		private CustomNodeDrawer nodeDrawer;
		private static readonly ThemeManager themeManager = new ThemeManager();
		private ITheme activeTheme;

		public FormManager()
		{
			InitializeComponent();
			nodeDrawer = new CustomNodeDrawer(ilTreeViewIcons);
			foreach (Control control in this.Controls)
				control.KeyDown += DetectKeys;
			this.Icon = Icon.ExtractAssociatedIcon(Path.GetFullPath(".\\BerichtManager.exe"));
			tvReports.TreeViewNodeSorter = new HelperClasses.TreeNodeSorter();
			UpdateTree();
			if (configHandler.LoadActive() == "")
			{
				miEditLatest.Enabled = false;
			}
			SetComponentPositions();
			activeTheme = themeManager.GetTheme(configHandler.ActiveTheme());
			if (activeTheme == null)
				activeTheme = new DarkMode();
			ThemeSetter.SetThemes(this, activeTheme);
			client = new Client(configHandler);
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
			splitterTreeBoxes.SplitPosition = tvReportsMaxWidth + 1;
			Rectangle bounds = scTextBoxes.Bounds;
			bounds.X = paFileTree.Bounds.Right + 1;
			bounds.Width = Width - 1 - paFileTree.Bounds.Width;
			scTextBoxes.Bounds = bounds;
		}

		/// <summary>
		/// Calculates the max size the tree view should have
		/// </summary>
		/// <param name="treeNode">root nodes of tree</param>
		private void TvReportsMaxWidth(TreeNode treeNode)
		{
			foreach(TreeNode node in treeNode.Nodes)
			{
				if(node.Nodes.Count > 0)
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
		 * Sets the global font in a document
		 * </summary>
		 * <param name="app">The Wordapp containing the document</param>
		 * <param name="doc">The Document which needs a font change</param>
		*/
		private void SetFontInDoc(Word.Document doc, Word.Application app)
		{
			doc.Content.Select();
			if (app.Selection.Font.Name != configHandler.LoadFont())
			{
				app.Selection.Font.Name = configHandler.LoadFont();
				MessageBox.Show("Changed report Font to: " + configHandler.LoadFont(), "Font changed!");
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
			try
			{
				if (File.Exists(templatePath))
				{
					int weekOfYear = culture.Calendar.GetWeekOfYear(baseDate, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
					doc = app.Documents.Add(Template: templatePath);

					if (doc.FormFields.Count != 10)
					{
						MessageBox.Show("Invalid template");
						doc.Close(SaveChanges: false);
						doc = null;
						app.Quit(SaveChanges: false);
						return;
					}

					EditForm form;
					//Fill name
					IEnumerator enumerator = doc.FormFields.GetEnumerator();
					enumerator.MoveNext();
					if (!string.IsNullOrEmpty(configHandler.LoadName()))
					{
						((Word.FormField)enumerator.Current).Result = configHandler.LoadName();
					}
					else
					{
						form = new EditForm("Enter your name", activeTheme, text: "Name Vorname");
						if (form.ShowDialog() == DialogResult.OK)
						{
							configHandler.SaveName(form.Result);
							((Word.FormField)enumerator.Current).Result = configHandler.LoadName();
						}
						else
						{
							MessageBox.Show("Cannot proceed without a name!", "Name required!");
							return;
						}
					}
					enumerator.MoveNext();

					//Enter report nr.
					if (int.TryParse(configHandler.LoadNumber(), out int number))
					{
						FillText(app, ((Word.FormField)enumerator.Current), (number + reportDifference).ToString());
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
						FillText(app, (Word.FormField)enumerator.Current, "-Urlaub");
					}
					else
					{
						form = new EditForm("Betriebliche Tätigkeiten" + "(KW " + weekOfYear + ")", activeTheme, isCreate: true);
						form.ShowDialog();
						if (form.DialogResult == DialogResult.OK)
						{
							FillText(app, (Word.FormField)enumerator.Current, form.Result);
						}
						else
						{
							if (form.DialogResult == DialogResult.Abort)
							{
								doc.Close(Word.WdSaveOptions.wdDoNotSaveChanges);
								doc = null;
								if (isSingle)
									app.Quit(SaveChanges: false);
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
						FillText(app, (Word.FormField)enumerator.Current, "-Urlaub");
					}
					else
					{
						form = new EditForm("Unterweisungen, betrieblicher Unterricht, sonstige Schulungen" + "(KW " + weekOfYear + ")", activeTheme, text: "-Keine-", isCreate: true);
						form.ShowDialog();
						if (form.DialogResult == DialogResult.OK)
						{
							FillText(app, (Word.FormField)enumerator.Current, form.Result);
						}
						else
						{
							if (form.DialogResult == DialogResult.Abort)
							{
								doc.Close(Word.WdSaveOptions.wdDoNotSaveChanges);
								doc= null;
								if (isSingle)
									app.Quit(SaveChanges: false);
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
							HelperClasses.Logger.LogError(e);
						}
						form = new EditForm("Berufsschule (Unterrichtsthemen)" + "(KW " + weekOfYear + ")", activeTheme, school: true, isCreate: true, text: classes);
					}
					else
					{
						form = new EditForm("Berufsschule (Unterrichtsthemen)" + "(KW " + weekOfYear + ")", activeTheme, text: client.getHolidaysForDate(baseDate), isCreate: true);
					}
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						FillText(app, (Word.FormField)enumerator.Current, form.Result);
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							doc.Close(Word.WdSaveOptions.wdDoNotSaveChanges);
							doc = null;
							if (isSingle)
								app.Quit(SaveChanges: false);
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


					Directory.CreateDirectory(Path.GetFullPath(Environment.CurrentDirectory + "\\..") + "\\" + today.Year);
					string path = Path.GetFullPath(".\\..\\" + today.Year) + "\\WochenberichtKW" + weekOfYear + ".docx";
					SetFontInDoc(doc, app);
					doc.SaveAs2(FileName: path);

					if (int.TryParse(configHandler.LoadNumber(), out int i)) configHandler.EditNumber("" + (i + 1));
					configHandler.SaveLastReportKW(culture.Calendar.GetWeekOfYear(today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday));
					configHandler.EditActive(path);
					miEditLatest.Enabled = true;
					MessageBox.Show("Created Document at: " + Path.GetFullPath(".\\..\\" + today.Year) + "\\WochenberichtKW" + weekOfYear + ".docx");

					doc.Close();
					doc = null;
					UpdateTree();

					//If single report close app
					if (isSingle)
					{
						try
						{
							app.Quit(SaveChanges: false);
						}
						catch 
						{
							
						}
					}
				}
				else
				{
					MessageBox.Show(configHandler.LoadPath() + " was not found was it moved or deleted?");
				}
			}
			catch (Exception ex)
			{
				switch (ex.HResult)
				{
					case -2147023174:
						MessageBox.Show("an unexpected problem occured this progam will now close!");
						break;
					case -2146823679:
						MessageBox.Show("Word closed unexpectedly");
						break;
					case -2146822750:
						//Document already fit on page
						doc.Save();
						doc.Close();
						doc = null;
						break;
					case -2146233088:
						MessageBox.Show("Connection refused by remotehost");
						break;
					default:
						HelperClasses.Logger.LogError(ex);
						MessageBox.Show(ex.StackTrace);
						try
						{
							app.Quit(false);
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
			int reportNr = configHandler.LoadLastReportKW();

			wordApp = new Word.Application();
			wordApp.Visible = visible;

			if (configHandler.LoadLastReportKW() < weekOfYear)
			{
				//Missing reports in current year
				DateTime today = DateTime.Today.AddDays(-(weekOfYear - reportNr) * 7);
				for (int i = 1; i < weekOfYear - reportNr; i++)
				{
					CreateDocument(configHandler.LoadPath(), today.AddDays( i * 7), wordApp, vacation: vacation);
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
					CreateDocument(configHandler.LoadPath(), today.AddDays(i * 7), wordApp, vacation: vacation);
				}
			}
			try
			{
				wordApp.Quit(SaveChanges: false);
				wordApp = null;
			}
			catch
			{

			}
		}

		private void btCreate_Click(object sender, EventArgs e)
		{
			//Check if report for this week was already created
			int currentWeek = culture.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
			if (File.Exists(Path.GetFullPath(".\\..\\" + DateTime.Today.Year) + "\\WochenberichtKW" + currentWeek + ".docx") || File.Exists(Path.GetFullPath(".\\..\\" + DateTime.Today.Year) + "\\Gedruckt\\WochenberichtKW" + currentWeek + ".docx"))
			{
				MessageBox.Show("A report has already been created for this week");
				return;
			}
			//Check if a report was created
			if (configHandler.LoadLastReportKW() > 0)
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

			wordApp = new Word.Application();
			wordApp.Visible = visible;
			CreateDocument(configHandler.LoadPath(), baseDate: DateTime.Today, wordApp, isSingle: true);
		}

		/// <summary>
		/// Returns the distance between the last created date in the config and today
		/// </summary>
		/// <returns>The number of weeks since last report creation</returns>
		private int getDistanceToToday()
		{
			int lastReportKW = configHandler.LoadLastReportKW();
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
			if (DocIsSamePathAsSelected())
				return;
			SaveOrExit();
			if (configHandler.LegacyEdit())
				Edit(configHandler.LoadActive());
			else
				EditInTb(configHandler.LoadActive());
		}

		private void btPrint_Click(object sender, EventArgs e)
		{
			if (tvReports.SelectedNode == null)
			{
				MessageBox.Show("No report selected");
				return;
			}
			PrintDocument(tvReports.SelectedNode.FullPath);
		}

		private void btPrintAll_Click(object sender, EventArgs e)
		{
			if (Directory.Exists(Path.GetFullPath(".\\..")))
			{
				Dictionary<string, List<string>> unPrintedFiles = new Dictionary<string, List<string>>();
				foreach (string dirName in Directory.GetDirectories(Path.GetFullPath(".\\..")))
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
							if (unPrintedFiles[key].Contains(configHandler.LoadActive()))
							{
								if (MessageBox.Show("Do you want to also print the last created report?\n(" + configHandler.LoadActive() + ")", "Print last created?", MessageBoxButtons.YesNo) == DialogResult.No)
								{
									unPrintedFiles[key].Remove(configHandler.LoadActive());
								}
							}
						}
					}
					try
					{
						wordApp = new Word.Application();
						wordApp.Visible = visible;
						foreach (string key in unPrintedFiles.Keys)
						{
							unPrintedFiles[key].ForEach((f) => 
							{
								Word.Document document = wordApp.Documents.Open(FileName: f, ReadOnly: true);
								document.PrintOut(Background: false);
								wordApp.Documents.Close();
							});
						}
						wordApp.Quit(SaveChanges: false);
						wordApp = null;

						foreach (string key in unPrintedFiles.Keys)
						{
							unPrintedFiles[key].ForEach((f) => 
							{
								File.Move(f, key + "\\Gedruckt\\" + Path.GetFileName(f));
							});
						}
						UpdateTree();
					}
					catch (Exception ex)
					{
						HelperClasses.Logger.LogError(ex);
						MessageBox.Show(ex.StackTrace);
						Console.Write(ex.StackTrace);
						try
						{
							wordApp.Quit(false);
							wordApp = null;
						}
						catch
						{

						}
					}

				}
			}
		}

		private void btEditExisting_Click(object sender, EventArgs e)
		{
			if (tvReports.SelectedNode == null)
			{
				MessageBox.Show("No report selected");
				return;
			}
			if (Path.GetExtension(Path.GetFullPath(".\\..\\..\\" + tvReports.SelectedNode.FullPath)) != ".docx")
			{
				MessageBox.Show("You may only edit Documents(*.docx) files");
			}
			SaveOrExit();
			Edit(Path.GetFullPath(".\\..\\..\\" + tvReports.SelectedNode.FullPath));
		}

		private void btDelete_Click(object sender, EventArgs e)
		{
			if (tvReports.SelectedNode == null)
			{
				MessageBox.Show("No report selected");
				return;
			}
			DeleteDocument(tvReports.SelectedNode.FullPath);
		}

		private void cbVisible_CheckedChanged(object sender, EventArgs e)
		{
			//visible = cbVisible.Checked;
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
					wordApp = new Word.Application();
					wordApp.Visible = visible;
					doc = wordApp.Documents.Open(path);

					if (doc.FormFields.Count != 10)
					{
						MessageBox.Show("Invalid document (you will have to manually edit)");
						doc.Close(SaveChanges: false);
						doc = null;
						wordApp.Quit();
						wordApp = null;
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
										edit.ShowDialog();
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
					doc.Close();
					doc = null;
					wordApp.Quit();
					wordApp = null;
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
					case -2146823679:
						MessageBox.Show("Word closed unexpectedly");
						break;
					case -2146822750:
						//Document is only one page long
						doc.Save();
						doc.Close();
						doc = null;
						wordApp.Quit();
						wordApp = null;
						break;
					case -2146233088:
						MessageBox.Show("Connection refused by remotehost");
						break;
					default:
						HelperClasses.Logger.LogError(ex);
						MessageBox.Show(ex.StackTrace);
						Console.Write(ex.StackTrace);
						break;
				}
				try
				{
					wordApp.Quit(false);
					wordApp = null;
				}
				catch
				{

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
				if (path == doc.Path)
					return;
			try
			{
				if (!File.Exists(path))
					return;
				if (Path.GetExtension(path) != ".docx" || Path.GetFileName(path).StartsWith("~$"))
					return;
				wordApp = new Word.Application();
				wordApp.Visible = visible;
				doc = wordApp.Documents.Open(path);
				if (doc.FormFields.Count != 10)
				{
					MessageBox.Show("Invalid document (you will have to manually edit)");
					doc.Close(SaveChanges: false);
					doc = null;
					wordApp.Quit();
					wordApp = null;
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
					case -2146823679:
						MessageBox.Show("Word closed unexpectedly");
						break;
					case -2146822750:
						//Document is only one page long
						doc.Save();
						doc.Close();
						doc = null;
						wordApp.Quit();
						wordApp = null;
						break;
					case -2146233088:
						MessageBox.Show("Connection refused by remotehost");
						break;
					default:
						HelperClasses.Logger.LogError(ex);
						MessageBox.Show(ex.StackTrace);
						Console.Write(ex.StackTrace);
						break;
				}
				try
				{
					wordApp.Quit(false);
					wordApp = null;
				}
				catch
				{

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
				if(doc == null || wordApp == null)
					return;
				FillText(wordApp, doc.FormFields[6], rtbWork.Text);
				FillText(wordApp, doc.FormFields[8], rtbSchool.Text);
				doc.Close(SaveChanges: true);
				MessageBox.Show("Saved changes", "Saved");
				wasEdited = false;
			}
			catch(Exception ex)
			{
				switch (ex.HResult)
				{
					case -2147023174:
						MessageBox.Show("an unexpected problem occured this progam will now close!");
						break;
					case -2146823679:
						MessageBox.Show("Word closed unexpectedly");
						break;
					case -2146822750:
						break;
					case -2146233088:
						MessageBox.Show("Connection refused by remotehost");
						break;
					default:
						HelperClasses.Logger.LogError(ex);
						MessageBox.Show(ex.StackTrace);
						Console.Write(ex.StackTrace);
						break;
				}
			}
		}

		/// <summary>
		/// Either saves the changes or quits word
		/// </summary>
		private void SaveOrExit()
		{
			if (!editMode)
				return;
			if (!wasEdited)
			{
				wordApp.Quit(SaveChanges: false);
				doc = null;
				wordApp = null;
				return;
			}
			if (DocIsSamePathAsSelected())
				return;

			if (MessageBox.Show("Save unsaved changes?", "Save?", MessageBoxButtons.YesNo) == DialogResult.Yes)
				SaveFromTb();
			else
			{
				wordApp.Quit(SaveChanges: false);
				doc = null;
				wordApp = null;
			}
		}

		/// <summary>
		/// Checks if the document path is the same as the selected report path
		/// </summary>
		/// <returns>is document path same as selected report path</returns>
		private bool DocIsSamePathAsSelected()
		{
			if (doc == null)
				return false;
			return Path.GetFullPath(".\\..\\..\\" + tvReports.SelectedNode.FullPath) == doc.Path + "\\" + doc.Name;
		}

		/**
		<summary>
		Method for printing a document contained at a path relative to the working directory
		</summary> 
		<param name="path">The path relative to the working directory</param>
		*/
		private void PrintDocument(string path)
		{
			if (Path.GetExtension(Path.GetFullPath(".\\..\\..\\" + path)) != ".docx")
			{
				MessageBox.Show("You may only print Documents(*.docx) files");
				return;
			}
			DirectoryInfo printed = new DirectoryInfo(Path.GetDirectoryName(Path.GetFullPath(".\\..\\..\\" + path)));
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
				if (File.Exists(Path.GetFullPath(".\\..\\..\\" + path)))
				{
					if (!Directory.Exists(Path.GetFullPath(".\\..\\..\\" + path).Substring(0, Path.GetFullPath(".\\..\\..\\" + path).Length - Path.GetFileName(".\\..\\..\\" + path).Length) + "\\Gedruckt"))
					{
						Directory.CreateDirectory(Path.GetFullPath(".\\..\\..\\" + path).Substring(0, Path.GetFullPath(".\\..\\..\\" + path).Length - Path.GetFileName(".\\..\\..\\" + path).Length) + "\\Gedruckt");
					}
					try
					{
						wordApp = new Word.Application();
						wordApp.Visible = visible;
						Word.Document document = wordApp.Documents.Open(Path.GetFullPath(".\\..\\..\\" + path), ReadOnly: true);
						document.PrintOut(Background: false);
						wordApp.Documents.Close();
						wordApp.Quit(false);
						wordApp = null;
						if (printed.Name != "Gedruckt")
						{
							File.Move(Path.GetFullPath(".\\..\\..\\" + path),
							Path.GetFullPath(".\\..\\..\\" + path).Substring(0, Path.GetFullPath(".\\..\\..\\" + path).Length - Path.GetFileName(".\\..\\..\\" + path).Length) + "\\Gedruckt\\" + Path.GetFileName(".\\..\\..\\" + path));
							UpdateTree();
						}
					}
					catch (Exception ex)
					{
						HelperClasses.Logger.LogError(ex);
						MessageBox.Show(ex.StackTrace);
						Console.Write(ex.StackTrace);
						try
						{
							wordApp.Quit(false);
							wordApp = null;
						}
						catch
						{

						}
					}
				}
			}
		}

		/**
		<summary>
		Method for deleting a file with a path relative to the working directory
		</summary> 
		<param name="path">The path relative to the working directory</param>
		*/
		private void DeleteDocument(string path)
		{
			if (File.GetAttributes(Path.GetFullPath(".\\..\\..\\" + path)) == FileAttributes.Directory)
			{
				MessageBox.Show("You may not delete folders using the manager");
				return;
			}
			if (MessageBox.Show("Are you sure you want to delete the selected file?", "Delete?", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				if (File.Exists(Path.GetFullPath(".\\..\\..\\" + path)))
				{
					if (Path.GetExtension(Path.GetFullPath(".\\..\\..\\" + path)) == ".docx" || Path.GetFileName(Path.GetFullPath(".\\..\\..\\" + path)).StartsWith("~$") || (path.Contains("Logs") && path.EndsWith(".txt")))
					{
						if (Path.GetFullPath(".\\..\\..\\" + path) == configHandler.LoadActive())
						{
							string[] split = path.Split('\\');
							if (split[split.Length - 1].Substring(15, ("" + culture.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday)).Length) == culture.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday).ToString())
							{
								if (int.TryParse(configHandler.LoadNumber(), out int number))
								{
									configHandler.EditNumber("" + (number - 1));
								}
								else
								{
									MessageBox.Show("Could not reset current number of report");
								}
							}
						}
						File.Delete(Path.GetFullPath(".\\..\\..\\" + path));
						UpdateTree();
						MessageBox.Show("File deleted successfully");
					}
					else
					{
						MessageBox.Show("You may only delete Documents(*.docx) or their temporary files");
					}
				}
				else
				{
					MessageBox.Show("File not Found (was it moved or deleted?)");
				}
			}
		}

		private void tvReports_DoubleClick(object sender, EventArgs e)
		{
			if (tvReports.SelectedNode == null)
				return;
			if (DocIsSamePathAsSelected())
				return;
			SaveOrExit();
			if (configHandler.LegacyEdit())
			{
				Edit(Path.GetFullPath(".\\..\\..\\" + tvReports.SelectedNode.FullPath));
			}
			else
			{
				EditInTb(Path.GetFullPath(".\\..\\..\\" + tvReports.SelectedNode.FullPath));
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
			DeleteDocument(tvReports.SelectedNode.FullPath);
		}

		private void miEdit_Click(object sender, EventArgs e)
		{
			SaveOrExit();
			Edit(Path.GetFullPath(".\\..\\..\\" + tvReports.SelectedNode.FullPath));
		}

		private void miPrint_Click(object sender, EventArgs e)
		{
			PrintDocument(tvReports.SelectedNode.FullPath);
		}

		private void miQuickEditWork_Click(object sender, EventArgs e)
		{
			SaveOrExit();
			Edit(Path.GetFullPath(".\\..\\..\\" + tvReports.SelectedNode.FullPath), quickEditFieldNr: 6, quickEditTitle: "Edit work");
		}

		private void miQuickEditSchool_Click(object sender, EventArgs e)
		{
			SaveOrExit();
			Edit(Path.GetFullPath(".\\..\\..\\" + tvReports.SelectedNode.FullPath), quickEditFieldNr: 8, quickEditTitle: "Edit school");
		}

		private void toRightClickMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			bool isInLogs = false;
			e.Cancel = true;
			if (tvReports.SelectedNode.Parent != null)
			{
				if (tvReports.SelectedNode.Parent.Text == "Logs")
				{
					isInLogs = true;
				}
			}
			if (tvReports.SelectedNode.Text.EndsWith(".docx") || isInLogs)
			{
				e.Cancel = false;
				miEdit.Enabled = true;
				miPrint.Enabled = true;
				miDelete.Enabled = true;
				miQuickEditOptions.Enabled = true;
				if (tvReports.SelectedNode.Text.StartsWith("~$") || isInLogs)
				{
					miEdit.Enabled = false;
					miPrint.Enabled = false;
					miQuickEditOptions.Enabled = false;
				}
			}
		}

		private void btOptions_Click(object sender, EventArgs e)
		{
			OptionMenu optionMenu = new OptionMenu(configHandler, activeTheme, themeManager);
			optionMenu.ShowDialog();
		}

		private void DetectKeys(object sender, KeyEventArgs e)
		{
			if(e.Control && e.KeyCode == Keys.S && editMode)
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
			visible = miWordVisible.Checked;
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
					doc.Close();
					doc = null;
				}
				if (wordApp != null)
				{
					wordApp.Quit(SaveChanges: false);
					wordApp = null;
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
	}
}
