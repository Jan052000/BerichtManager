using BerichtManager.OwnControls.OwnTreeView;
using BerichtManager.UploadChecking;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BerichtManager.ThemeManagement
{
	/// <summary>
	/// Class for custom rendering of nodes
	/// </summary>
	internal class CustomNodeDrawer : ICustomNodeDrawer
	{
		private bool DrawUploadStatus { get; set; }
		/// <summary>
		/// Short reference to active theme set in <see cref="ThemeManager"/>
		/// </summary>
		private ITheme Theme { get => ThemeManager.Instance.ActiveTheme; }
		/// <summary>
		/// <see cref="Color"/> for drawing dottet lines
		/// </summary>
		private Color DottedLinesColor { get => Theme.TreeViewDottedLineColor; }

		/// <summary>
		/// <see cref="Color"/> for filling the highlighted node
		/// </summary>
		private Color SelectedHilightColor { get => Theme.TreeViewHighlightedNodeColor; }

		/// <summary>
		/// <see cref="Color"/> of node background
		/// </summary>
		private Color BackColor { get => Theme.BackColor; }

		/// <summary>
		/// <see cref="Color"/> of check box outline
		/// </summary>
		private Color CheckBoxOutlineColor { get => Theme.TextBoxBorderColor; }

		/// <summary>
		/// <see cref="Color"/> of check mark
		/// </summary>
		private Color CheckColor { get => Theme.ForeColor; }

		/// <summary>
		/// <see cref="Color"/> of background of check box
		/// </summary>
		private Color CheckBoxBackColor { get => Theme.BackColor; }

		/// <summary>
		/// <see cref="Color"/> of background of node which has its report open for edit
		/// </summary>
		private Color ReportOpenedColor { get => Theme.TreeViewReportOpenedHighlightColor; }

		/// <summary>
		/// Icon displayed before a collapsed node
		/// </summary>
		private Bitmap FolderClosedIcon { get; set; } = new Bitmap(Properties.Resources.Folder_Closed);

		/// <summary>
		/// Icon displayed before an expanded node
		/// </summary>
		private Bitmap FolderOpenedIcon { get; set; } = new Bitmap(Properties.Resources.Folder_Open);

		/// <summary>
		/// Creates a CustomDrawer object
		/// <param name="drawUploadStatus">Wether or not the upload status or reports should be drawn</param>
		/// </summary>
		public CustomNodeDrawer(bool drawUploadStatus = true)
		{
			DrawUploadStatus = drawUploadStatus;
		}

		/// <summary>
		/// Draws nodes to treeview
		/// </summary>
		/// <param name="e">Event that is passed down when drawing nodes</param>
		public void DrawNode(DrawTreeNodeEventArgs e)
		{
			if (e.Bounds.Width < 1 || e.Bounds.Height < 1)
				return;
			e.DrawDefault = false;
			if (e.Node == e.Node.TreeView.SelectedNode)
				using (Brush selectedHilight = new SolidBrush(SelectedHilightColor))
					e.Graphics.FillRectangle(selectedHilight, e.Node.Bounds);
			else
				using (Brush backColor = new SolidBrush(BackColor))
					e.Graphics.FillRectangle(backColor, e.Bounds);
			if (e.Node is ReportNode node && node.IsOpened)
				using (Brush editHighlight = new SolidBrush(ReportOpenedColor))
					e.Graphics.FillRectangle(editHighlight, e.Node.Bounds);

			bool drawCheckBoxes = e.Node.TreeView.CheckBoxes;

			TextRenderer.DrawText(e.Graphics, e.Node.Text, e.Node.TreeView.Font, new Point(e.Node.Bounds.X, e.Node.Bounds.Y), e.Node.TreeView.ForeColor);
			int iconSize = 18;
			int boxWidth = 13;

			Rectangle iconBounds;
			Rectangle? checkBoxBounds = null;

			if (drawCheckBoxes)
			{
				checkBoxBounds = new Rectangle(e.Node.Bounds.X - 1 - boxWidth, e.Node.Bounds.Y + e.Node.Bounds.Height / 2 - boxWidth / 2, boxWidth, boxWidth);
				iconBounds = new Rectangle(checkBoxBounds.Value.X - iconSize - 1, e.Node.Bounds.Y, iconSize, e.Node.Bounds.Height);
				if (e.Node is CustomTreeNode custom)
					DrawCheckBox(e.Graphics, checkBoxBounds.Value, custom);
				else
					DrawCheckBox(e.Graphics, checkBoxBounds.Value, e.Node.Checked);
			}
			else
			{
				if (e.Node.Parent == null)
					iconBounds = new Rectangle(e.Node.Bounds.X - e.Node.Bounds.Height - 3, e.Node.Bounds.Y, e.Node.Bounds.Height, e.Node.Bounds.Height);
				else
					iconBounds = new Rectangle(e.Node.Parent.Bounds.X + 7 - e.Node.Bounds.Height / 2, e.Node.Bounds.Y, e.Node.Bounds.Height, e.Node.Bounds.Height);
			}

			if (e.Node.TreeView.ShowLines)
				DrawDottedLine(e, iconBounds, checkBoxBounds);

			if (FolderOpenedIcon.Width != iconBounds.Width || FolderOpenedIcon.Height != iconBounds.Height)
				FolderOpenedIcon = new Bitmap(FolderOpenedIcon, iconBounds.Size);
			if (FolderClosedIcon.Width != iconBounds.Width || FolderClosedIcon.Height != iconBounds.Height)
				FolderClosedIcon = new Bitmap(FolderClosedIcon, iconBounds.Size);

			if (e.Node.Nodes.Count > 0)
			{
				if (e.Node.IsExpanded)
				{
					e.Graphics.DrawImage(FolderOpenedIcon, iconBounds);
				}
				else
				{
					e.Graphics.DrawImage(FolderClosedIcon, iconBounds);
				}
			}

			if (DrawUploadStatus && e.Node is ReportNode report && report.UploadStatus != ReportNode.UploadStatuses.None)
			{
				//Offset of node left + right
				int nodeOffset = 3;
				//Default color is invisible white
				Color? statusColor = Color.FromArgb(0, Color.White);
				switch (report.UploadStatus)
				{
					case ReportNode.UploadStatuses.Uploaded:
						statusColor = Theme.ReportUploadedColor;
						break;
					case ReportNode.UploadStatuses.HandedIn:
						statusColor = Theme.ReportHandedInColor;
						break;
					case ReportNode.UploadStatuses.Accepted:
						statusColor = Theme.ReportAcceptedColor;
						break;
					case ReportNode.UploadStatuses.Rejected:
						statusColor = Theme.ReportRejectedColor;
						break;
				}
				Rectangle ellipseRect = new Rectangle(iconBounds.X + 3, iconBounds.Y + 3, iconBounds.Width - 2 * nodeOffset, iconBounds.Height - 2 * nodeOffset);
				using (Pen outline = new Pen((Color)statusColor))
					e.Graphics.DrawEllipse(outline, ellipseRect);
				using (SolidBrush upload = new SolidBrush((Color)statusColor))
					e.Graphics.FillEllipse(upload, ellipseRect);
				if (report.WasEditedLocally)
				{
					int editedCircleOffset = 1;
					Rectangle r = new Rectangle(ellipseRect.X + editedCircleOffset, ellipseRect.Y + editedCircleOffset, ellipseRect.Width - editedCircleOffset * 2, ellipseRect.Height - editedCircleOffset * 2);
					using (SolidBrush editBrush = new SolidBrush(Color.Black))
						e.Graphics.FillEllipse(editBrush, r);
					using (Pen smoothe = new Pen((Color)statusColor))
						e.Graphics.DrawEllipse(smoothe, r);
				}
			}
		}

		/// <summary>
		/// Draws dotted lines between nodes
		/// </summary>
		/// <param name="e">Event that is passed down when drawing nodes</param>
		private void DrawDottedLine(DrawTreeNodeEventArgs e, Rectangle? iconBounds = null, Rectangle? checkBoxBounds = null)
		{
			if (e.Node.Parent != null)
			{
				using (Pen dottedLines = new Pen(DottedLinesColor))
				{
					dottedLines.DashStyle = DashStyle.Dot;
					int lineOffset = e.Node.TreeView.CheckBoxes && iconBounds.HasValue ? -(e.Node.Parent.Bounds.X - (iconBounds.Value.X + iconBounds.Value.Width / 2)) : 7;
					Point verticalLineStart = new Point(e.Node.Parent.Bounds.X + lineOffset, e.Node.Bounds.Y);
					Point verticalLineEndHalf = new Point(e.Node.Parent.Bounds.X + lineOffset, e.Node.Bounds.Y + e.Node.Bounds.Height / 2);
					Point verticalLineEndFull = new Point(e.Node.Parent.Bounds.X + lineOffset, e.Node.Bounds.Y + e.Node.Bounds.Height);
					if (e.Node.Parent.IsExpanded)
					{
						//vertical lines from parent
						if (e.Node == e.Node.Parent.Nodes[e.Node.Parent.Nodes.Count - 1])
							e.Graphics.DrawLine(dottedLines, verticalLineStart, verticalLineEndHalf);
						else
							e.Graphics.DrawLine(dottedLines, verticalLineStart, verticalLineEndFull);
						//horizontal line from line to parent
						if (checkBoxBounds.HasValue)
							e.Graphics.DrawLine(dottedLines, verticalLineEndHalf, new Point(checkBoxBounds.Value.X, e.Node.Bounds.Y + e.Node.Bounds.Height / 2));
						else
							e.Graphics.DrawLine(dottedLines, verticalLineEndHalf, new Point(e.Node.Bounds.X, e.Node.Bounds.Y + e.Node.Bounds.Height / 2));
					}

					//lines from root
					TreeNode node = e.Node.Parent;
					while (node.Parent != null)
					{
						if (node != node.Parent.LastNode)
							e.Graphics.DrawLine(dottedLines, new Point(node.Parent.Bounds.X + lineOffset, e.Node.Bounds.Y), new Point(node.Parent.Bounds.X + lineOffset, e.Node.Bounds.Y + e.Node.Bounds.Height));
						node = node.Parent;
					}
				}
			}
		}

		private void DrawCheckBox(Graphics graphics, Rectangle bounds, CustomTreeNode node)
		{
			if (node == null)
				return;
			using (Brush backColor = new SolidBrush(CheckBoxBackColor))
				graphics.FillRectangle(backColor, bounds);
			int checkBoxBorderWidth = 2;
			float checkWidth = 1.5f;
			float barWidth = 1.5f;

			using (Pen outline = new Pen(CheckBoxOutlineColor, checkBoxBorderWidth))
				graphics.DrawRectangle(outline, bounds);
			switch (node.CheckStatus)
			{
				case CustomTreeNode.CheckStatuses.Checked:
					using (Pen check = new Pen(CheckColor, checkWidth))
					{
						graphics.DrawLine(check, new Point(bounds.X + 2, bounds.Y + 7), new Point(bounds.X + 4, bounds.Y + 9));
						graphics.DrawLine(check, new Point(bounds.X + 5, bounds.Y + 9), new Point(bounds.X + 5 + 5, bounds.Y + 9 - 5));
					}
					break;
				case CustomTreeNode.CheckStatuses.Partial:
					using (Pen partial = new Pen(CheckColor, barWidth))
					{
						graphics.DrawLine(partial, new Point(bounds.X + 2, bounds.Y + bounds.Height / 2 + (int)barWidth / 2), new Point(bounds.X + 5 + 5, bounds.Y + bounds.Height / 2 + (int)barWidth / 2));
					}
					break;
			}
		}

		/// <summary>
		/// Draws a check box on a <see cref="Graphics"/> object
		/// </summary>
		/// <param name="graphics"><see cref="Graphics"/> object to paint check box on</param>
		/// <param name="bounds">Place to draw check box in <paramref name="graphics"/></param>
		/// <param name="isChecked">If a check mark should be drawn</param>
		private void DrawCheckBox(Graphics graphics, Rectangle bounds, bool isChecked)
		{
			using (Brush backColor = new SolidBrush(CheckBoxBackColor))
				graphics.FillRectangle(backColor, bounds);
			int checkBoxBorderWidth = 2;
			float checkWidth = 1.5f;

			using (Pen outline = new Pen(CheckBoxOutlineColor, checkBoxBorderWidth))
				graphics.DrawRectangle(outline, bounds);
			if (isChecked)
			{
				using (Pen check = new Pen(CheckColor, checkWidth))
				{
					graphics.DrawLine(check, new Point(bounds.X + 2, bounds.Y + 7), new Point(bounds.X + 4, bounds.Y + 9));
					graphics.DrawLine(check, new Point(bounds.X + 5, bounds.Y + 9), new Point(bounds.X + 5 + 5, bounds.Y + 9 - 5));
				}
			}
		}

		public void DrawNodeText(DrawTreeNodeEventArgs e)
		{
			if (e.Bounds.X == -1)
				return;
			TextRenderer.DrawText(e.Graphics, e.Node.Text, e.Node.TreeView.Font, new Point(e.Node.Bounds.X, e.Node.Bounds.Y), e.Node.TreeView.ForeColor);
		}
	}
}
