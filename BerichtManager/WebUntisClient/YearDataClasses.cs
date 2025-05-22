using BerichtManager.HelperClasses;
using System.Globalization;

namespace BerichtManager.WebUntisClient
{
	[Serializable]
	public class YearData
	{
		public CurrentSchoolYear? currentSchoolYear { get; set; }
		public object? departments { get; set; }
		public List<NewDataHoliday>? holidays { get; set; }
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
	public class NewDataHoliday
	{
		public bool? bookable { get; set; }
		public string? end { get; set; }
		public int? id { get; set; }
		public string? name { get; set; }
		public string? start { get; set; }
		public bool DateIsInHoliday(DateTime? date)
		{
			if (date == null)
				return false;
			if (!DateTime.TryParse(start, out DateTime startDate) || !DateTime.TryParse(end, out DateTime endDate))
				return false;
			return date >= startDate && date <= endDate;
		}

		public bool HolidayIsRelevantForDate(DateTime? date)
		{
			if (date is not DateTime _date || date == new DateTime())
				return false;
			if (!DateTime.TryParse(start, out DateTime startDate) || !DateTime.TryParse(end, out DateTime endDate))
				return false;

			DateTime dateWeekStart = _date.AddDays(-(int)_date.DayOfWeek + 1);
			DateTime dateWeekEnd = dateWeekStart.AddDays(7).AddSeconds(-1);
			bool isInWeek = (startDate >= dateWeekStart && endDate <= dateWeekEnd);
			bool isStarting = (startDate >= dateWeekStart && startDate <= dateWeekEnd);
			bool isEnding = (endDate >= dateWeekStart && endDate <= dateWeekEnd);
			bool weekInEvent = (startDate <= dateWeekStart && endDate >= dateWeekEnd);

			return isInWeek || isStarting || isEnding || weekInEvent;
		}
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
		public bool DateIsInHoliday(DateTime? date)
		{
			if (date == null)
				return false;
			if (!DateTime.TryParseExact(startDate.ToString(), DateTimeUtils.OLDWEBUNTISHOLIDAYDATEFORMAT, CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault, out DateTime start) ||
				!DateTime.TryParseExact(endDate.ToString(), DateTimeUtils.OLDWEBUNTISHOLIDAYDATEFORMAT, CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault, out DateTime end))
				return false;
			return date >= start && date <= end;
		}

		public bool HolidayIsRelevantForDate(DateTime? date)
		{
			if (date is not DateTime _date || date == new DateTime())
				return false;
			if (!DateTime.TryParseExact(startDate.ToString(), DateTimeUtils.OLDWEBUNTISHOLIDAYDATEFORMAT, CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault, out DateTime start) ||
				!DateTime.TryParseExact(endDate.ToString(), DateTimeUtils.OLDWEBUNTISHOLIDAYDATEFORMAT, CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault, out DateTime end))
				return false;

			DateTime dateWeekStart = _date.AddDays(-(int)_date.DayOfWeek + 1);
			DateTime dateWeekEnd = dateWeekStart.AddDays(7).AddSeconds(-1);
			bool isInWeek = (start >= dateWeekStart && end <= dateWeekEnd);
			bool isStarting = (start >= dateWeekStart && start <= dateWeekEnd);
			bool isEnding = (end >= dateWeekStart && end <= dateWeekEnd);
			bool weekInEvent = (start <= dateWeekStart && end >= dateWeekEnd);

			return isInWeek || isStarting || isEnding || weekInEvent;
		}
	}
}
