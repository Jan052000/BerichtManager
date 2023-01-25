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
		private bool isDirty { get; set; }
		private readonly OptionConfigHandler configHandler = new OptionConfigHandler();
		public OptionMenu()
		{
			InitializeComponent();
			this.Icon = Icon.ExtractAssociatedIcon(Path.GetFullPath(".\\BerichtManager.exe"));
			cbUseCustomPrefix.Checked = configHandler.UseUserPrefix();
			tbCustomPrefix.Text = configHandler.GetCustomPrefix();
			isDirty = false;
			btSave.Enabled = false;
		}

		private void btClose_Click(object sender, EventArgs e)
		{
			if (isDirty) 
			{
				if (MessageBox.Show("Save changes?", "Save?", MessageBoxButtons.YesNo) == DialogResult.Yes) 
				{
					configHandler.SaveConfig();
				}
			}
			Close();
		}

		private void cbUseCustomPrefix_CheckedChanged(object sender, EventArgs e)
		{
			tbCustomPrefix.Enabled = cbUseCustomPrefix.Checked;
			isDirty = true;
			btSave.Enabled = true;
		}

		private void btSave_Click(object sender, EventArgs e)
		{
			try
			{
				if (isDirty) 
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

		private void tbCustomPrefix_TextChanged(object sender, EventArgs e)
		{
			isDirty = true;
			btSave.Enabled = true;
		}
	}
}
