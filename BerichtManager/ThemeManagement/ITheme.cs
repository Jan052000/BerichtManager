using System.Drawing;

namespace BerichtManager.ThemeManagement
{
	public interface ITheme
	{
		string Name { get; }
		Color TextBoxBackColor { get; }
		Color TextBoxDisabledBackColor { get; }
		Color TextBoxBorderColor { get; }
		Color TextBoxArrowColor { get; }
		Color ColoredComboBoxDropDownButtonBackColor { get; }
		Color ColoredComboBoxTextColor { get; }
		Color ColoredComboBoxDisabledColor { get; }
		Color ColoredComboBoxDisabledTextColor { get; }
		Color ColoredComboBoxHighlightColor { get; }
		Color MenuStripBackColor { get; }
		Color MenuStripDropdownBackColor { get; }
		Color MenuStripSelectedDropDownBackColor { get; }
		Color ForeColor { get; }
		Color BackColor { get; }
		Color ButtonColor { get; }
		Color ButtonDisabledColor { get; }
		Color ButtonDisabledTextColor { get; }
		Color ButtonFocusedBorderColor { get; }
		float ButtonFocusBorderWidth { get; }
		Color ButtonHoverColor { get; }
		Color SplitterColor { get; }
		Color TreeViewDottedLineColor { get; }
		Color TreeViewHighlightedNodeColor { get; }
		Color ReportUploadedColor { get; }
		Color ReportHandedInColor { get; }
	}
}
