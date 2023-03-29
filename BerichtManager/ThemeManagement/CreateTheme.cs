﻿using BerichtManager.Config;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace BerichtManager.ThemeManagement
{
	public partial class CreateTheme : Form
	{
		private readonly ConfigHandler ConfigHandler;
		private ITheme Theme;
		private readonly ThemeManager ThemeManager;
		public CreateTheme(ConfigHandler configHandler, ITheme theme, ThemeManager themeManager)
		{
			InitializeComponent();
			ThemeSetter.SetThemes(this, theme);
			ConfigHandler = configHandler;
			Theme = theme;
			ThemeManager = themeManager;
			btSave.Focus();
			foreach (Control control in Controls)
			{
				if (control == tbName)
					continue;
				switch (control)
				{
					case TextBox textBox:
						Color backColor = Color.FromArgb(int.Parse(textBox.Text));
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
		}

		private void btSave_Click(object sender, EventArgs e)
		{
			if (ConfigHandler == null || Theme == null || ThemeManager == null)
				return;
			if (string.IsNullOrEmpty(tbName.Text))
			{
				MessageBox.Show("Name may not be left empty", "Name empty");
				return;
			}
			ColorConverter colorConverter = new ColorConverter();
			SaveStatusCodes code = ThemeManager.SaveTheme(new ThemeSerialization()
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
			if (code == SaveStatusCodes.InvalidThemeName)
			{
				MessageBox.Show("Name is already in use, please select a different name", "Invalid name");
				return;
			}
			Close();
		}
	}
}
