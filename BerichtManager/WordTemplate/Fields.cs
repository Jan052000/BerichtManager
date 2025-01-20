namespace BerichtManager.WordTemplate
{
	/// <summary>
	/// Supported auto fill fields
	/// </summary>
	public enum Fields
	{
		Name,
		Number,
		[Field("Start date")]
		StartDate,
		[Field("End date")]
		EndDate,
		Year,
		Work,
		Seminars,
		School,
		[Field("Sign date you")]
		SignDateYou,
		[Field("Sign date supervisor")]
		SignDateSupervisor
	}
}
