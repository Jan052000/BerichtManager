using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BerichtManager.OwnControls
{
	/// <summary>
	/// <see cref="ComboBox"/> with customizable border and arrow colors
	/// </summary>
	public class ColoredComboBox : ComboBox
	{
		/// <summary>
		/// Windows message code for paint message
		/// </summary>
		private const int WM_PAINT = 0xF;
		/// <summary>
		/// Color of the inner border
		/// </summary>
		private Color borderColor = SystemColors.Window;
		/// <summary>
		/// Color of the inner border
		/// </summary>
		[Category("Colored Combo Box")]
		public Color BorderColor
		{
			get => borderColor;
			set
			{
				if (borderColor != value)
				{
					borderColor = value;
					Invalidate();
				}
			}
		}

		/// <summary>
		/// Color of the outer line border
		/// </summary>
		private Color outlineColor = Color.FromArgb(100, 100, 100);
		/// <summary>
		/// Color of the outer line border
		/// </summary>
		[Category("Colored Combo Box")]
		public Color OutlineColor
		{
			get => outlineColor;
			set
			{
				if (outlineColor != value)
				{
					outlineColor = value;
					Invalidate();
				}
			}
		}

		/// <summary>
		/// Color of the arrow drawn in dropdown button
		/// </summary>
		private Color arrowColor = Color.FromArgb(100, 100, 100);
		/// <summary>
		/// Color of the arrow drawn in dropdown button
		/// </summary>
		[Category("Colored Combo Box")]
		public Color ArrowColor
		{
			get => arrowColor;
			set
			{
				if (arrowColor != value)
				{
					arrowColor = value;
					Invalidate();
				}
			}
		}

		public ColoredComboBox()
		{
			SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
		}

		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);
			if (m.Msg == WM_PAINT)
			{
				var clientRect = ClientRectangle;
				var dropDownButtonWidth = SystemInformation.HorizontalScrollBarArrowWidth;
				var outerBorder = new Rectangle(clientRect.Location,
					new Size(clientRect.Width - 1, clientRect.Height - 1));
				var innerBorder = new Rectangle(outerBorder.X + 1, outerBorder.Y + 1,
					outerBorder.Width - dropDownButtonWidth - 2, outerBorder.Height - 2);
				var dropDownRect = new Rectangle(innerBorder.Right + 1, innerBorder.Y,
					dropDownButtonWidth, innerBorder.Height + 1);
				var middle = new Point(dropDownRect.Left + dropDownRect.Width / 2,
				dropDownRect.Top + dropDownRect.Height / 2);
				var arrow = new Point[]
				{
				new Point(middle.X - 3, middle.Y - 2),
				new Point(middle.X + 4, middle.Y - 2),
				new Point(middle.X, middle.Y + 2)
				};
				using (Graphics g = Graphics.FromHwnd(m.HWnd))
				{
					g.Clear(borderColor);
					using (Brush b = new SolidBrush(outlineColor))
					{
						g.FillRectangle(b, dropDownRect);
					}
					using (Pen pen = new Pen(outlineColor))
					{
						g.DrawRectangle(pen, outerBorder);
					}
					using (Brush b = new SolidBrush(arrowColor))
					{
						g.FillPolygon(b, arrow);
					}
				}
			}
		}
	}
}
