using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BerichtManager.WebUntisClient
{
	//Reference https://www.newtonsoft.com/json/help/html/serializationattributes.htm

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
		public bool? shift { get; set; }
		public bool? roomSubstitution { get; set; }
		public bool substitution { get; set; }
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
