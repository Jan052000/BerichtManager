using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Windows.Forms;
using BerichtManager.Forms;
using System.Collections.Generic;
using System.Globalization;
using BerichtManager.ThemeManagement;
using BerichtManager.OwnControls;

namespace BerichtManager.Config
{
	public class ConfigHandler
	{
		/// <summary>
		/// Path to directory containing config file
		/// </summary>
		private static string ConfigFolderPath { get; } = Environment.CurrentDirectory + "\\Config";
		/// <summary>
		/// Name of config file
		/// </summary>
		private static string FileName { get; } = "Config.json";
		/// <summary>
		/// Full path of config file
		/// </summary>
		private string FullPath { get; } = ConfigFolderPath + "\\" + FileName;
		/// <summary>
		/// Internal object to reduce number of IO calls
		/// </summary>
		private JObject ConfigObject { get; set; }

		#region Singleton
		private static ConfigHandler Singleton;
		public static ConfigHandler Instance
		{
			get
			{
				if (Singleton == null)
					Singleton = new ConfigHandler();
				return Singleton;
			}
		}
		#endregion

		#region Default config properties
		/// <summary>
		/// Holds all values for config with the keys as key and the type and default value as values
		/// </summary>
		Dictionary<string, (Type Type, object DefaultValue)> Config { get; } = new Dictionary<string, (Type, object)>()
		{
			{"TemplatePath", (Type.GetType("System.String"), "")},
			{"ReportNR", (Type.GetType("System.Int32"), 1)},
			{"Active", (Type.GetType("System.String"), "")},
			{"Username", (Type.GetType("System.String"), "")},
			{"Password", (Type.GetType("System.String"), "")},
			{"Name", (Type.GetType("System.String"), "")},
			{"Font", (Type.GetType("System.String"), "Arial")},
			{"EditorFontSize", (Type.GetType("System.Single"), 8.25f)},
			{"LastReportWeekOfYear", (Type.GetType("System.Int32"), new CultureInfo("de-DE").Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday) - 1)},
			{"StayLoggedIn", (Type.GetType("System.Boolean"), false)},
			{"UseCustomPrefix", (Type.GetType("System.Boolean"), false)},
			{"CustomPrefix", (Type.GetType("System.String"), "-")},
			{"WebUntisServer", (Type.GetType("System.String"), "borys")},
			{"SchoolName", (Type.GetType("System.String"), "pictorus-bk")},
			{"UseWebUntis", (Type.GetType("System.Boolean"), true)},
			{"EndWeekOnFriday", (Type.GetType("System.Boolean"), false)},
			{"EnableLegacyEdit", (Type.GetType("System.Boolean"), false)},
			{"ActiveTheme", (Type.GetType("System.String"), "Dark Mode")},
			{"ReportPath", (Type.GetType("System.String"), Path.GetFullPath(".\\.."))},
			{"PublishPath", (Type.GetType("System.String"), "T:\\Azubis\\Berichtmanager\\BerichtManager.exe")},
			{"TabStops", (Type.GetType("System.Int32"), 20)},
			{"NamingPattern", (Type.GetType("System.String"), "WochenberichtKW~+CW+~")}
		};
		#endregion

		private ConfigHandler()
		{
			bool isComplete = true;
			if (!ConfigExists())
			{
				Directory.CreateDirectory(ConfigFolderPath);
				File.Create(FullPath).Close();
				ConfigObject = new JObject();
				foreach (KeyValuePair<string, (Type Type, object DefaultValue)> kvp in Config)
				{
					ConfigObject.Add(new JProperty(kvp.Key, Convert.ChangeType(kvp.Value.DefaultValue, kvp.Value.Type)));
				}
				isComplete = false;
			}
			else
			{
				if (new FileInfo(FullPath).Length == 0)
				{
					ConfigObject = new JObject();
				}
				else
				{
					ConfigObject = JObject.Parse(File.ReadAllText(FullPath));
				}
				foreach (KeyValuePair<string, (Type Type, object DefaultValue)> kvp in Config)
				{
					if (!ConfigObject.ContainsKey(kvp.Key))
					{
						isComplete = false;
						switch (kvp.Key)
						{
							case "TemplatePath":
								OpenFileDialog dialog = new OpenFileDialog();
								dialog.Filter = "Word Templates (*.dotx)|*.dotx";
								ThemedMessageBox.Show(ThemeManager.Instance.ActiveTheme, "Please select a word template to use", "Select a template");
								if (dialog.ShowDialog() == DialogResult.OK)
									ThemedMessageBox.Show(ThemeManager.Instance.ActiveTheme, "Template selected: " + dialog.FileName, "Info");
								ConfigObject.Add("TemplatePath", dialog.FileName);
								break;
							case "ReportNR":
								EditForm reportNumberForm = new EditForm(title: "Edit Number of Report", text: "1", stopConfigCalls: true);
								if (reportNumberForm.ShowDialog() == DialogResult.OK)
								{
									if (int.TryParse(reportNumberForm.Result, out int value))
										ConfigObject.Add(new JProperty("ReportNR", value));
									else
									{
										ThemedMessageBox.Show(ThemeManager.Instance.ActiveTheme, "Invalid number, defaulting to 1! (This can be changed later in options menu)", "Invalid number!");
										ConfigObject.Add(new JProperty("ReportNR", 1));
									}
								}
								else
									ConfigObject.Add(new JProperty("ReportNR", 1));
								break;
							case "Name":
								EditForm nameForm = new EditForm(title: "Enter your name", text: "Name Vorname", stopConfigCalls: true);
								if (nameForm.ShowDialog() == DialogResult.OK)
									ConfigObject.Add(new JProperty("Name", nameForm.Result));
								else
									ConfigObject.Add(new JProperty("Name", ""));
								break;
							default:
								ConfigObject.Add(new JProperty(kvp.Key, Convert.ChangeType(kvp.Value.DefaultValue, kvp.Value.Type)));
								break;
						}
					}
				}
			}
			if (!isComplete)
				File.WriteAllText(FullPath, JsonConvert.SerializeObject(ConfigObject, Formatting.Indented));
		}

