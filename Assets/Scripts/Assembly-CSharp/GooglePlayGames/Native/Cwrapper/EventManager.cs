using System;
using System.Runtime.InteropServices;

namespace GooglePlayGames.Native.Cwrapper
{
	internal static class EventManager
	{
		internal delegate void FetchAllCallback(IntPtr arg0, IntPtr arg1);

		internal delegate void FetchCallback(IntPtr arg0, IntPtr arg1);

		[DllImport("gpg")]
		internal static extern void EventManager_FetchAll(HandleRef self, Types.DataSource data_source, FetchAllCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void EventManager_Fetch(HandleRef self, Types.DataSource data_source, string event_id, FetchCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void EventManager_Increment(HandleRef self, string event_id, uint steps);

		[DllImport("gpg")]
		internal static extern void EventManager_FetchAllResponse_Dispose(HandleRef self);

		[DllImport("gpg")]
		internal static extern CommonErrorStatus.ResponseStatus EventManager_FetchAllResponse_GetStatus(HandleRef self);

		[DllImport("gpg")]
		internal static extern UIntPtr EventManager_FetchAllResponse_GetData(HandleRef self, IntPtr[] out_arg, UIntPtr out_size);

		[DllImport("gpg")]
		internal static extern void EventManager_FetchResponse_Dispose(HandleRef self);

		[DllImport("gpg")]
		internal static extern CommonErrorStatus.ResponseStatus EventManager_FetchResponse_GetStatus(HandleRef self);

		[DllImport("gpg")]
		internal static extern IntPtr EventManager_FetchResponse_GetData(HandleRef self);
	}
}
