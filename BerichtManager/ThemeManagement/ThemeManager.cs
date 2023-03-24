using BerichtManager.ThemeManagement.DefaultThemes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		public List<ITheme> AvailableThemes = new List<ITheme>() { new DarkMode(), new LightMode()};
		/// <summary>
		/// List of available theme names
		/// </summary>
		public List<string> ThemeNames = new List<string>() { "Dark Mode", "Light Mode"};
		/// <summary>
		/// Path to themes folder
		/// </summary>
		private readonly string themesFolderPath = Path.GetFullPath(".\\Config\\Themes");

		public ThemeManager()
		{
			if(!Directory.Exists(themesFolderPath))
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
				ITheme theme = JsonConvert.DeserializeObject<ITheme>(File.ReadAllText(file));
				AvailableThemes.Add(theme);
				ThemeNames.Add(theme.Name);
			});
			return themes;
		}

		/// <summary>
		/// Updates themes list from folder
		/// </summary>
		public void UpdateThemesList()
		{
			ThemeNames = new List<string>() { "Dark Mode", "Light Mode" };
			AvailableThemes = new List<ITheme>() { new DarkMode(), new LightMode() };
			AvailableThemes.AddRange(GetThemes());
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
	}
}