		/// <summary>
		/// Checks if config file exists
		/// </summary>
		/// <returns>Config file exists</returns>
		public bool ConfigExists()
		{
			return File.Exists(FullPath);
		}

		/// <summary>
		/// Generic method for getting values from the configObject
		/// </summary>
		/// <typeparam name="T">Type of the value to get</typeparam>
		/// <param name="key">Key of the value to get</param>
		/// <returns>Value of <paramref name="key"/> converted to type <typeparamref name="T"/></returns>
		private T GenericGet<T>(string key)
		{
			return ConfigObject.Value<T>(key);
		}

		/// <summary>
		/// Generic method for setting values of the configObject
		/// </summary>
		/// <typeparam name="T">Type of the value to set</typeparam>
		/// <param name="key">Key of the value to set</param>
		/// <param name="value">Value to set</param>
		private void GenericSet<T>(string key, T value)
		{
			ConfigObject.Remove(key);
			ConfigObject.Add(new JProperty(key, value));
		}

		/// <summary>
		/// Saves the configObject to file
		/// </summary>
		public void SaveConfig()
		{
			File.WriteAllText(FullPath, JsonConvert.SerializeObject(ConfigObject, Formatting.Indented));
		}

		/// <summary>
		/// Reloads config from file
		/// </summary>
		public void ReloadConfig()
		{
			ConfigObject = JObject.Parse(File.ReadAllText(FullPath));
		}

		/// <summary>
		/// Loads the path to the template
		/// </summary>
		/// <returns>The path to the template</returns>
		public string TemplatePath()
		{
			return GenericGet<string>("TemplatePath");
		}

		/// <summary>
		/// Sets the template path
		/// </summary>
		/// <param name="templateFilePath">Path to template</param>
		public void TemplatePath(string templateFilePath)
		{
			GenericSet("TemplatePath", templateFilePath);
		}

		/// <summary>
		/// Loads the number of the next report
		/// </summary>
		/// <returns>The number of the next report</returns>
		public int ReportNumber()
		{
			return GenericGet<int>("ReportNR");
		}

		/// <summary>
		/// Sets the number of the next report
		/// </summary>
		/// <param name="number">Number of the next report</param>
		public void ReportNumber(int number)
		{
			GenericSet("ReportNR", number);
		}

		/// <summary>
		/// Sets the last active document path
		/// </summary>
		/// <param name="activeDocument">Path to last created document</param>
		public void LastCreated(string activeDocument)
		{
			GenericSet("Active", activeDocument);
		}

		/// <summary>
		/// Loads the last active document path
		/// </summary>
		/// <returns>Path to last created document</returns>
		public string LastCreated()
		{
			return GenericGet<string>("Active");
		}

		/// <summary>
		/// Loads the username for webuntis
		/// </summary>
		/// <returns>Username</returns>
		public string WebUntisUsername()
		{
			return GenericGet<string>("Username");
		}

		/// <summary>
		/// Sets the username for webuntis
		/// </summary>
		/// <param name="username">Username</param>
		private void WebUntisUsername(string username)
		{
			GenericSet("Username", username);
		}

		/// <summary>
		/// Loads the password for Webuntis
		/// </summary>
		/// <returns>Password</returns>
		public string WebUntisPassword()
		{
			return UserHandler.DecodePassword(GenericGet<string>("Password"));
		}

