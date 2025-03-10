namespace BerichtManager.OwnControls.CustomToolTipControls
{
	/// <summary>
	/// Interface for Controls that want to show custom tool tips using <see cref="CustomToolTipStripMenuItem"/>s
	/// </summary>
	public interface ICustomToolTipControl
	{
		/// <summary>
		/// Handler for the MouseEnter event
		/// </summary>
		/// <param name="item"><see cref="CustomToolTipStripMenuItem"/> that was entered</param>
		public void ItemMouseHover(CustomToolTipStripMenuItem item);
		/// <summary>
		/// Handler for the MouseLeave event
		/// </summary>
		public void ItemMouseLeave();
		/// <summary>
		/// Handler for the DropDownOpened event
		/// <br/>User should redraw the tool tip as it will be behind the drop down and item
		/// </summary>
		/// <param name="item"><see cref="CustomToolTipStripMenuItem"/> that had its drop down opened</param>
		public void ItemDropDownOpened(CustomToolTipStripMenuItem item);
	}
}
