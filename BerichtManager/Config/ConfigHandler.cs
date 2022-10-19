using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Windows.Forms;
using BerichtManager.AddForm;
using System.Collections.Generic;

namespace BerichtManager.Config
{
	public class ConfigHandler
	{
		private readonly string path = Environment.CurrentDirectory + "\\Config";
		public bool loginAborted = false;
		public ConfigHandler() 
		{
			if (!File.Exists(path + "\\Config.json")) 
			{
				Directory.CreateDirectory(path);
				File.Create(path + "\\Config.json").Close();
				JObject config = new JObject(new JProperty("TemplatePath", ""), new JProperty("ReportNR", "1"), new JProperty("Active", ""), new JProperty("Username", ""), new JProperty("Password", ""),
					new JProperty("Name", ""), new JProperty("Font", "Arial"), new JProperty("EditorFontSize", 8.25f), new JProperty("LastReportWeekOfYear", 0), new JProperty("StayLoggedIn", false));
				File.WriteAllText(path + "\\Config.json", JsonConvert.SerializeObject(config, Formatting.Indented));
			}
			else 
			{
				JObject config;
				if (new FileInfo(path + "\\Config.json").Length == 0)
				{
					config = new JObject();
				}
				else 
				{
					config = JObject.Parse(File.ReadAllText(path + "\\Config.json"));					
				}
				if (!config.ContainsKey("TemplatePath")) 
				{
					OpenFileDialog dialog = new OpenFileDialog();
					dialog.Filter = "Word Templates (*.dotx)|*.dotx";
					dialog.ShowDialog();
					Save(Path.GetFullPath(dialog.FileName));
					MessageBox.Show("Muster auf: " + Path.GetFullPath(dialog.FileName) + " gesetzt");
				}
				if (!config.ContainsKey("ReportNR")) 
				{
					EditForm form = new EditForm("Edit Number of Report", "", false);
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK)
					{
						EditNumber(form.Result);
					}
				}
				if (!config.ContainsKey("Active")) 
				{
					config.Add(new JProperty("Active", ""));
				}
				if (!config.ContainsKey("Username")) 
				{
					config.Add(new JProperty("Username", ""));
				}
				if (!config.ContainsKey("Password")) 
				{
					config.Add(new JProperty("Password", ""));
				}
				if (!config.ContainsKey("Name")) 
				{
					EditForm form = new EditForm("Enter your name", "Name Vorname", false);
					if (form.ShowDialog() == DialogResult.OK)
					{
						config.Add(new JProperty("Name", form.Result));
					}
					else 
					{
						config.Add(new JProperty("Name", ""));
					}
				}
				if (!config.ContainsKey("Font")) 
				{
					config.Add(new JProperty("Font", "Arial"));
				}
				if (!config.ContainsKey("EditorFontSize")) 
				{
					config.Add(new JProperty("EditorFontSize", 8.25f));
				}
				if (!config.ContainsKey("LastReportWeekOfYear")) 
				{
					config.Add(new JProperty("LastReportWeekOfYear", 0));
				}
				if (!config.ContainsKey("StayLoggedIn")) 
				{
					config.Add(new JProperty("StayLoggedIn", false));	
				}
				File.WriteAllText(path + "\\Config.json", JsonConvert.SerializeObject(config, Formatting.Indented));
			}
		}

		/**
		<summary>
		Loads the path to the template
		</summary> 
		*/
		public string LoadPath() 
		{
			return LoadGeneric<string>("TemplatePath");
		}

		/**
		<summary>
		Loads the number of the next
		</summary> 
		*/
		public string LoadNumber() 
		{
			return LoadGeneric<string>("ReportNR");
		}

		/**
		<summary>
		Sets the template path
		</summary> 
		*/
		public void Save(string templateFilePath) 
		{
			SaveGeneric<string>("TemplatePath", templateFilePath);
		}

		/**
		<summary>
		Sets the number of the next report
		</summary> 
		*/
		public void EditNumber(string number) 
		{
			SaveGeneric<string>("ReportNR", number);
		}

		/**
		<summary>
		Sets the last active document path
		</summary> 
		*/
		public void EditActive(string activeDocument) 
		{
			SaveGeneric<string>("Active", activeDocument);
		}

		/**
		<summary>
		Loads the last active document path
		</summary> 
		*/
		public string LoadActive() 
		{
			return LoadGeneric<string>("Active");
		}

		/**
		<summary>
		Loads the username for webuntis
		</summary> 
		*/
		public string LoadUsername() 
		{
			return LoadGeneric<string>("Username");
		}

