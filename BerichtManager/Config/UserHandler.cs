using System;
using System.Text;

namespace BerichtManager.Config
{
	public partial class UserHandler
	{
		private static string salt = "77445522";
		public static string EncodePassword(string password) 
		{
			if (string.IsNullOrEmpty(password)) return "";
			password += salt;
			var passwordBytes = Encoding.UTF8.GetBytes(password);
			return Convert.ToBase64String(passwordBytes);
		}

		public static string DecodePassword(string base64EncodeData) 
		{
			if (string.IsNullOrEmpty(base64EncodeData)) return "";
			var base64EncodeBytes = Convert.FromBase64String(base64EncodeData);
			var result = Encoding.UTF8.GetString(base64EncodeBytes);
			return result.Substring(0, result.Length - salt.Length);
		}
	}
}
