using Word = Microsoft.Office.Interop.Word;
using System.Collections.Generic;

namespace BerichtManager.HelperClasses
{
	/// <summary>
	/// Class holding methods used for interacting with properties of reports
	/// </summary>
	internal class ReportUtils
	{
		/// <summary>
		/// Checks wether or not a report is valid based on its full file name and extension
		/// </summary>
		/// <param name="name">Full name</param>
		/// <returns><see langword="true"/> if name is valid and <see langword="false"/> otherwise</returns>
		public static bool IsNameValid(string name)
		{
			return !string.IsNullOrEmpty(name) && !name.StartsWith("~$") && name.EndsWith(".docx");
		}

		/// <summary>
		/// Replaces all new line characters with \v
		/// </summary>
		/// <param name="text">Text to replace new lines in</param>
		/// <returns>String with all new line characters replaced with \v</returns>
		public static string TransformTextToWord(string text)
		{
			return ReplaceAllNewLine(text, "\v");
		}

		/// <summary>
		/// Replaces all new line characters with \n
		/// </summary>
		/// <param name="text">Text to replace new lines in</param>
		/// <returns>String with all new line characters replaced with \n</returns>
		public static string TransformTextToIHK(string text)
		{
			return ReplaceAllNewLine(text, "\n");
		}

		private static string ReplaceAllNewLine(string text, string newNewLine)
		{
			string result = text;
			List<string> newLines = new List<string>() { "\r\n", "\r", "\n", "\v" };
			foreach (string newLine in newLines)
			{
				if (newLine == newNewLine)
					continue;
				result = result.Replace(newLine, newNewLine);
			}
			return result;
		}

		/// <summary>
		/// Fills a WordInterop TextField with text
		/// </summary>
		/// <param name="app">The Word Application containing the documents with FormFields to fill</param>
		/// <param name="field">The FormField to fill with Text</param>
		/// <param name="text">The Text to Fill</param>
		public static void FillFormField(Word.Application app, Word.FormField field, string text)
		{
			if (text == null)
				return;
			text = TransformTextToWord(text);
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
	}
}
