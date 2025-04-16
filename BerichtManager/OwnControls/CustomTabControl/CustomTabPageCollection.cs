using System.Collections;

namespace BerichtManager.OwnControls.CustomTabControl
{
	/// <summary>
	/// The <see cref="CustomTabPageCollection"/> expands the default <see cref="TabControl.TabPageCollection"/> to allow for hiding and showing individual <see cref="TabPage"/>s
	/// </summary>
	public class CustomTabPageCollection : IList, ICollection, IEnumerable
	{
		/// <summary>
		/// <see cref="TabControl.TabPageCollection"/> of <see cref="ColoredTabControl"/> owning the collection
		/// </summary>
		protected TabControl.TabPageCollection BaseTabPages { get; }

		/// <summary>
		/// Internal <see cref="List{T}"/> of <see cref="TabPage"/>s used to synchronize with hidden tabs
		/// </summary>
		protected List<TabPageState> InternalTabPageOrder { get; } = new List<TabPageState>();

		/// <summary>
		/// Copy of <see cref="List{T}"/> containing both hidden and shown <see cref="TabPage"/>s
		/// </summary>
		public List<TabPageState> AllPages => [.. InternalTabPageOrder];

		/// <summary>
		/// Gets or sets the element at the specified index
		/// </summary>
		/// <param name="index">The zero based index of the element to get or set</param>
		/// <returns>The element at the specified index</returns>
		public TabPage this[int index] { get => BaseTabPages[index]; set => BaseTabPages[index] = value; }

		/// <summary>
		/// Gets a <see cref="TabPage"/> at the specified key
		/// </summary>
		/// <param name="key">Key of <see cref="TabPage"/> to get</param>
		/// <returns>The <see cref="TabPage"/> with the specified key</returns>
		public TabPage? this[string? key] { get => BaseTabPages[key]; }

		/// <summary>
		/// Creates a new <see cref="CustomTabPageCollection"/> instance
		/// </summary>
		/// <param name="owner"><see cref="TabControl"/> that owns the <see cref="CustomTabPageCollection"/></param>
		public CustomTabPageCollection(TabControl owner)
		{
			BaseTabPages = owner.TabPages;
			foreach (TabPage page in BaseTabPages)
			{
				InternalTabPageOrder.Add(new TabPageState(page));
			}
		}

		/// <summary>
		/// Clears all <see cref="TabPage"/>s
		/// </summary>
		public void Clear()
		{
			BaseTabPages.Clear();
			InternalTabPageOrder.Clear();
		}

		/// <summary>
		/// Hides all <see cref="TabPage"/>s
		/// </summary>
		public void HideAllPages()
		{
			BaseTabPages.Clear();
			InternalTabPageOrder.ForEach(state => state.Visible = false);
		}

		/// <summary>
		/// Hides a specific <paramref name="page"/>
		/// </summary>
		/// <param name="page"><see cref="TabPage"/> to hide</param>
		public void Hide(TabPage page)
		{
			if (InternalTabPageOrder.Find(p => p.TabPage == page) is not TabPageState toHide)
				return;
			BaseTabPages.Remove(page);
			toHide.Visible = false;
		}

		/// <summary>
		/// Shows a shows a specific hidden <see cref="TabPage"/>
		/// </summary>
		/// <param name="page"><see cref="TabPage"/> to show</param>
		public void Show(TabPage page)
		{
			if (InternalTabPageOrder.Find(p => p.TabPage == page) is not TabPageState toShow || toShow.Visible)
				return;
			int shownIndex = 0;
			for (int i = 0; i < InternalTabPageOrder.Count; i++)
			{
				TabPageState pageState = InternalTabPageOrder[i];
				if (pageState.TabPage == page)
					break;
				if (pageState.Visible)
					shownIndex++;
			}
			BaseTabPages.Insert(shownIndex, page);
			toShow.Visible = true;
		}

		/// <summary>
		/// Adds a <see cref="TabPage"/> to the end of <see cref="TabControl"/> tabs
		/// </summary>
		/// <param name="page"><see cref="TabPage"/> to add to <see cref="TabControl"/> tabs</param>
		public void Add(TabPage page)
		{
			BaseTabPages.Add(page);
			InternalTabPageOrder.Add(new TabPageState(page));
		}

