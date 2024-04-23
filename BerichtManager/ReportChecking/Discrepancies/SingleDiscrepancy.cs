namespace BerichtManager.ReportChecking.Discrepancies
{
	/// <summary>
	/// Class for discrepancies only affecting a single report
	/// </summary>
	public class SingleDiscrepancy : IReportDiscrepancy
	{
		/// <summary>
		/// Report discrepancy starts at
		/// </summary>
		protected string StartReport { get; set; }
		/// <summary>
		/// Next report
		/// </summary>
		protected string NextReport { get; set; }

		/// <summary>
		/// Creates a new <see cref="SingleDiscrepancy"/> object
		/// </summary>
		/// <param name="startAt">Report discrepancy starts at</param>
		/// <param name="next">Next report in system</param>
		public SingleDiscrepancy(string startAt, string next)
		{
			StartReport = startAt;
			NextReport = next;
		}

		public override string ToString()
		{
			return $"Discrepancy:\n{StartReport} -> {NextReport}";
		}
	}
}
