using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace BerichtManager.HelperClasses
{
	internal class Logger
	{
		public static void LogError(Exception ex, JObject additionalData = null)
		{
			string logFolder = Path.GetFullPath(".\\Logs");
			string errorDate = DateTime.Now.ToString("dd.M.yyyy.H.m.s");
			if (!Directory.Exists(logFolder))
			{
				Directory.CreateDirectory(logFolder);
			}
			if (additionalData != null)
			{
				File.WriteAllText(logFolder + "\\" + errorDate + ".txt", ex.Message
				+ "\n" + ex.StackTrace.ToString() + "\n" + ex.HResult + "\n\n" + JsonConvert.SerializeObject(additionalData));
			}
			else 
			{
				File.WriteAllText(logFolder + "\\" + errorDate + ".txt", ex.Message
				+ "\n" + ex.StackTrace.ToString() + "\n" + ex.HResult);
			}
		}
	}
}
