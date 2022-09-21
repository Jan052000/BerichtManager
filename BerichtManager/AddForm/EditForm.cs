using System;
using System.Collections.Generic;
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
	}
}
