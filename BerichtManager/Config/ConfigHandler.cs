using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using BerichtManager.AddForm;

namespace BerichtManager.Config
{
	public class ConfigHandler
	{
		private string path = Environment.CurrentDirectory + "\\Config";
		//private string path = Environment.CurrentDirectory + "\\Config\\Config.json";
		//private string path = ".\\..\\..\\Config\\Config.json";
		public ConfigHandler() 
		{
			if (!File.Exists(path + "\\Config.json")) 
			{
				Directory.CreateDirectory(path);
				File.Create(path + "\\Config.json").Close();
				JObject config = new JObject(new JProperty("TemplatePath", ""), new JProperty("ReportNR", "0"), new JProperty("Active", ""));
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
				File.WriteAllText(path + "\\Config.json", JsonConvert.SerializeObject(config, Formatting.Indented));
			}
			//Directory.CreateDirectory(path);
		}

		public string LoadPath() 
		{
			string template = "";
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

			return template;
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
	}
}
