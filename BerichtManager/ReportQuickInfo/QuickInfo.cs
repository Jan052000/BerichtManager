namespace BerichtManager.ReportQuickInfo
{
	/// <summary>
	/// Represents basic information about a report
	/// </summary>
	public class QuickInfo
	{
		/// <summary>
		/// Start date of report
		/// </summary>
		public string? StartDate { get; set; }
		/// <summary>
		/// Number of report
		/// </summary>
		public int? ReportNumber { get; set; }

		/// <summary>
		/// Creates a new <see cref="QuickInfo"/> instance
		/// </summary>
		/// <param name="startDate">Start date of report</param>
		/// <param name="reportNumber">Number of report</param>
		public QuickInfo(string? startDate, int? reportNumber)
		{
			StartDate = startDate;
			ReportNumber = reportNumber;
		}
	}
}
