using System.Globalization;

namespace BerichtManager.Extensions
{
	/// <summary>
	/// Holds extensions for <see cref="DateTime"/>
	/// </summary>
	public static class DateTimeExtensions
	{
		/// <summary>
		/// Calculates the week of year of <paramref name="dateTime"/>
		/// </summary>
		/// <param name="dateTime"><see cref="DateTime"/> to get week of year of</param>
		/// <returns>Number of week <paramref name="dateTime"/> is in</returns>
		public static int GetWeekOfYear(this DateTime dateTime)
		{
			return MainForm.Culture.Calendar.GetWeekOfYear(dateTime, MainForm.DateTimeFormatInfo.CalendarWeekRule, MainForm.DateTimeFormatInfo.FirstDayOfWeek);
		}

		/// <summary>
		/// Calculates the week of year to fit ISO 8601
		/// </summary>
		/// <param name="dateTime"><see cref="DateTime"/> to get week of year of</param>
		/// <returns>Number of week <paramref name="dateTime"/> is in according to ISO 8601</returns>
		public static int GetIsoWeekOfYear(this DateTime dateTime)
		{
			return ISOWeek.GetWeekOfYear(dateTime);
		}
	}
}
