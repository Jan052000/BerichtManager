using System;

namespace BerichtManager.IHKClient
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	internal class IHKFormDataNameAttribute : Attribute
	{
		/// <summary>
		/// Name of property it will have in the form
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Place in order of form
		/// </summary>
		public int FormOrder { get; set; }
		/// <summary>
		/// Wether or not the field is sent in form
		/// </summary>
		public bool IsActuallySent { get; set; } = true;
		public IHKFormDataNameAttribute(string name)
		{
			Name = name;
		}
	}
}
