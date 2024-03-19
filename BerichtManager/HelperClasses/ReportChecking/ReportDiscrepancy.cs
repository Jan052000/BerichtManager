using System;

namespace BerichtManager.HelperClasses.ReportChecking
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
		/// Creates a new <see cref="ReportDiscrepancy"/> object
		/// </summary>
		/// <param name="startAt">Report discrepancy starts at</param>
		/// <param name="next">Next report</param>
		/// <param name="kind"><see cref="DiscrepancyKind"/> of discrepancy</param>
		public ReportDiscrepancy(string startAt, string next, DiscrepancyKind kind)
		{
			StartAt = startAt;
			Next = next;
			Kind = kind;
		}
	}
}
