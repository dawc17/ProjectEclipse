using System;
using System.Collections;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Multiplayer;
using GooglePlayGames.Native.Cwrapper;
using GooglePlayGames.Native.PInvoke;
using GooglePlayGames.OurUtils;

namespace GooglePlayGames.Native
{
	public class NativeTurnBasedMultiplayerClient : ITurnBasedMultiplayerClient
	{
		private readonly TurnBasedManager mTurnBasedManager;

		private readonly NativeClient mNativeClient;

		private volatile Action<GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch, bool> mMatchDelegate;

		internal NativeTurnBasedMultiplayerClient(NativeClient nativeClient, TurnBasedManager manager)
		{
			mTurnBasedManager = manager;
			mNativeClient = nativeClient;
		}

		public void CreateQuickMatch(uint minOpponents, uint maxOpponents, uint variant, Action<bool, GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch> callback)
		{
			CreateQuickMatch(minOpponents, maxOpponents, variant, 0uL, callback);
		}

		public void CreateQuickMatch(uint minOpponents, uint maxOpponents, uint variant, ulong exclusiveBitmask, Action<bool, GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch> callback)
		{
			callback = GooglePlayGames.BasicApi.Callbacks.AsOnGameThreadCallback(callback);
			using (GooglePlayGames.Native.PInvoke.TurnBasedMatchConfigBuilder turnBasedMatchConfigBuilder = GooglePlayGames.Native.PInvoke.TurnBasedMatchConfigBuilder.Create())
			{
				turnBasedMatchConfigBuilder.SetVariant(variant).SetMinimumAutomatchingPlayers(minOpponents).SetMaximumAutomatchingPlayers(maxOpponents)
					.SetExclusiveBitMask(exclusiveBitmask);
				using (GooglePlayGames.Native.PInvoke.TurnBasedMatchConfig config = turnBasedMatchConfigBuilder.Build())
				{
					mTurnBasedManager.CreateMatch(config, BridgeMatchToUserCallback((UIStatus status, GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch match) =>
					{
						callback(status == UIStatus.Valid, match);
					}));
				}
			}
		}

		public void CreateWithInvitationScreen(uint minOpponents, uint maxOpponents, uint variant, Action<bool, GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch> callback)
		{
			CreateWithInvitationScreen(minOpponents, maxOpponents, variant, (UIStatus status, GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch match) =>
			{
				callback(status == UIStatus.Valid, match);
			});
		}

		public void CreateWithInvitationScreen(uint minOpponents, uint maxOpponents, uint variant, Action<UIStatus, GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch> callback)
		{
			callback = GooglePlayGames.BasicApi.Callbacks.AsOnGameThreadCallback(callback);
			mTurnBasedManager.ShowPlayerSelectUI(minOpponents, maxOpponents, true, (PlayerSelectUIResponse result) =>
			{
				if (result.Status() != CommonErrorStatus.UIStatus.VALID)
				{
					callback((UIStatus)result.Status(), null);
					return;
				}
				using (GooglePlayGames.Native.PInvoke.TurnBasedMatchConfigBuilder turnBasedMatchConfigBuilder = GooglePlayGames.Native.PInvoke.TurnBasedMatchConfigBuilder.Create())
				{
					turnBasedMatchConfigBuilder.PopulateFromUIResponse(result).SetVariant(variant);
					using (GooglePlayGames.Native.PInvoke.TurnBasedMatchConfig config = turnBasedMatchConfigBuilder.Build())
					{
						mTurnBasedManager.CreateMatch(config, BridgeMatchToUserCallback(callback));
					}
				}
			});
		}

		public void GetAllInvitations(Action<Invitation[]> callback)
		{
			mTurnBasedManager.GetAllTurnbasedMatches((TurnBasedManager.TurnBasedMatchesResponse allMatches) =>
			{
				Invitation[] array = new Invitation[allMatches.InvitationCount()];
				int num = 0;
				foreach (GooglePlayGames.Native.PInvoke.MultiplayerInvitation item in allMatches.Invitations())
				{
					array[num++] = item.AsInvitation();
				}
				callback(array);
			});
		}

		public void GetAllMatches(Action<GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch[]> callback)
		{
			mTurnBasedManager.GetAllTurnbasedMatches((TurnBasedManager.TurnBasedMatchesResponse allMatches) =>
			{
				int num = allMatches.MyTurnMatchesCount() + allMatches.TheirTurnMatchesCount() + allMatches.CompletedMatchesCount();
				GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch[] array = new GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch[num];
				int num2 = 0;
				foreach (NativeTurnBasedMatch item in allMatches.MyTurnMatches())
				{
					array[num2++] = item.AsTurnBasedMatch(mNativeClient.GetUserId());
				}
				foreach (NativeTurnBasedMatch item2 in allMatches.TheirTurnMatches())
				{
					array[num2++] = item2.AsTurnBasedMatch(mNativeClient.GetUserId());
				}
				foreach (NativeTurnBasedMatch item3 in allMatches.CompletedMatches())
				{
					array[num2++] = item3.AsTurnBasedMatch(mNativeClient.GetUserId());
				}
				callback(array);
			});
		}

