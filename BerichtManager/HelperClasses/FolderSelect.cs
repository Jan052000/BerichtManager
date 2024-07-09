using BerichtManager.ThemeManagement;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BerichtManager.HelperClasses
{
	public partial class FolderSelect : Form
	{
		/// <summary>
		/// <see cref="TreeNode"/> containing only selected nodes and the nodes leading to them
		/// </summary>
		public TreeNode FilteredNode { get; private set; }
		/// <summary>
		/// Instance of <see cref="ThemeManagement.CustomNodeDrawer"/>
		/// </summary>
		private CustomNodeDrawer CustomNodeDrawer { get; } = new CustomNodeDrawer();
		/// <summary>
		/// Flag to stop <see cref="CheckChildNodes(TreeNode)"/> from cascading endlessly
		/// </summary>
		private bool UpdatingChecks { get; set; } = false;
		/// <summary>
		/// Delegate for a node filter
		/// </summary>
		/// <param name="node"><see cref="TreeNode"/> node to filter</param>
		/// <returns><see langword="true"/> if node should be filtered and <see langword="false"/> if not</returns>
		public delegate bool NodeFilter(TreeNode node);

		/// <summary>
		/// Creates a new <see cref="Form"/> of <see cref="FolderSelect"/>
		/// </summary>
		/// <param name="node">Initial <see cref="TreeNode"/> which represents a <see cref="System.IO.Directory"/></param>
		public FolderSelect(TreeNode node)
		{
			InitializeComponent();
			ThemeSetter.SetThemes(this, ThemeManager.Instance.ActiveTheme);
			AddNode(node);
		}

		/// <summary>
		/// Creates a new <see cref="Form"/> of <see cref="FolderSelect"/>
		/// </summary>
		/// <param name="node">Initial <see cref="TreeNode"/> which represents a <see cref="System.IO.Directory"/></param>
		/// <param name="filter"><see cref="NodeFilter"/> used to filter <paramref name="node"/> removes nodes if <see langword="true"/> is returned</param>
		public FolderSelect(TreeNode node, NodeFilter filter)
		{
			InitializeComponent();
			ThemeSetter.SetThemes(this, ThemeManager.Instance.ActiveTheme);
			FilterNode(AddNode(node), filter);
		}

		/// <summary>
		/// Adds <paramref name="node"/> to <see cref="tvFolders"/>
		/// </summary>
		/// <param name="node">Root <see cref="TreeNode"/> to add</param>
		private TreeNode AddNode(TreeNode node)
		{
			int index;
			tvFolders.Nodes.Clear();
			if (node.TreeView == null)
				index = tvFolders.Nodes.Add(node);
			else
				index = tvFolders.Nodes.Add((TreeNode)node?.Clone());
			return tvFolders.Nodes[index];
		}

		/// <summary>
		/// Mutates and filters <paramref name="node"/> and its children
		/// </summary>
		/// <param name="node"><see cref="TreeNode"/> to check</param>
		/// <param name="filter">Filter function</param>
		/// <returns>Filtered <see cref="TreeNode"/> or <see langword="null"/> if <paramref name="node"/> was filtered</returns>
		private TreeNode FilterNode(TreeNode node, NodeFilter filter)
		{
			if (filter(node))
				return null;

			bool hadChildren = node.Nodes.Count > 0;

			List<TreeNode> children = new List<TreeNode>();
			foreach (TreeNode child in node.Nodes)
			{
				if (!filter(child) && FilterNode(child, filter) != null)
					children.Add(child);
			}

			node.Nodes.Clear();
			children.ForEach(child => node.Nodes.Add(child));
			if (hadChildren && children.Count == 0)
				return null;

			return node;
		}

		private void btClose_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void btConfirm_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;

			FilteredNode = FilterUncheckedNodes(tvFolders.Nodes[0]);

			Close();
		}

		private void tvFolders_DrawNode(object sender, DrawTreeNodeEventArgs e)
		{
			CustomNodeDrawer.DrawNode(e);
		}

		private void tvFolders_AfterCheck(object sender, TreeViewEventArgs e)
		{
			if (UpdatingChecks)
				return;

			if (e.Node.Nodes.Count > 0)
				CheckChildNodes(e.Node);

			UpdatingChecks = false;
		}

		/// <summary>
		/// Checks all child nodes of <paramref name="node"/>
		/// </summary>
		/// <param name="node"><see cref="TreeNode"/> to check all children for</param>
		private void CheckChildNodes(TreeNode node)
		{
			foreach (TreeNode childNode in node.Nodes)
			{
				childNode.Checked = node.Checked;
				if (childNode.Nodes.Count > 0)
					CheckChildNodes(childNode);
			}
		}

		/// <summary>
		/// Check if <paramref name="node"/> contains a child <see cref="TreeNode"/> which is checked
		/// </summary>
		/// <param name="node"><see cref="TreeNode"/> to check children of</param>
		/// <returns><see langword="true"/> if checked child was found and <see langword="false"/> otherwise</returns>
		private bool NodeHasCheckedChild(TreeNode node)
		{
			bool hasPassed = false;
			foreach (TreeNode childNode in node.Nodes)
			{
				if (childNode.Checked)
					return true;
				hasPassed |= NodeHasCheckedChild(childNode);
			}
			return hasPassed;
		}

		/// <summary>
		/// Filters all unchecked <see cref="TreeNode"/>s from <paramref name="node"/>, does not mutate <paramref name="node"/>
		/// </summary>
		/// <param name="node"><see cref="TreeNode"/> to filter</param>
		/// <returns>A new <see cref="TreeNode"/> object, which is a filtered clone of <paramref name="node"/> or <see langword="null"/> if no nodes remain</returns>
		private TreeNode FilterUncheckedNodes(TreeNode node)
		{
			if (!node.Checked && !NodeHasCheckedChild(node))
				return null;
			TreeNode result = (TreeNode)node.Clone();

			List<TreeNode> passed = new List<TreeNode>();
			foreach (TreeNode childNode in result.Nodes)
			{
				if (childNode.Checked || NodeHasCheckedChild(childNode))
					passed.Add(childNode);
			}
			result.Nodes.Clear();
			foreach (TreeNode add in passed)
			{
				result.Nodes.Add(FilterUncheckedNodes(add));
			}

			return result;
		}

		/// <summary>
		/// Filters all unchecked <see cref="TreeNode"/>s from <paramref name="node"/><br/>
		/// <b>! Caution:</b> will mutate <paramref name="node"/> !
		/// </summary>
		/// <param name="node"><see cref="TreeNode"/> to filter</param>
		private void FilterUncheckedNodesMutate(TreeNode node)
		{
			List<TreeNode> checkedNodes = new List<TreeNode>();
			foreach (TreeNode childNode in node.Nodes)
			{
				if (childNode.Checked)
					checkedNodes.Add(childNode);
			}
			node.Nodes.Clear();
			foreach (TreeNode add in checkedNodes)
			{
				node.Nodes.Add(add);
				FilterUncheckedNodesMutate(add);
			}
		}
	}
}
