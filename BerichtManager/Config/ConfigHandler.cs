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
		private string path = Environment.CurrentDirectory + "\\Config";
		public bool loginAborted = false;
		public ConfigHandler() 
		{
			if (!File.Exists(path + "\\Config.json")) 
			{
				Directory.CreateDirectory(path);
				File.Create(path + "\\Config.json").Close();
				JObject config = new JObject(new JProperty("TemplatePath", ""), new JProperty("ReportNR", "0"), new JProperty("Active", ""), new JProperty("Username", ""), new JProperty("Password", ""),
					new JProperty("Name", ""), new JProperty("Font", "Arial"), new JProperty("EditorFontSize", 8.25));
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
					//config.Add(new JProperty("TemplatePath", ""));
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
				File.WriteAllText(path + "\\Config.json", JsonConvert.SerializeObject(config, Formatting.Indented));
			}
			//Directory.CreateDirectory(path);
		}

		public string LoadPath() 
		{
			if (File.Exists(path + "\\Config.json"))
			{
				JObject config = JObject.Parse(File.ReadAllText(path + "\\Config.json"));
				return config.GetValue("TemplatePath").ToString();
			}
			else
			{
				File.Create(path + "\\Config.json").Close();
				MessageBox.Show("Config bei: " + Path.GetFullPath(path + "\\Config.json") + " erstellt");
			}

			return "";
		}

		public string LoadNumber() 
		{
			if (File.Exists(path + "\\Config.json"))
			{
				JObject config = JObject.Parse(File.ReadAllText(path + "\\Config.json"));
				return config.GetValue("ReportNR").ToString();
			}
			else
			{
				File.Create(path + "\\Config.json").Close();
				MessageBox.Show("Config bei: " + Path.GetFullPath(path + "\\Config.json") + " erstellt");
			}
			return "-1";
		}

		public void Save(string templateFilePath) 
		{
			if (File.Exists(path + "\\Config.json"))
			{
				JObject template = new JObject(new JProperty("TemplatePath", templateFilePath));
				JObject config = JObject.Parse(File.ReadAllText(path + "\\Config.json"));

				JToken token = config.First;
				for (int i = 0; i < config.Count; i++) 
				{
					if (((JProperty)token).Name != "TemplatePath")
					{
						template.Add((JProperty)token);
					}
					token = token.Next;
				}

				File.WriteAllText(path + "\\Config.json", JsonConvert.SerializeObject(template, Formatting.Indented));
			}
			else 
			{
				File.Create(path).Close();
				MessageBox.Show("Config bei: " + Path.GetFullPath(path) + " erstellt");
			}
		}

		public void EditNumber(string number) 
		{
			if (File.Exists(path + "\\Config.json"))
			{
				JObject template = new JObject(new JProperty("ReportNR", number));
				JObject config = JObject.Parse(File.ReadAllText(path + "\\Config.json"));

				JToken token = config.First;
				for (int i = 0; i < config.Count; i++)
				{
					if (((JProperty)token).Name != "ReportNR")
					{
						template.Add((JProperty)token);
					}
					token = token.Next;
				}

				File.WriteAllText(path + "\\Config.json", JsonConvert.SerializeObject(template, Formatting.Indented));
			}
			else
			{
				File.Create(path).Close();
				MessageBox.Show("Config bei: " + Path.GetFullPath(path) + " erstellt");
			}
		}

		public void EditActive(string activeDocument) 
		{
			if (File.Exists(path + "\\Config.json"))
			{
				JObject template = new JObject(new JProperty("Active", activeDocument));
				JObject config = JObject.Parse(File.ReadAllText(path + "\\Config.json"));

				JToken token = config.First;
				for (int i = 0; i < config.Count; i++)
				{
					if (((JProperty)token).Name != "Active")
					{
						template.Add((JProperty)token);
					}
					token = token.Next;
				}

				File.WriteAllText(path + "\\Config.json", JsonConvert.SerializeObject(template, Formatting.Indented));
			}
			else
			{
				File.Create(path).Close();
				MessageBox.Show("Config bei: " + Path.GetFullPath(path) + " erstellt");
			}
		}

		public string LoadActive() 
		{
			if (File.Exists(path + "\\Config.json"))
			{
				JObject config = JObject.Parse(File.ReadAllText(path + "\\Config.json"));
				return config.GetValue("Active").ToString();
			}
			else
			{
				File.Create(path + "\\Config.json").Close();
				MessageBox.Show("Config bei: " + Path.GetFullPath(path + "\\Config.json") + " erstellt");
			}
			return "";
		}

		public string LoadUsername() 
		{
			if (File.Exists(path + "\\Config.json"))
			{
				JObject config = JObject.Parse(File.ReadAllText(path + "\\Config.json"));
				return config.GetValue("Username").ToString();
			}
			return "";
		}

		public void SaveUsername(string username) 
		{
			if (File.Exists(path + "\\Config.json")) 
			{
				JObject userObject = new JObject(new JProperty("Username", username));
				JObject config = JObject.Parse(File.ReadAllText(path + "\\Config.json"));

				JToken token = config.First;
				for (int i = 0; i < config.Count; i++)
				{
					if (((JProperty)token).Name != "Username")
					{
						userObject.Add((JProperty)token);
					}
					token = token.Next;
				}

				File.WriteAllText(path + "\\Config.json", JsonConvert.SerializeObject(userObject, Formatting.Indented));
			}
		}

		public string LoadPassword() 
		{
			if (File.Exists(path + "\\Config.json"))
			{
				JObject config = JObject.Parse(File.ReadAllText(path + "\\Config.json"));
				return UserHandler.DecodePassword(config.GetValue("Password").ToString());
			}
			return "";
		}

		public void SavePassword(string password) 
		{
			if (File.Exists(path + "\\Config.json"))
			{
				JObject userObject = new JObject(new JProperty("Password", UserHandler.EncodePassword(password)));
				JObject config = JObject.Parse(File.ReadAllText(path + "\\Config.json"));

				JToken token = config.First;
				for (int i = 0; i < config.Count; i++)
				{
					if (((JProperty)token).Name != "Password")
					{
						userObject.Add((JProperty)token);
					}
					token = token.Next;
				}

				File.WriteAllText(path + "\\Config.json", JsonConvert.SerializeObject(userObject, Formatting.Indented));
			}
		}

		public bool doLogin() 
		{
			Login form = new Login();
			form.ShowDialog();
			if (form.DialogResult == DialogResult.OK)
			{
				if (File.Exists(path + "\\Config.json"))
				{
					if (!string.IsNullOrEmpty(form.Username) && !string.IsNullOrEmpty(form.Password))
					{
						SaveUsername(form.Username);
						SavePassword(form.Password);
						return true;
					}
				}
			}
			return false;
		}

		public string LoadName() 
		{
			if (File.Exists(path + "\\Config.json")) 
			{
				JObject config = JObject.Parse(File.ReadAllText(path + "\\Config.json"));
				return config.GetValue("Name").ToString();
			}
			return "";
		}

		public void SaveName(string name) 
		{
			if (File.Exists(path + "\\Config.json")) 
			{
				JObject nameObject = new JObject(new JProperty("Name", name));
				JObject config = JObject.Parse(File.ReadAllText(path + "\\Config.json"));

				JToken token = config.First;
				for (int i = 0; i < config.Count; i++)
				{
					if (((JProperty)token).Name != "Password")
					{
						nameObject.Add((JProperty)token);
					}
					token = token.Next;
				}

				File.WriteAllText(path + "\\Config.json", JsonConvert.SerializeObject(nameObject, Formatting.Indented));
			}
		}

		public string LoadFont() 
		{
			if (File.Exists(path + "\\Config.json")) 
			{
				return JObject.Parse(File.ReadAllText(path + "\\Config.json")).GetValue("Font").ToString();
			}
			return "Arial";
		}

		public void SaveFont(string fontName) 
		{
			if (File.Exists(path + "\\Config.json")) 
			{
				JObject fontObject = new JObject(new JProperty("Font", fontName));
				JObject config = JObject.Parse(File.ReadAllText(path + "\\Config.json"));

				JToken token = config.First;
				for (int i = 0; i < config.Count; i++)
				{
					if (((JProperty)token).Name != "Font")
					{
						fontObject.Add((JProperty)token);
					}
					token = token.Next;
				}

				File.WriteAllText(path + "\\Config.json", JsonConvert.SerializeObject(fontObject, Formatting.Indented));
			}
		}

		public float LoadEditorFontSize() 
		{
			if (ConfigExists()) 
			{
				if (float.TryParse(JObject.Parse(File.ReadAllText(path + "\\Config.json")).GetValue("EditorFontSize").ToString(), out float size))
				{
					return size;
				}
			}
			return 8.25f;
		}

		public void SaveEditorFontSize(float size) 
		{
			if (ConfigExists()) 
			{
				JObject sizeObject = new JObject(new JProperty("EditorFontSize", size));
				JObject config = JObject.Parse(File.ReadAllText(path + "\\Config.json"));

				JToken token = config.First;
				for (int i = 0; i < config.Count; i++)
				{
					if (((JProperty)token).Name != "EditorFontSize")
					{
						sizeObject.Add((JProperty)token);
					}
					token = token.Next;
				}

				File.WriteAllText(path + "\\Config.json", JsonConvert.SerializeObject(sizeObject, Formatting.Indented));
			}
		}

		public void SaveGeneric<T>(T toSave, string key) 
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

		public T LoadGeneric<T>(string key) 
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
			if (File.Exists(path + "\\Config.json"))
			{
				return true;
			}
			else 
			{
				return false;
			}
		}

		private void SortConfig() 
		{
			List<JProperty> jProperties = new List<JProperty>();
			JObject jobject = JObject.Parse(File.ReadAllText(path + "\\Config.json"));
			var test = jobject.Children<JProperty>().OrderBy(p => p.Name);
		}
	}

	public class DataNotFoundException : Exception 
	{
		public DataNotFoundException() : base("Key and / or Value not found")
		{
		}
	}
}
