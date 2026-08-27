using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using GooglePlayGames.Native.Cwrapper;
using GooglePlayGames.OurUtils;

namespace GooglePlayGames.Native.PInvoke
{
	internal class TurnBasedManager
	{
		internal delegate void TurnBasedMatchCallback(TurnBasedMatchResponse response);

		internal class MatchInboxUIResponse : BaseReferenceHolder
		{
			internal MatchInboxUIResponse(IntPtr selfPointer)
				: base(selfPointer)
			{
			}

			internal CommonErrorStatus.UIStatus UiStatus()
			{
				return TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_MatchInboxUIResponse_GetStatus(SelfPtr());
			}

			internal NativeTurnBasedMatch Match()
			{
				if (UiStatus() != CommonErrorStatus.UIStatus.VALID)
				{
					return null;
				}
				return new NativeTurnBasedMatch(TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_MatchInboxUIResponse_GetMatch(SelfPtr()));
			}

			protected override void CallDispose(HandleRef selfPointer)
			{
				TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_MatchInboxUIResponse_Dispose(selfPointer);
			}

			internal static MatchInboxUIResponse FromPointer(IntPtr pointer)
			{
				if (pointer.Equals(IntPtr.Zero))
				{
					return null;
				}
				return new MatchInboxUIResponse(pointer);
			}
		}

		internal class TurnBasedMatchResponse : BaseReferenceHolder
		{
			internal TurnBasedMatchResponse(IntPtr selfPointer)
				: base(selfPointer)
			{
			}

			internal CommonErrorStatus.MultiplayerStatus ResponseStatus()
			{
				return TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_TurnBasedMatchResponse_GetStatus(SelfPtr());
			}

			internal bool RequestSucceeded()
			{
				return ResponseStatus() > (CommonErrorStatus.MultiplayerStatus)0;
			}

			internal NativeTurnBasedMatch Match()
			{
				if (!RequestSucceeded())
				{
					return null;
				}
				return new NativeTurnBasedMatch(TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_TurnBasedMatchResponse_GetMatch(SelfPtr()));
			}

			protected override void CallDispose(HandleRef selfPointer)
			{
				TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_TurnBasedMatchResponse_Dispose(selfPointer);
			}

			internal static TurnBasedMatchResponse FromPointer(IntPtr pointer)
			{
				if (pointer.Equals(IntPtr.Zero))
				{
					return null;
				}
				return new TurnBasedMatchResponse(pointer);
			}
		}

		internal class TurnBasedMatchesResponse : BaseReferenceHolder
		{
			internal TurnBasedMatchesResponse(IntPtr selfPointer)
				: base(selfPointer)
			{
			}

			protected override void CallDispose(HandleRef selfPointer)
			{
				TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_TurnBasedMatchesResponse_Dispose(SelfPtr());
			}

			internal CommonErrorStatus.MultiplayerStatus Status()
			{
				return TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_TurnBasedMatchesResponse_GetStatus(SelfPtr());
			}

			internal IEnumerable<MultiplayerInvitation> Invitations()
			{
				return PInvokeUtilities.ToEnumerable(TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_TurnBasedMatchesResponse_GetInvitations_Length(SelfPtr()), (UIntPtr index) => new MultiplayerInvitation(TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_TurnBasedMatchesResponse_GetInvitations_GetElement(SelfPtr(), index)));
			}

			internal int InvitationCount()
			{
				return (int)TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_TurnBasedMatchesResponse_GetInvitations_Length(SelfPtr()).ToUInt32();
			}

			internal IEnumerable<NativeTurnBasedMatch> MyTurnMatches()
			{
				return PInvokeUtilities.ToEnumerable(TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_TurnBasedMatchesResponse_GetMyTurnMatches_Length(SelfPtr()), (UIntPtr index) => new NativeTurnBasedMatch(TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_TurnBasedMatchesResponse_GetMyTurnMatches_GetElement(SelfPtr(), index)));
			}

			internal int MyTurnMatchesCount()
			{
				return (int)TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_TurnBasedMatchesResponse_GetMyTurnMatches_Length(SelfPtr()).ToUInt32();
			}

