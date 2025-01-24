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
		public List<SelectedField> SelectedFields { get; set; } = new List<SelectedField>();
		/// <summary>
		/// Count of checked <see cref="CheckBox"/>es
		/// </summary>
		private int Count { get; set; } = 0;

		/// <summary>
		/// Creates a new <see cref="SelectEditFrom"/> object
		/// </summary>
		public SelectEditFrom()
		{
			InitializeComponent();
			AddCheckBoxes();
			ThemeSetter.SetThemes(this);
			btConfirm.Text = "Close";
		}

		/// <summary>
		/// Creates <see cref="CheckBox"/>es for active form fields in <see cref="FormFieldHandler"/> and adds them to <see cref="flpCheckBoxes"/>
		/// </summary>
		private void AddCheckBoxes()
		{
			foreach (KeyValuePair<Fields, FormField> kvp in FormFieldHandler.GetCurrentFields())
			{
				CheckBox cb = new CheckBox();
				cb.CheckedChanged += CbCheckedChanged;
				cb.Text = $"Edit {kvp.Value.DisplayText.ToLowerInvariant()}";
				cb.Tag = kvp.Key;
				flpCheckBoxes.Controls.Add(cb);
			}
		}

		private void btConfirm_Click(object sender, EventArgs e)
		{
			foreach (Control control in flpCheckBoxes.Controls)
			{
				if (!(control is CheckBox cb))
					continue;
				if (cb.Checked && cb.Tag is Fields field)
					SelectedFields.Add(new SelectedField(field, cb.Text));
			}
			DialogResult = DialogResult.OK;
			Close();
		}

		private void CbCheckedChanged(object sender, EventArgs e)
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
		public string CheckBoxText { get; set; }

		public SelectedField(Fields field, string displayText)
		{
			this.Field = field;
			this.CheckBoxText = displayText;
		}
	}
}
