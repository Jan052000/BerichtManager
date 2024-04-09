using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using BerichtManager.Config;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using BerichtManager.IHKClient.Exceptions;
using Word = Microsoft.Office.Interop.Word;
using BerichtManager.HelperClasses;

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
		private string BASEURL { get => "https://www.bildung-ihk-nordwestfalen.de/"; }
		/// <summary>
		/// Wether or not the client is logged in
		/// </summary>
		private bool LoggedIn { get; set; } = false;

		public IHKClient()
		{
			HttpClient = new HttpClient(HttpClientHandler);
			HttpClientHandler.CookieContainer = CookieContainer;
			HttpClientHandler.UseCookies = true;
			HttpClient.BaseAddress = new Uri(BASEURL);
			HttpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36 Edg/122.0.0.0");
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
			string path = "tibrosBB/BB_auszubildende.jsp";
			HttpResponseMessage response = await HttpClient.GetAsync(path);
			if (!response.IsSuccessStatusCode)
				return false;
			if (response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> setCookies))
			{
				if (setCookies.Count() == 0)
					return false;
				List<Cookie> cookies = CookieContainer.GetCookies(FromRelativeUri(path)).Cast<Cookie>().ToList();
				if (cookies.Count == 0)
					return false;
				cookies.ForEach(cookie => HttpClient.DefaultRequestHeaders.Add("Cookie", cookie.ToString()));
				HttpClient.DefaultRequestHeaders.Referrer = FromRelativeUri(path);
			}
			return true;
		}

		/// <summary>
		/// Logs the client in to the IHK website
		/// </summary>
		/// <returns><see langword="true"/> if login was successful or client was already logged in and <see langword="false"/> otherwise</returns>
		private async Task<bool> DoLogin()
		{
			if (LoggedIn)
				return true;
			if (!await SetFirstCookie())
				return false;

			string uri = "tibrosBB/azubiHome.jsp";
			string username = ConfigHandler.Instance.IHKUserName();
			string password = ConfigHandler.Instance.IHKPassword();
			Dictionary<string, string> content = new Dictionary<string, string>()
			{
				{ "login", username },
				{ "pass", password },
				{ "anmelden", null }
			};
			HttpResponseMessage response = await HttpClient.PostAsync(uri, new FormUrlEncodedContent(content));
			if (response.StatusCode != HttpStatusCode.Found && !response.IsSuccessStatusCode)
				return false;
			//Get second cookie if recieved
			HttpClient.DefaultRequestHeaders.Remove("Cookie");
			List<Cookie> cookies = CookieContainer.GetCookies(FromRelativeUri(uri)).Cast<Cookie>().ToList();
			cookies.ForEach(cookie => HttpClient.DefaultRequestHeaders.Add("Cookie", cookie.ToString()));

			HttpClient.DefaultRequestHeaders.Referrer = FromRelativeUri(uri);
			return true;
		}

		/// <summary>
		/// Creates and saves the report on IHK online campus
		/// </summary>
		/// <param name="document"><see cref="Word.Document"/> to upload</param>
		/// <returns><see langword="true"/> if creation was successful and <see langword="false"/> if not</returns>
		/// <inheritdoc cref="FillReportContent(Report, HtmlDocument)" path="/exception"/>
		/// <inheritdoc cref="ReportTransformer.WordToIHK(Word.Document)" path="/exception"/>
		public async Task<CreateResults> CreateReport(Word.Document document)
		{
			if (!LoggedIn)
				if (!await DoLogin())
					return CreateResults.Unauthorized;

			//Load list of reports and check session
			HttpResponseMessage response = await GetAndRefer("tibrosBB/azubiHeft.jsp");
			if (response.StatusCode == HttpStatusCode.Unauthorized)
			{
				LoggedIn = false;
				//LoggedIn = await DoLogin();
				return CreateResults.Unauthorized;
			}
			//Get new form from IHK
			response = await PostAndRefer("tibrosBB/azubiHeftEditForm.jsp", new FormUrlEncodedContent(new Dictionary<string, string>() { { "neu", null } }));
			if (!response.IsSuccessStatusCode)
				return CreateResults.CreationFailed;
			//Fill report with contents from new IHK report
			HtmlDocument doc = GetHtmlDocument(await response.Content.ReadAsStringAsync());
			Report report = new Report();
			FillReportContent(report, doc);
			//Overwrite contents from IHK
			ReportTransformer.WordToIHK(document, report);
			MultipartFormDataContent content = GetMultipartFormDataContent(report.ReportContent);
			//Add necessary save parameter IHK needs to save reports
			StringContent save = new StringContent("");
			save.Headers.Remove("Content-Type");
			content.Add(save, "\"save\"");
			//Post content to create report
			response = await PostAndRefer("tibrosBB/azubiHeftAdd.jsp", content);
			if (response.StatusCode != HttpStatusCode.Found && !response.IsSuccessStatusCode)
				return CreateResults.UploadFailed;
			if (response.Headers.Location == null || string.IsNullOrEmpty(response.Headers.Location.ToString()))
				await GetAndRefer("tibrosBB/azubiHeft.jsp");
			else
				await GetAndRefer(response.Headers.Location);
			return CreateResults.Success;
		}

		/// <summary>
		/// Results of the report creation process
		/// </summary>
		public enum CreateResults
		{
			/// <summary>
			/// Creation was successful
			/// </summary>
			Success,
			/// <summary>
			/// Creation failed as session expired
			/// </summary>
			Unauthorized,
			/// <summary>
			/// Creation failed while fetching creation form from IHK
			/// </summary>
			CreationFailed,
			/// <summary>
			/// Creation failed while sending form to IHK
			/// </summary>
			UploadFailed
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
			//IHK always sends an extra field which has nothing to do with the report
			if (inputs.Count - report.ReportContent.GetType().GetProperties().Length > 1)
			{
				foreach (HtmlElement htmlElement in inputs)
				{
					if (report.ReportContent.GetType().GetProperties().ToList().Find(prop =>
					{
						IHKFormDataNameAttribute attr = prop.GetCustomAttributes(typeof(IHKFormDataNameAttribute)).First() as IHKFormDataNameAttribute;
						if (attr == null)
							return false;
						return attr.Name == htmlElement.Name;
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
					IHKFormDataNameAttribute attr = prop.GetCustomAttributes(typeof(IHKFormDataNameAttribute)).First() as IHKFormDataNameAttribute;
					if (attr == null || !attr.IsActuallySent)
					{
#if DEBUG
						Console.WriteLine($"Property {prop.Name} of report has no IHKFormDataNameAttribute");
#endif
						return false;
					}

					return attr.Name == input.Name;
				});
				//If no matches found the field is unknown and new to the form
				if (matchingProps.Count == 0)
					throw new UnknownFieldException(input.Name);
				//Fill props with values from input
				matchingProps.ForEach(prop =>
				{
					switch (input.GetAttribute("type").ToLower())
					{
						case "file":
							//Uploading files is not implemented
							prop.SetValue(report.ReportContent, new byte[0]);
							break;
						default:
							prop.SetValue(report.ReportContent, Convert.ChangeType(input.GetAttribute("value"), prop.PropertyType));
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
		/// <returns><see langword="true"/> if any inputs were found and <see langword="false"/> otherwise</returns>
		private bool FindAllInputInElement(HtmlElement root, out List<HtmlElement> inputs)
		{
			inputs = new List<HtmlElement>();
			foreach (HtmlElement element in root.Children)
			{
				if (element.TagName.Equals("input", StringComparison.CurrentCultureIgnoreCase) || element.TagName.Equals("textarea", StringComparison.CurrentCultureIgnoreCase))
					inputs.Add(element);
				else
				{
					if (FindAllInputInElement(element, out List<HtmlElement> childInputs))
					{
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
		/// Generates an <see cref="HtmlDocument"/> from an HTML string
		/// </summary>
		/// <param name="html">HTML string to parse</param>
		/// <returns>Parsed <see cref="HtmlDocument"/></returns>
		private HtmlDocument GetHtmlDocument(string html)
		{
			WebBrowser browser = new WebBrowser();
			browser.ScriptErrorsSuppressed = true;
			browser.DocumentText = html;
			browser.Document.OpenNew(true);
			browser.Document.Write(html);
			browser.Refresh();
			return browser.Document;
		}
	}
}