		/**
		<summary>
		Sets the username for webuntis
		</summary> 
		*/
		public void SaveUsername(string username) 
		{
			SaveGeneric<string>("Username", username);
		}

		/**
		<summary>
		Loads the password for Webuntis
		</summary> 
		*/
		public string LoadPassword() 
		{
			if (File.Exists(path + "\\Config.json"))
			{
				JObject config = JObject.Parse(File.ReadAllText(path + "\\Config.json"));
				return UserHandler.DecodePassword(config.GetValue("Password").ToString());
			}
			return "";
		}

		/**
		<summary>
		Sets the password for Webuntis
		</summary> 
		*/
		public void SavePassword(string password) 
		{
			SaveGeneric<string>("Password", UserHandler.EncodePassword(password));
		}

		public User doLogin() 
		{
			Login form = new Login();
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
				return new User(username: form.Username, password: form.Password);
			}
			return new User();
		}

		/**
		<summary>
		Loads the name of the person
		</summary> 
		*/
		public string LoadName() 
		{
			if (File.Exists(path + "\\Config.json")) 
			{
				JObject config = JObject.Parse(File.ReadAllText(path + "\\Config.json"));
				return config.GetValue("Name").ToString();
			}
			return "";
		}

		/**
		<summary>
		Sets the name of the person
		</summary> 
		*/
		public void SaveName(string name) 
		{
			SaveGeneric<string>("Name", name);
		}

		/**
		<summary>
		Loads the font
		</summary> 
		*/
		public string LoadFont() 
		{
			if (File.Exists(path + "\\Config.json")) 
			{
				return JObject.Parse(File.ReadAllText(path + "\\Config.json")).GetValue("Font").ToString();
			}
			return "Arial";
		}

		/**
		<summary>
		Sets the font
		</summary> 
		*/
		public void SaveFont(string fontName) 
		{
			SaveGeneric<string>("Font", fontName);
		}

		/**
		<summary>
		Loads the fontsize
		</summary> 
		*/
		public float LoadEditorFontSize() 
		{
			return LoadGeneric<float>("EditorFontSize");
		}

		/**
		<summary>
		Sets the fontsize
		</summary> 
		*/
		public void SaveEditorFontSize(float size) 
		{
			SaveGeneric<float>("EditorFontSize", size);
		}

		/**
		<summary>
		Loads the weeknumber of the last report that was created
		</summary> 
		*/
		public int LoadLastReportKW() 
		{
			return LoadGeneric<int>("LastReportWeekOfYear");
		}

		/**
		<summary>
		Sets the weeknumber of the last report that was created
		</summary> 
		*/
		public void SaveLastReportKW(int kw) 
		{
			SaveGeneric<int>("LastReportWeekOfYear", kw);
		}

		/**
		<summary>
		Gets the boolean if the User wanted to stay logged in
		</summary>
		*/
		public bool StayLoggedIn() 
		{
			return LoadGeneric<bool>("StayLoggedIn");
		}

		/**
		<summary>
		Sets if the user wanted to stay logged in
		</summary> 
		*/
		public void StayLoggedIn(bool stayLoggedIn) 
		{
			SaveGeneric<bool>("StayLoggedIn", stayLoggedIn);
		}

		/**
		<summary>
		Only implement
		Sets the specified key and value in the config
		</summary> 
		*/
		private void SaveGeneric<T>(string key, T toSave) 
		{
			if (ConfigExists()) 
			{
				JObject obj = new JObject(new JProperty(key, toSave));
				JObject config = JObject.Parse(File.ReadAllText(path + "\\Config.json"));

				JToken token = config.First;
				for (int i = 0; i < config.Count; i++)
				{
					if (((JProperty)token).Name != key)
					{
						obj.Add((JProperty)token);
					}
					token = token.Next;
				}

				File.WriteAllText(path + "\\Config.json", JsonConvert.SerializeObject(obj, Formatting.Indented));
			}
		}

		/**
		<summary>
		Only implement
		Loads the value for the specified key
		</summary> 
		*/
		private T LoadGeneric<T>(string key) 
		{
			if (ConfigExists()) 
			{
				JObject obj = JObject.Parse(File.ReadAllText(path + "\\Config.json"));
				if (obj.ContainsKey(key)) 
				{
					return obj.Value<T>(key);
				}
			}
			throw new DataNotFoundException();
		}

		public bool ConfigExists() 
		{
			return File.Exists(path + "\\Config.json");
		}

		private void SortConfig() 
		{
			List<JProperty> jProperties = new List<JProperty>();
			JObject jobject = JObject.Parse(File.ReadAllText(path + "\\Config.json"));
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
