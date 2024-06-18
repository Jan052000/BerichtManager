using System;
using System.Net;

namespace BerichtManager.IHKClient
{
	/// <summary>
	/// Exception for process of logging in to IHK online portal
	/// </summary>
	internal class LoginException : Exception
	{
		/// <summary>
		/// Constructor for a new <see cref="LoginException"/>
		/// </summary>
		/// <param name="statusCode">Status code of login post request</param>
		public LoginException(HttpStatusCode statusCode) : base("Error while logging in, status code: " + statusCode.ToString())
		{
		}
	}
}
