using System;
using System.Text;

namespace BerichtManager.Config
{
	public partial class UserHandler
	{
		/// <summary>
		/// Salt added to passwords before encryption
		/// </summary>
		private static string Salt = "77445522";

		/// <summary>
		/// Encrypts a password string
		/// </summary>
		/// <param name="password">Passowrd to encrypt</param>
		/// <returns>Encrypted password</returns>
		public static string EncodePassword(string password) 
		{
			if (string.IsNullOrEmpty(password)) return "";
			password += Salt;
			var passwordBytes = Encoding.UTF8.GetBytes(password);
			return Convert.ToBase64String(passwordBytes);
		}

		/// <summary>
		/// Decrypts a password string
		/// </summary>
		/// <param name="base64EncodeData">Encrypted password to decode</param>
		/// <returns>Decoded password</returns>
		public static string DecodePassword(string base64EncodeData) 
		{
			if (string.IsNullOrEmpty(base64EncodeData)) return "";
			var base64EncodeBytes = Convert.FromBase64String(base64EncodeData);
			var result = Encoding.UTF8.GetString(base64EncodeBytes);
			return result.Substring(0, result.Length - Salt.Length);
		}
	}
}
