namespace BerichtManager.IHKClient.Exceptions
{
	internal class NoInputsFoundException : Exception
	{
		public NoInputsFoundException() : base("No inputs were found in form")
		{

		}
	}
}
