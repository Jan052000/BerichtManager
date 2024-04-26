using Newtonsoft.Json;
using System;

namespace BerichtManager.UploadChecking
{
	internal class UploadedReport
	{
		/// <summary>
		/// Start date of report
		/// </summary>
		[JsonProperty]
		public DateTime StartDate { get; set; }
		/// <summary>
		/// Status of report
		/// </summary>
		[JsonProperty]
		public ReportNode.UploadStatuses Status { get; set; }
		/// <summary>
		/// lfdnr of report on IHK servers
		/// </summary>
		[JsonProperty]
        public int? LfdNR { get; set; }

        /// <summary>
        /// Creates a new <see cref="UploadedReport"/> object
        /// </summary>
		/// <param name="startDate">Start date of report</param>
        /// <param name="status">Status of report, if none is provided default is <see cref="ReportNode.UploadStatuses.Uploaded"/></param>
		/// <param name="lfdNr">Identifyer number</param>
        public UploadedReport(DateTime startDate, ReportNode.UploadStatuses status = ReportNode.UploadStatuses.Uploaded, int? lfdNr = null)
		{
			StartDate = startDate;
			Status = status;
			LfdNR = lfdNr;
		}
	}
}
