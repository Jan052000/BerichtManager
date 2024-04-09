using BerichtManager.IHKClient;
using Word = Microsoft.Office.Interop.Word;
using System;
using BerichtManager.Config;

namespace BerichtManager.HelperClasses
{
	internal class ReportTransformer
	{
		/// <summary>
		/// Creates a <see cref="Report"/> object from a created report loaded from a <see cref="Word.Document"/>
		/// </summary>
		/// <param name="doc"><see cref="Word.Document"/> to convert</param>
		/// <returns>A new <see cref="Report"/> object which has its fields filled with values from <paramref name="doc"/></returns>
		/// <exception cref="InvalidDocumentException">Thrown if the document does not have the needed form fields</exception>
		public static Report WordToIHK(Word.Document doc)
		{
			if (doc.FormFields.Count < 10)
				throw new InvalidDocumentException();

			Report report = new Report();
			//Dates are auto filled by IHK
			//report.ReportContent.StartDate = doc.FormFields[3].Result;
			//report.ReportContent.EndDate = doc.FormFields[4].Result;
			report.ReportContent.JobField = ConfigHandler.Instance.IHKJobField();
			report.ReportContent.SupervisorEMail1 = ConfigHandler.Instance.IHKSupervisorEMail();
			report.ReportContent.SupervisorEMail2 = ConfigHandler.Instance.IHKSupervisorEMail();
			report.ReportContent.JobFieldContent = doc.FormFields[6].Result.Replace("\v", "\n");
			report.ReportContent.SeminarsField = doc.FormFields[7].Result.Replace("\v", "\n");
			report.ReportContent.SchoolField = doc.FormFields[8].Result.Replace("\v", "\n");

			return report;
		}

		/// <summary>
		/// Fills an existing <see cref="Report"/> with values from <paramref name="doc"/>
		/// </summary>
		/// <param name="doc"><see cref="Word.Document"/> to fill report with</param>
		/// <param name="report"><see cref="Report"/> to fill with content</param>
		/// <exception cref="InvalidDocumentException">Thrown if the document does not have the needed form fields</exception>
		public static void WordToIHK(Word.Document doc, Report report)
		{
			if (doc.FormFields.Count < 10)
				throw new InvalidDocumentException();

			//Dates are auto filled by IHK
			//report.ReportContent.StartDate = doc.FormFields[3].Result;
			//report.ReportContent.EndDate = doc.FormFields[4].Result;
			report.ReportContent.JobField = ConfigHandler.Instance.IHKJobField();
			report.ReportContent.SupervisorEMail1 = ConfigHandler.Instance.IHKSupervisorEMail();
			report.ReportContent.SupervisorEMail2 = ConfigHandler.Instance.IHKSupervisorEMail();
			report.ReportContent.JobFieldContent = doc.FormFields[6].Result.Replace("\v", "\n");
			report.ReportContent.SeminarsField = doc.FormFields[7].Result.Replace("\v", "\n");
			report.ReportContent.SchoolField = doc.FormFields[8].Result.Replace("\v", "\n");
		}
	}

	public class InvalidDocumentException : Exception
	{
		public InvalidDocumentException() : base("Document does not have the necessary form fields")
		{

		}
	}
}
