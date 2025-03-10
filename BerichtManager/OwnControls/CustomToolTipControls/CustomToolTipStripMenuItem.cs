using System.ComponentModel;

namespace BerichtManager.OwnControls.CustomToolTipControls
{
	public class CustomToolTipStripMenuItem : ToolStripMenuItem
	{
		public CustomToolTipStripMenuItem() : base()
		{

		}

		public CustomToolTipStripMenuItem(string? text = null, Image? image = null, EventHandler? onClick = null) : base(text, image, onClick)
		{

		}

		/// <summary>
		/// Indicates that the mouse is in the item
		/// </summary>
		private bool MouseIsIn { get; set; } = false;

		private string? toolTipText { get; set; } = null;
		/// <inheritdoc cref="ToolStripItem.ToolTipText" path=""/>
		[DefaultValue(null)]
		public new string? ToolTipText
		{
			get => toolTipText;
			set
			{
				if (toolTipText == value)
					return;
				toolTipText = value;
			}
		}

		/// <summary>
		/// Gets the menu that owns this item and the <see cref="ToolTip"/> to show tool tips on
		/// </summary>
		private ICustomToolTipControl? GetMenuStrip()
		{
			var item = OwnerItem;
			while (item?.OwnerItem != null)
			{
				item = item.OwnerItem;
			}
			return (item?.Owner ?? Owner) as ICustomToolTipControl;
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			MouseIsIn = false;
			GetMenuStrip()?.ItemMouseLeave();
		}

		protected override void OnDropDownOpened(EventArgs e)
		{
			base.OnDropDownOpened(e);
			if (!MouseIsIn)
				return;
			GetMenuStrip()?.ItemDropDownOpened(this);
		}

		protected override void OnMouseHover(EventArgs e)
		{
			base.OnMouseHover(e);
			GetMenuStrip()?.ItemMouseHover(this);
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);
			MouseIsIn = true;
		}
	}
}
