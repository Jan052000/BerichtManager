using BerichtManager.ThemeManagement;
using System.ComponentModel;

namespace BerichtManager.OwnControls.OwnTreeView
{
	public class CustomTreeView : TreeView
	{
		/// <summary>
		/// Internal cache to remember which <see cref="CustomTreeNode"/> had its tool tip displayed last
		/// </summary>
		private CustomTreeNode? LastHovered { get; set; }

		/// <inheritdoc cref="CascadeCheckedChanges" path=""/>
		private bool cascadeCheckedChanges { get; set; } = true;
		/// <summary>
		/// Wether or not changes in checked state should propagate outward
		/// </summary>
		[DefaultValue(true)]
		[Category("Advanced")]
		public bool CascadeCheckedChanges
		{
			get => cascadeCheckedChanges;
			set
			{
				if (cascadeCheckedChanges != value)
				{
					cascadeCheckedChanges = value;
				}
			}
		}

		/// <inheritdoc cref="ShowNodeToolTips" path=""/>
		private bool showNodeToolTips { get; set; } = false;
		/// <summary>
		/// Wether or not <see cref="CustomTreeView"/> should show node tool tips
		/// </summary>
		[DefaultValue(false)]
		[Category("Advanced")]
		public new bool ShowNodeToolTips
		{
			get => showNodeToolTips;
			set
			{
				if (showNodeToolTips != value)
				{
					showNodeToolTips = value;
					if (value && NodeToolTip == null)
						NodeToolTip = new ToolTip();
					else if (!value)
					{
						NodeToolTip?.RemoveAll();
						NodeToolTip?.Dispose();
						NodeToolTip = null;
					}
				}
			}
		}

		/// <inheritdoc cref="SuppressWindowsWarnOnKeyDown" path=""/>
		private bool suppressWindowsWarnOnKeyDown { get; set; } = false;
		/// <summary>
		/// Wether or not windows warn sound should be suppressed when pressing enter
		/// </summary>
		[DefaultValue(false)]
		[Category("Advanced")]
		public bool SuppressWindowsWarnOnKeyDown
		{
			get => suppressWindowsWarnOnKeyDown;
			set
			{
				if (suppressWindowsWarnOnKeyDown == value)
					return;
				suppressWindowsWarnOnKeyDown = value;
			}
		}

		/// <summary>
		/// Internal <see cref="ToolTip"/> to display node tool tips
		/// </summary>
		internal ToolTip? NodeToolTip { get; private set; }

		/// <inheritdoc cref="CustomNodeDrawer" path=""/>
		private ICustomNodeDrawer customNodeDrawer { get; set; } = new CustomNodeDrawer();
		/// <summary>
		/// <see cref="ICustomNodeDrawer"/> to use if <see cref="TreeView.DrawMode"/> is not <see cref="TreeViewDrawMode.Normal"/>
		/// </summary>
		public ICustomNodeDrawer CustomNodeDrawer
		{
			get => customNodeDrawer;
			set
			{
				if (value != customNodeDrawer)
					customNodeDrawer = value;
			}
		}

		/// <summary>
		/// <see cref="CustomTreeNode"/> used as root to hold all child nodes
		/// </summary>
		private CustomTreeNode Root { get; } = new CustomTreeNode();

		/// <inheritdoc cref="TreeView.Nodes" path=""/>
		public new NodeCollectionWrapper Nodes { get; }

		public CustomTreeView() : base()
		{
			Nodes = new NodeCollectionWrapper(base.Nodes, Root);
		}

		protected override void OnDrawNode(DrawTreeNodeEventArgs e)
		{
			if (CustomNodeDrawer == null)
			{
				base.OnDrawNode(e);
				return;
			}
			switch (DrawMode)
			{
				case TreeViewDrawMode.OwnerDrawAll:
					CustomNodeDrawer.DrawNode(e);
					break;
				case TreeViewDrawMode.OwnerDrawText:
					CustomNodeDrawer.DrawNodeText(e);
					break;
				default:
					base.OnDrawNode(e);
					break;
			}
		}

		private bool IsAlreadyUpdating { get; set; } = false;

		protected override void OnAfterCheck(TreeViewEventArgs e)
		{
			if (IsAlreadyUpdating || !(e.Node is CustomTreeNode customNode))
			{
				base.OnAfterCheck(e);
				return;
			}
			IsAlreadyUpdating = true;
			BeginUpdate();
			switch (customNode.CheckStatus)
			{
				case CustomTreeNode.CheckStatuses.Checked:
					customNode.CheckStatus = CustomTreeNode.CheckStatuses.Unchecked;
					break;
				case CustomTreeNode.CheckStatuses.Unchecked:
				case CustomTreeNode.CheckStatuses.Partial:
					customNode.CheckStatus = CustomTreeNode.CheckStatuses.Checked;
					break;
			}
			EndUpdate();
			IsAlreadyUpdating = false;
			base.OnAfterCheck(e);
		}

		protected override void OnMouseHover(EventArgs e)
		{
			if (ShowNodeToolTips)
				ShowToolTip();
			base.OnMouseHover(e);
		}

		/// <summary>
		/// Handles showing a new tool tip or removing old one if new hovered node does not have one set
		/// </summary>
		private void ShowToolTip()
		{
			Point mousePos = PointToClient(MousePosition);
			Point toolTipPos = new Point(mousePos.X + 2, mousePos.Y + 2);
			CustomTreeNode node = (CustomTreeNode)GetNodeAt(mousePos);
			if (LastHovered != node)
			{
				if (string.IsNullOrEmpty(node?.ToolTipText))
				{
					NodeToolTip?.RemoveAll();
				}
				else
				{
					NodeToolTip?.RemoveAll();
					NodeToolTip?.Show(node.ToolTipText, this, toolTipPos);
				}
				LastHovered = node;
			}
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			NodeToolTip?.RemoveAll();
			LastHovered = null;
			base.OnMouseLeave(e);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				NodeToolTip?.RemoveAll();
				NodeToolTip?.Dispose();
				NodeToolTip = null;
			}
			base.Dispose(disposing);
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (SuppressWindowsWarnOnKeyDown && (keyData == Keys.Enter || keyData == Keys.Escape))
			{
				OnKeyDown(new KeyEventArgs(keyData));
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}
	}
}
