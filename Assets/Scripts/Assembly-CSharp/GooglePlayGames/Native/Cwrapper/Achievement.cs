using System;
using System.Runtime.InteropServices;

namespace GooglePlayGames.Native.Cwrapper
{
	internal static class Achievement
	{
		[DllImport("gpg")]
		internal static extern uint Achievement_TotalSteps(HandleRef self);

		[DllImport("gpg")]
		internal static extern UIntPtr Achievement_Description(HandleRef self, [In][Out] byte[] out_arg, UIntPtr out_size);

		[DllImport("gpg")]
		internal static extern void Achievement_Dispose(HandleRef self);

		[DllImport("gpg")]
		internal static extern UIntPtr Achievement_UnlockedIconUrl(HandleRef self, [In][Out] byte[] out_arg, UIntPtr out_size);

		[DllImport("gpg")]
		internal static extern ulong Achievement_LastModifiedTime(HandleRef self);

		[DllImport("gpg")]
		internal static extern UIntPtr Achievement_RevealedIconUrl(HandleRef self, [In][Out] byte[] out_arg, UIntPtr out_size);

		[DllImport("gpg")]
		internal static extern uint Achievement_CurrentSteps(HandleRef self);

		[DllImport("gpg")]
		internal static extern Types.AchievementState Achievement_State(HandleRef self);

		[DllImport("gpg")]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool Achievement_Valid(HandleRef self);

		[DllImport("gpg")]
		internal static extern ulong Achievement_LastModified(HandleRef self);

		[DllImport("gpg")]
		internal static extern ulong Achievement_XP(HandleRef self);

		[DllImport("gpg")]
		internal static extern Types.AchievementType Achievement_Type(HandleRef self);

		[DllImport("gpg")]
		internal static extern UIntPtr Achievement_Id(HandleRef self, [In][Out] byte[] out_arg, UIntPtr out_size);

		[DllImport("gpg")]
		internal static extern UIntPtr Achievement_Name(HandleRef self, [In][Out] byte[] out_arg, UIntPtr out_size);
	}
}
