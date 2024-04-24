using BerichtManager.Config;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

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

		/// <summary>
		/// Retrieves all possible file name inserts from <paramref name="fileName"/>
		/// </summary>
		/// <param name="fileName">Full name of file</param>
		/// <returns><see cref="F"/></returns>
		public static ResolvedValues GetValuesFromName(string fileName)
		{
			int reportNumber = -1;
			int calendarWeek = -1;
			Regex regex = new Regex($@"({ConfigHandler.Instance.NamingPattern().Replace(CalendarWeek, @"(?<CW>\d+)").Replace(ReportNumber, @"(?<RN>\d+)")}.+?$)", RegexOptions.ExplicitCapture | RegexOptions.Singleline);
			MatchCollection matches = regex.Matches(fileName);
			foreach (Match match in matches)
			{
				foreach (Group group in match.Groups)
				{
					if (group.Name == "RN" && int.TryParse(group.Value, out int rn))
						reportNumber = rn;
					if (group.Name == "CW" && int.TryParse(group.Value, out int cw))
						calendarWeek = cw;
				}
			}

			return new ResolvedValues(reportNumber, calendarWeek);
		}
	}

	/// <summary>
	/// Class that holds metrics that were replaced in report name
	/// </summary>
	public class ResolvedValues
	{
		/// <summary>
		/// Report number
		/// </summary>
		public int ReportNumber { get; set; }
		/// <summary>
		/// Calendar week
		/// </summary>
		public int CalendarWeek { get; set; }

		/// <summary>
		/// Creates a new <see cref="ResolvedValues"/> object
		/// </summary>
		/// <param name="reportNumber">Found report number</param>
		/// <param name="calendarWeek">Found calendar week</param>
		public ResolvedValues(int reportNumber, int calendarWeek)
		{
			ReportNumber = reportNumber;
			CalendarWeek = calendarWeek;
		}
	}
}
