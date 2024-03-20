using BerichtManager.Config;
using System;
using System.Globalization;

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

		public static CultureInfo Culture = new CultureInfo("de-DE");

		/// <summary>
		/// Resolves report number and caledar week into the desired name
		/// </summary>
		/// <param name="calendarWeek">Week of report creation</param>
		/// <param name="reportNumber">Number of created report</param>
		/// <returns>Resolved name according to pattern in <see cref="ConfigHandler"/></returns>
		public static string ResolveName(string calendarWeek, string reportNumber)
		{
			return ConfigHandler.Instance.NamingPattern().Replace(CalendarWeek, calendarWeek).Replace(ReportNumber, reportNumber);
		}

		/// <summary>
		/// Resolves report number and caledar week into the desired name
		/// </summary>
		/// <param name="baseDate">Date in week of created report</param>
		/// <param name="reportNumber">Number of created report</param>
		/// <returns>Resolved name according to pattern in <see cref="ConfigHandler"/></returns>
		public static string ResolveName(DateTime baseDate, string reportNumber)
		{
			return ConfigHandler.Instance.NamingPattern().Replace(CalendarWeek, Culture.Calendar.GetWeekOfYear(baseDate, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday).ToString()).Replace(ReportNumber, reportNumber);
		}
	}
}
