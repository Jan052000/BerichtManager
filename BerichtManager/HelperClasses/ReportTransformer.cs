using BerichtManager.IHKClient;
using Word = Microsoft.Office.Interop.Word;
using System;
using BerichtManager.Config;
using BerichtManager.IHKClient.Exceptions;
using System.Globalization;

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

		/// <summary>
		/// Fills <paramref name="doc"/> with values from <paramref name="report"/> using <paramref name="wordApp"/>
		/// </summary>
		/// <param name="wordApp"><see cref="Word.Application"/> report is opened in</param>
		/// <param name="doc"><see cref="Word.Document"/> to fill</param>
		/// <param name="report"><see cref="Report"/> object containing values for report file</param>
		public static void IHKToWord(Word.Application wordApp, Word.Document doc, Report report)
		{
			FillFormField(wordApp, doc.FormFields[1], ReportUtils.TransformTextToWord(ConfigHandler.Instance.ReportUserName));
			FillFormField(wordApp, doc.FormFields[2], ReportUtils.TransformTextToWord(report.ReportNr.ToString()));
			FillFormField(wordApp, doc.FormFields[3], ReportUtils.TransformTextToWord(report.ReportContent.StartDate));
			FillFormField(wordApp, doc.FormFields[4], ReportUtils.TransformTextToWord(report.ReportContent.EndDate));
			FillFormField(wordApp, doc.FormFields[5], ReportUtils.TransformTextToWord(DateTime.ParseExact(report.ReportContent.StartDate, "dd.MM.yyyy", CultureInfo.CurrentCulture).Year.ToString()));
			FillFormField(wordApp, doc.FormFields[6], ReportUtils.TransformTextToWord(report.ReportContent.JobFieldContent));
			FillFormField(wordApp, doc.FormFields[7], ReportUtils.TransformTextToWord(report.ReportContent.SeminarsField));
			FillFormField(wordApp, doc.FormFields[8], ReportUtils.TransformTextToWord(report.ReportContent.SchoolField));
			FillFormField(wordApp, doc.FormFields[9], ReportUtils.TransformTextToWord(report.ReportContent.EndDate));
			FillFormField(wordApp, doc.FormFields[10], ReportUtils.TransformTextToWord(report.ReportContent.EndDate));
		}

		/// <summary>
		/// Fills a WordInterop TextField with text
		/// </summary>
		/// <param name="app">The Word Application containing the documents with FormFields to fill</param>
		/// <param name="field">The FormField to fill with Text</param>
		/// <param name="text">The Text to Fill</param>
		public static void FillFormField(Word.Application app, Word.FormField field, string text)
		{
			if (text == null)
				return;
			text = ReportUtils.TransformTextToWord(text);
			field.Select();
			for (int i = 1; i < 6; i++)
			{
				field.Range.Paragraphs.TabStops.Add(i * 14);
			}
			app.Selection.MoveLeft(Word.WdUnits.wdCharacter, 1);
			app.Selection.MoveRight(Word.WdUnits.wdCharacter, 1);
			if (text.Length > 254)
			{
				field.Result = " ";
				app.Selection.Text = text.Substring(0, 200);
				field.Result = field.Result.TrimEnd() + " ";
				app.Selection.MoveLeft(Word.WdUnits.wdCharacter, 1);
				app.Selection.TypeText(text.Substring(200));
				//Remove first space before text
				field.Select();
				app.Selection.MoveLeft(Word.WdUnits.wdCharacter, 1);
				app.Selection.MoveRight(Word.WdUnits.wdCharacter, 1);
				app.Selection.TypeBackspace();
			}
			else
			{
				field.Result = text;
			}
		}
	}

	public class InvalidDocumentException : Exception
	{
		public InvalidDocumentException() : base("Document does not have the necessary form fields")
		{

		}
	}
}
