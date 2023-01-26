using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BerichtManager.OptionsMenu
{
	internal class OptionConfigHandler
	{
		private readonly string path = Environment.CurrentDirectory + "\\Config";
		private readonly string configName = "UserOptions.json";
		private readonly JObject configObject;

		public OptionConfigHandler()
		{
			bool isComplete = true;
			if (!ConfigExists())
			{
				configObject = new JObject(new JProperty("UseCustomPrefix", false), new JProperty("CustomPrefix", "-"), new JProperty("WebUntisServer", "borys"), new JProperty("SchoolName", "pictorus-bk"), new JProperty("UseWebUntis", true));
				File.WriteAllText(path + "\\" + configName, JsonConvert.SerializeObject(configObject, Formatting.Indented));
			}
			else 
			{
				configObject = JObject.Parse(File.ReadAllText(path + "\\" + configName));
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
			}
			if (!isComplete)
			{
				File.WriteAllText(path + "\\" + configName, JsonConvert.SerializeObject(configObject, Formatting.Indented));
			}
		}

		private bool ConfigExists()
		{
			return File.Exists(Path.GetFullPath(path + "\\" + configName));
		}

		public void SaveConfig()
		{
			File.WriteAllText(path + "\\" + configName, JsonConvert.SerializeObject(configObject, Formatting.Indented));
		}

		private T GenericGet<T>(string key)
		{
			return configObject.Value<T>(key);
		}

		private void GenericSet<T>(string key, T value)
		{
			configObject.Remove(key);
			configObject.Add(new JProperty(key, value));
		}

		public bool UseUserPrefix()
		{
			return GenericGet<bool>("UseCustomPrefix");
		}

		public void SetUseUserPrefix(bool useUserPrefix)
		{
			GenericSet<bool>("UseCustomPrefix", useUserPrefix);
		}

		public string GetCustomPrefix()
		{
			return GenericGet<string>("CustomPrefix");
		}

		public void SetCustomPrefix(string customPrefix)
		{
			GenericSet<string>("CustomPrefix", customPrefix);
		}

		public string GetWebUntisServer()
		{
			return GenericGet<string>("WebUntisServer");
		}

		public void SetWebUntisServer(string server)
		{
			GenericSet<string>("WebUntisServer", server);
		}

		public string GetSchoolName()
		{
			return GenericGet<string>("SchoolName");
		}

		public void SetSchoolName(string schoolName)
		{
			GenericSet<string>("SchoolName", schoolName);
		}

		public bool UseWebUntis()
		{
			return GenericGet<bool>("UseWebUntis");
		}

		public void SetUseWebUntis(bool useWebUntis)
		{
			GenericSet<bool>("UseWebUntis", useWebUntis);
		}
	}
}
