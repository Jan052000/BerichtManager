using BerichtManager.ThemeManagement.DefaultThemes;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;
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
		/// List of available theme objects
		/// </summary>
		public List<ITheme> AvailableThemes { get; private set; } = new List<ITheme>() { new DarkMode(), new LightMode() };
		/// <summary>
		/// List of available theme names
		/// </summary>
		public List<string> ThemeNames { get; private set; } = new List<string>() { "Dark Mode", "Light Mode" };
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
			AvailableThemes.AddRange(GetThemes());
		}

		/// <summary>
		/// Loads themes from file
		/// </summary>
		/// <returns>List of themes</returns>
		private List<ITheme> GetThemes()
		{
			List<ITheme> themes = new List<ITheme>();
			Directory.GetFiles(ThemesFolderPath).ToList().ForEach(file =>
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
					ThemedMessageBox.Show(text: "Unable to load " + file + "!", title: "Errow while loading theme");
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
			UpdatedThemesList?.Invoke();
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
			if (File.Exists(ThemesFolderPath + theme.Name + ".bmtheme") || ThemeNames.Contains(theme.Name))
			{
				if (ThemedMessageBox.Show(text: "Overwrite existing file: " + ThemesFolderPath + theme.Name + ".bmtheme ?", title: "Overwrite file?", buttons: MessageBoxButtons.YesNo) != DialogResult.Yes)
					return SaveStatusCodes.OverwriteDeclined;
			}
			else returnCode = SaveStatusCodes.NewThemeCreated;
			File.WriteAllText(ThemesFolderPath + "\\" + theme.Name + ".bmtheme", JsonConvert.SerializeObject(theme, Formatting.Indented));
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
