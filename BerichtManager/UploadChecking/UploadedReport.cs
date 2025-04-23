using Newtonsoft.Json;
using System.ComponentModel;

namespace BerichtManager.UploadChecking
{
	public class UploadedReport
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
		/// Marks report as edited only locally
		/// </summary>
		[JsonProperty]
		[DefaultValue(false)]
		public bool WasEditedLocally { get; set; }
		/// <summary>
		/// Marks report as updated<br/>
		/// (Was introduced as rejected reports with local changes could be updated without handing them in resulting in users not being able to hand them in after)
		/// </summary>
		[JsonProperty]
		[DefaultValue(false)]
		public bool WasUpdated { get; set; }
		/// <summary>
		/// Last cached comment
		/// </summary>
		[JsonProperty]
		[DefaultValue(null)]
		public string? LastComment { get; set; }

		/// <summary>
		/// Creates a new <see cref="UploadedReport"/> object
		/// </summary>
		/// <param name="startDate">Start date of report</param>
		/// <param name="status">Status of report, if none is provided default is <see cref="ReportNode.UploadStatuses.Uploaded"/></param>
		/// <param name="lfdNr">Identifyer number</param>
		/// <param name="wasUpdated">Wether or not the report was previously rejected but had its' changes uploaded</param>
		/// <param name="lastComment">Last comment</param>
		public UploadedReport(DateTime startDate, ReportNode.UploadStatuses status = ReportNode.UploadStatuses.Uploaded, int? lfdNr = null, bool wasUpdated = false, string? lastComment = null)
		{
			StartDate = startDate;
			Status = status;
			LfdNR = lfdNr;
			WasUpdated = wasUpdated;
			LastComment = lastComment;
		}
	}
}
