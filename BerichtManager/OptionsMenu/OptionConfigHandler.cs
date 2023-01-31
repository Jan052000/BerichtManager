using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace BerichtManager.OptionsMenu
{
	/// <summary>
	/// Class for getting and setting user options in a file .\Config\UserOptions.json
	/// </summary>
	internal class OptionConfigHandler
	{
		/// <summary>
		/// Location of the config folder
		/// </summary>
		private readonly string path = Environment.CurrentDirectory + "\\Config";
		/// <summary>
		/// Name of the config
		/// </summary>
		private readonly string configName = "UserOptions.json";
		/// <summary>
		/// Object representing the config content
		/// </summary>
		private readonly JObject configObject;

		public OptionConfigHandler()
		{
			bool isComplete = true;
			if (!ConfigExists())
			{
				configObject = new JObject(new JProperty("UseCustomPrefix", false), new JProperty("CustomPrefix", "-"), new JProperty("WebUntisServer", "borys"), new JProperty("SchoolName", "pictorus-bk"),
					new JProperty("UseWebUntis", true), new JProperty("EndWeekOnFriday", false));
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
				if (!configObject.ContainsKey("EndWeekOnFriday"))
				{
					configObject.Add(new JProperty("EndWeekOnFriday", false));
					isComplete = false;
				}
			}
			if (!isComplete)
			{
				File.WriteAllText(path + "\\" + configName, JsonConvert.SerializeObject(configObject, Formatting.Indented));
			}
		}

		/// <summary>
		/// Checks if the config file exists
		/// </summary>
		/// <returns>If the config file exists</returns>
		private bool ConfigExists()
		{
			return File.Exists(Path.GetFullPath(path + "\\" + configName));
		}

		/// <summary>
		/// Saves the configObject to file
		/// </summary>
		public void SaveConfig()
		{
			File.WriteAllText(path + "\\" + configName, JsonConvert.SerializeObject(configObject, Formatting.Indented));
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
	}
}
