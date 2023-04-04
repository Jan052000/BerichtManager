using BerichtManager.OwnControls;
using System.Drawing;
using System.Windows.Forms;

namespace BerichtManager.ThemeManagement
{
	/// <summary>
	/// Class to set Theme in control
	/// </summary>
	internal class ThemeSetter
	{
		/// <summary>
		/// Sets dark mode theme
		/// </summary>
		/// <param name="control">Top control to set darkmode for</param>
		/// <param name="theme">Theme to be used for styling</param>
		public static void SetThemes(Control control, ITheme theme)
		{
			switch (control)
			{
				case RichTextBox rtb:
					rtb.BackColor = theme.TextBoxBackColor;
					rtb.BorderStyle = BorderStyle.None;
					break;
				case TextBox tb:
					if(tb.Enabled)
						tb.BackColor = theme.TextBoxBackColor;
					else
						tb.BackColor = theme.TextBoxDisabledBackColor;
						tb.BorderStyle = BorderStyle.FixedSingle;
					break;
				case ColoredComboBox comboBox:
					comboBox.BackColor = theme.TextBoxBackColor;
					comboBox.BorderColor = theme.TextBoxBorderColor;
					comboBox.ArrowColor = theme.TextBoxArrowColor;
					comboBox.DropDownButtonColor = theme.ColoredComboBoxDropDownButtonBackColor;
					comboBox.TextColor = theme.ColoredComboBoxTextColor;
					comboBox.DisabledColor = theme.ColoredComboBoxDisabledColor;
					comboBox.DisabledTextColor = theme.ColoredComboBoxDisabledTextColor;
					comboBox.HighlightColor = theme.ColoredComboBoxHighlightColor;
					break;
				case TreeView treeView:
					treeView.BackColor = theme.BackColor;
					break;
				case Form form:
					form.BackColor = theme.BackColor;
					break;
				case Button button:
					if (button.Enabled)
						button.BackColor = theme.ButtonColor;
					else
						button.BackColor = theme.ButtonDisabledColor;
					button.FlatStyle = FlatStyle.Flat;
					button.FlatAppearance.BorderSize = 0;
					break;
				case SplitContainer splitContainer:
				case Splitter splitter:
					control.BackColor = theme.SplitterColor;
					break;
				case MenuStrip menuStrip:
					menuStrip.BackColor = theme.MenuStripBackColor;
					menuStrip.Renderer = new ThemeRenderer(new ThemeColorTable(theme), theme);
					break;
				case ContextMenuStrip contextMenuStrip:
					contextMenuStrip.BackColor = theme.MenuStripBackColor;
					contextMenuStrip.Renderer = new ThemeRenderer(new ThemeColorTable(theme), theme);
					break;
				case ColoredGroupBox coloredGroupBox:
					coloredGroupBox.BackColor = theme.BackColor;
					coloredGroupBox.TitleColor = theme.ForeColor;
					coloredGroupBox.BorderColor = theme.SplitterColor;
					break;
			}
			control.ForeColor = theme.ForeColor;
			foreach (Control control1 in control.Controls)
			{
				SetThemes(control1, theme);
			}
		}
	}

	/// <summary>
	/// Color table for theme renderer
	/// </summary>
	internal class ThemeColorTable : ProfessionalColorTable
	{
		private ITheme theme;
		public ThemeColorTable(ITheme theme)
		{
			this.theme = theme;
		}
		public override Color ToolStripDropDownBackground => theme.MenuStripDropdownBackColor;
	}

	/// <summary>
	/// Renderer for themes of menu strips
	/// </summary>
	internal class ThemeRenderer : ToolStripProfessionalRenderer
	{
		private ITheme theme;
		public ThemeRenderer(ProfessionalColorTable table, ITheme theme): base(table)
		{
			this.theme = theme;
		}

		protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
		{
			e.TextColor = theme.ForeColor;
			base.OnRenderItemText(e);
		}

		protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
		{
			base.OnRenderMenuItemBackground(e);
			if (e.Item.Selected)
				e.Graphics.Clear(theme.MenuStripSelectedDropDownBackColor);
			else
				e.Graphics.Clear(theme.MenuStripBackColor);
		}
	}
}
