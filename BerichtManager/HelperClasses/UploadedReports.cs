using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace BerichtManager.HelperClasses
{
	internal class UploadedReports : Dictionary<string, List<string>>
	{
		/// <summary>
		/// Path to directory containing config file
		/// </summary>
		private static string ConfigFolderPath { get; } = Environment.CurrentDirectory + "\\Config";
		/// <summary>
		/// Name of config file
		/// </summary>
		private static string FileName { get; } = "UploadedReports.json";
		/// <summary>
		/// Full path of config file
		/// </summary>
		private string FullPath { get; } = Path.Combine(ConfigFolderPath, FileName);

		#region Singleton
		private static UploadedReports Singleton { get; set; }
		/// <summary>
		/// Instance of <see cref="UploadedReports"/>
		/// </summary>
		public static UploadedReports Instance
		{
			get
			{
				if (Singleton == null)
					Singleton = new UploadedReports();
				return Singleton;
			}
		}
		#endregion

		private UploadedReports() : base()
		{
			Load();
		}

		/// <summary>
		/// Adds <paramref name="value"/> to the <see cref="List{T}"/> at <paramref name="key"/>
		/// </summary>
		/// <param name="key">Key of the list to add <paramref name="value"/> to</param>
		/// <param name="value">Value to add to <see cref="List{T}"/> at <paramref name="key"/></param>
		public void Add(string key, string value)
		{
			if (!TryGetValue(key, out List<string> paths))
				Add(key, new List<string> { value });
			paths.Add(value);
			Save();
		}

		/// <summary>
		/// Loads the uploaded repor dictionary from file
		/// </summary>
		private void Load()
		{
			if (!File.Exists(FullPath))
				return;
			Dictionary<string, List<string>> reports = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(File.ReadAllText(FullPath));
			foreach (KeyValuePair<string, List<string>> kvp in reports)
			{
				Add(kvp.Key, kvp.Value);
			}
		}

		/// <summary>
		/// Saves contents to file
		/// </summary>
		private void Save()
		{
			File.WriteAllText(FullPath, JsonConvert.SerializeObject(this, Formatting.Indented));
		}
	}
}
