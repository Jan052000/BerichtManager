using BerichtManager.ThemeManagement;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BerichtManager.Forms
{
	/// <summary>
	/// A simple login form
	/// </summary>
	public partial class Login : Form
	{
		/// <summary>
		/// Username entered by user
		/// </summary>
		public string Username { get; private set; }
		/// <summary>
		/// Password entered by user
		/// </summary>
		public string Password { get; private set; }
		/// <summary>
		/// Wether or not user wants to stay logged in
		/// </summary>
		public bool KeepLoggedIn { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Login"/> <see cref="Form"/>
		/// </summary>
		/// <param name="logInTo">Name of site to log in to</param>
		public Login(string logInTo)
		{
			InitializeComponent();
			ThemeSetter.SetThemes(this, ThemeManager.Instance.ActiveTheme);
			this.Icon = Icon.ExtractAssociatedIcon(Path.GetFullPath(".\\BerichtManager.exe"));
			laLogin.Text = laLogin.Text.Replace("{Placeholder}", logInTo);
			laLogin.Width = TextRenderer.MeasureText(laLogin.Text, laLogin.Font).Width;
			laLogin.Location = new Point(Width / 2 - laLogin.Width / 2, laLogin.Location.Y);
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
			tbPassword.UseSystemPasswordChar = !cbShowPassword.Checked;
		}
	}
}
