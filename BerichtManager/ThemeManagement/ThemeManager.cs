using BerichtManager.ThemeManagement.DefaultThemes;
using Newtonsoft.Json;
using BerichtManager.OwnControls;
using System.ComponentModel;
using BerichtManager.Config;

namespace BerichtManager.ThemeManagement
{
	/// <summary>
	/// Class for loading and managing themes
	/// </summary>
	public class ThemeManager
	{
		/// <summary>
		/// <see cref="Dictionary{TKey, TValue}"/> containing a collection of <see cref="ITheme"/>s as values and their names as keys
		/// </summary>
		private Dictionary<string, ITheme> Themes { get; set; } = new Dictionary<string, ITheme>()
		{
			{"Dark Mode", new DarkMode()},
			{"Light Mode", new LightMode()},
			{"System", new SystemTheme()}
		};
		/// <summary>
		/// Path to themes folder
		/// </summary>
		private string ThemesFolderPath { get => Path.GetFullPath(".\\Config\\Themes"); }
		/// <summary>
		/// Event that is called when the themes list has been updated
		/// </summary>
		public event UpdateThemesListDelegate UpdatedThemesList;
		/// <summary>
		/// Delegate for <see cref="UpdatedThemesList"/> event
		/// </summary>
		public delegate void UpdateThemesListDelegate();
		/// <summary>
		/// Active theme to use
		/// </summary>
		public ITheme ActiveTheme
		{
			get
			{
				if (ConfigHandler.IsInitializing)
					return new DarkMode();
				ITheme activeTheme = Singleton.GetTheme(ConfigHandler.Instance.ActiveTheme);
				if (activeTheme == null) activeTheme = new DarkMode();
				return activeTheme;
			}
		}

		#region Singleton
		private static ThemeManager Singleton;
		/// <summary>
		/// Instance of <see cref="ThemeManager"/> object
		/// </summary>
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
			if (!Directory.Exists(ThemesFolderPath))
				Directory.CreateDirectory(ThemesFolderPath);
			Load();
		}

		/// <summary>
		/// Loads themes from file
		/// </summary>
		/// <returns>List of themes</returns>
		private void Load()
		{
			Directory.GetFiles(ThemesFolderPath).ToList().ForEach(file =>
			{
				try
				{
					ITheme theme = JsonConvert.DeserializeObject<ThemeSerialization>(File.ReadAllText(file));
					if (!Themes.ContainsKey(theme.Name))
						Themes.Add(theme.Name, theme);
				}
				catch
				{
					ThemedMessageBox.Show(text: "Unable to load " + file + "!", title: "Errow while loading theme");
				}
			});
		}

		/// <summary>
		/// Fetches theme from list
		/// </summary>
		/// <param name="name">Name of theme</param>
		/// <returns>Theme or <see langword="null"/> if not found</returns>
		public ITheme GetTheme(string name)
		{
			if (!Themes.ContainsKey(name))
				return null;
			return Themes[name];
		}

		/// <summary>
		/// Saves a theme to file and updates themes list
		/// </summary>
		/// <param name="theme">Theme to save</param>
		/// <returns>status code for save operation</returns>
		public SaveStatusCodes SaveTheme(ITheme theme)
		{
			SaveStatusCodes returnCode = SaveStatusCodes.Success;
			string themePath = Path.Combine(ThemesFolderPath, theme.Name + ".bmtheme");
			if (File.Exists(themePath) || Themes.ContainsKey(theme.Name))
			{
				if (ThemedMessageBox.Show(text: "Overwrite existing file: " + ThemesFolderPath + theme.Name + ".bmtheme ?", title: "Overwrite file?", buttons: MessageBoxButtons.YesNo) != DialogResult.Yes)
					return SaveStatusCodes.OverwriteDeclined;
			}
			else returnCode = SaveStatusCodes.NewThemeCreated;
			File.WriteAllText(themePath, JsonConvert.SerializeObject(theme, Formatting.Indented));

			if (Themes.ContainsKey(theme.Name))
			{
				Themes[theme.Name] = theme;
			}
			else
			{
				Themes.Add(theme.Name, theme);
			}
			UpdatedThemesList?.Invoke();

			return returnCode;
		}

		/// <summary>
		/// Gets the names of available <see cref="ITheme"/>s
		/// </summary>
		/// <returns><see cref="List{T}"/> of <see cref="ITheme"/> names</returns>
		public static List<string> GetThemeNames()
		{
			return Instance.Themes.Keys.ToList();
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
		[DefaultValue("Untitled")]
		public string Name { get; set; }
		[DefaultValue(typeof(Color), "White")]
		public Color TextBoxBackColor { get; set; }
		[DefaultValue(typeof(Color), "White")]
		public Color TextBoxDisabledBackColor { get; set; }
		[DefaultValue(typeof(Color), "White")]
		public Color TextBoxBorderColor { get; set; }
		[DefaultValue(typeof(Color), "White")]
		public Color TextBoxArrowColor { get; set; }
		[DefaultValue(typeof(Color), "White")]
		public Color TextBoxReadOnlyColor { get; set; }
		[DefaultValue(typeof(Color), "White")]
		public Color ColoredComboBoxDropDownButtonBackColor { get; set; }
		[DefaultValue(typeof(Color), "White")]
		public Color ColoredComboBoxTextColor { get; set; }
		[DefaultValue(typeof(Color), "White")]
		public Color ColoredComboBoxDisabledColor { get; set; }
		[DefaultValue(typeof(Color), "White")]
		public Color ColoredComboBoxDisabledTextColor { get; set; }
		[DefaultValue(typeof(Color), "White")]
		public Color ColoredComboBoxHighlightColor { get; set; }
		[DefaultValue(typeof(Color), "White")]
		public Color MenuStripBackColor { get; set; }
		[DefaultValue(typeof(Color), "White")]
		public Color MenuStripDropdownBackColor { get; set; }
		[DefaultValue(typeof(Color), "White")]
		public Color MenuStripSelectedDropDownBackColor { get; set; }
		[DefaultValue(typeof(Color), "White")]
		public Color ForeColor { get; set; }
		[DefaultValue(typeof(Color), "White")]
		public Color BackColor { get; set; }
		[DefaultValue(typeof(Color), "White")]
		public Color ButtonColor { get; set; }
		[DefaultValue(typeof(Color), "White")]
		public Color ButtonDisabledColor { get; set; }
		[DefaultValue(typeof(Color), "White")]
		public Color ButtonDisabledTextColor { get; set; }
		[DefaultValue(typeof(Color), "White")]
		public Color ButtonFocusedBorderColor { get; set; }
		[DefaultValue(1f)]
		public float ButtonFocusBorderWidth { get; set; }
		[DefaultValue(typeof(Color), "White")]
		public Color ButtonHoverColor { get; set; }
		[DefaultValue(typeof(Color), "White")]
		public Color SplitterColor { get; set; }
		[DefaultValue(typeof(Color), "White")]
		public Color TreeViewDottedLineColor { get; set; }
		[DefaultValue(typeof(Color), "White")]
		public Color TreeViewHighlightedNodeColor { get; set; }
		[DefaultValue(typeof(Color), "White")]
		public Color TreeViewReportOpenedHighlightColor { get; set; }
		[DefaultValue(typeof(Color), "White")]
		public Color ReportUploadedColor { get; set; }
		[DefaultValue(typeof(Color), "White")]
		public Color ReportHandedInColor { get; set; }
		[DefaultValue(typeof(Color), "White")]
		public Color ReportAcceptedColor { get; set; }
		[DefaultValue(typeof(Color), "White")]
		public Color ReportRejectedColor { get; set; }

	}
}
