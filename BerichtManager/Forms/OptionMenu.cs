using BerichtManager.Config;
using BerichtManager.OwnControls;
using BerichtManager.ThemeManagement;
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
		private bool IsDirty { get; set; }
		/// <summary>
		/// Cache object to reduce number of .Instance in code
		/// </summary>
		private ConfigHandler ConfigHandler { get; } = ConfigHandler.Instance;

		/// <summary>
		/// Name of the active theme
		/// </summary>
		public string ThemeName { get; set; }

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

		/// <summary>
		/// Emits when font size setting is changed
		/// </summary>
		public event fontSizeChanged FontSizeChanged;

		public OptionMenu()
		{
			InitializeComponent();
			ThemeSetter.SetThemes(this, ThemeManager.Instance.ActiveTheme);
			ThemeName = ThemeManager.Instance.ActiveTheme.Name;
			this.Icon = Icon.ExtractAssociatedIcon(Path.GetFullPath(".\\BerichtManager.exe"));
			//Set values of fields to values in config
			cbUseCustomPrefix.Checked = ConfigHandler.UseUserPrefix();
			cbShouldUseUntis.Checked = ConfigHandler.UseWebUntis();
			cbEndOfWeek.Checked = ConfigHandler.EndWeekOnFriday();
			tbCustomPrefix.Text = ConfigHandler.CustomPrefix();
			tbServer.Text = ConfigHandler.WebUntisServer();
			tbSchool.Text = ConfigHandler.SchoolName();
			cbLegacyEdit.Checked = ConfigHandler.LegacyEdit();

			ThemeManager.Instance.ThemeNames.ForEach(name => coTheme.Items.Add(name));
			int selectedIndex = coTheme.Items.IndexOf(ConfigHandler.ActiveTheme());
			coTheme.SelectedIndex = selectedIndex;
			ThemeName = coTheme.Text;
			ThemeManager.Instance.UpdatedThemesList += UpdateThemesList;

			tbTemplate.Text = ConfigHandler.TemplatePath();
			tbName.Text = ConfigHandler.ReportUserName();
			nudNumber.Value = ConfigHandler.ReportNumber();
			nudTabStops.Value = ConfigHandler.TabStops();
			nudFontSize.Value = (decimal)ConfigHandler.EditorFontSize();
			tbFolder.Text = ConfigHandler.ReportPath();
			tbUpdate.Text = ConfigHandler.PublishPath();
			tbNamingPattern.Text = ConfigHandler.NamingPattern();

			IsDirty = false;
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
		/// Delegat for the <see cref="TabStopsChanged"/> event
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="tabStops">Number of spaces per tab</param>
		public delegate void tabStopsChanged(object sender, int tabStops);

		/// <summary>
		/// Delegate for the <see cref="FontSizeChanged"/> event
		/// </summary>
		/// <param name="fontSize">Size to change font to</param>
		public delegate void fontSizeChanged(float fontSize);

		private void btClose_Click(object sender, EventArgs e)
		{
			if (IsDirty)
			{
				if (ThemedMessageBox.Show(ThemeManager.Instance.ActiveTheme, "Save changes?", "Save?", MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					try
					{
						SaveConfigChanges();
					}
					catch (Exception ex)
					{
						HelperClasses.Logger.LogError(ex);
						ThemedMessageBox.Show(ThemeManager.Instance.ActiveTheme, ex.StackTrace);
					}
				}
			}
			Close();
		}

		private void cbUseCustomPrefix_CheckedChanged(object sender, EventArgs e)
		{
			IsDirty = true;
			btSave.Enabled = true;
			tbCustomPrefix.Enabled = cbUseCustomPrefix.Checked;
		}

		private void btSave_Click(object sender, EventArgs e)
		{
			if (IsDirty)
			{
				try
				{
					SaveConfigChanges();
				}
				catch (Exception ex)
				{
					HelperClasses.Logger.LogError(ex);
					ThemedMessageBox.Show(ThemeManager.Instance.ActiveTheme, ex.StackTrace);
				}
			}
			btSave.Enabled = false;
			IsDirty = false;
		}

		/// <summary>
		/// Saves changes to config
		/// </summary>
		private void SaveConfigChanges()
		{
			//Prefix
			ConfigHandler.UseUserPrefix(cbUseCustomPrefix.Checked);
			if (cbUseCustomPrefix.Checked)
				ConfigHandler.CustomPrefix(tbCustomPrefix.Text);
			//WebUntis
			ConfigHandler.UseWebUntis(cbShouldUseUntis.Checked);
			if (cbShouldUseUntis.Checked)
			{
				ConfigHandler.WebUntisServer(tbServer.Text);
				ConfigHandler.SchoolName(tbSchool.Text);
			}
			//Report
			ConfigHandler.ReportUserName(tbName.Text);
			ConfigHandler.TemplatePath(tbTemplate.Text);
			ConfigHandler.ReportNumber((int)nudNumber.Value);
			ConfigHandler.EndWeekOnFriday(cbEndOfWeek.Checked);
			ConfigHandler.NamingPattern(tbNamingPattern.Text);
			//Manager
			if (ConfigHandler.TabStops() != (int)nudTabStops.Value)
			{
				ConfigHandler.TabStops((int)nudTabStops.Value);
				TabStopsChanged(this, (int)nudTabStops.Value);
			}
			ConfigHandler.LegacyEdit(cbLegacyEdit.Checked);
			if (nudFontSize.Value != (decimal)ConfigHandler.EditorFontSize())
			{
				ConfigHandler.EditorFontSize((float)nudFontSize.Value);
				FontSizeChanged((float)nudFontSize.Value);

			}
			ConfigHandler.ActiveTheme(coTheme.Text);
			if (ThemeName != coTheme.Text)
			{
				ThemeName = coTheme.Text;
				ITheme activeTheme = ThemeManager.Instance.GetTheme(ThemeName);
				ThemeSetter.SetThemes(this, activeTheme);
				ActiveThemeChanged(this, activeTheme);
			}
			if (ConfigHandler.ReportPath() != tbFolder.Text)
			{
				if (ThemedMessageBox.Show(ThemeManager.Instance.ActiveTheme, "Do you want to switch over imediately?", "Change directory?", MessageBoxButtons.YesNo) == DialogResult.Yes)
					ReportFolderChanged(this, tbFolder.Text);
				ConfigHandler.ReportPath(tbFolder.Text);
			}
			if (ConfigHandler.PublishPath() != tbUpdate.Text)
				ConfigHandler.PublishPath(tbUpdate.Text);
			ConfigHandler.SaveConfig();
		}

		/// <summary>
		/// Marks form as dirty
		/// </summary>
		/// <param name="sender">changed control</param>
		/// <param name="e">event</param>
		private void MarkAsDirty(object sender, EventArgs e)
		{
			IsDirty = true;
			btSave.Enabled = true;
		}

		private void cbShouldUseUntis_CheckedChanged(object sender, EventArgs e)
		{
			IsDirty = true;
			btSave.Enabled = true;
			tbSchool.Enabled = cbShouldUseUntis.Checked;
			tbServer.Enabled = cbShouldUseUntis.Checked;
			ITheme theme = ThemeManager.Instance.GetTheme(ThemeName);
			if (theme == null)
				return;
			ThemeSetter.SetThemes(tbSchool, theme);
			ThemeSetter.SetThemes(tbServer, theme);
		}

		private void btLogin_Click(object sender, EventArgs e)
		{
			ConfigHandler.DoLogin();
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
			if (new CreateTheme().ShowDialog() != DialogResult.OK) return;
			coTheme.Items.Clear();
			ThemeManager.Instance.ThemeNames.ForEach(name => coTheme.Items.Add(name));
		}

		private void btEditTheme_Click(object sender, EventArgs e)
		{
			OpenFileDialog fileDialog = new OpenFileDialog();
			fileDialog.Filter = "Themes (*.bmtheme)|*.bmtheme";
			fileDialog.InitialDirectory = Path.GetFullPath(".\\Config\\Themes");
			if (fileDialog.ShowDialog() == DialogResult.OK)
			{
				new CreateTheme(edit: ThemeManager.Instance.GetTheme(Path.GetFileNameWithoutExtension(fileDialog.FileName))).ShowDialog();
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
			e.Graphics.Clear(ThemeManager.Instance.ActiveTheme.BackColor);
			TextRenderer.DrawText(e.Graphics, e.ToolTipText, e.Font, e.Bounds, ThemeManager.Instance.ActiveTheme.ForeColor);
		}

		private void UpdateThemesList()
		{
			coTheme.Items.Clear();
			coTheme.Items.AddRange(ThemeManager.Instance.ThemeNames.ToArray());
		}

		private void OptionMenu_FormClosing(object sender, FormClosingEventArgs e)
		{
			ThemeManager.Instance.UpdatedThemesList -= UpdateThemesList;
		}
	}
}
