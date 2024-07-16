using System.Collections.Generic;

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
		/// <param name="selector">The part of the CSS selector to be handled</param>
		/// <param name="next">Next <see cref="BaseSelector"/> in the chain</param>
		/// <param name="root">Root <see cref="HtmlElement"/> to search</param>
		public ChildSelector(string selector, BaseSelector next, HtmlElement root) : base(selector)
		{
			Next = next;
			Root = root;
		}

		public override List<HtmlElement> Search(List<HtmlElement> elements)
		{
			if (Root != null)
				if (Next == null)
					return SearchElements(Root.Children);
				else
					return Next.Search(SearchElements(Root.Children));

			List<HtmlElement> result = new List<HtmlElement>();

			foreach (var element in elements)
			{
				if (Next == null)
					result.AddRange(SearchElements(element.Children));
				else
					result.AddRange(Next.Search(SearchElements(element.Children)) ?? new List<HtmlElement>());
			}

			return result;
		}
	}
}
