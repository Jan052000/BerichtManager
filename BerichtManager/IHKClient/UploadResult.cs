namespace BerichtManager.IHKClient
{
	internal class UploadResult
	{
		/// <summary>
		/// <see cref="IHKClient.CreateResults"/> of report
		/// </summary>
		public CreateResults Result { get; set; }
		/// <summary>
		/// Start date as set on IHK servers
		/// </summary>
		public DateTime StartDate { get; set; }
		/// <summary>
		/// lfdnr of report on IHK servers
		/// </summary>
		public int? LfdNR { get; set; }
		/// <summary>
		/// Additional info on result
		/// </summary>
		public string? AdditionalInfo { get; set; }

		/// <summary>
		/// Creates a new <see cref="UploadResult"/> object
		/// </summary>
		/// <param name="result">Result of upload process</param>
		/// <param name="startDate">Start date as set on IHK servers</param>
		public UploadResult(CreateResults result, DateTime startDate, int? lfdnr = null)
		{
			Result = result;
			StartDate = startDate;
			LfdNR = lfdnr;
		}

		/// <summary>
		/// Creates a new <see cref="UploadResult"/> object
		/// </summary>
		/// <param name="result">Result of upload process</param>
		/// <param name="additionalInfo">Additional info</param>
		public UploadResult(CreateResults result, string? additionalInfo = null)
		{
			Result = result;
			StartDate = new DateTime();
			AdditionalInfo = additionalInfo;
		}
	}
}
