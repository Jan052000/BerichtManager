using Newtonsoft.Json;

namespace BerichtManager.IHKClient
{
	internal class ReportContent
	{
		/// <summary>
		/// Token of report sent by IHK
		/// </summary>
		[JsonProperty("token")]
		[IHKFormDataName("token")]
		public string Token { get; set; }
		/// <summary>
		/// Number of report on IHK servers
		/// </summary>
		[JsonProperty("lfdnr")]
		[IHKFormDataName("lfdnr")]
		public string LfdNR { get; set; }
		[JsonProperty("edtvon")]
		[IHKFormDataName("edtvon")]
		public string StartDate { get; set; }
		[JsonProperty("edtbis")]
		[IHKFormDataName("edtbis")]
		public string EndDate { get; set; }
		[JsonProperty("ausbabschnitt")]
		[IHKFormDataName("ausbabschnitt")]
		public string JobField { get; set; }
		[JsonProperty("ausbabschnitt")]
		[IHKFormDataName("ausbabschnitt")]
		public string JobFieldOld { get; set; }
		[JsonProperty("ausbMail")]
		[IHKFormDataName("ausbMail")]
		public string SupervisorEMail1 { get; set; }
		[JsonProperty("ausbMail2")]
		[IHKFormDataName("ausbMail2")]
		public string SupervisorEMail2 { get; set; }
		[JsonProperty("ausbinhalt1")]
		[IHKFormDataName("ausbinhalt1")]
		public string JobFieldContent { get; set; }
		[JsonProperty("stdMo")]
		[IHKFormDataName("stdMo")]
		public string Monday { get; set; }
		[JsonProperty("stdDi")]
		[IHKFormDataName("stdDi")]
		public string Tuesday { get; set; }
		[JsonProperty("stdMi")]
		[IHKFormDataName("stdMi")]
		public string Wednesday { get; set; }
		[JsonProperty("stdDo")]
		[IHKFormDataName("stdDo")]
		public string Thursday { get; set; }
		[JsonProperty("stdFr")]
		[IHKFormDataName("stdFr")]
		public string Friday { get; set; }
		[JsonProperty("stdSa")]
		[IHKFormDataName("stdSa")]
		public string Saturday { get; set; }
		[JsonProperty("stdSo")]
		[IHKFormDataName("stdSo")]
		public string Sunday { get; set; }
		[JsonProperty("ausbinhalt2")]
		[IHKFormDataName("ausbinhalt2")]
		public string SeminarsField { get; set; }
		[JsonProperty("ausbinhalt12")]
		[IHKFormDataName("ausbinhalt12")]
		public string SeminarsFieldOld { get; set; }
		[JsonProperty("ausbinhalt3")]
		[IHKFormDataName("ausbinhalt3")]
		public string SchoolField { get; set; }
		[JsonProperty("ausbinhalt13")]
		[IHKFormDataName("ausbinhalt13")]
		public string SchoolFieldOld { get; set; }
		/// <summary>
		/// Seems to be an encoded PDF file
		/// </summary>
		[JsonProperty("file")]
		[IHKFormDataName("file")]
		public byte[] File { get; set; }
	}
}
