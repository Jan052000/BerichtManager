namespace BerichtManager.WordTemplate.Exceptions
{
	public class UnknownFieldTypeException : Exception
	{
		public UnknownFieldTypeException(Type type) : base($"Missing a type converter for type: {type.Name}!")
		{

		}
	}
}
