﻿using BerichtManager.Config;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;

namespace BerichtManager.AddForm
{
	public partial class EditForm : Form
	{
		public string Result { get; set; }
		ConfigHandler handler = new ConfigHandler();

		public EditForm(string title, string text, bool school)
		{
			InitializeComponent();
			this.Icon = Icon.ExtractAssociatedIcon(Path.GetFullPath(".\\BerichtManager.exe"));
			this.Text = title;
			rtInput.Multiline = true;
			//rtInput.Font = new Font("Arial", rtInput.Font.Size);
			nudFontSize.Value = (decimal)handler.LoadEditorFontSize();
			foreach (FontFamily family in (new InstalledFontCollection()).Families)
			{
				cbFontFamily.Items.Add(family.Name);
			}
			cbFontFamily.Text = handler.LoadFont();
			cbFontFamily.Enabled = false;

			rtInput.Font = new Font(cbFontFamily.Text, handler.LoadEditorFontSize());

			List<int> tabstops = new List<int>();
			for (int i = 1; i * 14 < rtInput.Size.Width && tabstops.Count < 32; i++) 
			{
				tabstops.Add(i * 14);
			}
			rtInput.SelectionTabs = tabstops.ToArray();
			if (school)
			{
				//https://borys.webuntis.com/WebUntis/?school=pictorus-bk#/basic/login
				//https://webuntis.com/
				Client client = new Client();
				List<string> classes = client.getClassesFromWebUntis();
				if (classes.Count == 0) MessageBox.Show("Login failed(wrong login details or api down?)");
				classes.ForEach((c) => 
				{
					if (c.Contains("\n\t-Ausgefallen"))
					{
						rtInput.Text += c.Replace("\n\t-Ausgefallen", ":\n\t-Ausgefallen\n");
					}
					else 
					{
						rtInput.Text += "-" + c + ":\n";
					}
				});
			}
			else 
			{
				rtInput.Text = text;
			}
		}

		private void SaveSize() 
		{
			if (((float)nudFontSize.Value) != handler.LoadEditorFontSize()) 
			{
				if (MessageBox.Show("Do you want to save the font size of the editor?", "Save font size", MessageBoxButtons.YesNo) == DialogResult.Yes) 
				{
					if (float.TryParse(nudFontSize.Text , out float size)) 
					{
						handler.SaveEditorFontSize(size);
					}
				}
			}
		}

		private void ChangeFont() 
		{
			if (rtInput.Font.FontFamily.Name != handler.LoadFont())
			{
				if (MessageBox.Show("Do you want to change the font of following reports to " + cbFontFamily.Text + "?\n(Standard: \"Arial\")", "Change Font?", MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					handler.SaveFont(rtInput.Font.FontFamily.Name);
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
	}
}
