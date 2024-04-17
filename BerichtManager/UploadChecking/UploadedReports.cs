using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace BerichtManager.UploadChecking
{
	internal class UploadedReports : Dictionary<string, Dictionary<string, ReportNode.UploadStatuses>>
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
		public void AddReport(string key, string value, ReportNode.UploadStatuses status)
		{
			var s = this.Keys;
			if (!TryGetValue(key, out Dictionary<string, ReportNode.UploadStatuses> paths))
			{
				paths = new Dictionary<string, ReportNode.UploadStatuses>();
				Add(key, paths);
			}
			paths.Add(value, status);
			Save();
		}

		/// <summary>
		/// Loads the uploaded repor dictionary from file
		/// </summary>
		private void Load()
		{
			if (!File.Exists(FullPath))
				return;
			Dictionary<string, Dictionary<string, ReportNode.UploadStatuses>> reports = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, ReportNode.UploadStatuses>>>(File.ReadAllText(FullPath));
			foreach (KeyValuePair<string, Dictionary<string, ReportNode.UploadStatuses>> kvp in reports)
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
