using System.Runtime.InteropServices;

namespace BerichtManager.OwnControls.Native
{
	[StructLayout(LayoutKind.Sequential)]
	public struct NMHDR
	{
		public IntPtr hwndFrom;
		public IntPtr idFrom;
		public int code;
	}
}
