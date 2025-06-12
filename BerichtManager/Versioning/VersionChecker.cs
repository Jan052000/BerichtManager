using BerichtManager.Config;
using BerichtManager.OwnControls;
using System.Diagnostics;

namespace BerichtManager.Versioning
{
	/// <summary>
	/// Class to compare version numbers
	/// </summary>
	internal class VersionChecker
	{
		/// <summary>
		/// Checks wether or not the current cersion is old
		/// </summary>
		/// <returns><see langword="true"/> when a newer version is available and <see langword="false"/> otherwise</returns>
		public static bool IsOlderVersion()
		{
			string uri = ConfigHandler.Instance.PublishPath;
			Uri updatePath = new Uri(uri);
			string thisVersion = MainForm.VersionNumber;
			if (updatePath.IsFile)
				return File.Exists(uri) && CompareVersionNumbers(thisVersion, FileVersionInfo.GetVersionInfo(uri).FileVersion) > 0;
			try
			{
				return CompareVersionNumbers(thisVersion, GetVersionFromGH(updatePath)) > 0;
			}
			catch (Exception ex)
			{
				ThemedMessageBox.Error(ex);
				return false;
			}
		}

		/// <summary>
		/// Fetches tags from <paramref name="uri"/> if it is a github page
		/// </summary>
		/// <param name="uri">Link to a github page</param>
		/// <returns>Version number of latest tag or <see langword="null"/> if <paramref name="uri"/> not a github.com <see cref="Uri"/></returns>
		private static string? GetVersionFromGH(Uri uri)
		{
			if (uri.Host != "github.com")
				return null;
			HttpClientHandler clientHandler = new HttpClientHandler();
			HttpClient client = new HttpClient(clientHandler);
			HttpResponseMessage response = client.GetAsync(uri).GetAwaiter().GetResult();
			if (!response.IsSuccessStatusCode)
				return null;
			HelperClasses.HtmlClasses.HtmlDocument doc = new(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
			List<HelperClasses.HtmlClasses.HtmlElement> elements = doc.Body?.CSSSelect("a.Link--primary.Link") ?? new();
			if (elements.Count == 0)
				return null;
			return ExtractVersionNumber(elements[0].InnerText);
		}

		/// <summary>
		/// Cuts potential 'v' from verson numbers in tags
		/// </summary>
		/// <param name="versionNumber"><see cref="string"/> to extract version number from</param>
		/// <returns>Raw version number or <see langword="null"/> if <paramref name="versionNumber"/> is <see langword="null"/></returns>
		private static string? ExtractVersionNumber(string? versionNumber)
		{
			if (string.IsNullOrEmpty(versionNumber))
				return null;
			//only a leading 'v' was added infront of version tags until now
			if (versionNumber.StartsWith('v'))
				return versionNumber.Substring(1);
			return versionNumber;
		}

		/// <summary>
		/// Compares two version numbers
		/// </summary>
		/// <param name="version1">Version number 1</param>
		/// <param name="version2">Version number 2</param>
		/// <returns>0 if versions are equal, positive if version2 is greater and negative if version2 is smaller</returns>
		private static int CompareVersionNumbers(string? version1, string? version2)
		{
			if (version1 == null || version2 == null)
				return string.Compare(version1, version2);
			string[] splitv1 = version1.Split('.');
			string[] splitv2 = version2.Split('.');
			if (splitv1.Length == splitv2.Length)
			{
				for (int i = 0; i < splitv1.Length; i++)
				{
					if (splitv1[i] == splitv2[i])
						continue;
					if (int.TryParse(splitv1[i], out int v1) && int.TryParse(splitv2[i], out int v2))
					{
						return v2 - v1;
					}
					else
					{
						return splitv2[i].CompareTo(splitv1[i]);
					}
				}
			}
			else
			{
				for (int i = 0; i < Math.Min(splitv1.Length, splitv2.Length); i++)
				{
					if (splitv1[i] == splitv2[i])
						continue;
					if (int.TryParse(splitv1[i], out int v1) && int.TryParse(splitv2[i], out int v2))
					{
						return v2 - v1;
					}
					else
					{
						return splitv2[i].CompareTo(splitv1[i]);
					}
				}
				return splitv2.Length - splitv1.Length;
			}
			return 0;
		}
	}
}
