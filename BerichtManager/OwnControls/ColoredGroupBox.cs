using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BerichtManager.OwnControls
{
	/// <summary>
	/// <see cref="GroupBox"/> with customizable text and border color
	/// </summary>
	internal class ColoredGroupBox : GroupBox
	{
		/// <summary>
		/// Windows message code for paint message
		/// </summary>
		private const int WM_PAINT = 0xF;

		/// <summary>
		/// Color of outer line
		/// </summary>
		private Color borderColor = Color.FromArgb(220, 220, 220);
		/// <summary>
		/// Color of outer line
		/// </summary>
		[DefaultValue(typeof(Color), "220; 220; 220")]
		[Category("Colored Group Box")]
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
		/// Color of title
		/// </summary>
		private Color titleColor = SystemColors.ControlText;
		/// <summary>
		/// Color of title
		/// </summary>
		[DefaultValue(typeof(Color), "ControlText")]
		[Category("Colored Group Box")]
		public Color TitleColor
		{
			get => titleColor;
			set
			{
				if (titleColor != value)
				{
					titleColor = value;
					Invalidate();
				}
			}
		}

		/// <summary>
		/// Border style
		/// </summary>
		private BorderStyle borderStyle = BorderStyle.FixedSingle;
		/// <summary>
		/// Border style
		/// </summary>
		[DefaultValue(BorderStyle.FixedSingle)]
		[Category("Colored Group Box")]
		public BorderStyle BorderStyle
		{
			get => borderStyle;
			set
			{
				if (borderStyle != value)
				{
					borderStyle = value;
					Invalidate();
				}
			}
		}

		/// <summary>
		/// Border style
		/// </summary>
		private ButtonBorderStyle buttonBorderStyle = ButtonBorderStyle.Solid;
		/// <summary>
		/// Border style
		/// </summary>
		[DefaultValue(ButtonBorderStyle.Solid)]
		[Category("Colored Group Box")]
		public ButtonBorderStyle ButtonBorderStyle
		{
			get => buttonBorderStyle;
			set
			{
				if (buttonBorderStyle != value)
				{
					buttonBorderStyle = value;
					Invalidate();
				}
			}
		}


		private Color backColor = SystemColors.Control;
		public override Color BackColor
		{
			get => backColor;
			set
			{
				if (backColor != value)
				{
					backColor = value;
					Invalidate();
				}
			}
		}

		public ColoredGroupBox() : base()
		{
			SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.ContainerControl, true);
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == WM_PAINT)
			{
				using (Graphics g = Graphics.FromHwnd(m.HWnd))
				{
					g.Clear(BackColor);

					Size textSize = TextRenderer.MeasureText(this.Text, this.Font);
					Rectangle borderRect = ClientRectangle;
					borderRect.Y += textSize.Height / 2;
					borderRect.Height -= textSize.Height / 2;

					ControlPaint.DrawBorder(g, borderRect, BorderColor, buttonBorderStyle);

					Rectangle textRect = ClientRectangle;
					textRect.X += 6;
					textRect.Width = textSize.Width;
					textRect.Height = textSize.Height;

					using (SolidBrush b = new SolidBrush(BackColor))
					{
						g.FillRectangle(b, textRect);
					}
					using (SolidBrush b = new SolidBrush(TitleColor))
					{
						TextRenderer.DrawText(g, Text, Font, textRect, TitleColor);
					}
				}
				DefWndProc(ref m);
			}
			else
			{
				base.WndProc(ref m);
			}
		}

		//https://stackoverflow.com/questions/2612487/how-to-fix-the-flickering-in-user-controls
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
				return cp;
			}
		}
	}
}
