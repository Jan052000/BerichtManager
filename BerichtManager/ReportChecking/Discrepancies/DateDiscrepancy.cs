namespace BerichtManager.ReportChecking.Discrepancies
{
	/// <summary>
	/// Class for skipped dates
	/// </summary>
	internal class DateDiscrepancy : SingleDiscrepancy
	{
		/// <summary>
		/// Start date of start report
		/// </summary>
		protected DateTime StartDateStartReport { get; set; }
		/// <summary>
		/// Start date of next report
		/// </summary>
		protected DateTime StartDateNextReport { get; set; }

		/// <summary>
		/// Creates a new <see cref="DateDiscrepancy"/> object
		/// </summary>
		/// <param name="startReport">Path of start report</param>
		/// <param name="nextReport">Path of next report</param>
		/// <param name="startDateStartReport">Start date of start report</param>
		/// <param name="startDateNextReport">Start date of next report</param>
		public DateDiscrepancy(string startReport, string nextReport, DateTime startDateStartReport, DateTime startDateNextReport) : base(startReport, nextReport)
		{
			StartDateNextReport = startDateNextReport;
			StartDateStartReport = startDateStartReport;
		}

		public override string ToString()
		{
			return $"Start date discrepancy:\n{StartReport}({StartDateStartReport:dd.MM.yyyy}) -> {NextReport}({StartDateNextReport:dd.MM.yyyy}))";
		}
	}
}
