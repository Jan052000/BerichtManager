using BerichtManager.UploadChecking;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BerichtManager.ThemeManagement
{
	/// <summary>
	/// Class for custom rendering of nodes
	/// </summary>
	internal class CustomNodeDrawer
	{
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
		/// Icon displayed before a collapsed node
		/// </summary>
		private readonly Bitmap FolderClosedIcon = new Bitmap(Properties.Resources.Folder_Closed);

		/// <summary>
		/// Icon displayed before an expanded node
		/// </summary>
		private readonly Bitmap FolderOpenedIcon = new Bitmap(Properties.Resources.Folder_Open);

		/// <summary>
		/// Creates a CustomDrawer object
		/// </summary>
		public CustomNodeDrawer()
		{

		}

		/// <summary>
		/// Draws nodes to treeview
		/// </summary>
		/// <param name="e">Event that is passed down when drawing nodes</param>
		public void DrawNode(DrawTreeNodeEventArgs e, bool drawUploadStatus = true)
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

			//When treeview has checkboxes enabled
			if (e.Node.TreeView.CheckBoxes)
			{
				TextRenderer.DrawText(e.Graphics, e.Node.Text, e.Node.TreeView.Font, new Point(e.Node.Bounds.X, e.Node.Bounds.Y), e.Node.TreeView.ForeColor);

				int iconSize = 18;
				int boxWidth = 13;

				//Draw check boxes
				Rectangle checkBoxBounds = new Rectangle(e.Node.Bounds.X - 1 - boxWidth, e.Node.Bounds.Y + e.Node.Bounds.Height / 2 - boxWidth / 2, boxWidth, boxWidth);
				DrawCheckBox(e.Graphics, checkBoxBounds, e.Node.Checked);

				//Draw folder icons
				Rectangle iconBounds = new Rectangle(checkBoxBounds.X - iconSize - 1, e.Node.Bounds.Y, iconSize, e.Node.Bounds.Height);
				if (e.Node.Nodes.Count > 0)
					e.Graphics.DrawImage(e.Node.IsExpanded ? FolderOpenedIcon : FolderClosedIcon, iconBounds);
				return;
			}

			TextRenderer.DrawText(e.Graphics, e.Node.Text, e.Node.TreeView.Font, new Point(e.Node.Bounds.X, e.Node.Bounds.Y), e.Node.TreeView.ForeColor);
			DrawDottedLine(e);
			if (e.Node.Nodes.Count > 0)
			{
				if (e.Node.IsExpanded)
				{
					if (e.Node.Parent != null)
						e.Graphics.DrawImage(FolderOpenedIcon, new Rectangle(e.Node.Parent.Bounds.X + 7 - e.Node.Bounds.Height / 2, e.Node.Bounds.Y, e.Node.Bounds.Height, e.Node.Bounds.Height));
					else
						e.Graphics.DrawImage(FolderOpenedIcon, new Rectangle(e.Node.Bounds.X - e.Node.Bounds.Height - 3, e.Node.Bounds.Y, e.Node.Bounds.Height, e.Node.Bounds.Height));
				}
				else
				{
					if (e.Node.Parent != null)
						e.Graphics.DrawImage(FolderClosedIcon, new Rectangle(e.Node.Parent.Bounds.X + 7 - e.Node.Bounds.Height / 2, e.Node.Bounds.Y, e.Node.Bounds.Height, e.Node.Bounds.Height));
					else
						e.Graphics.DrawImage(FolderClosedIcon, new Rectangle(e.Node.Bounds.X - e.Node.Bounds.Height - 3, e.Node.Bounds.Y, e.Node.Bounds.Height, e.Node.Bounds.Height));
				}
			}

			if (drawUploadStatus && e.Node is ReportNode report && report.UploadStatus != ReportNode.UploadStatuses.None)
			{
				//Offset of node left + right
				int nodeOffset = 6;
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
				using (Pen outline = new Pen((Color)statusColor))
					e.Graphics.DrawEllipse(outline, e.Node.Bounds.X - e.Node.Bounds.Height - 4 + nodeOffset / 2, e.Node.Bounds.Y + nodeOffset / 2, e.Node.Bounds.Height - nodeOffset, e.Node.Bounds.Height - nodeOffset);
				using (SolidBrush upload = new SolidBrush((Color)statusColor))
					e.Graphics.FillEllipse(upload, e.Node.Bounds.X - e.Node.Bounds.Height - 4 + nodeOffset / 2, e.Node.Bounds.Y + nodeOffset / 2, e.Node.Bounds.Height - nodeOffset, e.Node.Bounds.Height - nodeOffset);
			}
		}

		/// <summary>
		/// Draws dotted lines between nodes
		/// </summary>
		/// <param name="e">Event that is passed down when drawing nodes</param>
		private void DrawDottedLine(DrawTreeNodeEventArgs e)
		{
			if (e.Node.Parent != null)
			{
				using (Pen dottedLines = new Pen(DottedLinesColor))
				{
					dottedLines.DashStyle = DashStyle.Dot;
					int lineOffset = 7;
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
			using (Pen outline = new Pen(CheckBoxOutlineColor, 1))
				graphics.DrawRectangle(outline, bounds);
			if (isChecked)
			{
				using (Pen check = new Pen(CheckColor, 1.5f))
				{
					graphics.DrawLine(check, new Point(bounds.X + 2, bounds.Y + 7), new Point(bounds.X + 4, bounds.Y + 9));
					graphics.DrawLine(check, new Point(bounds.X + 5, bounds.Y + 9), new Point(bounds.X + 5 + 5, bounds.Y + 9 - 5));
				}
			}
		}
	}
}
