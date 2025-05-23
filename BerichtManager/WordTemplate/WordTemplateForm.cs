﻿using BerichtManager.OwnControls;
using BerichtManager.ThemeManagement;

namespace BerichtManager.WordTemplate
{
	public partial class WordTemplateForm : Form
	{
		/// <summary>
		/// Used to indicate that the form field order has been altered
		/// </summary>
		private bool IsDirty { get; set; } = false;

		public WordTemplateForm()
		{
			InitializeComponent();
			ThemeSetter.SetThemes(this);
			Setup();
		}

		/// <summary>
		/// Fills <see cref="flpFieldOptions"/> and <see cref="flpOrder"/> with <see cref="Label"/>s representing form field config
		/// </summary>
		private void Setup()
		{
			foreach (Control control in flpOrder.Controls)
			{
				if (control is Label la)
					LabelUnsubscribe(la);
			}
			foreach (Control control in flpFieldOptions.Controls)
			{
				if (control is Label la)
					LabelUnsubscribe(la);
			}

			flpOrder.Controls.Clear();
			flpFieldOptions.Controls.Clear();

			List<Fields> fields = Enum.GetValues(typeof(Fields)).Cast<Fields>().ToList();
			List<Fields> configFields = fields.Where(f => FormFieldHandler.TryGetFormField(f, out _)).OrderBy(FormFieldHandler.GetFormFieldIndex).ToList();
			List<Fields> unused = fields.Where(f => !FormFieldHandler.TryGetFormField(f, out _)).ToList();

			foreach (Fields field in configFields)
			{
				flpOrder.Controls.Add(GetLabel(field.ToString()));
			}

			foreach (Fields field in unused)
			{
				flpFieldOptions.Controls.Add(GetLabel(field.ToString()));
			}
		}

		/// <summary>
		/// Generates a styled <see cref="Label"/> containing <paramref name="text"/>
		/// </summary>
		/// <param name="text">Text in label</param>
		/// <returns>Styled <see cref="Label"/></returns>
		private Label GetLabel(string? text)
		{
			Label l = new Label();
			l.Text = text ?? "";
			Size s = TextRenderer.MeasureText(l.Text, l.Font);
			l.Width = s.Width;
			l.Height = s.Height;
			l.BackColor = ThemeManager.ActiveTheme.ButtonColor;
			l.ForeColor = ThemeManager.ActiveTheme.ForeColor;
			l.Margin = new Padding(3);
			LabelSubscribe(l);
			return l;
		}

		/// <summary>
		/// Subsctibes to events of <paramref name="label"/>
		/// </summary>
		/// <param name="label"><see cref="Label"/> to subscribe handlers to</param>
		private void LabelSubscribe(Label label)
		{
			label.MouseDown += LabelMouseDown;
		}

		/// <summary>
		/// Unsubscribes from events of <paramref name="label"/>
		/// </summary>
		/// <param name="label"><see cref="Label"/> to unsubscribe handlers of</param>
		private void LabelUnsubscribe(Label label)
		{
			label.MouseDown -= LabelMouseDown;
		}

		private void LabelMouseDown(object? sender, MouseEventArgs e)
		{
			if (sender is not Control control)
				return;
			control.DoDragDrop(new DataObject(DataFormats.Serializable, control), DragDropEffects.Move);
		}

		private void PanelDragEnter(object sender, DragEventArgs e)
		{
			if (e.Data?.GetData(DataFormats.Serializable) is Control control && control.Parent != null && control.Parent == sender)
				e.Effect = DragDropEffects.None;
			else
				e.Effect = DragDropEffects.Move;
		}

		private void PanelDragDrop(object sender, DragEventArgs e)
		{
			if (sender is not Control control)
				return;
			control.Controls.Add(e.Data?.GetData(DataFormats.Serializable) as Control);
			IsDirty = true;
		}

		private void OnCloseClicked(object sender, EventArgs e)
		{
			if (IsDirty && ThemedMessageBox.Show(text: "Save unsaved changes?", title: "Unsaved changes!", buttons: MessageBoxButtons.YesNo) == DialogResult.Yes)
				SaveConfig();
			Close();
		}

		private void OnSaveClicked(object sender, EventArgs e)
		{
			SaveConfig();
		}

		/// <summary>
		/// Saves config to <see cref="FormFieldHandler"/>
		/// </summary>
		/// <exception cref="Exception">Thrown if label text is not a value of <see cref="Fields"/></exception>
		private void SaveConfig()
		{
			List<(Fields Field, int Index)> fields = new List<(Fields Field, int Index)>();
			int index = 1;
			foreach (Control control in flpOrder.Controls)
			{
				if (control is not Label label)
					continue;
				if (!Enum.TryParse(label.Text, true, out Fields field))
					throw new Exception($"Field {label.Text} was not found in FormFieldHandler");
				fields.Add((Field: field, Index: index++));
			}
			FormFieldHandler.UpdateFormFieldIndexes(fields);
			IsDirty = false;
			ThemedMessageBox.Show(text: "Saved changes.", title: "Saved");
		}

		private void OnDefaultClicked(object sender, EventArgs e)
		{
			if (ThemedMessageBox.Show(text: "Reset order to default?", title: "Reset order?", buttons: MessageBoxButtons.YesNo) != DialogResult.Yes)
				return;
			Dictionary<Fields, FormField> formfieldsConfig = FormFieldHandler.GetInitialConfig();

			flpOrder.Controls.Clear();
			flpFieldOptions.Controls.Clear();

			foreach (KeyValuePair<Fields, FormField> kvp in formfieldsConfig)
			{
				flpOrder.Controls.Add(GetLabel(Enum.GetName(typeof(Fields), kvp.Key)));
			}

			foreach (Fields _enum in Enum.GetValues(typeof(Fields)))
			{
				if (!formfieldsConfig.ContainsKey(_enum))
					flpFieldOptions.Controls.Add(GetLabel(_enum.ToString()));
			}
		}

		private void OnResetClicked(object sender, EventArgs e)
		{
			if (ThemedMessageBox.Show(text: "Do you want to discard your changes?", title: "Discard changes?", buttons: MessageBoxButtons.YesNo) != DialogResult.Yes)
				return;
			Setup();
		}
	}
}
