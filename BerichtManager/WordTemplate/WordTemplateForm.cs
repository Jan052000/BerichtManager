using BerichtManager.ThemeManagement;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BerichtManager.WordTemplate
{
	public partial class WordTemplateForm : Form
	{
		public WordTemplateForm()
		{
			InitializeComponent();
			ThemeSetter.SetThemes(this);
			Setup();
		}

		private void Setup()
		{
			List<Fields> fields = Enum.GetValues(typeof(Fields)).Cast<Fields>().ToList();
			List<Fields> configFields = fields.Where(f => FormFieldHandler.GetFormField(f, out _)).OrderBy(f => FormFieldHandler.GetFormFieldIndex(f)).ToList();
			List<Fields> unused = fields.Where(f => !FormFieldHandler.GetFormField(f, out _)).ToList();

			foreach (Fields field in configFields)
			{
				flpOrder.Controls.Add(GetLabel(field.ToString()));
			}

			foreach (Fields field in unused)
			{
				flpFieldOptions.Controls.Add(GetLabel(field.ToString()));
			}


			/*foreach (Enum field in Enum.GetValues(typeof(Fields)))
			{
				flpFieldOptions.Controls.Add(GetLabel(field.ToString()));
			}*/
		}

		private Label GetLabel(string text)
		{
			Label l = new Label();
			l.Text = text;
			Size s = TextRenderer.MeasureText(l.Text, l.Font);
			l.Width = s.Width;
			l.Height = s.Height;
			l.BackColor = ThemeManager.Instance.ActiveTheme.ButtonColor;
			l.ForeColor = ThemeManager.Instance.ActiveTheme.ForeColor;
			l.Margin = new Padding(3);
			l.MouseDown += LabelMouseDown;
			return l;
		}

		private void LabelMouseDown(object sender, MouseEventArgs e)
		{
			(sender as Control)?.DoDragDrop(new DataObject(DataFormats.Serializable, sender), DragDropEffects.Move);
		}

		private void PanelDragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetData(DataFormats.Serializable) is Control control && control.Parent != null && control.Parent == sender)
				e.Effect = DragDropEffects.None;
			else
				e.Effect = DragDropEffects.Move;
		}

		private void PanelDragDrop(object sender, DragEventArgs e)
		{
			(sender as Control).Controls.Add(e.Data.GetData(DataFormats.Serializable) as Control);
		}

		private void OnCloseClicked(object sender, EventArgs e)
		{
			Close();
		}

		private void OnSaveClicked(object sender, EventArgs e)
		{

			//Delete unused field indexes
			foreach (Control control in flpFieldOptions.Controls)
			{
				if (!(control is Label label))
					continue;
				if (!Enum.TryParse(label.Text, true, out Fields field))
					continue;
				FormFieldHandler.DeleteFieldFromConfig(field);
			}

			//Update selected field indexes
			int index = 1;
			foreach (Control control in flpOrder.Controls)
			{
				if (!(control is Label label))
					continue;
				if (!Enum.TryParse(label.Text, true, out Fields field))
					throw new Exception($"Field {label.Text} was not found in FormFieldHandler");
				FormFieldHandler.UpdateFormFieldIndex(field, index++);
			}
		}
	}
}
