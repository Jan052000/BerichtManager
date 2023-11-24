﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BerichtManager.OwnControls
{
	//https://stackoverflow.com/questions/53000291/how-to-smooth-ugly-jitter-flicker-jumping-when-resizing-windows-especially-drag
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
		/// Color of title bar when window is focused
		/// </summary>
		private Color titleBarColorActive { get; set; } = Color.Red;//SystemColors.ActiveBorder;
		/// <summary>
		/// Color of title bar when window is focused
		/// </summary>
		[Category("Style")]
		[DefaultValue(typeof(Color), "ActiveBorder")]
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

		private Color titleBarColorInactive { get; set; } = Color.Blue;//SystemColors.InactiveBorder;
		[Category("Style")]
		[DefaultValue(typeof(Color), "InactiveBorder")]
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

		private bool IsActive { get; set; } = false;

		/// <summary>
		/// Indicates wether or not the close button in top right corner should be re when hovered
		/// </summary>
		[Category("Style")]
		[DefaultValue(true)]
		private bool RedCloseButtonHover { get; set; } = true;

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
			/// Message code for windows message WM_ACIVATE
			/// </summary>
			WM_ACTIVATE = 0x6,
			/// <summary>
			/// Message code for painting
			/// </summary>
			WM_PAINT = 0xF,
			/// <summary>
			/// Message for erasing content by drawing background
			/// </summary>
			WM_ERASEBKGND = 0x14,
			/// <summary>
			/// Message code for windows message WM_NCCALCSIZE
			/// </summary>
			WM_NCCALCSIZE = 0x83,
			/// <summary>
			/// Message code for windows message WM_NCHITTEST
			/// </summary>
			WM_NCHITTEST = 0x84,
			/// <summary>
			/// Message code for windows message WM_NCPAINT
			/// </summary>
			WM_NCPAINT = 0x85,
			/// <summary>
			/// Sent if non client area needs to be updated when acive changed
			/// </summary>
			WM_NCACTIVATE = 0x86
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

		[System.Runtime.InteropServices.DllImport("user32.dll", ExactSpelling = true)]
		public static extern IntPtr GetDCEx(IntPtr hWnd, IntPtr hrgnClip, int flags);
		#endregion

		public BorderlessForm()
		{
			FormBorderStyle = FormBorderStyle.None;
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
			using (Graphics graphics = Graphics.FromHdc(GetWindowDC(m.HWnd)))
			{
				graphics.ExcludeClip(new Rectangle(0, TitleBarHeight, Size.Width, Size.Height));
				graphics.Clear(IsActive ? TitleBarColorActive : TitleBarColorInactive);
			}
			EndPaint(m.HWnd, ref ncpaint);
			m.Result = (IntPtr)0;
		}

		/// <summary>
		/// Evaluates which part of the window is being hit
		/// </summary>
		/// <param name="m">Reference to the <see cref="Message"/> recieved from <see cref="WndProc(ref Message)"/></param>
		private void WM_NCHITTEST(ref Message m)
		{
			Point screenPoint = new Point(m.LParam.ToInt32());
			Point windowPoint = ToWindowCoordinates(screenPoint);
			NCHitTestResult result = NCHitTestResult.HT_CLIENT;
			Rectangle titleBar = new Rectangle(Location.X, Location.Y, Size.Width, TitleBarHeight);
			if (titleBar.Contains(screenPoint))
				result = (NCHitTestResult.HT_CAPTION);
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
		/// Paints the client area
		/// </summary>
		/// <param name="g"><see cref="Graphics"/> object that represents the client area</param>
		private void WM_PAINT(Graphics g)
		{
			using (Pen p = new Pen(Color.Green))
				g.DrawLine(p, 1, 1, 1, Size.Height - TitleBarHeight - 2);
			using (Pen p = new Pen(Color.Blue))
				g.DrawLine(p, 1, 1, Size.Width - 1, 1);
			using (Pen p = new Pen(Color.Red))
				g.DrawLine(p, Size.Width - 2, 1, Size.Width - 2, Size.Height - 2 - TitleBarHeight);
			using (Pen p = new Pen(Color.Yellow))
				g.DrawLine(p, 1, Size.Height - 2 - TitleBarHeight, Size.Width - 2, Size.Height - 2 - TitleBarHeight);
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
					m.Result = (IntPtr)0;
					break;
				/*case WMMessageCodes.WM_ERASEBKGND:
					m.Result = (IntPtr)1;
					break;*/
				/*case WMMessageCodes.WM_PAINT:
					BeginPaint(m.HWnd, out PAINTSTRUCT wmpaint);
					using (Graphics g = Graphics.FromHwnd(m.HWnd))
						WM_PAINT(g);
					EndPaint(m.HWnd, ref wmpaint);
					break;*/
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
