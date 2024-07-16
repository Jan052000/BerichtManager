using System.Collections.Generic;

namespace BerichtManager.HelperClasses.HtmlClasses.CSSSelectors
{
	/// <summary>
	/// Class for CSS selectors following a " "
	/// </summary>
	internal class Selector : BaseSelector
	{
		/// <summary>
		/// Creates a new <see cref="Selector"/> object
		/// </summary>
		/// <inheritdoc cref="BaseSelector(string, BaseSelector, HtmlElement)" path="/param"/>
		public Selector(string selector, BaseSelector next, HtmlElement root) : base(selector, next, root)
		{

		}

		protected override List<HtmlElement> SelectElements(HtmlElement element)
		{
			return element.All;
		}
	}
}