		private Action<TurnBasedManager.TurnBasedMatchResponse> BridgeMatchToUserCallback(Action<UIStatus, GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch> userCallback)
		{
			return (TurnBasedManager.TurnBasedMatchResponse callbackResult) =>
			{
				using (NativeTurnBasedMatch nativeTurnBasedMatch = callbackResult.Match())
				{
					if (nativeTurnBasedMatch == null)
					{
						UIStatus arg = UIStatus.InternalError;
						switch (callbackResult.ResponseStatus())
						{
						case CommonErrorStatus.MultiplayerStatus.VALID:
							arg = UIStatus.Valid;
							break;
						case CommonErrorStatus.MultiplayerStatus.VALID_BUT_STALE:
							arg = UIStatus.Valid;
							break;
						case CommonErrorStatus.MultiplayerStatus.ERROR_INTERNAL:
							arg = UIStatus.InternalError;
							break;
						case CommonErrorStatus.MultiplayerStatus.ERROR_NOT_AUTHORIZED:
							arg = UIStatus.NotAuthorized;
							break;
						case CommonErrorStatus.MultiplayerStatus.ERROR_VERSION_UPDATE_REQUIRED:
							arg = UIStatus.VersionUpdateRequired;
							break;
						case CommonErrorStatus.MultiplayerStatus.ERROR_TIMEOUT:
							arg = UIStatus.Timeout;
							break;
						}
						userCallback(arg, null);
					}
					else
					{
						GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch turnBasedMatch = nativeTurnBasedMatch.AsTurnBasedMatch(mNativeClient.GetUserId());
						Logger.d("Passing converted match to user callback:" + turnBasedMatch);
						userCallback(UIStatus.Valid, turnBasedMatch);
					}
				}
			};
		}

		public void AcceptFromInbox(Action<bool, GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch> callback)
		{
			callback = GooglePlayGames.BasicApi.Callbacks.AsOnGameThreadCallback(callback);
			mTurnBasedManager.ShowInboxUI((TurnBasedManager.MatchInboxUIResponse callbackResult) =>
			{
				using (NativeTurnBasedMatch nativeTurnBasedMatch = callbackResult.Match())
				{
					if (nativeTurnBasedMatch == null)
					{
						callback(false, null);
					}
					else
					{
						GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch turnBasedMatch = nativeTurnBasedMatch.AsTurnBasedMatch(mNativeClient.GetUserId());
						Logger.d("Passing converted match to user callback:" + turnBasedMatch);
						callback(true, turnBasedMatch);
					}
				}
			});
		}

		public void AcceptInvitation(string invitationId, Action<bool, GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch> callback)
		{
			callback = GooglePlayGames.BasicApi.Callbacks.AsOnGameThreadCallback(callback);
			FindInvitationWithId(invitationId, (GooglePlayGames.Native.PInvoke.MultiplayerInvitation invitation) =>
			{
				if (invitation == null)
				{
					Logger.e("Could not find invitation with id " + invitationId);
					callback(false, null);
				}
				else
				{
					mTurnBasedManager.AcceptInvitation(invitation, BridgeMatchToUserCallback((UIStatus status, GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch match) =>
					{
						callback(status == UIStatus.Valid, match);
					}));
				}
			});
		}

		private void FindInvitationWithId(string invitationId, Action<GooglePlayGames.Native.PInvoke.MultiplayerInvitation> callback)
		{
			mTurnBasedManager.GetAllTurnbasedMatches((TurnBasedManager.TurnBasedMatchesResponse allMatches) =>
			{
				if (allMatches.Status() <= (CommonErrorStatus.MultiplayerStatus)0)
				{
					callback(null);
				}
				else
				{
					foreach (GooglePlayGames.Native.PInvoke.MultiplayerInvitation item in allMatches.Invitations())
					{
						using (item)
						{
							if (item.Id().Equals(invitationId))
							{
								callback(item);
								return;
							}
						}
					}
					callback(null);
				}
			});
		}

		public void RegisterMatchDelegate(MatchDelegate del)
		{
			if (del == null)
			{
				mMatchDelegate = null;
				return;
			}
			mMatchDelegate = GooglePlayGames.BasicApi.Callbacks.AsOnGameThreadCallback((GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch match, bool autoLaunch) =>
			{
				del(match, autoLaunch);
			});
		}