		/// <summary>
		/// Sets the password for Webuntis
		/// </summary>
		/// <param name="password">Password</param>
		private void WebUntisPassword(string password)
		{
			GenericSet("Password", UserHandler.EncodePassword(password));
		}

		/// <summary>
		/// Starts the login process for the user
		/// </summary>
		/// <returns><see cref="User"/> object containing username and password</returns>
		public User DoWebUntisLogin()
		{
			Login form = new Login("WebUntis");
			form.ShowDialog();
			if (form.DialogResult == DialogResult.OK)
			{
				if (form.KeepLoggedIn)
				{
					WebUntisUsername(form.Username);
					WebUntisPassword(form.Password);
				}
				else
				{
					WebUntisUsername("");
					WebUntisPassword("");
				}
				StayLoggedIn(form.KeepLoggedIn);
				SaveConfig();
				return new User(username: form.Username, password: form.Password);
			}
			return new User();
		}

		/// <summary>
		/// Loads name to be used in report
		/// </summary>
		/// <returns>Name to be used in report</returns>
		public string ReportUserName()
		{
			return GenericGet<string>("Name");
		}

		/// <summary>
		/// Sets name to be used in report
		/// </summary>
		/// <param name="name">Name to be used in report</param>
		public void ReportUserName(string name)
		{
			GenericSet("Name", name);
		}

		/// <summary>
		/// Loads font to be used in editor and report
		/// </summary>
		/// <returns>Font to be used in editor and report</returns>
		public string EditorFont()
		{
			return GenericGet<string>("Font");
		}

		/// <summary>
		/// Sets font to be used in editor and report
		/// </summary>
		/// <param name="fontName">Font to be used in editor and report</param>
		public void EditorFont(string fontName)
		{
			GenericSet("Font", fontName);
		}

		/// <summary>
		/// Loads the font size
		/// </summary>
		/// <returns>Font size</returns>
		public float EditorFontSize()
		{
			return GenericGet<float>("EditorFontSize");
		}

		/// <summary>
		/// Sets the font size
		/// </summary>
		/// <param name="size">Font size</param>
		public void EditorFontSize(float size)
		{
			GenericSet("EditorFontSize", size);
		}

		/// <summary>
		/// Loads the weeknumber of the last report that was created
		/// </summary>
		/// <returns>The weeknumber of the last report that was created</returns>
		public int LastReportKW()
		{
			return GenericGet<int>("LastReportWeekOfYear");
		}

		/// <summary>
		/// Sets the weeknumber of the last report that was created
		/// </summary>
		/// <param name="kw">Weeknumber of the last report that was created</param>
		public void LastReportKW(int kw)
		{
			GenericSet("LastReportWeekOfYear", kw);
		}

		/// <summary>
		/// Gets the boolean if the User wanted to stay logged in
		/// </summary>
		/// <returns>If the User wanted to stay logged in</returns>
		public bool StayLoggedIn()
		{
			return GenericGet<bool>("StayLoggedIn");
		}

		/// <summary>
		/// Sets if the user wanted to stay logged in
		/// </summary>
		/// <param name="stayLoggedIn">Should the user stay logged in</param>
		public void StayLoggedIn(bool stayLoggedIn)
		{
			GenericSet("StayLoggedIn", stayLoggedIn);
		}

		/// <summary>
		/// Gets wether or not to use the custom prefix for listing classes
		/// </summary>
		/// <returns>custom prefix should be used</returns>
		public bool UseUserPrefix()
		{
			return GenericGet<bool>("UseCustomPrefix");
		}

		/// <summary>
		/// Sets wether or not to use the custom prefix for listing classes
		/// </summary>
		/// <param name="useUserPrefix">the custom prefix should be used</param>
		public void UseUserPrefix(bool useUserPrefix)
		{
			GenericSet<bool>("UseCustomPrefix", useUserPrefix);
		}

		/// <summary>
		/// Gets the custom prefix from the configObject
		/// </summary>
		/// <returns>custom prefix</returns>
		public string CustomPrefix()
		{
			return GenericGet<string>("CustomPrefix");
		}

		/// <summary>
		/// Sets the custom prefix
		/// </summary>
		/// <param name="customPrefix">customPrefix to use</param>
		public void CustomPrefix(string customPrefix)
		{
			GenericSet<string>("CustomPrefix", customPrefix);
		}

		/// <summary>
		/// Gets the WebUntis server from config
		/// </summary>
		/// <returns>WebUntis server to query</returns>
		public string WebUntisServer()
		{
			return GenericGet<string>("WebUntisServer");
		}

		/// <summary>
		/// Sets WebUntis server to query
		/// </summary>
		/// <param name="server">server to query</param>
		public void WebUntisServer(string server)
		{
			GenericSet<string>("WebUntisServer", server);
		}

