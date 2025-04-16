namespace BerichtManager.OwnControls.CustomTabControl
{
	/// <summary>
	/// Wrapper class to hook into functions of <see cref="TabControl.ControlCollection"/>
	/// </summary>
	public class ControlCollectionWrapper : TabControl.ControlCollection
	{
		/// <summary>
		/// Creates a new <see cref="ControlCollectionWrapper"/> instance
		/// </summary>
		/// <param name="owner"><see cref="ColoredTabControl"/> that owns this <see cref="ControlCollectionWrapper"/></param>
		public ControlCollectionWrapper(ColoredTabControl owner) : base(owner)
		{

		}

		public override void Add(Control? value)
		{
			if (value is not TabPage tabPage)
				throw new ArgumentException("Value is not a TabPage", nameof(value));
			if (Owner is ColoredTabControl tabControl)
				tabControl.TabPages.Add(tabPage);
			else
				base.Add(value);
		}

		public override void AddRange(Control[] controls)
		{
			for (int i = 0; i < controls.Length; i++)
			{
				Add(controls[i]);
			}
		}
	}
}
