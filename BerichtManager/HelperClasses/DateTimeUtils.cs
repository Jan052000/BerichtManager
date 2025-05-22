namespace BerichtManager.HelperClasses
{
	internal class DateTimeUtils
	{
		/// <summary>
		/// Default format for dates
		/// </summary>
		public const string DATEFORMAT = "dd.MM.yyyy";
		/// <summary>
		/// Default format for dates from IHK site
		/// </summary>
		public const string DATETIMEFORMATIHK = "dd.MM.yyyy";
		/// <summary>
		/// Format for getting time table from WebUntis
		/// </summary>
		public const string WEBUNTISTIMETABLEFORMAT = "yyyy-MM-dd";
		/// <summary>
		/// Format used in old API to express holiday start and end dates
		/// </summary>
		public const string OLDWEBUNTISHOLIDAYDATEFORMAT = "yyyyMMdd";
	}
}
