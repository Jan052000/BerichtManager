using System.Drawing;

namespace BerichtManager.ThemeManagement.DefaultThemes
{
	internal class DarkMode : ITheme
	{
		public string Name => "Dark Mode";
		public Color TextBoxBackColor => Color.FromArgb(70, 70, 70);
		public Color TextBoxDisabledBackColor => Color.FromArgb(64, 64, 64);
		public Color TextBoxBorderColor => Color.FromArgb(70, 70, 70);
		public Color TextBoxArrowColor => Color.Black;
		public Color ColoredComboBoxTextColor => Color.White;
		public Color ColoredComboBoxDropDownButtonBackColor => Color.FromArgb(100, 100, 100);
		public Color ColoredComboBoxDisabledColor => Color.FromArgb(64, 64, 64);
		public Color ColoredComboBoxDisabledTextColor => Color.FromArgb(109, 109, 109);
		public Color ColoredComboBoxHighlightColor => Color.FromArgb(90, 90, 90);
		public Color MenuStripBackColor => Color.FromArgb(50, 50, 50);
		public Color MenuStripDropdownBackColor => Color.FromArgb(50, 50, 50);
		public Color MenuStripSelectedDropDownBackColor => Color.FromArgb(60, 60, 60);
		public Color ForeColor => Color.White;
		public Color BackColor => Color.FromArgb(64, 64, 64);
		public Color ButtonColor => Color.DimGray;
		public Color ButtonDisabledColor => Color.FromArgb(105, 105, 105);
		public Color ButtonDisabledTextColor => Color.FromArgb(23, 23, 23);
		public Color ButtonFocusedBorderColor => Color.DarkGray;
		public float ButtonFocusBorderWidth => 1f;
		public Color ButtonHoverColor => Color.Gray;
		public Color SplitterColor => Color.DimGray;
		public Color TreeViewDottedLineColor => Color.White;
		public Color TreeViewHighlightedNodeColor => Color.FromArgb(90, 90, 90);
		public Color ReportUploadedColor => Color.DodgerBlue;
		public Color ReportHandedInColor => Color.LimeGreen;
	}
}
