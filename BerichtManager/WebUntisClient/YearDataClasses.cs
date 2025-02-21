namespace BerichtManager.WebUntisClient
{
	[Serializable]
	public class YearData
	{
		public CurrentSchoolYear? currentSchoolYear { get; set; }
		public object? departments { get; set; }
		public bool? isPlayground { get; set; }
		public MessengerData? messengerData { get; set; }
		public OneDriveData? oneDriveData { get; set; }
		public Tenant? tenant { get; set; }
		public bool? ui2020 { get; set; }
		public User? user { get; set; }
		public List<string>? permissions { get; set; }
		public List<string>? settings { get; set; }
		public List<object>? pollingJobs { get; set; }
	}

	[Serializable]
	public class CurrentSchoolYear
	{
		public DateRange? dateRange { get; set; }
		public int? id { get; set; }
		public string? name { get; set; }
	}

	[Serializable]
	public class DateRange
	{
		public string? start { get; set; }
		public string? end { get; set; }
	}

	[Serializable]
	public class MessengerData
	{
		public bool? hasMessenger { get; set; }
		public object? organizationId { get; set; }
		public object? serviceUrl { get; set; }
	}

	[Serializable]
	public class OneDriveData
	{
		public bool? hasOneDriveRight { get; set; }
		public string? oneDriveClientId { get; set; }
	}

	[Serializable]
	public class Tenant
	{
		public string? displayName { get; set; }
		public int? id { get; set; }
		public object? wuHostName { get; set; }
	}

	[Serializable]
	public class Permissions
	{
		public List<string>? views { get; set; }
	}

	[Serializable]
	public class Person
	{
		public string? displayName { get; set; }
		public int? id { get; set; }
		public object? imageUrl { get; set; }
	}

	[Serializable]
	public class User
	{
		public int? id { get; set; }
		public string? locale { get; set; }
		public string? name { get; set; }
		public Permissions? permissions { get; set; }
		public Person? person { get; set; }
		public List<string>? roles { get; set; }
		public List<object>? students { get; set; }
	}

	[Serializable]
	public class Holidays
	{
		public string? jsonrpc { get; set; }
		public string? id { get; set; }
		public List<HolidayEntrys>? result { get; set; }
	}

	[Serializable]
	public class HolidayEntrys
	{
		public int? id { get; set; }
		public string? name { get; set; }
		public string? longName { get; set; }
		public int? startDate { get; set; }
		public int? endDate { get; set; }
	}
}
