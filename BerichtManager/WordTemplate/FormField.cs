using System;
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
		/// Creates a new <see cref="FormField"/> object
		/// </summary>
		/// <param name="index">Index of <see cref="Word.FormField"/> in template</param>
		/// <param name="type">Type of <see cref="Word.FormField"/> content</param>
		public FormField(int index, Type type)
		{
			Index = index;
			FieldType = type;
		}

		public bool Equals(FormField other)
		{
			return Index == other.Index && FieldType == other.FieldType;
		}

		/// <summary>
		/// Generates string shown for object in debugger
		/// </summary>
		/// <returns>String representation of <see cref="FormField"/></returns>
		private string GetDebuggerDisplay()
		{
			return $"Index: {Index}, Type: {FieldType.Name}";
		}
	}
}