		internal void HandleMatchEvent(Types.MultiplayerEvent eventType, string matchId, NativeTurnBasedMatch match)
		{
			Action<GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch, bool> currentDelegate = mMatchDelegate;
			if (currentDelegate == null)
			{
				return;
			}
			if (eventType == Types.MultiplayerEvent.REMOVED)
			{
				Logger.d("Ignoring REMOVE event for match " + matchId);
				return;
			}
			bool shouldAutolaunch = eventType == Types.MultiplayerEvent.UPDATED_FROM_APP_LAUNCH;
			match.ReferToMe();
			GooglePlayGames.BasicApi.Callbacks.AsCoroutine(WaitForLogin(() =>
			{
				currentDelegate(match.AsTurnBasedMatch(mNativeClient.GetUserId()), shouldAutolaunch);
				match.ForgetMe();
			}));
		}

		private IEnumerator WaitForLogin(Action method)
		{
			if (string.IsNullOrEmpty(mNativeClient.GetUserId()))
			{
				yield return null;
			}
			method();
		}

		public void TakeTurn(GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch match, byte[] data, string pendingParticipantId, Action<bool> callback)
		{
			Logger.describe(data);
			callback = GooglePlayGames.BasicApi.Callbacks.AsOnGameThreadCallback(callback);
			FindEqualVersionMatchWithParticipant(match, pendingParticipantId, callback, (GooglePlayGames.Native.PInvoke.MultiplayerParticipant pendingParticipant, NativeTurnBasedMatch foundMatch) =>
			{
				mTurnBasedManager.TakeTurn(foundMatch, data, pendingParticipant, (TurnBasedManager.TurnBasedMatchResponse result) =>
				{
					if (result.RequestSucceeded())
					{
						callback(true);
					}
					else
					{
						Logger.d("Taking turn failed: " + result.ResponseStatus());
						callback(false);
					}
				});
			});
		}

		private void FindEqualVersionMatch(GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch match, Action<bool> onFailure, Action<NativeTurnBasedMatch> onVersionMatch)
		{
			mTurnBasedManager.GetMatch(match.MatchId, (TurnBasedManager.TurnBasedMatchResponse response) =>
			{
				using (NativeTurnBasedMatch nativeTurnBasedMatch = response.Match())
				{
					if (nativeTurnBasedMatch == null)
					{
						Logger.e(string.Format("Could not find match {0}", match.MatchId));
						onFailure(false);
					}
					else if (nativeTurnBasedMatch.Version() != match.Version)
					{
						Logger.e(string.Format("Attempted to update a stale version of the match. Expected version was {0} but current version is {1}.", match.Version, nativeTurnBasedMatch.Version()));
						onFailure(false);
					}
					else
					{
						onVersionMatch(nativeTurnBasedMatch);
					}
				}
			});
		}

		private void FindEqualVersionMatchWithParticipant(GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch match, string participantId, Action<bool> onFailure, Action<GooglePlayGames.Native.PInvoke.MultiplayerParticipant, NativeTurnBasedMatch> onFoundParticipantAndMatch)
		{
			FindEqualVersionMatch(match, onFailure, (NativeTurnBasedMatch foundMatch) =>
			{
				if (participantId == null)
				{
					using (GooglePlayGames.Native.PInvoke.MultiplayerParticipant arg = GooglePlayGames.Native.PInvoke.MultiplayerParticipant.AutomatchingSentinel())
					{
						onFoundParticipantAndMatch(arg, foundMatch);
						return;
					}
				}
				using (GooglePlayGames.Native.PInvoke.MultiplayerParticipant multiplayerParticipant = foundMatch.ParticipantWithId(participantId))
				{
					if (multiplayerParticipant == null)
					{
						Logger.e(string.Format("Located match {0} but desired participant with ID {1} could not be found", match.MatchId, participantId));
						onFailure(false);
					}
					else
					{
						onFoundParticipantAndMatch(multiplayerParticipant, foundMatch);
					}
				}
			});
		}

		public int GetMaxMatchDataSize()
		{
			throw new NotImplementedException();
		}

