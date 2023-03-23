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

		public Color MenuStripBackColor => Color.FromArgb(50, 50, 50);

		public Color MenuStripDropdownBackColor => Color.FromArgb(50, 50, 50);

		public Color MenuStripSelectedDropDownBackColor => Color.FromArgb(60, 60, 60);

		public Color ForeColor => Color.White;

		public Color BackColor => Color.FromArgb(64, 64, 64);

		public Color ButtonColor => Color.DimGray;
		public Color ButtonDisabledColor => Color.DarkGray;

		public Color SplitterColor => Color.DimGray;
	}
}
