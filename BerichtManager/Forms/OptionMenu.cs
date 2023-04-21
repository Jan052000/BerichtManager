using BerichtManager.Config;
using BerichtManager.ThemeManagement;
using BerichtManager.ThemeManagement.DefaultThemes;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BerichtManager.Forms
{
	public partial class OptionMenu : Form
	{
		/// <summary>
		/// Value if the form has been edited
		/// </summary>
		private bool isDirty { get; set; }
		private readonly ConfigHandler configHandler;

		/// <summary>
		/// Name of the active theme
		/// </summary>
		public string ThemeName;

		/// <summary>
		/// Emits when the active theme changes
		/// </summary>
		public event activeThemeChanged ActiveThemeChanged;

		/// <summary>
		/// Emits when the report folder path changes
		/// </summary>
		public event reportFolderChanged ReportFolderChanged;

		/// <summary>
		/// Emits when tab stops change
		/// </summary>
		public event tabStopsChanged TabStopsChanged;
		private ThemeManager ThemeManager;
		private ITheme Theme;
		public OptionMenu(ConfigHandler configHandler, ITheme theme, ThemeManager themeManager)
		{
			InitializeComponent();
			if (theme == null)
				theme = new DarkMode();
			Theme = theme;
			ThemeSetter.SetThemes(this, theme);
			ThemeName = theme.Name;
			this.Icon = Icon.ExtractAssociatedIcon(Path.GetFullPath(".\\BerichtManager.exe"));
			this.configHandler = configHandler;
			ThemeManager = themeManager;
			//Set values of fields to values in config
			cbUseCustomPrefix.Checked = configHandler.UseUserPrefix();
			cbShouldUseUntis.Checked = configHandler.UseWebUntis();
			cbEndOfWeek.Checked = configHandler.EndWeekOnFriday();
			tbCustomPrefix.Text = configHandler.CustomPrefix();
			tbServer.Text = configHandler.WebUntisServer();
			tbSchool.Text = configHandler.SchoolName();
			cbLegacyEdit.Checked = configHandler.LegacyEdit();

			themeManager.ThemeNames.ForEach(name => coTheme.Items.Add(name));
			int selectedIndex = coTheme.Items.IndexOf(configHandler.ActiveTheme());
			coTheme.SelectedIndex = selectedIndex;
			ThemeName = coTheme.Text;

			tbTemplate.Text = configHandler.TemplatePath();
			tbName.Text = configHandler.ReportUserName();
			if (int.TryParse(configHandler.ReportNumber(), out int value))
				nudNumber.Value = value;
			nudTabStops.Value = configHandler.TabStops();
			tbFolder.Text = configHandler.ReportPath();
			tbUpdate.Text = configHandler.PublishPath();
			tbNamingPattern.Text = configHandler.NamingPattern();

			isDirty = false;
			btSave.Enabled = false;
			tbCustomPrefix.Enabled = cbUseCustomPrefix.Checked;
			tbSchool.Enabled = cbShouldUseUntis.Checked;
			tbServer.Enabled = cbShouldUseUntis.Checked;
		}

		/// <summary>
		/// Delegate for the <see cref="ReportFolderChanged"/> event
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="reportFolderPath">New report folder path</param>
		public delegate void reportFolderChanged(object sender, string reportFolderPath);

		/// <summary>
		/// Delegate for the <see cref="ActiveThemeChanged"/> event
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="theme">New active theme</param>
		public delegate void activeThemeChanged(object sender, ITheme theme);

		/// <summary>
		/// Delegat fo the <see cref="TabStopsChanged"/> event
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="tabStops">Number of spaces per tab</param>
		public delegate void tabStopsChanged(object sender, int tabStops);

		private void btClose_Click(object sender, EventArgs e)
		{
			if (isDirty)
			{
				if (MessageBox.Show("Save changes?", "Save?", MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					configHandler.UseUserPrefix(cbUseCustomPrefix.Checked);
					if (cbUseCustomPrefix.Checked)
						configHandler.CustomPrefix(tbCustomPrefix.Text);
					if (cbShouldUseUntis.Checked && (tbServer.Text == "" || tbSchool.Text == ""))
					{
						if (MessageBox.Show("Either Webuntis server or school name is empty if you continue to save these changes, \nUse Web Untis will be unchecked and automatic query of timetable will not work", "Save?", MessageBoxButtons.YesNo) == DialogResult.Yes)
						{
							configHandler.UseWebUntis(false);
						}
						else
						{
							return;
						}
					}
					else
					{
						configHandler.UseWebUntis(false);
						configHandler.WebUntisServer(tbServer.Text);
						configHandler.SchoolName(tbSchool.Text);
					}
					configHandler.EndWeekOnFriday(cbEndOfWeek.Checked);
					configHandler.LegacyEdit(cbLegacyEdit.Checked);
					configHandler.ReportUserName(tbName.Text);
					configHandler.ReportNumber("" + nudNumber.Value);
					configHandler.TemplatePath(tbTemplate.Text);
					configHandler.ActiveTheme(coTheme.Text);
					if (ThemeName != (string)coTheme.SelectedValue)
					{
						ThemeName = coTheme.Text;
						ITheme activeTheme = ThemeManager.GetTheme(ThemeName);
						ThemeSetter.SetThemes(this, activeTheme);
						ActiveThemeChanged(this, activeTheme);
					}
					if (configHandler.ReportPath() != tbFolder.Text)
					{
						if (MessageBox.Show("Do you want to switch over imediately?", "Change directory?", MessageBoxButtons.YesNo) == DialogResult.Yes)
							ReportFolderChanged(this, tbFolder.Text);
						configHandler.ReportPath(tbFolder.Text);
					}
					if (configHandler.PublishPath() != tbUpdate.Text)
						configHandler.PublishPath(tbUpdate.Text);
					if (configHandler.TabStops() != (int)nudTabStops.Value)
					{
						configHandler.TabStops((int)nudTabStops.Value);
						TabStopsChanged(this, (int)nudTabStops.Value);
					}
					configHandler.NamingPattern(tbNamingPattern.Text);
					try
					{
						if (isDirty)
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
					configHandler.UseUserPrefix(cbUseCustomPrefix.Checked);
					if (cbUseCustomPrefix.Checked)
						configHandler.CustomPrefix(tbCustomPrefix.Text);
					if (cbShouldUseUntis.Checked)
					{
						configHandler.WebUntisServer(tbServer.Text);
						configHandler.SchoolName(tbSchool.Text);
					}
					configHandler.UseWebUntis(cbShouldUseUntis.Checked);
					configHandler.EndWeekOnFriday(cbEndOfWeek.Checked);
					configHandler.LegacyEdit(cbLegacyEdit.Checked);
					configHandler.ReportUserName(tbName.Text);
					configHandler.ReportNumber("" + nudNumber.Value);
					configHandler.TemplatePath(tbTemplate.Text);
					configHandler.ActiveTheme(coTheme.Text);
					if (ThemeName != coTheme.Text)
					{
						ThemeName = coTheme.Text;
						ITheme activeTheme = ThemeManager.GetTheme(ThemeName);
						ThemeSetter.SetThemes(this, activeTheme);
						ActiveThemeChanged(this, activeTheme);
					}
					if (configHandler.ReportPath() != tbFolder.Text)
					{
						if (MessageBox.Show("Do you want to switch over imediately?", "Change directory?", MessageBoxButtons.YesNo) == DialogResult.Yes)
							ReportFolderChanged(this, tbFolder.Text);
						configHandler.ReportPath(tbFolder.Text);
					}
					if (configHandler.PublishPath() != tbUpdate.Text)
						configHandler.PublishPath(tbUpdate.Text);
					if (configHandler.TabStops() != (int)nudTabStops.Value)
					{
						configHandler.TabStops((int)nudTabStops.Value);
						TabStopsChanged(this, (int)nudTabStops.Value);
					}
					configHandler.NamingPattern(tbNamingPattern.Text);
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
			ITheme theme = ThemeManager.GetTheme(ThemeName);
			if (theme == null)
				return;
			ThemeSetter.SetThemes(tbSchool, theme);
			ThemeSetter.SetThemes(tbServer, theme);
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

		//https://stackoverflow.com/questions/2612487/how-to-fix-the-flickering-in-user-controls
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
				return cp;
			}
		}

		private void btCreateTheme_Click(object sender, EventArgs e)
		{
			new CreateTheme(configHandler, ThemeManager.GetTheme(ThemeName), ThemeManager).ShowDialog();
			coTheme.Items.Clear();
			ThemeManager.ThemeNames.ForEach(name => coTheme.Items.Add(name));
		}

		private void btEditTheme_Click(object sender, EventArgs e)
		{
			OpenFileDialog fileDialog = new OpenFileDialog();
			fileDialog.Filter = "Themes (*.bmtheme)|*.bmtheme";
			fileDialog.InitialDirectory = Path.GetFullPath(".\\Config\\Themes");
			if (fileDialog.ShowDialog() == DialogResult.OK)
			{
				new CreateTheme(configHandler, ThemeManager.GetTheme(ThemeName), ThemeManager, ThemeManager.GetTheme(Path.GetFileNameWithoutExtension(fileDialog.FileName))).ShowDialog();
			}
		}

		private void tbFolder_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog folderDialog = new FolderBrowserDialog();
			if (folderDialog.ShowDialog() == DialogResult.OK)
			{
				tbFolder.Text = folderDialog.SelectedPath;
				MarkAsDirty(sender, e);
			}
		}

		private void tbUpdate_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Executables (*.exe)|*.exe";
			if (openFileDialog.ShowDialog() == DialogResult.OK)
				tbUpdate.Text = openFileDialog.FileName;
		}

		private void toolTip1_Draw(object sender, DrawToolTipEventArgs e)
		{
			e.Graphics.Clear(Theme.BackColor);
			TextRenderer.DrawText(e.Graphics, e.ToolTipText, e.Font, e.Bounds, Theme.ForeColor);
		}
	}
}
