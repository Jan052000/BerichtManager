using BerichtManager.Config;
using BerichtManager.ResponseClasses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows.Forms;

namespace BerichtManager.AddForm
{
	public partial class Client : Form
	{
		ConfigHandler configHandler = new ConfigHandler();
		public Client()
		{
			InitializeComponent();
		}

		private void btClose_Click(object sender, EventArgs e)
		{
			Close();
		}

		public List<string> getClassesFromWebUntis() 
		{
			List<string> classes = new List<string>();
			HttpClient client = new HttpClient();

			//Generate Headers and Login

			if (string.IsNullOrEmpty(configHandler.LoadUsername()) || string.IsNullOrEmpty(configHandler.LoadPassword())) 
			{
				if (!configHandler.doLogin()) 
				{
					MessageBox.Show("You need to login to automatically enter classes");
					return classes;
				}
			}
			HttpResponseMessage responseMessage = client.GetAsync("https://borys.webuntis.com/WebUntis/j_spring_security_check?school=pictorus-bk&j_username=" + configHandler.LoadUsername() + "&j_password=" + configHandler.LoadPassword()).Result;

			//Obtain Api Key
			string apiKey = client.GetAsync("https://borys.webuntis.com/WebUntis/api/token/new").Result.Content.ReadAsStringAsync().Result;
			if (AuthenticationHeaderValue.TryParse("Bearer " + apiKey, out AuthenticationHeaderValue authorize))
				client.DefaultRequestHeaders.Authorization = authorize;

			//Guarantee complete Header
			responseMessage = client.GetAsync("https://borys.webuntis.com/WebUntis/api/rest/view/v1/app/data").Result;
			YearData yearData = JsonConvert.DeserializeObject<YearData>(responseMessage.Content.ReadAsStringAsync().Result);

			//Request Timetable for the week
			DateTime baseDate = DateTime.Today;
			string date = baseDate.ToString("yyyy-MM-dd");

			//Stundenplan api https://borys.webuntis.com/WebUntis/api/public/timetable/weekly/data?elementType=1&elementId=1414&date=2022-09-28&formatId=2
			responseMessage = client.GetAsync("https://borys.webuntis.com/WebUntis/api/public/timetable/weekly/data?elementType=1&elementId=" + yearData.user.person.id.ToString() + "&date=" + date + "&formatId=2").Result;
			string jsonResponse = responseMessage.Content.ReadAsStringAsync().Result;

			//Deserialize Root from response
			JObject testObject = JsonConvert.DeserializeObject<JObject>(jsonResponse);
			Root testRoot = JsonConvert.DeserializeObject<Root>(testObject.First.First.ToString());
			List<string> classkeys = testRoot.result.data.elementPeriods.Keys.ToList();

			//Register ClassIds to be had that week
			List<int> classids = new List<int>();
			Dictionary<int, bool> cancelled = new Dictionary<int, bool>();
			classkeys.ForEach((k) =>
			{
				testRoot.result.data.elementPeriods[k].ForEach((en) =>
				{
					en.elements.ForEach((id) =>
					{
						if (id.type == 3)
						{
							classids.Add(id.id);

							//Chack for cancellation
							if (en.@is.cancelled == true)
							{
								cancelled.Add(id.id, true);
							}
							else
							{
								cancelled.Add(id.id, false);
							}
						}
					});
				});
			});

			//Crosscheck ClassIds to Coursenames
			testRoot.result.data.elements.ForEach((element) =>
			{
				if (element.type == 3 && classids.Contains(element.id))
				{
					if (cancelled[element.id])
					{
						if (!classes.Contains(element.name))
						{
							classes.Add(element.name + "\n\t-Ausgefallen");
						}
					}
					else 
					{
						if (classes.Contains(element.name + "\n\t-Ausgefallen"))
						{
							classes.Remove(element.name + "\n\t-Ausgefallen");
							classes.Add(element.name);
						}
						else 
						{
							classes.Add(element.name);
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

				Holidays holidays = GetHolidays();
				holidays.result.ForEach((holiday) => 
				{
					bool isInWeek = (holiday.startDate >= weekStart && holiday.endDate <= weekEnd);
					bool isStarting = (holiday.startDate >= weekStart && holiday.startDate <= weekEnd);
					bool isEnding = (holiday.endDate >= weekStart && holiday.endDate <= weekEnd);
					bool weekInEvent = (holiday.startDate <= weekStart && holiday.endDate >= weekEnd);
					if (isInWeek || isStarting || isEnding || weekInEvent) 
					{
						classes.Add(holiday.longName);
					}
				});	
			}

			classes.Sort();
			client.Dispose();
			return classes;
		}

		private Holidays GetHolidays() 
		{
			//https://untis-sr.ch/wp-content/uploads/2019/11/2018-09-20-WebUntis_JSON_RPC_API.pdf
			HttpClient client = new HttpClient();

			var trash = client.GetAsync("https://borys.webuntis.com/WebUntis/j_spring_security_check?school=pictorus-bk&j_username=" + configHandler.LoadUsername() + "&j_password=" + configHandler.LoadPassword()).Result;
			//https://borys.webuntis.com/WebUntis/jsonrpc.do
			var response = client.GetAsync("https://borys.webuntis.com/WebUntis/jsonrpc.do").Result;
			var id = response.Headers.GetValues("requestId");

			JObject cont = new JObject(new JProperty("id", id.ToString()), new JProperty("method", "getHolidays"), new JProperty("jsonrpc", "2.0"));

			HttpContent content = new StringContent(cont.ToString());
			client.DefaultRequestHeaders.Add("Accept", "application/json");
			var response1 = client.PostAsync("https://borys.webuntis.com/WebUntis/jsonrpc.do", content).Result;
			return JsonConvert.DeserializeObject<Holidays>(response1.Content.ReadAsStringAsync().Result);
		}

		private void btTest_Click(object sender, EventArgs e)
		{
			List<string> test = getClassesFromWebUntis();
		}
	}

	//Lesen https://www.newtonsoft.com/json/help/html/serializationattributes.htm

	[Serializable]
	public class Courses 
	{
		public int type { get; set; }
		public int id { get; set; }
		public string name { get; set; }
		public string longName { get; set; }
		public string displayname { get; set; }
		public string alternatename { get; set; }
		public bool canViewTimetable { get; set; }
		public int roomCapacity { get; set; }
		public string externKey { get; set; }
	}

	[Serializable]
	public class Classes 
	{
		public int type { get; set; }
		public int id { get; set; }
		public int orgId { get; set; }
		public bool missing { get; set; }
		public string state { get; set; }
	}

	[Serializable]
	public class @is 
	{
		public bool standard { get; set; }
		[JsonProperty("event")]
		public bool @event { get; set; }
		public bool? cancelled { get; set; }
	}

	[Serializable]
	public class Entry 
	{
		public int id { get; set; }
		public int lessonId { get; set; }
		public int lessonNumber { get; set; }
		public string lessonCode { get; set; }
		public string lessonText { get; set; }
		public string periodText { get; set; }
		public bool hasPeriodText { get; set; }
		public string periodInfo { get; set; }
		public List<object> periodAttachments { get; set; }
		public string substText { get; set; }
		public int date { get; set; }
		public int startTime { get; set; }
		public int endTime { get; set; }
		public List<Classes> elements { get; set; }
		public int code { get; set; }
		public string cellState { get; set; }
		public int priority { get; set; }
		[JsonProperty("is")]
		public @is @is { get; set; }
		public int roomCapacity { get; set; }
		public int studentCount { get; set; }
	}

	[Serializable]
	public class Data 
	{
		public bool noDetails { get; set; }
		public List<int> elementIds { get; set; }
		public Dictionary<string, List<Entry>> elementPeriods { get; set; }
		public List<Courses> elements { get; set; }
	}

	[Serializable]
	public class Result 
	{
		public Data data { get; set; }
		public long lastImportTimestamp { get; set; }
	}

	[Serializable]
	public class Root 
	{
		public Result result { get; set; }
	}
}
