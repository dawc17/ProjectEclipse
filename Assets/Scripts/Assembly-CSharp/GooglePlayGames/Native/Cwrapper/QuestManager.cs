using System;
using System.Runtime.InteropServices;

namespace GooglePlayGames.Native.Cwrapper
{
	internal static class QuestManager
	{
		internal delegate void FetchCallback(IntPtr arg0, IntPtr arg1);

		internal delegate void FetchListCallback(IntPtr arg0, IntPtr arg1);

		internal delegate void AcceptCallback(IntPtr arg0, IntPtr arg1);

		internal delegate void ClaimMilestoneCallback(IntPtr arg0, IntPtr arg1);

		internal delegate void QuestUICallback(IntPtr arg0, IntPtr arg1);

		[DllImport("gpg")]
		internal static extern void QuestManager_FetchList(HandleRef self, Types.DataSource data_source, int fetch_flags, FetchListCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void QuestManager_Accept(HandleRef self, IntPtr quest, AcceptCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void QuestManager_ShowAllUI(HandleRef self, QuestUICallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void QuestManager_ShowUI(HandleRef self, IntPtr quest, QuestUICallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void QuestManager_ClaimMilestone(HandleRef self, IntPtr milestone, ClaimMilestoneCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void QuestManager_Fetch(HandleRef self, Types.DataSource data_source, string quest_id, FetchCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void QuestManager_FetchResponse_Dispose(HandleRef self);

		[DllImport("gpg")]
		internal static extern CommonErrorStatus.ResponseStatus QuestManager_FetchResponse_GetStatus(HandleRef self);

		[DllImport("gpg")]
		internal static extern IntPtr QuestManager_FetchResponse_GetData(HandleRef self);

		[DllImport("gpg")]
		internal static extern void QuestManager_FetchListResponse_Dispose(HandleRef self);

		[DllImport("gpg")]
		internal static extern CommonErrorStatus.ResponseStatus QuestManager_FetchListResponse_GetStatus(HandleRef self);

		[DllImport("gpg")]
		internal static extern UIntPtr QuestManager_FetchListResponse_GetData_Length(HandleRef self);

		[DllImport("gpg")]
		internal static extern IntPtr QuestManager_FetchListResponse_GetData_GetElement(HandleRef self, UIntPtr index);

		[DllImport("gpg")]
		internal static extern void QuestManager_AcceptResponse_Dispose(HandleRef self);

		[DllImport("gpg")]
		internal static extern CommonErrorStatus.QuestAcceptStatus QuestManager_AcceptResponse_GetStatus(HandleRef self);

		[DllImport("gpg")]
		internal static extern IntPtr QuestManager_AcceptResponse_GetAcceptedQuest(HandleRef self);

		[DllImport("gpg")]
		internal static extern void QuestManager_ClaimMilestoneResponse_Dispose(HandleRef self);

		[DllImport("gpg")]
		internal static extern CommonErrorStatus.QuestClaimMilestoneStatus QuestManager_ClaimMilestoneResponse_GetStatus(HandleRef self);

		[DllImport("gpg")]
		internal static extern IntPtr QuestManager_ClaimMilestoneResponse_GetClaimedMilestone(HandleRef self);

		[DllImport("gpg")]
		internal static extern IntPtr QuestManager_ClaimMilestoneResponse_GetQuest(HandleRef self);

		[DllImport("gpg")]
		internal static extern void QuestManager_QuestUIResponse_Dispose(HandleRef self);

		[DllImport("gpg")]
		internal static extern CommonErrorStatus.UIStatus QuestManager_QuestUIResponse_GetStatus(HandleRef self);

		[DllImport("gpg")]
		internal static extern IntPtr QuestManager_QuestUIResponse_GetAcceptedQuest(HandleRef self);

		[DllImport("gpg")]
		internal static extern IntPtr QuestManager_QuestUIResponse_GetMilestoneToClaim(HandleRef self);
	}
}
