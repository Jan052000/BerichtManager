using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BerichtManager.OwnControls
{
	//Useful:
	//https://stackoverflow.com/questions/53000291/how-to-smooth-ugly-jitter-flicker-jumping-when-resizing-windows-especially-drag
	public class BorderlessForm : Form
	{
		#region Designer and form variables

		/// <summary>
		/// Variable that tells wether or not the form is currently active
		/// </summary>
		private bool IsActive { get; set; } = false;

		/// <summary>
		/// Stores which button the left mouse button clicked on
		/// </summary>
		private int ClickedButton { get; set; } = -1;

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
		/// Color of title bar when window is active
		/// </summary>
		private Color titleBarColorActive { get; set; } = SystemColors.ActiveCaption;
		/// <summary>
		/// Color of title bar when window is active
		/// </summary>
		[Category("Style")]
		[DefaultValue(typeof(Color), "ActiveCaption")]
		public Color TitleBarColorActive
		{
			get => titleBarColorActive;
			set
			{
				if (titleBarColorActive != value)
				{
					titleBarColorActive = value;
					Invalidate();
				}
			}
		}

		/// <summary>
		/// Color of title bar when window is inactive
		/// </summary>
		private Color titleBarColorInactive { get; set; } = SystemColors.InactiveCaption;
		/// <summary>
		/// Color of title bar when window is inactive
		/// </summary>
		[Category("Style")]
		[DefaultValue(typeof(Color), "InactiveCaption")]
		public Color TitleBarColorInactive
		{
			get => titleBarColorInactive;
			set
			{
				if (titleBarColorInactive != value)
				{
					titleBarColorInactive = value;
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

		/// <summary>
		/// Controls width of title bar buttons
		/// </summary>
		private int titleBarButtonWidth { get; set; } = 45;
		/// <summary>
		/// Controls width of title bar buttons
		/// </summary>
		[Category("Style")]
		[DefaultValue(45)]
		public int TitleBarButtonWidth
		{
			get => titleBarButtonWidth;
			set
			{
				if (titleBarButtonWidth != value)
				{
					titleBarButtonWidth = value;
					Invalidate();
				}
			}
		}

		/// <summary>
		/// Color that title ar buttons have when hovered
		/// </summary>
		private Color titleBarButtonHoverColor { get; set; } = SystemColors.ControlLight;
		/// <summary>
		/// Color that title ar buttons have when hovered
		/// </summary>
		[Category("Style")]
		[DefaultValue(typeof(Color), "Gray")]
		public Color TitleBarButtonHoverColor
		{
			get => titleBarButtonHoverColor;
			set
			{
				if (titleBarButtonHoverColor != value)
				{
					titleBarButtonHoverColor = value;
					Invalidate();
				}
			}
		}

		/// <summary>
		/// Color of the text in title bar buttons
		/// </summary>
		private Color titleBarButtonForeColor { get; set; } = Color.Black;
		/// <summary>
		/// Color of the text in title bar buttons
		/// </summary>
		[Category("Style")]
		[DefaultValue(typeof(Color), "Black")]
		public Color TitleBarButtonForeColor
		{
			get => titleBarButtonForeColor;
			set
			{
				if (titleBarButtonForeColor != value)
				{
					titleBarButtonForeColor = value;
					Invalidate();
				}
			}
		}

		/// <summary>
		/// Font size of the icons in title bar buttons
		/// </summary>
		private float titleBarButtonFontSize { get; set; } = 12;
		/// <summary>
		/// Font size of the icons in title bar buttons
		/// </summary>
		[Category("Style")]
		[DefaultValue(12)]
		public float TitleBarButtonFontSize
		{
			get => titleBarButtonFontSize;
			set
			{
				if (titleBarButtonFontSize != value)
				{
					titleBarButtonFontSize = value;
					Invalidate();
				}
			}
		}

		/// <summary>
		/// Indicates if and which button is hovered (right to left)
		/// </summary>
		private int hoveringButton { get; set; } = -1;
		/// <summary>
		/// Indicates if and which button is hovered (right to left)
		/// </summary>
		private int HoveringButton
		{
			get => hoveringButton;
			set
			{
				if (hoveringButton != value)
				{
					hoveringButton = value;
					SendMessage(Handle, (int)WMMessageCodes.WM_NCPAINT, 0, 0);
				}
			}
		}

		/// <summary>
		/// Color of the close button when hovered
		/// </summary>
		private Color closeButtonHoverColor { get; set; } = Color.Red;
		/// <summary>
		/// Color of the close button when hovered
		/// </summary>
		[Category("Style")]
		[DefaultValue(typeof(Color), "Red")]
		public Color CloseButtonHoverColor
		{
			get => closeButtonHoverColor;
			set
			{
				if (closeButtonHoverColor != value)
				{
					closeButtonHoverColor = value;
					Invalidate();
				}
			}
		}

		/// <summary>
		/// Bounds of the close button in title bar (right)
		/// </summary>
		private Rectangle CloseButtonBounds
		{
			get => new Rectangle(Width - TitleBarButtonWidth - ResizeHitbox, 2, TitleBarButtonWidth, TitleBarHeight - 1);
		}

		/// <summary>
		/// Bounds of the zoom button in title bar (middle)
		/// </summary>
		private Rectangle ZoomButtonBounds
		{
			get => new Rectangle(CloseButtonBounds.X - TitleBarButtonWidth - 1, CloseButtonBounds.Y, CloseButtonBounds.Width, CloseButtonBounds.Height);
		}

		/// <summary>
		/// Bounds of the reduce button in title bar (left)
		/// </summary>
		private Rectangle ReduceButtonBounds
		{
			get => new Rectangle(ZoomButtonBounds.X - TitleBarButtonWidth - 1, ZoomButtonBounds.Y, ZoomButtonBounds.Width, ZoomButtonBounds.Height);
		}
		#endregion

		#region Windows message constants

		/// <summary>
		/// Message codes for windows message
		/// </summary>
		private enum WMMessageCodes
		{
			/// <summary>
			/// Message code for windows message WM_ACIVATE
			/// </summary>
			WM_ACTIVATE = 0x6,
			/// <summary>
			/// Message code for painting
			/// </summary>
			WM_PAINT = 0xF,
			/// <summary>
			/// Message for erasing background
			/// </summary>
			WM_ERASEBKGND = 0x14,
			/// <summary>
			/// Message for calculating the new client size after resize
			/// </summary>
			WM_NCCALCSIZE = 0x83,
			/// <summary>
			/// Message for calculating if and what the cursor is interacting with
			/// </summary>
			WM_NCHITTEST = 0x84,
			/// <summary>
			/// Message for painting the form border
			/// </summary>
			WM_NCPAINT = 0x85,
			/// <summary>
			/// Sent if non client area needs to be updated when acive changed
			/// </summary>
			WM_NCACTIVATE = 0x86,
			/// <summary>
			/// Sent if left mouse button was clicked over non client area
			/// </summary>
			WM_NCLBUTTONDOWN = 0xA1,
			/// <summary>
			/// Sent if left button goes up over non client area
			/// </summary>
			WM_NCLBUTTONUP = 0xA2
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
			/// Return value for mouse over minimize button
			/// </summary>
			HT_MINBUTTON = 0x8,
			/// <summary>
			/// Return value for mouse over minimize button
			/// </summary>
			HT_REDUCE = 0x8,
			/// <summary>
			/// Return value for mouse over max/min button
			/// </summary>
			HT_MAXBUTTON = 0x9,
			/// <summary>
			/// Return value for mouse over max/min button
			/// </summary>
			HT_ZOOM = 0x9,
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
			/// <summary>
			/// Return value for mouse over closing button
			/// </summary>
			HT_CLOSE = 0x14
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

		#region External
		//RECT Structure
		[DllImport("user32.dll")]
		static extern IntPtr BeginPaint(IntPtr hwnd, out PAINTSTRUCT lpPaint);

		[DllImport("user32.dll")]
		static extern bool EndPaint(IntPtr hWnd, [In] ref PAINTSTRUCT lpPaint);

		[StructLayout(LayoutKind.Sequential)]
		public struct PAINTSTRUCT
		{
			public IntPtr hdc;
			public bool fErase;
			public Rectangle rcPaint;
			public bool fRestore;
			public bool fIncUpdate;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public byte[] rgbReserved;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int left, top, right, bottom;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct WINDOWPOS
		{
			public IntPtr hwnd;
			public IntPtr hwndinsertafter;
			public int x, y, cx, cy;
			public int flags;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct NCCALCSIZE_PARAMS
		{
			public RECT rgrc0, rgrc1, rgrc2;
			public WINDOWPOS lppos;
		}

		[DllImport("user32.dll", ExactSpelling = true)]
		public static extern IntPtr GetWindowDC(IntPtr hWnd);

		[DllImport("user32.dll", ExactSpelling = true)]
		public static extern IntPtr GetDCEx(IntPtr hWnd, IntPtr hrgnClip, int flags);

		[DllImport("user32.dll")]
		static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDc);

		[DllImport("user32.DLL")]
		private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);
		#endregion

		public BorderlessForm()
		{
			FormBorderStyle = FormBorderStyle.None;
			MinimumSize = new Size(3 * TitleBarButtonWidth + 3, TitleBarHeight);
		}

		/// <summary>
		/// Converts a <see cref="Point"/> on the screen to window coordinates
		/// </summary>
		/// <param name="p">Input point to convert</param>
		/// <returns>New <see cref="Point"/> with x and y coordinates relative to the window location</returns>
		private Point ToWindowCoordinates(Point p)
		{
			return new Point(p.X - Location.X, p.Y - Location.Y);
		}

		/// <summary>
		/// Paints the non client areas
		/// </summary>
		/// <param name="m">Reference to the <see cref="Message"/> recieved from <see cref="WndProc(ref Message)"/></param>
		private void WM_NCPAINT(ref Message m)
		{
			BeginPaint(m.HWnd, out PAINTSTRUCT ncpaint);
			IntPtr hdc = GetWindowDC(m.HWnd);
			using (Graphics graphics = Graphics.FromHdc(hdc))
			{
				graphics.ExcludeClip(new Rectangle(0, TitleBarHeight, Size.Width, Size.Height));
				graphics.Clear(IsActive ? TitleBarColorActive : TitleBarColorInactive);

				//Draw buttons
				using (SolidBrush b = new SolidBrush(TitleBarButtonHoverColor))
				using (SolidBrush red = new SolidBrush(CloseButtonHoverColor))
				{
					if (HoveringButton == 0)
						graphics.FillRectangle(red, CloseButtonBounds);
					TextRenderer.DrawText(graphics, "r", new Font("webdings", TitleBarButtonFontSize), CloseButtonBounds, TitleBarButtonForeColor);
					if (HoveringButton == 1)
						graphics.FillRectangle(b, ZoomButtonBounds);
					TextRenderer.DrawText(graphics, WindowState == FormWindowState.Maximized ? "2" : "1", new Font("webdings", TitleBarButtonFontSize), ZoomButtonBounds, TitleBarButtonForeColor);
					if (HoveringButton == 2)
						graphics.FillRectangle(b, ReduceButtonBounds);
					TextRenderer.DrawText(graphics, "0", new Font("webdings", TitleBarButtonFontSize), ReduceButtonBounds, TitleBarButtonForeColor);
				}
			}
			//If dc is not released then title bar will not update color unless form was resized prior
			ReleaseDC(m.HWnd, hdc);
			EndPaint(m.HWnd, ref ncpaint);
			m.Result = (IntPtr)0;
		}

		/// <summary>
		/// Evaluates which part of the window is being hit
		/// </summary>
		/// <param name="m">Reference to the <see cref="Message"/> recieved from <see cref="WndProc(ref Message)"/></param>
		private void WM_NCHITTEST(ref Message m)
		{
			Point screenPoint = new Point(IntPtr.Size == 8 ? unchecked((int)m.LParam.ToInt64()) : m.LParam.ToInt32());
			Point windowPoint = ToWindowCoordinates(screenPoint);
			NCHitTestResult result = NCHitTestResult.HT_CLIENT;
			Rectangle titleBar = new Rectangle(Location.X, Location.Y, Size.Width, TitleBarHeight);
			if (titleBar.Contains(screenPoint))
			{
				if (CloseButtonBounds.Contains(windowPoint))
					HoveringButton = 0;
				else if (ZoomButtonBounds.Contains(windowPoint))
					HoveringButton = 1;
				else if (ReduceButtonBounds.Contains(windowPoint))
					HoveringButton = 2;
				else
					HoveringButton = -1;
				//HoveringButton = (Width - 2 - windowPoint.X) / (TitleBarButtonWidth + 1);
				switch (HoveringButton)
				{
					case 0:
						result = NCHitTestResult.HT_CLOSE;
						break;
					case 1:
						result = NCHitTestResult.HT_ZOOM;
						break;
					case 2:
						result = NCHitTestResult.HT_REDUCE;
						break;
					default:
						result = NCHitTestResult.HT_CAPTION;
						break;
				}
			}
			else
				HoveringButton = -1;
			if (windowPoint.X <= ResizeHitbox && windowPoint.Y >= 0 + ResizeHitbox && windowPoint.Y <= Size.Height - ResizeHitbox)
				result = NCHitTestResult.HT_LEFT;
			else if (windowPoint.X >= Size.Width - ResizeHitbox && windowPoint.Y >= 0 + ResizeHitbox && windowPoint.Y <= Size.Height - ResizeHitbox)
				result = NCHitTestResult.HT_RIGHT;
			else if (windowPoint.Y <= ResizeHitbox && windowPoint.X >= ResizeHitbox && windowPoint.X <= Size.Width - ResizeHitbox)
				result = NCHitTestResult.HT_TOP;
			else if (windowPoint.X <= ResizeHitbox && windowPoint.Y <= ResizeHitbox)
				result = NCHitTestResult.HT_TOPLEFT;
			else if (windowPoint.X >= Size.Width - ResizeHitbox && windowPoint.Y <= ResizeHitbox)
				result = NCHitTestResult.HT_TOPRIGHT;
			else if (windowPoint.Y >= Size.Height - ResizeHitbox && windowPoint.X >= ResizeHitbox && windowPoint.X <= Size.Width - ResizeHitbox)
				result = NCHitTestResult.HT_BOTTOM;
			else if (windowPoint.X <= ResizeHitbox && windowPoint.Y >= Size.Height - ResizeHitbox)
				result = NCHitTestResult.HT_BOTTOMLEFT;
			else if (windowPoint.X >= Size.Width - ResizeHitbox && windowPoint.Y >= Size.Height - ResizeHitbox)
				result = NCHitTestResult.HT_BOTTOMRIGHT;
			m.Result = (IntPtr)result;
		}

		/// <summary>
		/// Calculates and sets the new client area
		/// </summary>
		/// <param name="m">Reference to the <see cref="Message"/> recieved from <see cref="WndProc(ref Message)"/></param>
		/// https://stackoverflow.com/questions/28277039/how-to-set-the-client-area-clientrectangle-in-a-borderless-form
		private void WM_NCCALCSIZE(ref Message m)
		{
			if (m.WParam != (IntPtr)0)
			{
				NCCALCSIZE_PARAMS nccp = (NCCALCSIZE_PARAMS)Marshal.PtrToStructure(m.LParam, typeof(NCCALCSIZE_PARAMS));
				nccp.rgrc0.top += TitleBarHeight;
				Marshal.StructureToPtr(nccp, m.LParam, true);

			}
			else
			{
				RECT rect = (RECT)Marshal.PtrToStructure(m.LParam, typeof(RECT));
				rect.top += TitleBarHeight;
				Marshal.StructureToPtr(rect, m.LParam, true);
			}
			m.Result = (IntPtr)0;
		}

		/// <summary>
		/// Handles the activation and deactivation of the window border
		/// </summary>
		/// <param name="m">Reference to the <see cref="Message"/> recieved from <see cref="WndProc(ref Message)"/></param>
		private void WM_NCACTIVATE(ref Message m)
		{
			if (WindowState == FormWindowState.Minimized)
				base.DefWndProc(ref m);
			else
			{
				IsActive = m.WParam.ToInt32() == 1;
				WM_NCPAINT(ref m);
				m.Result = (IntPtr)1;
			}
		}

		/// <summary>
		/// Hanldes left button down in non client area
		/// </summary>
		/// <param name="m">Reference to the <see cref="Message"/> recieved from <see cref="WndProc(ref Message)"/></param>
		private void WM_NCLBUTTONDOWN(ref Message m)
		{
			switch ((NCHitTestResult)m.WParam)
			{
				case NCHitTestResult.HT_CLOSE:
					ClickedButton = 0;
					break;
				case NCHitTestResult.HT_ZOOM:
					ClickedButton = 1;
					break;
				case NCHitTestResult.HT_REDUCE:
					ClickedButton = 2;
					break;
				default:
					ClickedButton = -1;
					base.WndProc(ref m);
					break;
			}
		}

		/// <summary>
		/// Hanldes left button up in non client area
		/// </summary>
		/// <param name="m">Reference to the <see cref="Message"/> recieved from <see cref="WndProc(ref Message)"/></param>
		private void WM_NCLBUTTONUP(ref Message m)
		{
			switch ((NCHitTestResult)m.WParam)
			{
				case NCHitTestResult.HT_CLOSE:
					if (ClickedButton == 0)
						Close();
					break;
				case NCHitTestResult.HT_ZOOM:
					if (ClickedButton == 1)
						WindowState = WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
					break;
				case NCHitTestResult.HT_REDUCE:
					if (ClickedButton == 2)
						WindowState = FormWindowState.Minimized;
					break;
				default:
					ClickedButton = -1;
					base.WndProc(ref m);
					break;
			}
		}

		protected override void WndProc(ref Message m)
		{
			if (FormBorderStyle != FormBorderStyle.None)
			{
				base.WndProc(ref m);
				return;
			}

			switch ((WMMessageCodes)m.Msg)
			{
				case WMMessageCodes.WM_NCHITTEST:
					WM_NCHITTEST(ref m);
					break;
				case WMMessageCodes.WM_NCPAINT:
					WM_NCPAINT(ref m);
					break;
				case WMMessageCodes.WM_NCCALCSIZE:
					WM_NCCALCSIZE(ref m);
					break;
				case WMMessageCodes.WM_NCACTIVATE:
					WM_NCACTIVATE(ref m);
					break;
				case WMMessageCodes.WM_NCLBUTTONDOWN:
					WM_NCLBUTTONDOWN(ref m);
					break;
				case WMMessageCodes.WM_NCLBUTTONUP:
					WM_NCLBUTTONUP(ref m);
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