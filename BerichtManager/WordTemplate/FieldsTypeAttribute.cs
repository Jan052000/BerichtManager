using System;

namespace BerichtManager.WordTemplate
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	internal class FieldsTypeAttribute : Attribute
	{
		public Type FieldType { get; set; }
	}
}
