using BerichtManager.Config;
using BerichtManager.HelperClasses;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace BerichtManager.ReportQuickInfo
{
	public class QuickInfos : Dictionary<string, Dictionary<string, QuickInfo>>
	{
		/// <summary>
		/// Path to directory containing config file
		/// </summary>
		private static string ConfigFolderPath { get; } = Environment.CurrentDirectory + "\\Config";
		/// <summary>
		/// Name of config file
		/// </summary>
		private static string FileName { get; } = "ReportQuickInfos.json";
		/// <summary>
		/// Full path of config file
		/// </summary>
		private static string FullPath { get; } = ConfigFolderPath + "\\" + FileName;

		#region Singleton
		private static QuickInfos? Singleton { get; set; }
		private static QuickInfos Instance
		{
			get
			{
				return Singleton ??= new QuickInfos();
			}
		}
		#endregion

		private QuickInfos()
		{
			Load();
		}

		/// <summary>
		/// Loads the uploaded report dictionary from file
		/// </summary>
		private void Load()
		{
			if (!File.Exists(FullPath))
				return;
			foreach (var kvp in JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, QuickInfo>>>(File.ReadAllText(FullPath)) ?? [])
			{
				Add(kvp.Key, kvp.Value);
			}
		}

		/// <summary>
		/// Saves contents to file
		/// </summary>
		private void Save()
		{
			File.WriteAllText(FullPath, JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings() { DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate }));
		}

		/// <summary>
		/// Gets a <see cref="QuickInfo"/> for the report at <paramref name="path"/> if possible
		/// </summary>
		/// <param name="path">Path of report, either relative or absolute</param>
		/// <param name="info">Found <see cref="QuickInfo"/> or <see langword="null"/> if not found</param>
		/// <returns><see langword="true"/> if a <see cref="QuickInfo"/> was found and <see langword="false"/> otherwise</returns>
		public static bool GetQuickInfo(string? path, [NotNullWhen(true)] out QuickInfo? info)
		{
			info = null;
			if (!Instance.TryGetValue(ConfigHandler.Instance.ReportPath, out var activeReportDir))
				return false;
			if (PathHelper.ExtractRelativeReportPath(path) is not string relPath || !activeReportDir.TryGetValue(relPath, out QuickInfo? finfo))
				return false;
			info = finfo;
			return true;
		}

		/// <summary>
		/// Adds or updates a <see cref="QuickInfo"/>
		/// </summary>
		/// <param name="path">Path of report the info belongs to, either relative or absolute</param>
		/// <param name="info">Report infos</param>
		/// <exception cref="ArgumentException">Thrown if <paramref name="path"/> is not relative to <see cref="ConfigHandler.ReportPath"/></exception>
		public static void AddOrUpdateQuickInfo(string path, QuickInfo info)
		{
			if (!Instance.TryGetValue(ConfigHandler.Instance.ReportPath, out var reportDir))
			{
				reportDir = new Dictionary<string, QuickInfo>();
				Instance.Add(ConfigHandler.Instance.ReportPath, reportDir);
			}
			string? relPath = PathHelper.ExtractRelativeReportPath(path);
			if (relPath == null)
				throw new ArgumentException($"Path {path} is not relative to active report directory {ConfigHandler.Instance.ReportPath}", nameof(path));
			if (!reportDir.TryAdd(relPath, info))
				reportDir[relPath] = info;
			Instance.Save();
		}

		/// <summary>
		/// Gets the count of reports that had their quick info collected
		/// </summary>
		/// <returns>Number of reports that had their quick info collected</returns>
		public static int CountInfosForCurrentDir()
		{
			if (!Instance.TryGetValue(ConfigHandler.Instance.ReportPath, out var reportDir))
				return 0;
			return reportDir.Count;
		}

		/// <summary>
		/// Gets data from <see cref="QuickInfos"/> and forms it to match <paramref name="predicate"/>
		/// </summary>
		/// <typeparam name="T"><see cref="Type"/> of return value</typeparam>
		/// <param name="predicate"><see cref="Func{T, TResult}"/> to transform each <see cref="QuickInfo"/></param>
		/// <returns><see cref="List{T}"/> containing values of <see cref="Type"/> <typeparamref name="T"/></returns>
		public static List<T> GetData<T>(Func<QuickInfo, T> predicate)
		{
			List<T> data = new List<T>();
			if (!Instance.TryGetValue(ConfigHandler.Instance.ReportPath, out var reportDir))
				return data;
			foreach (var kvp in reportDir)
			{
				data.Add(predicate(kvp.Value));
			}
			return data;
		}

		/// <summary>
		/// Gets all <see cref="QuickInfo"/>s
		/// </summary>
		/// <returns><see cref="List{T}"/> of all <see cref="QuickInfo"/>s</returns>
		public static List<QuickInfo> GetQuickInfos()
		{
			if (!Instance.TryGetValue(ConfigHandler.Instance.ReportPath, out var reportDir))
				return new List<QuickInfo>();
			return [.. reportDir.Values];
		}
	}
}
