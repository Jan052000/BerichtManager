using BerichtManager.Config;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace BerichtManager.UploadChecking
{
	internal class UploadedReports : Dictionary<string, Dictionary<string, UploadedReport>>
	{
		/// <summary>
		/// Path to directory containing config file
		/// </summary>
		private static string ConfigFolderPath { get; } = Path.Combine(Environment.CurrentDirectory, "Config");
		/// <summary>
		/// Name of config file
		/// </summary>
		private static string FileName { get; } = "UploadedReports.json";
		/// <summary>
		/// Full path of config file
		/// </summary>
		private string FullPath { get; } = Path.Combine(ConfigFolderPath, FileName);

		#region Singleton
		protected static UploadedReports? Singleton { get; set; }
		/// <summary>
		/// Instance of <see cref="UploadedReports"/>
		/// </summary>
		protected static UploadedReports Instance
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
		/// Extracts the relative part of a report path anchored to report path of <see cref="ConfigHandler.ReportPath"/>
		/// </summary>
		/// <param name="path">Path to extrace relative path from</param>
		/// <returns>Relative path including base directory or <see langword="null"/> if <paramref name="path"/> is not in <see cref="ConfigHandler.ReportPath"/></returns>
		private static string? ExtractRelativePath(string? path)
		{
			if (string.IsNullOrEmpty(path))
				return null;
			if (path.StartsWith("/") || path.StartsWith("\\"))
				path = path.Substring(1);
			if (Path.IsPathRooted(path) && !path.StartsWith(ConfigHandler.Instance.ReportPath))
				return null;
			string toSplit = path.Replace('/', '\\');
			List<string> splitPath = toSplit.Split('\\').ToList();
			string reportRoot = ConfigHandler.Instance.ReportPath.Replace('/', '\\').Split('\\').Last();
			if (!splitPath.Contains(reportRoot))
				return null;
			splitPath.RemoveRange(0, splitPath.IndexOf(reportRoot));
			return String.Join('\\'.ToString(), splitPath);
		}

		/// <summary>
		/// Adds <paramref name="report"/> to the <see cref="UploadedReports"/>
		/// </summary>
		/// <param name="path">Path of report</param>
		/// <param name="report">Report object to save</param>
		/// <returns><see langword="true"/> if report was added and <see langword="false"/> otherwise</returns>
		/// <inheritdoc cref="Dictionary{TKey, TValue}.Add(TKey, TValue)" path="/exception"/>
		public static bool AddReport(string? path, UploadedReport report)
		{
			path = ExtractRelativePath(path);
			if (path == null)
				return false;
			if (!Instance.TryGetValue(ConfigHandler.Instance.ReportPath, out Dictionary<string, UploadedReport>? paths))
			{
				paths = new Dictionary<string, UploadedReport>();
				Instance.Add(ConfigHandler.Instance.ReportPath, paths);
			}
			if (paths.ContainsKey(path))
				paths[path] = report;
			else
				paths.Add(path, report);
			Instance.Save();
			return true;
		}

		/// <summary>
		/// Updates the upload status of the rpeort starting on <paramref name="startDate"/> and will update lfdnr if <paramref name="lfdnr"/> is set and <see cref="UploadedReport.LfdNR"/> is not set
		/// </summary>
		/// <param name="startDate"><see cref="DateTime"/> of rpeort start date</param>
		/// <param name="status"><see cref="ReportNode.UploadStatuses"/> to update to</param>
		/// <param name="lfdnr">Identification number of report on IHK servers</param>
		/// <returns><see langword="true"/> if a report was updated and <see langword="false"/> otherwise</returns>
		public static bool UpdateReportStatus(DateTime startDate, ReportNode.UploadStatuses status, int? lfdnr)
		{
			bool save = false;
			if (!Instance.TryGetValue(ConfigHandler.Instance.ReportPath, out Dictionary<string, UploadedReport>? paths))
				return save;
			UploadedReport? toUpdate = paths.Values.ToList().Find(report => report.StartDate == startDate);
			if (toUpdate == null)
				return false;
			if (toUpdate.Status != status)
			{
				toUpdate.Status = status;
				save = true;
			}
			if (!toUpdate.LfdNR.HasValue && lfdnr.HasValue && toUpdate.LfdNR != lfdnr)
			{
				toUpdate.LfdNR = lfdnr;
				save = true;
			}
			if (save)
				Instance.Save();
			return save;
		}

		/// <summary>
		/// Updates an <see cref="UploadedReport"/> at <paramref name="path"/>
		/// </summary>
		/// <param name="path">Path of report</param>
		/// <param name="status">New upload status</param>
		/// <param name="lfdnr">New lfdnr</param>
		/// <param name="wasEdited">New edited status</param>
		/// <param name="wasUpdated">New was updated status</param>
		public static void UpdateReport(string? path, ReportNode.UploadStatuses? status = null, int? lfdnr = null, bool? wasEdited = null, bool? wasUpdated = null)
		{
			if (!GetUploadedReport(path, out UploadedReport? report))
				return;
			bool save = false;
			if (status.HasValue && report.Status != status)
			{
				save = true;
				report.Status = status.Value;
			}
			if (lfdnr.HasValue && report.LfdNR != lfdnr)
			{
				save = true;
				report.LfdNR = lfdnr.Value;
			}
			if (wasEdited.HasValue && report.WasEditedLocally != wasEdited)
			{
				save = true;
				report.WasEditedLocally = wasEdited.Value;
			}
			if (wasUpdated.HasValue && report.WasUpdated != wasUpdated)
			{
				save = true;
				report.WasUpdated = wasUpdated.Value;
			}

			if (save)
				Instance.Save();
		}

		/// <summary>
		/// Gets status of uploaded report if it was uploaded
		/// </summary>
		/// <param name="path">Path of report</param>
		/// <param name="status">Upload status of report at <paramref name="path"/></param>
		/// <returns><see langword="true"/> if report is uploaded and <see langword="false"/> otherwise</returns>
		public static bool GetUploadStatus(string? path, out ReportNode.UploadStatuses status)
		{
			status = ReportNode.UploadStatuses.None;
			if (path == null)
				return false;
			if (!GetUploadedReport(path, out UploadedReport? result))
				return false;
			status = result.Status;
			return true;
		}

		/// <summary>
		/// Searches for a report with path <paramref name="path"/> under active path in <see cref="ConfigHandler"/>
		/// </summary>
		/// <param name="path">Path of report</param>
		/// <param name="report">Found <see cref="UploadedReport"/> or <see langword="null"/> if it was not found</param>
		/// <returns><see langword="true"/> if a report was found and <see langword="false"/> otherwise</returns>
		public static bool GetUploadedReport(string? path, [NotNullWhen(true)] out UploadedReport? report)
		{
			report = null;
			path = ExtractRelativePath(path);
			if (path == null)
				return false;
			if (!Instance.TryGetValue(ConfigHandler.Instance.ReportPath, out Dictionary<string, UploadedReport>? paths))
				return false;
			if (!paths.TryGetValue(path, out UploadedReport? result))
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
		public static bool GetUploadedReport(DateTime startDate, [NotNullWhen(true)] out UploadedReport? report)
		{
			report = null;
			if (!Instance.TryGetValue(ConfigHandler.Instance.ReportPath, out Dictionary<string, UploadedReport>? paths))
				return false;
			UploadedReport? foundReport = paths.Values.ToList().Find(x => x.StartDate.Date == startDate.Date);
			if (foundReport is not UploadedReport result)
				return false;
			report = result;
			return true;
		}

		/// <summary>
		/// Searches for a report with lfdnr of <paramref name="lfdnr"/> under active path in <see cref="ConfigHandler"/>
		/// </summary>
		/// <param name="lfdnr">Identification number of report on IHK servers</param>
		/// <param name="report">Found <see cref="UploadedReport"/> or <see langword="null"/> if it was not found</param>
		/// <returns><see langword="true"/> if a report was found and <see langword="false"/> otherwise</returns>
		public static bool GetUploadedReport(int lfdnr, [NotNullWhen(true)] out UploadedReport? report)
		{
			report = null;
			if (!Instance.TryGetValue(ConfigHandler.Instance.ReportPath, out Dictionary<string, UploadedReport>? paths))
				return false;
			UploadedReport? foundReport = paths.Values.ToList().Find(r => r.LfdNR == lfdnr);
			if (foundReport is not UploadedReport result)
				return false;
			report = result;
			return true;
		}

		/// <inheritdoc cref="GetUploadedReport(int, out UploadedReport?)"/>
		public static bool GetUploadedReport(int? lfdnr, [NotNullWhen(true)] out UploadedReport? report)
		{
			report = null;
			if (lfdnr == null || !GetUploadedReport((int)lfdnr, out UploadedReport? foundReport))
				return false;
			report = foundReport;
			return true;
		}

		/// <summary>
		/// Moves a report from <paramref name="oldPath"/> to <paramref name="newPath"/>
		/// </summary>
		/// <returns><see langword="true"/> when report was moved and <see langword="false"/> otherwise</returns>
		/// <param name="oldPath">Old path relative to <see cref="ConfigHandler.Instance"/>s report path</param>
		/// <param name="newPath">New path relative to <see cref="ConfigHandler.Instance"/>s report path</param>
		public static bool MoveReport(string? oldPath, string? newPath)
		{
			oldPath = ExtractRelativePath(oldPath);
			newPath = ExtractRelativePath(newPath);
			if (oldPath == null || newPath == null)
				return false;
			if (!Instance.TryGetValue(ConfigHandler.Instance.ReportPath, out Dictionary<string, UploadedReport>? reports))
				return false;
			if (!reports.TryGetValue(oldPath, out UploadedReport? toMove))
				return false;
			reports.Remove(oldPath);
			reports.Add(newPath, toMove);
			Instance.Save();
			return true;
		}

		/// <summary>
		/// Sets synchronization status of local report with IHK
		/// </summary>
		/// <param name="path">Path relative to <see cref="ConfigHandler.ReportPath()"/></param>
		/// <param name="wasEdited">Status of synchronization with IHK</param>
		public static void SetEdited(string path, bool wasEdited)
		{
			if (!GetUploadedReport(path, out UploadedReport? toMark))
				return;
			toMark.WasEditedLocally = wasEdited;
			Instance.Save();
		}

		/// <summary>
		/// Sets synchronization status of local report with IHK
		/// </summary>
		/// <param name="startDate"><see cref="DateTime"/> of report start date</param>
		/// <param name="wasEdited">Status of synchronization with IHK</param>
		public static void SetEdited(DateTime startDate, bool wasEdited)
		{
			if (!Instance.TryGetValue(ConfigHandler.Instance.ReportPath, out Dictionary<string, UploadedReport>? reports))
				return;
			UploadedReport? foundReport = reports.Values.ToList().Find(x => x.StartDate.Date == startDate.Date);
			if (foundReport is not UploadedReport toMark)
				return;
			toMark.WasEditedLocally = wasEdited;
			Instance.Save();
		}

		/// <summary>
		/// Gets a <see cref="List{T}"/> relative paths to uploaded reports
		/// </summary>
		/// <param name="paths"><see cref="List{T}"/> of relative paths to uploaded reports</param>
		/// <returns><see langword="true"/> if reports were uploaded in <see cref="ConfigHandler.ReportPath"/> and <see langword="false"/> otherwise</returns>
		public static bool GetUploadedPaths([NotNull] out List<string> paths)
		{
			paths = new List<string>();
			if (!Instance.TryGetValue(ConfigHandler.Instance.ReportPath, out Dictionary<string, UploadedReport>? uploadedPaths))
				return false;
			paths = uploadedPaths.Keys.ToList();
			return true;
		}

		/// <summary>
		/// Sets flag wether or not report was updated on IHK site
		/// </summary>
		/// <param name="startDate">Start date of report</param>
		/// <param name="wasUpdated">New wasUpdated status</param>
		public static void SetWasUpdated(DateTime startDate, bool wasUpdated)
		{
			if (!GetUploadedReport(startDate, out UploadedReport? toMark))
				return;
			SetWasUpdated(toMark, wasUpdated);
		}

		/// <inheritdoc cref="SetWasUpdated(DateTime, bool)" path="/summary"/>
		/// <param name="path">Path to the report to set wasUpdated status of</param>
		/// <inheritdoc cref="SetWasUpdated(DateTime, bool)" path="/param"/>
		public static void SetWasUpdated(string path, bool wasUpdated)
		{
			if (!GetUploadedReport(path, out UploadedReport? toMark))
				return;
			SetWasUpdated(toMark, wasUpdated);
		}

		/// <inheritdoc cref="SetWasUpdated(DateTime, bool)" path="/summary"/>
		/// <param name="lfdnr">Lfdnr of report on IHK servers</param>
		/// <inheritdoc cref="SetWasUpdated(DateTime, bool)" path="/param"/>
		public static void SetWasUpdated(int? lfdnr, bool wasUpdated)
		{
			if (lfdnr is not int _lfdnr)
				return;
			if (!GetUploadedReport(_lfdnr, out UploadedReport? toMark))
				return;
			SetWasUpdated(toMark, wasUpdated);
		}

		/// <summary>
		/// Sets wasUpdated of <paramref name="report"/> to <paramref name="wasUpdated"/> and saves changes
		/// </summary>
		/// <param name="report"><see cref="UploadedReport"/> to update</param>
		/// <param name="wasUpdated">New WasUpdated status</param>
		private static void SetWasUpdated(UploadedReport report, bool wasUpdated)
		{
			report.WasUpdated = wasUpdated;
			Instance.Save();
		}

		/// <summary>
		/// Gets <see cref="UploadedReport.WasUpdated"/> of <paramref name="toGet"/>
		/// </summary>
		/// <param name="toGet"><see cref="UploadedReport"/> to get updated status from</param>
		/// <param name="wasUpdated"><see cref="UploadedReport.WasUpdated"/> status of <paramref name="toGet"/></param>
		/// <returns><see langword="true"/> if wasUpdated was found and <see langword="false"/> otherwise</returns>
		private static bool GetWasUpdated(UploadedReport toGet, [NotNullWhen(true)] out bool? wasUpdated)
		{
			wasUpdated = null;
			if (toGet == null)
				return false;
			wasUpdated = toGet.WasUpdated;
			return true;
		}

		/// <summary>
		/// Gets <see cref="UploadedReport.WasUpdated"/> of report with <paramref name="startDate"/>
		/// </summary>
		/// <param name="startDate"><see cref="DateTime"/> start date of report</param>
		/// <param name="wasUpdated"><see cref="UploadedReport.WasUpdated"/> status of report or <see langword="null"/> if no report was found</param>
		/// <returns><see langword="true"/> if get was successful and <see langword="false"/> otherwise</returns>
		public static bool GetWasUpdated(DateTime startDate, [NotNullWhen(true)] out bool? wasUpdated)
		{
			wasUpdated = null;
			if (!GetUploadedReport(startDate, out UploadedReport? toGet))
				return false;
			return GetWasUpdated(toGet, out wasUpdated);
		}

		/// <summary>
		/// Gets <see cref="UploadedReport.WasUpdated"/> of report at <paramref name="path"/>
		/// </summary>
		/// <param name="path">Path of report</param>
		/// <inheritdoc cref="GetWasUpdated(DateTime, out bool?)" path="/param"/>
		/// <inheritdoc cref="GetWasUpdated(DateTime, out bool?)" path="/returns"/>
		public static bool GetWasUpdated(string path, [NotNullWhen(true)] out bool? wasUpdated)
		{
			wasUpdated = null;
			if (!GetUploadedReport(path, out UploadedReport? toGet))
				return false;
			return GetWasUpdated(toGet, out wasUpdated);
		}

		/// <summary>
		/// Gets <see cref="UploadedReport.WasUpdated"/> of report with <paramref name="lfdnr"/>
		/// </summary>
		/// <param name="lfdnr">Lfdnr of report on IHK servers</param>
		/// <inheritdoc cref="GetWasUpdated(DateTime, out bool?)" path="/param"/>
		/// <inheritdoc cref="GetWasUpdated(DateTime, out bool?)" path="/returns"/>
		public static bool GetWasUpdated(int? lfdnr, [NotNullWhen(true)] out bool? wasUpdated)
		{
			wasUpdated = null;
			if (lfdnr is not int _lfdnr)
				return false;
			if (!GetUploadedReport(_lfdnr, out UploadedReport? toGet))
				return false;
			return GetWasUpdated(toGet, out wasUpdated);
		}

		/// <summary>
		/// Determines wether or not a report can be edited
		/// </summary>
		/// <param name="path">File path of report</param>
		/// <returns><see langword="true"/> if report can be edited and <see langword="false"/> if not</returns>
		public static bool CanBeEdited(string path)
		{
			if (!GetUploadStatus(path, out ReportNode.UploadStatuses status))
				return true;
			return CanBeEdited(status);
		}

		/// <inheritdoc cref="CanBeEdited(string)" path="/summary"/>
		/// <inheritdoc cref="CanBeEdited(string)" path="/returns"/>
		/// <param name="report">Report to check</param>
		public static bool CanBeEdited(UploadedReport report)
		{
			return CanBeEdited(report.Status);
		}

		/// <summary>
		/// Determines if a report with <paramref name="status"/> can be edited
		/// </summary>
		/// <param name="status">Upload status of report</param>
		/// <inheritdoc cref="CanBeEdited(string)" path="/returns"/>
		public static bool CanBeEdited(ReportNode.UploadStatuses status)
		{
			return status != ReportNode.UploadStatuses.HandedIn && status != ReportNode.UploadStatuses.Accepted;
		}

		/// <summary>
		/// Sets the last cached comment for a report
		/// </summary>
		/// <param name="comment">Comment to cache</param>
		/// <inheritdoc cref="GetUploadedReport(int?, out UploadedReport?)" path="/param"/>
		public static void SetLastComment(int? lfdnr, string? comment)
		{
			if (lfdnr == null || !GetUploadedReport(lfdnr, out UploadedReport? toEdit))
				return;
			toEdit.LastComment = comment;
			Instance.Save();
		}

		/// <summary>
		/// <inheritdoc cref="SetLastComment(int?, string?)" path="/summary"/>
		/// </summary>
		/// <inheritdoc cref="GetUploadedReport(DateTime, out UploadedReport?)" path="/param"/>
		/// <inheritdoc cref="SetLastComment(int?, string?)" path="/param"/>
		public static void SetLastComment(DateTime startDate, string? comment)
		{
			if (!GetUploadedReport(startDate, out UploadedReport? toEdit))
				return;
			toEdit.LastComment = comment;
			Instance.Save();
		}

		/// <summary>
		/// <inheritdoc cref="SetLastComment(int?, string?)" path="/summary"/>
		/// </summary>
		/// <inheritdoc cref="GetUploadedReport(string?, out UploadedReport?)" path="/param"/>
		/// <inheritdoc cref="SetLastComment(int?, string?)" path="/param"/>
		public static void SetLastComment(string? path, string? comment)
		{
			if (!GetUploadedReport(path, out UploadedReport? toEdit))
				return;
			toEdit.LastComment = comment;
			Instance.Save();
		}

		/// <summary>
		/// Gets the last cached comment if set
		/// </summary>
		/// <inheritdoc cref="GetUploadedReport(int?, out UploadedReport?)" path="/param"/>
		/// <returns>Comment if cached or <see langword="null"/> otherwise</returns>
		public static string? GetLastComment(int? lfdnr)
		{
			if (lfdnr == null || !GetUploadedReport(lfdnr, out UploadedReport? report))
				return null;
			return report.LastComment;
		}

		/// <summary>
		/// <inheritdoc cref="GetLastComment(int?)" path="/summary"/>
		/// </summary>
		/// <inheritdoc cref="GetUploadedReport(DateTime, out UploadedReport?)" path="/param"/>
		/// <returns><inheritdoc cref="GetLastComment(int?)" path="/returns"/></returns>
		public static string? GetLastComment(DateTime startDate)
		{
			if (!GetUploadedReport(startDate, out UploadedReport? report))
				return null;
			return report.LastComment;
		}

		/// <summary>
		/// <inheritdoc cref="GetLastComment(int?)" path="/summary"/>
		/// </summary>
		/// <inheritdoc cref="GetUploadedReport(string?, out UploadedReport?)" path="/param"/>
		/// <returns><inheritdoc cref="GetLastComment(int?)" path="/returns"/></returns>
		public static string? GetLastComment(string? path)
		{
			if (!GetUploadedReport(path, out UploadedReport? report))
				return null;
			return report.LastComment;
		}

		/// <summary>
		/// Loads the uploaded repor dictionary from file
		/// </summary>
		private void Load()
		{
			if (!File.Exists(FullPath))
				return;
			Dictionary<string, Dictionary<string, UploadedReport>>? reports = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, UploadedReport>>>(File.ReadAllText(FullPath));
			if (reports == null)
				return;
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
			File.WriteAllText(FullPath, JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings() { DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate }));
		}
	}
}
