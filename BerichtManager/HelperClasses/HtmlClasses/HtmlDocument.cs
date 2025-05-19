using System.Diagnostics;

namespace BerichtManager.HelperClasses.HtmlClasses
{
	/// <summary>
	/// Copy of <see cref="System.Windows.Forms.HtmlDocument"/>
	/// </summary>
	[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
	public class HtmlDocument
	{
		/// <summary>
		/// <see cref="HtmlElement"/> representation of <see cref="System.Windows.Forms.HtmlDocument.Body"/>
		/// </summary>
		public HtmlElement? Body { get; set; }
		/// <summary>
		/// Count of children copies from <see cref="System.Windows.Forms.HtmlDocument"/>
		/// </summary>
		private int ElementCount { get => Body?.All?.Count ?? 0; }
		/// <summary>
		/// Title of <see cref="System.Windows.Forms.HtmlDocument"/>
		/// </summary>
		private string? Title { get; set; }
		/// <summary>
		/// List of forms in <see cref="Body"/>
		/// </summary>
		public List<HtmlFormElement> Forms
		{
			get => (Body?.All ?? new List<HtmlElement>()).Where(element => element is HtmlFormElement).Cast<HtmlFormElement>().ToList();
		}

		/// <summary>
		/// Creates a new <see cref="HtmlDocument"/> object which contains <see cref="System.Windows.Forms.HtmlDocument"/>
		/// </summary>
		/// <param name="doc"><see cref="System.Windows.Forms.HtmlDocument"/> to clone</param>
		public HtmlDocument(System.Windows.Forms.HtmlDocument doc)
		{
			Init(doc);
		}

		/// <summary>
		/// Creates a new <see cref="HtmlDocument"/>
		/// </summary>
		/// <param name="html">HTML text of page</param>
		public HtmlDocument(string html)
		{
			Thread browserThread = new Thread(() =>
			{
				WebBrowser browser = new WebBrowser();
				browser.ScriptErrorsSuppressed = true;
				browser.DocumentText = html;
				browser.Document!.OpenNew(true);
				browser.Document!.Write(html);
				browser.Refresh();
				Init(browser.Document);
			});
			browserThread.SetApartmentState(ApartmentState.STA);
			browserThread.Start();
			browserThread.Join();
		}

		/// <summary>
		/// Sets values from <paramref name="doc"/>
		/// </summary>
		/// <param name="doc"><see cref="System.Windows.Forms.HtmlDocument"/> to copy</param>
		private void Init(System.Windows.Forms.HtmlDocument doc)
		{
			Body = new HtmlElement(doc.Body!);
			Title = doc.Title;
		}

		/// <summary>
		/// Generates string shown for object in debugger
		/// </summary>
		/// <returns>String representation of <see cref="HtmlDocument"/></returns>
		private string GetDebuggerDisplay()
		{
			return $"{{ {Title}: {ElementCount} Elements }}";
		}
	}
}
