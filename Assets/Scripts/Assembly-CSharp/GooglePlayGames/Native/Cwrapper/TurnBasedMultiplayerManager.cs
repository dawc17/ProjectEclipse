using System;
using System.Runtime.InteropServices;

namespace GooglePlayGames.Native.Cwrapper
{
	internal static class TurnBasedMultiplayerManager
	{
		internal delegate void TurnBasedMatchCallback(IntPtr arg0, IntPtr arg1);

		internal delegate void MultiplayerStatusCallback(CommonErrorStatus.MultiplayerStatus arg0, IntPtr arg1);

		internal delegate void TurnBasedMatchesCallback(IntPtr arg0, IntPtr arg1);

		internal delegate void MatchInboxUICallback(IntPtr arg0, IntPtr arg1);

		internal delegate void PlayerSelectUICallback(IntPtr arg0, IntPtr arg1);

		[DllImport("gpg")]
		internal static extern void TurnBasedMultiplayerManager_ShowPlayerSelectUI(HandleRef self, uint minimum_players, uint maximum_players, [MarshalAs(UnmanagedType.I1)] bool allow_automatch, PlayerSelectUICallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void TurnBasedMultiplayerManager_CancelMatch(HandleRef self, IntPtr match, MultiplayerStatusCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void TurnBasedMultiplayerManager_DismissMatch(HandleRef self, IntPtr match);

		[DllImport("gpg")]
		internal static extern void TurnBasedMultiplayerManager_ShowMatchInboxUI(HandleRef self, MatchInboxUICallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void TurnBasedMultiplayerManager_SynchronizeData(HandleRef self);

		[DllImport("gpg")]
		internal static extern void TurnBasedMultiplayerManager_Rematch(HandleRef self, IntPtr match, TurnBasedMatchCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void TurnBasedMultiplayerManager_DismissInvitation(HandleRef self, IntPtr invitation);

		[DllImport("gpg")]
		internal static extern void TurnBasedMultiplayerManager_FetchMatch(HandleRef self, string match_id, TurnBasedMatchCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void TurnBasedMultiplayerManager_DeclineInvitation(HandleRef self, IntPtr invitation);

		[DllImport("gpg")]
		internal static extern void TurnBasedMultiplayerManager_FinishMatchDuringMyTurn(HandleRef self, IntPtr match, byte[] match_data, UIntPtr match_data_size, IntPtr results, TurnBasedMatchCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void TurnBasedMultiplayerManager_FetchMatches(HandleRef self, TurnBasedMatchesCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void TurnBasedMultiplayerManager_CreateTurnBasedMatch(HandleRef self, IntPtr config, TurnBasedMatchCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void TurnBasedMultiplayerManager_AcceptInvitation(HandleRef self, IntPtr invitation, TurnBasedMatchCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void TurnBasedMultiplayerManager_TakeMyTurn(HandleRef self, IntPtr match, byte[] match_data, UIntPtr match_data_size, IntPtr results, IntPtr next_participant, TurnBasedMatchCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void TurnBasedMultiplayerManager_ConfirmPendingCompletion(HandleRef self, IntPtr match, TurnBasedMatchCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void TurnBasedMultiplayerManager_LeaveMatchDuringMyTurn(HandleRef self, IntPtr match, IntPtr next_participant, MultiplayerStatusCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void TurnBasedMultiplayerManager_LeaveMatchDuringTheirTurn(HandleRef self, IntPtr match, MultiplayerStatusCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void TurnBasedMultiplayerManager_TurnBasedMatchResponse_Dispose(HandleRef self);

		[DllImport("gpg")]
		internal static extern CommonErrorStatus.MultiplayerStatus TurnBasedMultiplayerManager_TurnBasedMatchResponse_GetStatus(HandleRef self);

		[DllImport("gpg")]
		internal static extern IntPtr TurnBasedMultiplayerManager_TurnBasedMatchResponse_GetMatch(HandleRef self);

		[DllImport("gpg")]
		internal static extern void TurnBasedMultiplayerManager_TurnBasedMatchesResponse_Dispose(HandleRef self);

		[DllImport("gpg")]
		internal static extern CommonErrorStatus.MultiplayerStatus TurnBasedMultiplayerManager_TurnBasedMatchesResponse_GetStatus(HandleRef self);

		[DllImport("gpg")]
		internal static extern UIntPtr TurnBasedMultiplayerManager_TurnBasedMatchesResponse_GetInvitations_Length(HandleRef self);

		[DllImport("gpg")]
		internal static extern IntPtr TurnBasedMultiplayerManager_TurnBasedMatchesResponse_GetInvitations_GetElement(HandleRef self, UIntPtr index);

		[DllImport("gpg")]
		internal static extern UIntPtr TurnBasedMultiplayerManager_TurnBasedMatchesResponse_GetMyTurnMatches_Length(HandleRef self);

		[DllImport("gpg")]
		internal static extern IntPtr TurnBasedMultiplayerManager_TurnBasedMatchesResponse_GetMyTurnMatches_GetElement(HandleRef self, UIntPtr index);

		[DllImport("gpg")]
		internal static extern UIntPtr TurnBasedMultiplayerManager_TurnBasedMatchesResponse_GetTheirTurnMatches_Length(HandleRef self);

		[DllImport("gpg")]
		internal static extern IntPtr TurnBasedMultiplayerManager_TurnBasedMatchesResponse_GetTheirTurnMatches_GetElement(HandleRef self, UIntPtr index);

		[DllImport("gpg")]
		internal static extern UIntPtr TurnBasedMultiplayerManager_TurnBasedMatchesResponse_GetCompletedMatches_Length(HandleRef self);

		[DllImport("gpg")]
		internal static extern IntPtr TurnBasedMultiplayerManager_TurnBasedMatchesResponse_GetCompletedMatches_GetElement(HandleRef self, UIntPtr index);

		[DllImport("gpg")]
		internal static extern void TurnBasedMultiplayerManager_MatchInboxUIResponse_Dispose(HandleRef self);

		[DllImport("gpg")]
		internal static extern CommonErrorStatus.UIStatus TurnBasedMultiplayerManager_MatchInboxUIResponse_GetStatus(HandleRef self);

		[DllImport("gpg")]
		internal static extern IntPtr TurnBasedMultiplayerManager_MatchInboxUIResponse_GetMatch(HandleRef self);

		[DllImport("gpg")]
		internal static extern void TurnBasedMultiplayerManager_PlayerSelectUIResponse_Dispose(HandleRef self);

		[DllImport("gpg")]
		internal static extern CommonErrorStatus.UIStatus TurnBasedMultiplayerManager_PlayerSelectUIResponse_GetStatus(HandleRef self);

		[DllImport("gpg")]
		internal static extern UIntPtr TurnBasedMultiplayerManager_PlayerSelectUIResponse_GetPlayerIds_Length(HandleRef self);

		[DllImport("gpg")]
		internal static extern UIntPtr TurnBasedMultiplayerManager_PlayerSelectUIResponse_GetPlayerIds_GetElement(HandleRef self, UIntPtr index, [In][Out] byte[] out_arg, UIntPtr out_size);

		[DllImport("gpg")]
		internal static extern uint TurnBasedMultiplayerManager_PlayerSelectUIResponse_GetMinimumAutomatchingPlayers(HandleRef self);

		[DllImport("gpg")]
		internal static extern uint TurnBasedMultiplayerManager_PlayerSelectUIResponse_GetMaximumAutomatchingPlayers(HandleRef self);
	}
}
