using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BerichtManager.HelperClasses.HtmlClasses.CSSSelectors
{
	/// <summary>
	/// Base class for CSS selectors
	/// </summary>
	internal abstract class BaseSelector
	{
		/// <summary>
		/// <see cref="Regex"/> string used to match title
		/// </summary>
		protected string TagRegex { get; } = @"^.+?(?=\.| |$|\n|#)";
		/// <summary>
		/// <see cref="Regex"/> string used to match classes
		/// </summary>
		/// Old @"(?<Class>(?<=\.)[a-zA-z0-9]+?)(?=(\.|#|$|\n| ))"
		protected string ClassesRegex { get; } = @"(?<=\.).+?(?=\.| |$|\n|#)";
		/// <summary>
		/// <see cref="Regex"/> string used to match ids
		/// </summary>
		protected string IDRegex { get; } = @"(?<=#).+?(?=\.|$|\n| |#)";
		/// <summary>
		/// CSS string to search with
		/// </summary>
		private string Selector { get; }
		/// <summary>
		/// Root <see cref="HtmlElement"/> to search if start of chain
		/// </summary>
		private HtmlElement Root { get; set; }
		/// <summary>
		/// Next <see cref="BaseSelector"/> in chain
		/// </summary>
		private BaseSelector Next { get; set; }

		/// <summary>
		/// Creates a new <see cref="BaseSelector"/> object
		/// </summary>
		/// <param name="selector">The part of the CSS selector to be handled</param>
		/// <param name="next">Next <see cref="BaseSelector"/> in the chain</param>
		/// <param name="root">Root <see cref="HtmlElement"/> to search</param>
		public BaseSelector(string selector, BaseSelector next, HtmlElement root)
		{
			Selector = selector;
			Next = next;
			Root = root;
		}

		/// <summary>
		/// Selects which collection of child <see cref="HtmlElement"/>s from <paramref name="element"/> to use
		/// </summary>
		/// <param name="element"><see cref="HtmlElement"/> to select a <see cref="List{T}"/> of future inputs from</param>
		/// <returns><see cref="List{T}"/> of <see cref="HtmlElement"/>s from <paramref name="element"/> to use</returns>
		protected abstract List<HtmlElement> SelectElements(HtmlElement element);

		/// <summary>
		/// Starts the CSS search
		/// </summary>
		/// <param name="elements"><see cref="List{T}"/> of <see cref="HtmlElement"/>s to search</param>
		/// <returns><see cref="List{T}"/> of <see cref="HtmlElement"/>s that match <see cref="Selector"/></returns>
		public List<HtmlElement> Search(List<HtmlElement> elements)
		{
			if (Root != null)
				if (Next == null)
					return SearchElements(SelectElements(Root));
				else
					return Next.Search(SearchElements(SelectElements(Root)));

			List<HtmlElement> result = new List<HtmlElement>();

			foreach (var element in elements)
			{
				List<HtmlElement> elemResults = SearchElements(SelectElements(element));
				if (Next == null)
					result.AddRange(elemResults);
				else
					result.AddRange(Next.Search(elemResults));
			}

			return result;
		}

		/// <summary>
		/// Searches all <see cref="HtmlElement"/>s in <paramref name="elements"/> for elements matching <see cref="Selector"/>
		/// </summary>
		/// <returns><see cref="List{T}"/> of <see cref="HtmlElement"/>s matching <see cref="Selector"/></returns>
		private List<HtmlElement> SearchElements(List<HtmlElement> elements)
		{
			string tag = Regex.Match(Selector, TagRegex, RegexOptions.ExplicitCapture | RegexOptions.Multiline, new TimeSpan(0, 1, 0)).Value;
			List<string> classes = new List<string>();
			foreach (Match match in Regex.Matches(Selector, ClassesRegex, RegexOptions.ExplicitCapture | RegexOptions.Multiline, new TimeSpan(0, 1, 0)))
			{
				if (match.Success)
					classes.Add(match.Value);
			}
			string id = Regex.Match(Selector, IDRegex, RegexOptions.ExplicitCapture | RegexOptions.Multiline, new TimeSpan(0, 1, 0)).Value;
			if (string.IsNullOrEmpty(id))
				id = null;

			List<HtmlElement> result = new List<HtmlElement>();
			foreach (HtmlElement element in elements)
			{
				if (element.Tag.ToLower() != tag.ToLower() && !string.IsNullOrEmpty(tag) && tag != "*")
					continue;
				bool hasAllClasses = true;
				classes.ForEach(_class => hasAllClasses &= element.Classes.Contains(_class));
				if (!hasAllClasses || id != element.ID && id != null)
					continue;

				result.Add(element);
			}
			return result;
		}
	}
}
