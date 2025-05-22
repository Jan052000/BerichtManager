using BerichtManager.Config;
using BerichtManager.HelperClasses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;

namespace BerichtManager.WebUntisClient
{
	public partial class Client
	{
		/// <summary>
		/// Cache object to reduce number of .Instance in code
		/// </summary>
		private ConfigHandler ConfigHandler { get; } = ConfigHandler.Instance;
		/// <summary>
		/// Server part of the url
		/// </summary>
		private string Server => ConfigHandler.WebUntisServer;
		/// <summary>
		/// Name of school in clear text
		/// </summary>
		private string SchoolName => ConfigHandler.SchoolName;
		/// <summary>
		/// <see cref="System.Net.Http.HttpClient"/> to send requests
		/// </summary>
		private HttpClient HttpClient { get; }
		/// <summary>
		/// <see cref="System.Net.CookieContainer"/> to hold cookies of <see cref="HttpClient"/>
		/// </summary>
		private CookieContainer CookieContainer { get; }
		/// <summary>
		/// <see cref="System.Timers.Timer"/> with an interval of 15m to end login session
		/// </summary>
		private System.Timers.Timer LoginSessionTimeout { get; } = new System.Timers.Timer
		{
			Interval = 900000,
			Enabled = false,
			AutoReset = false
		};
		/// <summary>
		/// Wether or not <see cref="HttpClient"/> is logged in
		/// </summary>
		private bool LoggedIn { get; set; } = false;

		public Client()
		{
			LoginSessionTimeout.Elapsed += LoginSessionTimeoutElapsed;
			HttpClientHandler httpClientHandler = new HttpClientHandler();
			CookieContainer = new CookieContainer();
			httpClientHandler.CookieContainer = CookieContainer;
			httpClientHandler.UseCookies = true;
			httpClientHandler.AllowAutoRedirect = false;
			HttpClient = new HttpClient(httpClientHandler);
			HttpClient.BaseAddress = new Uri($"https://{Server}.webuntis.com");
			ResetDefaultHeaders();
		}

		private void ResetDefaultHeaders()
		{
			HttpClient.DefaultRequestHeaders.Clear();
			HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");
			HttpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36 Edg/122.0.0.0");
		}

		private void LoginSessionTimeoutElapsed(object? sender, System.Timers.ElapsedEventArgs e)
		{
			LoggedIn = false;
		}

		/// <summary>
		/// Loggs <see cref="HttpClient"/> in to WebUntis
		/// </summary>
		/// <returns>Error message or <see langword="null"/> if login was successfu</returns>
		private async Task<string?> DoLogin()
		{
			if (LoggedIn)
			{
				LoginSessionTimeout.Stop();
				LoginSessionTimeout.Start();
				return null;
			}
			ResetDefaultHeaders();
			string userName;
			string password;
			if (ConfigHandler.WebUntisStayLoggedIn)
			{
				userName = ConfigHandler.WebUntisUsername;
				password = ConfigHandler.WebUntisPassword;
			}
			else
			{
				Config.User user = ConfigHandler.DoWebUntisLogin();
				if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
					return "Not logged in";
				userName = user.Username;
				password = user.Password;
			}
			Dictionary<string, string> content = new()
			{
				{ "school", SchoolName },
				{ "j_username", userName },
				{ "j_password", password },
				{ "token", "" }
			};
			string jspring = "/WebUntis/j_spring_security_check";
			HttpResponseMessage response = await HttpClient.PostAsync(jspring, new FormUrlEncodedContent(content));
			if (!response.IsSuccessStatusCode)
				return $"Login failed: {response.StatusCode}";
			foreach (Cookie s in CookieContainer.GetCookies(new Uri(HttpClient.BaseAddress!, jspring)).Cast<Cookie>())
			{
				HttpClient.DefaultRequestHeaders.Add("Cookie", s.ToString());
			}
			string? message = await GetAuthorization();
			LoggedIn = true;
			LoginSessionTimeout.Start();
			return message;
		}

		/// <summary>
		/// Gets and sets the authorization token from WebUntis
		/// </summary>
		/// <returns>Error message or <see langword="null"/> if authorization token was set</returns>
		private async Task<string?> GetAuthorization()
		{
			//Obtain Api Key
			string apiKey = await HttpClient.GetAsync("/WebUntis/api/token/new").Result.Content.ReadAsStringAsync();
			if (!AuthenticationHeaderValue.TryParse("Bearer " + apiKey, out AuthenticationHeaderValue? authorize))
				return "Failed to get Authentorization token";
			HttpClient.DefaultRequestHeaders.Authorization = authorize;
			return null;
		}

		/// <summary>
		/// Gets classes text from WebUntis
		/// </summary>
		/// <param name="date"><see cref="DateTime"/> to get classes for</param>
		/// <returns>Result containing WebUntis classes and an error message if an error was encountered</returns>
		public async Task<(List<string> Classes, string? Error)> GetClassesFromWebUntisAsync(DateTime date)
		{
			(List<string> Classes, string? Error) result = (new List<string>(), null);

			if (await DoLogin() is string message)
			{
				result.Error = message;
				return result;
			}

			var accountData = await GetAccountData();

			if (accountData.Message != null)
			{
				result.Error = accountData.Message;
				return result;
			}

			var timeTable = await GetTimeTable(accountData.AccountType!.Value, accountData.UserId!.Value, date);
			if (timeTable.Message != null)
			{
				result.Error = timeTable.Message;
				return result;
			}
			if (timeTable.Classes.Count > 0)
				result.Classes.AddRange(timeTable.Classes);
			else if (accountData.Holidays?.Count > 0)
			{
				var holidays = accountData.Holidays.Where(hd => hd.HolidayIsRelevantForDate(date) && hd.name != null).Select(hd => $"{ConfigHandler.CustomPrefix}{hd.name}").Cast<string>();
				result.Classes.AddRange(holidays);
			}
			else
			{
				if (await GetHolidaysAsync(date) is string holidays)
					result.Classes.Add(holidays);
			}

			return result;
		}

