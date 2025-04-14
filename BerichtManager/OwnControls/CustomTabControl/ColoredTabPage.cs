using System.ComponentModel;

namespace BerichtManager.OwnControls.CustomTabControl
{
	/// <summary>
	/// Extends <see cref="TabPage"/> to improve styling options
	/// </summary>
	public class ColoredTabPage : TabPage
	{
		private Color tabHeadBackColor;
		/// <summary>
		/// <see cref="Color"/> that should be used for drawing the background of item head
		/// </summary>
		[Category("Theme")]
		[DefaultValue(typeof(Color), "Control")]
		public Color TabHeadBackColor
		{
			get => tabHeadBackColor;
			set
			{
				if (tabHeadBackColor != value)
				{
					tabHeadBackColor = value;
					Parent?.Invalidate();
				}
			}
		}

		private Color tabHeadTextColor;
		/// <summary>
		/// <see cref="Color"/> that should be used for drawing the item head text
		/// </summary>
		[Category("Theme")]
		[DefaultValue(typeof(Color), "ControlText")]
		public Color TabHeadTextColor
		{
			get => tabHeadTextColor;
			set
			{
				if (tabHeadTextColor != value)
				{
					tabHeadTextColor = value;
					Parent?.Invalidate();
				}
			}
		}
	}
}
