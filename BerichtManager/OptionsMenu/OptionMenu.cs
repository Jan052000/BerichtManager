using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BerichtManager.OptionsMenu
{
	public partial class OptionMenu : Form
	{
		public bool Dirty { get; set; }
		private readonly OptionConfigHandler configHandler = new OptionConfigHandler();
		public OptionMenu()
		{
			InitializeComponent();
			this.Icon = Icon.ExtractAssociatedIcon(Path.GetFullPath(".\\BerichtManager.exe"));
			Dirty = false;
			cbUseCustomPrefix.Checked = configHandler.UseUserPrefix();
			tbCustomPrefix.Text = configHandler.GetCustomPrefix();
		}

		private void btClose_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void cbUseCustomPrefix_CheckedChanged(object sender, EventArgs e)
		{
			tbCustomPrefix.Enabled = cbUseCustomPrefix.Checked;
			Dirty = true;
		}

		private void btSave_Click(object sender, EventArgs e)
		{
			try
			{
				if (Dirty) 
				{
					configHandler.SetUseUserPrefix(cbUseCustomPrefix.Checked);
					if (cbUseCustomPrefix.Checked)
						configHandler.SetCustomPrefix(tbCustomPrefix.Text);
				}
				configHandler.SaveConfig();
			}
			catch (Exception ex)
			{
				HelperClasses.Logger.LogError(ex);
				MessageBox.Show(ex.StackTrace);	
			}
			Close();
		}
	}
}
