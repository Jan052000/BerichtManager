using BerichtManager.OwnControls.OwnTreeView;
using BerichtManager.ThemeManagement;

namespace BerichtManager.HelperClasses
{
	public partial class FolderSelect : Form
	{
		/// <summary>
		/// <see cref="CustomTreeNode"/> containing only selected nodes and the nodes leading to them
		/// </summary>
		public CustomTreeNode? FilteredNode { get; private set; }
		/// <summary>
		/// Delegate for a node filter
		/// </summary>
		/// <param name="node"><see cref="CustomTreeNode"/> node to filter</param>
		/// <returns><see langword="true"/> if node should be filtered and <see langword="false"/> if not</returns>
		public delegate bool NodeFilter(CustomTreeNode node);

		/// <summary>
		/// Creates a new <see cref="Form"/> of <see cref="FolderSelect"/>
		/// </summary>
		/// <param name="node">Initial <see cref="CustomTreeNode"/> which represents a <see cref="System.IO.Directory"/></param>
		/// <param name="title">Title of <see cref="FolderSelect"/></param>
		public FolderSelect(CustomTreeNode node, string title = "")
		{
			InitializeComponent();
			Text = title;
			ThemeSetter.SetThemes(this);
			AddNode(node);
		}

		/// <summary>
		/// Creates a new <see cref="Form"/> of <see cref="FolderSelect"/>
		/// </summary>
		/// <param name="node">Initial <see cref="CustomTreeNode"/> which represents a <see cref="System.IO.Directory"/></param>
		/// <param name="removeFilter"><see cref="NodeFilter"/> used to filter <paramref name="node"/> removes nodes if <see langword="true"/> is returned</param>
		/// <param name="title">Title of <see cref="FolderSelect"/></param>
		public FolderSelect(CustomTreeNode node, NodeFilter removeFilter, string title = "")
		{
			InitializeComponent();
			Text = title;
			ThemeSetter.SetThemes(this);
			FilterNode(AddNode(node), removeFilter);
		}

		/// <summary>
		/// Adds <paramref name="node"/> to <see cref="tvFolders"/>
		/// </summary>
		/// <param name="node">Root <see cref="CustomTreeNode"/> to add</param>
		private CustomTreeNode AddNode(CustomTreeNode node)
		{
			int index;
			tvFolders.Nodes.Clear();
			if (node.TreeView == null)
				index = tvFolders.Nodes.Add(node);
			else
				index = tvFolders.Nodes.Add((CustomTreeNode)node.Clone());
			return tvFolders.Nodes[index];
		}

		/// <summary>
		/// Mutates and filters <paramref name="node"/> and its children
		/// </summary>
		/// <param name="node"><see cref="CustomTreeNode"/> to check</param>
		/// <param name="filter">Filter function</param>
		/// <returns>Filtered <see cref="CustomTreeNode"/> or <see langword="null"/> if <paramref name="node"/> was filtered</returns>
		private CustomTreeNode? FilterNode(CustomTreeNode node, NodeFilter filter)
		{
			if (filter(node))
				return null;

			bool hadChildren = node.Nodes.Count > 0;

			List<CustomTreeNode> children = new List<CustomTreeNode>();
			foreach (CustomTreeNode child in node.Nodes)
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

		/// <summary>
		/// Check if <paramref name="node"/> contains a child <see cref="CustomTreeNode"/> which is checked
		/// </summary>
		/// <param name="node"><see cref="CustomTreeNode"/> to check children of</param>
		/// <returns><see langword="true"/> if checked child was found and <see langword="false"/> otherwise</returns>
		private bool NodeHasCheckedChild(CustomTreeNode node)
		{
			bool hasPassed = false;
			foreach (CustomTreeNode childNode in node.Nodes)
			{
				if (childNode.Checked)
					return true;
				hasPassed |= NodeHasCheckedChild(childNode);
			}
			return hasPassed;
		}

		/// <summary>
		/// Filters all unchecked <see cref="CustomTreeNode"/>s from <paramref name="node"/>, does not mutate <paramref name="node"/>
		/// </summary>
		/// <param name="node"><see cref="CustomTreeNode"/> to filter</param>
		/// <returns>A new <see cref="CustomTreeNode"/> object, which is a filtered clone of <paramref name="node"/> or <see langword="null"/> if no nodes remain</returns>
		private CustomTreeNode? FilterUncheckedNodes(CustomTreeNode node)
		{
			if (!node.Checked && !NodeHasCheckedChild(node))
				return null;
			CustomTreeNode result = (CustomTreeNode)node.Clone();

			List<CustomTreeNode> passed = new List<CustomTreeNode>();
			foreach (CustomTreeNode childNode in result.Nodes)
			{
				if (childNode.Checked || NodeHasCheckedChild(childNode))
					passed.Add(childNode);
			}
			result.Nodes.Clear();
			foreach (CustomTreeNode add in passed)
			{
				if (FilterUncheckedNodes(add) is CustomTreeNode ctn)
					result.Nodes.Add(ctn);
			}

			return result;
		}

		/// <summary>
		/// Filters all unchecked <see cref="CustomTreeNode"/>s from <paramref name="node"/><br/>
		/// <b>! Caution:</b> will mutate <paramref name="node"/> !
		/// </summary>
		/// <param name="node"><see cref="CustomTreeNode"/> to filter</param>
		private void FilterUncheckedNodesMutate(CustomTreeNode node)
		{
			List<CustomTreeNode> checkedNodes = new List<CustomTreeNode>();
			foreach (CustomTreeNode childNode in node.Nodes)
			{
				if (childNode.Checked)
					checkedNodes.Add(childNode);
			}
			node.Nodes.Clear();
			foreach (CustomTreeNode add in checkedNodes)
			{
				node.Nodes.Add(add);
				FilterUncheckedNodesMutate(add);
			}
		}
	}
}
