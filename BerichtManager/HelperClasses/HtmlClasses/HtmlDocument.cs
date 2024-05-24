using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace BerichtManager.HelperClasses.HtmlClasses
{
	/// <summary>
	/// Copy of <see cref="System.Windows.Forms.HtmlDocument"/>
	/// </summary>
	[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
	internal class HtmlDocument
	{
		/// <summary>
		/// <see cref="HtmlElement"/> representation of <see cref="System.Windows.Forms.HtmlDocument.Body"/>
		/// </summary>
		public HtmlElement Body { get; set; }
		/// <summary>
		/// Count of children copies from <see cref="System.Windows.Forms.HtmlDocument"/>
		/// </summary>
		private int ElementCount { get => Body.All.Count; }
		/// <summary>
		/// Title of <see cref="System.Windows.Forms.HtmlDocument"/>
		/// </summary>
		private string Title { get; set; }
		/// <summary>
		/// List of forms in <see cref="Body"/>
		/// </summary>
		public List<HtmlElement> Forms
		{
			get => GetForms(Body);
		}

		/// <summary>
		/// Creates a new <see cref="HtmlDocument"/> object which contains <see cref="System.Windows.Forms.HtmlDocument"/>
		/// </summary>
		/// <param name="doc"><see cref="System.Windows.Forms.HtmlDocument"/> to clone</param>
		public HtmlDocument(System.Windows.Forms.HtmlDocument doc)
		{
			Init(doc);
		}

		public HtmlDocument(string html)
		{
			Thread browserThread = new Thread(() =>
			{
				System.Windows.Forms.WebBrowser browser = new System.Windows.Forms.WebBrowser();
				browser.ScriptErrorsSuppressed = true;
				browser.DocumentText = html;
				browser.Document.OpenNew(true);
				browser.Document.Write(html);
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
			Body = new HtmlElement(doc.Body);
			Title = doc.Title;
		}

		/// <summary>
		/// Gets a <see cref="List{T}"/> of forms in <paramref name="root"/>
		/// </summary>
		/// <param name="root"><see cref="HtmlElement"/> to search for forms</param>
		/// <returns><see cref="List{T}"/> containing all form tags in <paramref name="root"/></returns>
		private List<HtmlElement> GetForms(HtmlElement root)
		{
			List<HtmlElement> forms = new List<HtmlElement>();

			if (root.Tag.ToLower() == "form")
				forms.Add(root);
			root.Children.ForEach(child => forms.AddRange(GetForms(child)));

			return forms;
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
