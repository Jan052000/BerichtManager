namespace BerichtManager.IHKClient
{
	internal class GetReportResult
	{
		/// <summary>
		/// Content of fetched report
		/// </summary>
		public ReportContent ReportContent { get; set; }
		/// <summary>
		/// Result of fetching report
		/// </summary>
		public ResultStatuses Result { get; set; }
		/// <summary>
		/// Result statuses
		/// </summary>
		public enum ResultStatuses
		{
			/// <summary>
			/// Report lfdnr was not valid
			/// </summary>
			InvalidLfdnr,
			/// <summary>
			/// Login failed
			/// </summary>
			LoginFailed,
			/// <summary>
			/// User is not authorized to access reports
			/// </summary>
			Unauthorized,
			/// <summary>
			/// Could not open report for edit on IHK servers
			/// </summary>
			UnableToOpenReport,
			/// <summary>
			/// Could not fill report content with values from IHK servers
			/// </summary>
			UnableToFillReport,
			/// <summary>
			/// Content was fetched successfully
			/// </summary>
			Success
		}

		/// <summary>
		/// Creates a new <see cref="GetReportResult"/> object
		/// </summary>
		/// <param name="result">Result status</param>
		/// <param name="reportContent"><see cref="ReportContents.ReportContent"/></param>
		public GetReportResult(ResultStatuses result, ReportContent reportContent = null)
		{
			Result = result;
			ReportContent = reportContent;
		}
	}
}
