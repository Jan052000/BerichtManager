namespace BerichtManager.ReportChecking.Discrepancies
{
	/// <summary>
	/// Class for duplicate report numbers
	/// </summary>
	internal class DuplicateNumbersDiscrepancy : DuplicateDiscrepancy
	{
		/// <summary>
		/// Report number of affected reports
		/// </summary>
		protected int ReportNumber { get; set; }

		/// <summary>
		/// Creates a new <see cref="DuplicateNumbersDiscrepancy"/> object
		/// </summary>
		/// <param name="paths">Paths of affected reports</param>
		/// <param name="reportNumber">Report number of affected reports</param>
		public DuplicateNumbersDiscrepancy(List<string> paths, int reportNumber) : base(paths)
		{
			ReportNumber = reportNumber;
		}

		public override string ToString()
		{
			return $"Duplicate numbers detected:\n{CombinePaths()} -> {ReportNumber}";
		}
	}
}
