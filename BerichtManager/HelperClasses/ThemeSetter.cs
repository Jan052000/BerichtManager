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
					rtb.BackColor = Color.FromArgb(64, 64, 64);
					rtb.BorderStyle = BorderStyle.None;
					break;
				case TextBox tb:
					if(tb.Enabled)
						tb.BackColor = Color.DimGray;
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

			}
			control.ForeColor = Color.White;
			foreach (Control control1 in control.Controls)
			{
				SetThemes(control1);
			}
		}
	}
}
