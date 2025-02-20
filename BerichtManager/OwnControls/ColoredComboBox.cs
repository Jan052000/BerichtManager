using System.ComponentModel;

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
		[DefaultValue("Window")]
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
		[DefaultValue(typeof(Color), "100; 100; 100")]
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
		/// Color of the drop down button background
		/// </summary>
		private Color dropDownButtonColor = SystemColors.Menu;
		/// <summary>
		/// Color of the drop down button background
		/// </summary>
		[Category("Colored Combo Box")]
		[DefaultValue("Menu")]
		public Color DropDownButtonColor
		{
			get => dropDownButtonColor;
			set
			{
				if (dropDownButtonColor != value)
				{
					dropDownButtonColor = value;
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
		[DefaultValue(typeof(Color), "100; 100; 100")]
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

		/// <summary>
		/// Color to be used when control is disabled
		/// </summary>
		private Color disabledColor = SystemColors.Control;
		/// <summary>
		/// Color to be used when control is disabled
		/// </summary>
		[Category("Colored Combo Box")]
		[DefaultValue("Control")]
		public Color DisabledColor
		{
			get => disabledColor;
			set
			{
				if (disabledColor != value)
				{
					disabledColor = value;
					Invalidate();
				}
			}
		}

		/// <summary>
		/// Color to be used for text when control is disabled
		/// </summary>
		private Color disabledTextColor = SystemColors.GrayText;
		/// <summary>
		/// Color to be used for text when control is disabled
		/// </summary>
		[Category("Colored Combo Box")]
		[DefaultValue("GrayText")]
		public Color DisabledTextColor
		{
			get => disabledTextColor;
			set
			{
				if (disabledTextColor != value)
				{
					disabledTextColor = value;
					Invalidate();
				}
			}
		}

		/// <summary>
		/// Color of displayed text
		/// </summary>
		private Color textColor = SystemColors.WindowText;
		/// <summary>
		/// Color of displayed text
		/// </summary>
		[Category("Colored Combo Box")]
		[DefaultValue("WindowText")]
		public Color TextColor
		{
			get => textColor;
			set
			{
				if (textColor != value)
				{
					textColor = value;
					Invalidate();
				}
			}
		}

		/// <summary>
		/// Color of hilighted item from drop down list
		/// </summary>
		private Color highlightColor = SystemColors.Highlight;
		/// <summary>
		/// Color of hilighted item from drop down list
		/// </summary>
		[Category("Colored Combo Box")]
		[DefaultValue("Highlight")]
		public Color HighlightColor
		{
			get => highlightColor;
			set
			{
				if (highlightColor != value)
				{
					highlightColor = value;
					Invalidate();
				}
			}
		}

		private ComboBoxStyle dropDownStyle = ComboBoxStyle.DropDownList;
		[DefaultValue("DropDownList")]
		public new ComboBoxStyle DropDownStyle
		{
			get => dropDownStyle;
			set
			{
				if (base.DropDownStyle != value)
					base.DropDownStyle = value;
				if (dropDownStyle != value)
				{
					dropDownStyle = value;
					Invalidate();
				}
			}
		}

		private DrawMode drawMode = DrawMode.OwnerDrawFixed;
		[DefaultValue("OwnerDrawFixed")]
		public new DrawMode DrawMode
		{
			get => drawMode;
			set
			{
				if (base.DrawMode != value)
					base.DrawMode = value;
				if (drawMode != value)
				{
					drawMode = value;
					Invalidate();
				}
			}
		}

		public ColoredComboBox()
		{
			SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.CacheText, true);
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == WM_PAINT && DropDownStyle != ComboBoxStyle.Simple)
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
				DefWndProc(ref m);
				using (Graphics g = Graphics.FromHwnd(m.HWnd))
				{
					g.Clear(Enabled ? BackColor : disabledColor);
					using (Brush b = Enabled ? new SolidBrush(dropDownButtonColor) : new SolidBrush(disabledColor))
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
					if (DropDownStyle == ComboBoxStyle.DropDownList)
						TextRenderer.DrawText(g, Text, Font, new Point(innerBorder.X, innerBorder.Y + 3), Enabled ? textColor : disabledTextColor);
				}
			}
			else
				base.WndProc(ref m);
		}

		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			e.DrawBackground();
			if (e.Index >= 0)
			{
				using (Brush b = (e.State & DrawItemState.Selected) == DrawItemState.Selected ? new SolidBrush(highlightColor) : new SolidBrush(BackColor))
				{
					e.Graphics.FillRectangle(b, e.Bounds);
				}
				TextRenderer.DrawText(e.Graphics, Items[e.Index].ToString(), Font, new Point(e.Bounds.X, e.Bounds.Y), textColor);
			}
			e.DrawFocusRectangle();
		}
	}
}
