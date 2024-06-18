using BerichtManager.Config;
using BerichtManager.OwnControls;
using BerichtManager.ThemeManagement;
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
		/// <summary>
		/// Text from <see cref="rtInput"/> after form closed
		/// </summary>
		public string Result { get; set; }
		/// <summary>
		/// Cache object to reduce number of .Instance in code
		/// </summary>
		private ConfigHandler ConfigHandler { get; }
		/// <summary>
		/// Stops calls to ConfigHandler being made
		/// </summary>
		private bool StopConfigCalls { get; set; }
		/// <summary>
		/// Event that is called when config should be reloaded
		/// </summary>
		public event TriggerUpdate RefreshConfigs;

		/// <summary>
		/// Creates a new <see cref="EditForm"/> object
		/// </summary>
		/// <param name="title">Title displayed in title bar</param>
		/// <param name="text">Text to b set in input</param>
		/// <param name="isCreate"><see cref="bool"/> If form is in creation mode which changes button texts, enabled status and tool tips</param>
		/// <param name="stopConfigCalls">If <see cref="EditForm"/> is called while completing config no calls to <see cref="ConfigHandler"/> are made</param>
		public EditForm(string title = "", string text = "", bool isCreate = false, bool stopConfigCalls = false)
		{
			InitializeComponent();
			if (!stopConfigCalls) ConfigHandler = ConfigHandler.Instance;
			ThemeSetter.SetThemes(this, ThemeManager.Instance.ActiveTheme);
			this.Icon = Icon.ExtractAssociatedIcon(Path.GetFullPath(".\\BerichtManager.exe"));
			this.Text = title;
			List<int> tabstops = new List<int>();
			if (stopConfigCalls)
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
				nudFontSize.Value = (decimal)ConfigHandler.EditorFontSize();
				cbFontFamily.Text = ConfigHandler.EditorFont();
				rtInput.Font = new Font(ConfigHandler.EditorFont(), (float)nudFontSize.Value);
				for (int i = 1; tabstops.Count < 32; i++)
				{
					tabstops.Add(i * ConfigHandler.TabStops());
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
			if (ConfigHandler == null)
				return;
			if (StopConfigCalls)
				return;
			if (((float)nudFontSize.Value) != ConfigHandler.EditorFontSize())
			{
				if (ThemedMessageBox.Show(text: "Do you want to save the font size of the editor?", title: "Save font size", buttons: MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					if (float.TryParse(nudFontSize.Text, out float size))
					{
						ConfigHandler.EditorFontSize(size);
						ConfigHandler.SaveConfig();
						RefreshConfigs();
					}
				}
			}
		}

		private void ChangeFont()
		{
			if (ConfigHandler == null)
				return;
			if (StopConfigCalls)
				return;
			if (rtInput.Font.FontFamily.Name != ConfigHandler.EditorFont())
			{
				ConfigHandler.EditorFont(rtInput.Font.FontFamily.Name);
				ConfigHandler.SaveConfig();
				RefreshConfigs();
			}
			SaveSize();
		}

		private void btClose_Click(object sender, EventArgs e)
		{
			Result = rtInput.Text;
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
			ChangeFont();
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
			DialogResult = DialogResult.Ignore;
			Close();
		}
	}
}
