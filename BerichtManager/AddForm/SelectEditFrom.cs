using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BerichtManager.AddForm
{
	public partial class SelectEditFrom : Form
	{
		public List<EditState> SelectedItems { get; set; }
		private int Count = 0;
		public SelectEditFrom()
		{
			InitializeComponent();
			SelectedItems = new List<EditState>();
			btConfirm.Text = "Close";
		}

		private void btClose_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void btConfirm_Click(object sender, EventArgs e)
		{
			SelectedItems.Add(new EditState(cbEditName.Checked, "Enter your name"));
			SelectedItems.Add(new EditState(cbEditNumber.Checked, "Edit report nr."));
			SelectedItems.Add(new EditState(cbEditStartDate.Checked, "Edit start of week"));
			SelectedItems.Add(new EditState(cbEditEndDate.Checked, "Edit end of week"));
			SelectedItems.Add(new EditState(cbEditYear.Checked, "Edit year"));
			SelectedItems.Add(new EditState(cbEditWork.Checked, "Edit work"));
			SelectedItems.Add(new EditState(cbEditSeminars.Checked, "Edit seminar"));
			SelectedItems.Add(new EditState(cbEditSchool.Checked, "Edit school"));
			SelectedItems.Add(new EditState(cbEditSignY.Checked, "Edit signdate (you)"));
			SelectedItems.Add(new EditState(cbEditSign.Checked, "Edit signdate (not you)"));
			DialogResult = DialogResult.OK;
			Close();
		}

		private void cbChecked_Changed(object sender, EventArgs e) 
		{
			if (((CheckBox)sender).Checked)
			{
				Count++;
			}
			else 
			{
				Count--;
			}
			if (Count > 0)
			{
				btConfirm.Text = "Start edit";
			}
			else 
			{
				btConfirm.Text = "Close";
			}
		}
	}

	/**
	<summary>
	Container class for checks if field should be edited
	</summary>
	*/
	public class EditState 
	{
		public bool ShouldEdit { get; set; }
		public string EditorTitle { get; set; }
		public EditState(bool shouldEdit, string editorTitle)
		{
			ShouldEdit = shouldEdit;
			EditorTitle = editorTitle;
		}
	}
}
