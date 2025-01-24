using System;

namespace BerichtManager.WordTemplate
{
	/// <summary>
	/// Supported auto fill fields
	/// </summary>
	public enum Fields
	{
		[FieldsType(FieldType = typeof(string))]
		Name,
		[FieldsType(FieldType = typeof(int))]
		Number,
		[Field("Start date")]
		[FieldsType(FieldType = typeof(DateTime))]
		StartDate,
		[Field("End date")]
		[FieldsType(FieldType = typeof(DateTime))]
		EndDate,
		[FieldsType(FieldType = typeof(int))]
		Year,
		[FieldsType(FieldType = typeof(string))]
		Work,
		[FieldsType(FieldType = typeof(string))]
		Seminars,
		[FieldsType(FieldType = typeof(string))]
		School,
		[Field("Sign date (you)")]
		[FieldsType(FieldType = typeof(DateTime))]
		SignDateYou,
		[Field("Sign date (supervisor)")]
		[FieldsType(FieldType = typeof(DateTime))]
		SignDateSupervisor
	}
}
