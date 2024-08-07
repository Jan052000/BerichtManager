using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using BerichtManager.Config;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using BerichtManager.IHKClient.Exceptions;
using Word = Microsoft.Office.Interop.Word;
using BerichtManager.HelperClasses;
using System.Text.RegularExpressions;
using BerichtManager.UploadChecking;
using System.Globalization;
using System.Threading;
using BerichtManager.HelperClasses.HtmlClasses;
using BerichtManager.IHKClient.ReportContents;

namespace BerichtManager.IHKClient
{
	/// <summary>
	/// Client for communicating with the IHK website
	/// </summary>
	internal class IHKClient
	{
		/// <summary>
		/// Client to use for requests
		/// </summary>
		private HttpClient HttpClient { get; }
		/// <summary>
		/// Handler mainly to set cookies in <see cref="HttpClient"/>
		/// </summary>
		private HttpClientHandler HttpClientHandler { get; } = new HttpClientHandler();
		/// <summary>
		/// Container for cookies used by <see cref="HttpClient"/>
		/// </summary>
		private CookieContainer CookieContainer { get; } = new CookieContainer();
		/// <summary>
		/// Base url for IHK endpoint
		/// </summary>
		private string BASEURL { get => ConfigHandler.Instance.IHKBaseUrl; }
		/// <summary>
		/// Wether or not the client is logged in
		/// </summary>
		private bool LoggedIn { get; set; } = false;
		/// <summary>
		/// <see cref="System.Timers.Timer"/> with an interval of 15m to end login session
		/// </summary>
		private System.Timers.Timer LoginSessionTimeout { get; } = new System.Timers.Timer
		{
			Interval = 900000,
			Enabled = false,
			AutoReset = false
		};

		public IHKClient()
		{
			LoginSessionTimeout.Elapsed += LoginSessionTimeout_Elapsed;
			HttpClient = new HttpClient(HttpClientHandler);
			HttpClientHandler.CookieContainer = CookieContainer;
			HttpClientHandler.UseCookies = true;
			HttpClient.BaseAddress = new Uri(BASEURL);
			HttpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36 Edg/122.0.0.0");
		}

		/// <summary>
		/// Changes the <see cref="HttpClient"/> base address to address in <see cref="ConfigHandler"/>
		/// </summary>
		public void UpdateBaseURL()
		{
			HttpClient.BaseAddress = new Uri(BASEURL);
		}

		/// <summary>
		/// Handler for <see cref="LoginSessionTimeout"/> elapsed event
		/// </summary>
		private void LoginSessionTimeout_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			LoggedIn = false;
			HttpClient.DefaultRequestHeaders.Remove("Cookie");
		}

		/// <summary>
		/// Resets <see cref="LoginSessionTimeout"/>
		/// </summary>
		private void ResetTimer()
		{
			LoginSessionTimeout.Stop();
			LoginSessionTimeout.Start();
		}

		/// <summary>
		/// Generates full Uri from a relative path
		/// </summary>
		/// <param name="path">Relative path</param>
		/// <returns>Full Uri</returns>
		private Uri FromRelativeUri(string path)
		{
			return new Uri(HttpClient.BaseAddress, path);
		}

		/// <summary>
		/// Sends an async post request using <see cref="HttpClient"/> and sets the referer to the <paramref name="uri"/> path
		/// </summary>
		/// <param name="uri">Relative path</param>
		/// <param name="content">Content to be posted</param>
		/// <returns><see cref="HttpResponseMessage"/> of request</returns>
		private async Task<HttpResponseMessage> PostAndRefer(string uri, HttpContent content)
		{
			//await Task.Delay(200 + new Random().Next(-50, 50));
			HttpResponseMessage response = await HttpClient.PostAsync(uri, content);
			HttpClient.DefaultRequestHeaders.Referrer = FromRelativeUri(uri);
			return response;
		}

