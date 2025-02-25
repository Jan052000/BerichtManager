namespace BerichtManager.IHKClient.Exceptions
{
	/// <summary>
	/// <see cref="Exception"/> in case there is a mismatch between IHK suggested start date and actual report start date
	/// </summary>
	internal class StartDateMismatchException : Exception
	{
		/// <summary>
		/// Creates a new <see cref="StartDateMismatchException"/> 
		/// </summary>
		/// <param name="suggestedStartDate">Start date as suggested by IHK report creation</param>
		/// <param name="actualStartDate">Start date of actual report</param>
		public StartDateMismatchException(string suggestedStartDate, string actualStartDate) : base($"StartDate {actualStartDate} does not match IHK StartDate {suggestedStartDate}")
		{
			
		}
	}
}
