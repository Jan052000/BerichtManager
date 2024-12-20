using System;
using System.Windows.Forms;

namespace BerichtManager.Extensions
{
	/// <summary>
	/// Holds extensions for <see cref="Control"/>
	/// </summary>
	public static class ControlExtensions
	{
		/// <summary>
		/// Checks if invoke is required and executes the provided <paramref name="action"/>
		/// </summary>
		/// <param name="control">The <see cref="Control"/> that should call the invoke if it is required</param>
		/// <param name="action">The <see cref="Action"/> that should be taken</param>
		public static void ExecuteWithInvoke(this Control control, Action action)
		{
			if (action == null)
				return;
			if (control.InvokeRequired)
				control.Invoke(action);
			else
				action();
		}
	}
}
