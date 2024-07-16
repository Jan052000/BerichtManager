using System.Collections.Generic;

namespace BerichtManager.IHKClient
{
	/// <summary>
	/// Class to hold values used to select <see cref="HtmlElement"/>s
	/// </summary>
	public class CSSSelector
	{
		/// <summary>
		/// Tag name of element
		/// </summary>
		public string TagName { get; set; }
		/// <summary>
		/// List of class names
		/// </summary>
		public List<string> Classes { get; set; }

		/// <summary>
		/// Creates a new <see cref="CSSSelector"/> object
		/// </summary>
		/// <param name="tagName">Tag name of <see cref="HtmlElement"/></param>
		/// <param name="classes">Class names of <see cref="HtmlElement"/></param>
		public CSSSelector(string tagName, List<string> classes)
		{
			TagName = tagName;
			Classes = classes;
		}
	}
}
