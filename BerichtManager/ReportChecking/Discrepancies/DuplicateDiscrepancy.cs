namespace BerichtManager.ReportChecking.Discrepancies
{
	/// <summary>
	/// Class for discrepancies affecting multiple reports
	/// </summary>
	internal class DuplicateDiscrepancy : IReportDiscrepancy
	{
		/// <summary>
		/// Paths of affected reports
		/// </summary>
		protected List<string> Paths { get; set; }

		/// <summary>
		/// Creates a new <see cref="DuplicateDiscrepancy"/> object
		/// </summary>
		/// <param name="paths">Paths of affected reports</param>
		public DuplicateDiscrepancy(List<string> paths)
		{
			Paths = paths;
		}

		/// <summary>
		/// Combines all paths in <see cref="Paths"/>
		/// </summary>
		/// <returns>Combination of all paths in <see cref="Paths"/></returns>
		protected string CombinePaths()
		{
			string pathString = "";
			string separator = ", ";
			Paths.ForEach(p =>
			{
				pathString += p + separator;
			});
			return pathString.Substring(0, pathString.Length - separator.Length);
		}
	}
}
