using BerichtManager.HelperClasses.HtmlClasses.CSSSelectors;
using System.Diagnostics;

namespace BerichtManager.HelperClasses.HtmlClasses
{
	/// <summary>
	/// Copy of <see cref="System.Windows.Forms.HtmlElement"/>
	/// </summary>
	[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
	public class HtmlElement
	{
		/// <summary>
		/// Name of tag
		/// </summary>
		public string Tag { get; private set; }
		/// <inheritdoc cref="Classes"/>
		private List<string> classes { get; set; }
		/// <summary>
		/// <see cref="List{T}"/> of css classes
		/// </summary>
		public List<string> Classes { get => classes; }
		/// <summary>
		/// Name of element in HTML
		/// </summary>
		public string Name { get; private set; }
		/// <summary>
		/// ID of element in HTML
		/// </summary>
		public string ID { get; private set; }
		/// <summary>
		/// HTML text inside of element
		/// </summary>
		public string InnerHTML { get; private set; }
		/// <summary>
		/// Text inside of element
		/// </summary>
		public string InnerText { get; private set; }
		/// <summary>
		/// HTML text including element
		/// </summary>
		public string OuterHTML { get; private set; }
		private List<HtmlElement> children { get; set; }
		/// <summary>
		/// <see cref="List{T}"/> of child <see cref="HtmlElement"/>
		/// </summary>
		public List<HtmlElement> Children { get => children; }
		/// <summary>
		/// <see cref="List{T}"/> of all input and textarea <see cref="HtmlElement"/> that are enabled
		/// </summary>
		public List<HtmlElement> Inputs { get => GetInputElements(); }
		/// <summary>
		/// <see cref="List{T}"/> of all input and textarea <see cref="HtmlElement"/>
		/// </summary>
		public List<HtmlElement> AllInputs { get => GetInputElements(addDisabled: true); }
		/// <summary>
		/// Wether of not the <see cref="HtmlElement"/> is enabled or not
		/// </summary>
		public bool Enabled { get; private set; }
		/// <summary>
		/// All child <see cref="HtmlElement"/>s including children of children
		/// </summary>
		public List<HtmlElement> All { get => GetAllElements(true); }
		/// <summary>
		/// Value of element
		/// </summary>
		public string Value { get; private set; }
		/// <summary>
		/// Type of element
		/// </summary>
		public string Type { get; private set; }
		/// <summary>
		/// Wether or not element is hidden
		/// </summary>
		public bool IsHidden { get; private set; }

		/// <summary>
		/// Creates a new <see cref="HtmlElement"/> object which contains fields of <paramref name="element"/>
		/// </summary>
		/// <param name="element"><see cref="System.Windows.Forms.HtmlElement"/> to copy</param>
		public HtmlElement(System.Windows.Forms.HtmlElement element)
		{
			ArgumentNullException.ThrowIfNull(element, nameof(element));
			Tag = element.TagName;
			classes = element.GetAttribute("className").Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
			Name = element.Name;
			ID = element.Id;
			InnerText = element.InnerText;
			InnerHTML = element.InnerHtml;
			OuterHTML = element.OuterHtml;
			children = new List<HtmlElement>();
			foreach (System.Windows.Forms.HtmlElement child in element.Children)
			{
				switch (child.TagName.ToLower())
				{
					case "form":
						Children.Add(new HtmlFormElement(child));
						break;
					default:
						Children.Add(new HtmlElement(child));
						break;
				}
			}
			Enabled = !Convert.ToBoolean(element.GetAttribute("disabled"));
			Value = element.GetAttribute("value");
			Type = element.GetAttribute("type").ToLower();
			IsHidden = Type.Contains("hidden", StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Gets a <see cref="List{T}"/> of <see cref="HtmlElement"/> containing all input and textarea <see cref="HtmlElement"/>s
		/// </summary>
		/// <param name="addDisabled"><see langword="true"/> if disabled <see cref="HtmlElement"/>s should be added to <see cref="List{T}"/></param>
		/// <returns><see cref="List{T}"/> of <see cref="HtmlElement"/>s containing all input and textarea <see cref="HtmlElement"/>s</returns>
		private List<HtmlElement> GetInputElements(bool addDisabled = false)
		{
			if ((Tag.ToLower() == "input" || Tag.ToLower() == "textarea") && (Enabled || addDisabled))
				return new List<HtmlElement> { this };
			List<HtmlElement> inputs = new List<HtmlElement>();

			Children.ForEach(child =>
			{
				inputs.AddRange(child.GetInputElements(addDisabled));
			});

			return inputs;
		}

		/// <summary>
		/// Gets all <see cref="HtmlElement"/>s contained within including children of children
		/// </summary>
		/// <param name="isRoot">Root <see cref="HtmlElement"/> will not be added to <see cref="All"/></param>
		/// <returns><see cref="List{T}"/> of all contained <see cref="HtmlElement"/>s</returns>
		private List<HtmlElement> GetAllElements(bool isRoot = false)
		{
			List<HtmlElement> result = new List<HtmlElement>();
			if (!isRoot)
				result.Add(this);

			Children.ForEach(child => result.AddRange(child.GetAllElements()));

			return result;
		}

		/// <summary>
		/// Generates the string representation of <see href="this"/>
		/// </summary>
		/// <returns>The string representation of <see href="this"/></returns>
		protected virtual string GetStringDebuggerDisplay()
		{
			return $"{Tag}{(!string.IsNullOrEmpty(Name) ? $" {Name}" : "")}{(!Enabled ? ", Disabled" : "")}{(Children.Count > 0 ? $", {Children.Count} {(Children.Count == 1 ? "Child" : "Children")}" : "")}";
		}

		/// <summary>
		/// Generates string shown for object in debugger
		/// </summary>
		/// <returns>String representation of <see cref="HtmlElement"/></returns>
		private string GetDebuggerDisplay()
		{
			return $"{{ {GetStringDebuggerDisplay()} }}";
		}

		public List<HtmlElement> GetElementsByTag(string tag, bool ignoreCase = false)
		{
			List<HtmlElement> result = new List<HtmlElement>();
			if (ignoreCase)
			{
				if (Tag.ToLower() == tag.ToLower())
					result.Add(this);
			}
			else
			{
				if (Tag == tag)
					result.Add(this);
			}
			if (Children.Count > 0)
			{
				foreach (HtmlElement child in Children)
				{
					result.AddRange(child.GetElementsByTag(tag, ignoreCase));
				}
			}
			return result;
		}

		/// <summary>
		/// Searches <paramref name="root"/> for all <see cref="HtmlElement"/>s that fit <paramref name="cssSelector"/>
		/// </summary>
		/// <param name="root">Root <see cref="HtmlElement"/> to search</param>
		/// <param name="cssSelector">Selector to use</param>
		/// <returns><see cref="List{T}"/> of matching <see cref="HtmlElement"/>s</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="cssSelector"/> is <see langword="null"/></exception>
		public List<HtmlElement> CSSSelect(string cssSelector)
		{
			if (cssSelector == null)
				throw new ArgumentNullException(nameof(cssSelector));
			return new CSSSelectorChain(cssSelector, this).StartSearch();
		}
	}
}
