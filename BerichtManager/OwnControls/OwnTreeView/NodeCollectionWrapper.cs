using System.Collections;

namespace BerichtManager.OwnControls.OwnTreeView
{
	public class NodeCollectionWrapper : IList, ICollection, IEnumerable
	{
		#region IEnumerable
		public IEnumerator GetEnumerator()
		{
			return Collection.GetEnumerator();
		}
		#endregion

		#region ICollection
		public int Count => Collection.Count;

		object ICollection.SyncRoot => ((ICollection)Collection).SyncRoot;

		bool ICollection.IsSynchronized => ((ICollection)Collection).IsSynchronized;

		public void CopyTo(Array array, int index)
		{
			Collection.CopyTo(array, index);
		}
		#endregion

		#region IList
		object IList.this[int index]
		{
			get => this[index];
			set
			{
				if (value is CustomTreeNode ctn)
				{
					this[index] = ctn;
					return;
				}

				throw new ArgumentException("Invalid node", "value");
			}
		}

		public bool IsReadOnly => Collection.IsReadOnly;

		bool IList.IsFixedSize => ((IList)Collection).IsFixedSize;

		int IList.Add(object value)
		{
			if (value is CustomTreeNode ctn)
				return Add(ctn);
			return Add(value.ToString()).Index;
		}

		public void Clear()
		{
			Collection.Clear();
		}

		bool IList.Contains(object value)
		{
			if (value is CustomTreeNode ctn)
				return Contains(ctn);
			return false;
		}

		int IList.IndexOf(object value)
		{
			if (value is CustomTreeNode ctn)
				return IndexOf(ctn);
			return -1;
		}

		void IList.Insert(int index, object value)
		{
			if (value is CustomTreeNode ctn)
				Insert(index, ctn);
		}

		void IList.Remove(object value)
		{
			if (value is CustomTreeNode ctn)
				Remove(ctn);
		}

		public void RemoveAt(int index)
		{
			Collection.RemoveAt(index);
		}
		#endregion

		private TreeNodeCollection Collection { get; set; }

		private CustomTreeNode Owner { get; }

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

		public int Add(CustomTreeNode item)
		{
			if (item.Parent != null)
				throw new System.Exception("Can not add node that already has a parent");
			return Collection.Add(item);
		}

		public CustomTreeNode Add(string text)
		{
			CustomTreeNode node = new CustomTreeNode(text);
			Collection.Add(node);
			return node;
		}

		public void AddRange(List<CustomTreeNode> nodes)
		{
			Collection.AddRange(nodes.ToArray());
		}

		public void AddRange(CustomTreeNode[] nodes)
		{
			Collection.AddRange(nodes);
		}

		public bool Contains(CustomTreeNode item)
		{
			return Collection.Contains(item);
		}

		public void CopyTo(CustomTreeNode[] array, int arrayIndex)
		{
			Collection.CopyTo(array, arrayIndex);
		}

		public void Remove(CustomTreeNode item)
		{
			if (Collection.Contains(item))
				Collection.Remove(item);
		}
	}
}
