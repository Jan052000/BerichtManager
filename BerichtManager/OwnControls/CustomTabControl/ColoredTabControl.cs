using BerichtManager.OwnControls.Native;
using System.ComponentModel;

namespace BerichtManager.OwnControls.CustomTabControl
{
	/// <summary>
	/// Extends <see cref="TabControl"/> to improve styling options
	/// </summary>
	public class ColoredTabControl : TabControl
	{
		private Color tabFocusBorderColor = SystemColors.ActiveBorder;
		/// <summary>
		/// <see cref="Color"/> the border of the selected tabs' head should be drawn in
		/// </summary>
		[Category("Theme")]
		[DefaultValue(typeof(Color), "ActiveBorder")]
		public Color TabFocusBorderColor
		{
			get => tabFocusBorderColor;
			set
			{
				if (value != tabFocusBorderColor)
				{
					tabFocusBorderColor = value;
					Invalidate();
				}
			}
		}

		private float tabFocusBorderWidth = 1f;
		/// <summary>
		/// Width the border of the selected tab should have
		/// </summary>
		[Category("Theme")]
		[DefaultValue(1f)]
		public float TabFocusBorderWidth
		{
			get => tabFocusBorderWidth;
			set
			{
				if (value != tabFocusBorderWidth)
				{
					tabFocusBorderWidth = value;
					Invalidate();
				}
			}
		}

		private Color borderColor = SystemColors.ControlLight;
		/// <summary>
		/// <see cref="Color"/> of the border surrounding the tab page content and the tab heads
		/// </summary>
		[Category("Theme")]
		[DefaultValue(typeof(Color), "ControlLight")]
		public Color BorderColor
		{
			get => borderColor;
			set
			{
				if (value != borderColor)
				{
					borderColor = value;
					Invalidate();
				}
			}
		}

		/// <summary>
		/// Duration in milliseconds that the tool tip of the tab head should last<br/>
		/// Set to <c>0</c> to deactivate
		/// </summary>
		[Category("ToolTips")]
		[DefaultValue(0)]
		public int ItemHeadToolTipDuration { get; set; }

		/// <summary>
		/// <see cref="ToolTip"/> that the tool tips for the tab head are displayed with
		/// </summary>
		[Category("ToolTips")]
		public ToolTip ToolTipTabs { get; set; } = new ToolTip();

		/// <summary>
		/// Override to hide from designer
		/// </summary>
		[Browsable(false)]
		[DefaultValue(typeof(TabDrawMode), "OwnerDrawFixed")]
		public new TabDrawMode DrawMode
		{
			get => TabDrawMode.OwnerDrawFixed;
		}

		/// <summary>
		/// Index of the <see cref="TabPage"/> that was hovered last
		/// </summary>
		private int? LastHovered { get; set; }

		/// <inheritdoc cref="TabPages"/>
		private readonly CustomTabPageCollection tabPages;
		/// <summary>
		/// Collection of contained <see cref="TabPage"/>s
		/// </summary>
		public new CustomTabPageCollection TabPages { get => tabPages; }

		/// <inheritdoc cref="Controls"/>
		private readonly ControlCollectionWrapper controls;
		/// <summary>
		/// Collection of contained <see cref="Control"/>s
		/// </summary>
		public new ControlCollectionWrapper Controls { get => controls; }

		/// <summary>
		/// Creates a new <see cref="ColoredTabControl"/> instance
		/// </summary>
		public ColoredTabControl()
		{
			tabPages = new CustomTabPageCollection(this);
			controls = new ControlCollectionWrapper(this);
		}

		/// <summary>
		/// Handles drawing the tab heads inside the <see cref="WndProc(ref Message)"/>
		/// </summary>
		/// <param name="graphics"><see cref="Graphics"/> to draw on</param>
		/// <param name="drawItem"><see cref="DrawItemStruct"/> that contains detailed information about how to draw the tab head</param>
		protected virtual void OCM_DRAWITEM(Graphics graphics, DrawItemStruct drawItem)
		{
			TabPage page = TabPages[drawItem.ItemID];
			DrawItemEventArgs args = new DrawItemEventArgs(graphics, Font, drawItem.RcItem.ToRectangle(), drawItem.ItemID, drawItem.ItemState, page.ForeColor, page.BackColor);
			OnDrawItem(args);
			args.Dispose();
		}

		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			base.OnDrawItem(e);
		}

		/// <summary>
		/// Draws the tab head
		/// </summary>
		/// <param name="g"><see cref="Graphics"/> to draw tab head on</param>
		/// <param name="pageIndex">Index of <see cref="TabPage"/> to draw tab head of</param>
		protected virtual void DrawItemHead(Graphics g, int pageIndex)
		{
			var page = TabPages[pageIndex];
			var tabRect = GetTabRect(pageIndex);
			using Brush tb = new SolidBrush(GetTabItemBackColor(page));
			g.FillRectangle(tb, tabRect);
			TextRenderer.DrawText(g, page.Text, page.Font, tabRect, GetTabItemTextColor(page));
		}