		public void Finish(GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch match, byte[] data, MatchOutcome outcome, Action<bool> callback)
		{
			callback = GooglePlayGames.BasicApi.Callbacks.AsOnGameThreadCallback(callback);
			FindEqualVersionMatch(match, callback, (NativeTurnBasedMatch foundMatch) =>
			{
				GooglePlayGames.Native.PInvoke.ParticipantResults participantResults = foundMatch.Results();
				foreach (string participantId in outcome.ParticipantIds)
				{
					Types.MatchResult matchResult = ResultToMatchResult(outcome.GetResultFor(participantId));
					uint placementFor = outcome.GetPlacementFor(participantId);
					if (participantResults.HasResultsForParticipant(participantId))
					{
						Types.MatchResult matchResult2 = participantResults.ResultsForParticipant(participantId);
						uint num = participantResults.PlacingForParticipant(participantId);
						if (matchResult != matchResult2 || placementFor != num)
						{
							Logger.e(string.Format("Attempted to override existing results for participant {0}: Placing {1}, Result {2}", participantId, num, matchResult2));
							callback(false);
							return;
						}
					}
					else
					{
						GooglePlayGames.Native.PInvoke.ParticipantResults participantResults2 = participantResults;
						participantResults = participantResults2.WithResult(participantId, placementFor, matchResult);
						participantResults2.Dispose();
					}
				}
				mTurnBasedManager.FinishMatchDuringMyTurn(foundMatch, data, participantResults, (TurnBasedManager.TurnBasedMatchResponse response) =>
				{
					callback(response.RequestSucceeded());
				});
			});
		}

		private static Types.MatchResult ResultToMatchResult(MatchOutcome.ParticipantResult result)
		{
			switch (result)
			{
			case MatchOutcome.ParticipantResult.Loss:
				return Types.MatchResult.LOSS;
			case MatchOutcome.ParticipantResult.None:
				return Types.MatchResult.NONE;
			case MatchOutcome.ParticipantResult.Tie:
				return Types.MatchResult.TIE;
			case MatchOutcome.ParticipantResult.Win:
				return Types.MatchResult.WIN;
			default:
				Logger.e("Received unknown ParticipantResult " + result);
				return Types.MatchResult.NONE;
			}
		}

		public void AcknowledgeFinished(GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch match, Action<bool> callback)
		{
			callback = GooglePlayGames.BasicApi.Callbacks.AsOnGameThreadCallback(callback);
			FindEqualVersionMatch(match, callback, (NativeTurnBasedMatch foundMatch) =>
			{
				mTurnBasedManager.ConfirmPendingCompletion(foundMatch, (TurnBasedManager.TurnBasedMatchResponse response) =>
				{
					callback(response.RequestSucceeded());
				});
			});
		}

		public void Leave(GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch match, Action<bool> callback)
		{
			callback = GooglePlayGames.BasicApi.Callbacks.AsOnGameThreadCallback(callback);
			FindEqualVersionMatch(match, callback, (NativeTurnBasedMatch foundMatch) =>
			{
				mTurnBasedManager.LeaveMatchDuringTheirTurn(foundMatch, (CommonErrorStatus.MultiplayerStatus status) =>
				{
					callback(status > (CommonErrorStatus.MultiplayerStatus)0);
				});
			});
		}

		public void LeaveDuringTurn(GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch match, string pendingParticipantId, Action<bool> callback)
		{
			callback = GooglePlayGames.BasicApi.Callbacks.AsOnGameThreadCallback(callback);
			FindEqualVersionMatchWithParticipant(match, pendingParticipantId, callback, (GooglePlayGames.Native.PInvoke.MultiplayerParticipant pendingParticipant, NativeTurnBasedMatch foundMatch) =>
			{
				mTurnBasedManager.LeaveDuringMyTurn(foundMatch, pendingParticipant, (CommonErrorStatus.MultiplayerStatus status) =>
				{
					callback(status > (CommonErrorStatus.MultiplayerStatus)0);
				});
			});
		}

		public void Cancel(GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch match, Action<bool> callback)
		{
			callback = GooglePlayGames.BasicApi.Callbacks.AsOnGameThreadCallback(callback);
			FindEqualVersionMatch(match, callback, (NativeTurnBasedMatch foundMatch) =>
			{
				mTurnBasedManager.CancelMatch(foundMatch, (CommonErrorStatus.MultiplayerStatus status) =>
				{
					callback(status > (CommonErrorStatus.MultiplayerStatus)0);
				});
			});
		}

		public void Rematch(GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch match, Action<bool, GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch> callback)
		{
			callback = GooglePlayGames.BasicApi.Callbacks.AsOnGameThreadCallback(callback);
			FindEqualVersionMatch(match, (bool failed) =>
			{
				callback(false, null);
			}, (NativeTurnBasedMatch foundMatch) =>
			{
				mTurnBasedManager.Rematch(foundMatch, BridgeMatchToUserCallback((UIStatus status, GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch m) =>
				{
					callback(status == UIStatus.Valid, m);
				}));
			});
		}

		public void DeclineInvitation(string invitationId)
		{
			FindInvitationWithId(invitationId, (GooglePlayGames.Native.PInvoke.MultiplayerInvitation invitation) =>
			{
				if (invitation != null)
				{
					mTurnBasedManager.DeclineInvitation(invitation);
				}
			});
		}
	}
}
