using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BerichtManager.IHKClient
{
	/// <summary>
	/// Results of the report creation process
	/// </summary>
	public enum CreateResults
	{
		/// <summary>
		/// Creation was successful
		/// </summary>
		Success,
		/// <summary>
		/// Creation failed as session expired
		/// </summary>
		Unauthorized,
		/// <summary>
		/// Creation failed while fetching creation form from IHK
		/// </summary>
		CreationFailed,
		/// <summary>
		/// Creation failed while sending form to IHK
		/// </summary>
		UploadFailed
	}
}
