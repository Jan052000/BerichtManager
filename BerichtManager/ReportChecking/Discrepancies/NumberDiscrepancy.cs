namespace BerichtManager.ReportChecking.Discrepancies
{
	/// <summary>
	/// Class for skipped report numbers
	/// </summary>
	internal class NumberDiscrepancy : SingleDiscrepancy
	{
		/// <summary>
		/// Report number of start report
		/// </summary>
		protected int NumberStartReport { get; set; }
		/// <summary>
		/// Report number of following report
		/// </summary>
		protected int NumberNextReport { get; set; }

		/// <summary>
		/// Creates a new <see cref="NumberDiscrepancy"/>
		/// </summary>
		/// <param name="startReport">Path of start report</param>
		/// <param name="nextReport">Path of next report</param>
		/// <param name="numberStartReport">Number of start report</param>
		/// <param name="numbernextReport">Number of next report</param>
		/// <param name="isDuplicate">If number is a duplicate</param>
		public NumberDiscrepancy(string startReport, string nextReport, int numberStartReport, int numbernextReport) : base(startReport, nextReport)
		{
			NumberNextReport = numbernextReport;
			NumberStartReport = numberStartReport;
		}

		public override string ToString()
		{
			return $"Report number discrepancy:\n{StartReport}({NumberStartReport}) -> {NextReport}({NumberNextReport})";
		}
	}
}
