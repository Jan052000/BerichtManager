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

namespace BerichtManager
{
	public partial class FormManager : Form
	{
		private Word.Document doc = null;
		private Word.Application wordApp = null;
		private readonly ConfigHandler handler = new ConfigHandler();
		private readonly Client client = new Client();
		private readonly DirectoryInfo info = new DirectoryInfo(Path.GetFullPath(".\\.."));
		private bool visible = false;
		private readonly CultureInfo culture = new CultureInfo("de-DE");

		public FormManager()
		{
			InitializeComponent();
			this.Icon = Icon.ExtractAssociatedIcon(Path.GetFullPath(".\\BerichtManager.exe"));
			//handler = new ConfigHandler();
			//client = new Client();
			UpdateTree();
			if (handler.LoadActive() == "") 
			{
				btEdit.Enabled = false;
			}
			//tvReports.ContextMenuStrip = toRightClickMenu;
		}

		/**
		 *<summary>Fills the TreeView of the main form with nodes of the upper directory</summary> 
		*/
		private void UpdateTree() 
		{
			tvReports.Nodes.Clear();
			tvReports.Nodes.Add(CreateDirectoryNode(info));
		}

		/**
		 * <summary>
		 * Generates TreeNodes from files and directorys contained in the upper directory
		 * </summary>
		 * <param name="directoryInfo">The target directory</param>
		 * <returns>A Treenode containing the files and folders in <c>directoryInfo</c></returns>
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
			if (app.Selection.Font.Name != handler.LoadFont()) 
			{
				app.Selection.Font.Name = handler.LoadFont();
				MessageBox.Show("Changed report Font to: " + handler.LoadFont(), "Font changed!");
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
				Word.Document doc = null;

				if (File.Exists(templatePath))
				{
					int weekOfYear = culture.Calendar.GetWeekOfYear(baseDate, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
					doc = app.Documents.Add(Template: templatePath);

					if (doc.FormFields.Count != 10)
					{
						MessageBox.Show("Invalid template");
						doc.Close(SaveChanges: false);
						app.Quit(SaveChanges: false);
						return;
					}

					EditForm form;
					//Fill name
					IEnumerator enumerator = doc.FormFields.GetEnumerator();
					enumerator.MoveNext();
					if (!string.IsNullOrEmpty(handler.LoadName()))
					{
						((Word.FormField)enumerator.Current).Result = handler.LoadName();
					}
					else
					{
						form = new EditForm("Enter your name", text: "Name Vorname");
						if (form.ShowDialog() == DialogResult.OK)
						{
							handler.SaveName(form.Result);
							((Word.FormField)enumerator.Current).Result = handler.LoadName();
						}
						else
						{
							MessageBox.Show("Cannot proceed without a name!", "Name required!");
							return;
						}
					}
					enumerator.MoveNext();

					//Enter report nr.
					if (int.TryParse(handler.LoadNumber(), out int number)) 
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
					((Word.FormField)enumerator.Current).Result = thisWeekEnd.ToString("dd.MM.yyyy");

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
						form = new EditForm("Betriebliche Tätigkeiten" + "(KW " + weekOfYear + ")", isCreate: true);
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
						form = new EditForm("Unterweisungen, betrieblicher Unterricht, sonstige Schulungen" + "(KW " + weekOfYear + ")", text: "-Keine-", isCreate: true);
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
						form = new EditForm("Berufsschule (Unterrichtsthemen)" + "(KW " + weekOfYear + ")", school: true, isCreate: true);
					}
					else 
					{
						form = new EditForm("Berufsschule (Unterrichtsthemen)" + "(KW " + weekOfYear + ")", text: client.getHolidaysForDate(baseDate), isCreate: true);
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

					if (int.TryParse(handler.LoadNumber(), out int i)) handler.EditNumber("" + (i + 1));
					handler.SaveLastReportKW(culture.Calendar.GetWeekOfYear(today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday));
					handler.EditActive(path);
					btEdit.Enabled = true;
					MessageBox.Show("Created Document at: " + Path.GetFullPath(".\\..\\" + today.Year) + "\\WochenberichtKW" + weekOfYear + ".docx");

					doc.Close();
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
					MessageBox.Show(handler.LoadPath() + " was not found was it moved or deleted?");
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
			try
			{
				if (doc != null) 
				{
					doc.Close();
				}
				if (wordApp != null) 
				{
					wordApp.Quit(SaveChanges: false);
				}
				Close();
			}
			catch (Exception ex) 
			{
				Console.Write(ex.StackTrace);
				Close();
			}
		}

		private void btSetTemplate_Click(object sender, EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = "Word Templates (*.dotx)|*.dotx";
			dialog.ShowDialog();
			handler.Save(Path.GetFullPath(dialog.FileName));
			MessageBox.Show("Muster auf: "+ Path.GetFullPath(dialog.FileName) + " gesetzt");
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

			Calendar cal = dfi.Calendar;
			int weekOfYear = culture.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
			int reportNr = handler.LoadLastReportKW();

			//Word.Application multipleApp = new Word.Application();
			//multipleApp.Visible = visible;
			wordApp = new Word.Application();
			wordApp.Visible = visible;

			if (handler.LoadLastReportKW() < weekOfYear)
			{
				//Missing reports in current year
				DateTime today = DateTime.Today.AddDays(-(weekOfYear - reportNr) * 7);
				for (int i = 1; i < weekOfYear - reportNr; i++)
				{
					//Console.WriteLine("Created report for week " + culture.Calendar.GetWeekOfYear(today.AddDays(i * (7)), CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday));
					CreateDocument(handler.LoadPath(), today.AddDays( i * 7), wordApp/*multipleApp*/, vacation: vacation);
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
					//Console.WriteLine("Creating report for week " + culture.Calendar.GetWeekOfYear(today.AddDays(i * (7)), CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday));
					CreateDocument(handler.LoadPath(), today.AddDays(i * 7), wordApp/*multipleApp*/, vacation: vacation);
				}
			}
			try
			{
				//multipleApp.Quit(SaveChanges: false);
				wordApp.Quit(SaveChanges: false);
			}
			catch
			{

			}
		}

		private void btCreate_Click(object sender, EventArgs e)
		{
			//Check if a report was created
			if (handler.LoadLastReportKW() > 0)
			{
				//Check if report for last week was created
				if (handler.LoadLastReportKW() > culture.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday) - 1)
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

			//Check if report for this week was already created
			int currentWeek = culture.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
			if (File.Exists(Path.GetFullPath(".\\..\\" + DateTime.Today.Year) + "\\WochenberichtKW" + currentWeek + ".docx") || File.Exists(Path.GetFullPath(".\\..\\" + DateTime.Today.Year) + "\\Gedruckt\\WochenberichtKW" + currentWeek + ".docx"))
			{
				MessageBox.Show("A report has already been created for this week");
				return;
			}
			wordApp = new Word.Application();
			wordApp.Visible = visible;
			CreateDocument(handler.LoadPath(), baseDate: DateTime.Today, wordApp/*new Word.Application { Visible = visible}*/, isSingle: true);
		}

		private void btSetNumber_Click(object sender, EventArgs e)
		{
			EditForm form = new EditForm("Edit Number of Report");
			form.ShowDialog();
			if (form.DialogResult == DialogResult.OK)
			{
				if (int.TryParse(form.Result, out int i))
				{
					handler.EditNumber(form.Result);
				}
				else 
				{
					MessageBox.Show("Entered value is not a number, the report number was thus not set");
				}
			}
		}

		private void btEdit_Click(object sender, EventArgs e)
		{
			Edit(handler.LoadActive());
		}

		private void btTest_Click(object sender, EventArgs e)
		{
			//Client form = new Client();
			//form.ShowDialog();
		}

		private void btPrint_Click(object sender, EventArgs e)
		{
			if (tvReports.SelectedNode == null) 
			{
				MessageBox.Show("No report selected");
				return;
			}
			PrintDocument(tvReports.SelectedNode.FullPath);
			/*if (Path.GetExtension(Path.GetFullPath(".\\..\\..\\" + tvReports.SelectedNode.FullPath)) != ".docx")
			{
				MessageBox.Show("You may only print Documents(*.docx) files");
				return;
			}
			DirectoryInfo printed = new DirectoryInfo(Path.GetDirectoryName(Path.GetFullPath(".\\..\\..\\" + tvReports.SelectedNode.FullPath)));
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
				Word.Application printApp = null;
				if (File.Exists(Path.GetFullPath(".\\..\\..\\" + tvReports.SelectedNode.FullPath)))
				{
					if (!Directory.Exists(Path.GetFullPath(".\\..\\..\\" + tvReports.SelectedNode.FullPath).Substring(0, Path.GetFullPath(".\\..\\..\\" + tvReports.SelectedNode.FullPath).Length - Path.GetFileName(".\\..\\..\\" + tvReports.SelectedNode.FullPath).Length) + "\\Gedruckt")) 
					{
						Directory.CreateDirectory(Path.GetFullPath(".\\..\\..\\" + tvReports.SelectedNode.FullPath).Substring(0, Path.GetFullPath(".\\..\\..\\" + tvReports.SelectedNode.FullPath).Length - Path.GetFileName(".\\..\\..\\" + tvReports.SelectedNode.FullPath).Length) + "\\Gedruckt");
					}
					try
					{
						printApp = new Word.Application();
						printApp.Visible = visible;
						Word.Document document = printApp.Documents.Open(Path.GetFullPath(".\\..\\..\\" + tvReports.SelectedNode.FullPath), ReadOnly: true);
						document.PrintOut(Background: false);
						printApp.Documents.Close();
						printApp.Quit(false);
						if (printed.Name != "Gedruckt") 
						{
							File.Move(Path.GetFullPath(".\\..\\..\\" + tvReports.SelectedNode.FullPath),
							Path.GetFullPath(".\\..\\..\\" + tvReports.SelectedNode.FullPath).Substring(0, Path.GetFullPath(".\\..\\..\\" + tvReports.SelectedNode.FullPath).Length - Path.GetFileName(".\\..\\..\\" + tvReports.SelectedNode.FullPath).Length) + "\\Gedruckt\\" + Path.GetFileName(".\\..\\..\\" + tvReports.SelectedNode.FullPath));
						}
						UpdateTree();
					}
					catch (Exception ex)
					{
						Console.Write(ex.Message);
						Console.Write("\n" + ex.StackTrace);
						try
						{
							printApp.Quit(false);
						}
						catch
						{

						}
					}
				}
			}*/
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
					//Word.Application printApp = null;
					if (unPrintedFiles.Count == 0)
					{
						MessageBox.Show("No unprinted reports found");
						return;
					}
					else 
					{
						foreach (string key in unPrintedFiles.Keys)
						{
							if (unPrintedFiles[key].Contains(handler.LoadActive()))
							{
								if (MessageBox.Show("Do you want to also print the last created report?\n(" + handler.LoadActive() + ")", "Print last created?", MessageBoxButtons.YesNo) == DialogResult.No)
								{
									unPrintedFiles[key].Remove(handler.LoadActive());
								}
							}
						}
					}
					try
					{
						//printApp = new Word.Application();
						//printApp.Visible = visible;
						wordApp = new Word.Application();
						wordApp.Visible = visible;
						foreach (string key in unPrintedFiles.Keys) 
						{
							unPrintedFiles[key].ForEach((f) => 
							{
								//Word.Document document = printApp.Documents.Open(FileName: f, ReadOnly: true);
								//document.PrintOut(Background: false);
								//printApp.Documents.Close();
								Word.Document document = wordApp.Documents.Open(FileName: f, ReadOnly: true);
								document.PrintOut(Background: false);
								wordApp.Documents.Close();
							});
						}
						//printApp.Quit(false);
						wordApp.Quit(SaveChanges: false);

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
						//Console.Write(ex.Message);
						//Console.Write("\n" + ex.StackTrace);
						try
						{
							//printApp.Quit(false);
							wordApp.Quit(false);
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
			/*if (MessageBox.Show("Are you sure you want to delete the selected file?", "Delete?", MessageBoxButtons.YesNo) == DialogResult.Yes) 
			{
				if (File.Exists(Path.GetFullPath(".\\..\\..\\" + tvReports.SelectedNode.FullPath)))
				{
					if (Path.GetExtension(Path.GetFullPath(".\\..\\..\\" + tvReports.SelectedNode.FullPath)) == ".docx" || Path.GetFileName(Path.GetFullPath(".\\..\\..\\" + tvReports.SelectedNode.FullPath)).StartsWith("~$"))
					{
						if (Path.GetFullPath(".\\..\\..\\" + tvReports.SelectedNode.FullPath) == handler.LoadActive())
						{
							if (tvReports.SelectedNode.Text.Substring(15, ("" + culture.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday)).Length) == culture.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday).ToString()) 
							{
								if (int.TryParse(handler.LoadNumber(), out int number))
								{
									handler.EditNumber("" + (number - 1));
								}
								else
								{
									MessageBox.Show("Could not reset current number of report");
								}
							}
						}
						File.Delete(Path.GetFullPath(".\\..\\..\\" + tvReports.SelectedNode.FullPath));
						//tvReports.Nodes.Remove(tvReports.SelectedNode);
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
			}*/
		}

		private void btLogin_Click(object sender, EventArgs e)
		{
			handler.doLogin();
		}

		private void btEditName_Click(object sender, EventArgs e)
		{
			EditForm form = new EditForm("Enter your name", text: "Name Vorname");
			if (form.ShowDialog() == DialogResult.OK)
			{
				if (form.Result != "Name Vorname") 
				{
					handler.SaveName(form.Result);
				}
			}

		}

		private void cbVisible_CheckedChanged(object sender, EventArgs e)
		{
			visible = cbVisible.Checked;
		}
		
		/**
		<summary>
		Method for editing a Word document at a path relative to the working directory
		</summary> 
		<param name="path">The path relative to the working directory</param>
		*/
		public void Edit(string path) 
		{
			try
			{
				if (File.Exists(path))
				{
					wordApp = new Word.Application();
					wordApp.Visible = visible;
					doc = wordApp.Documents.Open(path);

					if (doc.FormFields.Count != 10)
					{
						MessageBox.Show("Invalid document (you will have to manually edit)");
						doc.Close(SaveChanges: false);
						wordApp.Quit();
						return;
					}

					SelectEditFrom selectEdit = new SelectEditFrom();
					if (selectEdit.ShowDialog() == DialogResult.OK) 
					{
						IEnumerator enumerator = doc.FormFields.GetEnumerator();
						EditForm edit;
						foreach(EditState si in selectEdit.SelectedItems) 
						{
							if (enumerator.MoveNext()) 
							{
								if (si.ShouldEdit)
								{
									edit = new EditForm(si.EditorTitle, text: ((Word.FormField)enumerator.Current).Result);
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

					SetFontInDoc(doc, wordApp);

					doc.Save();
					doc.Close();
					wordApp.Quit();
				}
				else
				{
					MessageBox.Show(path + "not found was it deleted or moved?");
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
						wordApp.Quit();
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
				}
				catch
				{

				}
			}
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
				//Word.Application printApp = null;
				if (File.Exists(Path.GetFullPath(".\\..\\..\\" + path)))
				{
					if (!Directory.Exists(Path.GetFullPath(".\\..\\..\\" + path).Substring(0, Path.GetFullPath(".\\..\\..\\" + path).Length - Path.GetFileName(".\\..\\..\\" + path).Length) + "\\Gedruckt"))
					{
						Directory.CreateDirectory(Path.GetFullPath(".\\..\\..\\" + path).Substring(0, Path.GetFullPath(".\\..\\..\\" + path).Length - Path.GetFileName(".\\..\\..\\" + path).Length) + "\\Gedruckt");
					}
					try
					{
						//printApp = new Word.Application();
						//printApp.Visible = visible;
						//Word.Document document = printApp.Documents.Open(Path.GetFullPath(".\\..\\..\\" + path), ReadOnly: true);
						//document.PrintOut(Background: false);
						//printApp.Documents.Close();
						//printApp.Quit(false);
						wordApp = new Word.Application();
						wordApp.Visible = visible;
						Word.Document document = wordApp.Documents.Open(Path.GetFullPath(".\\..\\..\\" + path), ReadOnly: true);
						document.PrintOut(Background: false);
						wordApp.Documents.Close();
						wordApp.Quit(false);
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
						//Console.Write(ex.Message);
						//Console.Write("\n" + ex.StackTrace);
						try
						{
							//printApp.Quit(false);
							wordApp.Quit(false);
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
						if (Path.GetFullPath(".\\..\\..\\" + path) == handler.LoadActive())
						{
							string[] split = path.Split('\\');
							if (split[split.Length - 1].Substring(15, ("" + culture.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday)).Length) == culture.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday).ToString())
							{
								if (int.TryParse(handler.LoadNumber(), out int number))
								{
									handler.EditNumber("" + (number - 1));
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
			if (Path.GetExtension(Path.GetFullPath(".\\..\\..\\" + tvReports.SelectedNode.FullPath)) == ".docx")
			{
				Edit(Path.GetFullPath(".\\..\\..\\" + tvReports.SelectedNode.FullPath));
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
			Edit(Path.GetFullPath(".\\..\\..\\" + tvReports.SelectedNode.FullPath));
		}

		private void miPrint_Click(object sender, EventArgs e)
		{
			PrintDocument(tvReports.SelectedNode.FullPath);
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
			//((ContextMenuStrip)sender).Items.Clear();
			if (tvReports.SelectedNode.Text.EndsWith(".docx") || isInLogs)
			{
				e.Cancel = false;
				miEdit.Enabled = true;
				miPrint.Enabled = true;
				miDelete.Enabled = true;
				if (tvReports.SelectedNode.Text.StartsWith("~$") || isInLogs)
				{
					miEdit.Enabled = false;
					miPrint.Enabled = false;
				}
			}
			/*
			 e.Cancel = true;
			((ContextMenuStrip)sender).Items.Clear();
			if (tvReports.SelectedNode.Text.EndsWith(".docx"))
			{
				e.Cancel = false;
				if (!tvReports.SelectedNode.Text.StartsWith("~$"))
				{
					//ToolStripMenuItem miEdit = new ToolStripMenuItem("Edit");
					//miEdit.Click += new EventHandler(miEdit_Click);
					((ContextMenuStrip)sender).Items.Add(miEdit);
					//ToolStripMenuItem miPrint = new ToolStripMenuItem("Print");
					//miPrint.Click += new EventHandler(miPrint_Click);
					((ContextMenuStrip)sender).Items.Add(miPrint);
				}
				//ToolStripMenuItem miDelete = new ToolStripMenuItem("Delete");
				//miDelete.Click += new EventHandler(miDelete_Click);
				((ContextMenuStrip)sender).Items.Add(miDelete);
			}
			 */
		}
	}
}
