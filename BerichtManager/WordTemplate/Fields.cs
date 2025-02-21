namespace BerichtManager.WordTemplate
{
	/// <summary>
	/// Supported auto fill fields
	/// </summary>
	public enum Fields
	{
		[FieldsType(typeof(string))]
		Name,
		[FieldsType(typeof(int))]
		Number,
		[Field("Start date")]
		[FieldsType(typeof(DateTime))]
		StartDate,
		[Field("End date")]
		[FieldsType(typeof(DateTime))]
		EndDate,
		[FieldsType(typeof(int))]
		Year,
		[FieldsType(typeof(string))]
		Work,
		[FieldsType(typeof(string))]
		Seminars,
		[FieldsType(typeof(string))]
		School,
		[Field("Sign date (you)")]
		[FieldsType(typeof(DateTime))]
		SignDateYou,
		[Field("Sign date (supervisor)")]
		[FieldsType(typeof(DateTime))]
		SignDateSupervisor
	}
}
