using System;
using System.Windows.Forms;

namespace BerichtManager.OwnControls.OwnTreeView
{
	public class CustomTreeNode : TreeNode
	{
		/// <summary>
		/// Tri state check status of <see cref="CustomTreeNode"/>s
		/// </summary>
		public enum CheckStatuses
		{
			/// <summary>
			/// Node is unchecked
			/// </summary>
			Unchecked,
			/// <summary>
			/// Node has checked or partially checked children
			/// </summary>
			Partial,
			/// <summary>
			/// Node is checked
			/// </summary>
			Checked
		}

		/// <inheritdoc cref="CheckStatus" path=""/>
		private CheckStatuses checkStatus { get; set; } = CheckStatuses.Unchecked;
		/// <summary>
		/// <see cref="CheckStatuses"/> check status of node
		/// </summary>
		public CheckStatuses CheckStatus
		{
			get => checkStatus;
			set
			{
				if (checkStatus != value)
				{
					bool propagate = TreeView is CustomTreeView ctv && ctv.CascadeCheckedChanges;
					ChainChangeCheckStatus(value, propagate, propagate);
				}
			}
		}

		/// <inheritdoc cref="TreeNode.Nodes" path=""/>
		public new NodeCollectionWrapper Nodes { get; }

#pragma warning disable IDE0051 // Nicht verwendete private Member entfernen
		private new bool Checked
		{
			get => throw new Exception("Property blended out, please do not use");
		}
#pragma warning restore IDE0051 // Nicht verwendete private Member entfernen

		public CustomTreeNode() : base()
		{
			Nodes = new NodeCollectionWrapper(base.Nodes, this);
		}

		public CustomTreeNode(string text) : base(text)
		{
			Nodes = new NodeCollectionWrapper(base.Nodes, this);
		}

		/// <summary>
		/// Changes <see cref="CheckStatus"/> according to <paramref name="childStatus"/> and propagates up or down if necessary
		/// </summary>
		/// <param name="childStatus"><see cref="CheckStatuses"/> of child node propagating from below or new <see cref="CheckStatuses"/> if initial node</param>
		/// <param name="propagateUp">Wether or not changes should be applied up to parent</param>
		/// <param name="propagateDown">Wether or not changes should be applied to children</param>
		private void ChainChangeCheckStatus(CheckStatuses childStatus, bool propagateUp = false, bool propagateDown = false)
		{
			CustomTreeNode parent = Parent as CustomTreeNode;
			if (CheckStatus == childStatus)
				return;
			if (!propagateUp && !propagateDown)
				checkStatus = childStatus;
			switch (childStatus)
			{
				case CheckStatuses.Unchecked:
					if (propagateDown)
					{
						CheckAllNodes(this, childStatus);
					}
					if (propagateUp)
					{
						if (CountChecked() == 0 && CountCheckAndPartial() == 0)
						{
							checkStatus = childStatus;
							parent?.ChainChangeCheckStatus(childStatus, true, false);
						}
						else
						{
							checkStatus = CheckStatuses.Partial;
							parent?.ChainChangeCheckStatus(CheckStatuses.Partial, true, false);
						}
					}
					break;
				case CheckStatuses.Partial:
					checkStatus = childStatus;
					parent?.ChainChangeCheckStatus(childStatus, true, false);
					break;
				case CheckStatuses.Checked:
					if (propagateDown)
					{
						CheckAllNodes(this, childStatus);
					}
					if (propagateUp)
					{
						if (Nodes.Count == CountChecked())
						{
							checkStatus = CheckStatuses.Checked;
							parent?.ChainChangeCheckStatus(childStatus, true, false);
						}
						else
						{
							checkStatus = CheckStatuses.Partial;
							parent?.ChainChangeCheckStatus(CheckStatuses.Partial, true, false);
						}
					}
					break;
			}
		}

		/// <summary>
		/// Changes <see cref="CheckStatus"/> of all nodes contained in <paramref name="node"/> to <paramref name="newStatus"/>
		/// </summary>
		/// <param name="node"><see cref="CustomTreeNode"/> to change child statused of</param>
		/// <param name="newStatus">New <see cref="CheckStatuses"/> to set</param>
		private void CheckAllNodes(CustomTreeNode node, CheckStatuses newStatus)
		{
			foreach (CustomTreeNode child in node.Nodes)
			{
				child.checkStatus = newStatus;
				CheckAllNodes(child, newStatus);
			}
		}

		/// <summary>
		/// Counts only direct children that have <see cref="CheckStatus"/> set to <see cref="CheckStatuses.Checked"/>
		/// </summary>
		/// <returns>Count of checked direct children</returns>
		public int CountChecked()
		{
			int count = 0;
			foreach (CustomTreeNode child in Nodes)
			{
				if (child.CheckStatus == CheckStatuses.Checked)
					count++;
			}
			return count;
		}

		/// <summary>
		/// Counts all direct children that have <see cref="CheckStatus"/> set to either <see cref="CheckStatuses.Checked"/> or <see cref="CheckStatuses.Partial"/>
		/// </summary>
		/// <returns>Count of direct children that are fully or partially checked</returns>
		public int CountCheckAndPartial()
		{
			int count = CountChecked();
			foreach (CustomTreeNode child in Nodes)
			{
				if (child.CheckStatus == CheckStatuses.Partial)
					count++;
			}
			return count;
		}

		/// <summary>
		/// Checks wether or not <paramref name="node"/> has any sub nodes that pass <see cref="IsNodeChecked(CustomTreeNode)"/>
		/// </summary>
		/// <param name="node"><see cref="CustomTreeNode"/> to check for checked nodes</param>
		/// <returns><see langword="true"/> if a checked node was found and <see langword="false"/> otherwise</returns>
		public bool HasCheckedNodes(CustomTreeNode node)
		{
			bool hasCheckedChildren = false;

			foreach (CustomTreeNode child in node.Nodes)
			{
				if (IsNodeChecked(child))
					return true;
				hasCheckedChildren |= HasCheckedNodes(child);
			}

			return hasCheckedChildren;
		}

		/// <summary>
		/// Checks wether or not <paramref name="node"/> is checked or partially checked
		/// </summary>
		/// <param name="node"><see cref="CustomTreeNode"/> to check status of</param>
		/// <returns><see langword="true"/> if <paramref name="node"/> is checked or partially checked and <see langword="false"/> otherwise</returns>
		private bool IsNodeChecked(CustomTreeNode node)
		{
			return node.CheckStatus == CheckStatuses.Checked || node.CheckStatus == CheckStatuses.Partial;
		}
	}
}
