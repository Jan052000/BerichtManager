using System;

namespace BerichtManager.IHKClient.Exceptions
{
	/// <summary>
	/// Exception in case there is a mismatch between found form inputs and report content properties
	/// </summary>
	internal class InputFieldsMismatchException : Exception
	{
		/// <summary>
		/// Constructor for a new <see cref="InputFieldsMismatchException"/>
		/// </summary>
		public InputFieldsMismatchException() : base("A mismatch between report fields and input fields was detected")
		{
			
		}
	}
}
