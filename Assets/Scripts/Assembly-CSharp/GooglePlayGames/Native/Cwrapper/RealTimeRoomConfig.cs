using System;
using System.Runtime.InteropServices;

namespace GooglePlayGames.Native.Cwrapper
{
	internal static class RealTimeRoomConfig
	{
		[DllImport("gpg")]
		internal static extern UIntPtr RealTimeRoomConfig_PlayerIdsToInvite_Length(HandleRef self);

		[DllImport("gpg")]
		internal static extern UIntPtr RealTimeRoomConfig_PlayerIdsToInvite_GetElement(HandleRef self, UIntPtr index, [In][Out] char[] out_arg, UIntPtr out_size);

		[DllImport("gpg")]
		internal static extern uint RealTimeRoomConfig_Variant(HandleRef self);

		[DllImport("gpg")]
		internal static extern long RealTimeRoomConfig_ExclusiveBitMask(HandleRef self);

		[DllImport("gpg")]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool RealTimeRoomConfig_Valid(HandleRef self);

		[DllImport("gpg")]
		internal static extern uint RealTimeRoomConfig_MaximumAutomatchingPlayers(HandleRef self);

		[DllImport("gpg")]
		internal static extern uint RealTimeRoomConfig_MinimumAutomatchingPlayers(HandleRef self);

		[DllImport("gpg")]
		internal static extern void RealTimeRoomConfig_Dispose(HandleRef self);
	}
}
