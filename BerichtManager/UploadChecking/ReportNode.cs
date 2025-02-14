using BerichtManager.HelperClasses;
using BerichtManager.OwnControls.OwnTreeView;
using System;

namespace BerichtManager.UploadChecking
{
	public class ReportNode : CustomTreeNode
	{
		/// <summary>
		/// Status of report on IHK servers
		/// </summary>
		public UploadStatuses UploadStatus { get; set; } = UploadStatuses.None;
		/// <summary>
		/// lfdnr of report on IHK servers
		/// </summary>
		public int? LfdNr { get; set; }
		/// <summary>
		/// Marks report as edited only locally
		/// </summary>
		public bool WasEditedLocally { get; set; } = false;
		/// <summary>
		/// Marks the report behind this node as opened
		/// </summary>
		public bool IsOpened { get; set; } = false;
		/// <summary>
		/// Marks the report as updated
		/// </summary>
		public bool WasUpdated { get; set; } = false;
		/// <summary>
		/// Last edit time of file represented by node
		/// </summary>
		public DateTime? FileLastWriteTime { get; private set; } = null;

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
			HandedIn,
			/// <summary>
			/// Report was handed in and accepted
			/// </summary>
			Accepted,
			/// <summary>
			/// Report was handed in and rejected
			/// </summary>
			Rejected
		}

		public ReportNode() : base() { }

		public ReportNode(string text = "", DateTime? lastWriteTime = null) : base(text)
		{
			FileLastWriteTime = lastWriteTime;
		}

		public override object Clone()
		{
			ReportNode _this = (ReportNode)base.Clone();
			_this.UploadStatus = UploadStatus;
			_this.WasEditedLocally = WasEditedLocally;
			_this.IsOpened = IsOpened;
			_this.LfdNr = LfdNr;
			_this.WasUpdated = WasUpdated;
			_this.FileLastWriteTime = FileLastWriteTime;
			return _this;
		}

		/// <summary>
		/// Copies the properties from <paramref name="report"/> to <see langword="this"/>
		/// </summary>
		/// <param name="report"><see cref="UploadedReport"/> to copy values from</param>
		public void SetReportProperties(UploadedReport report)
		{
			WasEditedLocally = report.WasEditedLocally;
			UploadStatus = report.Status;
			LfdNr = report.LfdNR;
			WasUpdated = report.WasUpdated;
		}

		/// <summary>
		/// Sets tool tip to be displayed
		/// </summary>
		public void SetToolTip()
		{
			string ttip = $"Status: {UploadStatus}";
			if (FileLastWriteTime is DateTime dt)
				ttip += $"\nLast write: {dt.ToString(DateTimeUtils.DATEFORMAT)}";
			ToolTipText = ttip;
		}
	}
}