		/// <summary>
		/// Gets school name from config
		/// </summary>
		/// <returns>school name to use</returns>
		public string SchoolName()
		{
			return GenericGet<string>("SchoolName");
		}

		/// <summary>
		/// Sets the school name to use
		/// </summary>
		/// <param name="schoolName">school name to use</param>
		public void SchoolName(string schoolName)
		{
			GenericSet<string>("SchoolName", schoolName);
		}

		/// <summary>
		/// Gets if the classes should be queried from WebUntis
		/// </summary>
		/// <returns>classes should be queried</returns>
		public bool UseWebUntis()
		{
			return GenericGet<bool>("UseWebUntis");
		}

		/// <summary>
		/// Sets if the classes should be queried from WebUntis
		/// </summary>
		/// <param name="useWebUntis">classes should be queried</param>
		public void UseWebUntis(bool useWebUntis)
		{
			GenericSet<bool>("UseWebUntis", useWebUntis);
		}

		/// <summary>
		/// Gets if week end dates should be friday instead of sunday
		/// </summary>
		/// <returns>week end dates should be friday instead of sunday</returns>
		public bool EndWeekOnFriday()
		{
			return GenericGet<bool>("EndWeekOnFriday");
		}

		/// <summary>
		/// Sets if week end dates should be friday instead of sunday
		/// </summary>
		/// <param name="endWeekOnFriday">week end dates should be friday instead of sunday</param>
		public void EndWeekOnFriday(bool endWeekOnFriday)
		{
			GenericSet<bool>("EndWeekOnFriday", endWeekOnFriday);
		}

		/// <summary>
		/// Gets if legacy edit should be used
		/// </summary>
		/// <returns>if legacy edit should be used</returns>
		public bool LegacyEdit()
		{
			return GenericGet<bool>("EnableLegacyEdit");
		}

		/// <summary>
		/// Sets if legacy edit should be used
		/// </summary>
		/// <param name="legacyEdit">if legacy edit should be used</param>
		public void LegacyEdit(bool legacyEdit)
		{
			GenericSet("EnableLegacyEdit", legacyEdit);
		}

		/// <summary>
		/// Gets name of theme to be used
		/// </summary>
		/// <returns>Theme name</returns>
		public string ActiveTheme()
		{
			return GenericGet<string>("ActiveTheme");
		}

		/// <summary>
		/// Sets name of theme to be used
		/// </summary>
		/// <param name="themeName">Name of theme to be used</param>
		public void ActiveTheme(string themeName)
		{
			GenericSet("ActiveTheme", themeName);
		}

		/// <summary>
		/// Gets path of folder containing all reports
		/// </summary>
		/// <returns>Path of folder containing all reports</returns>
		public string ReportPath()
		{
			return GenericGet<string>("ReportPath");
		}

		/// <summary>
		/// Sets path of folder containing all reports
		/// </summary>
		/// <param name="path">Path of folder containing all reports</param>
		public void ReportPath(string path)
		{
			GenericSet("ReportPath", path);
		}

		/// <summary>
		/// Gets path of published application to check for version number
		/// </summary>
		/// <returns>Path of published application</returns>
		public string PublishPath()
		{
			return GenericGet<string>("PublishPath");
		}

		/// <summary>
		/// Sets path of published application to check for version number
		/// </summary>
		/// <param name="path">Path of published application</param>
		public void PublishPath(string path)
		{
			GenericSet("PublishPath", path);
		}

		/// <summary>
		/// Gets number to be used for tab stops
		/// </summary>
		/// <returns>Number of spaces per tab</returns>
		public int TabStops()
		{
			return GenericGet<int>("TabStops");
		}

		/// <summary>
		/// Sets number to be used for tab stops
		/// </summary>
		/// <param name="tabStops">Number of spaces per tab</param>
		public void TabStops(int tabStops)
		{
			GenericSet("TabStops", tabStops);
		}

		/// <summary>
		/// Gets naming patterm
		/// </summary>
		/// <returns>Naming pattern</returns>
		public string NamingPattern()
		{
			return GenericGet<string>("NamingPattern");
		}

		/// <summary>
		/// Sets naming pattern
		/// </summary>
		/// <param name="pattern">Naming pattern</param>
		public void NamingPattern(string pattern)
		{
			GenericSet("NamingPattern", pattern);
		}

		private void SortConfig()
		{
			List<JProperty> jProperties = new List<JProperty>();
			JObject jobject = JObject.Parse(File.ReadAllText(FullPath));
			var test = jobject.Children<JProperty>().OrderBy(p => p.Name);
		}
	}

	public class User
	{
		public string Username { get; set; }
		public string Password { get; set; }
		public User(string username = "", string password = "")
		{
			Username = username;
			Password = password;
		}
	}
}
