using System;

namespace BerichtManager.IHKClient
{
	internal class UploadResult
	{
		/// <summary>
		/// <see cref="IHKClient.CreateResults"/> of report
		/// </summary>
		public IHKClient.CreateResults Result { get; set; }
		/// <summary>
		/// Start date as set on ihk servers
		/// </summary>
		public DateTime StartDate { get; set; }

		/// <summary>
		/// Creates a new <see cref="UploadResult"/> object
		/// </summary>
		/// <param name="result">Result of upload process</param>
		/// <param name="startDate">Start date as set on ihk servers</param>
		public UploadResult(IHKClient.CreateResults result, DateTime startDate)
		{
			Result = result;
			StartDate = startDate;
		}

		/// <summary>
		/// Creates a new <see cref="UploadResult"/> object
		/// </summary>
		/// <param name="result">Result of upload process</param>
		public UploadResult(IHKClient.CreateResults result)
		{
			Result = result;
			StartDate = new DateTime();
		}
	}
}
