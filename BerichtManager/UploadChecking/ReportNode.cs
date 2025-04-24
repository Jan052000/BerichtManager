using BerichtManager.Config;
using BerichtManager.HelperClasses;
using BerichtManager.OwnControls.OwnTreeView;
using BerichtManager.ReportQuickInfo;
using System.Text;

namespace BerichtManager.UploadChecking
{
	public class ReportNode : CustomTreeNode
	{
		/// <summary>
		/// Status of report on IHK servers
		/// </summary>
		public UploadStatuses UploadStatus { get; private set; } = UploadStatuses.None;
		/// <summary>
		/// lfdnr of report on IHK servers
		/// </summary>
		public int? LfdNr { get; private set; }
		/// <summary>
		/// Marks report as edited only locally
		/// </summary>
		public bool WasEditedLocally { get; private set; } = false;
		/// <summary>
		/// Marks the report behind this node as opened
		/// </summary>
		public bool IsOpened { get; set; } = false;
		/// <summary>
		/// Marks the report as updated
		/// </summary>
		public bool WasUpdated { get; private set; } = false;
		/// <summary>
		/// Last edit time of file represented by node
		/// </summary>
		public DateTime? FileLastWriteTime { get; private set; } = null;
		/// <summary>
		/// Start date of report from IHK
		/// </summary>
		public DateTime? StartDateIHK { get; private set; } = null;
		/// <summary>
		/// Last cached comment
		/// </summary>
		public string? LastComment { get; private set; }

		/// <summary>
		/// Report number of report if not uploaded
		/// </summary>
		public int? ReportNumber { get; private set; }
		/// <summary>
		/// Start date of report from file
		/// </summary>
		public string? StartDateReport { get; private set; } = null;

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
			_this.StartDateIHK = StartDateIHK;
			_this.LastComment = LastComment;
			_this.StartDateReport = StartDateReport;
			_this.ReportNumber = ReportNumber;
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
			StartDateIHK = report.StartDate;
			LastComment = report.LastComment;
		}

		/// <summary>
		/// Copies the properties from <paramref name="info"/> to <see langword="this"/>
		/// </summary>
		/// <param name="info"><see cref="QuickInfo"/> to copy values from</param>
		public void SetReportProperties(QuickInfo info)
		{
			StartDateReport = info.StartDate;
			ReportNumber = info.ReportNumber;
		}

		/// <summary>
		/// Sets tool tip to be displayed
		/// </summary>
		public void SetToolTip()
		{
			string ttip = $"Status: {UploadStatus}";
			if (StartDateReport is string dtStartReport)
				ttip += $"\nStart date report: {dtStartReport}";
			if (ReportNumber is int number)
				ttip += $"\nReport number: {number}";
			if (UploadStatus != UploadStatuses.None)
				ttip += "\nIHK:";
			if (StartDateIHK is DateTime dtStart)
				ttip += $"\nStart date: {dtStart.ToString(DateTimeUtils.DATEFORMAT)}";
			if (LfdNr.HasValue)
				ttip += $"\nLfdnr: {LfdNr}";
			if (FileLastWriteTime is DateTime dt)
				ttip += $"\nLast write: {dt.ToString(DateTimeUtils.DATEFORMAT)}";
			if (WasEditedLocally)
				ttip += "\nHas local changes";
			if (WasUpdated)
				ttip += "\nChanges were uploaded to IHK";
			if (!string.IsNullOrWhiteSpace(LastComment))
				ttip += $"\nComment:\n{SplitToolTip(LastComment, ConfigHandler.Instance.MaxReportToolTipWidth)}";
			ToolTipText = ttip;
		}

		/// <summary>
		/// Splits the provided <paramref name="text"/> into lines with width <paramref name="maxLineWidth"/>
		/// </summary>
		/// <param name="text">Text to split</param>
		/// <param name="maxLineWidth">Width of individual lines</param>
		/// <returns>Formatted tool tip</returns>
		public string SplitToolTip(string text, int maxLineWidth)
		{
			StringBuilder stringBuilder = new StringBuilder();

			Font font = TreeView?.Font ?? new Font(FontFamily.GenericMonospace, 12f);
			int spaceWidth = TextRenderer.MeasureText(" ", font).Width;

			List<string> currentLine = new List<string>();
			int width = 0;
			foreach (string snippet in text.Split(' '))
			{
				Size snippetDims = TextRenderer.MeasureText(snippet, font);
				if (snippetDims.Width >= maxLineWidth)
				{
					stringBuilder.AppendLine(string.Join(' ', currentLine));
					stringBuilder.AppendLine(snippet);
					width = 0;
					currentLine.Clear();
				}
				else if (width + snippetDims.Width >= maxLineWidth)
				{
					stringBuilder.AppendLine(string.Join(' ', currentLine));
					width = 0;
					currentLine.Clear();
					currentLine.Add(snippet);
					width = snippetDims.Width + spaceWidth;
				}
				else
				{
					currentLine.Add(snippet);
					width += snippetDims.Width + spaceWidth;
				}
			}
			if (currentLine.Count > 0)
				stringBuilder.AppendLine(string.Join(' ', currentLine));

			return stringBuilder.ToString();
		}
	}
}
