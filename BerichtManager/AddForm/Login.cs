using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BerichtManager.AddForm
{
	public partial class Login : Form
	{
		public string Username;
		public string Password;
		public bool KeepLoggedIn;
		public Login()
		{
			InitializeComponent();
			this.Icon = Icon.ExtractAssociatedIcon(Path.GetFullPath(".\\BerichtManager.exe"));
		}

		private void btClose_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void btLogin_Click(object sender, EventArgs e)
		{
			Username = tbUsername.Text;
			Password = tbPassword.Text;
			KeepLoggedIn = cbKeepLogin.Checked;
			DialogResult = DialogResult.OK;
			Close();
		}

		private void cbShowPassword_CheckedChanged(object sender, EventArgs e)
		{
			if (cbShowPassword.Checked)
			{
				tbPassword.UseSystemPasswordChar = false;
			}
			else 
			{
				tbPassword.UseSystemPasswordChar = true;
			}
		}
	}
}
