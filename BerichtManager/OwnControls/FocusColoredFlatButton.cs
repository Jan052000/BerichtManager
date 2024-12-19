using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BerichtManager.OwnControls
{
	public class FocusColoredFlatButton : Button
	{
		/// <summary>
		/// Used to check if the mouse is on the button
		/// </summary>
		private bool IsMouseHovering { get; set; }

		/// <summary>
		/// Width of the focus box arround edges of the button
		/// </summary>
		private float buttonFocusBoxWidth { get; set; } = 1f;
		/// <summary>
		/// Width of the focus box arround edges of the button
		/// </summary>
		[Category("Style")]
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
		[Category("Style")]
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

		/// <summary>
		/// BackColor of button when disabled
		/// </summary>
		private Color buttonDisabledColor { get; set; } = SystemColors.ControlDark;
		/// <summary>
		/// BackColor of button when disabled
		/// </summary>
		[Category("Style")]
		[DefaultValue(typeof(Color), "ControlDark")]
		public Color ButtonDisabledColor
		{
			get => buttonDisabledColor;
			set
			{
				if (buttonDisabledColor != value)
				{
					buttonDisabledColor = value;
					Invalidate();
				}
			}
		}

		/// <summary>
		/// BackColor of button when mouse is hovering
		/// </summary>
		private Color buttonHoverColor { get; set; } = Color.Gray;
		/// <summary>
		/// BackColor of button when mouse is hovering
		/// </summary>
		[Category("Style")]
		[DefaultValue(typeof(Color), "Gray")]
		public Color ButtonHoverColor
		{
			get => buttonHoverColor;
			set
			{
				if (buttonHoverColor != value)
				{
					buttonHoverColor = value;
					Invalidate();
				}
			}
		}

		/// <summary>
		/// Text color when button is disabled
		/// </summary>
		private Color buttonDisabledTextColor { get; set; } = SystemColors.GrayText;
		/// <summary>
		/// Text color when button is disabled
		/// </summary>
		[Category("Style")]
		[DefaultValue(typeof(Color), "GrayText")]
		public Color ButtonDisabledTextColor
		{
			get => buttonDisabledTextColor;
			set
			{
				if (buttonDisabledTextColor != value)
				{
					buttonDisabledTextColor = value;
					Invalidate();
				}
			}
		}

		/// <summary>
		/// Overwritten base FlatAppearance to hide it from designer
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue(typeof(FlatButtonAppearance), "")]
		public new string FlatAppearance { get; set; }

		/// <summary>
		/// Variable to pass FlatStyle through to base <see cref="Button"/> class
		/// </summary>
		[Browsable(true)]
		[Category("Style")]
		[DefaultValue(typeof(FlatStyle), "Standard")]
		public new FlatStyle FlatStyle { get => base.FlatStyle; set => base.FlatStyle = value; }

		public FocusColoredFlatButton()
		{
			//if (FlatStyle != FlatStyle.Flat)
			//base.FlatAppearance.BorderSize = 1;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (FlatStyle != FlatStyle.Flat)
			{
				base.OnPaint(e);
				return;
			}
			Rectangle focusBounds = new Rectangle(new Point(0, 0), Size);
			if (ButtonFocusBoxWidth == 1)
			{
				focusBounds.Width -= 1;
				focusBounds.Height -= 1;
			}
			Graphics g = e.Graphics;
			if (IsMouseHovering)
				g.Clear(ButtonHoverColor);
			else
				g.Clear(Enabled ? BackColor : ButtonDisabledColor);
			TextRenderer.DrawText(g, Text, Font, focusBounds, Enabled ? ForeColor : ButtonDisabledTextColor);

			if (Focused)
			{
				using (Pen p = new Pen(buttonFocusBoxColor, buttonFocusBoxWidth) { Alignment = System.Drawing.Drawing2D.PenAlignment.Inset })
					e.Graphics.DrawRectangle(p, focusBounds);
			}
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			IsMouseHovering = true;
			base.OnMouseEnter(e);
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			IsMouseHovering = false;
			base.OnMouseLeave(e);
		}
	}
}
