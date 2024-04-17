using System.Windows.Forms;

namespace BerichtManager.UploadChecking
{
	internal class ReportNode : TreeNode
	{
		/// <summary>
		/// Status of report on IHK servers
		/// </summary>
		public UploadStatuses UploadStatus { get; set; } = UploadStatuses.None;

		/// <summary>
		/// Statuses of report on IHK servers
		/// </summary>
		public enum UploadStatuses
		{
			/// <summary>
			/// Report is not uploaded
			/// </summary>
			None,
			/// <summary>
			/// Report was uploaded but not handed in
			/// </summary>
			Uploaded,
			/// <summary>
			/// Report was uploaded and handed in
			/// </summary>
			HandedIn
		}

		public ReportNode() : base() { }

		public ReportNode(string text) : base(text) { }
	}
}
