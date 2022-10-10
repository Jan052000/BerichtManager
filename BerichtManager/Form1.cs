using System;
using System.IO;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;
using System.Reflection;
using BerichtManager.Config;
using BerichtManager.AddForm;
using System.Globalization;
using System.Collections.Generic;
using System.Drawing;

namespace BerichtManager
{
	public partial class FormManager : Form
	{
		private Word.Document doc = null;
		private Word.Application wordApp = null;
		private ConfigHandler handler;
		private DirectoryInfo info = new DirectoryInfo(Path.GetFullPath(".\\.."));
		private bool visible = false;

		public FormManager()
		{
			InitializeComponent();
			this.Icon = Icon.ExtractAssociatedIcon(Path.GetFullPath(".\\BerichtManager.exe"));
			handler = new ConfigHandler();
			UpdateTree();
			if (handler.LoadActive() == "") 
			{
				btEdit.Enabled = false;
			}
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
			var directoryNode = new TreeNode(directoryInfo.Name);
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
			var temp = field.Range.Paragraphs.TabStops.Count;
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
		 * Creates a new Word document from a given template
		 * </summary>
		 * <param name="templatePath">The full path to the template to be used</param>
		*/
		private void CreateDocument(object templatePath) 
		{
			try 
			{
				object missing = Missing.Value;
				Word.Document doc = null;

				if (File.Exists((string)templatePath))
				{
					/*Word.Application */
					wordApp = new Word.Application();
					wordApp.Visible = visible;
					doc = wordApp.Documents.Add(ref templatePath);

					if (doc.FormFields.Count != 10)
					{
						MessageBox.Show("Invalid template");
						doc.Close(SaveChanges: false);
						wordApp.Quit();
						return;
					}

					EditForm form;
					//Fill name
					var enumerator = doc.FormFields.GetEnumerator();
					enumerator.MoveNext();
					if (!string.IsNullOrEmpty(handler.LoadName()))
					{
						((Word.FormField)enumerator.Current).Result = handler.LoadName();
					}
					else 
					{
						form = new EditForm("Enter your name", "Name Vorname", false);
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
					//((Word.FormField)enumerator.Current).Range.Text = handler.LoadNumber();
					((Word.FormField)enumerator.Current).Result = handler.LoadNumber();

					//Enter week start and end
					DateTime baseDate = DateTime.Today;
					var today = baseDate;
					var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek + 1);
					var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
					enumerator.MoveNext();
					((Word.FormField)enumerator.Current).Result = thisWeekStart.ToString("dd.MM.yyyy");
					enumerator.MoveNext();
					((Word.FormField)enumerator.Current).Result = thisWeekEnd.ToString("dd.MM.yyyy");

					//Enter Year
					enumerator.MoveNext();
					((Word.FormField)enumerator.Current).Result = today.Year.ToString();

					//Enter work field
					form = new EditForm("Betriebliche Tätigkeiten", "", false, isCreate: true);
					enumerator.MoveNext();
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
						//((Word.FormField)enumerator.Current).Result = form.Result.Replace("\n", "\v");
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							doc.Close(Word.WdSaveOptions.wdDoNotSaveChanges);
							wordApp.Quit();
							return;
						}
						else
						{
							((Word.FormField)enumerator.Current).Result = "-Keine-";
						}
					}

					//Enter work seminars
					form = new EditForm("Unterweisungen, betrieblicher Unterricht, sonstige Schulungen", "-Keine-", false, isCreate: true);
					enumerator.MoveNext();
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
						//((Word.FormField)enumerator.Current).Result = form.Result.Replace("\n", "\v");
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							doc.Close(Word.WdSaveOptions.wdDoNotSaveChanges);
							wordApp.Quit();
							return;
						}
						else
						{
							((Word.FormField)enumerator.Current).Result = "-Keine-";
						}
					}

