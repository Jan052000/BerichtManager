using BerichtManager.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
			if (paths.ContainsKey(path))
				paths[path] = report;
			else
				paths.Add(path, report);
			Save();
		}

		/// <summary>
		/// Updates the upload status of the rpeort starting on <paramref name="startDate"/>
		/// </summary>
		/// <param name="startDate"><see cref="DateTime"/> of rpeort start date</param>
		/// <param name="status"><see cref="ReportNode.UploadStatuses"/> to update to</param>
		/// <param name="lfdnr">Identification number of report on IHK servers</param>
		/// <returns><see langword="true"/> if a report was updated and <see langword="false"/> otherwise</returns>
		public static bool UpdateReportStatus(DateTime startDate, ReportNode.UploadStatuses status, int? lfdnr)
		{
			bool save = false;
			if (!Instance.TryGetValue(ConfigHandler.Instance.ReportPath(), out Dictionary<string, UploadedReport> paths))
				return save;
			List<UploadedReport> uploadedReports = paths.Where(kvp => kvp.Value.StartDate == startDate && (!kvp.Value.LfdNR.HasValue || kvp.Value.LfdNR == lfdnr)).ToList().Select(x => x.Value).ToList();
			uploadedReports.ForEach(report =>
			{
				save |= report.Status != status;
				report.Status = status;
				if (!report.LfdNR.HasValue && lfdnr.HasValue)
				{
					report.LfdNR = lfdnr;
					save = true;
				}
			});
			if (save)
				Instance.Save();
			return save;
		}

		/// <summary>
		/// Searches for a report with path <paramref name="path"/> under active path in <see cref="ConfigHandler"/>
		/// </summary>
		/// <param name="path">Path of report</param>
		/// <param name="report">Found <see cref="UploadedReport"/> or <see langword="null"/> if it was not found</param>
		/// <returns><see langword="true"/> if a report was found and <see langword="false"/> otherwise</returns>
		public static bool GetUploadedReport(string path, out UploadedReport report)
		{
			report = null;
			if (!Instance.TryGetValue(ConfigHandler.Instance.ReportPath(), out Dictionary<string, UploadedReport> paths))
				return false;
			if (!paths.TryGetValue(path, out UploadedReport result))
				return false;
			report = result;
			return true;
		}

		/// <summary>
		/// Searches for a report with a start date of <paramref name="startDate"/> under active path in <see cref="ConfigHandler"/>
		/// </summary>
		/// <param name="startDate">Start date of report, only <see cref="DateTime.Date"/> components are compared</param>
		/// <param name="report">Found <see cref="UploadedReport"/> or <see langword="null"/> if it was not found</param>
		/// <returns><see langword="true"/> if a report was found and <see langword="false"/> otherwise</returns>
		public static bool GetUploadedReport(DateTime startDate, out UploadedReport report)
		{
			report = null;
			if (!Instance.TryGetValue(ConfigHandler.Instance.ReportPath(), out Dictionary<string, UploadedReport> paths))
				return false;
			UploadedReport foundReport = paths.Values.ToList().Find(x => x.StartDate.Date == startDate.Date);
			if (!(foundReport is UploadedReport result))
				return false;
			report = result;
			return true;
		}

		/// <summary>
		/// Moves a report from <paramref name="oldPath"/> to <paramref name="newPath"/>
		/// </summary>
		/// <param name="oldPath">Old path relative to <see cref="ConfigHandler.Instance"/>s report path</param>
		/// <param name="newPath">New path relative to <see cref="ConfigHandler.Instance"/>s report path</param>
		public static void MoveReport(string oldPath, string newPath)
		{
			if (!Instance.TryGetValue(ConfigHandler.Instance.ReportPath(), out Dictionary<string, UploadedReport> reports))
				return;
			if (!reports.TryGetValue(oldPath, out UploadedReport toMove))
				return;
			reports.Remove(oldPath);
			reports.Add(newPath, toMove);
			Instance.Save();
		}

		/// <summary>
		/// Sets synchronization status of local report with IHK
		/// </summary>
		/// <param name="path">Path relative to <see cref="ConfigHandler.ReportPath()"/></param>
		/// <param name="wasEdited">Status of synchronization with IHK</param>
		public static void SetEdited(string path, bool wasEdited)
		{
			if (!Instance.TryGetValue(ConfigHandler.Instance.ReportPath(), out Dictionary<string, UploadedReport> reports))
				return;
			if (!reports.TryGetValue(path, out UploadedReport toMark))
				return;
			toMark.WasEditedLocally = wasEdited;
			Instance.Save();
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
			File.WriteAllText(FullPath, JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings() { DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate}));
		}
	}
}
