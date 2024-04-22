using BerichtManager.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace BerichtManager.UploadChecking
{
	internal class UploadedReports : Dictionary<string, Dictionary<string, UploadedReport>>
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
		/// <param name="path">Path of report</param>
		/// <param name="report">Report object to save</param>
		/// <inheritdoc cref="Dictionary{TKey, TValue}.Add(TKey, TValue)" path="/exception"/>
		public void AddReport(string path, UploadedReport report)
		{
			if (!TryGetValue(ConfigHandler.Instance.ReportPath(), out Dictionary<string, UploadedReport> paths))
			{
				paths = new Dictionary<string, UploadedReport>();
				Add(ConfigHandler.Instance.ReportPath(), paths);
			}
			paths.Add(path, report);
			Save();
		}

		/// <summary>
		/// Loads the uploaded repor dictionary from file
		/// </summary>
		private void Load()
		{
			if (!File.Exists(FullPath))
				return;
			Dictionary<string, Dictionary<string, UploadedReport>> reports = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, UploadedReport>>>(File.ReadAllText(FullPath));
			foreach (KeyValuePair<string, Dictionary<string, UploadedReport>> kvp in reports)
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
