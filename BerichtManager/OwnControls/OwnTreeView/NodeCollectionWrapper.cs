using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace BerichtManager.OwnControls.OwnTreeView
{
	public class NodeCollectionWrapper : IList<CustomTreeNode>
	{
		private TreeNodeCollection Collection { get; set; }

		private CustomTreeNode Owner { get; }

		public int Count => Collection.Count;

		public bool IsReadOnly => Collection.IsReadOnly;

		public CustomTreeNode this[int index] { get => (CustomTreeNode)Collection[index]; set => Collection[index] = value; }
		public CustomTreeNode this[string key] { get => (CustomTreeNode)Collection[key]; }

		internal NodeCollectionWrapper(TreeNodeCollection nodeCollection, CustomTreeNode owner)
		{
			Collection = nodeCollection;
			Owner = owner;
		}

		public int IndexOf(CustomTreeNode item)
		{
			return Collection.IndexOf(item);
		}

		public void Insert(int index, CustomTreeNode item)
		{
			Collection.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			Collection.RemoveAt(index);
		}

		public void Add(CustomTreeNode item)
		{
			if (item.Parent != null)
				throw new System.Exception("Can not add node that already has a parent");
			Collection.Add(item);
		}

		public void Add(string text)
		{
			Collection.Add(new CustomTreeNode(text));
		}

		public void AddRange(List<CustomTreeNode> nodes)
		{
			Collection.AddRange(nodes.ToArray());
		}

		public void AddRange(CustomTreeNode[] nodes)
		{
			Collection.AddRange(nodes);
		}

		public void Clear()
		{
			Collection.Clear();
		}

		public bool Contains(CustomTreeNode item)
		{
			return Collection.Contains(item);
		}

		public void CopyTo(CustomTreeNode[] array, int arrayIndex)
		{
			Collection.CopyTo(array, arrayIndex);
		}

		public bool Remove(CustomTreeNode item)
		{
			if (!Collection.Contains(item))
				return false;
			Collection.Remove(item);
			return !Collection.Contains(item);
		}

		public IEnumerator<CustomTreeNode> GetEnumerator()
		{
			return Collection.Cast<CustomTreeNode>().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Collection.GetEnumerator();
		}
	}
}
