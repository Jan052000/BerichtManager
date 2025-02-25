using BerichtManager.IHKClient.ReportContents;

namespace BerichtManager.IHKClient
{
	internal class Report
	{
		/// <summary>
		/// Number of report
		/// </summary>
		public int? ReportNr { get; set; }
		/// <summary>
		/// Contents of report as they would be sent to IHK
		/// </summary>
		public ReportContent ReportContent { get; set; }

		/// <summary>
		/// Creates a new object of <see cref="Report"/>
		/// </summary>
		/// <param name="reportContent">Contents of the report</param>
		public Report(ReportContent? reportContent = null)
		{
			ReportContent = reportContent ?? new ReportContent();
		}
	}
}