					//Shool stuff
					form = new EditForm("Berufsschule (Unterrichtsthemen)", "", true, isCreate: true);
					enumerator.MoveNext();
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							doc.Close(Word.WdSaveOptions.wdDoNotSaveChanges);
							wordApp.Quit();
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
					string path = Path.GetFullPath(".\\..\\" + today.Year) + "\\WochenberichtKW" + new CultureInfo("de-DE").Calendar.GetWeekOfYear(today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday) + ".docx";
					SetFontInDoc(doc, wordApp);
					doc.SaveAs2(FileName: path);
					
					if (int.TryParse(handler.LoadNumber(), out int i)) handler.EditNumber("" + (i + 1));
					handler.EditActive(path);
					handler.SaveLastReportKW(new CultureInfo("de-DE").Calendar.GetWeekOfYear(today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday));
					MessageBox.Show("Created Document at: " + Path.GetFullPath(".\\..\\" + today.Year) + "\\WochenberichtKW" + new CultureInfo("de-DE").Calendar.GetWeekOfYear(today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday) + ".docx");

					doc.Close();
					wordApp.Quit();
					UpdateTree();
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
						wordApp.Quit();
						break;
					default:
						MessageBox.Show(ex.StackTrace);
						break;
				}
				try
				{
					wordApp.Quit(false);
				}
				catch
				{

				}
				Close();
			}
			/*catch (Exception ex)
			{
				if (ex.HResult == -2147023174)
				{
					MessageBox.Show("an unexpected problem occured this progam will now close!");
				}
				try
				{
					wordApp.Quit(false);
				}
				catch (Exception exx)
				{

				}
				Close();
			}*/
		}

		/**
		 * <summary>
		 * Creates a new Word document from a given template for a given time.
		 * Useful for creating reports in bulk due to the Wordapp not being closed
		 * </summary>
		 * <param name="templatePath">The full path of the template to be used</param>
		 * <param name="baseDate">The date of the report to be created</param>
		 * <param name="app">The Wordapp that will create the document</param>
		 * <param name="vacation">If you missed reports due to vacation</param>
		*/
		private void CreateDocument(object templatePath, DateTime baseDate, Word.Application app, bool vacation = false)
		{
			try
			{
				Word.Document doc = null;

				if (File.Exists((string)templatePath))
				{
					/*Word.Application */
					//app = new Word.Application();
					//app.Visible = visible;
					doc = app.Documents.Add(ref templatePath);

					if (doc.FormFields.Count != 10)
					{
						MessageBox.Show("Invalid template");
						doc.Close(SaveChanges: false);
						app.Quit(SaveChanges: false);
						return;
					}

					EditForm form;
					//Fill name
					var enumerator = doc.FormFields.GetEnumerator();
					enumerator.MoveNext();
					if (!string.IsNullOrEmpty(handler.LoadName()))
					{
						((Word.FormField)enumerator.Current).Result = handler.LoadName();
					}
					else
					{
						form = new EditForm("Enter your name", "Name Vorname", false);
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
					((Word.FormField)enumerator.Current).Result = handler.LoadNumber();

					//Enter week start and end
					var today = baseDate;
					var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek + 1);
					var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
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
						form = new EditForm("Betriebliche Tätigkeiten", "", false, isCreate: true);
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
								//app.Quit();
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
						form = new EditForm("Unterweisungen, betrieblicher Unterricht, sonstige Schulungen", "-Keine-", false, isCreate: true);
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
								//app.Quit();
								return;
							}
							else
							{
								((Word.FormField)enumerator.Current).Result = "-Keine-";
							}
						}
					}

					//Shool stuff
					//form = new EditForm("Berufsschule (Unterrichtsthemen)", "", true);
					enumerator.MoveNext();
					form = new EditForm("Berufsschule (Unterrichtsthemen)", "", isCreate: true);
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
							//app.Quit();
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
					string path = Path.GetFullPath(".\\..\\" + today.Year) + "\\WochenberichtKW" + new CultureInfo("de-DE").Calendar.GetWeekOfYear(today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday) + ".docx";
					SetFontInDoc(doc, app);
					doc.SaveAs2(FileName: path);

					if (int.TryParse(handler.LoadNumber(), out int i)) handler.EditNumber("" + (i + 1));
					handler.EditActive(path);
					handler.SaveLastReportKW(new CultureInfo("de-DE").Calendar.GetWeekOfYear(today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday));
					MessageBox.Show("Created Document at: " + Path.GetFullPath(".\\..\\" + today.Year) + "\\WochenberichtKW" + new CultureInfo("de-DE").Calendar.GetWeekOfYear(today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday) + ".docx");

					doc.Close();
					//app.Quit();
					UpdateTree();
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
						//app.Quit();
						break;
					default:
						MessageBox.Show(ex.StackTrace);
						break;
				}
				/*try
				{
					app.Quit(false);
				}
				catch
				{

				}*/
				Close();
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
					wordApp.Quit();
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
			CultureInfo culture = new CultureInfo("de-DE");
			DateTimeFormatInfo dfi = culture.DateTimeFormat;
			DateTime date1 = new DateTime(DateTime.Today.Year, 12, 31);

			Calendar cal = dfi.Calendar;
			int weekOfYear = culture.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);

			DateTime today = DateTime.Today;
			Word.Application multipleApp = new Word.Application();
			multipleApp.Visible = visible;
			if (handler.LoadLastReportKW() < weekOfYear)
			{
				//Missing reports in current year
				int reportNr = handler.LoadLastReportKW();
				for (int i = 1; i < weekOfYear - reportNr; i++)
				{
					//Console.WriteLine("Created report for week " + culture.Calendar.GetWeekOfYear(today.AddDays(i * (-7)), CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday));
					CreateDocument(handler.LoadPath(), today.AddDays( i * (-7)), multipleApp, vacation: vacation);
				}
			}
			else
			{
				//Missing missing reports over multiple years
				int nrOfWeeksLastYear = culture.Calendar.GetWeekOfYear(new DateTime(DateTime.Today.Year - 1, 12, 31), dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
				int weekOfCurrentYear = culture.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);

				int reportNr = handler.LoadLastReportKW();
				DateTime endOfLastYear = new DateTime(DateTime.Today.Year - 1, 12, 31);

				int repeats = nrOfWeeksLastYear - reportNr + weekOfCurrentYear;
				//Generate reports for missing reports over 2 years
				for (int i = 1; i < repeats; i++)
				{
					//Console.WriteLine("Creating report for week " + culture.Calendar.GetWeekOfYear(today.AddDays(i * (-7)), CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday));
					CreateDocument(handler.LoadPath(), today.AddDays( i * (-7)), multipleApp, vacation: vacation);
				}
				/*
				//Generate reports from last year
				for (int i = 0; i < nrOfWeeksLastYear - reportNr; i++) 
				{
					Console.WriteLine("Created report for week " + culture.Calendar.GetWeekOfYear(endOfLastYear.AddDays(i * (-7)), CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday));
				}

				//Generate missing reports for this year
				for (int i = 1; i < weekOfCurrentYear - 1; i++) 
				{
					Console.WriteLine("Created report for week of new year " + culture.Calendar.GetWeekOfYear(/*DateTime.Today*//*date1.AddDays(i * (-7)), CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday));
				}*/
			}
			try
			{
				multipleApp.Quit(SaveChanges: false);
			}
			catch
			{

			}
			return;
		}

		private void btCreate_Click(object sender, EventArgs e)
		{
			CultureInfo culture = new CultureInfo("de-DE");
			//Check if a report was created
			if (handler.LoadLastReportKW() > 0)
			{
				//Check if report for last week was created
				if (handler.LoadLastReportKW() < culture.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday) - 1)
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
			if (File.Exists(Path.GetFullPath(".\\..\\" + DateTime.Today.Year) + "\\WochenberichtKW" + new CultureInfo("de-DE").Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday) + ".docx"))
			{
				MessageBox.Show("A report has already been created for this week");
				return;
			}
			CreateDocument(handler.LoadPath());
		}

		private void btSetNumber_Click(object sender, EventArgs e)
		{
			EditForm form = new EditForm("Edit Number of Report", "", false);
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
			try
			{
				if (File.Exists(handler.LoadActive()))
				{
					wordApp = new Word.Application();
					wordApp.Visible = visible;
					doc = wordApp.Documents.Open(handler.LoadActive());

					if (doc.FormFields.Count != 10)
					{
						MessageBox.Show("Invalid document (you will have to manually edit)");
						doc.Close(SaveChanges: false);
						wordApp.Quit();
						return;
					}

					EditForm form;
					//Fill Name
					var enumerator = doc.FormFields.GetEnumerator();
					enumerator.MoveNext();
					if (!string.IsNullOrEmpty(handler.LoadName()))
					{
						FillText(wordApp, (Word.FormField)enumerator.Current, handler.LoadName());
						//((Word.FormField)enumerator.Current).Result = handler.LoadName();
					}
					else
					{
						form = new EditForm("Enter your name", "Name Vorname", false);
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
					form = new EditForm("Edit report nr.", ((Word.FormField)enumerator.Current).Result, false);
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
						//((Word.FormField)enumerator.Current).Result = form.Result;
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							doc.Close(SaveChanges: false);
							wordApp.Quit();
							return;
						}
						else 
						{
							if (form.DialogResult == DialogResult.Ignore) 
							{
								FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
								doc.Save();
								doc.Close();
								wordApp.Quit();
								return;
							}
						}
					}

					//Enter week start and end
					DateTime baseDate = DateTime.Today;
					var today = baseDate;
					var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek + 1);
					var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
					enumerator.MoveNext();
					form = new EditForm("Edit start of week", ((Word.FormField)enumerator.Current).Result, false);
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
						//((Word.FormField)enumerator.Current).Result = form.Result;
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							doc.Close(SaveChanges: false);
							wordApp.Quit();
							return;
						}
						else
						{
							if (form.DialogResult == DialogResult.Ignore)
							{
								FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
								doc.Save();
								doc.Close();
								wordApp.Quit();
								return;
							}
						}
					}

					enumerator.MoveNext();
					form = new EditForm("Edit end of week", ((Word.FormField)enumerator.Current).Result, false);
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
						//((Word.FormField)enumerator.Current).Result = form.Result;
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							doc.Close(SaveChanges: false);
							wordApp.Quit();
							return;
						}
						else
						{
							if (form.DialogResult == DialogResult.Ignore)
							{
								FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
								doc.Save();
								doc.Close();
								wordApp.Quit();
								return;
							}
						}
					}

					//Enter Year
					enumerator.MoveNext();
					form = new EditForm("Edit year", ((Word.FormField)enumerator.Current).Result, false);
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
						//((Word.FormField)enumerator.Current).Result = form.Result;
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							doc.Close(SaveChanges: false);
							wordApp.Quit();
							return;
						}
						else
						{
							if (form.DialogResult == DialogResult.Ignore)
							{
								FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
								doc.Save();
								doc.Close();
								wordApp.Quit();
								return;
							}
						}
					}

					//Enter work field
					enumerator.MoveNext();
					form = new EditForm("Betriebliche Tätigkeiten", ((Word.FormField)enumerator.Current).Result, false);
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
						//((Word.FormField)enumerator.Current).Result = form.Result.Replace("\n", "\v");
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							doc.Close(SaveChanges: false);
							wordApp.Quit();
							return;
						}
						else
						{
							if (form.DialogResult == DialogResult.Ignore)
							{
								FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
								doc.Save();
								doc.Close();
								wordApp.Quit();
								return;
							}
						}
					}

					//Enter work seminars
					enumerator.MoveNext();
					form = new EditForm("Unterweisungen, betrieblicher Unterricht, sonstige Schulungen", ((Word.FormField)enumerator.Current).Result, false);
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
						//((Word.FormField)enumerator.Current).Result = form.Result.Replace("\n", "\v");
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							doc.Close(SaveChanges: false);
							wordApp.Quit();
							return;
						}
						else
						{
							if (form.DialogResult == DialogResult.Ignore)
							{
								FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
								doc.Save();
								doc.Close();
								wordApp.Quit();
								return;
							}
						}
					}

					//Shool stuff
					enumerator.MoveNext();
					form = new EditForm("Berufsschule (Unterrichtsthemen)", ((Word.FormField)enumerator.Current).Result, false);
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
						//((Word.FormField)enumerator.Current).Result = form.Result.Replace("\n", "\v");
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							doc.Close(SaveChanges: false);
							wordApp.Quit();
							return;
						}
						else
						{
							if (form.DialogResult == DialogResult.Ignore)
							{
								FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
								doc.Save();
								doc.Close();
								wordApp.Quit();
								return;
							}
						}
					}

					//Fridy of week
					enumerator.MoveNext();
					form = new EditForm("Edit signdate (you)", ((Word.FormField)enumerator.Current).Result, false);
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
						//((Word.FormField)enumerator.Current).Result = form.Result;
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							doc.Close(SaveChanges: false);
							wordApp.Quit();
							return;
						}
						else
						{
							if (form.DialogResult == DialogResult.Ignore)
							{
								FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
								doc.Save();
								doc.Close();
								wordApp.Quit();
								return;
							}
						}
					}

					//Sign date 2
					enumerator.MoveNext();
					form = new EditForm("Edit signdate (not you)", ((Word.FormField)enumerator.Current).Result, false);
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
						//((Word.FormField)enumerator.Current).Result = form.Result;
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							doc.Close(SaveChanges: false);
							wordApp.Quit();
							return;
						}
						else
						{
							if (form.DialogResult == DialogResult.Ignore)
							{
								FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
								doc.Save();
								doc.Close();
								wordApp.Quit();
								return;
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
					MessageBox.Show(handler.LoadActive() + "not found was it deleted or moved?");
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
					default:
						MessageBox.Show(ex.StackTrace);
						break;
				}
				try
				{
					wordApp.Quit(false);
				}
				catch
				{

				}
				Close();
			}
		}

		private void btTest_Click(object sender, EventArgs e)
		{
			Client form = new Client();
			form.ShowDialog();
		}

		private void btPrint_Click(object sender, EventArgs e)
		{
			/*MessageBox.Show("Not yet implemented");
			return;*/
			if (tvReports.SelectedNode == null) 
			{
				MessageBox.Show("No report selected");
				return;
			}
			if (Path.GetExtension(Path.GetFullPath(".\\..\\..\\" + tvReports.SelectedNode.FullPath)) != ".docx")
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
			}
		}

		private void btPrintAll_Click(object sender, EventArgs e)
		{
			/*MessageBox.Show("Not yet implemented");
			return;*/
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

				foreach (string key in unPrintedFiles.Keys) 
				{
					if (unPrintedFiles[key].Contains(handler.LoadActive())) 
					{
						if (MessageBox.Show("Do you want to also print the last created report?\n(" + handler.LoadActive() + ")", "Print last created?", MessageBoxButtons.YesNo) != DialogResult.Yes) 
						{
							unPrintedFiles[key].Remove(handler.LoadActive());
						}
					}
				}

				PrintDialog printDialog = new PrintDialog();
				if (printDialog.ShowDialog() == DialogResult.OK)
				{
					Word.Application printApp = null;
					if (unPrintedFiles.Count == 0)
					{
						MessageBox.Show("No unprinted reports found");
						return;
					}
					try
					{
						printApp = new Word.Application();
						printApp.Visible = visible;
						foreach (string key in unPrintedFiles.Keys) 
						{
							unPrintedFiles[key].ForEach((f) => 
							{
								printApp.Documents.Open(FileName: f, ReadOnly: true);
							});
						}
						printApp.PrintOut(Background: false);
						printApp.Documents.Close();
						printApp.Quit(false);

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
			try
			{
				if (File.Exists(Path.GetFullPath(".\\..\\..\\" + tvReports.SelectedNode.FullPath)))
				{
					wordApp = new Word.Application();
					wordApp.Visible = visible;
					doc = wordApp.Documents.Open(Path.GetFullPath(".\\..\\..\\" + tvReports.SelectedNode.FullPath));

					if (doc.FormFields.Count != 10) 
					{
						MessageBox.Show("Invalid document (you will have to manually edit)");
						doc.Close(SaveChanges: false);
						wordApp.Quit();
						return;
					}

					EditForm form;
					//Fill Name
					var enumerator = doc.FormFields.GetEnumerator();
					enumerator.MoveNext();
					if (!string.IsNullOrEmpty(handler.LoadName()))
					{
						FillText(wordApp, (Word.FormField)enumerator.Current, handler.LoadName());
						//((Word.FormField)enumerator.Current).Result = handler.LoadName();
					}
					else
					{
						form = new EditForm("Enter your name", "Name Vorname", false);
						if (form.ShowDialog() == DialogResult.OK)
						{
							handler.SaveName(form.Result);
							FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
							//((Word.FormField)enumerator.Current).Result = handler.LoadName();
						}
						else
						{
							MessageBox.Show("Cannot proceed without a name!", "Name required!");
							return;
						}
					}
					enumerator.MoveNext();

					//Enter report nr.
					form = new EditForm("Edit report nr.", ((Word.FormField)enumerator.Current).Result, false);
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
						//((Word.FormField)enumerator.Current).Result = form.Result;
					}
					else 
					{
						if (form.DialogResult == DialogResult.Abort) 
						{
							doc.Close(SaveChanges: false);
							wordApp.Quit();
							return;
						}
						else
						{
							if (form.DialogResult == DialogResult.Ignore)
							{
								FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
								doc.Save();
								doc.Close();
								wordApp.Quit();
								return;
							}
						}
					}

					//Enter week start and end
					DateTime baseDate = DateTime.Today;
					var today = baseDate;
					var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek + 1);
					var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
					enumerator.MoveNext();
					form = new EditForm("Edit start of week", ((Word.FormField)enumerator.Current).Result, false);
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
						//((Word.FormField)enumerator.Current).Result = form.Result;
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							doc.Close(SaveChanges: false);
							wordApp.Quit();
							return;
						}
						else
						{
							if (form.DialogResult == DialogResult.Ignore)
							{
								FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
								doc.Save();
								doc.Close();
								wordApp.Quit();
								return;
							}
						}
					}

					enumerator.MoveNext();
					form = new EditForm("Edit end of week", ((Word.FormField)enumerator.Current).Result, false);
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
						//((Word.FormField)enumerator.Current).Result = form.Result;
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							doc.Close(SaveChanges: false);
							wordApp.Quit();
							return;
						}
						else
						{
							if (form.DialogResult == DialogResult.Ignore)
							{
								FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
								doc.Save();
								doc.Close();
								wordApp.Quit();
								return;
							}
						}
					}

					//Enter Year
					enumerator.MoveNext();
					form = new EditForm("Edit year", ((Word.FormField)enumerator.Current).Result, false);
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
						//((Word.FormField)enumerator.Current).Result = form.Result;
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							doc.Close(SaveChanges: false);
							wordApp.Quit();
							return;
						}
						else
						{
							if (form.DialogResult == DialogResult.Ignore)
							{
								FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
								doc.Save();
								doc.Close();
								wordApp.Quit();
								return;
							}
						}
					}

					//Enter work field
					enumerator.MoveNext();
					form = new EditForm("Betriebliche Tätigkeiten", ((Word.FormField)enumerator.Current).Result, false);
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
						//((Word.FormField)enumerator.Current).Result = form.Result.Replace("\n", "\v");
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							doc.Close(SaveChanges: false);
							wordApp.Quit();
							return;
						}
						else
						{
							if (form.DialogResult == DialogResult.Ignore)
							{
								FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
								doc.Save();
								doc.Close();
								wordApp.Quit();
								return;
							}
						}
					}

					//Enter work seminars
					enumerator.MoveNext();
					form = new EditForm("Unterweisungen, betrieblicher Unterricht, sonstige Schulungen", ((Word.FormField)enumerator.Current).Result, false);
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
						//((Word.FormField)enumerator.Current).Result = form.Result.Replace("\n", "\v");
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							doc.Close(SaveChanges: false);
							wordApp.Quit();
							return;
						}
						else
						{
							if (form.DialogResult == DialogResult.Ignore)
							{
								FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
								doc.Save();
								doc.Close();
								wordApp.Quit();
								return;
							}
						}
					}

					//Shool stuff
					enumerator.MoveNext();
					form = new EditForm("Berufsschule (Unterrichtsthemen)", ((Word.FormField)enumerator.Current).Result, false);
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
						//((Word.FormField)enumerator.Current).Result = form.Result.Replace("\n", "\v");
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							doc.Close(SaveChanges: false);
							wordApp.Quit();
							return;
						}
						else
						{
							if (form.DialogResult == DialogResult.Ignore)
							{
								FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
								doc.Save();
								doc.Close();
								wordApp.Quit();
								return;
							}
						}
					}

					//Fridy of week
					enumerator.MoveNext();
					form = new EditForm("Edit signdate (you)", ((Word.FormField)enumerator.Current).Result, false);
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
						//((Word.FormField)enumerator.Current).Result = form.Result;
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							doc.Close(SaveChanges: false);
							wordApp.Quit();
							return;
						}
						else
						{
							if (form.DialogResult == DialogResult.Ignore)
							{
								FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
								doc.Save();
								doc.Close();
								wordApp.Quit();
								return;
							}
						}
					}

					//Sign date 2
					enumerator.MoveNext();
					form = new EditForm("Edit signdate (not you)", ((Word.FormField)enumerator.Current).Result, false);
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
						//((Word.FormField)enumerator.Current).Result = form.Result;
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							doc.Close(SaveChanges: false);
							wordApp.Quit();
							return;
						}
						else
						{
							if (form.DialogResult == DialogResult.Ignore)
							{
								FillText(wordApp, (Word.FormField)enumerator.Current, form.Result);
								doc.Save();
								doc.Close();
								wordApp.Quit();
								return;
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
					MessageBox.Show(handler.LoadActive() + "not found was it deleted or moved?");
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
						wordApp.Quit();
						break;
					default:
						MessageBox.Show(ex.StackTrace);
						break;
				}
				try
				{
					wordApp.Quit(false);
				}
				catch
				{

				}
				Close();
			}
			/*catch (Exception ex)
			{
				if (ex.HResult == -2147023174)
				{
					MessageBox.Show("an unexpected problem occured this progam will now close!");
				}
				try
				{
					wordApp.Quit(false);
				}
				catch (Exception exx)
				{

				}
				Close();
			}*/
		
		}

		private void btDelete_Click(object sender, EventArgs e)
		{
			if (tvReports.SelectedNode == null)
			{
				MessageBox.Show("No report selected");
				return;
			}
			if (MessageBox.Show("Are you sure you want to delete the selected file?", "Delete?", MessageBoxButtons.YesNo) == DialogResult.Yes) 
			{
				if (File.Exists(Path.GetFullPath(".\\..\\..\\" + tvReports.SelectedNode.FullPath)))
				{
					if (Path.GetExtension(Path.GetFullPath(".\\..\\..\\" + tvReports.SelectedNode.FullPath)) == ".docx" || Path.GetFileName(Path.GetFullPath(".\\..\\..\\" + tvReports.SelectedNode.FullPath)).StartsWith("~$"))
					{
						if (Path.GetFullPath(".\\..\\..\\" + tvReports.SelectedNode.FullPath) == handler.LoadActive())
						{
							if (tvReports.SelectedNode.Text.Substring(15, ("" + new CultureInfo("de-DE").Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday)).Length) == new CultureInfo("de-DE").Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday).ToString()) 
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
						tvReports.Nodes.Remove(tvReports.SelectedNode);
						MessageBox.Show("File deleted successfully");
					}
					else 
					{
						MessageBox.Show("You may only delete Documents(*.docx) files");
					}
				}
				else 
				{
					if (Directory.Exists(Path.GetFullPath(".\\..\\..\\" + tvReports.SelectedNode.FullPath)))
					{
						MessageBox.Show("You may only delete Documents(*.docx) files");
					}
					else
					{
						MessageBox.Show("File not Found (was it moved or deleted?)");
					}
				}
			}
		}

		private void btLogin_Click(object sender, EventArgs e)
		{
			handler.doLogin();
		}

		private void btEditName_Click(object sender, EventArgs e)
		{
			EditForm form = new EditForm("Enter your name", "Name Vorname", false);
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
	}
}