		/// <summary>
		/// Gets the tab head back color of <paramref name="page"/>
		/// </summary>
		/// <param name="page"><see cref="TabPage"/> to get colors from</param>
		/// <returns>Back color of tab head</returns>
		private Color GetTabItemBackColor(TabPage page)
		{
			if (page is ColoredTabPage cPage)
				return cPage.TabHeadBackColor;
			return BorderColor;
		}

		/// <summary>
		/// Gets the tab fore color of <paramref name="page"/>
		/// </summary>
		/// <param name="page"><see cref="TabPage"/> to get colors from</param>
		/// <returns>Fore color of tab head</returns>
		private Color GetTabItemTextColor(TabPage page)
		{
			if (page is ColoredTabPage cPage)
				return cPage.TabHeadTextColor;
			return DefaultForeColor;
		}

		/// <summary>
		/// Hides <paramref name="page"/> from user
		/// </summary>
		/// <param name="page"><see cref="TabPage"/> to hide</param>
		public void HidePage(TabPage page)
		{
			TabPages.Hide(page);
			Invalidate();
		}

		/// <summary>
		/// Shows a hidden <paramref name="page"/> to user
		/// </summary>
		/// <param name="page"><see cref="TabPage"/> to show</param>
		public void ShowPage(TabPage page)
		{
			TabPages.Show(page);
			Invalidate();
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			ToolTipTabs.Hide(this);
			ToolTipTabs.RemoveAll();
			LastHovered = null;
			base.OnMouseLeave(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			Point mousePos = e.Location;
			if (LastHovered is int lastIndex && LastHovered > -1 && LastHovered < TabPages.Count && !GetTabRect(lastIndex).Contains(mousePos))
			{
				ToolTipTabs.Hide(this);
				ToolTipTabs.RemoveAll();
			}
			base.OnMouseMove(e);
		}

		protected override void OnSelecting(TabControlCancelEventArgs e)
		{
			if (LastHovered != e.TabPageIndex)
			{
				ToolTipTabs.Hide(this);
				ToolTipTabs.RemoveAll();
			}
			base.OnSelecting(e);
		}

		protected override void WndProc(ref Message m)
		{
			switch ((WindowsMessageCodes)m.Msg)
			{
				case WindowsMessageCodes.WM_NOTIFY:
				case WindowsMessageCodes.OCM_NOTIFY:
					if (m.GetLParam(typeof(NMHDR)) is not NMHDR nMHDR)
					{
						base.WndProc(ref m);
						break;
					}
					switch (nMHDR.code)
					{
						case -520:
						case -530:
							if (!ShowToolTips)
								break;
							ToolTipTabs.Hide(this);
							ToolTipTabs.RemoveAll();
							Point toolTipPos = PointToClient(MousePosition);
							toolTipPos.Offset(0, Cursor.Size.Height / 2);
							int hovering = nMHDR.idFrom.ToInt32();
							if (ItemHeadToolTipDuration > 0)
								ToolTipTabs.Show(GetToolTipText(TabPages[hovering]), this, toolTipPos, ItemHeadToolTipDuration);
							else
								ToolTipTabs.Show(GetToolTipText(TabPages[hovering]), this, toolTipPos);
							LastHovered = hovering;
							break;
						default:
							base.WndProc(ref m);
							return;
					}
					m.Result = 1;
					break;
				case WindowsMessageCodes.OCM_DRAWITEM:
					if (m.GetLParam(typeof(DrawItemStruct)) is not DrawItemStruct str)
					{
						base.WndProc(ref m);
						break;
					}
					using (Graphics g = Graphics.FromHdc(str.HDC))
					{
						OCM_DRAWITEM(g, str);
					}
					m.Result = 1;
					break;
				default:
					base.WndProc(ref m);
					break;
			}
		}

		protected override void DefWndProc(ref Message m)
		{
			switch ((WindowsMessageCodes)m.Msg)
			{
				case WindowsMessageCodes.WM_PAINT:
					base.DefWndProc(ref m);
					using (Graphics g = Graphics.FromHwnd(m.HWnd))
					{
						using Brush b = new SolidBrush(BorderColor);
						g.FillRectangle(b, g.ClipBounds);
						for (int i = 0; i < TabPages.Count; i++)
						{
							g.IntersectClip(GetTabRect(i));
							DrawItemHead(g, i);
							g.ResetClip();
						}
						if (SelectedIndex == -1)
							return;
						using Pen line = new Pen(TabFocusBorderColor, TabFocusBorderWidth) { Alignment = System.Drawing.Drawing2D.PenAlignment.Inset };
						g.DrawRectangle(line, GetTabRect(SelectedIndex));
					}
					break;
				default:
					base.DefWndProc(ref m);
					break;
			}
		}
	}
}
