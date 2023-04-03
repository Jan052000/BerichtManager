using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Windows.Forms;
using BerichtManager.AddForm;
using System.Collections.Generic;
using System.Globalization;
using BerichtManager.ThemeManagement;

namespace BerichtManager.Config
{
	public class ConfigHandler
	{
		private readonly static string path = Environment.CurrentDirectory + "\\Config";
		private readonly static string FileName = "Config.json";
		private readonly string FullPath = path + "\\" + FileName;
		private readonly JObject configObject;
		public bool loginAborted = false;
		private ThemeManager themeManager;
		public ConfigHandler(ThemeManager themeManager)
		{
			this.themeManager = themeManager;
			bool isComplete = true;
			if (!ConfigExists())
			{
				Directory.CreateDirectory(path);
				File.Create(FullPath).Close();
				configObject = new JObject(new JProperty("TemplatePath", ""), new JProperty("ReportNR", "1"), new JProperty("Active", ""), new JProperty("Username", ""), new JProperty("Password", ""),
					new JProperty("Name", ""), new JProperty("Font", "Arial"), new JProperty("EditorFontSize", 8.25f), new JProperty("LastReportWeekOfYear", new CultureInfo("de-DE").Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday) - 1),
					new JProperty("StayLoggedIn", false), new JProperty("UseCustomPrefix", false), new JProperty("CustomPrefix", "-"), new JProperty("WebUntisServer", "borys"), new JProperty("SchoolName", "pictorus-bk"),
					new JProperty("UseWebUntis", true), new JProperty("EndWeekOnFriday", false), new JProperty("EnableLegacyEdit", false), new JProperty("ActiveTheme", "Dark Mode"),
					new JProperty("ReportPath", Path.GetFullPath(".\\..")), new JProperty("PublishPath", "T:\\Azubis\\Berichtmanager\\BerichtManager.exe"));
				File.WriteAllText(FullPath, JsonConvert.SerializeObject(configObject, Formatting.Indented));
			}
			else
			{
				if (new FileInfo(FullPath).Length == 0)
				{
					configObject = new JObject();
				}
				else
				{
					configObject = JObject.Parse(File.ReadAllText(FullPath));
				}
				if (!configObject.ContainsKey("TemplatePath"))
				{
					OpenFileDialog dialog = new OpenFileDialog();
					dialog.Filter = "Word Templates (*.dotx)|*.dotx";
					dialog.ShowDialog();
					Save(Path.GetFullPath(dialog.FileName));
					MessageBox.Show("Muster auf: " + Path.GetFullPath(dialog.FileName) + " gesetzt");
					isComplete = false;
				}
				if (!configObject.ContainsKey("ActiveTheme"))
				{
					configObject.Add(new JProperty("ActiveTheme", "Dark Mode"));
					isComplete = false;
				}
				if (!configObject.ContainsKey("ReportNR"))
				{
					EditForm form = new EditForm("Edit Number of Report", themeManager.GetTheme(ActiveTheme()), "", false);
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						EditNumber(form.Result);
					}
					isComplete = false;
				}
				if (!configObject.ContainsKey("Active"))
				{
					configObject.Add(new JProperty("Active", ""));
					isComplete = false;
				}
				if (!configObject.ContainsKey("Username"))
				{
					configObject.Add(new JProperty("Username", ""));
					isComplete = false;
				}
				if (!configObject.ContainsKey("Password"))
				{
					configObject.Add(new JProperty("Password", ""));
					isComplete = false;
				}
				if (!configObject.ContainsKey("Name"))
				{
					EditForm form = new EditForm("Enter your name", themeManager.GetTheme(ActiveTheme()), "Name Vorname", false);
					if (form.ShowDialog() == DialogResult.OK)
					{
						configObject.Add(new JProperty("Name", form.Result));
					}
					else
					{
						configObject.Add(new JProperty("Name", ""));
					}
					isComplete = false;
				}
				if (!configObject.ContainsKey("Font"))
				{
					configObject.Add(new JProperty("Font", "Arial"));
					isComplete = false;
				}
				if (!configObject.ContainsKey("EditorFontSize"))
				{
					configObject.Add(new JProperty("EditorFontSize", 8.25f));
					isComplete = false;
				}
				if (!configObject.ContainsKey("LastReportWeekOfYear"))
				{
					configObject.Add(new JProperty("LastReportWeekOfYear", new CultureInfo("de-DE").Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday) - 1));
					isComplete = false;
				}
				if (!configObject.ContainsKey("StayLoggedIn"))
				{
					configObject.Add(new JProperty("StayLoggedIn", false));
					isComplete = false;
				}
				if (!configObject.ContainsKey("UseCustomPrefix"))
				{
					configObject.Add(new JProperty("UseCustomPrefix", false));
					isComplete = false;
				}
				if (!configObject.ContainsKey("CustomPrefix"))
				{
					configObject.Add(new JProperty("CustomPrefix", "-"));
					isComplete = false;
				}
				if (!configObject.ContainsKey("WebUntisServer"))
				{
					configObject.Add(new JProperty("WebUntisServer", "borys"));
					isComplete = false;
				}
				if (!configObject.ContainsKey("SchoolName"))
				{
					configObject.Add(new JProperty("SchoolName", "pictorus-bk"));
					isComplete = false;
				}
				if (!configObject.ContainsKey("UseWebUntis"))
				{
					configObject.Add(new JProperty("UseWebUntis", true));
					isComplete = false;
				}
				if (!configObject.ContainsKey("EndWeekOnFriday"))
				{
					configObject.Add(new JProperty("EndWeekOnFriday", false));
					isComplete = false;
				}
				if (!configObject.ContainsKey("EnableLegacyEdit"))
				{
					configObject.Add(new JProperty("EnableLegacyEdit", false));
					isComplete = false;
				}
				if (!configObject.ContainsKey("ReportPath"))
				{
					configObject.Add(new JProperty("ReportPath", Path.GetFullPath(".\\..")));
					isComplete = false;
				}
				if (!configObject.ContainsKey("PublishPath"))
				{
					configObject.Add(new JProperty("PublishPath", "T:\\Azubis\\Berichtmanager\\BerichtManager.exe"));
					isComplete = false;
				}
			}
			if (!isComplete)
				File.WriteAllText(FullPath, JsonConvert.SerializeObject(configObject, Formatting.Indented));
		}

		/// <summary>
		/// Generic method for getting values from the configObject
		/// </summary>
		/// <typeparam name="T">Type of the value to get</typeparam>
		/// <param name="key">Key of the value to get</param>
		/// <returns></returns>
		private T GenericGet<T>(string key)
		{
			return configObject.Value<T>(key);
		}

		/// <summary>
		/// Generic method for setting values of the configObject
		/// </summary>
		/// <typeparam name="T">Type of the value to set</typeparam>
		/// <param name="key">Key of the value to set</param>
		/// <param name="value">Value to set</param>
		private void GenericSet<T>(string key, T value)
		{
			configObject.Remove(key);
			configObject.Add(new JProperty(key, value));
		}

		/// <summary>
		/// Saves the configObject to file
		/// </summary>
		public void SaveConfig()
		{
			File.WriteAllText(FullPath, JsonConvert.SerializeObject(configObject, Formatting.Indented));
		}

		/// <summary>
		/// Loads the path to the template
		/// </summary>
		/// <returns>The path to the template</returns>
		public string LoadPath()
		{
			return GenericGet<string>("TemplatePath");
		}

		/// <summary>
		/// Loads the number of the next report
		/// </summary>
		/// <returns>The number of the next report</returns>
		public string LoadNumber()
		{
			return GenericGet<string>("ReportNR");
		}

		/// <summary>
		/// Sets the template path
		/// </summary>
		/// <param name="templateFilePath">Path to template</param>
		public void Save(string templateFilePath)
		{
			GenericSet("TemplatePath", templateFilePath);
		}

		/// <summary>
		/// Sets the number of the next report
		/// </summary>
		/// <param name="number">Number of the next report</param>
		public void EditNumber(string number)
		{
			GenericSet("ReportNR", number);
		}

		/// <summary>
		/// Sets the last active document path
		/// </summary>
		/// <param name="activeDocument">Path to last created document</param>
		public void EditActive(string activeDocument)
		{
			GenericSet("Active", activeDocument);
		}

		/// <summary>
		/// Loads the last active document path
		/// </summary>
		/// <returns>Path to last created document</returns>
		public string LoadActive()
		{
			return GenericGet<string>("Active");
		}

		/// <summary>
		/// Loads the username for webuntis
		/// </summary>
		/// <returns>Username</returns>
		public string LoadUsername()
		{
			return GenericGet<string>("Username");
		}

		/// <summary>
		/// Sets the username for webuntis
		/// </summary>
		/// <param name="username">Username</param>
		public void SaveUsername(string username)
		{
			GenericSet("Username", username);
		}

		/// <summary>
		/// Loads the password for Webuntis
		/// </summary>
		/// <returns>Password</returns>
		public string LoadPassword()
		{
			return UserHandler.DecodePassword(GenericGet<string>("Password"));
		}

		/// <summary>
		/// Sets the password for Webuntis
		/// </summary>
		/// <param name="password">Password</param>
		public void SavePassword(string password)
		{
			GenericSet("Password", UserHandler.EncodePassword(password));
		}

		/// <summary>
		/// Starts the login process for the user
		/// </summary>
		/// <returns><see cref="User"/> object containing username and password</returns>
		public User doLogin()
		{
			Login form = new Login(themeManager.GetTheme(ActiveTheme()));
			form.ShowDialog();
			if (form.DialogResult == DialogResult.OK)
			{
				if (form.KeepLoggedIn)
				{
					SaveUsername(form.Username);
					SavePassword(form.Password);
				}
				else
				{
					SaveUsername("");
					SavePassword("");
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
		public string LoadName()
		{
			return GenericGet<string>("Name");
		}

		/// <summary>
		/// Sets name to be used in report
		/// </summary>
		/// <param name="name">Name to be used in report</param>
		public void SaveName(string name)
		{
			GenericSet("Name", name);
		}

		/// <summary>
		/// Loads font to be used in editor and report
		/// </summary>
		/// <returns>Font to be used in editor and report</returns>
		public string LoadFont()
		{
			return GenericGet<string>("Font");
		}

		/// <summary>
		/// Sets font to be used in editor and report
		/// </summary>
		/// <param name="fontName">Font to be used in editor and report</param>
		public void SaveFont(string fontName)
		{
			GenericSet("Font", fontName);
		}

		/// <summary>
		/// Loads the font size
		/// </summary>
		/// <returns>Font size</returns>
		public float LoadEditorFontSize()
		{
			return GenericGet<float>("EditorFontSize");
		}

		/// <summary>
		/// Sets the font size
		/// </summary>
		/// <param name="size">Font size</param>
		public void SaveEditorFontSize(float size)
		{
			GenericSet("EditorFontSize", size);
		}

		/// <summary>
		/// Loads the weeknumber of the last report that was created
		/// </summary>
		/// <returns>The weeknumber of the last report that was created</returns>
		public int LoadLastReportKW()
		{
			return GenericGet<int>("LastReportWeekOfYear");
		}

		/// <summary>
		/// Sets the weeknumber of the last report that was created
		/// </summary>
		/// <param name="kw">Weeknumber of the last report that was created</param>
		public void SaveLastReportKW(int kw)
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
		public void SetUseUserPrefix(bool useUserPrefix)
		{
			GenericSet<bool>("UseCustomPrefix", useUserPrefix);
		}

		/// <summary>
		/// Gets the custom prefix from the configObject
		/// </summary>
		/// <returns>custom prefix</returns>
		public string GetCustomPrefix()
		{
			return GenericGet<string>("CustomPrefix");
		}

		/// <summary>
		/// Sets the custom prefix
		/// </summary>
		/// <param name="customPrefix">customPrefix to use</param>
		public void SetCustomPrefix(string customPrefix)
		{
			GenericSet<string>("CustomPrefix", customPrefix);
		}

		/// <summary>
		/// Gets the WebUntis server from config
		/// </summary>
		/// <returns>WebUntis server to query</returns>
		public string GetWebUntisServer()
		{
			return GenericGet<string>("WebUntisServer");
		}

		/// <summary>
		/// Sets WebUntis server to query
		/// </summary>
		/// <param name="server">server to query</param>
		public void SetWebUntisServer(string server)
		{
			GenericSet<string>("WebUntisServer", server);
		}

		/// <summary>
		/// Gets school name from config
		/// </summary>
		/// <returns>school name to use</returns>
		public string GetSchoolName()
		{
			return GenericGet<string>("SchoolName");
		}

		/// <summary>
		/// Sets the school name to use
		/// </summary>
		/// <param name="schoolName">school name to use</param>
		public void SetSchoolName(string schoolName)
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
		public void SetUseWebUntis(bool useWebUntis)
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
		/// Checks if config file exists
		/// </summary>
		/// <returns>Config file exists</returns>
		public bool ConfigExists()
		{
			return File.Exists(FullPath);
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

	public class DataNotFoundException : Exception
	{
		public DataNotFoundException() : base("Key and / or Value not found")
		{
		}
	}
}
