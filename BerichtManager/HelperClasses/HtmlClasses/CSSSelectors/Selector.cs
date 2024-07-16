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
		/// <param name="selector">The part of the CSS selector to be handled</param>
		/// <param name="next">Next <see cref="BaseSelector"/> in the chain</param>
		/// <param name="root">Root <see cref="HtmlElement"/> to search</param>
		public Selector(string selector, BaseSelector next, HtmlElement root) : base(selector)
		{
			Next = next;
			Root = root;
		}

		public override List<HtmlElement> Search(List<HtmlElement> elements)
		{
			if (Root != null)
				if (Next == null)
					return SearchElements(Root.All);
				else
					return Next.Search(SearchElements(Root.All));

			List<HtmlElement> result = new List<HtmlElement>();

			foreach (var element in elements)
			{
				if (Next == null)
					result.AddRange(SearchElements(element.All));
				else
					result.AddRange(Next.Search(SearchElements(element.All)) ?? new List<HtmlElement>());
			}

			return result;
		}
	}
}