			internal IEnumerable<NativeTurnBasedMatch> TheirTurnMatches()
			{
				return PInvokeUtilities.ToEnumerable(TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_TurnBasedMatchesResponse_GetTheirTurnMatches_Length(SelfPtr()), (UIntPtr index) => new NativeTurnBasedMatch(TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_TurnBasedMatchesResponse_GetTheirTurnMatches_GetElement(SelfPtr(), index)));
			}

			internal int TheirTurnMatchesCount()
			{
				return (int)TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_TurnBasedMatchesResponse_GetTheirTurnMatches_Length(SelfPtr()).ToUInt32();
			}

			internal IEnumerable<NativeTurnBasedMatch> CompletedMatches()
			{
				return PInvokeUtilities.ToEnumerable(TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_TurnBasedMatchesResponse_GetCompletedMatches_Length(SelfPtr()), (UIntPtr index) => new NativeTurnBasedMatch(TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_TurnBasedMatchesResponse_GetCompletedMatches_GetElement(SelfPtr(), index)));
			}

			internal int CompletedMatchesCount()
			{
				return (int)TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_TurnBasedMatchesResponse_GetCompletedMatches_Length(SelfPtr()).ToUInt32();
			}

			internal static TurnBasedMatchesResponse FromPointer(IntPtr pointer)
			{
				if (PInvokeUtilities.IsNull(pointer))
				{
					return null;
				}
				return new TurnBasedMatchesResponse(pointer);
			}
		}

		private readonly GameServices mGameServices;

		internal TurnBasedManager(GameServices services)
		{
			mGameServices = services;
		}

		internal void GetMatch(string matchId, Action<TurnBasedMatchResponse> callback)
		{
			TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_FetchMatch(mGameServices.AsHandle(), matchId, InternalTurnBasedMatchCallback, ToCallbackPointer(callback));
		}

		[AOT.MonoPInvokeCallback(typeof(TurnBasedMultiplayerManager.TurnBasedMatchCallback))]
		internal static void InternalTurnBasedMatchCallback(IntPtr response, IntPtr data)
		{
			Callbacks.PerformInternalCallback("TurnBasedManager#InternalTurnBasedMatchCallback", Callbacks.Type.Temporary, response, data);
		}

		internal void CreateMatch(TurnBasedMatchConfig config, Action<TurnBasedMatchResponse> callback)
		{
			TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_CreateTurnBasedMatch(mGameServices.AsHandle(), config.AsPointer(), InternalTurnBasedMatchCallback, ToCallbackPointer(callback));
		}

		internal void ShowPlayerSelectUI(uint minimumPlayers, uint maxiumPlayers, bool allowAutomatching, Action<PlayerSelectUIResponse> callback)
		{
			TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_ShowPlayerSelectUI(mGameServices.AsHandle(), minimumPlayers, maxiumPlayers, allowAutomatching, InternalPlayerSelectUIcallback, Callbacks.ToIntPtr(callback, PlayerSelectUIResponse.FromPointer));
		}

		[AOT.MonoPInvokeCallback(typeof(TurnBasedMultiplayerManager.PlayerSelectUICallback))]
		internal static void InternalPlayerSelectUIcallback(IntPtr response, IntPtr data)
		{
			Callbacks.PerformInternalCallback("TurnBasedManager#PlayerSelectUICallback", Callbacks.Type.Temporary, response, data);
		}

		internal void GetAllTurnbasedMatches(Action<TurnBasedMatchesResponse> callback)
		{
			TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_FetchMatches(mGameServices.AsHandle(), InternalTurnBasedMatchesCallback, Callbacks.ToIntPtr(callback, TurnBasedMatchesResponse.FromPointer));
		}

		[AOT.MonoPInvokeCallback(typeof(TurnBasedMultiplayerManager.TurnBasedMatchesCallback))]
		internal static void InternalTurnBasedMatchesCallback(IntPtr response, IntPtr data)
		{
			Callbacks.PerformInternalCallback("TurnBasedManager#TurnBasedMatchesCallback", Callbacks.Type.Temporary, response, data);
		}

		internal void AcceptInvitation(MultiplayerInvitation invitation, Action<TurnBasedMatchResponse> callback)
		{
			Logger.d("Accepting invitation: " + invitation.AsPointer().ToInt64());
			TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_AcceptInvitation(mGameServices.AsHandle(), invitation.AsPointer(), InternalTurnBasedMatchCallback, ToCallbackPointer(callback));
		}

