using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Windows.Forms;
using BerichtManager.Forms;
using System.Collections.Generic;
using System.Globalization;
using BerichtManager.OwnControls;
using System.Net.Mail;
using BerichtManager.HelperClasses;

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
			{"LastCreated", (Type.GetType("System.String"), "")},
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
			{"UseLegacyEdit", (Type.GetType("System.Boolean"), false)},
			{"ActiveTheme", (Type.GetType("System.String"), "Dark Mode")},
			{"ReportPath", (Type.GetType("System.String"), Path.GetFullPath(".\\.."))},
			{"PublishPath", (Type.GetType("System.String"), "T:\\Azubis\\Berichtmanager\\BerichtManager.exe")},
			{"TabStops", (Type.GetType("System.Int32"), 20)},
			{"NamingPattern", (Type.GetType("System.String"), "WochenberichtKW~+CW+~")},
			{"AutoSyncStatusesWithIHK", (Type.GetType("System.Boolean"), false)},
			{"IHKUserName", (Type.GetType("System.String"), "")},
			{"IHKPassword", (Type.GetType("System.String"), "")},
			{"IHKStayLoggedIn", (Type.GetType("System.Boolean"), false)},
			{"IHKJobField", (Type.GetType("System.String"), "")},
			{"IHKSupervisorEMail", (Type.GetType("System.String"), "")},
			{"IHKBaseUrl", (Type.GetType("System.String"), "https://www.bildung-ihk-nordwestfalen.de/")},
			{"IHKUploadDelay", (Type.GetType("System.Int32"), 500)},
			{"IHKCheckMatchingStartDates", (Type.GetType("System.Boolean"), true)}
		};
		#endregion

		#region Config properties
		/// <summary>
		/// Path of template to use for reports
		/// </summary>
		public string TemplatePath
		{
			get => GenericGet<string>("TemplatePath");
			set => GenericSet("TemplatePath", value);
		}

		/// <summary>
		/// Number of the next report
		/// </summary>
		public int ReportNumber
		{
			get => GenericGet<int>("ReportNR");
			set => GenericSet("ReportNR", value);
		}

		/// <summary>
		/// Path to last created report
		/// </summary>
		public string LastCreated
		{
			get => GenericGet<string>("LastCreated");
			set => GenericSet("LastCreated", value);
		}

		/// <summary>
		/// Username for WebUntis login
		/// </summary>
		public string WebUntisUsername
		{
			get => GenericGet<string>("Username");
			set => GenericSet("Username", value);
		}

		/// <summary>
		/// Password for WebUntis login
		/// </summary>
		public string WebUntisPassword
		{
			get => UserHandler.DecodePassword(GenericGet<string>("Password"));
			private set => GenericSet("Password", UserHandler.EncodePassword(value));
		}

		/// <summary>
		/// Name to use in reports
		/// </summary>
		public string ReportUserName
		{
			get => GenericGet<string>("Name");
			set => GenericSet("Name", value);
		}

		/// <summary>
		/// Font of the editor
		/// </summary>
		public string EditorFont
		{
			get => GenericGet<string>("Font");
			set => GenericSet("Font", value);
		}

		/// <summary>
		/// Size of the font for the editor
		/// </summary>
		public float EditorFontSize
		{
			get => GenericGet<float>("EditorFontSize");
			set => GenericSet("EditorFontSize", value);
		}

		/// <summary>
		/// Week of year the last report was created
		/// </summary>
		public int LastReportWeekOfYear
		{
			get => GenericGet<int>("LastReportWeekOfYear");
			set => GenericSet("LastReportWeekOfYear", value);
		}

		/// <summary>
		/// Wether or not the user login info for WebUntis is saved
		/// </summary>
		public bool WebUntisStayLoggedIn
		{
			get => GenericGet<bool>("StayLoggedIn");
			set => GenericSet("StayLoggedIn", value);
		}

		/// <summary>
		/// Wether or not to use the custom prefix when listing
		/// </summary>
		public bool UseCustomPrefix
		{
			get => GenericGet<bool>("UseCustomPrefix");
			set => GenericSet("UseCustomPrefix", value);
		}

		/// <summary>
		/// Custom prefix to use when listing
		/// </summary>
		public string CustomPrefix
		{
			get => GenericGet<string>("CustomPrefix");
			set => GenericSet("CustomPrefix", value);
		}

		/// <summary>
		/// Name of the WebUntis server to query
		/// </summary>
		public string WebUntisServer
		{
			get => GenericGet<string>("WebUntisServer");
			set => GenericSet("WebUntisServer", value);
		}

		/// <summary>
		/// Name of the school in WebUntis
		/// </summary>
		public string SchoolName
		{
			get => GenericGet<string>("SchoolName");
			set => GenericSet("SchoolName", value);
		}

		/// <summary>
		/// Wether or not to use WebUntis to fetch classes
		/// </summary>
		public bool UseWebUntis
		{
			get => GenericGet<bool>("UseWebUntis");
			set => GenericSet("UseWebUntis", value);
		}

		/// <summary>
		/// Wether or not the report week should end on a friday
		/// </summary>
		public bool EndWeekOnFriday
		{
			get => GenericGet<bool>("EndWeekOnFriday");
			set => GenericSet("EndWeekOnFriday", value);
		}

		/// <summary>
		/// Wether or not to use legacy edit
		/// </summary>
		public bool UseLegacyEdit
		{
			get => GenericGet<bool>("UseLegacyEdit");
			set => GenericSet("UseLegacyEdit", value);
		}

		/// <summary>
		/// Name of the active theme
		/// </summary>
		public string ActiveTheme
		{
			get => GenericGet<string>("ActiveTheme");
			set => GenericSet("ActiveTheme", value);
		}

		/// <summary>
		/// Path to use as base folder
		/// </summary>
		public string ReportPath
		{
			get => GenericGet<string>("ReportPath");
			set => GenericSet("ReportPath", value);
		}

		/// <summary>
		/// Path of published exe
		/// </summary>
		public string PublishPath
		{
			get => GenericGet<string>("PublishPath");
			set => GenericSet("PublishPath", value);
		}

		/// <summary>
		/// Size of tab stops in editor
		/// </summary>
		public int TabStops
		{
			get => GenericGet<int>("TabStops");
			set => GenericSet("TabStops", value);
		}

		/// <summary>
		/// Pattern to be used when naming report files
		/// </summary>
		public string NamingPattern
		{
			get => GenericGet<string>("NamingPattern");
			set => GenericSet("NamingPattern", value);
		}

		/// <summary>
		/// Wether or not to fetch report statuses from IHK servers on start
		/// </summary>
		public bool AutoSyncStatusesWithIHK
		{
			get => GenericGet<bool>("AutoSyncStatusesWithIHK");
			set => GenericSet("AutoSyncStatusesWithIHK", value);
		}

		/// <summary>
		/// Username for IHK login
		/// </summary>
		public string IHKUserName
		{
			get => GenericGet<string>("IHKUserName");
			set => GenericSet("IHKUserName", value);
		}

		/// <summary>
		/// Password for IHK login
		/// </summary>
		public string IHKPassword
		{
			get => UserHandler.DecodePassword(GenericGet<string>("IHKPassword"));
			private set => GenericSet("IHKPassword", UserHandler.EncodePassword(value));
		}

		/// <summary>
		/// Wether or not the user login info for IHK is saved
		/// </summary>
		public bool IHKStayLoggedIn
		{
			get => GenericGet<bool>("IHKStayLoggedIn");
			set => GenericSet("IHKStayLoggedIn", value);
		}

		/// <summary>
		/// Job field to fill in report on IHK servers
		/// </summary>
		public string IHKJobField
		{
			get => GenericGet<string>("IHKJobField");
			set => GenericSet("IHKJobField", value);
		}

		/// <summary>
		/// Supervisor email to fill in report on IHK servers
		/// </summary>
		public string IHKSupervisorEMail
		{
			get => GenericGet<string>("IHKSupervisorEMail");
			set
			{
				try
				{
					MailAddress mailAddress = new MailAddress(value);
					GenericSet("IHKSupervisorEMail", value);
				}
				catch (FormatException)
				{
					ThemedMessageBox.Show(text: "Invalid e-mail", title: "Invalid mail");
					GenericSet("IHKSupervisorEMail", "");
				}
				catch (Exception e)
				{
					string logPath = Logger.LogError(e);
					ThemedMessageBox.Show(text: "Something went wrong saving the supervisor e-mail, a log was saved to:\n" + logPath, title: "An error occurred");
				}
			}
		}

		/// <summary>
		/// URL of IHK server without the endpoint
		/// </summary>
		public string IHKBaseUrl
		{
			get => GenericGet<string>("IHKBaseUrl");
			set => GenericSet("IHKBaseUrl", value);
		}

		/// <summary>
		/// Delay after uploading a report to IHK servers
		/// </summary>
		public int IHKUploadDelay
		{
			get => GenericGet<int>("IHKUploadDelay");
			set => GenericSet("IHKUploadDelay", value);
		}

		/// <summary>
		/// Wether or not to check for matching report start dates when uploading a report to IHK servers
		/// </summary>
		public bool IHKCheckMatchingStartDates
		{
			get => GenericGet<bool>("IHKCheckMatchingStartDates");
			set => GenericSet("IHKCheckMatchingStartDates", value);
		}
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
								ThemedMessageBox.Show(text: "Please select a word template to use", title: "Select a template");
								if (dialog.ShowDialog() == DialogResult.OK)
									ThemedMessageBox.Show(text: "Template selected: " + dialog.FileName, title: "Info");
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
										ThemedMessageBox.Show(text: "Invalid number, defaulting to 1! (This can be changed later in options menu)", title: "Invalid number!");
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
			{
				//Clean unused fields
				List<string> removeFields = new List<string>();
				foreach (KeyValuePair<string, JToken> kvp in ConfigObject)
				{
					if (!Config.ContainsKey(kvp.Key))
						removeFields.Add(kvp.Key);
				}
				removeFields.ForEach(field => ConfigObject.Remove(field));

				File.WriteAllText(FullPath, JsonConvert.SerializeObject(ConfigObject, Formatting.Indented));
			}
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
					WebUntisUsername = form.Username;
					WebUntisPassword = form.Password;
				}
				else
				{
					WebUntisUsername = "";
					WebUntisPassword = "";
				}
				WebUntisStayLoggedIn = form.KeepLoggedIn;
				SaveConfig();
				return new User(username: form.Username, password: form.Password);
			}
			return new User();
		}

		/// <summary>
		/// Opens a <see cref="Login"/> form and saves the login data
		/// </summary>
		/// <returns>New <see cref="User"/> containing username and password or <see langword="null"/> if login was aborted</returns>
		public User DoIHKLogin()
		{
			Login login = new Login("IHK");
			User user = null;
			if (login.ShowDialog() == DialogResult.OK)
			{
				if (login.KeepLoggedIn)
				{
					IHKUserName = login.Username;
					IHKPassword = login.Password;
				}
				else
				{
					IHKUserName = "";
					IHKPassword = "";
				}
				IHKStayLoggedIn = login.KeepLoggedIn;
				SaveConfig();
				user = new User(username: login.Username, password: login.Password);
			}
			return user;
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
