using System.Collections.Generic;

namespace BerichtManager.IHKClient
{
	/// <summary>
	/// Class to hold values used to select <see cref="HtmlElement"/>s
	/// </summary>
	public class Selector
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
		/// Creates a new <see cref="Selector"/> object
		/// </summary>
		/// <param name="tagName">Tag name of <see cref="HtmlElement"/></param>
		/// <param name="classes">Class names of <see cref="HtmlElement"/></param>
		public Selector(string tagName, List<string> classes)
		{
			TagName = tagName;
			Classes = classes;
		}
	}
}
