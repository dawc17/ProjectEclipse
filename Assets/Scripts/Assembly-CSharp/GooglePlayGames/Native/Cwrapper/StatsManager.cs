using System;
using System.Runtime.InteropServices;

namespace GooglePlayGames.Native.Cwrapper
{
	internal static class StatsManager
	{
		internal delegate void FetchForPlayerCallback(IntPtr arg0, IntPtr arg1);

		[DllImport("gpg")]
		internal static extern void StatsManager_FetchForPlayer(HandleRef self, Types.DataSource data_source, FetchForPlayerCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void StatsManager_FetchForPlayerResponse_Dispose(HandleRef self);

		[DllImport("gpg")]
		internal static extern CommonErrorStatus.ResponseStatus StatsManager_FetchForPlayerResponse_GetStatus(HandleRef self);

		[DllImport("gpg")]
		internal static extern IntPtr StatsManager_FetchForPlayerResponse_GetData(HandleRef self);
	}
}
