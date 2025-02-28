using BerichtManager.OwnControls;
using BerichtManager.OwnControls.OwnTreeView;
using BerichtManager.Properties;

namespace BerichtManager.ThemeManagement
{
	/// <summary>
	/// Class to set Theme in control
	/// </summary>
	internal class ThemeSetter
	{
		/// <summary>
		/// Active theme set in <see cref="ThemeManager"/>
		/// </summary>
		private static ITheme Theme { get => ThemeManager.ActiveTheme; }

		/// <summary>
		/// Sets theme for <see href="control"/> and its children
		/// </summary>
		/// <param name="control">Top control to set theme for</param>
		public static void SetThemes(Control? control)
		{
			if (control is null)
				return;
			switch (control)
			{
				case RichTextBox rtb:
					if (rtb.ReadOnly)
						rtb.BackColor = Theme.TextBoxReadOnlyColor;
					else if (rtb.Enabled)
						rtb.BackColor = Theme.TextBoxBackColor;
					else
						rtb.BackColor = Theme.TextBoxDisabledBackColor;
					rtb.BorderStyle = BorderStyle.None;
					break;
				case TextBox tb:
					if (tb.ReadOnly)
						tb.BackColor = Theme.TextBoxReadOnlyColor;
					else
					if (tb.Enabled)
						tb.BackColor = Theme.TextBoxBackColor;
					else
						tb.BackColor = Theme.TextBoxDisabledBackColor;
					tb.BorderStyle = BorderStyle.FixedSingle;
					break;
				case ColoredComboBox comboBox:
					comboBox.BackColor = Theme.TextBoxBackColor;
					comboBox.BorderColor = Theme.TextBoxBorderColor;
					comboBox.ArrowColor = Theme.TextBoxArrowColor;
					comboBox.DropDownButtonColor = Theme.ColoredComboBoxDropDownButtonBackColor;
					comboBox.TextColor = Theme.ColoredComboBoxTextColor;
					comboBox.DisabledColor = Theme.ColoredComboBoxDisabledColor;
					comboBox.DisabledTextColor = Theme.ColoredComboBoxDisabledTextColor;
					comboBox.HighlightColor = Theme.ColoredComboBoxHighlightColor;
					break;
				case TreeView treeView:
					treeView.BackColor = Theme.BackColor;
					treeView.ForeColor = Theme.ForeColor;
					treeView.LineColor = Theme.TreeViewDottedLineColor;
					if (treeView is CustomTreeView ctv && ctv.NodeToolTip != null)
						SetThemes(ctv.NodeToolTip);
					break;
				case Form form:
					form.BackColor = Theme.BackColor;
					form.Icon = Resources.AppIcon;
					form.AutoScaleMode = AutoScaleMode.Inherit;
					break;
				case FocusColoredFlatButton flatButton:
					flatButton.BackColor = Theme.ButtonColor;
					flatButton.ButtonDisabledColor = Theme.ButtonDisabledColor;
					flatButton.FlatStyle = FlatStyle.Flat;
					flatButton.ButtonFocusBoxColor = Theme.ButtonFocusedBorderColor;
					flatButton.ButtonFocusBoxWidth = Theme.ButtonFocusBorderWidth;
					flatButton.ButtonDisabledColor = Theme.ButtonDisabledColor;
					flatButton.ButtonDisabledTextColor = Theme.ButtonDisabledTextColor;
					flatButton.ButtonHoverColor = Theme.ButtonHoverColor;
					break;
				case Button button:
					if (button.Enabled)
						button.BackColor = Theme.ButtonColor;
					else
						button.BackColor = Theme.ButtonDisabledColor;
					button.FlatStyle = FlatStyle.Flat;
					button.FlatAppearance.BorderSize = 0;
					break;
				case SplitContainer splitContainer:
				case Splitter splitter:
					control.BackColor = Theme.SplitterColor;
					break;
				case MenuStrip menuStrip:
					menuStrip.BackColor = Theme.MenuStripBackColor;
					menuStrip.ForeColor = Theme.ForeColor;
					menuStrip.Renderer = new ThemeRenderer(new ThemeColorTable(Theme), Theme);
					break;
				case ContextMenuStrip contextMenuStrip:
					contextMenuStrip.BackColor = Theme.MenuStripBackColor;
					contextMenuStrip.Renderer = new ThemeRenderer(new ThemeColorTable(Theme), Theme);
					contextMenuStrip.ForeColor = Theme.ForeColor;
					break;
				case ColoredGroupBox coloredGroupBox:
					coloredGroupBox.BackColor = Theme.BackColor;
					coloredGroupBox.TitleColor = Theme.ForeColor;
					coloredGroupBox.BorderColor = Theme.SplitterColor;
					break;
				default:
					control.BackColor = Theme.BackColor;
					break;
			}
			control.ForeColor = Theme.ForeColor;
			if (control.ContextMenuStrip != null)
				SetThemes(control.ContextMenuStrip);
			foreach (Control control1 in control.Controls)
			{
				SetThemes(control1);
			}
		}

		/// <summary>
		/// Sets the theme of <paramref name="toolTip"/>
		/// </summary>
		/// <param name="toolTip"><see cref="ToolTip"/> to set theme of</param>
		public static void SetThemes(ToolTip? toolTip)
		{
			if (toolTip is null)
				return;
			toolTip.Draw -= ToolTip_Draw;
			toolTip.BackColor = Theme.BackColor;
			toolTip.ForeColor = Theme.ForeColor;
			toolTip.OwnerDraw = true;
			toolTip.Draw += ToolTip_Draw;
		}

		private static void ToolTip_Draw(object? sender, DrawToolTipEventArgs e)
		{
			e.DrawBackground();
			e.DrawBorder();
			Color foreColor = (sender as ToolTip)?.ForeColor ?? e.AssociatedControl?.ForeColor ?? Theme.ForeColor;
			TextRenderer.DrawText(e.Graphics, e.ToolTipText, e.Font, e.Bounds, foreColor);
		}
	}

	/// <summary>
	/// Color table for theme renderer
	/// </summary>
	internal class ThemeColorTable : ProfessionalColorTable
	{
		private ITheme Theme { get; }
		public ThemeColorTable(ITheme theme) : base()
		{
			Theme = theme;
		}
		public override Color ToolStripDropDownBackground => Theme.MenuStripDropdownBackColor;
	}

	/// <summary>
	/// Renderer for themes of menu strips
	/// </summary>
	internal class ThemeRenderer : ToolStripProfessionalRenderer
	{
		private ITheme Theme { get; }
		public ThemeRenderer(ProfessionalColorTable table, ITheme theme) : base(table)
		{
			Theme = theme;
		}

		protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
		{
			e.TextColor = Theme.ForeColor;
			base.OnRenderItemText(e);
		}

		protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
		{
			base.OnRenderMenuItemBackground(e);
			if (e.Item.Selected)
				e.Graphics.Clear(Theme.MenuStripSelectedDropDownBackColor);
			else
				e.Graphics.Clear(Theme.MenuStripBackColor);
		}
	}
}
