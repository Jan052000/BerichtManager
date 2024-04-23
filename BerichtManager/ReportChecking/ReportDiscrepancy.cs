using System;

namespace BerichtManager.ReportChecking
{
	/// <summary>
	/// Class that hold data for discrepancies
	/// </summary>
	public class ReportDiscrepancy
	{
		/// <summary>
		/// Report discrepancy starts at
		/// </summary>
		public string StartAt { get; set; }
		/// <summary>
		/// Next report
		/// </summary>
		public string Next { get; set; }
		/// <summary>
		/// Kind of discrepancy for combined checks
		/// </summary>
		public DiscrepancyKind Kind { get; set; }
		/// <summary>
		/// Kinds of discrepancy detectable
		/// </summary>
		[Flags]
		public enum DiscrepancyKind
		{
			Number = 1,
			Date = 2
		}
		/// <summary>
		/// Number of report for the start of the <see cref="ReportDiscrepancy"/>
		/// </summary>
		public int ReportNumberStart { get; set; }
		/// <summary>
		/// Start date for the start of the <see cref="ReportDiscrepancy"/>
		/// </summary>
		public DateTime StartDateStart { get; set; }
		/// <summary>		  
		/// Number of report for the end of the <see cref="ReportDiscrepancy"/>		  
		/// </summary>
		public int ReportNumberEnd { get; set; }
		/// <summary>
		/// Start date for the end of the <see cref="ReportDiscrepancy"/>
		/// </summary>
		public DateTime StartDateEnd { get; set; }

		/// <summary>
		/// Creates a new <see cref="ReportDiscrepancy"/> object
		/// </summary>
		/// <param name="startAt">Report discrepancy starts at</param>
		/// <param name="next">Next report</param>
		/// <param name="kind"><see cref="DiscrepancyKind"/> of discrepancy</param>
		public ReportDiscrepancy(string startAt, string next, DiscrepancyKind kind, int reportNumberStart, DateTime discrepancsStart, int reportNumberEnd, DateTime discrepancyEnd)
		{
			StartAt = startAt;
			Next = next;
			Kind = kind;
			ReportNumberStart = reportNumberStart;
			StartDateStart = discrepancsStart;
			ReportNumberEnd = reportNumberEnd;
			StartDateEnd = discrepancyEnd;
		}
	}
}
