using BerichtManager.IHKClient;
using Word = Microsoft.Office.Interop.Word;
using BerichtManager.Config;
using BerichtManager.IHKClient.Exceptions;
using System.Globalization;
using BerichtManager.WordTemplate;

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
			report.ReportContent.StartDate = FormFieldHandler.GetValueFromDoc<string>(Fields.StartDate, doc);
			report.ReportContent.EndDate = ReportUtils.TransformTextToIHK(FormFieldHandler.GetValueFromDoc<string>(Fields.EndDate, doc));
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
			if (!FormFieldHandler.ValidFormFieldCount(doc))
				throw new InvalidDocumentException();
			string? reportStartDate = report.ReportContent.StartDate;
			string? documentStartDate = FormFieldHandler.GetValueFromDoc<string>(Fields.StartDate, doc);
			if (reportStartDate == null || documentStartDate == null)
				throw new NullOrEmptyStartDateException();
			if (throwMismatchStartDate && reportStartDate != documentStartDate)
				throw new StartDateMismatchException(documentStartDate, reportStartDate);

			//Dates are auto filled by IHK
			//report.ReportContent.StartDate = FormFieldHandler.GetValueFromDoc<string>(Fields.StartDate, doc);
			//report.ReportContent.EndDate = FormFieldHandler.GetValueFromDoc<string>(Fields.EndDate, doc);
			report.ReportContent.JobField = ConfigHandler.Instance.IHKJobField;
			report.ReportContent.SupervisorEMail1 = ConfigHandler.Instance.IHKSupervisorEMail;
			report.ReportContent.SupervisorEMail2 = ConfigHandler.Instance.IHKSupervisorEMail;
			report.ReportContent.JobFieldContent = ReportUtils.TransformTextToIHK(FormFieldHandler.GetValueFromDoc<string>(Fields.Work, doc));
			report.ReportContent.SeminarsField = ReportUtils.TransformTextToIHK(FormFieldHandler.GetValueFromDoc<string>(Fields.Seminars, doc));
			report.ReportContent.SchoolField = ReportUtils.TransformTextToIHK(FormFieldHandler.GetValueFromDoc<string>(Fields.School, doc));
		}

		/// <summary>
		/// Fills <paramref name="doc"/> with values from <paramref name="report"/> using <paramref name="wordApp"/>
		/// </summary>
		/// <param name="doc"><see cref="Word.Document"/> to fill</param>
		/// <param name="report"><see cref="Report"/> object containing values for report file</param>
		public static void IHKToWord(Word.Document doc, Report report)
		{
			FormFieldHandler.SetValueInDoc(Fields.Name, doc, ConfigHandler.Instance.ReportUserName);
			FormFieldHandler.SetValueInDoc(Fields.Number, doc, report.ReportNr.ToString());
			FormFieldHandler.SetValueInDoc(Fields.StartDate, doc, report.ReportContent.StartDate);
			FormFieldHandler.SetValueInDoc(Fields.EndDate, doc, report.ReportContent.EndDate);
			FormFieldHandler.SetValueInDoc(Fields.Year, doc, MainForm.GetYearOfReport(DateTime.ParseExact(report.ReportContent.StartDate!, "dd.MM.yyyy", CultureInfo.CurrentCulture)).ToString());
			FormFieldHandler.SetValueInDoc(Fields.Work, doc, report.ReportContent.JobFieldContent);
			FormFieldHandler.SetValueInDoc(Fields.Seminars, doc, report.ReportContent.SeminarsField);
			FormFieldHandler.SetValueInDoc(Fields.School, doc, report.ReportContent.SchoolField);
			FormFieldHandler.SetValueInDoc(Fields.SignDateYou, doc, report.ReportContent.EndDate);
			FormFieldHandler.SetValueInDoc(Fields.SignDateSupervisor, doc, report.ReportContent.EndDate);
		}
	}

	public class InvalidDocumentException : Exception
	{
		public InvalidDocumentException() : base("Document does not have the necessary form fields")
		{

		}
	}

	public class NullOrEmptyStartDateException : Exception
	{
		public NullOrEmptyStartDateException() : base("Start date of either report document or generated report object is invalid")
		{

		}
	}
}
