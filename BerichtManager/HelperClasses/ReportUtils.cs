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
	}
}
