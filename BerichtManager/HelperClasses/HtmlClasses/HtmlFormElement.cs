using BerichtManager.IHKClient;
using BerichtManager.IHKClient.ReportContents;
using System.Reflection;

namespace BerichtManager.HelperClasses.HtmlClasses
{
	/// <summary>
	/// Representation of HTML form element
	/// </summary>
	public class HtmlFormElement : HtmlElement
	{
		/// <summary>
		/// Creates a new <see cref="HtmlFormElement"/> instance
		/// </summary>
		/// <inheritdoc cref="HtmlElement(System.Windows.Forms.HtmlElement)" path="/param"/>
		public HtmlFormElement(System.Windows.Forms.HtmlElement element) : base(element)
		{

		}

		/// <inheritdoc cref="GetMultipartFormDataContent(bool, bool, string?, ReportContent?)"/>
		public MultipartFormDataContent GetMultipartFormDataContent(bool addDefaultInputContentType, bool encapsulateName, string action)
		{
			MultipartFormDataContent content = new MultipartFormDataContent();
			foreach (HtmlElement input in Inputs)
			{
				HttpContent toAdd;
				string name;
				if (encapsulateName)
					name = $@"""{input.Name}""";
				else
					name = input.Name;
				switch (input.Type)
				{
					//Files are not supported yet
					case "file":
						Console.WriteLine("Sending files is not implemented");
						toAdd = new ByteArrayContent(Array.Empty<byte>());
						content.Add(toAdd, name);
						toAdd.Headers.Add("Content-Type", "application/octet-stream");
						toAdd.Headers.ContentDisposition!.FileName = @"""""";
						break;
					default:
						toAdd = new StringContent(input.Value);
						content.Add(toAdd, name);
						if (!addDefaultInputContentType)
							toAdd.Headers.Remove("Content-Type");
						break;
				}
			}
			if (string.IsNullOrWhiteSpace(action))
				return content;
			StringContent save = new StringContent("");
			save.Headers.Remove("Content-Type");
			content.Add(save, @"""save""");
			return content;
		}

		/// <summary>
		/// Generates a <see cref="MultipartFormDataContent"/>
		/// </summary>
		/// <param name="addDefaultInputContentType">Wether or not Content-Type should be stripped from most <see cref="HttpContent"/>s</param>
		/// <param name="encapsulateName">Wether or not input names should be encapsulated in quotation marks</param>
		/// <param name="action">Name of the action</param>
		/// <param name="reportContent"><see cref="ReportContent"/> to use to fill visible inputs leave <see langword="null"/> to send only input values</param>
		/// <returns>Filled <see cref="MultipartFormDataContent"/></returns>
		public MultipartFormDataContent GetMultipartFormDataContent(bool addDefaultInputContentType, bool encapsulateName, string? action = null, ReportContent? reportContent = null)
		{
			MultipartFormDataContent content = new MultipartFormDataContent();
			List<PropertyInfo> properties = reportContent == null ? new List<PropertyInfo>() : reportContent.GetType().GetProperties().ToList();
			foreach (HtmlElement input in Inputs)
			{
				PropertyInfo? inputInfo = input.IsHidden ? null : properties.FirstOrDefault(info => PropertyFinder(info, input.Name));
				HttpContent toAdd;
				string name;
				if (encapsulateName)
					name = $@"""{input.Name}""";
				else
					name = input.Name;
				switch (input.Type)
				{
					//Files are not supported yet
					case "file":
						Console.WriteLine("Sending files is not implemented");
						toAdd = new ByteArrayContent(inputInfo?.GetValue(reportContent) as byte[] ?? Array.Empty<byte>());
						content.Add(toAdd, name);
						toAdd.Headers.Add("Content-Type", "application/octet-stream");
						toAdd.Headers.ContentDisposition!.FileName = @"""""";
						break;
					default:
						toAdd = new StringContent(inputInfo?.GetValue(reportContent) as string ?? input.Value);
						content.Add(toAdd, name);
						if (!addDefaultInputContentType)
							toAdd.Headers.Remove("Content-Type");
						break;
				}
			}
			if (string.IsNullOrWhiteSpace(action))
				return content;
			StringContent save = new StringContent("");
			save.Headers.Remove("Content-Type");
			content.Add(save, @"""save""");
			return content;
		}

		/// <summary>
		/// <see cref="Predicate{T}"/> to find <see cref="PropertyInfo"/>s whose name matches with <paramref name="inputName"/>
		/// </summary>
		/// <param name="info"><see cref="PropertyInfo"/> to check</param>
		/// <param name="inputName">Name of input elemenr</param>
		/// <returns><see langword="true"/> if <paramref name="info"/> matches names with <paramref name="inputName"/> and <see langword="false"/> otherwise</returns>
		private bool PropertyFinder(PropertyInfo info, string inputName)
		{
			if (info.GetCustomAttribute<IHKFormDataNameAttribute>() is IHKFormDataNameAttribute attr)
			{
				if (inputName.Equals(attr.Name, StringComparison.Ordinal))
					return true;
				else if (inputName.Equals(info.Name, StringComparison.Ordinal))
					return true;
			}
			return false;
		}

		protected override string GetStringDebuggerDisplay()
		{
			return $"{base.GetStringDebuggerDisplay().Trim()}, #AllInputs: {AllInputs.Count}, #Inputs: {Inputs.Count}";
		}
	}
}
