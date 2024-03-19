using BerichtManager.ThemeManagement;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BerichtManager.HelperClasses.ReportChecking
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
		/// Creates a new form of <see cref="FolderSelect"/>
		/// </summary>
		/// <param name="node">Initial <see cref="TreeNode"/> which represents a <see cref="System.IO.Directory"/></param>
		public FolderSelect(TreeNode node)
		{
			InitializeComponent();
			ThemeSetter.SetThemes(this, ThemeManager.Instance.ActiveTheme);
			tvFolders.Nodes.Clear();
			try
			{
				tvFolders.Nodes.Add(node);
			}
			catch (ArgumentException)
			{
				//HResult -2147024809 means that the node is already in use in a control so a clone is added to tvFolders
				tvFolders.Nodes.Add((TreeNode)node.Clone());
			}
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
