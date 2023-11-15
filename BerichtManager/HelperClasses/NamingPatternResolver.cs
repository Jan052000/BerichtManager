namespace BerichtManager.HelperClasses
{
	internal class NamingPatternResolver
	{
		/// <summary>
		/// Pattern for substituting calendar week
		/// </summary>
		public const string CalendarWeek = "~+CW+~";

		/// <summary>
		/// Pattern for substituting report number
		/// </summary>
		public const string ReportNumber = "~+RN+~";
	}
}
