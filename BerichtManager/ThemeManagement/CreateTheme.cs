using BerichtManager.Config;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace BerichtManager.ThemeManagement
{
	public partial class CreateTheme : Form
	{
		public CreateTheme(ITheme theme, ITheme edit = null)
		{
			InitializeComponent();
			InitializeEdit(edit);
			ThemeSetter.SetThemes(this, theme);
			btSave.Focus();
			ColorConverter colorConverter = new ColorConverter();
			foreach (Control control in Controls)
			{
				if (control == tbName)
					continue;
				switch (control)
				{
					case TextBox textBox:
						Color backColor = (Color)colorConverter.ConvertFromString(textBox.Text);
						textBox.BackColor = backColor;
						if ((backColor.R * 0.299 + backColor.G * 0.587 + backColor.B * 0.114) > 186)
							textBox.ForeColor = Color.Black;
						else
							textBox.ForeColor = Color.White;
						break;
				}
			}
		}

		private void TextBox_Entered(object sender, EventArgs e)
		{
			ColorDialog colorDialog = new ColorDialog();
			if (colorDialog.ShowDialog() == DialogResult.OK)
				((TextBox)sender).Text = ColorTranslator.ToHtml(colorDialog.Color);
		}

		private void btClose_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void TextBoxTextChanged(object sender, EventArgs e)
		{
			Color backColor = (Color)new ColorConverter().ConvertFromString(((TextBox)sender).Text);
			if ((backColor.R * 0.299 + backColor.G * 0.587 + backColor.B * 0.114) > 186)
				((TextBox)sender).ForeColor = Color.Black;
			else
				((TextBox)sender).ForeColor = Color.White;
			((TextBox)sender).BackColor = backColor;
			if (string.IsNullOrEmpty(((TextBox)sender).Text))
				((TextBox)sender).Text = "-1";
		}

		private void btSave_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(tbName.Text))
			{
				MessageBox.Show("Name may not be left empty", "Name empty");
				return;
			}
			ColorConverter colorConverter = new ColorConverter();
			SaveStatusCodes code = ThemeManager.Instance.SaveTheme(new ThemeSerialization()
			{
				BackColor = (Color)colorConverter.ConvertFromString(tbBack.Text),
				ButtonColor = (Color)colorConverter.ConvertFromString(tbBt.Text),
				ButtonDisabledColor = (Color)colorConverter.ConvertFromString(tbBTDisabled.Text),
				ColoredComboBoxDisabledColor = (Color)colorConverter.ConvertFromString(tbCCBDisabled.Text),
				ColoredComboBoxDisabledTextColor = (Color)colorConverter.ConvertFromString(tbCCBDisabledText.Text),
				ColoredComboBoxDropDownButtonBackColor = (Color)colorConverter.ConvertFromString(tbCCBDropDownButton.Text),
				ColoredComboBoxHighlightColor = (Color)colorConverter.ConvertFromString(tbHighlight.Text),
				ColoredComboBoxTextColor = (Color)colorConverter.ConvertFromString(tbCCBText.Text),
				ForeColor = (Color)colorConverter.ConvertFromString(tbFore.Text),
				MenuStripBackColor = (Color)colorConverter.ConvertFromString(tbMenuStripBack.Text),
				MenuStripDropdownBackColor = (Color)colorConverter.ConvertFromString(tbMenuStripDropDown.Text),
				MenuStripSelectedDropDownBackColor = (Color)colorConverter.ConvertFromString(tbMenuStripSelectedDropDown.Text),
				Name = tbName.Text,
				SplitterColor = (Color)colorConverter.ConvertFromString(tbSplitter.Text),
				TextBoxArrowColor = (Color)colorConverter.ConvertFromString(tbTBArrowColor.Text),
				TextBoxBackColor = (Color)colorConverter.ConvertFromString(tbBack.Text),
				TextBoxBorderColor = (Color)colorConverter.ConvertFromString(tbTBBorderColor.Text),
				TextBoxDisabledBackColor = (Color)colorConverter.ConvertFromString(tbTBDisabledBackColor.Text),
				TreeViewDottedLineColor = (Color)colorConverter.ConvertFromString(tbTVDottedLine.Text),
				TreeViewHighlightedNodeColor = (Color)colorConverter.ConvertFromString(tbHighlight.Text)
			});
			if (code == SaveStatusCodes.OverwriteDeclined)
				return;
			MessageBox.Show("Changes saved", "Changes saved");
			Close();
		}

		/// <summary>
		/// Sets <see cref="TextBox"/> text to values of <see href="theme"/>
		/// </summary>
		/// <param name="theme">Theme to be edited</param>
		private void InitializeEdit(ITheme theme)
		{
			if (theme == null)
				return;
			tbName.Text = theme.Name;
			tbBack.Text = ColorTranslator.ToHtml(theme.BackColor);
			tbBt.Text = ColorTranslator.ToHtml(theme.ButtonColor);
			tbBTDisabled.Text = ColorTranslator.ToHtml(theme.ButtonDisabledColor);
			tbCCBDisabled.Text = ColorTranslator.ToHtml(theme.ColoredComboBoxDisabledColor);
			tbCCBDisabledText.Text = ColorTranslator.ToHtml(theme.ColoredComboBoxDisabledTextColor);
			tbCCBDropDownButton.Text = ColorTranslator.ToHtml(theme.ColoredComboBoxDropDownButtonBackColor);
			tbHighlight.Text = ColorTranslator.ToHtml(theme.ColoredComboBoxHighlightColor);
			tbCCBText.Text = ColorTranslator.ToHtml(theme.ColoredComboBoxTextColor);
			tbFore.Text = ColorTranslator.ToHtml(theme.ForeColor);
			tbMenuStripBack.Text = ColorTranslator.ToHtml(theme.MenuStripBackColor);
			tbMenuStripDropDown.Text = ColorTranslator.ToHtml(theme.MenuStripDropdownBackColor);
			tbMenuStripSelectedDropDown.Text = ColorTranslator.ToHtml(theme.MenuStripSelectedDropDownBackColor);
			tbSplitter.Text = ColorTranslator.ToHtml(theme.SplitterColor);
			tbTBArrowColor.Text = ColorTranslator.ToHtml(theme.TextBoxArrowColor);
			tbTBBackColor.Text = ColorTranslator.ToHtml(theme.TextBoxBackColor);
			tbTBBorderColor.Text = ColorTranslator.ToHtml(theme.TextBoxBorderColor);
			tbTBDisabledBackColor.Text = ColorTranslator.ToHtml(theme.TextBoxDisabledBackColor);
			tbTVDottedLine.Text = ColorTranslator.ToHtml(theme.TreeViewDottedLineColor);
		}
	}
}
