﻿using System;
using System.IO;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;
using System.Reflection;
using BerichtManager.Config;
using BerichtManager.AddForm;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BerichtManager
{
	public partial class FormManager : Form
	{
		Word.Document doc = null;
		Word.Application wordApp = null;
		ConfigHandler handler;
		DirectoryInfo info = new DirectoryInfo(Path.GetFullPath(".\\.."));
		private bool visible = false;

		public FormManager()
		{
			InitializeComponent();
			handler = new ConfigHandler();

			UpdateTree();
		}

		private void UpdateTree() 
		{
			tvReports.Nodes.Clear();
			tvReports.Nodes.Add(CreateDirectoryNode(info));
		}

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

		private void CreateDocument(object templatePath) 
		{
			if (File.Exists(Path.GetFullPath(".\\..\\" + DateTime.Today.Year) + "\\WochenberichtKW" + new CultureInfo("de-DE").Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday) + ".docx")) 
			{
				MessageBox.Show("A report has already been created for this week");
				return;
			}
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

					//Enter report nr.
					var enumerator = doc.FormFields.GetEnumerator();
					enumerator.MoveNext();
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
					EditForm form = new EditForm("Betriebliche Tätigkeiten", "", false);
					enumerator.MoveNext();
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						((Word.FormField)enumerator.Current).Result = form.Result.Replace("\n", "\v");
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
					form = new EditForm("Unterweisungen, betrieblicher Unterricht, sonstige Schulungen", "", false);
					enumerator.MoveNext();
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						((Word.FormField)enumerator.Current).Result = form.Result.Replace("\n", "\v");
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
					form = new EditForm("Berufsschule (Unterrichtsthemen)", "", true);
					enumerator.MoveNext();
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						((Word.FormField)enumerator.Current).Result = form.Result.Replace("\n", "\v");
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
					doc.SaveAs2(path);
					//doc.SaveAs2(path, Word.WdSaveFormat.wdFormatDocument, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, true);
					if (int.TryParse(handler.LoadNumber(), out int i)) handler.EditNumber((i + 1).ToString());
					handler.EditActive(path);
					MessageBox.Show("Created Document at: " + Path.GetFullPath(".\\..\\" + today.Year) + "\\WochenberichtKW" + new CultureInfo("de-DE").Calendar.GetWeekOfYear(today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday) + ".docx");

					doc.Close();
					wordApp.Quit();

				}
				else
				{
					MessageBox.Show(handler.LoadPath() + " was not found was it moved or deleted?");
				}
			}
			catch (Exception ex)
			{
				if (ex.HResult == -2147023174)
				{
					MessageBox.Show("an unexpected problem occured this progam will now close!");
					try
					{
						wordApp.Quit(false);
					}
					catch (Exception exx)
					{

					}
					Close();
				}
			}
			/*finally
			{
				if (doc != null)
				{
					doc.Close();
				}
				if (wordApp != null)
				{
					wordApp.Quit();
				}
			}*/
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

		private void btCreate_Click(object sender, EventArgs e)
		{
			CreateDocument(handler.LoadPath());
		}

		private void btSetNumber_Click(object sender, EventArgs e)
		{
			EditForm form = new EditForm("Edit Number of Report", "", false);
			form.ShowDialog();
			if (form.DialogResult == DialogResult.OK)
			{
				handler.EditNumber(form.Result);
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

					//Enter report nr.
					var enumerator = doc.FormFields.GetEnumerator();
					enumerator.MoveNext();
					EditForm form = new EditForm("Edit report nr.", ((Word.FormField)enumerator.Current).Result, false);
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						((Word.FormField)enumerator.Current).Result = form.Result;
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							doc.Save();
							doc.Close();
							wordApp.Quit();
							return;
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
						((Word.FormField)enumerator.Current).Result = form.Result;
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							doc.Save();
							doc.Close();
							wordApp.Quit();
							return;
						}
					}

					enumerator.MoveNext();
					form = new EditForm("Edit end of week", ((Word.FormField)enumerator.Current).Result, false);
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						((Word.FormField)enumerator.Current).Result = form.Result;
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							doc.Save();
							doc.Close();
							wordApp.Quit();
							return;
						}
					}

					//Enter Year
					enumerator.MoveNext();
					form = new EditForm("Edit year", ((Word.FormField)enumerator.Current).Result, false);
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						((Word.FormField)enumerator.Current).Result = form.Result;
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							doc.Save();
							doc.Close();
							wordApp.Quit();
							return;
						}
					}

					//Enter work field
					enumerator.MoveNext();
					form = new EditForm("Betriebliche Tätigkeiten", ((Word.FormField)enumerator.Current).Result, false);
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						((Word.FormField)enumerator.Current).Result = form.Result.Replace("\n", "\v");
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							doc.Save();
							doc.Close();
							wordApp.Quit();
							return;
						}
						else
						{
							((Word.FormField)enumerator.Current).Result = form.Result.Replace("\n", "\v");
						}
					}

					//Enter work seminars
					enumerator.MoveNext();
					form = new EditForm("Unterweisungen, betrieblicher Unterricht, sonstige Schulungen", ((Word.FormField)enumerator.Current).Result, false);
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						((Word.FormField)enumerator.Current).Result = form.Result.Replace("\n", "\v");
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							doc.Save();
							doc.Close();
							wordApp.Quit();
							return;
						}
						else
						{
							((Word.FormField)enumerator.Current).Result = form.Result.Replace("\n", "\v");
						}
					}

					//Shool stuff
					enumerator.MoveNext();
					form = new EditForm("Berufsschule (Unterrichtsthemen)", ((Word.FormField)enumerator.Current).Result, false);
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						((Word.FormField)enumerator.Current).Result = form.Result.Replace("\n", "\v");
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							doc.Save();
							doc.Close();
							wordApp.Quit();
							return;
						}
						else
						{
							((Word.FormField)enumerator.Current).Result = form.Result.Replace("\n", "\v");
						}
					}

					//Fridy of week
					enumerator.MoveNext();
					form = new EditForm("Edit signdate (you)", ((Word.FormField)enumerator.Current).Result, false);
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						((Word.FormField)enumerator.Current).Result = form.Result;
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							doc.Save();
							doc.Close();
							wordApp.Quit();
							return;
						}
					}

					//Sign date 2
					enumerator.MoveNext();
					form = new EditForm("Edit signdate (not you)", ((Word.FormField)enumerator.Current).Result, false);
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						((Word.FormField)enumerator.Current).Result = form.Result;
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							doc.Save();
							doc.Close();
							wordApp.Quit();
							return;
						}
					}

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

				if (ex.HResult == -2147023174)
				{
					MessageBox.Show("an unexpected problem occured this progam will now close!");
					try
					{
						wordApp.Quit(false);
					}
					catch (Exception exx) 
					{
						
					}
					Close();
				}
			}
			/*finally 
			{
				if (doc != null)
				{
					doc.Close();
				}
				if (wordApp != null)
				{
					wordApp.Quit();
				}
			}*/
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
				MessageBox.Show("Nothing selected");
				return;
			}
			if (Path.GetExtension(Path.GetFullPath(".\\..\\..\\" + tvReports.SelectedNode.FullPath)) != ".docx")
			{
				MessageBox.Show("You may only print Documents(*.docx) files");
			}
			PrintDialog printDialog = new PrintDialog();
			if (printDialog.ShowDialog() == DialogResult.OK)
			{
				Word.Application printApp = null;
				if (tvReports.SelectedNode == null)
				{
					MessageBox.Show("No report selected");
					return;
				}
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
						File.Move(Path.GetFullPath(".\\..\\..\\" + tvReports.SelectedNode.FullPath), 
							Path.GetFullPath(".\\..\\..\\" + tvReports.SelectedNode.FullPath).Substring(0, Path.GetFullPath(".\\..\\..\\" + tvReports.SelectedNode.FullPath).Length - Path.GetFileName(".\\..\\..\\" + tvReports.SelectedNode.FullPath).Length) + "\\Gedruckt\\" + Path.GetFileName(".\\..\\..\\" + tvReports.SelectedNode.FullPath));
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
						catch (Exception exx)
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
						catch (Exception exx)
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

					if (doc.FormFields.Count != 9) 
					{
						MessageBox.Show("Invalid document (you will have to manually edit)");
						return;
					}

					//Enter report nr.
					var enumerator = doc.FormFields.GetEnumerator();
					enumerator.MoveNext();
					EditForm form = new EditForm("Edit report nr.", ((Word.FormField)enumerator.Current).Result, false);
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						((Word.FormField)enumerator.Current).Result = form.Result;
					}
					else 
					{
						if (form.DialogResult == DialogResult.Abort) 
						{
							doc.Save();
							doc.Close();
							wordApp.Quit();
							return;
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
						((Word.FormField)enumerator.Current).Result = form.Result;
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							doc.Save();
							doc.Close();
							wordApp.Quit();
							return;
						}
					}

					enumerator.MoveNext();
					form = new EditForm("Edit end of week", ((Word.FormField)enumerator.Current).Result, false);
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						((Word.FormField)enumerator.Current).Result = form.Result;
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							doc.Save();
							doc.Close();
							wordApp.Quit();
							return;
						}
					}

					//Enter Year
					enumerator.MoveNext();
					form = new EditForm("Edit year", ((Word.FormField)enumerator.Current).Result, false);
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						((Word.FormField)enumerator.Current).Result = form.Result;
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							doc.Save();
							doc.Close();
							wordApp.Quit();
							return;
						}
					}

					//Enter work field
					enumerator.MoveNext();
					form = new EditForm("Betriebliche Tätigkeiten", ((Word.FormField)enumerator.Current).Result, false);
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						((Word.FormField)enumerator.Current).Result = form.Result.Replace("\n", "\v");
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							doc.Save();
							doc.Close();
							wordApp.Quit();
							return;
						}
						else
						{
							((Word.FormField)enumerator.Current).Result = form.Result.Replace("\n", "\v");
							((Word.FormField)enumerator.Current).Range.AutoFormat();
						}
					}

					//Enter work seminars
					enumerator.MoveNext();
					form = new EditForm("Unterweisungen, betrieblicher Unterricht, sonstige Schulungen", ((Word.FormField)enumerator.Current).Result, false);
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						((Word.FormField)enumerator.Current).Result = form.Result.Replace("\n", "\v");
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							doc.Save();
							doc.Close();
							wordApp.Quit();
							return;
						}
						else
						{
							((Word.FormField)enumerator.Current).Result = form.Result.Replace("\n", "\v");
						}
					}

					//Shool stuff
					enumerator.MoveNext();
					form = new EditForm("Berufsschule (Unterrichtsthemen)", ((Word.FormField)enumerator.Current).Result, false);
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						((Word.FormField)enumerator.Current).Result = form.Result.Replace("\n", "\v");
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							doc.Save();
							doc.Close();
							wordApp.Quit();
							return;
						}
						else 
						{
							((Word.FormField)enumerator.Current).Result = form.Result.Replace("\n", "\v");
						}
					}

					//Fridy of week
					enumerator.MoveNext();
					form = new EditForm("Edit signdate (you)", ((Word.FormField)enumerator.Current).Result, false);
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						((Word.FormField)enumerator.Current).Result = form.Result;
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							doc.Save();
							doc.Close();
							wordApp.Quit();
							return;
						}
					}

					//Sign date 2
					enumerator.MoveNext();
					form = new EditForm("Edit signdate (not you)", ((Word.FormField)enumerator.Current).Result, false);
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						((Word.FormField)enumerator.Current).Result = form.Result;
					}
					else
					{
						if (form.DialogResult == DialogResult.Abort)
						{
							doc.Save();
							doc.Close();
							wordApp.Quit();
							return;
						}
					}

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
				if (ex.HResult == -2147023174)
				{
					MessageBox.Show("an unexpected problem occured this progam will now close!");
					try
					{
						wordApp.Quit(false);
					}
					catch (Exception exx)
					{

					}
					Close();
				}
			}
			/*finally
			{
				if (doc != null)
				{
					doc.Close();
				}
				if (wordApp != null)
				{
					wordApp.Quit();
				}
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
					MessageBox.Show("File not Found (was it moved or deleted?)");
				}
			}
		}

		private void btLogin_Click(object sender, EventArgs e)
		{
			handler.doLogin();
		}
	}
}
