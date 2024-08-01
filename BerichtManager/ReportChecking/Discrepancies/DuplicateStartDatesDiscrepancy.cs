using System;
using System.Collections.Generic;

namespace BerichtManager.ReportChecking.Discrepancies
{
	/// <summary>
	/// Class for duplicate start dates
	/// </summary>
	internal class DuplicateStartDatesDiscrepancy : DuplicateDiscrepancy
	{
		/// <summary>
		/// Start date of affected reports
		/// </summary>
		protected DateTime StartDate { get; set; }

		/// <summary>
		/// Creates a new <see cref="DuplicateStartDatesDiscrepancy"/> object
		/// </summary>
		/// <param name="paths">Paths of affected reports</param>
		/// <param name="startDate"><see cref="DateTime"/> of all reports</param>
		public DuplicateStartDatesDiscrepancy(List<string> paths, DateTime startDate) : base(paths)
		{
			StartDate = startDate;
		}

		public override string ToString()
		{
			return $"Duplicate start dates detected:\n{CombinePaths()} -> {StartDate:dd.MM.yyyy}";
		}
	}
}
