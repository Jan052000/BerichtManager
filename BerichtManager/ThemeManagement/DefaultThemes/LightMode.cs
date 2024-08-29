using System.Drawing;

namespace BerichtManager.ThemeManagement.DefaultThemes
{
	internal class LightMode : ITheme
	{
		public string Name => "Light Mode";
		public Color TextBoxBackColor => Color.FromArgb(255, 255, 255);
		public Color TextBoxDisabledBackColor => Color.FromArgb(235, 235, 228);
		public Color TextBoxBorderColor => Color.FromArgb(255, 255, 255);
		public Color TextBoxArrowColor => Color.Black;
		public Color TextBoxReadOnlyColor => Color.FromArgb(240, 240, 240);
		public Color ColoredComboBoxDropDownButtonBackColor => Color.FromArgb(255, 255, 255);
		public Color ColoredComboBoxTextColor => Color.Black;
		public Color ColoredComboBoxDisabledColor => Color.FromArgb(109, 109, 109);
		public Color ColoredComboBoxDisabledTextColor => Color.FromArgb(109, 109, 109);
		public Color ColoredComboBoxHighlightColor => Color.FromArgb(100, 150, 240);
		public Color MenuStripBackColor => Color.FromArgb(240, 240, 240);
		public Color MenuStripDropdownBackColor => Color.FromArgb(240, 240, 240);
		public Color MenuStripSelectedDropDownBackColor => Color.FromArgb(100, 150, 240);
		public Color ForeColor => Color.Black;
		public Color BackColor => Color.White;
		public Color ButtonColor => Color.LightGray;
		public Color ButtonDisabledColor => Color.FromArgb(235, 235, 228);
		public Color ButtonDisabledTextColor => Color.Gray;
		public Color ButtonFocusedBorderColor => Color.FromArgb(0, 120, 215);
		public float ButtonFocusBorderWidth => 1f;
		public Color ButtonHoverColor => Color.FromArgb(229, 241, 251);
		public Color SplitterColor => Color.FromArgb(150, 150, 150);
		public Color TreeViewDottedLineColor => Color.Gray;
		public Color TreeViewHighlightedNodeColor => Color.FromArgb(100, 150, 240);
		public Color TreeViewReportOpenedHighlightColor => Color.FromArgb(0, 120, 255);
		public Color ReportUploadedColor => Color.Blue;
		public Color ReportHandedInColor => Color.FromArgb(242, 222, 0);
		public Color ReportAcceptedColor => Color.LimeGreen;
		public Color ReportRejectedColor => Color.Red;

	}
}
