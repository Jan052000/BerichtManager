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
		/// CSS string to search with
		/// </summary>
		private string Selector { get; }
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
		/// Root <see cref="HtmlElement"/> to search if start of chain
		/// </summary>
		protected HtmlElement Root { get; set; }
		/// <summary>
		/// Next <see cref="BaseSelector"/> in chain
		/// </summary>
		protected BaseSelector Next { get; set; }

		/// <summary>
		/// Creates a new <see cref="BaseSelector"/> object
		/// </summary>
		/// <param name="selector">CSS selector part</param>
		public BaseSelector(string selector)
		{
			Selector = selector;
		}

		/// <summary>
		/// Starts the CSS search
		/// </summary>
		/// <param name="elements"><see cref="List{T}"/> of <see cref="HtmlElement"/>s to search</param>
		/// <returns><see cref="List{T}"/> of <see cref="HtmlElement"/>s that match <see cref="Selector"/></returns>
		public abstract List<HtmlElement> Search(List<HtmlElement> elements);

		/// <summary>
		/// Searches all <see cref="HtmlElement"/>s in <paramref name="elements"/> for elements matching <see cref="Selector"/>
		/// </summary>
		/// <returns><see cref="List{T}"/> of <see cref="HtmlElement"/>s matching <see cref="Selector"/></returns>
		protected List<HtmlElement> SearchElements(List<HtmlElement> elements)
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
