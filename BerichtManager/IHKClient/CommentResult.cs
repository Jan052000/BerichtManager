namespace BerichtManager.IHKClient
{
	/// <summary>
	/// Class for combined fields for fetch result for IHK report comments
	/// </summary>
	internal class CommentResult
	{
		/// <summary>
		/// Result of fetching comment
		/// </summary>
		public ResultStatus UploadResult { get; set; }
		/// <summary>
		/// Comment fetched
		/// </summary>
		public string? Comment { get; set; }
		/// <summary>
		/// Exception if one was thrown
		/// </summary>
		public Exception? Exception { get; set; }

		/// <summary>
		/// Creates a new <see cref="CommentResult"> object
		/// </summary>
		/// <param name="uploadResult"></param>
		/// <param name="comment"></param>
		public CommentResult(ResultStatus uploadResult, string? comment = null, Exception? exception = null)
		{
			UploadResult = uploadResult;
			Comment = comment;
			Exception = exception;
		}

		public enum ResultStatus
		{
			/// <summary>
			/// Status code for success
			/// </summary>
			Success,
			/// <summary>
			/// Status code for when the lfdnr could not be read
			/// </summary>
			NoLfdnr,
			/// <summary>
			/// Status code for when login failed
			/// </summary>
			LoginFailed,
			/// <summary>
			/// Status code for when login was faulty
			/// </summary>
			Unauthorized,
			/// <summary>
			/// Status code for when unable to open edit form for report
			/// </summary>
			OpenReportFailed,
			/// <summary>
			/// Status code for comment field missing in HTML
			/// </summary>
			CommentFieldNotFound,
			/// <summary>
			/// Status code for when an exception occurred
			/// </summary>
			Exception
		}
	}
}
