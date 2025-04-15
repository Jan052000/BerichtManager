using System.Runtime.InteropServices;

namespace BerichtManager.OwnControls.Native
{
	[StructLayout(LayoutKind.Sequential)]
	public struct DrawItemStruct
	{
		private uint ctlType;
		public readonly CtlType CtlType => (CtlType)ctlType;
		public uint CtlID;
		public int ItemID;
		private uint itemAction;
		public ItemAction ItemAction => (ItemAction)itemAction;
		private uint itemState;
		public DrawItemState ItemState => (DrawItemState)itemState;
		public IntPtr HWNDItem;
		public IntPtr HDC;
		public Rect RcItem;
		public UIntPtr itemData;
	}

	[Flags]
	public enum CtlType
	{
		ODT_BUTTON = 0x1,
		ODT_COMBOBOX = 0x2,
		ODT_LISTBOX = 0x4,
		ODT_LISTVIEW = 0x8,
		ODT_MENU = 0xF,
		ODT_STATIC = 0x20,
		ODT_TAB = 0x40,
	}

	public enum ItemAction
	{
		ODA_DRAWENTIRE,
		ODA_FOCUS,
		ODA_SELECT
	}
}
