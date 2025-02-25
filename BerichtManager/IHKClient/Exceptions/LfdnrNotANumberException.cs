namespace BerichtManager.IHKClient
{
	internal class LfdnrNotANumberException : Exception
	{
		public LfdnrNotANumberException() : base("lfdnr as recieved from IHK server could not be parsed")
		{

		}
	}
}
