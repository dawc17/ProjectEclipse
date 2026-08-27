using System;
using System.Runtime.InteropServices;

namespace GooglePlayGames.Native.Cwrapper
{
	internal static class IosPlatformConfiguration
	{
		[DllImport("gpg")]
		internal static extern IntPtr IosPlatformConfiguration_Construct();

		[DllImport("gpg")]
		internal static extern void IosPlatformConfiguration_Dispose(HandleRef self);

		[DllImport("gpg")]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool IosPlatformConfiguration_Valid(HandleRef self);

		[DllImport("gpg")]
		internal static extern void IosPlatformConfiguration_SetClientID(HandleRef self, string client_id);
	}
}
