namespace BerichtManager.IHKClient.Exceptions
{
	/// <summary>
	/// Exception for when no form element was found in html
	/// </summary>
	internal class NoFormFoundException : Exception
	{
		/// <summary>
		/// Creates a new <see cref="NoFormFoundException"/> exception
		/// </summary>
		public NoFormFoundException() : base("No form was found in html")
		{

		}
	}
}