		internal void DeclineInvitation(MultiplayerInvitation invitation)
		{
			TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_DeclineInvitation(mGameServices.AsHandle(), invitation.AsPointer());
		}

		internal void TakeTurn(NativeTurnBasedMatch match, byte[] data, MultiplayerParticipant nextParticipant, Action<TurnBasedMatchResponse> callback)
		{
			TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_TakeMyTurn(mGameServices.AsHandle(), match.AsPointer(), data, new UIntPtr((uint)data.Length), match.Results().AsPointer(), nextParticipant.AsPointer(), InternalTurnBasedMatchCallback, ToCallbackPointer(callback));
		}

		[AOT.MonoPInvokeCallback(typeof(TurnBasedMultiplayerManager.MatchInboxUICallback))]
		internal static void InternalMatchInboxUICallback(IntPtr response, IntPtr data)
		{
			Callbacks.PerformInternalCallback("TurnBasedManager#MatchInboxUICallback", Callbacks.Type.Temporary, response, data);
		}

		internal void ShowInboxUI(Action<MatchInboxUIResponse> callback)
		{
			TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_ShowMatchInboxUI(mGameServices.AsHandle(), InternalMatchInboxUICallback, Callbacks.ToIntPtr(callback, MatchInboxUIResponse.FromPointer));
		}

		[AOT.MonoPInvokeCallback(typeof(TurnBasedMultiplayerManager.MultiplayerStatusCallback))]
		internal static void InternalMultiplayerStatusCallback(CommonErrorStatus.MultiplayerStatus status, IntPtr data)
		{
			Logger.d("InternalMultiplayerStatusCallback: " + status);
			Action<CommonErrorStatus.MultiplayerStatus> action = Callbacks.IntPtrToTempCallback<Action<CommonErrorStatus.MultiplayerStatus>>(data);
			try
			{
				action(status);
			}
			catch (Exception ex)
			{
				Logger.e("Error encountered executing InternalMultiplayerStatusCallback. Smothering to avoid passing exception into Native: " + ex);
			}
		}

		internal void LeaveDuringMyTurn(NativeTurnBasedMatch match, MultiplayerParticipant nextParticipant, Action<CommonErrorStatus.MultiplayerStatus> callback)
		{
			TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_LeaveMatchDuringMyTurn(mGameServices.AsHandle(), match.AsPointer(), nextParticipant.AsPointer(), InternalMultiplayerStatusCallback, Callbacks.ToIntPtr(callback));
		}

		internal void FinishMatchDuringMyTurn(NativeTurnBasedMatch match, byte[] data, ParticipantResults results, Action<TurnBasedMatchResponse> callback)
		{
			TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_FinishMatchDuringMyTurn(mGameServices.AsHandle(), match.AsPointer(), data, new UIntPtr((uint)data.Length), results.AsPointer(), InternalTurnBasedMatchCallback, ToCallbackPointer(callback));
		}

		internal void ConfirmPendingCompletion(NativeTurnBasedMatch match, Action<TurnBasedMatchResponse> callback)
		{
			TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_ConfirmPendingCompletion(mGameServices.AsHandle(), match.AsPointer(), InternalTurnBasedMatchCallback, ToCallbackPointer(callback));
		}

		internal void LeaveMatchDuringTheirTurn(NativeTurnBasedMatch match, Action<CommonErrorStatus.MultiplayerStatus> callback)
		{
			TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_LeaveMatchDuringTheirTurn(mGameServices.AsHandle(), match.AsPointer(), InternalMultiplayerStatusCallback, Callbacks.ToIntPtr(callback));
		}

		internal void CancelMatch(NativeTurnBasedMatch match, Action<CommonErrorStatus.MultiplayerStatus> callback)
		{
			TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_CancelMatch(mGameServices.AsHandle(), match.AsPointer(), InternalMultiplayerStatusCallback, Callbacks.ToIntPtr(callback));
		}

		internal void Rematch(NativeTurnBasedMatch match, Action<TurnBasedMatchResponse> callback)
		{
			TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_Rematch(mGameServices.AsHandle(), match.AsPointer(), InternalTurnBasedMatchCallback, ToCallbackPointer(callback));
		}

		private static IntPtr ToCallbackPointer(Action<TurnBasedMatchResponse> callback)
		{
			return Callbacks.ToIntPtr(callback, TurnBasedMatchResponse.FromPointer);
		}
	}
}
