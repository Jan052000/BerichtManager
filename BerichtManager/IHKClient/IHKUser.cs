using Newtonsoft.Json;

namespace BerichtManager.IHKClient
{
	internal class IHKUser
	{
		/// <summary>
		/// Username or IHK number of user
		/// </summary>
		[JsonProperty("login")]
		public string Username { get; set; }
		/// <summary>
		/// User password
		/// </summary>
		[JsonProperty("pass")]
		public string Password { get; set; }
		/// <summary>
		/// Unused property
		/// </summary>
		[JsonProperty("anmelden")]
		public string Unused { get; set; }

		public IHKUser(string username, string password)
		{
			Username = username;
			Password = password;
		}
	}
}
