using Newtonsoft.Json;

namespace BerichtManager.IHKClient.ReportContents
{
	internal class ReportCancelContent : ReportContent
	{
        [JsonProperty("cancel")]
        public object Cancel { get; set; }
    }
}
