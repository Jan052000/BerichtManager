using BerichtManager.Config;

namespace BerichtManager.HelperClasses
{
	public class PathHelper
	{
		/// <summary>
		/// Extracts the relative part of a report path anchored to report path of <see cref="ConfigHandler.ReportPath"/>
		/// </summary>
		/// <param name="path">Path to extrace relative path from</param>
		/// <returns>Relative path including base directory or <see langword="null"/> if <paramref name="path"/> is not in <see cref="ConfigHandler.ReportPath"/></returns>
		public static string? ExtractRelativeReportPath(string? path)
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
	}
}
