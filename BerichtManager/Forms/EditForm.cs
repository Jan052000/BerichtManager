﻿using BerichtManager.Config;
using BerichtManager.OwnControls;
using BerichtManager.ThemeManagement;
using System.Drawing.Text;

namespace BerichtManager.Forms
{
	public partial class EditForm : Form
	{
		/// <summary>
		/// Text from <see cref="rtInput"/> after form closed
		/// </summary>
		public string? Result { get; set; }
		/// <summary>
		/// Cache object to reduce number of .Instance in code
		/// </summary>
		private ConfigHandler? ConfigHandler { get; }
		/// <summary>
		/// The default value if string is left empty
		/// </summary>
		private string DefaultValue { get; set; }
		/// <summary>
		/// Event that is called when config should be reloaded
		/// </summary>
		public event TriggerUpdate? RefreshConfigs;

		/// <summary>
		/// Creates a new <see cref="EditForm"/> object
		/// </summary>
		/// <param name="title">Title displayed in title bar</param>
		/// <param name="text">Text to be set in input</param>
		/// <param name="isCreate"><see cref="bool"/> If form is in creation mode which changes button texts, enabled status and tool tips</param>
		/// <param name="defaultValue">The default value <see cref="Result"/> should have if Input is left empty</param>
		public EditForm(string title = "", string? text = null, bool isCreate = false, string defaultValue = "", bool isReadonly = false)
		{
			InitializeComponent();
			ThemeSetter.SetThemes(this);
			this.Text = title;
			DefaultValue = defaultValue;
			List<int> tabstops = new List<int>();
			if (ConfigHandler.IsInitializing)
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
				ConfigHandler = ConfigHandler.Instance;
				nudFontSize.Value = (decimal)ConfigHandler.EditorFontSize;
				cbFontFamily.Text = ConfigHandler.EditorFont;
				rtInput.Font = new Font(ConfigHandler.EditorFont, (float)nudFontSize.Value);
				rtInput.WordWrap = ConfigHandler.UseWordWrap;
				for (int i = 1; tabstops.Count < 32; i++)
				{
					tabstops.Add(i * ConfigHandler.TabStops);
				}
			}
			foreach (FontFamily family in (new InstalledFontCollection()).Families)
			{
				cbFontFamily.Items.Add(family.Name);
			}
			cbFontFamily.Enabled = false;
			rtInput.SelectionTabs = tabstops.ToArray();
			rtInput.Text = text ?? "";
			if (isCreate)
			{
				btSaveAndQuit.Enabled = false;
				btQuit.Text = "Cancel";
				ttTips.SetToolTip(btQuit, "Cancels the creation process");
			}


			rtInput.ReadOnly = isReadonly;
			if (isReadonly)
			{
				btSkip.Text = "Close";
				btConfirm.Visible = false;
				btQuit.Visible = false;
				btSaveAndQuit.Visible = false;
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
			if (((float)nudFontSize.Value) != ConfigHandler.EditorFontSize)
			{
				if (ThemedMessageBox.Show(text: "Do you want to save the font size of the editor?", title: "Save font size", buttons: MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					if (float.TryParse(nudFontSize.Text, out float size))
					{
						ConfigHandler.EditorFontSize = size;
						ConfigHandler.SaveConfig();
						RefreshConfigs?.Invoke();
					}
				}
			}
		}

		private void ChangeFont()
		{
			if (ConfigHandler == null)
				return;
			if (rtInput.Font.FontFamily.Name != ConfigHandler.EditorFont)
			{
				ConfigHandler.EditorFont = rtInput.Font.FontFamily.Name;
				ConfigHandler.SaveConfig();
				RefreshConfigs?.Invoke();
			}
			SaveSize();
		}

		private void btSkip_Click(object sender, EventArgs e)
		{
			Result = rtInput.Text;
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void btConfirm_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(rtInput.Text))
			{
				Result = DefaultValue;
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
				Result = DefaultValue;
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
