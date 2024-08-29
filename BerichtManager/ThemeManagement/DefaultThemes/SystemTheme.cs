using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BerichtManager.ThemeManagement.DefaultThemes
{
	internal class SystemTheme : ITheme
	{
		public string Name => "System";

		public Color TextBoxBackColor => BackColor;

		public Color TextBoxDisabledBackColor => SystemColors.ControlLight;

		public Color TextBoxBorderColor => SystemColors.ActiveBorder;

		public Color TextBoxArrowColor => SystemColors.ControlText;

		public Color TextBoxReadOnlyColor => SystemColors.Control;

		public Color ColoredComboBoxDropDownButtonBackColor => BackColor;

		public Color ColoredComboBoxTextColor => SystemColors.ControlText;

		public Color ColoredComboBoxDisabledColor => SystemColors.InactiveCaption;

		public Color ColoredComboBoxDisabledTextColor => SystemColors.InactiveCaptionText;

		public Color ColoredComboBoxHighlightColor => SystemColors.MenuHighlight;

		public Color MenuStripBackColor => SystemColors.MenuBar;

		public Color MenuStripDropdownBackColor => SystemColors.Menu;

		public Color MenuStripSelectedDropDownBackColor => SystemColors.MenuHighlight;

		public Color ForeColor => SystemColors.ControlText;

		public Color BackColor => SystemColors.Window;

		public Color ButtonColor => SystemColors.ControlLight;

		public Color ButtonDisabledColor => SystemColors.ControlLight;

		public Color ButtonDisabledTextColor => SystemColors.GrayText;

		public Color ButtonFocusedBorderColor => SystemColors.MenuHighlight;

		public float ButtonFocusBorderWidth => 2f;

		public Color ButtonHoverColor => SystemColors.ButtonHighlight;

		public Color SplitterColor => SystemColors.ControlLight;

		public Color TreeViewDottedLineColor => SystemColors.ControlText;

		public Color TreeViewHighlightedNodeColor => SystemColors.MenuHighlight;

		public Color TreeViewReportOpenedHighlightColor => SystemColors.HighlightText;

		public Color ReportUploadedColor => Color.Blue;

		public Color ReportHandedInColor => Color.Yellow;

		public Color ReportAcceptedColor => Color.Green;

		public Color ReportRejectedColor => Color.Red;
	}
}
