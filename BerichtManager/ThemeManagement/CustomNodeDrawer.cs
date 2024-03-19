using System.Drawing;
using System.Windows.Forms;

namespace BerichtManager.ThemeManagement
{
	/// <summary>
	/// Class for custom rendering of nodes
	/// </summary>
	internal class CustomNodeDrawer
	{
		/// <summary>
		/// Pen for drawing dottet lines
		/// </summary>
		private Pen DottedLines { get; set; }

		/// <summary>
		/// Brush for filling the highlighted node
		/// </summary>
		private Brush SelectedHilight { get; set; }

		/// <summary>
		/// Back color
		/// </summary>
		private Brush BackColor { get; set; }

		/// <summary>
		/// <see cref="Color"/> of check box outline
		/// </summary>
		private Color CheckBoxOutlineColor { get; set; }

		/// <summary>
		/// <see cref="Color"/> of check mark
		/// </summary>
		private Color CheckColor { get; set; }

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
		/// <param name="icons">list of icons to be used in treeview [0] closed [1] open</param>
		public CustomNodeDrawer()
		{
			ITheme theme = ThemeManager.Instance.ActiveTheme;
			DottedLines = new Pen(theme.TreeViewDottedLineColor);
			SelectedHilight = new SolidBrush(theme.TreeViewHighlightedNodeColor);
			DottedLines.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
			BackColor = new SolidBrush(theme.BackColor);
			CheckBoxOutlineColor = theme.TextBoxBorderColor;
			CheckColor = theme.ForeColor;
		}

		/// <summary>
		/// Changes color to theme
		/// </summary>
		/// <param name="theme">Theme to draw with</param>
		public void SetTheme(ITheme theme)
		{
			DottedLines = new Pen(theme.TreeViewDottedLineColor);
			SelectedHilight = new SolidBrush(theme.TreeViewHighlightedNodeColor);
			DottedLines.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
			BackColor = new SolidBrush(theme.BackColor);
			CheckBoxOutlineColor = theme.TextBoxBorderColor;
			CheckColor = theme.ForeColor;
		}

		/// <summary>
		/// Draws nodes to treeview
		/// </summary>
		/// <param name="e">Event that is passed down when drawing nodes</param>
		public void DrawNode(DrawTreeNodeEventArgs e)
		{
			e.DrawDefault = false;
			if (e.Node == e.Node.TreeView.SelectedNode)
				e.Graphics.FillRectangle(SelectedHilight, e.Node.Bounds);
			else
				e.Graphics.FillRectangle(BackColor, e.Bounds);

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
		}

		/// <summary>
		/// Draws dotted lines between nodes
		/// </summary>
		/// <param name="e">Event that is passed down when drawing nodes</param>
		private void DrawDottedLine(DrawTreeNodeEventArgs e)
		{
			if (e.Node.Parent != null)
			{
				int lineOffset = 7;
				Point verticalLineStart = new Point(e.Node.Parent.Bounds.X + lineOffset, e.Node.Bounds.Y);
				Point verticalLineEndHalf = new Point(e.Node.Parent.Bounds.X + lineOffset, e.Node.Bounds.Y + e.Node.Bounds.Height / 2);
				Point verticalLineEndFull = new Point(e.Node.Parent.Bounds.X + lineOffset, e.Node.Bounds.Y + e.Node.Bounds.Height);
				if (e.Node.Parent.IsExpanded)
				{
					//vertical lines from parent
					if (e.Node == e.Node.Parent.Nodes[e.Node.Parent.Nodes.Count - 1])
						e.Graphics.DrawLine(DottedLines, verticalLineStart, verticalLineEndHalf);
					else
						e.Graphics.DrawLine(DottedLines, verticalLineStart, verticalLineEndFull);
					//horizontal line from line to parent
					e.Graphics.DrawLine(DottedLines, verticalLineEndHalf, new Point(e.Node.Bounds.X, e.Node.Bounds.Y + e.Node.Bounds.Height / 2));
				}

				//lines from root
				TreeNode node = e.Node.Parent;
				while (node.Parent != null)
				{
					if (node != node.Parent.LastNode)
						e.Graphics.DrawLine(DottedLines, new Point(node.Parent.Bounds.X + lineOffset, e.Node.Bounds.Y), new Point(node.Parent.Bounds.X + lineOffset, e.Node.Bounds.Y + e.Node.Bounds.Height));
					node = node.Parent;
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
			using (Pen outline = new Pen(CheckBoxOutlineColor, 1))
				graphics.DrawRectangle(outline, bounds);
			if (isChecked)
				using (Pen check = new Pen(CheckColor, 1.5f))
				{
					graphics.DrawLine(check, new Point(bounds.X + 2, bounds.Y + 7), new Point(bounds.X + 4, bounds.Y + 9));
					graphics.DrawLine(check, new Point(bounds.X + 5, bounds.Y + 9), new Point(bounds.X + 5 + 5, bounds.Y + 9 - 5));
					//graphics.DrawLine(check, new Point(bounds.X, bounds.Y + bounds.Height / 2), new Point(bounds.X + bounds.Width, bounds.Y + bounds.Height / 2));
				}
		}
	}
}
