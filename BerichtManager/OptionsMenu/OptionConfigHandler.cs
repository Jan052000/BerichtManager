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
			if (!ConfigExists())
			{
				configObject = new JObject(new JProperty("UseCustomPrefix", false), new JProperty("CustomPrefix", "-"));
				File.WriteAllText(path + "\\" + configName, JsonConvert.SerializeObject(configObject, Formatting.Indented));
			}
			else 
			{
				configObject = JObject.Parse(File.ReadAllText(path + "\\" + configName));
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
	}
}
