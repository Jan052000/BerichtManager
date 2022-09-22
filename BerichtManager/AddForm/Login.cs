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
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
