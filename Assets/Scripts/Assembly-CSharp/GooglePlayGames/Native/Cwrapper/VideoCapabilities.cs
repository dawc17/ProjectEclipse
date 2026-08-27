using System.Runtime.InteropServices;

namespace GooglePlayGames.Native.Cwrapper
{
	internal static class VideoCapabilities
	{
		[DllImport("gpg")]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool VideoCapabilities_IsCameraSupported(HandleRef self);

		[DllImport("gpg")]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool VideoCapabilities_IsMicSupported(HandleRef self);

		[DllImport("gpg")]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool VideoCapabilities_IsWriteStorageSupported(HandleRef self);

		[DllImport("gpg")]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool VideoCapabilities_SupportsCaptureMode(HandleRef self, Types.VideoCaptureMode capture_mode);

		[DllImport("gpg")]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool VideoCapabilities_SupportsQualityLevel(HandleRef self, Types.VideoQualityLevel quality_level);

		[DllImport("gpg")]
		internal static extern void VideoCapabilities_Dispose(HandleRef self);

		[DllImport("gpg")]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool VideoCapabilities_Valid(HandleRef self);
	}
}