		/// <summary>
		/// Sends an async get request using <see cref="HttpClient"/> and sets the referer to the <paramref name="uri"/> path
		/// </summary>
		/// <param name="uri">Relative path</param>
		/// <returns><see cref="HttpResponseMessage"/> of request</returns>
		private async Task<HttpResponseMessage> GetAndRefer(string uri)
		{
			//await Task.Delay(200 + new Random().Next(-50, 50));
			HttpResponseMessage response = await HttpClient.GetAsync(uri);
			HttpClient.DefaultRequestHeaders.Referrer = FromRelativeUri(uri);
			return response;
		}

		/// <summary>
		/// Sends an async get request using <see cref="HttpClient"/> and sets the referer to the <paramref name="uri"/> path
		/// </summary>
		/// <param name="uri">Uri</param>
		/// <returns><see cref="HttpResponseMessage"/> of request</returns>
		private async Task<HttpResponseMessage> GetAndRefer(Uri uri)
		{
			//await Task.Delay(200 + new Random().Next(-50, 50));
			HttpResponseMessage response = await HttpClient.GetAsync(uri);
			HttpClient.DefaultRequestHeaders.Referrer = uri;
			return response;
		}

		/// <summary>
		/// Retrieves and sets the first necessary cookies and referer from IHK
		/// </summary>
		/// <param name="path">Relative path to get cookies from</param>
		/// <returns><see langword="true"/> if cookies were recieved and set, <see langword="false"/> otherwise</returns>
		private async Task<bool> SetFirstCookie()
		{
			HttpClient.DefaultRequestHeaders.Remove("Cookie");
			string path = "tibrosBB/BB_auszubildende.jsp";
			HttpResponseMessage response = await GetAndRefer(path);
			if (!response.IsSuccessStatusCode)
				return false;
			if (!response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> setCookies))
				return false;
			if (setCookies.Count() == 0)
				return false;
			List<Cookie> cookies = CookieContainer.GetCookies(FromRelativeUri(path)).Cast<Cookie>().ToList();
			if (cookies.Count == 0)
				return false;
			cookies.ForEach(cookie => HttpClient.DefaultRequestHeaders.Add("Cookie", cookie.ToString()));
			HttpClient.DefaultRequestHeaders.Referrer = FromRelativeUri(path);
			return true;
		}

		/// <summary>
		/// Logs the client in to the IHK website
		/// </summary>
		/// <returns><see langword="true"/> if login was successful or client was already logged in and <see langword="false"/> otherwise</returns>
		private async Task<bool> DoLogin()
		{
			if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
				return false;
			if (LoggedIn)
				return true;
			if (!await SetFirstCookie())
				return false;

			string uri = "tibrosBB/azubiHome.jsp";
			string username = ConfigHandler.Instance.IHKUserName;
			string password = ConfigHandler.Instance.IHKPassword;
			if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
			{
				User user = ConfigHandler.Instance.DoIHKLogin();
				if (user == null)
					return false;
				username = user.Username;
				password = user.Password;
				if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
					return false;
			}
			Dictionary<string, string> content = new Dictionary<string, string>()
			{
				{ "login", username },
				{ "pass", password },
				{ "anmelden", null }
			};
			HttpResponseMessage response = await PostAndRefer(uri, new FormUrlEncodedContent(content));
			if (response.StatusCode != HttpStatusCode.Found && !response.IsSuccessStatusCode)
				return false;
			//Get second cookie if recieved
			HttpClient.DefaultRequestHeaders.Remove("Cookie");
			List<Cookie> cookies = CookieContainer.GetCookies(FromRelativeUri(uri)).Cast<Cookie>().ToList();
			if (cookies.Count == 0)
				return false;
			cookies.ForEach(cookie => HttpClient.DefaultRequestHeaders.Add("Cookie", cookie.ToString()));

			HttpClient.DefaultRequestHeaders.Referrer = FromRelativeUri(uri);
			LoggedIn = true;
			ResetTimer();
			return true;
		}

		/// <summary>
		/// Fetches a <see cref="List{T}"/> of <see cref="UploadedReport"/>s from IHK site
		/// </summary>
		/// <returns><see cref="List{T}"/> with found <see cref="UploadedReport"/>s</returns>
		/// <exception cref="HttpRequestException"></exception>
		public async Task<List<UploadedReport>> GetIHKReports()
		{
			if (!LoggedIn)
				if (!await DoLogin())
					return new List<UploadedReport>();
			HttpResponseMessage response = await GetAndRefer("tibrosBB/azubiHeft.jsp");
			if (!response.IsSuccessStatusCode)
				return new List<UploadedReport>();
			HtmlDocument doc = GetHtmlDocument(await response.Content.ReadAsStringAsync());
			List<HtmlElement> reportElements = doc.Body.CSSSelect("div.reihe");
			ResetTimer();
			return TransformHtmlToReports(reportElements);
		}

		/// <summary>
		/// Transform <see cref="HtmlElement"/>s to <see cref="UploadedReport"/>s
		/// </summary>
		/// <param name="reportElements">List of report <see cref="HtmlElement"/>s from IHK page</param>
		/// <returns><see cref="List{T}"/> with transformed <see cref="UploadedReport"/>s</returns>
		/// <exception cref="HttpRequestException"></exception>
		private List<UploadedReport> TransformHtmlToReports(List<HtmlElement> reportElements)
		{
			List<UploadedReport> uploadedReports = new List<UploadedReport>();
			reportElements.ForEach(reportElement =>
			{
				if (reportElement.Children.Count < Enum.GetNames(typeof(ReportElementFields)).Length)
					return;
				List<HtmlElement> rows = reportElement.CSSSelect("div.col-md-8");
				Regex datesRegex = new Regex("(\\d+?\\.\\d+?\\.\\d+)");
				if (!DateTime.TryParseExact(datesRegex.Match(rows[(int)ReportElementFields.TimeSpan].InnerText).Value, "dd.MM.yyyy", null, DateTimeStyles.None, out DateTime startDate))
					return;
				if (!new ReportStatuses().TryGetValue(rows[(int)ReportElementFields.Status].InnerText.Trim(), out ReportNode.UploadStatuses status))
					return;
				if (ExtractLfdNr(rows[(int)ReportElementFields.ButtonAction].InnerHTML.Trim(), out int? lfdnr))
					uploadedReports.Add(new UploadedReport(startDate, status: status, lfdNr: lfdnr));
			});
			return uploadedReports;
		}

		/// <summary>
		/// Searches <paramref name="hRefLink"/> for lfdnr s
		/// </summary>
		/// <param name="hRefLink">Text to search</param>
		/// <param name="lfdnr">Parsed lfdnr if possible and <see langword="null"/> otherwise</param>
		/// <returns><see langword="true"/> if lfdnr was found and parsed or <see langword="false"/> otherwise</returns>
		private bool ExtractLfdNr(string hRefLink, out int? lfdnr)
		{
			lfdnr = null;
			Regex regex = new Regex(@"(lfdnr=(?<lfdnr>\d+))", RegexOptions.Singleline | RegexOptions.ExplicitCapture);
			Match match = regex.Match(hRefLink);
			if (match.Success)
			{
				if (int.TryParse(match.Groups["lfdnr"].Value, out int result))
				{
					lfdnr = result;
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Position of report values in <see cref="HtmlElement"/> form IHK
		/// </summary>
		private enum ReportElementFields
		{
			/// <summary>
			/// <see cref="TimeSpan"/> of report
			/// </summary>
			TimeSpan,
			/// <summary>
			/// Job field
			/// </summary>
			JobField,
			/// <summary>
			/// Supervisor e-mail
			/// </summary>
			Supervisor,
			/// <summary>
			/// Upload status
			/// </summary>
			Status,
			/// <summary>
			/// Button actions
			/// </summary>
			ButtonAction
		}

		/// <summary>
		/// Creates and saves the report on IHK online campus
		/// </summary>
		/// <param name="document"><see cref="Word.Document"/> to upload</param>
		/// <param name="checkMatchingStartDates">If IHK report creation should check for matching start dates</param>
		/// <returns><see cref="UploadResult"/> object containing status and start date of report</returns>
		/// <inheritdoc cref="CreateOrEditReport(Word.Document, int)" path="/exception"/>
		public async Task<UploadResult> CreateReport(Word.Document document, bool checkMatchingStartDates = false)
		{
			return await CreateOrEditReport(document, checkMatchingStartDates: checkMatchingStartDates);
		}

		/// <summary>
		/// Fills <paramref name="report"/> with form fields from <paramref name="doc"/>
		/// </summary>
		/// <param name="report"><see cref="Report"/> which should have its content filled</param>
		/// <param name="doc"><see cref="HtmlElement"/> of html form</param>
		/// <exception cref="NoFormFoundException">Thrown if no form element was found in <paramref name="doc"/></exception>
		/// <exception cref="NoInputsFoundException">Thrown if no input elements were found in <paramref name="doc"/></exception>
		/// <exception cref="UnknownFieldException">Thrown if a property that is unknown to <see cref="Report"/> is in <paramref name="doc"/></exception>
		/// <exception cref="InputFieldsMismatchException">Thrown if the length of found inputs does not match the number of properties to fill</exception>
		private void FillReportContent(Report report, HtmlDocument doc)
		{
			if (doc.Forms.Count == 0)
				throw new NoFormFoundException();
			if (!FindAllInputInElement(doc.Forms[0], out List<HtmlElement> inputs))
				throw new NoInputsFoundException();
			if (inputs.Count - report.ReportContent.GetType().GetProperties().Length != 0)
			{
				foreach (HtmlElement htmlElement in inputs)
				{
					if (report.ReportContent.GetType().GetProperties().ToList().Find(prop =>
					{
						IHKFormDataNameAttribute attr = prop.GetCustomAttributes(typeof(IHKFormDataNameAttribute)).First() as IHKFormDataNameAttribute;
						if (attr == null)
							return false;
						return attr.Name == htmlElement.Name || prop.Name == htmlElement.Name;
					}) == null)
						throw new UnknownFieldException(htmlElement.Name);
				}
				throw new InputFieldsMismatchException();
			}

			List<string> filledProps = new List<string>();
			foreach (HtmlElement input in inputs)
			{
				//Find list of properties which have the same IHKForm name as input
				List<PropertyInfo> matchingProps = report.ReportContent.GetType().GetProperties().ToList().FindAll(prop =>
				{
					if (!(prop.GetCustomAttributes(typeof(IHKFormDataNameAttribute)).First() is IHKFormDataNameAttribute attr))
					{
#if DEBUG
						Console.WriteLine($"Property {prop.Name} of report has no IHKFormDataNameAttribute");
#endif
						return false;
					}
					if (!attr.IsActuallySent)
						return false;

					return attr.Name == input.Name || prop.Name == input.Name;
				});
				//If no matches found the field is unknown and new to the form
				if (matchingProps.Count == 0)
					throw new UnknownFieldException(input.Name);
				//Fill props with values from input
				matchingProps.ForEach(prop =>
				{
					switch (input.Type.ToLower())
					{
						case "file":
							//Uploading files is not implemented
							prop.SetValue(report.ReportContent, new byte[0]);
							break;
						default:
							prop.SetValue(report.ReportContent, Convert.ChangeType(input.Value, prop.PropertyType));
							break;
					}
				});
			}
		}

		/// <summary>
		/// Finds all input and textarea tags in <paramref name="root"/>
		/// </summary>
		/// <param name="root"><see cref="HtmlElement"/> root of form or element to check</param>
		/// <param name="inputs"><see cref="List{T}"/> of <see cref="HtmlElement"/> inputs</param>
		/// <param name="addDisabled"><see langword="true"/> to add disabled html elements to output</param>
		/// <returns><see langword="true"/> if any inputs were found and <see langword="false"/> otherwise</returns>
		private bool FindAllInputInElement(HtmlElement root, out List<HtmlElement> inputs, bool addDisabled = false)
		{
			inputs = new List<HtmlElement>();
			foreach (HtmlElement element in root.Children)
			{
				if (element.Tag.Equals("input", StringComparison.CurrentCultureIgnoreCase) || element.Tag.Equals("textarea", StringComparison.CurrentCultureIgnoreCase))
					inputs.Add(element);
				else
				{
					if (FindAllInputInElement(element, out List<HtmlElement> childInputs))
					{
						if (!addDisabled)
							childInputs = childInputs.Where(x => x.Enabled).ToList();
						inputs.AddRange(childInputs);
					}
				}
			}
			return inputs.Count > 0;
		}

		/// <summary>
		/// Generates a new <see cref="MultipartFormDataContent"/> containing the properties of <paramref name="content"/>
		/// </summary>
		/// <param name="content"><see cref="ReportContent"/> content to use</param>
		/// <returns>New filled <see cref="MultipartFormDataContent"/> object</returns>
		private MultipartFormDataContent GetMultipartFormDataContent(ReportContent content)
		{
			MultipartFormDataContent mPFDC = new MultipartFormDataContent();
			content.GetType().GetProperties().ToList().ForEach(prop =>
			{
				IHKFormDataNameAttribute attr = prop.GetCustomAttributes(typeof(IHKFormDataNameAttribute)).First() as IHKFormDataNameAttribute;
				if (!attr.IsActuallySent)
					return;
				string propName;
				if (attr != null)
					propName = attr.Name;
				else
					propName = prop.Name;

				if (prop.PropertyType == typeof(byte[]))
				{
					ByteArrayContent file = new ByteArrayContent((byte[])prop.GetValue(content) ?? new byte[0]);
					mPFDC.Add(file, $"\"{propName}\"");
					file.Headers.Add("Content-Type", "application/octet-stream");
					file.Headers.ContentDisposition.FileName = "\"\"";
				}
				else
				{
					StringContent c = new StringContent($"{prop.GetValue(content)}");
					c.Headers.Remove("Content-Type");
					mPFDC.Add(c, $"\"{propName}\"");
				}
			});
			return mPFDC;
		}

		/// <summary>
		/// Hands in a report
		/// </summary>
		/// <param name="lfdnr">lfdnr of report to hand in</param>
		/// <returns><see langword="true"/> if report was handed in or <see langword="false"/> otherwise</returns>
		/// <exception cref="HttpRequestException">Thrown if an exception in connection occurs</exception>
		public async Task<bool> HandInReport(int lfdnr)
		{
			if (!LoggedIn && !await DoLogin())
				return false;
			if (!await EnsureReferrer("tibrosBB/azubiHeft.jsp"))
				return false;
			//Opens first form
			HttpResponseMessage response = await GetAndRefer($"tibrosBB/azubiHeftEintragDetails.jsp?lfdnr={lfdnr}");
			if (!response.IsSuccessStatusCode)
				return false;
			HtmlDocument doc = GetHtmlDocument(await response.Content.ReadAsStringAsync());
			if (doc.Forms.Count == 0)
				throw new NoFormFoundException();
			if (!FindAllInputInElement(doc.Forms[0], out List<HtmlElement> inputs))
				throw new NoInputsFoundException();
			if (!(inputs.Find(element => element.Name == "token") is HtmlElement tokenElement))
				throw new InputFieldsMismatchException();

			response = await PostAndRefer("tibrosBB/azubiHeftEintragSenden.jsp", new FormUrlEncodedContent(new Dictionary<string, string> { { "token", tokenElement.Value }, { "senden", "" } }));
			if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.Found)
				return false;

			if (response.Headers.Location == null || string.IsNullOrEmpty(response.Headers.Location.ToString()))
				response = await GetAndRefer("tibrosBB/azubiHeft.jsp");
			else
				response = await GetAndRefer(response.Headers.Location);

			ResetTimer();
			return true;
		}

		/// <summary>
		/// Edits a report with number <paramref name="lfdnr"/> to fit contents of <paramref name="document"/>
		/// </summary>
		/// <param name="document"><see cref="Word.Document"/> Content of report</param>
		/// <param name="lfdnr">Lfdnr on IHK servers if edit</param>
		/// <returns><see cref="UploadResult"/> result of creation process</returns>
		/// <inheritdoc cref="CreateOrEditReport(Word.Document, int)" path="/exception"/>
		public async Task<UploadResult> EditReport(Word.Document document, int lfdnr)
		{
			return await CreateOrEditReport(document, lfdnr);
		}

		/// <summary>
		/// Creates or edits reports based on <paramref name="lfdNR"/>
		/// </summary>
		/// <param name="document"><see cref="Word.Document"/> to use for content</param>
		/// <param name="lfdNR">Number of report on IHK servers if it should be edited</param>
		/// <param name="checkMatchingStartDates">If IHK report creation should check for matching start dates</param>
		/// <returns><see cref="UploadResult"/></returns>
		/// <inheritdoc cref="FillReportContent(Report, HtmlDocument)" path="/exception"/>
		/// <inheritdoc cref="ReportTransformer.WordToIHK(Word.Document, Report, bool)" path="/exception"/>
		/// <exception cref="HttpRequestException"></exception>
		private async Task<UploadResult> CreateOrEditReport(Word.Document document, int? lfdNR = null, bool checkMatchingStartDates = false)
		{
			if (!LoggedIn)
				if (!await DoLogin())
					return new UploadResult(CreateResults.Unauthorized);

			if (!await EnsureReferrer("tibrosBB/azubiHeft.jsp"))
				return new UploadResult(CreateResults.Unauthorized);
			HttpResponseMessage response;
			if (lfdNR.HasValue && lfdNR >= 0)
				response = await GetAndRefer($"tibrosBB/azubiHeftEditForm.jsp?lfdnr={lfdNR}");
			else
				response = await PostAndRefer("tibrosBB/azubiHeftEditForm.jsp", new FormUrlEncodedContent(new Dictionary<string, string>() { { "neu", null } }));
			if (!response.IsSuccessStatusCode)
				return new UploadResult(CreateResults.CreationFailed);
			//Fill report with contents from new IHK report
			HtmlDocument doc = GetHtmlDocument(await response.Content.ReadAsStringAsync());
			Report report = new Report();
			FillReportContent(report, doc);
			//Overwrite contents from IHK
			ReportTransformer.WordToIHK(document, report, checkMatchingStartDates);
			MultipartFormDataContent content = GetMultipartFormDataContent(report.ReportContent);
			//Add necessary save parameter IHK needs to save reports
			StringContent save = new StringContent("");
			save.Headers.Remove("Content-Type");
			content.Add(save, "\"save\"");
			//Post content to create report
			response = await PostAndRefer("tibrosBB/azubiHeftAdd.jsp", content);
			if (response.StatusCode != HttpStatusCode.Found && !response.IsSuccessStatusCode)
				return new UploadResult(CreateResults.UploadFailed);
			if (response.Headers.Location == null || string.IsNullOrEmpty(response.Headers.Location.ToString()))
				response = await GetAndRefer("tibrosBB/azubiHeft.jsp");
			else
				response = await GetAndRefer(response.Headers.Location);
			int? lfdnr = lfdNR;
			//Get lfdnr from last report in list as shown in html on IHK site
			if (!lfdnr.HasValue || lfdnr < 0)
			{
				doc = GetHtmlDocument(await response.Content.ReadAsStringAsync());
				List<UploadedReport> uploadedReports = TransformHtmlToReports(doc.Body.CSSSelect("div.reihe"));
				if (uploadedReports.Find(ureport => ureport.StartDate == DateTime.Parse(report.ReportContent.StartDate)) is UploadedReport currentReport)
					lfdnr = currentReport.LfdNR;
			}
			ResetTimer();
			return new UploadResult(CreateResults.Success, DateTime.Parse(report.ReportContent.StartDate), lfdnr: lfdnr);
		}

		/// <summary>
		/// Gets the contents of the report with number <paramref name="lfdnr"/>
		/// </summary>
		/// <param name="lfdnr">Number of report on IHK servers</param>
		/// <returns><see cref="GetReportResult"/> of fetching report content</returns>
		public async Task<GetReportResult> GetReportContent(int? lfdnr)
		{
			if (!lfdnr.HasValue || lfdnr < 0)
				return new GetReportResult(GetReportResult.ResultStatuses.InvalidLfdnr);
			if (!LoggedIn && !await DoLogin())
				return new GetReportResult(GetReportResult.ResultStatuses.LoginFailed);
			if (!await EnsureReferrer("tibrosBB/azubiHeft.jsp"))
				return new GetReportResult(GetReportResult.ResultStatuses.Unauthorized);
			HttpResponseMessage response = await GetAndRefer($"tibrosBB/azubiHeftEditForm.jsp?lfdnr={lfdnr}");
			if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.Found)
				return new GetReportResult(GetReportResult.ResultStatuses.UnableToOpenReport);

			HtmlDocument doc = new HtmlDocument(await response.Content.ReadAsStringAsync());
			Report report = new Report();
			TryFillReportContent(report, doc);

			//IHK report has different fields when report was accepted
			/*
			MultipartFormDataContent content = GetMultipartFormDataContent(report.ReportContent);
			StringContent cancel = new StringContent("");
			cancel.Headers.Remove("Content-Type");
			content.Add(cancel, "\"cancel\"");
			await PostAndRefer("tibrosBB/azubiHeftAdd.jsp", content);

			if (response.Headers.Location == null || string.IsNullOrEmpty(response.Headers.Location.ToString()))
				await GetAndRefer("tibrosBB/azubiHeft.jsp");
			else
			*/
			await GetAndRefer(response.Headers.Location);

			ResetTimer();
			return new GetReportResult(GetReportResult.ResultStatuses.Success, report.ReportContent);
		}

		/// <summary>
		/// Fills a <see cref="Report"/>s content where its properties match inputs in <paramref name="doc"/> and does not handle missing or mismatched properties
		/// </summary>
		/// <param name="report"><see cref="Report"/> to fill</param>
		/// <param name="doc"><see cref="HtmlDocument"/> to get inputs from</param>
		private bool TryFillReportContent(Report report, HtmlDocument doc)
		{
			if (doc.Forms.Count == 0)
				return false;
			List<HtmlElement> inputs = doc.Forms[0]?.AllInputs;
			if (inputs == null || (inputs != null && inputs.Count == 0))
				return false;
			List<string> filledProps = new List<string>();
			foreach (HtmlElement input in inputs)
			{
				//Find list of properties which have the same IHKForm name as input
				List<PropertyInfo> matchingProps = report.ReportContent.GetType().GetProperties().ToList().FindAll(prop =>
				{
					if (!(prop.GetCustomAttributes(typeof(IHKFormDataNameAttribute)).First() is IHKFormDataNameAttribute attr))
					{
#if DEBUG
						Console.WriteLine($"Property {prop.Name} of report has no IHKFormDataNameAttribute");
#endif
						return false;
					}
					if (!attr.IsActuallySent)
						return false;

					return attr.Name == input.Name || prop.Name == input.Name;
				});
				//Fill props with values from input
				matchingProps.ForEach(prop =>
				{
					switch (input.Type.ToLower())
					{
						case "file":
							//Uploading files is not implemented
							prop.SetValue(report.ReportContent, new byte[0]);
							break;
						default:
							prop.SetValue(report.ReportContent, Convert.ChangeType(input.Value, prop.PropertyType));
							break;
					}
				});
			}
			return true;
		}

		/// <summary>
		/// Gets the supervisors' comment for the report with number <paramref name="lfdNR"/>
		/// </summary>
		/// <param name="lfdNR">Number of report on IHK servers</param>
		/// <returns>The <see cref="CommentResult"/> of fetching the comment</returns>
		public async Task<CommentResult> GetCommentFromReport(int? lfdNR)
		{
			if (!lfdNR.HasValue || lfdNR < 0)
				return new CommentResult(CommentResult.ResultStatus.NoLfdnr);
			if (!LoggedIn && !await DoLogin())
				return new CommentResult(CommentResult.ResultStatus.LoginFailed);
			if (!await EnsureReferrer("tibrosBB/azubiHeft.jsp"))
				return new CommentResult(CommentResult.ResultStatus.Unauthorized);
			HttpResponseMessage response = await GetAndRefer($"tibrosBB/azubiHeftEditForm.jsp?lfdnr={lfdNR}");
			if (!response.IsSuccessStatusCode)
				return new CommentResult(CommentResult.ResultStatus.OpenReportFailed);

			HtmlDocument doc = new HtmlDocument(await response.Content.ReadAsStringAsync());
			await GetAndRefer("tibrosBB/azubiHeft.jsp");
			ResetTimer();

			return GetComment(doc);
		}

		/// <summary>
		/// Searches <paramref name="doc"/> for the supervisors' comment
		/// </summary>
		/// <param name="doc"><see cref="HtmlDocument"/> to search for a comment</param>
		/// <returns><see cref="CommentResult"/> of finding the comment</returns>
		private CommentResult GetComment(HtmlDocument doc)
		{
			List<HtmlElement> list = doc.Body.CSSSelect("div.noc_table > div.row > div");
			int commentIndex = 2 * (int)EditFormInfoFields.Comment + 1;
			if (list.Count < commentIndex)
				return new CommentResult(CommentResult.ResultStatus.CommentFieldNotFound);
			return new CommentResult(CommentResult.ResultStatus.Success, comment: list[commentIndex].InnerText);
		}

		/// <summary>
		/// Indexes of edit form info fields
		/// (are in pairs of 2)
		/// </summary>
		public enum EditFormInfoFields
		{
			/// <summary>
			/// Index of azubi name
			/// </summary>
			Name,
			/// <summary>
			/// Index of azubi number
			/// </summary>
			AzubiNumber,
			/// <summary>
			/// Index of job title
			/// </summary>
			JobTitle,
			/// <summary>
			/// Index of span where the contract is binding
			/// </summary>
			ContractSpan,
			/// <summary>
			/// Index of report status
			/// </summary>
			Status,
			/// <summary>
			/// Index of date the report was accepted
			/// </summary>
			AcceptDate,
			/// <summary>
			/// Index of comment
			/// </summary>
			Comment
		}

		/// <summary>
		/// Makes sure the referer is set to <paramref name="path"/>
		/// </summary>
		/// <param name="path">Relative path</param>
		private async Task<bool> EnsureReferrer(string path)
		{
			if (HttpClient.DefaultRequestHeaders.Referrer == new Uri(HttpClient.BaseAddress, path))
				return true;
			try
			{
				await GetAndRefer(path);
				return true;
			}
			catch (HttpRequestException)
			{
				return false;
			}
		}

		/// <summary>
		/// Generates an <see cref="HtmlDocument"/> from an HTML string
		/// </summary>
		/// <param name="html">HTML string to parse</param>
		/// <returns>Parsed <see cref="HtmlDocument"/></returns>
		private HtmlDocument GetHtmlDocument(string html)
		{
			HtmlDocument result = null;
			Thread browserThread = new Thread(() =>
			{
				System.Windows.Forms.WebBrowser browser = new System.Windows.Forms.WebBrowser();
				browser.ScriptErrorsSuppressed = true;
				browser.DocumentText = html;
				browser.Document.OpenNew(true);
				browser.Document.Write(html);
				browser.Refresh();
				result = new HtmlDocument(browser.Document);
			});
			browserThread.SetApartmentState(ApartmentState.STA);
			browserThread.Start();
			browserThread.Join();
			return result;
		}
	}
}
