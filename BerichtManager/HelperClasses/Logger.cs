using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BerichtManager.HelperClasses
{
	internal class Logger
	{
		/// <summary>
		/// Writes an <paramref name="ex"/> to a file
		/// </summary>
		/// <param name="ex"><see cref="Exception"/> to log</param>
		/// <param name="additionalData">Additional data contained in exception</param>
		/// <returns>Full path the log was saved to</returns>
		public static string LogError(Exception ex, JObject additionalData = null)
		{
			string logFolder = Path.GetFullPath(".\\Logs");
			string errorDate = DateTime.Now.ToString("dd.M.yyyy.H.m.s");
			string filePath = logFolder + "\\" + errorDate + ".txt";
			string message = $"{ex.Message}\n{ex.StackTrace}\n{ex.HResult}";
			if (!Directory.Exists(logFolder))
				Directory.CreateDirectory(logFolder);
			if (additionalData != null)
				message += $"\nAdditional data:\n{JsonConvert.SerializeObject(additionalData, Formatting.Indented)}";

			File.WriteAllText(filePath, message);
			return filePath;
		}
	}
}
