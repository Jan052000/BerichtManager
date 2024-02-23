using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

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
			if (!Directory.Exists(logFolder))
			{
				Directory.CreateDirectory(logFolder);
			}
			if (additionalData != null)
			{
				File.WriteAllText(filePath, ex.Message
				+ "\n" + ex.StackTrace.ToString() + "\n" + ex.HResult + "\n\n" + JsonConvert.SerializeObject(additionalData));
			}
			else
			{
				File.WriteAllText(filePath, ex.Message
				+ "\n" + ex.StackTrace.ToString() + "\n" + ex.HResult);
			}
			return filePath;
		}
	}
}
