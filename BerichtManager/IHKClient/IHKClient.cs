using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using BerichtManager.Config;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;

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
			HttpResponseMessage response = await HttpClient.PostAsync(uri, content);
			HttpClient.DefaultRequestHeaders.Referrer = FromRelativeUri(uri);
			return response;
		}

		/// <summary>
		/// Sends an async get request using <see cref="HttpClient"/> and sets the referer to the <paramref name="uri"/> path
		/// </summary>
		/// <param name="content">Content to be posted</param>
		/// <returns><see cref="HttpResponseMessage"/> of request</returns>
		private async Task<HttpResponseMessage> GetAndRefer(string uri)
		{
			HttpResponseMessage response = await HttpClient.GetAsync(uri);
			HttpClient.DefaultRequestHeaders.Referrer = FromRelativeUri(uri);
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
		internal async Task<bool> DoLogin()
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
	}
}
