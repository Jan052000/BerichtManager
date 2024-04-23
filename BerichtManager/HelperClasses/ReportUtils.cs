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
	}
}
