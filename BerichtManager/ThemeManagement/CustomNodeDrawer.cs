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
		private Pen dottedLines;
		/// <summary>
		/// Brush for filling the highlighted node
		/// </summary>
		private Brush hilightSelected;
		/// <summary>
		/// List of icons for folders
		/// </summary>
		private ImageList treeIcons;
		/// <summary>
		/// Back color
		/// </summary>
		private Brush BackColor;
		/// <summary>
		/// Creates a CustomDrawer object
		/// </summary>
		/// <param name="icons">list of icons to be used in treeview [0] closed [1] open</param>
		public CustomNodeDrawer(ImageList icons, ITheme theme)
		{
			dottedLines = new Pen(theme.TreeViewDottedLineColor);
			hilightSelected = new SolidBrush(theme.TreeViewHighlightedNodeColor);
			dottedLines.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
			treeIcons = icons;
			BackColor = new SolidBrush(theme.BackColor);
		}

		/// <summary>
		/// Changes color to theme
		/// </summary>
		/// <param name="theme">Theme to draw with</param>
		public void SetTheme(ITheme theme)
		{
			dottedLines = new Pen(theme.TreeViewDottedLineColor);
			hilightSelected = new SolidBrush(theme.TreeViewHighlightedNodeColor);
			dottedLines.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
			BackColor = new SolidBrush(theme.BackColor);
		}

		/// <summary>
		/// Draws nodes to treeview
		/// </summary>
		/// <param name="e">Event that is passed down when drawing nodes</param>
		public void DrawNode(DrawTreeNodeEventArgs e)
		{
			e.DrawDefault = false;
			if (e.Node == e.Node.TreeView.SelectedNode)
				e.Graphics.FillRectangle(hilightSelected, e.Node.Bounds);
			else
				e.Graphics.FillRectangle(BackColor, e.Bounds);
			TextRenderer.DrawText(e.Graphics, e.Node.Text, e.Node.TreeView.Font, new Point(e.Node.Bounds.X, e.Node.Bounds.Y), e.Node.TreeView.ForeColor);
			DrawDottedLine(e);
			if (e.Node.Nodes.Count > 0)
			{
				if (e.Node.IsExpanded)
				{
					if (e.Node.Parent != null)
						e.Graphics.DrawImage(treeIcons.Images[1], new Rectangle(e.Node.Parent.Bounds.X + 7 - e.Node.Bounds.Height / 2, e.Node.Bounds.Y, e.Node.Bounds.Height, e.Node.Bounds.Height));
				}
				else
				{
					if (e.Node.Parent != null)
						e.Graphics.DrawImage(treeIcons.Images[0], new Rectangle(e.Node.Parent.Bounds.X + 7 - e.Node.Bounds.Height / 2, e.Node.Bounds.Y, e.Node.Bounds.Height, e.Node.Bounds.Height));
				}
			}
			if (e.Node == e.Node.TreeView.Nodes[0])
			{
				if (e.Node.IsExpanded)
					e.Graphics.DrawImage(treeIcons.Images[1], new Rectangle(e.Node.Bounds.X - e.Node.Bounds.Height - 3, e.Node.Bounds.Y, e.Node.Bounds.Height, e.Node.Bounds.Height));
				else
					e.Graphics.DrawImage(treeIcons.Images[0], new Rectangle(e.Node.Bounds.X - e.Node.Bounds.Height - 3, e.Node.Bounds.Y, e.Node.Bounds.Height, e.Node.Bounds.Height));
			}
			if ((e.State & TreeNodeStates.Hot) == TreeNodeStates.Hot)
				return;
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
}
