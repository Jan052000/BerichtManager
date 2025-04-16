namespace BerichtManager.OwnControls.CustomTabControl
{
	/// <summary>
	/// Object representing the visibility state of a <see cref="System.Windows.Forms.TabPage"/>
	/// </summary>
	public class TabPageState
	{
		/// <summary>
		/// <see cref="System.Windows.Forms.TabPage"/> to track visibility of
		/// </summary>
		public TabPage TabPage { get; set; }
		/// <summary>
		/// Visibility status of <see cref="TabPage"/>
		/// </summary>
		public bool Visible { get; set; }

		/// <summary>
		/// Creates a new <see cref="TabPageState"/> instance
		/// </summary>
		/// <param name="tabPage"><inheritdoc cref="TabPage" path="/summary"/></param>
		/// <param name="visible"><inheritdoc cref="Visible" path="/summary"/></param>
		public TabPageState(TabPage tabPage, bool visible = true)
		{
			TabPage = tabPage;
			Visible = visible;
		}
	}
}
