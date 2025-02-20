namespace BerichtManager.HelperClasses.HtmlClasses.CSSSelectors
{
	/// <summary>
	/// Class for CSS selectors following a ">"
	/// </summary>
	internal class ChildSelector : BaseSelector
	{
		/// <summary>
		/// Creates a new <see cref="ChildSelector"/> object
		/// </summary>
		/// <inheritdoc cref="BaseSelector(string, BaseSelector, HtmlElement)" path="/param"/>
		public ChildSelector(string selector, BaseSelector next, HtmlElement root) : base(selector, next, root)
		{

		}

		protected override List<HtmlElement> SelectElements(HtmlElement element)
		{
			return element.Children;
		}
	}
}
