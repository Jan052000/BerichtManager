using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace BerichtManager.AddForm
{
	public partial class EditForm : Form
	{
		public string Result { get; set; }
		public EditForm(string title, string text, bool school)
		{
			InitializeComponent();
			this.Text = title;
			rtInput.Multiline = true;
			nudFontSize.Text = rtInput.Font.Size.ToString();
			foreach (FontFamily family in (new InstalledFontCollection()).Families) 
			{
				cbFontFamily.Items.Add(family.Name);
			}
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
					rtInput.Text += "-" + c + ":\n";
				});
			}
			else 
			{
				rtInput.Text = text;
			}
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
			if (float.TryParse(nudFontSize.Text, out float fontSize))
			{
				if (fontSize > 0) 
				{
					rtInput.Font = new Font(rtInput.Font.FontFamily, fontSize);
				}
			}
		}

		private void cbFontFamily_SelectedValueChanged(object sender, EventArgs e)
		{
			rtInput.Font = new Font(cbFontFamily.Text, rtInput.Font.Size);
		}
	}
}