		/// <summary>
		/// Adds a range of <see cref="TabPage"/>s to the end of <see cref="TabControl"/> tabs
		/// </summary>
		/// <param name="pages"><see cref="TabPage"/>s to add to <see cref="TabControl"/> tabs</param>
		public void AddRange(TabPage[] pages)
		{
			foreach (TabPage page in pages)
			{
				Add(page);
			}
		}

		/// <summary>
		/// Removes a specific <see cref="TabPage"/> from <see cref="TabControl"/> tabs
		/// </summary>
		/// <param name="page"><see cref="TabPage"/> to remofe from <see cref="TabControl"/> tabs</param>
		public void Remove(TabPage page)
		{
			BaseTabPages.Remove(page);
			InternalTabPageOrder.RemoveAll(state => state.TabPage == page);
		}

		/// <summary>
		/// Removes a <see cref="TabPage"/> at the specified <paramref name="index"/>
		/// </summary>
		/// <param name="index">Index of <see cref="TabPage"/> to remove</param>
		public void RemoveAt(int index)
		{
			TabPage page = BaseTabPages[index];
			BaseTabPages.RemoveAt(index);
			if (InternalTabPageOrder.Find(state => state.TabPage == page) is TabPageState toRemove)
				InternalTabPageOrder.Remove(toRemove);
		}

		/// <summary>
		/// Calculates the index that a <see cref="TabPage"/> should have inside of <see cref="InternalTabPageOrder"/> to appear at <paramref name="normalIndex"/> in <see cref="BaseTabPages"/>
		/// </summary>
		/// <param name="normalIndex"></param>
		/// <returns></returns>
		private int GetInternalInsertIndex(int normalIndex)
		{
			int visibleCount = 0;
			for (int i = 0; i < InternalTabPageOrder.Count; i++)
			{
				if (normalIndex == visibleCount)
					return i;
				if (InternalTabPageOrder[i].Visible)
					visibleCount++;
			}
			return 0;
		}

		/// <summary>
		/// Inserts <paramref name="tabPage"/> at the desired <paramref name="index"/>
		/// </summary>
		/// <param name="index">Index to add tab at</param>
		/// <param name="tabPage">Tab to insert</param>
		public void Insert(int index, TabPage tabPage)
		{
			BaseTabPages.Insert(index, tabPage);
			InternalTabPageOrder.Insert(GetInternalInsertIndex(index), new TabPageState(tabPage));
		}

		#region IEnumerable
		IEnumerator IEnumerable.GetEnumerator()
		{
			return BaseTabPages.GetEnumerator();
		}
		#endregion

		#region ICollection
		public int Count => BaseTabPages.Count;

		bool ICollection.IsSynchronized => ((ICollection)BaseTabPages).IsSynchronized;

		object ICollection.SyncRoot => ((ICollection)BaseTabPages).SyncRoot;

		void ICollection.CopyTo(Array array, int index)
		{
			((ICollection)BaseTabPages).CopyTo(array, index);
		}
		#endregion

		#region IList
		bool IList.IsFixedSize => ((IList)BaseTabPages).IsFixedSize;

		bool IList.IsReadOnly => ((IList)BaseTabPages).IsReadOnly;

		object? IList.this[int index] { get => BaseTabPages[index]; set => BaseTabPages[index] = value as TabPage ?? throw new ArgumentException("Value is not a TabPage", nameof(value)); }

		int IList.Add(object? value)
		{
			if (value is not TabPage page)
				throw new ArgumentException("Value is not a TabPage", nameof(value));
			Add(page);
			return BaseTabPages.IndexOf(page);
		}

		bool IList.Contains(object? value)
		{
			if (value is not TabPage page)
				throw new ArgumentException("Value is not a TabPage", nameof(value));
			return BaseTabPages.Contains(page);
		}

		int IList.IndexOf(object? value)
		{
			if (value is not TabPage page)
				throw new ArgumentException("Value is not a TabPage", nameof(value));
			return BaseTabPages.IndexOf(page);
		}

		void IList.Insert(int index, object? value)
		{
			if (value is not TabPage page)
				throw new ArgumentException("Value is not a TabPage", nameof(value));
			Insert(index, page);
		}

		void IList.Remove(object? value)
		{
			if (value is not TabPage page)
				throw new ArgumentException("Value is not a TabPage", nameof(value));
			Remove(page);
		}
		#endregion

	}
}
