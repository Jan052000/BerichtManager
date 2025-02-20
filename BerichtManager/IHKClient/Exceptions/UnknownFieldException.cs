namespace BerichtManager.IHKClient.Exceptions
{
	/// <summary>
	/// Exception for when an unknown field is encountered in html form of report
	/// </summary>
	internal class UnknownFieldException : Exception
	{
		/// <summary>
		/// Creates a new <see cref="UnknownFieldException"/>
		/// </summary>
		/// <param name="fieldName">Name of unknown field</param>
		public UnknownFieldException(string fieldName) : base($"{fieldName} is not a known field of upload form!")
		{

		}
	}
}
