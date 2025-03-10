using System.ComponentModel;

namespace BerichtManager.OwnControls.CustomToolTipControls
{
	public class CustomToolTipContextMenuStrip : ContextMenuStrip, ICustomToolTipControl
	{
		/// <summary>
		/// Indicator wether or not to show tool tips
		/// </summary>
		private bool showItemToolTips { get; set; } = true;
		/// <summary>
		/// Gets or sets a value to indicate wether or not tool tips are shown and sets <see cref="ToolTip"/> accordingly
		/// </summary>
		[DefaultValue(true)]
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
		internal ToolTip? ToolTip { get; private set; }

		public CustomToolTipContextMenuStrip() : base()
		{
			Init();
		}

		public CustomToolTipContextMenuStrip(IContainer container) : base(container)
		{
			Init();
		}

		private void Init()
		{
			base.ShowItemToolTips = false;
			if (DefaultShowItemToolTips)
				ToolTip = new ToolTip();
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

		protected override void OnClosing(ToolStripDropDownClosingEventArgs e)
		{
			base.OnClosing(e);
			ToolTip?.RemoveAll();
		}
	}
}
