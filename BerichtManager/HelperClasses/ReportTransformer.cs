using BerichtManager.IHKClient;
using Word = Microsoft.Office.Interop.Word;
using System;
using BerichtManager.Config;
using BerichtManager.IHKClient.Exceptions;

namespace BerichtManager.HelperClasses
{
	internal class ReportTransformer
	{
		/// <summary>
		/// Creates a <see cref="Report"/> object from a created report loaded from a <see cref="Word.Document"/>
		/// </summary>
		/// <param name="doc"><see cref="Word.Document"/> to convert</param>
		/// <returns>A new <see cref="Report"/> object which has its fields filled with values from <paramref name="doc"/></returns>
		/// <inheritdoc cref="WordToIHK(Word.Document, Report, bool)" path="/exception"/>
		public static Report WordToIHK(Word.Document doc, bool throwMismatchStartDate = false)
		{
			Report report = new Report();
			WordToIHK(doc, report, throwMismatchStartDate);
			return report;
		}

		/// <summary>
		/// Fills an existing <see cref="Report"/> with values from <paramref name="doc"/>
		/// </summary>
		/// <param name="doc"><see cref="Word.Document"/> to fill report with</param>
		/// <param name="report"><see cref="Report"/> to fill with content</param>
		/// <param name="throwMismatchStartDate">If IHK report creation should check for matching start dates</param>
		/// <exception cref="InvalidDocumentException">Thrown if the document does not have the needed form fields</exception>
		/// <exception cref="StartDateMismatchException">Thrown if the start dates of report and newly created report do not match</exception>
		public static void WordToIHK(Word.Document doc, Report report, bool throwMismatchStartDate = false)
		{
			if (doc.FormFields.Count < 10)
				throw new InvalidDocumentException();
			if (throwMismatchStartDate && report.ReportContent.StartDate != doc.FormFields[3].Result)
				throw new StartDateMismatchException(doc.FormFields[3].Result, report.ReportContent.StartDate);

			//Dates are auto filled by IHK
			//report.ReportContent.StartDate = doc.FormFields[3].Result;
			//report.ReportContent.EndDate = doc.FormFields[4].Result;
			report.ReportContent.JobField = ConfigHandler.Instance.IHKJobField;
			report.ReportContent.SupervisorEMail1 = ConfigHandler.Instance.IHKSupervisorEMail;
			report.ReportContent.SupervisorEMail2 = ConfigHandler.Instance.IHKSupervisorEMail;
			report.ReportContent.JobFieldContent = ReportUtils.TransformTextToIHK(doc.FormFields[6].Result);
			report.ReportContent.SeminarsField = ReportUtils.TransformTextToIHK(doc.FormFields[7].Result);
			report.ReportContent.SchoolField = ReportUtils.TransformTextToIHK(doc.FormFields[8].Result);
		}
	}

	public class InvalidDocumentException : Exception
	{
		public InvalidDocumentException() : base("Document does not have the necessary form fields")
		{

		}
	}
}
