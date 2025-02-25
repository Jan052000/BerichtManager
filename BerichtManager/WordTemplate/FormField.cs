using System.Diagnostics;
using Word = Microsoft.Office.Interop.Word;

namespace BerichtManager.WordTemplate
{
	[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
	public class FormField : IEquatable<FormField>
	{
		/// <summary>
		/// Index of <see cref="Word.FormField"/>
		/// </summary>
		public int Index { get; set; }
		/// <summary>
		/// <see cref="Type"/> of <see cref="Word.FormField"/>
		/// </summary>
		public Type FieldType { get; set; }
		/// <summary>
		/// The default formatted text
		/// </summary>
		public string DisplayText { get; set; }

		/// <summary>
		/// Creates a new <see cref="FormField"/> object
		/// </summary>
		/// <param name="index"><inheritdoc cref="Index" path="/summary"/></param>
		/// <param name="type"><inheritdoc cref="FieldType" path="/summary"/></param>
		/// <param name="displayText"><inheritdoc cref="DisplayText" path="/summary"/></param>
		public FormField(int index, Type type, string displayText)
		{
			Index = index;
			FieldType = type;
			DisplayText = displayText ?? FieldType.ToString();
		}

		public bool Equals(FormField? other)
		{
			return other != null && Index == other.Index && FieldType == other.FieldType && DisplayText == other.DisplayText;
		}

		/// <summary>
		/// Generates string shown for object in debugger
		/// </summary>
		/// <returns>String representation of <see cref="FormField"/></returns>
		private string GetDebuggerDisplay()
		{
			return $"Index: {Index}, Type: {FieldType.Name}, DisplayText: {DisplayText}";
		}
	}
}
