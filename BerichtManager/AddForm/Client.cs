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
			//HttpResponseMessage responseMessage = client.GetAsync("https://borys.webuntis.com/WebUntis/j_spring_security_check?school=pictorus-bk&j_username=Username&j_password=Password").Result;
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
			//Stundenplan api https://borys.webuntis.com/WebUntis/api/public/timetable/weekly/data?elementType=1&elementId=1414&date=2022-09-09&formatId=2
			//responseMessage = client.GetAsync("https://borys.webuntis.com/WebUntis/api/public/timetable/weekly/data?elementType=1&elementId=1414&date=" + date + "&formatId=2").Result;
			responseMessage = client.GetAsync("https://borys.webuntis.com/WebUntis/api/public/timetable/weekly/data?elementType=1&elementId=" + yearData.user.person.id.ToString() + "&date=" + date + "&formatId=2").Result;
			string jsonResponse = responseMessage.Content.ReadAsStringAsync().Result;

			//Deserialize Root from response
			JObject testObject = JsonConvert.DeserializeObject<JObject>(jsonResponse);
			Root testRoot = JsonConvert.DeserializeObject<Root>(testObject.First.First.ToString());
			List<string> classkeys = testRoot.result.data.elementPeriods.Keys.ToList();

			//Register ClassIds to be had that week
			List<int> classids = new List<int>();
			classkeys.ForEach((k) =>
			{
				testRoot.result.data.elementPeriods[k].ForEach((en) =>
				{
					en.elements.ForEach((id) =>
					{
						if (id.type == 3)
						{
							classids.Add(id.id);
						}
					});
				});
			});

			//Crosscheck ClassIds to Coursenames
			testRoot.result.data.elements.ForEach((element) =>
			{
				if (element.type == 3 && classids.Contains(element.id))
				{
					classes.Add(element.name);
				}
			});

			classes.Sort();
			client.Dispose();
			return classes;
		}

		private void btTest_Click(object sender, EventArgs e)
		{
			//https://borys.webuntis.com/WebUntis/?school=pictorus-bk#/basic/login
			//https://webuntis.com/

			//https://borys.webuntis.com/WebUntis/api/token/new

			HttpClient client = new HttpClient();

			//Generate Headers
			HttpResponseMessage responseMessage = client.GetAsync("https://borys.webuntis.com/WebUntis/j_spring_security_check?school=pictorus-bk&j_username=Username&j_password=Password").Result;
			Console.Write(responseMessage.Headers.ToString());

			//Obtain Api Key
			string apiKey = client.GetAsync("https://borys.webuntis.com/WebUntis/api/token/new").Result.Content.ReadAsStringAsync().Result;
			if(AuthenticationHeaderValue.TryParse("Bearer " + apiKey, out AuthenticationHeaderValue authorize))
			client.DefaultRequestHeaders.Authorization = authorize;

			//Guarantee complete Header
			responseMessage = client.GetAsync("https://borys.webuntis.com/WebUntis/api/rest/view/v1/app/data").Result;

//			Console.WriteLine("\nFinished");

			DateTime baseDate = DateTime.Today;
			string date = baseDate.ToString("yyyy-MM-dd");

			//Stundenplan api https://borys.webuntis.com/WebUntis/api/public/timetable/weekly/data?elementType=1&elementId=1414&date=2022-09-09&formatId=2
			responseMessage = client.GetAsync("https://borys.webuntis.com/WebUntis/api/public/timetable/weekly/data?elementType=1&elementId=1414&date=" + date + "&formatId=2").Result;
			string jsonResponse = responseMessage.Content.ReadAsStringAsync().Result;

//			Console.WriteLine("\n\n test");

			//Data1 Abschneiden
		//	var l = "{" + jsonResponse.Substring(9, jsonResponse.Length - 11) + "}";
		//	Console.WriteLine(jsonResponse.Substring(19, jsonResponse.Length - 100));

			//Levi lösung
/*			var temp = JsonConvert.DeserializeObject(jsonResponse);
			Result Test1= JsonConvert.DeserializeObject<Result>(jsonstring);
			Result Test2 = JsonConvert.DeserializeObject<Result>(((JObject)temp).First.First.First.First.ToString());
			//	Result test2 = JsonConvert.DeserializeObject<Result>()
			var ts = Test1.data;*/

			//Geht
			JObject testObject = JsonConvert.DeserializeObject<JObject>(jsonResponse);
			Root testRoot = JsonConvert.DeserializeObject<Root>(testObject.First.First.ToString());

			List<string> classes = new List<string>();
			List<string> classkeys = testRoot.result.data.elementPeriods.Keys.ToList();

			//Register ClassIds to be had that week
			List<int> classids = new List<int>();
			classkeys.ForEach((k) => 
			{
				testRoot.result.data.elementPeriods[k].ForEach((en) => 
				{
					en.elements.ForEach((id) => 
					{
						if (id.type == 3) 
						{
							classids.Add(id.id);
						}
					});
				});
			});

			//Crosscheck ClassIds to Coursenames
			testRoot.result.data.elements.ForEach((element) => 
			{
				if (element.type == 3 && classids.Contains(element.id)) 
				{
					classes.Add(element.name);
				}
			});
			classes.Sort();

/*			JsonSerializer serializer = new JsonSerializer();
//			serializer.Formatting = Formatting.Indented;
			serializer.NullValueHandling = NullValueHandling.Ignore;
//			serializer.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full;
			var timetable = serializer.Deserialize<Root>(new JsonTextReader(new StringReader(jsonResponse)));
			Root root = JsonConvert.DeserializeObject<Root>(jsonResponse);


			//Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(test);
			//Data2 data2 = JsonConvert.DeserializeObject<Data2>(test);
*/

			client.Dispose();
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
	public class ElementPeriods 
	{
		public List<Entry> _1414;
	}

	[Serializable]
	public class Data 
	{
		public bool noDetails { get; set; }
		public List<int> elementIds { get; set; }
		//public ElementPeriods elementPeriods { get; set; }
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



	// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
	/*public class Lesson
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
		public List<Element> elements { get; set; }
		public int code { get; set; }
		public string cellState { get; set; }
		public int priority { get; set; }
		public Is @is { get; set; }
		public int roomCapacity { get; set; }
		public int studentCount { get; set; }
	}

	public class Data
	{
		public Result result { get; set; }
		public bool noDetails { get; set; }
		public List<int> elementIds { get; set; }
		public ElementPeriods elementPeriods { get; set; }
		public List<Element> elements { get; set; }
	}

	public class Element
	{
		public int type { get; set; }
		public int id { get; set; }
		public int orgId { get; set; }
		public bool missing { get; set; }
		public string state { get; set; }
		public string name { get; set; }
		public string longName { get; set; }
		public string displayname { get; set; }
		public string alternatename { get; set; }
		public bool canViewTimetable { get; set; }
		public int roomCapacity { get; set; }
		public string externKey { get; set; }
	}

	public class ElementPeriods
	{
		public List<Lesson> _1414 { get; set; }
	}

	public class Is
	{
		public bool standard { get; set; }
		public bool @event { get; set; }
	}

	public class Result
	{
		public Data data { get; set; }
		public long lastImportTimestamp { get; set; }
	}

	public class Root
	{
		public Data data { get; set; }
	}*/

	//[Serializable]
	
}
