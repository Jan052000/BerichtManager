using BerichtManager.Config;
using BerichtManager.HelperClasses;
using BerichtManager.OwnControls;
using BerichtManager.ThemeManagement;
using BerichtManager.WordTemplate;
using System;
using System.IO;
using System.Linq;
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
		/// Internal cache for last tool tip of <see cref="tbNamingPattern"/> on <see cref="toolTip1"/>
		/// </summary>
		private string NamingPatternToolTip { get; set; }
		/// <summary>
		/// Cache object to reduce number of .Instance in code
		/// </summary>
		private ConfigHandler ConfigHandler { get; } = ConfigHandler.Instance;

		/// <summary>
		/// Name of the active theme
		/// </summary>
		private string ThemeName { get => ThemeManager.Instance.ActiveTheme.Name; }

		/// <summary>
		/// Emits when the active theme changes
		/// </summary>
		public event ActiveThemeChangedDelegate ActiveThemeChanged;

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

		/// <summary>
		/// Emits when ihk base address has been changed
		/// </summary>
		public event IHKBaseAddressChangedDelegate IHKBaseAddressChanged;

		/// <summary>
		/// Emits when use word wrap changes
		/// </summary>
		public event UseWordWrapDelegate UseWordWrapChanged;

		public OptionMenu()
		{
			InitializeComponent();
			ThemeSetter.SetThemes(this);
			ThemeSetter.SetThemes(toolTip1);
			ThemeSetter.SetThemes(ttErrors);

			NamingPatternToolTip = toolTip1.GetToolTip(tbNamingPattern);
			//Set values of fields to values in config
			cbUseCustomPrefix.Checked = ConfigHandler.UseCustomPrefix;
			cbShouldUseUntis.Checked = ConfigHandler.UseWebUntis;
			cbEndOfWeek.Checked = ConfigHandler.EndWeekOnFriday;
			tbCustomPrefix.Text = ConfigHandler.CustomPrefix;
			tbServer.Text = ConfigHandler.WebUntisServer;
			tbSchool.Text = ConfigHandler.SchoolName;
			cbLegacyEdit.Checked = ConfigHandler.UseLegacyEdit;

			UpdateThemesList();
			int selectedIndex = coTheme.Items.IndexOf(ConfigHandler.ActiveTheme);
			coTheme.SelectedIndex = selectedIndex;
			ThemeManager.Instance.UpdatedThemesList += UpdateThemesList;

			tbTemplate.Text = ConfigHandler.TemplatePath;
			tbName.Text = ConfigHandler.ReportUserName;
			nudNumber.Value = ConfigHandler.ReportNumber;
			nudTabStops.Value = ConfigHandler.TabStops;
			nudFontSize.Value = (decimal)ConfigHandler.EditorFontSize;
			tbFolder.Text = ConfigHandler.ReportPath;
			tbUpdate.Text = ConfigHandler.PublishPath;
			tbNamingPattern.Text = ConfigHandler.NamingPattern;
			cbUseWordWrap.Checked = ConfigHandler.UseWordWrap;

			//IHK
			nudUploadDelay.Value = ConfigHandler.IHKUploadDelay;
			tbJobField.Text = ConfigHandler.IHKJobField;
			tbSupervisorMail.Text = ConfigHandler.IHKSupervisorEMail;
			cbAutoSyncStatusesWithIHK.Checked = ConfigHandler.AutoSyncStatusesWithIHK;
			tbIHKBaseUrl.Text = ConfigHandler.IHKBaseUrl;
			cbIHKCheckMatchingStartDates.Checked = ConfigHandler.IHKCheckMatchingStartDates;
			cbIHKAutoGetComment.Checked = ConfigHandler.IHKAutoGetComment;

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
		public delegate void ActiveThemeChangedDelegate();

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

		/// <summary>
		/// Delegate for the <see cref="IHKBaseAddressChanged"/> event
		/// </summary>
		public delegate void IHKBaseAddressChangedDelegate();

		/// <summary>
		/// Delegate for the <see cref="UseWordWrapChanged"/> event
		/// </summary>
		/// <param name="useWordWrap">New value of use word wrap option</param>
		public delegate void UseWordWrapDelegate(bool useWordWrap);

		private void btClose_Click(object sender, EventArgs e)
		{
			if (IsDirty)
			{
				if (ValidateNamingPattern() is string)
				{
					if (ThemedMessageBox.Show(text: "Invalid naming pattern detected, can not save, close menu anyways?", title: "Can not save changes", buttons: MessageBoxButtons.YesNo) == DialogResult.Yes)
						Close();
					return;
				}
				if (ThemedMessageBox.Show(text: "Save changes?", title: "Save?", buttons: MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					try
					{
						SaveConfigChanges();
					}
					catch (Exception ex)
					{
						Logger.LogError(ex);
						ThemedMessageBox.Show(text: ex.StackTrace);
					}
				}
			}
			Close();
		}

		private void cbUseCustomPrefix_CheckedChanged(object sender, EventArgs e)
		{
			MarkAsDirty(sender, e);
			tbCustomPrefix.Enabled = cbUseCustomPrefix.Checked;
		}

		private void btSave_Click(object sender, EventArgs e)
		{
			if (IsDirty)
			{
				if (ValidateNamingPattern() is string message)
				{
					ThemedMessageBox.Info(text: message, title: "Invalid naming pattern");
					return;
				}
				try
				{
					SaveConfigChanges();
				}
				catch (Exception ex)
				{
					HelperClasses.Logger.LogError(ex);
					ThemedMessageBox.Show(text: ex.StackTrace);
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
			int? newTabStops = null;
			float? newFontSize = null;
			bool activeThemeChanged = false;
			bool wordWrapChanged = false;
			bool ihkBaseAddressChanged = false;

			//Prefix
			ConfigHandler.UseCustomPrefix = cbUseCustomPrefix.Checked;
			if (cbUseCustomPrefix.Checked)
				ConfigHandler.CustomPrefix = tbCustomPrefix.Text;
			//WebUntis
			ConfigHandler.UseWebUntis = cbShouldUseUntis.Checked;
			if (cbShouldUseUntis.Checked)
			{
				ConfigHandler.WebUntisServer = tbServer.Text;
				ConfigHandler.SchoolName = tbSchool.Text;
			}
			//Report
			ConfigHandler.ReportUserName = tbName.Text;
			ConfigHandler.TemplatePath = tbTemplate.Text;
			ConfigHandler.ReportNumber = (int)nudNumber.Value;
			ConfigHandler.EndWeekOnFriday = cbEndOfWeek.Checked;
			ConfigHandler.NamingPattern = tbNamingPattern.Text;
			//Manager
			if (ConfigHandler.TabStops != (int)nudTabStops.Value)
			{
				ConfigHandler.TabStops = (int)nudTabStops.Value;
				newTabStops = (int)nudTabStops.Value;
			}
			ConfigHandler.UseLegacyEdit = cbLegacyEdit.Checked;
			if (nudFontSize.Value != (decimal)ConfigHandler.EditorFontSize)
			{
				ConfigHandler.EditorFontSize = (float)nudFontSize.Value;
				newFontSize = (float)nudFontSize.Value;
			}
			if (ThemeName != coTheme.Text)
			{
				ConfigHandler.ActiveTheme = coTheme.Text;
				ThemeSetter.SetThemes(this);
				ThemeSetter.SetThemes(toolTip1);
				ThemeSetter.SetThemes(ttErrors);
				activeThemeChanged = true;
			}
			if (ConfigHandler.ReportPath != tbFolder.Text)
			{
				if (ThemedMessageBox.Show(text: "Do you want to switch over imediately?", title: "Change directory?", buttons: MessageBoxButtons.YesNo) == DialogResult.Yes)
					ReportFolderChanged?.Invoke(this, tbFolder.Text);
				ConfigHandler.ReportPath = tbFolder.Text;
			}
			if (ConfigHandler.PublishPath != tbUpdate.Text)
				ConfigHandler.PublishPath = tbUpdate.Text;
			if (ConfigHandler.UseWordWrap != cbUseWordWrap.Checked)
			{
				ConfigHandler.UseWordWrap = cbUseWordWrap.Checked;
				wordWrapChanged = true;
			}
			//IHK
			ConfigHandler.IHKUploadDelay = (int)nudUploadDelay.Value;
			ConfigHandler.IHKJobField = tbJobField.Text;
			ConfigHandler.IHKSupervisorEMail = tbSupervisorMail.Text;
			ConfigHandler.AutoSyncStatusesWithIHK = cbAutoSyncStatusesWithIHK.Checked;
			if (ConfigHandler.IHKBaseUrl != tbIHKBaseUrl.Text)
				ihkBaseAddressChanged = true;
			ConfigHandler.IHKBaseUrl = tbIHKBaseUrl.Text;
			ConfigHandler.IHKCheckMatchingStartDates = cbIHKCheckMatchingStartDates.Checked;
			ConfigHandler.IHKAutoGetComment = cbIHKAutoGetComment.Checked;

			ConfigHandler.SaveConfig();

			if (newTabStops != null)
				TabStopsChanged?.Invoke(this, (int)nudTabStops.Value);
			if (newFontSize != null)
				FontSizeChanged?.Invoke((float)nudFontSize.Value);
			if (activeThemeChanged)
				ActiveThemeChanged?.Invoke();
			if (wordWrapChanged)
				UseWordWrapChanged?.Invoke(cbUseWordWrap.Checked);
			if (ihkBaseAddressChanged)
				IHKBaseAddressChanged?.Invoke();
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

		/// <summary>
		/// Handles a value change for <see cref="tbNamingPattern"/>
		/// </summary>
		/// <param name="sender"><see cref="tbNamingPattern"/> object</param>
		/// <param name="e">Event args</param>
		private void NamingPatternChanged(object sender, EventArgs e)
		{
			MarkAsDirty(sender, e);
			if (!(sender is TextBox tb))
				return;
			if (ValidateNamingPattern() is string message)
			{
				ttErrors.SetToolTip(tbNamingPattern, message);
				NamingPatternToolTip = toolTip1.GetToolTip(tbNamingPattern);
				toolTip1.SetToolTip(tbNamingPattern, null);
			}
			else
			{
				ttErrors.SetToolTip(tbNamingPattern, null);
				toolTip1.SetToolTip(tbNamingPattern, NamingPatternToolTip);
			}
		}

		/// <summary>
		/// Validates the text of <see cref="tbNamingPattern"/>
		/// </summary>
		/// <returns><see cref="string"/> containing error message or <see langword="null"/> if naming pattern is valid</returns>
		private string ValidateNamingPattern()
		{
			if (tbNamingPattern.Text.Length == 0)
				return "An empty naming pattern is not allowed!";
			if (!NamingPatternResolver.PatternContainsValues(tbNamingPattern.Text))
				return "Caution: pattern does not contain any identifying values, new reports would overwrite each other!";
			char[] invalidChars = Path.GetInvalidFileNameChars();
			int indexOfFirstInvalid = tbNamingPattern.Text.IndexOfAny(invalidChars);
			if (indexOfFirstInvalid > -1)
			{
				string errorMessage = $"Invalid file name character in naming pattern\n({String.Join(", ", invalidChars.Where(c => !char.IsControl(c)).Select(c => c.ToString()))})\nat position {indexOfFirstInvalid + 1} (\"{tbNamingPattern.Text[indexOfFirstInvalid]}\")";
				return errorMessage;
			}
			return null;
		}

		private void cbShouldUseUntis_CheckedChanged(object sender, EventArgs e)
		{
			MarkAsDirty(sender, e);
			tbSchool.Enabled = cbShouldUseUntis.Checked;
			tbServer.Enabled = cbShouldUseUntis.Checked;
			ITheme theme = ThemeManager.Instance.GetTheme(ThemeName);
			if (theme == null)
				return;
			ThemeSetter.SetThemes(tbSchool);
			ThemeSetter.SetThemes(tbServer);
		}

		private void btLogin_Click(object sender, EventArgs e)
		{
			ConfigHandler.DoWebUntisLogin();
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
			CreateTheme create = new CreateTheme();
			if (create.ShowDialog() != DialogResult.OK)
				return;
			UpdateThemesList();
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
			coTheme.Items.AddRange(ThemeManager.GetThemeNames().ToArray());
		}

		private void OptionMenu_FormClosing(object sender, FormClosingEventArgs e)
		{
			ThemeManager.Instance.UpdatedThemesList -= UpdateThemesList;
		}

		private void btIHKLogin_Click(object sender, EventArgs e)
		{
			ConfigHandler.DoIHKLogin();
		}

		private void btFieldsConfig_Click(object sender, EventArgs e)
		{
			if (new WordTemplateForm().ShowDialog() == DialogResult.Retry)
				btFieldsConfig_Click(sender, e);
		}
	}
}
