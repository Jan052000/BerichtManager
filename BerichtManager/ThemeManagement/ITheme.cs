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
		Color MenuStripBackColor { get; }
		Color MenuStripDropdownBackColor { get; }
		Color MenuStripSelectedDropDownBackColor { get; }
		Color ForeColor { get; }
		Color BackColor { get; }
		Color ButtonColor { get; }
		Color ButtonDisabledColor { get; }
		Color SplitterColor { get; }
	}
}
