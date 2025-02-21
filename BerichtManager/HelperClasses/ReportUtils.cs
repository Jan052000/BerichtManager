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
		public static bool IsNameValid(string? name)
		{
			return !string.IsNullOrEmpty(name) && !name.StartsWith("~$") && name.EndsWith(".docx");
		}

		/// <summary>
		/// Replaces all new line characters with \v
		/// </summary>
		/// <param name="text">Text to replace new lines in</param>
		/// <returns>String with all new line characters replaced with \v</returns>
		public static string TransformTextToWord(string? text)
		{
			return ReplaceAllNewLine(text, "\v") ?? "";
		}

		/// <summary>
		/// Replaces all new line characters with \n
		/// </summary>
		/// <param name="text">Text to replace new lines in</param>
		/// <returns>String with all new line characters replaced with \n</returns>
		public static string TransformTextToIHK(string? text)
		{
			return ReplaceAllNewLine(text, "\n") ?? "";
		}

		/// <summary>
		/// Replaces all new lines in <paramref name="text"/> with <paramref name="newNewLine"/>
		/// </summary>
		/// <param name="text">Text to replace new lines in</param>
		/// <param name="newNewLine">New line character to replace others with</param>
		/// <returns><paramref name="text"/> with replaced new lines and empty string if text is <see langword="null"/></returns>
		private static string ReplaceAllNewLine(string? text, string newNewLine)
		{
			if (text == null)
				return "";
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
