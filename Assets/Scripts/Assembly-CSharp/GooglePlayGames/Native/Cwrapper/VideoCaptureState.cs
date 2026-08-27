using System.Runtime.InteropServices;

namespace GooglePlayGames.Native.Cwrapper
{
	internal static class VideoCaptureState
	{
		[DllImport("gpg")]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool VideoCaptureState_IsCapturing(HandleRef self);

		[DllImport("gpg")]
		internal static extern Types.VideoCaptureMode VideoCaptureState_CaptureMode(HandleRef self);

		[DllImport("gpg")]
		internal static extern Types.VideoQualityLevel VideoCaptureState_QualityLevel(HandleRef self);

		[DllImport("gpg")]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool VideoCaptureState_IsOverlayVisible(HandleRef self);

		[DllImport("gpg")]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool VideoCaptureState_IsPaused(HandleRef self);

		[DllImport("gpg")]
		internal static extern void VideoCaptureState_Dispose(HandleRef self);

		[DllImport("gpg")]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool VideoCaptureState_Valid(HandleRef self);
	}
}
