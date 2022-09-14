using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
