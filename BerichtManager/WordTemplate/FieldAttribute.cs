namespace BerichtManager.WordTemplate
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	internal class FieldAttribute : Attribute
	{
		/// <summary>
		/// Formatted name of value that could be displayed
		/// </summary>
		public string FieldFormattedName { get; set; }

		/// <summary>
		/// Creates a new <see cref="FieldAttribute"/> object
		/// </summary>
		/// <param name="fieldFormattedName"><inheritdoc cref="FieldFormattedName" path="/summary"/></param>
		public FieldAttribute(string fieldFormattedName)
		{
			FieldFormattedName = fieldFormattedName;
		}
	}
}
