namespace BerichtManager.WordTemplate
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	internal class FieldsTypeAttribute : Attribute
	{
		public Type FieldType { get; }

		public FieldsTypeAttribute(Type fieldType)
		{
			FieldType = fieldType;
		}
	}
}
