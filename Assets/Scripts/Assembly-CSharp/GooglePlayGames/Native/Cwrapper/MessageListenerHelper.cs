using System;
using System.Runtime.InteropServices;

namespace GooglePlayGames.Native.Cwrapper
{
	internal static class MessageListenerHelper
	{
		internal delegate void OnMessageReceivedCallback(long arg0, string arg1, IntPtr arg2, UIntPtr arg3, [MarshalAs(UnmanagedType.I1)] bool arg4, IntPtr arg5);

		internal delegate void OnDisconnectedCallback(long arg0, string arg1, IntPtr arg2);

		[DllImport("gpg")]
		internal static extern void MessageListenerHelper_SetOnMessageReceivedCallback(HandleRef self, OnMessageReceivedCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void MessageListenerHelper_SetOnDisconnectedCallback(HandleRef self, OnDisconnectedCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern IntPtr MessageListenerHelper_Construct();

		[DllImport("gpg")]
		internal static extern void MessageListenerHelper_Dispose(HandleRef self);
	}
}
