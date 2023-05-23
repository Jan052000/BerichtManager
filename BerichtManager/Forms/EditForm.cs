using BerichtManager.Config;
using BerichtManager.ThemeManagement;
using BerichtManager.ThemeManagement.DefaultThemes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;

namespace BerichtManager.Forms
{
	public partial class EditForm : Form
	{
		public string Result { get; set; }
		private readonly ConfigHandler handler;
		/// <summary>
		/// Stops calls to ConfigHandler being made
		/// </summary>
		private bool stopConfigCalls { get; set; }
		/// <summary>
		/// Event that is called when config should be reloaded
		/// </summary>
		public event TriggerUpdate RefreshConfigs;

		/// <summary>
		/// Creates a new <see cref="EditForm"/> object
		/// </summary>
		/// <param name="title">Title displayed in title bar</param>
		/// <param name="theme">Theme to be used</param>
		/// <param name="text">Text to b set in input</param>
		/// <param name="isCreate"><see cref="bool"/> if form is in creation mode which changes button texts, enabled status and tool tips</param>
		/// <param name="isConfigHandlerInitializing">if <see cref="ConfigHandler"/> is completing config file, no calls to it are made</param>
		public EditForm(string title, ITheme theme, string text = "", bool school = false, bool isCreate = false, bool isConfigHandlerInitializing = false)
		{
			InitializeComponent();
			stopConfigCalls = isConfigHandlerInitializing;
			if (!stopConfigCalls)
				handler = new ConfigHandler(null);
			if (theme == null)
				theme = new DarkMode();
			ThemeSetter.SetThemes(this, theme);
			this.Icon = Icon.ExtractAssociatedIcon(Path.GetFullPath(".\\BerichtManager.exe"));
			this.Text = title;
			List<int> tabstops = new List<int>();
			if (handler == null)
			{
				nudFontSize.Value = (decimal)8.25f;
				cbFontFamily.Text = "Arial";
				for (int i = 1; i * 14 < rtInput.Size.Width && tabstops.Count < 32; i++)
				{
					tabstops.Add(i * 14);
				}
			}
			else
			{
				nudFontSize.Value = (decimal)handler.EditorFontSize();
				cbFontFamily.Text = handler.EditorFont();
				rtInput.Font = new Font(handler.EditorFont(), (float)nudFontSize.Value);
				for (int i = 1; tabstops.Count < 32; i++)
				{
					tabstops.Add(i * handler.TabStops());
				}
			}
			foreach (FontFamily family in (new InstalledFontCollection()).Families)
			{
				cbFontFamily.Items.Add(family.Name);
			}
			cbFontFamily.Enabled = false;
			rtInput.SelectionTabs = tabstops.ToArray();
			rtInput.Text = text;
			if (isCreate)
			{
				btSaveAndQuit.Enabled = false;
				btQuit.Text = "Cancel";
				ttTips.SetToolTip(btQuit, "Cancels the creation process");
			}
		}

		/// <summary>
		/// Delegate for signaling updates to properties
		/// </summary>
		public delegate void TriggerUpdate();

		private void SaveSize()
		{
			if (handler == null)
				return;
			if (stopConfigCalls)
				return;
			if (((float)nudFontSize.Value) != handler.EditorFontSize())
			{
				if (MessageBox.Show("Do you want to save the font size of the editor?", "Save font size", MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					if (float.TryParse(nudFontSize.Text, out float size))
					{
						handler.EditorFontSize(size);
						handler.SaveConfig();
						RefreshConfigs();
					}
				}
			}
		}

		private void ChangeFont()
		{
			if (handler == null)
				return;
			if (stopConfigCalls)
				return;
			if (rtInput.Font.FontFamily.Name != handler.EditorFont())
			{
				if (MessageBox.Show("Do you want to change the font of following reports to " + cbFontFamily.Text + "?\n(Standard: \"Arial\")", "Change Font?", MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					handler.EditorFont(rtInput.Font.FontFamily.Name);
					handler.SaveConfig();
					RefreshConfigs();
				}
			}
			SaveSize();
		}

		private void btClose_Click(object sender, EventArgs e)
		{
			Result = rtInput.Text;

			ChangeFont();
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void btConfirm_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(rtInput.Text))
			{
				Result = "-Keine-";
			}
			else
			{
				Result = rtInput.Text;
			}
			ChangeFont();
			DialogResult = DialogResult.OK;
			Close();
		}

		private void btQuit_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Abort;
		}

		private void nudFontSize_ValueChanged(object sender, EventArgs e)
		{
			if (nudFontSize.Value > 0)
			{
				rtInput.Font = new Font(rtInput.Font.FontFamily, (float)nudFontSize.Value);
			}
		}

		private void cbFontFamily_SelectedValueChanged(object sender, EventArgs e)
		{
			rtInput.Font = new Font(cbFontFamily.Text, rtInput.Font.Size);
		}

		private void cbEditorFont_CheckedChanged(object sender, EventArgs e)
		{
			if (cbEditorFont.Checked)
			{
				cbFontFamily.Enabled = true;
				if (cbFontFamily.Items.Count == 0)
				{
					foreach (FontFamily family in (new InstalledFontCollection()).Families)
					{
						cbFontFamily.Items.Add(family.Name);
					}
				}
			}
			else
			{
				cbFontFamily.Text = "Arial";
				cbFontFamily.Enabled = false;
			}
		}

		private void btSaveAndQuit_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(rtInput.Text))
			{
				Result = "-Keine-";
			}
			else
			{
				Result = rtInput.Text;
			}
			ChangeFont();
			DialogResult = DialogResult.Ignore;
			Close();
		}
	}
}
