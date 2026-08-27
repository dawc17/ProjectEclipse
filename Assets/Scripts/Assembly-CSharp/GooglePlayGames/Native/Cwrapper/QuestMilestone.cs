using System;
using System.Runtime.InteropServices;

namespace GooglePlayGames.Native.Cwrapper
{
	internal static class QuestMilestone
	{
		[DllImport("gpg")]
		internal static extern UIntPtr QuestMilestone_EventId(HandleRef self, [In][Out] byte[] out_arg, UIntPtr out_size);

		[DllImport("gpg")]
		internal static extern ulong QuestMilestone_CurrentCount(HandleRef self);

		[DllImport("gpg")]
		internal static extern IntPtr QuestMilestone_Copy(HandleRef self);

		[DllImport("gpg")]
		internal static extern void QuestMilestone_Dispose(HandleRef self);

		[DllImport("gpg")]
		internal static extern ulong QuestMilestone_TargetCount(HandleRef self);

		[DllImport("gpg")]
		internal static extern UIntPtr QuestMilestone_QuestId(HandleRef self, [In][Out] byte[] out_arg, UIntPtr out_size);

		[DllImport("gpg")]
		internal static extern UIntPtr QuestMilestone_CompletionRewardData(HandleRef self, [In][Out] byte[] out_arg, UIntPtr out_size);

		[DllImport("gpg")]
		internal static extern Types.QuestMilestoneState QuestMilestone_State(HandleRef self);

		[DllImport("gpg")]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool QuestMilestone_Valid(HandleRef self);

		[DllImport("gpg")]
		internal static extern UIntPtr QuestMilestone_Id(HandleRef self, [In][Out] byte[] out_arg, UIntPtr out_size);
	}
}
