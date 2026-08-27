using System;
using System.Runtime.InteropServices;

namespace GooglePlayGames.Native.Cwrapper
{
	internal static class VideoManager
	{
		internal delegate void CaptureCapabilitiesCallback(IntPtr arg0, IntPtr arg1);

		internal delegate void CaptureStateCallback(IntPtr arg0, IntPtr arg1);

		internal delegate void IsCaptureAvailableCallback(IntPtr arg0, IntPtr arg1);

		[DllImport("gpg")]
		internal static extern void VideoManager_GetCaptureCapabilities(HandleRef self, CaptureCapabilitiesCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void VideoManager_ShowCaptureOverlay(HandleRef self);

		[DllImport("gpg")]
		internal static extern void VideoManager_GetCaptureState(HandleRef self, CaptureStateCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void VideoManager_IsCaptureAvailable(HandleRef self, Types.VideoCaptureMode capture_mode, IsCaptureAvailableCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool VideoManager_IsCaptureSupported(HandleRef self);

		[DllImport("gpg")]
		internal static extern void VideoManager_RegisterCaptureOverlayStateChangedListener(HandleRef self, IntPtr helper);

		[DllImport("gpg")]
		internal static extern void VideoManager_UnregisterCaptureOverlayStateChangedListener(HandleRef self);

		[DllImport("gpg")]
		internal static extern void VideoManager_GetCaptureCapabilitiesResponse_Dispose(HandleRef self);

		[DllImport("gpg")]
		internal static extern CommonErrorStatus.ResponseStatus VideoManager_GetCaptureCapabilitiesResponse_GetStatus(HandleRef self);

		[DllImport("gpg")]
		internal static extern IntPtr VideoManager_GetCaptureCapabilitiesResponse_GetVideoCapabilities(HandleRef self);

		[DllImport("gpg")]
		internal static extern void VideoManager_GetCaptureStateResponse_Dispose(HandleRef self);

		[DllImport("gpg")]
		internal static extern CommonErrorStatus.ResponseStatus VideoManager_GetCaptureStateResponse_GetStatus(HandleRef self);

		[DllImport("gpg")]
		internal static extern IntPtr VideoManager_GetCaptureStateResponse_GetVideoCaptureState(HandleRef self);

		[DllImport("gpg")]
		internal static extern void VideoManager_IsCaptureAvailableResponse_Dispose(HandleRef self);

		[DllImport("gpg")]
		internal static extern CommonErrorStatus.ResponseStatus VideoManager_IsCaptureAvailableResponse_GetStatus(HandleRef self);

		[DllImport("gpg")]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool VideoManager_IsCaptureAvailableResponse_GetIsCaptureAvailable(HandleRef self);
	}
}
