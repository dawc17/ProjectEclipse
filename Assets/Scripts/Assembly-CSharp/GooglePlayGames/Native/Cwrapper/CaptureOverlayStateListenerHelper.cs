using System;
using System.Runtime.InteropServices;

namespace GooglePlayGames.Native.Cwrapper
{
	internal static class CaptureOverlayStateListenerHelper
	{
		internal delegate void OnCaptureOverlayStateChangedCallback(Types.VideoCaptureOverlayState arg0, IntPtr arg1);

		[DllImport("gpg")]
		internal static extern void CaptureOverlayStateListenerHelper_SetOnCaptureOverlayStateChangedCallback(HandleRef self, OnCaptureOverlayStateChangedCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern IntPtr CaptureOverlayStateListenerHelper_Construct();

		[DllImport("gpg")]
		internal static extern void CaptureOverlayStateListenerHelper_Dispose(HandleRef self);
	}
}
