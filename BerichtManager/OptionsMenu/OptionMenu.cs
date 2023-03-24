using BerichtManager.AddForm;
using BerichtManager.Config;
using BerichtManager.ThemeManagement;
using BerichtManager.ThemeManagement.DefaultThemes;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BerichtManager.OptionsMenu
{
	public partial class OptionMenu : Form
	{
		/// <summary>
		/// Value if the form has been edited
		/// </summary>
		private bool isDirty { get; set; }
		private readonly ConfigHandler configHandler;
		public OptionMenu(ConfigHandler configHandler, ITheme theme, ThemeManager themeManager)
		{
			InitializeComponent();
			if (theme == null)
				theme = new DarkMode();
			ThemeSetter.SetThemes(this, theme);
			this.Icon = Icon.ExtractAssociatedIcon(Path.GetFullPath(".\\BerichtManager.exe"));
			this.configHandler = configHandler;
			//Set values of fields to values in config
			cbUseCustomPrefix.Checked = configHandler.UseUserPrefix();
			cbShouldUseUntis.Checked = configHandler.UseWebUntis();
			cbEndOfWeek.Checked = configHandler.EndWeekOnFriday();
			tbCustomPrefix.Text = configHandler.GetCustomPrefix();
			tbServer.Text = configHandler.GetWebUntisServer();
			tbSchool.Text = configHandler.GetSchoolName();
			cbLegacyEdit.Checked = configHandler.LegacyEdit();
			themeManager.ThemeNames.ForEach(name => coTheme.Items.Add(name));
			int selectedIndex = coTheme.Items.IndexOf(configHandler.ActiveTheme());
			coTheme.SelectedIndex = selectedIndex;
			tbTemplate.Text = configHandler.LoadPath();
			tbName.Text = configHandler.LoadName();
			if (int.TryParse(configHandler.LoadNumber(), out int value))
				nudNumber.Value = value;
			isDirty = false;
			btSave.Enabled = false;
			tbCustomPrefix.Enabled = cbUseCustomPrefix.Checked;
			tbSchool.Enabled = cbShouldUseUntis.Checked;
			tbServer.Enabled = cbShouldUseUntis.Checked;
		}

		private void btClose_Click(object sender, EventArgs e)
		{
			if (isDirty)
			{
				if (MessageBox.Show("Save changes?", "Save?", MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					configHandler.SetUseUserPrefix(cbUseCustomPrefix.Checked);
					if (cbUseCustomPrefix.Checked)
						configHandler.SetCustomPrefix(tbCustomPrefix.Text);
					if (cbShouldUseUntis.Checked && (tbServer.Text == "" || tbSchool.Text == ""))
					{
						if (MessageBox.Show("Either Webuntis server or school name is empty if you continue to save these changes, \nUse Web Untis will be unchecked and automatic query of timetable will not work", "Save?", MessageBoxButtons.YesNo) == DialogResult.Yes)
						{
							configHandler.SetUseWebUntis(false);
						}
						else
						{
							return;
						}
					}
					else
					{
						configHandler.SetUseWebUntis(false);
						configHandler.SetWebUntisServer(tbServer.Text);
						configHandler.SetSchoolName(tbSchool.Text);
					}
					configHandler.EndWeekOnFriday(cbEndOfWeek.Checked);
					configHandler.LegacyEdit(cbLegacyEdit.Checked);
					configHandler.SaveName(tbName.Text);
					configHandler.EditNumber("" + nudNumber.Value);
					configHandler.Save(tbTemplate.Text);
					configHandler.ActiveTheme(coTheme.Text);
					try
					{
						configHandler.SaveConfig();
					}
					catch (Exception ex)
					{
						HelperClasses.Logger.LogError(ex);
						MessageBox.Show(ex.StackTrace);
					}
				}
			}
			Close();
		}

		private void cbUseCustomPrefix_CheckedChanged(object sender, EventArgs e)
		{
			isDirty = true;
			btSave.Enabled = true;
			tbCustomPrefix.Enabled = cbUseCustomPrefix.Checked;
		}

		private void btSave_Click(object sender, EventArgs e)
		{
			try
			{
				if (isDirty) 
				{
					configHandler.SetUseUserPrefix(cbUseCustomPrefix.Checked);
					if (cbUseCustomPrefix.Checked)
						configHandler.SetCustomPrefix(tbCustomPrefix.Text);
					if (cbShouldUseUntis.Checked)
					{
						configHandler.SetWebUntisServer(tbServer.Text);
						configHandler.SetSchoolName(tbSchool.Text);
					}
					configHandler.SetUseWebUntis(cbShouldUseUntis.Checked);
					configHandler.EndWeekOnFriday(cbEndOfWeek.Checked);
					configHandler.LegacyEdit(cbLegacyEdit.Checked);
					configHandler.SaveName(tbName.Text);
					configHandler.EditNumber("" + nudNumber.Value);
					configHandler.Save(tbTemplate.Text);
					configHandler.ActiveTheme(coTheme.Text);
				}
				configHandler.SaveConfig();
			}
			catch (Exception ex)
			{
				HelperClasses.Logger.LogError(ex);
				MessageBox.Show(ex.StackTrace);	
			}
			btSave.Enabled = false;
			isDirty = false;
		}

		/// <summary>
		/// Marks form as dirty
		/// </summary>
		/// <param name="sender">changed control</param>
		/// <param name="e">event</param>
		private void MarkAsDirty(object sender, EventArgs e)
		{
			isDirty = true;
			btSave.Enabled = true;
		}

		private void cbShouldUseUntis_CheckedChanged(object sender, EventArgs e)
		{
			isDirty = true;
			btSave.Enabled = true;
			tbSchool.Enabled = cbShouldUseUntis.Checked;
			tbServer.Enabled = cbShouldUseUntis.Checked;
		}

		private void btLogin_Click(object sender, EventArgs e)
		{
			configHandler.doLogin();
		}

		private void tbTemplate_Click(object sender, EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = "Word Templates (*.dotx)|*.dotx";
			if (dialog.ShowDialog() == DialogResult.OK)
			{
				tbTemplate.Text = dialog.FileName;
				MarkAsDirty(sender, e);
			}
		}
	}
}
