using System;
using System.Runtime.InteropServices;

namespace GooglePlayGames.Native.Cwrapper
{
	internal static class RealTimeEventListenerHelper
	{
		internal delegate void OnRoomStatusChangedCallback(IntPtr arg0, IntPtr arg1);

		internal delegate void OnRoomConnectedSetChangedCallback(IntPtr arg0, IntPtr arg1);

		internal delegate void OnP2PConnectedCallback(IntPtr arg0, IntPtr arg1, IntPtr arg2);

		internal delegate void OnP2PDisconnectedCallback(IntPtr arg0, IntPtr arg1, IntPtr arg2);

		internal delegate void OnParticipantStatusChangedCallback(IntPtr arg0, IntPtr arg1, IntPtr arg2);

		internal delegate void OnDataReceivedCallback(IntPtr arg0, IntPtr arg1, IntPtr arg2, UIntPtr arg3, [MarshalAs(UnmanagedType.I1)] bool arg4, IntPtr arg5);

		[DllImport("gpg")]
		internal static extern void RealTimeEventListenerHelper_SetOnParticipantStatusChangedCallback(HandleRef self, OnParticipantStatusChangedCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern IntPtr RealTimeEventListenerHelper_Construct();

		[DllImport("gpg")]
		internal static extern void RealTimeEventListenerHelper_SetOnP2PDisconnectedCallback(HandleRef self, OnP2PDisconnectedCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void RealTimeEventListenerHelper_SetOnDataReceivedCallback(HandleRef self, OnDataReceivedCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void RealTimeEventListenerHelper_SetOnRoomStatusChangedCallback(HandleRef self, OnRoomStatusChangedCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void RealTimeEventListenerHelper_SetOnP2PConnectedCallback(HandleRef self, OnP2PConnectedCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void RealTimeEventListenerHelper_SetOnRoomConnectedSetChangedCallback(HandleRef self, OnRoomConnectedSetChangedCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void RealTimeEventListenerHelper_Dispose(HandleRef self);
	}
}
