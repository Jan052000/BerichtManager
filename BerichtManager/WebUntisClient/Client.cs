using BerichtManager.Config;
using BerichtManager.OwnControls;
using BerichtManager.ThemeManagement;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
		private string Server { get; set; }
		/// <summary>
		/// Name of school in clear text
		/// </summary>
		private string SchoolName { get; set; }
		public Client()
		{
			UpdateConfigData();
		}

		/// <summary>
		/// Updates the <see cref="Server"/> and <see cref="SchoolName"/> variables
		/// </summary>
		private void UpdateConfigData()
		{
			//Get school and server
			SchoolName = ConfigHandler.SchoolName();
			Server = ConfigHandler.WebUntisServer();
		}

		/// <summary>
		/// Requests time table from WebUntis server
		/// </summary>
		/// <returns>List containing names of classes in time table</returns>
		public List<string> GetClassesFromWebUntis()
		{
			if (!ConfigHandler.UseWebUntis()) return new List<string>();

			UpdateConfigData();

			List<string> classes = new List<string>();

			//Enable cookies to get JSESSION token from WebUntis server
			HttpClientHandler httpClientHandler = new HttpClientHandler();
			httpClientHandler.CookieContainer = new CookieContainer();
			httpClientHandler.UseCookies = true;
			HttpClient client = new HttpClient(httpClientHandler);
			HttpResponseMessage responseMessage;
			string jSpringURL = "https://" + Server + ".webuntis.com/WebUntis/j_spring_security_check";

			//Generate Headers and Login
			Config.User user = null;
			if (ConfigHandler.StayLoggedIn())
			{
				//Obtain JSessionId
				Dictionary<string, string> content = new Dictionary<string, string>()
				{
					{ "school", SchoolName },
					{ "j_username", ConfigHandler.WebUntisUsername() },
					{ "j_password", ConfigHandler.WebUntisPassword() },
					{ "token", "" }
				};
				responseMessage = client.PostAsync(jSpringURL, new FormUrlEncodedContent(content)).Result;
			}
			else
			{
				user = ConfigHandler.DoWebUntisLogin();
				if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
				{
					ThemedMessageBox.Show(ThemeManager.Instance.ActiveTheme, "You need to login to automatically enter classes");
					return classes;
				}
				else
				{
					Dictionary<string, string> content = new Dictionary<string, string>()
					{
						{ "school", SchoolName },
						{ "j_username", user.Username },
						{ "j_password", user.Password },
						{ "token", "" }
					};
					responseMessage = client.PostAsync(jSpringURL, new FormUrlEncodedContent(content)).Result;
				}
			}
			//HttpResponseMessage responseMessage = client.GetAsync("https://borys.webuntis.com/WebUntis/j_spring_security_check?school=pictorus-bk&j_username=" + configHandler.LoadUsername() + "&j_password=" + configHandler.LoadPassword()).Result;

			//Set cookie data
			string newCookie = "schoolname=\"_" + System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(SchoolName)) + "\"; ";
			if (responseMessage.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> setCookies))
			{
				List<string> cookieHeaders = new List<string>();
				IEnumerator<string> cookieEnumerator = setCookies.GetEnumerator();
				while (cookieEnumerator.MoveNext())
				{
					if (cookieEnumerator.Current.Contains("traceId"))
						cookieHeaders = cookieEnumerator.Current.Split(';').ToList<string>();
				}
				cookieHeaders.ForEach(header =>
				{
					if (header.Trim().Contains("traceId"))
						newCookie += header + "; ";
				});
			}
			foreach (Cookie cookie in httpClientHandler.CookieContainer.GetCookies(new Uri(jSpringURL)))
			{
				if (cookie.Name.Equals("JSESSIONID"))
					newCookie += cookie.Name + "=" + cookie.Value;
			}
			client.DefaultRequestHeaders.Add("Cookie", newCookie);

			//Obtain Api Key
			string apiKey = client.GetAsync("https://" + Server + ".webuntis.com/WebUntis/api/token/new").Result.Content.ReadAsStringAsync().Result;
			if (AuthenticationHeaderValue.TryParse("Bearer " + apiKey, out AuthenticationHeaderValue authorize))
			{
				client.DefaultRequestHeaders.Authorization = authorize;
			}
			else
			{
				ThemedMessageBox.Show(ThemeManager.Instance.ActiveTheme, "There was an error while logging in\n(if you just entered your login info you should check if they are correct)");
				return new List<string>();
			}

			//Set accept to application/json
			client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Add("Accept", "application/json");

			//Get account data
			responseMessage = client.GetAsync("https://" + Server + ".webuntis.com/WebUntis/api/rest/view/v1/app/data").Result;
			if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
			{
				ThemedMessageBox.Show(ThemeManager.Instance.ActiveTheme, "Your account is unauthorized", "Unauthorized");
				return new List<string>();
			}
			YearData yearData = JsonConvert.DeserializeObject<YearData>(responseMessage.Content.ReadAsStringAsync().Result);

			//Request Timetable for the week
			DateTime baseDate = DateTime.Today;
			string date = baseDate.ToString("yyyy-MM-dd");

			//Check account privilages
			if (!Enum.TryParse<ElementTypes>(yearData.user.roles[0], out ElementTypes elementType))
			{
				ThemedMessageBox.Show(ThemeManager.Instance.ActiveTheme, "Could not resolve the rights your account has\non the WebUntis server of your school", "Unknown account type");
				return new List<string>();
			}

			//Stundenplan api https://borys.webuntis.com/WebUntis/api/public/timetable/weekly/data?elementType=1&elementId=1414&date=2022-09-28&formatId=2
			if (yearData.user.roles.Count > 0)
			{
				responseMessage = client.GetAsync("https://" + Server + ".webuntis.com/WebUntis/api/public/timetable/weekly/data?elementType=" + (int)elementType + "&elementId=" + yearData.user.person.id.ToString() + "&date=" + date + "&formatId=1"/* + pageConfigData.data.selectedFormatId*/).Result;
			}
			else
			{
				ThemedMessageBox.Show(ThemeManager.Instance.ActiveTheme, "Your account does not have the rights to view its timetable", "Insifficient permissions");
				return new List<string>();
			}
			//responseMessage = client.GetAsync("https://" + server + ".webuntis.com/WebUntis/api/public/timetable/weekly/data?elementType=" + configHandler.TableElementType() + "&elementId=" + yearData.user.person.id.ToString() + "&date=" + date + "&formatId=2").Result;
			string jsonResponse = responseMessage.Content.ReadAsStringAsync().Result;

			//Deserialize Root from response
			JObject responseObject = JsonConvert.DeserializeObject<JObject>(jsonResponse);
			Root rootObject = JsonConvert.DeserializeObject<Root>(responseObject.First.First.ToString());
			List<string> classkeys = rootObject.result.data.elementPeriods.Keys.ToList();

			//Register ClassIds to be had that week
			List<int> classids = new List<int>();
			Dictionary<int, bool> cancelled = new Dictionary<int, bool>();
			classkeys.ForEach((k) =>
			{
				rootObject.result.data.elementPeriods[k].ForEach((en) =>
				{
					en.elements.ForEach((id) =>
					{
						if (id.type == 3)
						{
							classids.Add(id.id);

							//Check for cancellation
							if (en.@is.cancelled == true)// || en.@is.substitution)
							{
								if (!cancelled.Keys.Contains(id.id))
								{
									cancelled.Add(id.id, true);
								}
							}
							else
							{
								if (cancelled.Keys.Contains(id.id))
								{
									cancelled[id.id] = false;
								}
								else
								{
									cancelled.Add(id.id, false);
								}
							}
						}
					});
				});
			});

			//Crosscheck ClassIds to Coursenames
			bool useUserPrefix = ConfigHandler.UseUserPrefix();
			rootObject.result.data.elements.ForEach((element) =>
			{
				if (element.type == 3 && classids.Contains(element.id))
				{
					if (useUserPrefix)
					{
						if (cancelled[element.id])
						{
							if (!classes.Contains(ConfigHandler.CustomPrefix() + element.name + "\n\t" + ConfigHandler.CustomPrefix() + "\n"))
							{
								classes.Add(ConfigHandler.CustomPrefix() + element.name + "\n\t" + ConfigHandler.CustomPrefix() + "Ausgefallen\n");
							}
						}
						else
						{
							if (classes.Contains(ConfigHandler.CustomPrefix() + element.name + "\n\t" + ConfigHandler.CustomPrefix() + "Ausgefallen\n"))
							{
								classes.Remove(ConfigHandler.CustomPrefix() + element.name + "\n\t" + ConfigHandler.CustomPrefix() + "Ausgefallen\n");
								classes.Add(ConfigHandler.CustomPrefix() + element.name + "\n\t" + ConfigHandler.CustomPrefix() + "\n");
							}
							else
							{
								classes.Add(ConfigHandler.CustomPrefix() + element.name + "\n\t" + ConfigHandler.CustomPrefix() + "\n");
							}
						}
					}
					else
					{
						if (cancelled[element.id])
						{
							if (!classes.Contains("-" + element.name + "\n\t-\n"))
							{
								classes.Add("-" + element.name + "\n\t-Ausgefallen\n");
							}
						}
						else
						{
							if (classes.Contains("-" + element.name + "\n\t-Ausgefallen\n"))
							{
								classes.Remove("-" + element.name + "\n\t-Ausgefallen\n");
								classes.Add("-" + element.name + "\n\t-\n");
							}
							else
							{
								classes.Add("-" + element.name + "\n\t-\n");
							}
						}
					}
				}
			});

			//Check for Holidays if list empty
			if (classes.Count == 0)
			{
				DateTime thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek + 1);
				DateTime thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
				if (int.TryParse(thisWeekStart.ToString("yyyyMMdd"), out int weekStart)) { }
				if (int.TryParse(thisWeekEnd.ToString("yyyyMMdd"), out int weekEnd)) { }

				Holidays holidays = GetHolidays(client);
				if (holidays.result == null)
				{
					ThemedMessageBox.Show(ThemeManager.Instance.ActiveTheme, "An error has occurred on the web untis server", "Server did not respond");
					return new List<string>();
				}
				holidays.result.ForEach((holiday) =>
				{
					bool isInWeek = (holiday.startDate >= weekStart && holiday.endDate <= weekEnd);
					bool isStarting = (holiday.startDate >= weekStart && holiday.startDate <= weekEnd);
					bool isEnding = (holiday.endDate >= weekStart && holiday.endDate <= weekEnd);
					bool weekInEvent = (holiday.startDate <= weekStart && holiday.endDate >= weekEnd);
					if (isInWeek || isStarting || isEnding || weekInEvent)
					{
						if (ConfigHandler.UseUserPrefix())
							classes.Add(ConfigHandler.CustomPrefix() + holiday.longName + "\n");
						else
							classes.Add("-" + holiday.longName + "\n");
					}
				});
			}

			classes.Sort();
			client.Dispose();
			return classes;
		}

		/// <summary>
		/// Requests public holidays and vacations from WebUntis jsonrpc server
		/// if no client is provided a new client will be created for the method call
		/// </summary>
		/// <param name="useClient"><see cref="HttpClient"/> to be used for requests assumes client is logged in and has cookies and api key set</param>
		/// <returns><see cref="Holidays"/> object if request was successful or null if not</returns>
		private Holidays GetHolidays(HttpClient useClient = null)
		{
			UpdateConfigData();
			//https://untis-sr.ch/wp-content/uploads/2019/11/2018-09-20-WebUntis_JSON_RPC_API.pdf
			HttpClient client;
			IEnumerable<string> id;
			if (useClient != null) client = useClient;
			else
			{
				//Enable cookies to get JSESSION token from WebUntis server
				HttpClientHandler httpClientHandler = new HttpClientHandler();
				httpClientHandler.CookieContainer = new CookieContainer();
				httpClientHandler.UseCookies = true;
				client = new HttpClient(httpClientHandler);
				HttpResponseMessage responseMessage;
				string jSpringURL = "https://" + Server + ".webuntis.com/WebUntis/j_spring_security_check";
				Dictionary<string, string> loginContent;

				//Generate Headers and Login
				if (ConfigHandler.StayLoggedIn())
				{
					//Obtain JSessionId
					loginContent = new Dictionary<string, string>()
					{
						{ "school", SchoolName },
						{ "j_username", ConfigHandler.WebUntisUsername() },
						{ "j_password", ConfigHandler.WebUntisPassword() },
						{ "token", "" }
					};
				}
				else
				{
					Config.User user = ConfigHandler.DoWebUntisLogin();
					if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
					{
						ThemedMessageBox.Show(ThemeManager.Instance.ActiveTheme, "You need to login to automatically enter classes");
						return null;
					}
					else
					{
						loginContent = new Dictionary<string, string>()
						{
							{ "school", SchoolName },
							{ "j_username", user.Username },
							{ "j_password", user.Password },
							{ "token", "" }
						};
					}
				}
				responseMessage = client.PostAsync(jSpringURL, new FormUrlEncodedContent(loginContent)).Result;
				//HttpResponseMessage responseMessage = client.GetAsync("https://borys.webuntis.com/WebUntis/j_spring_security_check?school=pictorus-bk&j_username=" + configHandler.LoadUsername() + "&j_password=" + configHandler.LoadPassword()).Result;

				//Set cookie data
				string newCookie = "schoolname=\"_" + System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(SchoolName)) + "\"; ";
				if (responseMessage.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> setCookies))
				{
					List<string> cookieHeaders = new List<string>();
					IEnumerator<string> cookieEnumerator = setCookies.GetEnumerator();
					while (cookieEnumerator.MoveNext())
					{
						if (cookieEnumerator.Current.Contains("traceId"))
							cookieHeaders = cookieEnumerator.Current.Split(';').ToList<string>();
					}
					cookieHeaders.ForEach(header =>
					{
						if (header.Trim().Contains("traceId"))
							newCookie += header + "; ";
					});
				}
				foreach (Cookie cookie in httpClientHandler.CookieContainer.GetCookies(new Uri(jSpringURL)))
				{
					if (cookie.Name.Equals("JSESSIONID"))
						newCookie += cookie.Name + "=" + cookie.Value;
				}
				client.DefaultRequestHeaders.Add("Cookie", newCookie);

				//Obtain Api Key
				string apiKey = client.GetAsync("https://" + Server + ".webuntis.com/WebUntis/api/token/new").Result.Content.ReadAsStringAsync().Result;
				if (AuthenticationHeaderValue.TryParse("Bearer " + apiKey, out AuthenticationHeaderValue authorize))
				{
					client.DefaultRequestHeaders.Authorization = authorize;
				}
				else
				{
					ThemedMessageBox.Show(ThemeManager.Instance.ActiveTheme, "There was an error while logging in\n(if you just entered your login info you should check if they are correct)");
					return null;
				}
			}

			//https://borys.webuntis.com/WebUntis/jsonrpc.do
			HttpResponseMessage response = client.GetAsync("https://" + Server + ".webuntis.com/WebUntis/jsonrpc.do").Result;
			id = response.Headers.GetValues("requestId");
			if (id.Count() == 0)
				return null;
			JObject cont = new JObject(new JProperty("id", id.ElementAt(0)), new JProperty("method", "getHolidays"), new JProperty("jsonrpc", "2.0"));

			HttpContent content = new StringContent(cont.ToString());
			client.DefaultRequestHeaders.Add("Accept", "application/json");
			HttpResponseMessage response1 = client.PostAsync("https://" + Server + ".webuntis.com/WebUntis/jsonrpc.do", content).Result;
			return JsonConvert.DeserializeObject<Holidays>(response1.Content.ReadAsStringAsync().Result);
		}

		/// <summary>
		/// Requests holidays and vacations from WebUntis jsonrpc server and filters for a specific date
		/// </summary>
		/// <param name="time">Date to filter for</param>
		/// <returns>Contatinated string containing all holidays and vacations for the provided date</returns>
		public string GetHolidaysForDate(DateTime time)
		{
			string str = "";
			DateTime thisWeekStart = time.AddDays(-(int)time.DayOfWeek + 1);
			DateTime thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
			if (int.TryParse(thisWeekStart.ToString("yyyyMMdd"), out int weekStart)) { }
			if (int.TryParse(thisWeekEnd.ToString("yyyyMMdd"), out int weekEnd)) { }

			Holidays holidays = GetHolidays();
			if (holidays.result == null)
				return "";

			holidays.result.ForEach((holiday) =>
			{
				bool isInWeek = (holiday.startDate >= weekStart && holiday.endDate <= weekEnd);
				bool isStarting = (holiday.startDate >= weekStart && holiday.startDate <= weekEnd);
				bool isEnding = (holiday.endDate >= weekStart && holiday.endDate <= weekEnd);
				bool weekInEvent = (holiday.startDate <= weekStart && holiday.endDate >= weekEnd);
				if (isInWeek || isStarting || isEnding || weekInEvent)
				{
					if (ConfigHandler.UseUserPrefix())
						str += ConfigHandler.CustomPrefix() + holiday.longName + "\n";
					else
						str += "-" + holiday.longName + "\n";
				}
			});
			return str;
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