		/// <summary>
		/// Gets holidays from WebUntis
		/// </summary>
		/// <param name="date"><see cref="DateTime"/> to get holidays for</param>
		/// <returns>Holidays for <paramref name="date"/></returns>
		public async Task<string?> GetHolidaysAsync(DateTime date)
		{
			if (await DoLogin() is string message)
				return null;
			//https://borys.webuntis.com/WebUntis/jsonrpc.do
			HttpResponseMessage response = await HttpClient.GetAsync("/WebUntis/jsonrpc.do");
			string? id = response.Headers.GetValues("requestId").FirstOrDefault();
			if (id == null)
				return null;
			JObject cont = new JObject(new JProperty("id", id), new JProperty("method", "getHolidays"), new JProperty("jsonrpc", "2.0"));

			HttpContent content = new StringContent(cont.ToString());
			response = await HttpClient.PostAsync("/WebUntis/jsonrpc.do", content);
			Holidays? holidays = JsonConvert.DeserializeObject<Holidays>(await response.Content.ReadAsStringAsync());
			if (holidays?.result == null)
				return null;
			return string.Join($"{ConfigHandler.CustomPrefix}\n", holidays.result.Where(hd => hd.HolidayIsRelevantForDate(date)).Select(hd => hd.longName));
		}

		/// <summary>
		/// Gets time table data from WebUntis
		/// </summary>
		/// <param name="accountType">Type of account</param>
		/// <param name="userId">Id of account</param>
		/// <param name="date"><see cref="DateTime"/> to get week time table for</param>
		/// <returns><see cref="List{T}"/> of classes and Error message if an error was encountered or <see langword="null"/> if successful</returns>
		private async Task<(List<string> Classes, string? Message)> GetTimeTable(ElementTypes accountType, int userId, DateTime date)
		{
			(List<string> Classes, string? message) result = (new List<string>(), null);
			//Stundenplan api https://borys.webuntis.com/WebUntis/api/public/timetable/weekly/data?elementType=1&elementId=1414&date=2022-09-28&formatId=2
			HttpResponseMessage responseMessage = await HttpClient.GetAsync($"/WebUntis/api/public/timetable/weekly/data?elementType={(int)accountType}&elementId={userId}&date={date.ToString(DateTimeUtils.WEBUNTISTIMETABLEFORMAT)}&formatId=1"/* + pageConfigData.data.selectedFormatId*/);
			//responseMessage = client.GetAsync("https://" + server + ".webuntis.com/WebUntis/api/public/timetable/weekly/data?elementType=" + configHandler.TableElementType() + "&elementId=" + yearData.user.person.id.ToString() + "&date=" + date + "&formatId=2").Result;
			string jsonResponse = await responseMessage.Content.ReadAsStringAsync();

			//Deserialize Root from response
			TimeTableResponse? res = JsonConvert.DeserializeObject<TimeTableResponse>(jsonResponse);
			if (res?.data?.result?.data?.elementPeriods?.Count == null)
			{
				result.message = "Unable to get time table from WebUntis";
				return result;
			}
			foreach (var kvp in res.data.result.data.elementPeriods)
			{
				foreach (Entry _class in kvp.Value)
				{
					Classes? classes = _class.elements?.FirstOrDefault(en => en.type == 3);
					if (classes?.id == null)
						continue;
					Courses? courses = res.data.result.data.elements?.FirstOrDefault(course => course.id == classes.id);
					if (courses == null)
						continue;
					string classString = $"{ConfigHandler.CustomPrefix}{courses.displayname}\n\t{ConfigHandler.CustomPrefix}\n";
					if (_class.@is?.cancelled == true)
						classString += "Ausgefallen";
					result.Classes.Add(classString);
				}
			}
			return result;
		}

		/// <summary>
		/// Gets account data from WebUntis
		/// </summary>
		/// <returns>Account data, list of holidays if new API and an error message or <see langword="null"/> if successful</returns>
		private async Task<(ElementTypes? AccountType, int? UserId, List<NewDataHoliday>? Holidays, string? Message)> GetAccountData()
		{
			(ElementTypes? AccountType, int? UserId, List<NewDataHoliday>? Holidays, string? Message) result = (null, null, null, null);
			HttpResponseMessage responseMessage = await HttpClient.GetAsync($"/WebUntis/api/rest/view/v1/app/data");
			if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
			{
				result.Message = "Your account is unauthorized";
				return result;
			}
			YearData? yearData = JsonConvert.DeserializeObject<YearData>(await responseMessage.Content.ReadAsStringAsync());
			if (yearData == null)
			{
				result.Message = "Could not parse year data";
				return result;
			}
			if (yearData?.user?.roles?.Count <= 0 || !Enum.TryParse(yearData?.user?.roles?[0], out ElementTypes elementType))
			{
				result.Message = "Could not resolve the rights your account has on the WebUntis server of your school";
				return result;
			}
			result.AccountType = elementType;
			if (yearData?.user?.person?.id == null)
			{
				result.Message = "Could not read user id from recieved account data";
				return result;
			}
			result.Holidays = yearData.holidays;
			result.UserId = yearData.user.person.id;
			return result;
		}

		/// <summary>
		/// Enum for types of WebUntis accounts
		/// </summary>
		public enum ElementTypes
		{
			KLASSE = 1,
			TEACHER = 2,
			SUBJECT = 3,
			ROOM = 4,
			STUDENT = 5
		}
	}
}
