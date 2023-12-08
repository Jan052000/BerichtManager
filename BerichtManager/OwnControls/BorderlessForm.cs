using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BerichtManager.OwnControls
{
	/// <summary>
	/// Class which extends the functionalities of a standard winforms <see cref="Form"/> and contains a customizable title bar
	/// </summary>
	public class BorderlessForm : Form
	{
		//Useful:
		//https://stackoverflow.com/questions/53000291/how-to-smooth-ugly-jitter-flicker-jumping-when-resizing-windows-especially-drag

		#region Designer and form variables
		/// <summary>
		/// Graphics object to draw the title bar to memory between paints
		/// </summary>
		private BufferedGraphics BufferedGraphics { get; set; } = null;
		/// <summary>
		/// Management of memory allocation of <see cref="BufferedGraphics"/>
		/// </summary>
		private BufferedGraphicsContext BufferedGraphicsContext { get; set; } = new BufferedGraphicsContext();

		/// <summary>
		/// Variable that tells wether or not the form is currently active
		/// </summary>
		private bool IsActive { get; set; } = false;

		/// <summary>
		/// Stores which button the left mouse button clicked on
		/// </summary>
		private int ClickedButton { get; set; } = -1;

		/// <summary>
		/// Tells the <see cref="WM_NCPAINT(ref Message)"/> function to clear the buffers and redraw the non client area
		/// </summary>
		private bool ForceNCRedraw { get; set; } = false;

		/// <summary>
		/// Bounds of window before maximize, minimize or reduce
		/// </summary>
		private Rectangle OriginalBounds { get; set; }

		/// <summary>
		/// Tells the form wether or not to maximize when restoring position from minimized state
		/// </summary>
		private bool ShouldMaximizeOnRestore { get; set; } = false;

		/// <summary>
		/// Stores the screen this window was last on
		/// </summary>
		private Screen LastScreen { get; set; }

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
					SendMessage(Handle, (int)WMMessageCodes.WM_USER_REDRAWBUTTONS, (IntPtr)0, (IntPtr)0);
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

		/// <summary>
		/// Bounds of the icon
		/// </summary>
		private Rectangle IconBounds
		{
			get => new Rectangle(IconPadding, TitleBarHeight / 2 - IconSize.Height / 2, IconSize.Width, IconSize.Height);
		}

		/// <summary>
		/// Padding the icon has to the left of the window
		/// </summary>
		private int iconPadding { get; set; } = 16;
		/// <summary>
		/// Padding the icon has to the left of the window
		/// </summary>
		[Category("Style")]
		[DefaultValue(16)]
		public int IconPadding
		{
			get => iconPadding;
			set
			{
				if (iconPadding != value)
				{
					iconPadding = value;
					Invalidate();
				}
			}
		}

		/// <summary>
		/// Size of the icon
		/// </summary>
		private Size iconSize { get; set; } = new Size(16, 16);
		/// <summary>
		/// Size of the icon
		/// </summary>
		[Category("Style")]
		[DefaultValue(typeof(Size), "16; 16")]
		public Size IconSize
		{
			get => iconSize;
			set
			{
				if (iconSize.Width != value.Width || iconSize.Height != value.Height)
				{
					iconSize = value;
					Invalidate();
				}
			}
		}

		/// <summary>
		/// Color if the title when window is active
		/// </summary>
		private Color activeTitleColor { get; set; } = SystemColors.ActiveCaptionText;
		/// <summary>
		/// Color if the title when window is active
		/// </summary>
		[Category("Style")]
		[DefaultValue(typeof(Color), "ActiveCaptionText")]
		public Color ActiveTitleColor
		{
			get => activeTitleColor;
			set
			{
				if (activeTitleColor != value)
				{
					activeTitleColor = value;
					Invalidate();
				}
			}
		}

		/// <summary>
		/// Color if the title when window is inactive
		/// </summary>
		private Color inactiveTitleColor { get; set; } = SystemColors.InactiveCaptionText;
		/// <summary>
		/// Color if the title when window is inactive
		/// </summary>
		[Category("Style")]
		[DefaultValue(typeof(Color), "InactiveCaptionText")]
		public Color InactiveTitleColor
		{
			get => inactiveTitleColor;
			set
			{
				if (inactiveTitleColor != value)
				{
					inactiveTitleColor = value;
					Invalidate();
				}
			}
		}

		/// <summary>
		/// Padding title has to the icon
		/// </summary>
		private int textToIconPadding { get; set; } = 8;
		/// <summary>
		/// Padding title has to the icon
		/// </summary>
		[Category("Style")]
		[DefaultValue(8)]
		public int TextToIconPadding
		{
			get => textToIconPadding;
			set
			{
				if (textToIconPadding != value)
				{
					textToIconPadding = value;
					Invalidate();
				}
			}
		}

		/// <summary>
		/// Title as it should be rendered (will shorten if title too long)
		/// </summary>
		private string RenderedText
		{
			get
			{
				int availableTextSpace = ReduceButtonBounds.X - (IconBounds.X + IconBounds.Width + TextToIconPadding);
				if (availableTextSpace < TextRenderer.MeasureText(base.Text, Font).Width)
				{
					string newTitle = "";
					for (int i = 0; i < base.Text.Length; i++)
					{
						if (availableTextSpace >= TextRenderer.MeasureText(newTitle + base.Text[i] + "...", Font).Width)
							newTitle += base.Text[i];
						else
						{
							if (base.Text == "")
								return "";
							return newTitle + "...";
						}

					}
				}
				return base.Text;
			}
		}
		#endregion

		#region Windows message constants

		/// <summary>
		/// Message codes for windows message
		/// </summary>
		public enum WMMessageCodes
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
			WM_NCLBUTTONUP = 0xA2,
			/// <summary>
			/// Sent if the nca is double clicked with left mouse button
			/// </summary>
			WM_NCLBUTTONDBLCLK = 0xA3,
			/// <summary>
			/// Sent to window that is moved by the user
			/// </summary>
			WM_MOVING = 0x0216,
			/// <summary>
			/// Start of WM_USER message range for private messages in this control (0x0400 thorugh 0x7FFF)
			/// </summary>
			WM_USER = 0x0400,
			/// <summary>
			/// Message code for repainting the buttons of the title bar
			/// HWND should be the windows handle, LParam and WParam are not used
			/// </summary>
			WM_USER_REDRAWBUTTONS = WM_USER + 0x02,
			/// <summary>
			/// Message code for repainting title
			/// </summary>
			WM_USER_REDRAWTITLE = WM_USER + 0x03
		}

		/// <summary>
		/// Return values for "WM_NCHITTEST" message
		/// </summary>
		public enum NCHitTestResult
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
			/// Return value for mouse over top left corner
			/// </summary>
			HT_TOPLEFT = 0xD,
			/// <summary>
			/// Return value for mouse over top right corner
			/// </summary>
			HT_TOPRIGHT = 0xE,
			/// <summary>
			/// Return value for mouse over bottom edge
			/// </summary>
			HT_BOTTOM = 0xF,
			/// <summary>
			/// Return value for mouse over bottom left corner
			/// </summary>
			HT_BOTTOMLEFT = 0x10,
			/// <summary>
			/// Return value for mouse over bottom right corner
			/// </summary>
			HT_BOTTOMRIGHT = 0x11,
			/// <summary>
			/// Return value for mouse over closing button
			/// </summary>
			HT_CLOSE = 0x14
		}
		#endregion

		#region Overwrite variables
		/// <summary>
		/// Overwrite and pass through for base.FormBorderStyle to change designer category
		/// </summary>
		[Category("Style")]
		[DefaultValue(typeof(FormBorderStyle), "None")]
		public new FormBorderStyle FormBorderStyle { get => base.FormBorderStyle; set => base.FormBorderStyle = value; }

		/// <summary>
		/// Overwrite and pass through to catch title changing and redrawing non client area
		/// </summary>
		[Category("Appearance")]
		[DefaultValue("")]
		public new string Text
		{
			get => base.Text;
			set
			{
				if (Text != value)
				{
					base.Text = value;
					SendMessage(Handle, (int)WMMessageCodes.WM_USER_REDRAWTITLE, (IntPtr)0, (IntPtr)0);
				}
			}
		}

		/// <summary>
		/// Holds the <see cref="FormWindowState"/> of the window
		/// </summary>
		private FormWindowState windowState = FormWindowState.Normal;
		/// <summary>
		/// Is overwritten to reset hovered title bar button as no nca update is done when switching window states
		/// and to stop reduce from cutting off a <see cref="TitleBarHeight"/> sized rectangle from the bottom of the window
		/// </summary>
		private new FormWindowState WindowState
		{
			get => windowState;
			set
			{
				if (windowState != value)
				{
					FormWindowState oldValue = windowState;
					HoveringButton = -1;
					windowState = value;
					//Save the old bounds when switching from normal state 
					if (oldValue == FormWindowState.Normal)
						OriginalBounds = new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height);
					switch (value)
					{
						case FormWindowState.Normal:
							//Force clearing the buffers on next WM_NCPAINT
							if (oldValue == FormWindowState.Minimized)
								ForceNCRedraw = true;
							//If last state before minimizing was Maximized then maximize window else restore bounds to original
							if (ShouldMaximizeOnRestore)
							{
								Bounds = LastScreen.WorkingArea;
								windowState = FormWindowState.Maximized;
							}
							else
							{
								Bounds = new Rectangle(OriginalBounds.X, OriginalBounds.Y, OriginalBounds.Width, OriginalBounds.Height);
							}
							break;
						case FormWindowState.Minimized:
							//Move window to the out of bounds area window moves its minimized windows to
							Bounds = new Rectangle(-32000, -32000, OriginalBounds.Width, OriginalBounds.Height);
							break;
						case FormWindowState.Maximized:
							//Save which screen the form was last on
							LastScreen = Screen.FromControl(this);
							//Set window to entire screen minus task bar
							Bounds = LastScreen.WorkingArea;
							break;
					}
				}
			}
		}
		#endregion

		#region Structs and dll imports for Win32 API
		/// <summary>
		/// Tells the window manager that painting has begun in a window (Needs to have <see cref="EndPaint(IntPtr, ref PAINTSTRUCT)"/> called after painting is completed)
		/// </summary>
		/// <param name="hwnd">A pointer to the hndle of the window you want to draw on</param>
		/// <param name="lpPaint">A <see cref="PAINTSTRUCT"/> object</param>
		/// <returns>A pointer to the handle of a device context of the window if successful or <see langword="null"/> if not</returns>
		/// https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-beginpaint
		[DllImport("user32.dll")]
		private static extern IntPtr BeginPaint(IntPtr hwnd, out PAINTSTRUCT lpPaint);

		/// <summary>
		/// Tells the window manager that painting has been completed (Needs to be called after <see cref="BeginPaint(IntPtr, out PAINTSTRUCT)"/> when painting is completed)
		/// </summary>
		/// <param name="hWnd"></param>
		/// <param name="lpPaint"></param>
		/// <returns><see langword="true"/></returns>
		/// https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-endpaint
		[DllImport("user32.dll")]
		private static extern bool EndPaint(IntPtr hWnd, [In] ref PAINTSTRUCT lpPaint);

		//https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-paintstruct
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

		//https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-windowpos
		[StructLayout(LayoutKind.Sequential)]
		public struct WINDOWPOS
		{
			public IntPtr hwnd;
			public IntPtr hwndinsertafter;
			public int x, y, cx, cy;
			public int flags;
		}

		//https://learn.microsoft.com/en-us/windows/win32/api/windef/ns-windef-rect
		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int left, top, right, bottom;
		}

		//https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-nccalcsize_params
		[StructLayout(LayoutKind.Sequential)]
		public struct NCCALCSIZE_PARAMS
		{
			public RECT rgrc0, rgrc1, rgrc2;
			public WINDOWPOS lppos;
		}

		/// <summary>
		/// Retrieves the device context for a window
		/// </summary>
		/// <param name="hWnd">A pointer to the window of which you want to get the device context</param>
		/// <returns>A pointer to the device context if successful or <see langword="null"/> if not</returns>
		/// https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindowdc
		[DllImport("user32.dll", ExactSpelling = true)]
		private static extern IntPtr GetWindowDC(IntPtr hWnd);

		/// <summary>
		/// Releases the device context for a window (Needs to be called after <see cref="GetWindowDC(IntPtr)"/> and from the same thread)
		/// </summary>
		/// <param name="hWnd">A pointer to the window handle</param>
		/// <param name="hDc">A pointer to the evice context that belongs to the window which is to be released</param>
		/// <returns><see langword="true"/> if dc was released or <see langword="false"/> if not</returns>
		/// https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-releasedc
		[DllImport("user32.dll")]
		private static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDc);

		/// <summary>
		/// Sends a message to the window
		/// </summary>
		/// <param name="hWnd">Handle to the window that will recieve the message</param>
		/// <param name="wMsg">Message code of the message</param>
		/// <param name="wParam">WParam of message</param>
		/// <param name="lParam">LParam of message</param>
		/// <returns></returns>
		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);
		#endregion

		public BorderlessForm()
		{
			SetStyle(ControlStyles.ContainerControl | ControlStyles.CacheText | ControlStyles.OptimizedDoubleBuffer, true);
			base.FormBorderStyle = FormBorderStyle.None;
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
		/// Forces the non client areas to be redrawn and the buffered graphics to be discarded
		/// </summary>
		private void ForceRedrawNCA()
		{
			Message newMessage = new Message
			{
				HWnd = Handle,
				Msg = (int)WMMessageCodes.WM_NCPAINT,
				WParam = (IntPtr)0,
				LParam = (IntPtr)0
			};
			ForceNCRedraw = true;
			WndProc(ref newMessage);
		}

		/// <summary>
		/// Redraw title on form and in graphics buffer
		/// </summary>
		/// <param name="m">Message passed from <see cref="WndProc(ref Message)"/></param>
		private void RedrawTitle(ref Message m)
		{
			IntPtr hdc = GetWindowDC(m.HWnd);
			BeginPaint(m.HWnd, out PAINTSTRUCT __);
			using (SolidBrush backgrnd = new SolidBrush(IsActive ? TitleBarColorActive : TitleBarColorInactive))
			using (Pen p = new Pen(IsActive ? ActiveTitleColor : InactiveTitleColor))
			{
				Rectangle titleRect = new Rectangle(IconBounds.X + IconBounds.Width + TextToIconPadding, 0, ReduceButtonBounds.X - (IconBounds.X + IconBounds.Width + TextToIconPadding), TitleBarHeight);
				using (Graphics graphics = Graphics.FromHdc(hdc))
				{
					graphics.FillRectangle(backgrnd, titleRect);
					TextRenderer.DrawText(graphics, RenderedText, Font, new Point(IconBounds.X + IconBounds.Width + TextToIconPadding, IconBounds.Y), IsActive ? ActiveTitleColor : InactiveTitleColor);
				}
				BufferedGraphics.Graphics.FillRectangle(backgrnd, titleRect);
				TextRenderer.DrawText(BufferedGraphics.Graphics, RenderedText, Font, new Point(IconBounds.X + IconBounds.Width + TextToIconPadding, IconBounds.Y), IsActive ? ActiveTitleColor : InactiveTitleColor);
			}
			EndPaint(m.HWnd, ref __);
			ReleaseDC(m.HWnd, hdc);
		}

		/// <summary>
		/// Redraw buttons on form and in graphics buffer
		/// </summary>
		/// <param name="m">Message passed from <see cref="WndProc(ref Message)"/></param>
		private void RedrawButtons(ref Message m)
		{
			IntPtr hdc = GetWindowDC(m.HWnd);
			BeginPaint(m.HWnd, out PAINTSTRUCT __);
			using (SolidBrush b = new SolidBrush(TitleBarButtonHoverColor))
			using (SolidBrush red = new SolidBrush(CloseButtonHoverColor))
			using (SolidBrush backgrnd = new SolidBrush(IsActive ? TitleBarColorActive : TitleBarColorInactive))
			{
				using (Graphics graphics = Graphics.FromHdc(hdc))
				{
					{
						if (HoveringButton == 0)
							graphics.FillRectangle(red, CloseButtonBounds);
						else
							graphics.FillRectangle(backgrnd, CloseButtonBounds);
						TextRenderer.DrawText(graphics, "r", new Font("webdings", TitleBarButtonFontSize), CloseButtonBounds, TitleBarButtonForeColor);
						if (HoveringButton == 1)
							graphics.FillRectangle(b, ZoomButtonBounds);
						else
							graphics.FillRectangle(backgrnd, ZoomButtonBounds);
						TextRenderer.DrawText(graphics, WindowState == FormWindowState.Maximized ? "2" : "1", new Font("webdings", TitleBarButtonFontSize), ZoomButtonBounds, TitleBarButtonForeColor);
						if (HoveringButton == 2)
							graphics.FillRectangle(b, ReduceButtonBounds);
						else
							graphics.FillRectangle(backgrnd, ReduceButtonBounds);
						TextRenderer.DrawText(graphics, "0", new Font("webdings", TitleBarButtonFontSize), ReduceButtonBounds, TitleBarButtonForeColor);
					}
				}
				if (HoveringButton == 0)
					BufferedGraphics.Graphics.FillRectangle(red, CloseButtonBounds);
				else
					BufferedGraphics.Graphics.FillRectangle(backgrnd, CloseButtonBounds);
				TextRenderer.DrawText(BufferedGraphics.Graphics, "r", new Font("webdings", TitleBarButtonFontSize), CloseButtonBounds, TitleBarButtonForeColor);
				if (HoveringButton == 1)
					BufferedGraphics.Graphics.FillRectangle(b, ZoomButtonBounds);
				else
					BufferedGraphics.Graphics.FillRectangle(backgrnd, ZoomButtonBounds);
				TextRenderer.DrawText(BufferedGraphics.Graphics, WindowState == FormWindowState.Maximized ? "2" : "1", new Font("webdings", TitleBarButtonFontSize), ZoomButtonBounds, TitleBarButtonForeColor);
				if (HoveringButton == 2)
					BufferedGraphics.Graphics.FillRectangle(b, ReduceButtonBounds);
				else
					BufferedGraphics.Graphics.FillRectangle(backgrnd, ReduceButtonBounds);
				TextRenderer.DrawText(BufferedGraphics.Graphics, "0", new Font("webdings", TitleBarButtonFontSize), ReduceButtonBounds, TitleBarButtonForeColor);
			}
			EndPaint(m.HWnd, ref __);
			ReleaseDC(Handle, hdc);
		}

		/// <summary>
		/// Draws the title bar, the title and the icon
		/// </summary>
		/// <param name="graphics">Graphics object to draw title bar on</param>
		private void DrawTitleBar(Graphics graphics)
		{
			graphics.ExcludeClip(new Rectangle(0, TitleBarHeight, Size.Width, Size.Height));
			graphics.Clear(IsActive ? TitleBarColorActive : TitleBarColorInactive);

			//Draw Text
			TextRenderer.DrawText(graphics, RenderedText, Font, new Point(IconBounds.X + IconBounds.Width + TextToIconPadding, IconBounds.Y), IsActive ? ActiveTitleColor : InactiveTitleColor);

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

			//Draw Icon
			graphics.DrawIcon(Icon, IconBounds);

			graphics.Flush(System.Drawing.Drawing2D.FlushIntention.Sync);
		}

		/// <summary>
		/// Paints the non client areas
		/// </summary>
		/// <param name="m">Reference to the <see cref="Message"/> recieved from <see cref="WndProc(ref Message)"/></param>
		private void WM_NCPAINT(ref Message m)
		{
			BeginPaint(m.HWnd, out PAINTSTRUCT ncpaint);
			IntPtr hdc = GetWindowDC(m.HWnd);
			if (ForceNCRedraw)
			{
				ForceNCRedraw = false;
				using (Graphics graphics = Graphics.FromHdc(hdc))
					DrawTitleBar(graphics);
				EndPaint(m.HWnd, ref ncpaint);
				if (BufferedGraphics != null)
					BufferedGraphics.Dispose();
				BufferedGraphics = BufferedGraphicsContext.Allocate(hdc, new Rectangle(0, 0, Size.Width, TitleBarHeight));
				DrawTitleBar(BufferedGraphics.Graphics);
				ReleaseDC(m.HWnd, hdc);
			}
			else
			{
				if (BufferedGraphics == null)
				{
					BufferedGraphics = BufferedGraphicsContext.Allocate(hdc, new Rectangle(0, 0, Size.Width, TitleBarHeight));
					DrawTitleBar(BufferedGraphics.Graphics);
					BufferedGraphics.Render();
				}
				else
					BufferedGraphics.Render();
				EndPaint(m.HWnd, ref ncpaint);
				BufferedGraphics.Dispose();
				BufferedGraphics = BufferedGraphicsContext.Allocate(hdc, new Rectangle(0, 0, Size.Width, TitleBarHeight));
				DrawTitleBar(BufferedGraphics.Graphics);
				//If dc is not released then title bar will not update color unless form was resized prior
				ReleaseDC(m.HWnd, hdc);
			}
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
			//Remove resize handles when window is maximized
			if (CheckIfBorder(result) && WindowState == FormWindowState.Maximized)
				result = NCHitTestResult.HT_CLIENT;
			m.Result = (IntPtr)result;
		}

		/// <summary>
		/// Checks if the result contans a value indicating a hit with the border
		/// </summary>
		/// <param name="result">Current result to check if it is a border hit</param>
		/// <returns><see langword="true"/> if the hit was on the border and <see langword="false"/> otherwise</returns>
		private bool CheckIfBorder(NCHitTestResult result)
		{
			bool newResult =
			result == NCHitTestResult.HT_LEFT ||
			result == NCHitTestResult.HT_RIGHT ||
			result == NCHitTestResult.HT_TOP ||
			result == NCHitTestResult.HT_TOPLEFT ||
			result == NCHitTestResult.HT_TOPRIGHT ||
			result == NCHitTestResult.HT_BOTTOM ||
			result == NCHitTestResult.HT_BOTTOMLEFT ||
			result == NCHitTestResult.HT_BOTTOMRIGHT;
			return newResult;
		}

		/// <summary>
		/// Calculates and sets the new client area
		/// </summary>
		/// <param name="m">Reference to the <see cref="Message"/> recieved from <see cref="WndProc(ref Message)"/></param>
		/// https://stackoverflow.com/questions/28277039/how-to-set-the-client-area-clientrectangle-in-a-borderless-form
		private void WM_NCCALCSIZE(ref Message m)
		{
			ForceNCRedraw = true;
			if (m.WParam != (IntPtr)0)
			{
				NCCALCSIZE_PARAMS nccp = (NCCALCSIZE_PARAMS)m.GetLParam(typeof(NCCALCSIZE_PARAMS));
				nccp.rgrc0.top += TitleBarHeight;
				Marshal.StructureToPtr(nccp, m.LParam, true);

			}
			else
			{
				RECT rect = (RECT)m.GetLParam(typeof(RECT));
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
				WindowState = FormWindowState.Normal;
			//base.DefWndProc(ref m);
			else
			{
				IsActive = m.WParam.ToInt32() == 1;
				ForceRedrawNCA();
				m.Result = (IntPtr)1;
			}
		}

		/// <summary>
		/// Handles left button down in non client area
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
		/// Handles left button up in non client area
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
					{
						ShouldMaximizeOnRestore = !ShouldMaximizeOnRestore;
						WindowState = WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
					}
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

		/// <summary>
		/// Handles double click on nca
		/// </summary>
		/// <param name="m">Reference to the <see cref="Message"/> recieved from <see cref="WndProc(ref Message)"/></param>
		private void WM_NCLBUTTONDBLCLK(ref Message m)
		{
			Point point = new Point(IntPtr.Size == 8 ? unchecked((int)m.LParam.ToInt64()) : m.LParam.ToInt32());
			Rectangle titleBar = new Rectangle(0, 0, Width, TitleBarHeight);
			if (!titleBar.Contains(ToWindowCoordinates(point)))
			{
				base.WndProc(ref m);
				return;
			}
			ShouldMaximizeOnRestore = !ShouldMaximizeOnRestore;
			if (WindowState == FormWindowState.Maximized)
				WindowState = FormWindowState.Normal;
			else
				WindowState = FormWindowState.Maximized;
			m.Result = (IntPtr)0;
		}

		/// <summary>
		/// Handles window being moved by user
		/// </summary>
		/// <param name="m">Reference to the <see cref="Message"/> recieved from <see cref="WndProc(ref Message)"/></param>
		private void WM_MOVING(ref Message m)
		{
			if (WindowState == FormWindowState.Maximized)
			{
				ShouldMaximizeOnRestore = false;
				Rectangle newOriginalBounds = new Rectangle(OriginalBounds.X, OriginalBounds.Y, OriginalBounds.Width, OriginalBounds.Height);
				RECT currentWindow = (RECT)m.GetLParam(typeof(RECT));
				WindowState = FormWindowState.Normal;
				if (Width % 2 == 0)
				{
					currentWindow.left = MousePosition.X - newOriginalBounds.Width / 2;
					currentWindow.right = MousePosition.X + newOriginalBounds.Width / 2;
					currentWindow.top = MousePosition.Y;
					currentWindow.bottom = MousePosition.Y + newOriginalBounds.Height;
				}
				else
				{
					currentWindow.left = (int)Math.Floor((double)MousePosition.X - newOriginalBounds.Width / 2);
					currentWindow.right = (int)Math.Ceiling((double)MousePosition.X + newOriginalBounds.Width / 2);
					currentWindow.top = MousePosition.Y;
					currentWindow.bottom = MousePosition.Y + newOriginalBounds.Height;
				}
				Marshal.StructureToPtr(currentWindow, m.LParam, true);
				WindowState = FormWindowState.Normal;
			}
			m.Result = (IntPtr)1;
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
				case WMMessageCodes.WM_USER_REDRAWBUTTONS:
					RedrawButtons(ref m);
					break;
				case WMMessageCodes.WM_USER_REDRAWTITLE:
					RedrawTitle(ref m);
					break;
				case WMMessageCodes.WM_NCLBUTTONDBLCLK:
					WM_NCLBUTTONDBLCLK(ref m);
					break;
				case WMMessageCodes.WM_MOVING:
					WM_MOVING(ref m);
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
				cp.ExStyle |= 0x02000000;
				return cp;
			}
		}
	}
}
