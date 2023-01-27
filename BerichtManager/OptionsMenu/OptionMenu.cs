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
		/// <summary>
		/// Value if the form has been edited
		/// </summary>
		private bool isDirty { get; set; }
		private readonly OptionConfigHandler configHandler = new OptionConfigHandler();
		public OptionMenu()
		{
			InitializeComponent();
			this.Icon = Icon.ExtractAssociatedIcon(Path.GetFullPath(".\\BerichtManager.exe"));
			//Set values of fields to values in config
			cbUseCustomPrefix.Checked = configHandler.UseUserPrefix();
			cbShouldUseUntis.Checked = configHandler.UseWebUntis();
			cbEndOfWeek.Checked = configHandler.EndWeekOnFriday();
			tbCustomPrefix.Text = configHandler.GetCustomPrefix();
			tbServer.Text = configHandler.GetWebUntisServer();
			tbSchool.Text = configHandler.GetSchoolName();
			isDirty = false;
			btSave.Enabled = false;
			tbCustomPrefix.Enabled = cbUseCustomPrefix.Checked;
			tbSchool.Enabled = cbShouldUseUntis.Checked;
			tbServer.Enabled = cbShouldUseUntis.Checked;
		}

		private void btClose_Click(object sender, EventArgs e)
		{
			if (isDirty)
			{
				if (MessageBox.Show("Save changes?", "Save?", MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					configHandler.SetUseUserPrefix(cbUseCustomPrefix.Checked);
					if (cbUseCustomPrefix.Checked)
						configHandler.SetCustomPrefix(tbCustomPrefix.Text);
					if (cbShouldUseUntis.Checked && (tbServer.Text == "" || tbSchool.Text == ""))
					{
						if (MessageBox.Show("Either Webuntis server or school name is empty if you continue to save these changes, \nUse Web Untis will be unchecked and automatic query of timetable will not work", "Save?", MessageBoxButtons.YesNo) == DialogResult.Yes)
						{
							configHandler.SetUseWebUntis(false);
						}
						else
						{
							return;
						}
					}
					else
					{
						configHandler.SetUseWebUntis(false);
						configHandler.SetWebUntisServer(tbServer.Text);
						configHandler.SetSchoolName(tbSchool.Text);
					}
					configHandler.EndWeekOnFriday(cbEndOfWeek.Checked);
					try
					{
						configHandler.SaveConfig();
					}
					catch (Exception ex)
					{
						HelperClasses.Logger.LogError(ex);
						MessageBox.Show(ex.StackTrace);
					}
				}
			}
			Close();
		}

		private void cbUseCustomPrefix_CheckedChanged(object sender, EventArgs e)
		{
			isDirty = true;
			btSave.Enabled = true;
			tbCustomPrefix.Enabled = cbUseCustomPrefix.Checked;
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
					if (cbShouldUseUntis.Checked)
					{
						configHandler.SetWebUntisServer(tbServer.Text);
						configHandler.SetSchoolName(tbSchool.Text);
					}
					configHandler.SetUseWebUntis(cbShouldUseUntis.Checked);
					configHandler.EndWeekOnFriday(cbEndOfWeek.Checked);
				}
				configHandler.SaveConfig();
			}
			catch (Exception ex)
			{
				HelperClasses.Logger.LogError(ex);
				MessageBox.Show(ex.StackTrace);	
			}
			btSave.Enabled = false;
			isDirty = false;
		}

		private void tbCustomPrefix_TextChanged(object sender, EventArgs e)
		{
			isDirty = true;
			btSave.Enabled = true;
		}

		private void cbShouldUseUntis_CheckedChanged(object sender, EventArgs e)
		{
			isDirty = true;
			btSave.Enabled = true;
			tbSchool.Enabled = cbShouldUseUntis.Checked;
			tbServer.Enabled = cbShouldUseUntis.Checked;
		}

		private void cbEndOfWeek_CheckedChanged(object sender, EventArgs e)
		{
			isDirty = true;
			btSave.Enabled = true;
		}
	}
}
