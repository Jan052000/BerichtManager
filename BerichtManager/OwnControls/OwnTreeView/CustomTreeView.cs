using BerichtManager.ThemeManagement;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BerichtManager.OwnControls.OwnTreeView
{
	public class CustomTreeView : TreeView
	{
		#region Theme

		/// <inheritdoc cref="HightlightColor" path=""/>
		private Color hightlightColor { get; set; } = SystemColors.MenuHighlight;
		/// <summary>
		/// Color of highlighted node
		/// </summary>
		[DefaultValue(typeof(SystemColors), "MenuHighlight")]
		[Category("Advanced")]
		public Color HightlightColor
		{
			get => hightlightColor;
			set
			{
				if (value != hightlightColor)
				{
					hightlightColor = value;
					Invalidate();
				}
			}
		}

		/// <inheritdoc cref="RootPathColor" path=""/>
		private Color rootPathColor { get; set; } = SystemColors.ControlText;
		/// <summary>
		/// Color of the path drawn between nodes
		/// </summary>
		[DefaultValue(typeof(SystemColors), "ControlText")]
		[Category("Advanced")]
		public Color RootPathColor
		{
			get => rootPathColor;
			set
			{
				if (value != rootPathColor)
				{
					rootPathColor = value;
					Invalidate();
				}
			}
		}

		#endregion

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

		/// <inheritdoc cref="TreeView.HotTracking" path=""/>
		[Category("Style")]
		[DefaultValue(false)]
		public new bool HotTracking { get => base.HotTracking; set => base.HotTracking = value; }

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

		protected override void OnAfterCheck(TreeViewEventArgs e)
		{
			if (e.Action == TreeViewAction.Collapse || e.Action == TreeViewAction.Expand)
			{
				base.OnAfterCheck(e);
				return;
			}
			if (!(e.Node is CustomTreeNode customNode))
			{
				base.OnAfterCheck(e);
				return;
			}
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
		}
	}
}
