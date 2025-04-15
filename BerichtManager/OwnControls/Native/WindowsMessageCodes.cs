namespace BerichtManager.OwnControls.Native
{
    internal enum WindowsMessageCodes : int
    {
        WM_PAINT = 0xF,
        WM_MOUSEMOVE = 0x200,
        WM_MOUSELEAVE = 0x2A3,
        WM_DRAWITEM = 0x2B,
        WM_NOTIFY = 0x4E,
		OCM_DRAWITEM = 0x202B,
		OCM_NOTIFY = 0x204E
	}
}
