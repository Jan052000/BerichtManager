using Newtonsoft.Json;

namespace BerichtManager.IHKClient.ReportContents
{
	internal class ReportAddContent : ReportContent
	{
		[JsonProperty("save")]
		[IHKFormDataName("save", FormOrder = 22)]
		public string Save { get; set; }

		public ReportAddContent(ReportContent content)
		{
			Token = content.Token;
			LfdNR = content.LfdNR;
			StartDate = content.StartDate;
			EndDate = content.EndDate;
			JobField = content.JobField;
			SupervisorEMail1 = content.SupervisorEMail1;
			SupervisorEMail2 = content.SupervisorEMail2;
			JobFieldContent = content.JobFieldContent;
			JobFieldOld = content.JobFieldOld;
			Monday = content.Monday;
			Tuesday = content.Tuesday;
			Wednesday = content.Wednesday;
			Thursday = content.Thursday;
			Friday = content.Friday;
			Saturday = content.Saturday;
			Sunday = content.Sunday;
			SeminarsField = content.SeminarsField;
			SeminarsFieldOld = content.SeminarsFieldOld;
			SchoolField = content.SchoolField;
			SchoolFieldOld = content.SchoolFieldOld;
			File = content.File;
		}
	}
}
