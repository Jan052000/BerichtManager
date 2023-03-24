using System.Drawing;

namespace BerichtManager.ThemeManagement.DefaultThemes
{
	internal class LightMode : ITheme
	{
		public string Name => "Light Mode";
		public Color TextBoxBackColor => SystemColors.Window;

		public Color TextBoxDisabledBackColor => Color.FromArgb(235, 235, 228);
		public Color TextBoxBorderColor => SystemColors.Window;
		public Color TextBoxArrowColor => Color.Black;
		public Color ColoredComboBoxDropDownButtonBackColor => SystemColors.Window;

		public Color MenuStripBackColor => SystemColors.Window;

		public Color MenuStripDropdownBackColor => SystemColors.Control;

		public Color MenuStripSelectedDropDownBackColor => SystemColors.Control;

		public Color ForeColor => SystemColors.WindowText;

		public Color BackColor => SystemColors.Control;

		public Color ButtonColor => Color.LightGray;
		public Color ButtonDisabledColor => Color.Gray;

		public Color SplitterColor => SystemColors.Window;
	}
}
