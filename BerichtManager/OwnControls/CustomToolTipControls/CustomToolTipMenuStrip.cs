using System.ComponentModel;

namespace BerichtManager.OwnControls.CustomToolTipControls
{
	internal class CustomToolTipMenuStrip : MenuStrip, ICustomToolTipControl
	{
		/// <summary>
		/// Indicator wether or not to show tool tips
		/// </summary>
		private bool showItemToolTips { get; set; }
		/// <summary>
		/// Gets or sets a value to indicate wether or not tool tips are shown and sets <see cref="ToolTip"/> accordingly
		/// </summary>
		[DefaultValue(false)]
		public new bool ShowItemToolTips
		{
			get => showItemToolTips;
			set
			{
				if (showItemToolTips == value)
					return;
				if (value)
				{
					ToolTip ??= new ToolTip();
				}
				else
				{
					ToolTip?.RemoveAll();
					ToolTip?.Dispose();
					ToolTip = null;
				}
				showItemToolTips = value;
			}
		}

		/// <summary>
		/// <see cref="System.Windows.Forms.ToolTip"/> used to show tool tips
		/// </summary>
		[Browsable(false)]
		internal ToolTip? ToolTip { get; private set; }

		public CustomToolTipMenuStrip()
		{
			base.ShowItemToolTips = false;
			showItemToolTips = DefaultShowItemToolTips;
		}

		public void ItemMouseHover(CustomToolTipStripMenuItem item)
		{
			Point p = PointToClient(MousePosition);
			Point toolTipPos = new Point(p.X, p.Y + item.Height + 1);
			ToolTip?.Show(item.ToolTipText, this, toolTipPos);
		}

		public void ItemMouseLeave()
		{
			ToolTip?.Hide(this);
			ToolTip?.RemoveAll();
		}

		public void ItemDropDownOpened(CustomToolTipStripMenuItem item)
		{
			ItemMouseHover(item);
		}
	}
}
