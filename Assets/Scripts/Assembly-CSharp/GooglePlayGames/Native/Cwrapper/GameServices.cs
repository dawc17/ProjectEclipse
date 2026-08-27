using System;
using System.Runtime.InteropServices;

namespace GooglePlayGames.Native.Cwrapper
{
	internal static class GameServices
	{
		internal delegate void FlushCallback(CommonErrorStatus.FlushStatus arg0, IntPtr arg1);

		[DllImport("gpg")]
		internal static extern void GameServices_Flush(HandleRef self, FlushCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool GameServices_IsAuthorized(HandleRef self);

		[DllImport("gpg")]
		internal static extern void GameServices_Dispose(HandleRef self);

		[DllImport("gpg")]
		internal static extern void GameServices_SignOut(HandleRef self);

		[DllImport("gpg")]
		internal static extern void GameServices_StartAuthorizationUI(HandleRef self);
	}
}
