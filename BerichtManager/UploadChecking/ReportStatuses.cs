namespace BerichtManager.UploadChecking
{
	/// <summary>
	/// Class to map status messages from IHK to <see cref="ReportNode.UploadStatuses"/>
	/// </summary>
	internal class ReportStatuses : Dictionary<string, ReportNode.UploadStatuses>
	{
		/// <summary>
		/// Creates a new <see cref="ReportStatuses"/> object
		/// </summary>
		public ReportStatuses() : base()
		{
			Add("Nachweis genehmigt", ReportNode.UploadStatuses.Accepted);
			Add("Nachweis abgelehnt", ReportNode.UploadStatuses.Rejected);
			Add("Warten auf Genehmigung", ReportNode.UploadStatuses.HandedIn);
			Add("in Bearbeitung bei Azubi", ReportNode.UploadStatuses.Uploaded);
		}
	}
}
