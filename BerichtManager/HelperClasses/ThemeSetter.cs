using System.Drawing;
using System.Windows.Forms;

namespace BerichtManager.HelperClasses
{
	/// <summary>
	/// Class to set Theme in control
	/// </summary>
	internal class ThemeSetter
	{
		/// <summary>
		/// Sets dark mode theme
		/// </summary>
		/// <param name="control">Top control to set darkmode for</param>
		public static void SetThemes(Control control)
		{
			switch (control)
			{
				case RichTextBox rtb:
					rtb.BackColor = Color.FromArgb(70, 70, 70);
					rtb.BorderStyle = BorderStyle.None;
					break;
				case TextBox tb:
					if(tb.Enabled)
						tb.BackColor = Color.FromArgb(70, 70, 70);
					else
						tb.BackColor = Color.FromArgb(64, 64, 64);
					tb.BorderStyle = BorderStyle.FixedSingle;
					break;
				case TreeView treeView:
					treeView.BackColor = Color.FromArgb(64, 64, 64);
					break;
				case Form form:
					form.BackColor = Color.FromArgb(64, 64, 64);
					break;
				case Button button:
					button.BackColor = Color.DimGray;
					button.FlatStyle = FlatStyle.Flat;
					button.FlatAppearance.BorderSize = 0;
					break;
				case SplitContainer splitContainer:
				case Splitter splitter:
					control.BackColor = Color.DimGray;
					break;
				case MenuStrip menuStrip:
					menuStrip.BackColor = Color.FromArgb(50, 50, 50);
					menuStrip.Renderer = new DarkModeRenderer(new DarkModeTheme());
					break;
			}
			control.ForeColor = Color.White;
			foreach (Control control1 in control.Controls)
			{
				SetThemes(control1);
			}
		}
	}

	/// <summary>
	/// Color table for dark mode
	/// </summary>
	internal class DarkModeTheme : ProfessionalColorTable
	{
		public override Color ToolStripDropDownBackground => Color.FromArgb(50, 50, 50);
	}

	/// <summary>
	/// Renderer for dark mode
	/// </summary>
	internal class DarkModeRenderer : ToolStripProfessionalRenderer
	{
		public DarkModeRenderer(ProfessionalColorTable table): base(table)
		{

		}

		protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
		{
			e.TextColor = Color.White;
			base.OnRenderItemText(e);
		}

		protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
		{
			base.OnRenderMenuItemBackground(e);
			if (e.Item.Selected)
				e.Graphics.Clear(Color.FromArgb(60, 60, 60));
			else
				e.Graphics.Clear(Color.FromArgb(50, 50, 50));
		}
	}
}
