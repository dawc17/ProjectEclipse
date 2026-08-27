using System;
using System.Runtime.InteropServices;

namespace GooglePlayGames.Native.Cwrapper
{
	internal static class Quest
	{
		[DllImport("gpg")]
		internal static extern UIntPtr Quest_Description(HandleRef self, [In][Out] byte[] out_arg, UIntPtr out_size);

		[DllImport("gpg")]
		internal static extern UIntPtr Quest_BannerUrl(HandleRef self, [In][Out] byte[] out_arg, UIntPtr out_size);

		[DllImport("gpg")]
		internal static extern long Quest_ExpirationTime(HandleRef self);

		[DllImport("gpg")]
		internal static extern UIntPtr Quest_IconUrl(HandleRef self, [In][Out] byte[] out_arg, UIntPtr out_size);

		[DllImport("gpg")]
		internal static extern Types.QuestState Quest_State(HandleRef self);

		[DllImport("gpg")]
		internal static extern IntPtr Quest_Copy(HandleRef self);

		[DllImport("gpg")]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool Quest_Valid(HandleRef self);

		[DllImport("gpg")]
		internal static extern long Quest_StartTime(HandleRef self);

		[DllImport("gpg")]
		internal static extern void Quest_Dispose(HandleRef self);

		[DllImport("gpg")]
		internal static extern IntPtr Quest_CurrentMilestone(HandleRef self);

		[DllImport("gpg")]
		internal static extern long Quest_AcceptedTime(HandleRef self);

		[DllImport("gpg")]
		internal static extern UIntPtr Quest_Id(HandleRef self, [In][Out] byte[] out_arg, UIntPtr out_size);

		[DllImport("gpg")]
		internal static extern UIntPtr Quest_Name(HandleRef self, [In][Out] byte[] out_arg, UIntPtr out_size);
	}
}
