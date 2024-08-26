using BerichtManager.ThemeManagement;
using BerichtManager.WordTemplate;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BerichtManager.Forms
{
	/// <summary>
	/// Form for selecting <see cref="Fields"/> of a report to edit
	/// </summary>
	public partial class SelectEditFrom : Form
	{
		/// <summary>
		/// <see cref="List{T}"/> of <see cref="Fields"/> selected for edit
		/// </summary>
		public List<SelectedField> SelectedFields { get; set; }
		/// <summary>
		/// Count of checked <see cref="CheckBox"/>es
		/// </summary>
		private int Count = 0;

		/// <summary>
		/// Creates a new <see cref="SelectEditFrom"/> object
		/// </summary>
		public SelectEditFrom()
		{
			InitializeComponent();
			ThemeSetter.SetThemes(this);
			SelectedFields = new List<SelectedField>();
			btConfirm.Text = "Close";
		}

		private void btClose_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void btConfirm_Click(object sender, EventArgs e)
		{
			AddToResult(cbEditName, Fields.Name, "Enter your name");
			AddToResult(cbEditNumber, Fields.Number, "Edit report nr.");
			AddToResult(cbEditStartDate, Fields.StartDate, "Edit start of week");
			AddToResult(cbEditEndDate, Fields.EndDate, "Edit end of week");
			AddToResult(cbEditYear, Fields.Year, "Edit year");
			AddToResult(cbEditWork, Fields.Work, "Edit work");
			AddToResult(cbEditSeminars, Fields.Seminars, "Edit seminar");
			AddToResult(cbEditSchool, Fields.School, "Edit school");
			AddToResult(cbEditSignY, Fields.SignDateYou, "Edit signdate (you)");
			AddToResult(cbEditSign, Fields.SignDateSupervisor, "Edit signdate (not you)");
			DialogResult = DialogResult.OK;
			Close();
		}

		/// <summary>
		/// Adds <paramref name="field"/> to <see cref="SelectedFields"/> if <paramref name="cb"/> is checked
		/// </summary>
		/// <param name="cb"><see cref="CheckBox"/> to check</param>
		/// <param name="field"><see cref="Fields"/> field to add to <see cref="SelectedFields"/> if <paramref name="cb"/> is checked</param>
		private void AddToResult(CheckBox cb, Fields field, string displayText = "")
		{
			if (cb.Checked)
				SelectedFields.Add(new SelectedField(field, displayText));
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

	/// <summary>
	/// Class that holds information of selected <see cref="Fields"/>
	/// </summary>
	public class SelectedField
	{
		/// <summary>
		/// <see cref="Fields"/> that was selected
		/// </summary>
		public Fields Field { get; set; }
		/// <summary>
		/// Text that can be used instead of <see cref="Field"/> name
		/// </summary>
		public string DisplayText { get; set; }

		public SelectedField(Fields field, string displayText)
		{
			this.Field = field;
			this.DisplayText = displayText;
		}
	}
}
