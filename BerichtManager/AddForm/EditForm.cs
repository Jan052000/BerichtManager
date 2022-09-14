using System;
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
			if (school)
			{
				//https://borys.webuntis.com/WebUntis/?school=pictorus-bk#/basic/login
				//https://webuntis.com/
				Client client = new Client();
				client.getClassesFromWebUntis().ForEach((c) => 
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

		private void rtInput_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == 13) 
			{
				
			}
		}
	}
}
