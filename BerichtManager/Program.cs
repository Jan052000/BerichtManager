using System;
using System.IO;
using System.Windows.Forms;

namespace BerichtManager
{
	internal static class Program
	{
		/// <summary>
		/// Der Haupteinstiegspunkt für die Anwendung.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			try
			{
				Application.Run(new MainForm());
			}
			catch(Exception e) 
			{
				string logFolder = Path.GetFullPath(".\\Logs");
				string errorDate = DateTime.Now.ToString("dd.M.yyyy.H.m.s");
				if (!Directory.Exists(logFolder)) 
				{
					Directory.CreateDirectory(logFolder);
				}
				File.WriteAllText(logFolder + "\\" + errorDate + ".txt", e.Message + " : " + e.HResult
					+ "\n" + e.StackTrace.ToString());
				MessageBox.Show("An unexpected error has occurred and the program has crashed a crash-log has been created at: " + logFolder + "\\" + errorDate + ".txt", "Application has crashed", MessageBoxButtons.OK);
			}
		}
	}
}
