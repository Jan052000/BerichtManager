using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BerichtManager.OwnControls
{
	public class FocusColoredFlatButton : Button
	{
		/// <summary>
		/// Width of the focus box arround edges of the button
		/// </summary>
		private float buttonFocusBoxWidth { get; set; } = 1f;
		/// <summary>
		/// Width of the focus box arround edges of the button
		/// </summary>
		[Category("Focus Border")]
		[DefaultValue(1f)]
		public float ButtonFocusBoxWidth
		{
			get => buttonFocusBoxWidth;
			set
			{
				if (buttonFocusBoxWidth != value)
				{
					buttonFocusBoxWidth = value;
					Invalidate();
				}
			}
		}

		/// <summary>
		/// Color of the focus box arround edges of the button
		/// </summary>
		private Color buttonFocusBoxColor { get; set; } = SystemColors.MenuHighlight;
		/// <summary>
		/// Color of the focus box arround edges of the button
		/// </summary>
		[Category("Focus Border")]
		[DefaultValue(typeof(Color), "MenuHighlight")]
		public Color ButtonFocusBoxColor
		{
			get => buttonFocusBoxColor;
			set
			{
				if (buttonFocusBoxColor != value)
				{
					buttonFocusBoxColor = value;
					Invalidate();
				}
			}
		}

		public FocusColoredFlatButton()
		{
			FlatStyle = FlatStyle.Flat;
			FlatAppearance.BorderSize = 0;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			Rectangle bounds = new Rectangle((int)e.Graphics.ClipBounds.X, (int)e.Graphics.ClipBounds.Y,
				(int)e.Graphics.ClipBounds.Width - (int)buttonFocusBoxWidth, (int)e.Graphics.ClipBounds.Height - (int)buttonFocusBoxWidth);
			if (Focused)
				using (Pen p = new Pen(buttonFocusBoxColor, buttonFocusBoxWidth))
					e.Graphics.DrawRectangle(p, bounds);
		}
	}
}
