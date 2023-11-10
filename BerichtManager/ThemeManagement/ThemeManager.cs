using BerichtManager.ThemeManagement.DefaultThemes;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;

namespace BerichtManager.ThemeManagement
{
	/// <summary>
	/// Class for loading and managing themes
	/// </summary>
	public class ThemeManager
	{
		/// <summary>
		/// List of available theme objects
		/// </summary>
		public List<ITheme> AvailableThemes = new List<ITheme>() { new DarkMode(), new LightMode() };
		/// <summary>
		/// List of available theme names
		/// </summary>
		public List<string> ThemeNames = new List<string>() { "Dark Mode", "Light Mode" };
		/// <summary>
		/// Path to themes folder
		/// </summary>
		private readonly string themesFolderPath = Path.GetFullPath(".\\Config\\Themes");
		/// <summary>
		/// Event that is called when the themes list has been updated
		/// </summary>
		public event UpdateThemesListDelegate UpdatedThemesList;
		/// <summary>
		/// Delegate for <see cref="UpdatedThemesList"/> event
		/// </summary>
		public delegate void UpdateThemesListDelegate();
		public ITheme ActiveTheme
		{
			get
			{
				ITheme activeTheme = Singleton.GetTheme(Config.ConfigHandler.Instance.ActiveTheme());
				if (activeTheme == null) activeTheme = new DarkMode();
				return activeTheme;
			}
		}

		#region Singleton
		private static ThemeManager Singleton;
		public static ThemeManager Instance
		{
			get
			{
				if (Singleton == null)
					Singleton = new ThemeManager();
				return Singleton;
			}
		}
		#endregion

		private ThemeManager()
		{
			if (!Directory.Exists(themesFolderPath))
				Directory.CreateDirectory(themesFolderPath);
			AvailableThemes.AddRange(GetThemes());
		}

		/// <summary>
		/// Loads themes from file
		/// </summary>
		/// <returns>List of themes</returns>
		private List<ITheme> GetThemes()
		{
			List<ITheme> themes = new List<ITheme>();
			Directory.GetFiles(themesFolderPath).ToList().ForEach(file =>
			{
				try
				{
					ITheme theme = JsonConvert.DeserializeObject<ThemeSerialization>(File.ReadAllText(file));
					if (ThemeNames.Contains(theme.Name))
						return;
					AvailableThemes.Add(theme);
					ThemeNames.Add(theme.Name);
				}
				catch
				{
					MessageBox.Show("Unable to load " + file + "!", "Errow while loading theme");
				}
			});
			return themes;
		}

		/// <summary>
		/// Updates themes list from folder
		/// </summary>
		private void UpdateThemesList()
		{
			ThemeNames = new List<string>() { "Dark Mode", "Light Mode" };
			AvailableThemes = new List<ITheme>() { new DarkMode(), new LightMode() };
			AvailableThemes.AddRange(GetThemes());
			UpdatedThemesList();
		}

		/// <summary>
		/// Fetches theme from list
		/// </summary>
		/// <param name="name">Name of theme</param>
		/// <returns>Theme or <see langword="null"/> if not found</returns>
		public ITheme GetTheme(string name)
		{
			return AvailableThemes.Find(theme => theme.Name == name);
		}

		/// <summary>
		/// Saves a theme to file and updates themes list
		/// </summary>
		/// <param name="theme">Theme to save</param>
		/// <returns>status code for save operation</returns>
		public SaveStatusCodes SaveTheme(ITheme theme)
		{
			SaveStatusCodes returnCode = SaveStatusCodes.Success;
			if (File.Exists(themesFolderPath + theme.Name + ".bmtheme") || ThemeNames.Contains(theme.Name))
			{
				if (MessageBox.Show("Overwrite existing file: " + themesFolderPath + theme.Name + ".bmtheme ?", "Overwrite file?", MessageBoxButtons.YesNo) != DialogResult.Yes)
					return SaveStatusCodes.OverwriteDeclined;
			}
			else returnCode = SaveStatusCodes.NewThemeCreated;
			File.WriteAllText(themesFolderPath + "\\" + theme.Name + ".bmtheme", JsonConvert.SerializeObject(theme, Formatting.Indented));
			UpdateThemesList();
			return returnCode;
		}
	}

	/// <summary>
	/// Status codes for <see cref="ThemeManager.SaveTheme"/>
	/// </summary>
	public enum SaveStatusCodes
	{
		Success,
		OverwriteDeclined,
		InvalidThemeName,
		NewThemeCreated
	}

	/// <summary>
	/// Class for serializing and deserializing an <see cref="ITheme"/> object
	/// </summary>
	public class ThemeSerialization : ITheme
	{
		public string Name { get; set; }
		public Color TextBoxBackColor { get; set; }
		public Color TextBoxDisabledBackColor { get; set; }
		public Color TextBoxBorderColor { get; set; }
		public Color TextBoxArrowColor { get; set; }
		public Color ColoredComboBoxDropDownButtonBackColor { get; set; }
		public Color ColoredComboBoxTextColor { get; set; }
		public Color ColoredComboBoxDisabledColor { get; set; }
		public Color ColoredComboBoxDisabledTextColor { get; set; }
		public Color ColoredComboBoxHighlightColor { get; set; }
		public Color MenuStripBackColor { get; set; }
		public Color MenuStripDropdownBackColor { get; set; }
		public Color MenuStripSelectedDropDownBackColor { get; set; }
		public Color ForeColor { get; set; }
		public Color BackColor { get; set; }
		public Color ButtonColor { get; set; }
		public Color ButtonDisabledColor { get; set; }
		public Color SplitterColor { get; set; }
		public Color TreeViewDottedLineColor { get; set; }
		public Color TreeViewHighlightedNodeColor { get; set; }
	}
}
