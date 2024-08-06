using System;

namespace BerichtManager.HelperClasses.HtmlClasses.CSSSelectors
{
	/// <summary>
	/// <see cref="Exception"/> for unhandled types
	/// </summary>
	internal class UnhandledTypeException : Exception
	{
		/// <summary>
		/// Creates a new <see cref="UnhandledTypeException"/> object
		/// </summary>
		/// <param name="type"><see cref="Type"/> that was not handled</param>
		public UnhandledTypeException(Type type) : base($"Type {type.Name} is not handled") { }
	}
}
