using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BerichtManager.OwnControls
{
	public class BorderlessForm : Form
	{
		#region Designer and form variables
		/// <summary>
		/// Sets height of title bar
		/// </summary>
		private int titleBarHeight { get; set; } = 32;
		/// <summary>
		/// Sets height of title bar
		/// </summary>
		[Category("Style")]
		[DefaultValue(32)]
		public int TitleBarHeight
		{
			get => titleBarHeight;
			set
			{
				if (titleBarHeight != value)
				{
					titleBarHeight = value;
					Invalidate();
				}
			}
		}

		/// <summary>
		/// Sets region outside of form which is able to trigger resize
		/// </summary>
		private int resizeHitbox { get; set; } = 3;
		/// <summary>
		/// Sets region outside of form which is able to trigger resize
		/// </summary>
		[Category("Style")]
		[DefaultValue(3)]
		public int ResizeHitbox
		{
			get => resizeHitbox;
			set
			{
				if (resizeHitbox != value)
				{
					resizeHitbox = value;
					Invalidate();
				}
			}
		}
		#endregion

		#region Windows message constants
		/// <summary>
		/// Message codes for windows message
		/// </summary>
		private enum WMMessageCodes
		{
			/// <summary>
			/// Message code for windows message WM_NCHITTEST
			/// </summary>
			WM_NCHITTEST = 0x84
		}
		/// <summary>
		/// Return values for "WM_NCHITTEST" message
		/// </summary>
		private enum NCHitTestResult
		{
			/// <summary>
			/// Return value for mouse over client area with no aditional functions
			/// </summary>
			HT_CLIENT = 0x1,
			/// <summary>
			/// Return value for mouse over with title bar
			/// </summary>
			HT_CAPTION = 0x2,
			/// <summary>
			/// Return value for mouse over left edge
			/// </summary>
			HT_LEFT = 0xA,
			/// <summary>
			/// Return value for mouse over right edge
			/// </summary>
			HT_RIGHT = 0xB,
			/// <summary>
			/// Return value for mouse over top edge
			/// </summary>
			HT_TOP = 0xC,
			/// <summary>
			/// Return value for mouse over top left point
			/// </summary>
			HT_TOPLEFT = 0xD,
			/// <summary>
			/// Return value for mouse over top right point
			/// </summary>
			HT_TOPRIGHT = 0xE,
			/// <summary>
			/// Return value for mouse over bottom edge
			/// </summary>
			HT_BOTTOM = 0xF,
			/// <summary>
			/// Return value for mouse over bottom left point
			/// </summary>
			HT_BOTTOMLEFT = 0x10,
			/// <summary>
			/// Return value for mouse over bottom right point
			/// </summary>
			HT_BOTTOMRIGHT = 0x11,
		}
		#endregion

		#region Overwrite variables for designer
		/// <summary>
		/// Overwrite and pass through for base.FormBorderStyle to change designer category
		/// </summary>
		[Category("Style")]
		[DefaultValue(typeof(FormBorderStyle), "None")]
		public new FormBorderStyle FormBorderStyle { get => base.FormBorderStyle; set => base.FormBorderStyle = value; }
		#endregion

		public BorderlessForm()
		{
			FormBorderStyle = FormBorderStyle.None;
		}

		protected override void WndProc(ref Message m)
		{
			if (FormBorderStyle != FormBorderStyle.None)
			{
				base.WndProc(ref m);
				return;
			}

			switch (m.Msg)
			{
				case (int)WMMessageCodes.WM_NCHITTEST:
					Point screenPoint = new Point(m.LParam.ToInt32());
					Point clientPoint = PointToClient(screenPoint);
					//Default result is client
					m.Result = (IntPtr)(NCHitTestResult.HT_CLIENT);

					if (clientPoint.Y < TitleBarHeight)
						m.Result = (IntPtr)(NCHitTestResult.HT_CAPTION);
					if (clientPoint.X <= ResizeHitbox && clientPoint.Y >= 0 + ResizeHitbox && clientPoint.Y <= Size.Height - ResizeHitbox)
						m.Result = (IntPtr)NCHitTestResult.HT_LEFT;
					else if (clientPoint.X >= Size.Width - ResizeHitbox && clientPoint.Y >= 0 + ResizeHitbox && clientPoint.Y <= Size.Height - ResizeHitbox)
						m.Result = (IntPtr)NCHitTestResult.HT_RIGHT;
					else if (clientPoint.Y <= ResizeHitbox && clientPoint.X >= ResizeHitbox && clientPoint.X <= Size.Width - ResizeHitbox)
						m.Result = (IntPtr)NCHitTestResult.HT_TOP;
					else if (clientPoint.X <= ResizeHitbox && clientPoint.Y <= ResizeHitbox)
						m.Result = (IntPtr)NCHitTestResult.HT_TOPLEFT;
					else if (clientPoint.X >= Size.Width - ResizeHitbox && clientPoint.Y <= ResizeHitbox)
						m.Result = (IntPtr)NCHitTestResult.HT_TOPRIGHT;
					else if (clientPoint.Y >= Size.Height - ResizeHitbox && clientPoint.X >= ResizeHitbox && clientPoint.X <= Size.Width - ResizeHitbox)
						m.Result = (IntPtr)NCHitTestResult.HT_BOTTOM;
					else if (clientPoint.X <= ResizeHitbox && clientPoint.Y >= Size.Height - ResizeHitbox)
						m.Result = (IntPtr)NCHitTestResult.HT_BOTTOMLEFT;
					else if (clientPoint.X >= Size.Width - ResizeHitbox && clientPoint.Y >= Size.Height - ResizeHitbox)
						m.Result = (IntPtr)NCHitTestResult.HT_BOTTOMRIGHT;
					break;
				default:
					base.WndProc(ref m);
					break;
			}
		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				//cp.Style |= 0x20000; // <--- use 0x20000
				cp.ExStyle |= 0x02000000;
				return cp;
			}
		}
	}
}
