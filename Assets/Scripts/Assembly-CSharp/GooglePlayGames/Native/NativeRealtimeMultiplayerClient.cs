using System;
using System.Collections.Generic;
using System.Linq;
using GooglePlayGames.BasicApi.Multiplayer;
using GooglePlayGames.Native.Cwrapper;
using GooglePlayGames.Native.PInvoke;
using GooglePlayGames.OurUtils;

namespace GooglePlayGames.Native
{
	public class NativeRealtimeMultiplayerClient : IRealTimeMultiplayerClient
	{
		private class NoopListener : RealTimeMultiplayerListener
		{
			public void OnRoomSetupProgress(float percent)
			{
			}

			public void OnRoomConnected(bool success)
			{
			}

			public void OnLeftRoom()
			{
			}

			public void OnParticipantLeft(Participant participant)
			{
			}

			public void OnPeersConnected(string[] participantIds)
			{
			}

			public void OnPeersDisconnected(string[] participantIds)
			{
			}

			public void OnRealTimeMessageReceived(bool isReliable, string senderId, byte[] data)
			{
			}
		}

		private class RoomSession
		{
			private readonly object mLifecycleLock = new object();

			private readonly OnGameThreadForwardingListener mListener;

			private readonly RealtimeManager mManager;

			private volatile string mCurrentPlayerId;

			private volatile State mState;

			private volatile bool mStillPreRoomCreation;

			private Invitation mInvitation;

			private volatile bool mShowingUI;

			private uint mMinPlayersToStart;

			internal bool ShowingUI
			{
				get
				{
					return mShowingUI;
				}
				set
				{
					mShowingUI = value;
				}
			}

			internal uint MinPlayersToStart
			{
				get
				{
					return mMinPlayersToStart;
				}
				set
				{
					mMinPlayersToStart = value;
				}
			}

			internal RoomSession(RealtimeManager manager, RealTimeMultiplayerListener listener)
			{
				mManager = Misc.CheckNotNull(manager);
				mListener = new OnGameThreadForwardingListener(listener);
				EnterState(new BeforeRoomCreateStartedState(this), false);
				mStillPreRoomCreation = true;
			}

			internal RealtimeManager Manager()
			{
				return mManager;
			}

			internal bool IsActive()
			{
				return mState.IsActive();
			}

			internal string SelfPlayerId()
			{
				return mCurrentPlayerId;
			}

			public void SetInvitation(Invitation invitation)
			{
				mInvitation = invitation;
			}

			public Invitation GetInvitation()
			{
				return mInvitation;
			}

			internal OnGameThreadForwardingListener OnGameThreadListener()
			{
				return mListener;
			}

			internal void EnterState(State handler)
			{
				EnterState(handler, true);
			}

			internal void EnterState(State handler, bool fireStateEnteredEvent)
			{
				lock (mLifecycleLock)
				{
					mState = Misc.CheckNotNull(handler);
					if (fireStateEnteredEvent)
					{
						Logger.d("Entering state: " + handler.GetType().Name);
						mState.OnStateEntered();
					}
				}
			}

			internal void LeaveRoom()
			{
				if (!ShowingUI)
				{
					lock (mLifecycleLock)
					{
						mState.LeaveRoom();
						return;
					}
				}
				Logger.d("Not leaving room since showing UI");
			}

			internal void ShowWaitingRoomUI()
			{
				mState.ShowWaitingRoomUI(MinPlayersToStart);
			}

			internal void StartRoomCreation(string currentPlayerId, Action createRoom)
			{
				lock (mLifecycleLock)
				{
					if (!mStillPreRoomCreation)
					{
						Logger.e("Room creation started more than once, this shouldn't happen!");
						return;
					}
					if (!mState.IsActive())
					{
						Logger.w("Received an attempt to create a room after the session was already torn down!");
						return;
					}
					mCurrentPlayerId = Misc.CheckNotNull(currentPlayerId);
					mStillPreRoomCreation = false;
					EnterState(new RoomCreationPendingState(this));
					createRoom();
				}
			}

			internal void OnRoomStatusChanged(NativeRealTimeRoom room)
			{
				lock (mLifecycleLock)
				{
					mState.OnRoomStatusChanged(room);
				}
			}

			internal void OnConnectedSetChanged(NativeRealTimeRoom room)
			{
				lock (mLifecycleLock)
				{
					mState.OnConnectedSetChanged(room);
				}
			}

			internal void OnParticipantStatusChanged(NativeRealTimeRoom room, GooglePlayGames.Native.PInvoke.MultiplayerParticipant participant)
			{
				lock (mLifecycleLock)
				{
					mState.OnParticipantStatusChanged(room, participant);
				}
			}

			internal void HandleRoomResponse(RealtimeManager.RealTimeRoomResponse response)
			{
				lock (mLifecycleLock)
				{
					mState.HandleRoomResponse(response);
				}
			}

			internal void OnDataReceived(NativeRealTimeRoom room, GooglePlayGames.Native.PInvoke.MultiplayerParticipant sender, byte[] data, bool isReliable)
			{
				mState.OnDataReceived(room, sender, data, isReliable);
			}

			internal void SendMessageToAll(bool reliable, byte[] data)
			{
				SendMessageToAll(reliable, data, 0, data.Length);
			}

			internal void SendMessageToAll(bool reliable, byte[] data, int offset, int length)
			{
				mState.SendToAll(data, offset, length, reliable);
			}

			internal void SendMessage(bool reliable, string participantId, byte[] data)
			{
				SendMessage(reliable, participantId, data, 0, data.Length);
			}

			internal void SendMessage(bool reliable, string participantId, byte[] data, int offset, int length)
			{
				mState.SendToSpecificRecipient(participantId, data, offset, length, reliable);
			}

			internal List<Participant> GetConnectedParticipants()
			{
				return mState.GetConnectedParticipants();
			}

			internal virtual Participant GetSelf()
			{
				return mState.GetSelf();
			}

			internal virtual Participant GetParticipant(string participantId)
			{
				return mState.GetParticipant(participantId);
			}

			internal virtual bool IsRoomConnected()
			{
				return mState.IsRoomConnected();
			}
		}

		private class OnGameThreadForwardingListener
		{
			private readonly RealTimeMultiplayerListener mListener;

			internal OnGameThreadForwardingListener(RealTimeMultiplayerListener listener)
			{
				mListener = Misc.CheckNotNull(listener);
			}

			public void RoomSetupProgress(float percent)
			{
				PlayGamesHelperObject.RunOnGameThread(() =>
				{
					mListener.OnRoomSetupProgress(percent);
				});
			}

			public void RoomConnected(bool success)
			{
				PlayGamesHelperObject.RunOnGameThread(() =>
				{
					mListener.OnRoomConnected(success);
				});
			}

			public void LeftRoom()
			{
				PlayGamesHelperObject.RunOnGameThread(() =>
				{
					mListener.OnLeftRoom();
				});
			}

			public void PeersConnected(string[] participantIds)
			{
				PlayGamesHelperObject.RunOnGameThread(() =>
				{
					mListener.OnPeersConnected(participantIds);
				});
			}

			public void PeersDisconnected(string[] participantIds)
			{
				PlayGamesHelperObject.RunOnGameThread(() =>
				{
					mListener.OnPeersDisconnected(participantIds);
				});
			}

			public void RealTimeMessageReceived(bool isReliable, string senderId, byte[] data)
			{
				PlayGamesHelperObject.RunOnGameThread(() =>
				{
					mListener.OnRealTimeMessageReceived(isReliable, senderId, data);
				});
			}

			public void ParticipantLeft(Participant participant)
			{
				PlayGamesHelperObject.RunOnGameThread(() =>
				{
					mListener.OnParticipantLeft(participant);
				});
			}
		}

		internal abstract class State
		{
			internal virtual void HandleRoomResponse(RealtimeManager.RealTimeRoomResponse response)
			{
				Logger.d(GetType().Name + ".HandleRoomResponse: Defaulting to no-op.");
			}

			internal virtual bool IsActive()
			{
				Logger.d(GetType().Name + ".IsNonPreemptable: Is preemptable by default.");
				return true;
			}

			internal virtual void LeaveRoom()
			{
				Logger.d(GetType().Name + ".LeaveRoom: Defaulting to no-op.");
			}

			internal virtual void ShowWaitingRoomUI(uint minimumParticipantsBeforeStarting)
			{
				Logger.d(GetType().Name + ".ShowWaitingRoomUI: Defaulting to no-op.");
			}

			internal virtual void OnStateEntered()
			{
				Logger.d(GetType().Name + ".OnStateEntered: Defaulting to no-op.");
			}

			internal virtual void OnRoomStatusChanged(NativeRealTimeRoom room)
			{
				Logger.d(GetType().Name + ".OnRoomStatusChanged: Defaulting to no-op.");
			}

			internal virtual void OnConnectedSetChanged(NativeRealTimeRoom room)
			{
				Logger.d(GetType().Name + ".OnConnectedSetChanged: Defaulting to no-op.");
			}

			internal virtual void OnParticipantStatusChanged(NativeRealTimeRoom room, GooglePlayGames.Native.PInvoke.MultiplayerParticipant participant)
			{
				Logger.d(GetType().Name + ".OnParticipantStatusChanged: Defaulting to no-op.");
			}

			internal virtual void OnDataReceived(NativeRealTimeRoom room, GooglePlayGames.Native.PInvoke.MultiplayerParticipant sender, byte[] data, bool isReliable)
			{
				Logger.d(GetType().Name + ".OnDataReceived: Defaulting to no-op.");
			}

			internal virtual void SendToSpecificRecipient(string recipientId, byte[] data, int offset, int length, bool isReliable)
			{
				Logger.d(GetType().Name + ".SendToSpecificRecipient: Defaulting to no-op.");
			}

			internal virtual void SendToAll(byte[] data, int offset, int length, bool isReliable)
			{
				Logger.d(GetType().Name + ".SendToApp: Defaulting to no-op.");
			}

			internal virtual List<Participant> GetConnectedParticipants()
			{
				Logger.d(GetType().Name + ".GetConnectedParticipants: Returning empty connected participants");
				return new List<Participant>();
			}

			internal virtual Participant GetSelf()
			{
				Logger.d(GetType().Name + ".GetSelf: Returning null self.");
				return null;
			}

			internal virtual Participant GetParticipant(string participantId)
			{
				Logger.d(GetType().Name + ".GetSelf: Returning null participant.");
				return null;
			}

			internal virtual bool IsRoomConnected()
			{
				Logger.d(GetType().Name + ".IsRoomConnected: Returning room not connected.");
				return false;
			}
		}

		private abstract class MessagingEnabledState : State
		{
			protected readonly RoomSession mSession;

			protected NativeRealTimeRoom mRoom;

			protected Dictionary<string, GooglePlayGames.Native.PInvoke.MultiplayerParticipant> mNativeParticipants;

			protected Dictionary<string, Participant> mParticipants;

			internal MessagingEnabledState(RoomSession session, NativeRealTimeRoom room)
			{
				mSession = Misc.CheckNotNull(session);
				UpdateCurrentRoom(room);
			}

			internal void UpdateCurrentRoom(NativeRealTimeRoom room)
			{
				if (mRoom != null)
				{
					mRoom.Dispose();
				}
				mRoom = Misc.CheckNotNull(room);
				mNativeParticipants = mRoom.Participants().ToDictionary((GooglePlayGames.Native.PInvoke.MultiplayerParticipant p) => p.Id());
				mParticipants = mNativeParticipants.Values.Select((GooglePlayGames.Native.PInvoke.MultiplayerParticipant p) => p.AsParticipant()).ToDictionary((Participant p) => p.ParticipantId);
			}

			internal sealed override void OnRoomStatusChanged(NativeRealTimeRoom room)
			{
				HandleRoomStatusChanged(room);
				UpdateCurrentRoom(room);
			}

			internal virtual void HandleRoomStatusChanged(NativeRealTimeRoom room)
			{
			}

			internal sealed override void OnConnectedSetChanged(NativeRealTimeRoom room)
			{
				HandleConnectedSetChanged(room);
				UpdateCurrentRoom(room);
			}

			internal virtual void HandleConnectedSetChanged(NativeRealTimeRoom room)
			{
			}

			internal sealed override void OnParticipantStatusChanged(NativeRealTimeRoom room, GooglePlayGames.Native.PInvoke.MultiplayerParticipant participant)
			{
				HandleParticipantStatusChanged(room, participant);
				UpdateCurrentRoom(room);
			}

			internal virtual void HandleParticipantStatusChanged(NativeRealTimeRoom room, GooglePlayGames.Native.PInvoke.MultiplayerParticipant participant)
			{
			}

			internal sealed override List<Participant> GetConnectedParticipants()
			{
				List<Participant> list = mParticipants.Values.Where((Participant p) => p.IsConnectedToRoom).ToList();
				list.Sort();
				return list;
			}

			internal override void SendToSpecificRecipient(string recipientId, byte[] data, int offset, int length, bool isReliable)
			{
				if (!mNativeParticipants.ContainsKey(recipientId))
				{
					Logger.e("Attempted to send message to unknown participant " + recipientId);
					return;
				}
				if (isReliable)
				{
					mSession.Manager().SendReliableMessage(mRoom, mNativeParticipants[recipientId], Misc.GetSubsetBytes(data, offset, length), null);
					return;
				}
				mSession.Manager().SendUnreliableMessageToSpecificParticipants(mRoom, new List<GooglePlayGames.Native.PInvoke.MultiplayerParticipant> { mNativeParticipants[recipientId] }, Misc.GetSubsetBytes(data, offset, length));
			}

			internal override void SendToAll(byte[] data, int offset, int length, bool isReliable)
			{
				byte[] subsetBytes = Misc.GetSubsetBytes(data, offset, length);
				if (isReliable)
				{
					foreach (string key in mNativeParticipants.Keys)
					{
						SendToSpecificRecipient(key, subsetBytes, 0, subsetBytes.Length, true);
					}
					return;
				}
				mSession.Manager().SendUnreliableMessageToAll(mRoom, subsetBytes);
			}

			internal override void OnDataReceived(NativeRealTimeRoom room, GooglePlayGames.Native.PInvoke.MultiplayerParticipant sender, byte[] data, bool isReliable)
			{
				mSession.OnGameThreadListener().RealTimeMessageReceived(isReliable, sender.Id(), data);
			}
		}

		private class BeforeRoomCreateStartedState : State
		{
			private readonly RoomSession mContainingSession;

			internal BeforeRoomCreateStartedState(RoomSession session)
			{
				mContainingSession = Misc.CheckNotNull(session);
			}

			internal override void LeaveRoom()
			{
				Logger.d("Session was torn down before room was created.");
				mContainingSession.OnGameThreadListener().RoomConnected(false);
				mContainingSession.EnterState(new ShutdownState(mContainingSession));
			}
		}

		private class RoomCreationPendingState : State
		{
			private readonly RoomSession mContainingSession;

			internal RoomCreationPendingState(RoomSession session)
			{
				mContainingSession = Misc.CheckNotNull(session);
			}

			internal override void HandleRoomResponse(RealtimeManager.RealTimeRoomResponse response)
			{
				if (!response.RequestSucceeded())
				{
					mContainingSession.EnterState(new ShutdownState(mContainingSession));
					mContainingSession.OnGameThreadListener().RoomConnected(false);
				}
				else
				{
					mContainingSession.EnterState(new ConnectingState(response.Room(), mContainingSession));
				}
			}

			internal override bool IsActive()
			{
				return true;
			}

			internal override void LeaveRoom()
			{
				Logger.d("Received request to leave room during room creation, aborting creation.");
				mContainingSession.EnterState(new AbortingRoomCreationState(mContainingSession));
			}
		}

		private class ConnectingState : MessagingEnabledState
		{
			private const float InitialPercentComplete = 20f;

			private static readonly HashSet<Types.ParticipantStatus> FailedStatuses = new HashSet<Types.ParticipantStatus>
			{
				Types.ParticipantStatus.DECLINED,
				Types.ParticipantStatus.LEFT
			};

			private HashSet<string> mConnectedParticipants = new HashSet<string>();

			private float mPercentComplete = 20f;

			private float mPercentPerParticipant;

			internal ConnectingState(NativeRealTimeRoom room, RoomSession session)
				: base(session, room)
			{
				mPercentPerParticipant = 80f / (float)session.MinPlayersToStart;
			}

			internal override void OnStateEntered()
			{
				mSession.OnGameThreadListener().RoomSetupProgress(mPercentComplete);
			}

			internal override void HandleConnectedSetChanged(NativeRealTimeRoom room)
			{
				HashSet<string> hashSet = new HashSet<string>();
				if ((room.Status() == Types.RealTimeRoomStatus.AUTO_MATCHING || room.Status() == Types.RealTimeRoomStatus.CONNECTING) && mSession.MinPlayersToStart <= room.ParticipantCount())
				{
					mSession.MinPlayersToStart += room.ParticipantCount();
					mPercentPerParticipant = 80f / (float)mSession.MinPlayersToStart;
				}
				foreach (GooglePlayGames.Native.PInvoke.MultiplayerParticipant item in room.Participants())
				{
					using (item)
					{
						if (item.IsConnectedToRoom())
						{
							hashSet.Add(item.Id());
						}
					}
				}
				if (mConnectedParticipants.Equals(hashSet))
				{
					Logger.w("Received connected set callback with unchanged connected set!");
					return;
				}
				IEnumerable<string> source = mConnectedParticipants.Except(hashSet);
				if (room.Status() == Types.RealTimeRoomStatus.DELETED)
				{
					Logger.e("Participants disconnected during room setup, failing. Participants were: " + string.Join(",", source.ToArray()));
					mSession.OnGameThreadListener().RoomConnected(false);
					mSession.EnterState(new ShutdownState(mSession));
					return;
				}
				IEnumerable<string> source2 = hashSet.Except(mConnectedParticipants);
				Logger.d("New participants connected: " + string.Join(",", source2.ToArray()));
				if (room.Status() == Types.RealTimeRoomStatus.ACTIVE)
				{
					Logger.d("Fully connected! Transitioning to active state.");
					mSession.EnterState(new ActiveState(room, mSession));
					mSession.OnGameThreadListener().RoomConnected(true);
				}
				else
				{
					mPercentComplete += mPercentPerParticipant * (float)source2.Count();
					mConnectedParticipants = hashSet;
					mSession.OnGameThreadListener().RoomSetupProgress(mPercentComplete);
				}
			}

			internal override void HandleParticipantStatusChanged(NativeRealTimeRoom room, GooglePlayGames.Native.PInvoke.MultiplayerParticipant participant)
			{
				if (FailedStatuses.Contains(participant.Status()))
				{
					mSession.OnGameThreadListener().ParticipantLeft(participant.AsParticipant());
					if (room.Status() != Types.RealTimeRoomStatus.CONNECTING && room.Status() != Types.RealTimeRoomStatus.AUTO_MATCHING)
					{
						LeaveRoom();
					}
				}
			}

			internal override void LeaveRoom()
			{
				mSession.EnterState(new LeavingRoom(mSession, mRoom, () =>
				{
					mSession.OnGameThreadListener().RoomConnected(false);
				}));
			}

			internal override void ShowWaitingRoomUI(uint minimumParticipantsBeforeStarting)
			{
				mSession.ShowingUI = true;
				mSession.Manager().ShowWaitingRoomUI(mRoom, minimumParticipantsBeforeStarting, (RealtimeManager.WaitingRoomUIResponse response) =>
				{
					mSession.ShowingUI = false;
					Logger.d("ShowWaitingRoomUI Response: " + response.ResponseStatus());
					if (response.ResponseStatus() == CommonErrorStatus.UIStatus.VALID)
					{
						Logger.d("Connecting state ShowWaitingRoomUI: room pcount:" + response.Room().ParticipantCount() + " status: " + response.Room().Status());
						if (response.Room().Status() == Types.RealTimeRoomStatus.ACTIVE)
						{
							mSession.EnterState(new ActiveState(response.Room(), mSession));
						}
					}
					else if (response.ResponseStatus() == CommonErrorStatus.UIStatus.ERROR_LEFT_ROOM)
					{
						LeaveRoom();
					}
					else
					{
						mSession.OnGameThreadListener().RoomSetupProgress(mPercentComplete);
					}
				});
			}
		}

		private class ActiveState : MessagingEnabledState
		{
			internal ActiveState(NativeRealTimeRoom room, RoomSession session)
				: base(session, room)
			{
			}

			internal override void OnStateEntered()
			{
				if (GetSelf() == null)
				{
					Logger.e("Room reached active state with unknown participant for the player");
					LeaveRoom();
				}
			}

			internal override bool IsRoomConnected()
			{
				return true;
			}

			internal override Participant GetParticipant(string participantId)
			{
				if (!mParticipants.ContainsKey(participantId))
				{
					Logger.e("Attempted to retrieve unknown participant " + participantId);
					return null;
				}
				return mParticipants[participantId];
			}

			internal override Participant GetSelf()
			{
				foreach (Participant value in mParticipants.Values)
				{
					if (value.Player != null && value.Player.id.Equals(mSession.SelfPlayerId()))
					{
						return value;
					}
				}
				return null;
			}

			internal override void HandleConnectedSetChanged(NativeRealTimeRoom room)
			{
				List<string> list = new List<string>();
				List<string> list2 = new List<string>();
				Dictionary<string, GooglePlayGames.Native.PInvoke.MultiplayerParticipant> dictionary = room.Participants().ToDictionary((GooglePlayGames.Native.PInvoke.MultiplayerParticipant p) => p.Id());
				foreach (string key in mNativeParticipants.Keys)
				{
					GooglePlayGames.Native.PInvoke.MultiplayerParticipant multiplayerParticipant = dictionary[key];
					GooglePlayGames.Native.PInvoke.MultiplayerParticipant multiplayerParticipant2 = mNativeParticipants[key];
					if (!multiplayerParticipant.IsConnectedToRoom())
					{
						list2.Add(key);
					}
					if (!multiplayerParticipant2.IsConnectedToRoom() && multiplayerParticipant.IsConnectedToRoom())
					{
						list.Add(key);
					}
				}
				foreach (GooglePlayGames.Native.PInvoke.MultiplayerParticipant value in mNativeParticipants.Values)
				{
					value.Dispose();
				}
				mNativeParticipants = dictionary;
				mParticipants = mNativeParticipants.Values.Select((GooglePlayGames.Native.PInvoke.MultiplayerParticipant p) => p.AsParticipant()).ToDictionary((Participant p) => p.ParticipantId);
				Logger.d("Updated participant statuses: " + string.Join(",", mParticipants.Values.Select((Participant p) => p.ToString()).ToArray()));
				if (list2.Contains(GetSelf().ParticipantId))
				{
					Logger.w("Player was disconnected from the multiplayer session.");
				}
				string selfId = GetSelf().ParticipantId;
				list = list.Where((string peerId) => !peerId.Equals(selfId)).ToList();
				list2 = list2.Where((string peerId) => !peerId.Equals(selfId)).ToList();
				if (list.Count > 0)
				{
					list.Sort();
					mSession.OnGameThreadListener().PeersConnected(list.Where((string peer) => !peer.Equals(selfId)).ToArray());
				}
				if (list2.Count > 0)
				{
					list2.Sort();
					mSession.OnGameThreadListener().PeersDisconnected(list2.Where((string peer) => !peer.Equals(selfId)).ToArray());
				}
			}

			internal override void LeaveRoom()
			{
				mSession.EnterState(new LeavingRoom(mSession, mRoom, () =>
				{
					mSession.OnGameThreadListener().LeftRoom();
				}));
			}
		}

		private class ShutdownState : State
		{
			private readonly RoomSession mSession;

			internal ShutdownState(RoomSession session)
			{
				mSession = Misc.CheckNotNull(session);
			}

			internal override bool IsActive()
			{
				return false;
			}

			internal override void LeaveRoom()
			{
				mSession.OnGameThreadListener().LeftRoom();
			}
		}

		private class LeavingRoom : State
		{
			private readonly RoomSession mSession;

			private readonly NativeRealTimeRoom mRoomToLeave;

			private readonly Action mLeavingCompleteCallback;

			internal LeavingRoom(RoomSession session, NativeRealTimeRoom room, Action leavingCompleteCallback)
			{
				mSession = Misc.CheckNotNull(session);
				mRoomToLeave = Misc.CheckNotNull(room);
				mLeavingCompleteCallback = Misc.CheckNotNull(leavingCompleteCallback);
			}

			internal override bool IsActive()
			{
				return false;
			}

			internal override void OnStateEntered()
			{
				mSession.Manager().LeaveRoom(mRoomToLeave, (CommonErrorStatus.ResponseStatus status) =>
				{
					mLeavingCompleteCallback();
					mSession.EnterState(new ShutdownState(mSession));
				});
			}
		}

		private class AbortingRoomCreationState : State
		{
			private readonly RoomSession mSession;

			internal AbortingRoomCreationState(RoomSession session)
			{
				mSession = Misc.CheckNotNull(session);
			}

			internal override bool IsActive()
			{
				return false;
			}

			internal override void HandleRoomResponse(RealtimeManager.RealTimeRoomResponse response)
			{
				if (!response.RequestSucceeded())
				{
					mSession.EnterState(new ShutdownState(mSession));
					mSession.OnGameThreadListener().RoomConnected(false);
				}
				else
				{
					mSession.EnterState(new LeavingRoom(mSession, response.Room(), () =>
					{
						mSession.OnGameThreadListener().RoomConnected(false);
					}));
				}
			}
		}

		private readonly object mSessionLock = new object();

		private readonly NativeClient mNativeClient;

		private readonly RealtimeManager mRealtimeManager;

		private volatile RoomSession mCurrentSession;

		internal NativeRealtimeMultiplayerClient(NativeClient nativeClient, RealtimeManager manager)
		{
			mNativeClient = Misc.CheckNotNull(nativeClient);
			mRealtimeManager = Misc.CheckNotNull(manager);
			mCurrentSession = GetTerminatedSession();
			PlayGamesHelperObject.AddPauseCallback(HandleAppPausing);
		}

		private RoomSession GetTerminatedSession()
		{
			RoomSession roomSession = new RoomSession(mRealtimeManager, new NoopListener());
			roomSession.EnterState(new ShutdownState(roomSession), false);
			return roomSession;
		}

		public void CreateQuickGame(uint minOpponents, uint maxOpponents, uint variant, RealTimeMultiplayerListener listener)
		{
			CreateQuickGame(minOpponents, maxOpponents, variant, 0uL, listener);
		}

		public void CreateQuickGame(uint minOpponents, uint maxOpponents, uint variant, ulong exclusiveBitMask, RealTimeMultiplayerListener listener)
		{
			lock (mSessionLock)
			{
				RoomSession newSession = new RoomSession(mRealtimeManager, listener);
				if (mCurrentSession.IsActive())
				{
					Logger.e("Received attempt to create a new room without cleaning up the old one.");
					newSession.LeaveRoom();
					return;
				}
				mCurrentSession = newSession;
				Logger.d("QuickGame: Setting MinPlayersToStart = " + minOpponents);
				mCurrentSession.MinPlayersToStart = minOpponents;
				using (RealtimeRoomConfigBuilder realtimeRoomConfigBuilder = RealtimeRoomConfigBuilder.Create())
				{
					RealtimeRoomConfig config = realtimeRoomConfigBuilder.SetMinimumAutomatchingPlayers(minOpponents).SetMaximumAutomatchingPlayers(maxOpponents).SetVariant(variant)
						.SetExclusiveBitMask(exclusiveBitMask)
						.Build();
					using (config)
					{
						GooglePlayGames.Native.PInvoke.RealTimeEventListenerHelper helper = HelperForSession(newSession);
						try
						{
							newSession.StartRoomCreation(mNativeClient.GetUserId(), () =>
							{
								mRealtimeManager.CreateRoom(config, helper, newSession.HandleRoomResponse);
							});
						}
						finally
						{
							if (helper != null)
							{
								((IDisposable)helper).Dispose();
							}
						}
					}
				}
			}
		}

		private static GooglePlayGames.Native.PInvoke.RealTimeEventListenerHelper HelperForSession(RoomSession session)
		{
			return GooglePlayGames.Native.PInvoke.RealTimeEventListenerHelper.Create().SetOnDataReceivedCallback((NativeRealTimeRoom room, GooglePlayGames.Native.PInvoke.MultiplayerParticipant participant, byte[] data, bool isReliable) =>
			{
				session.OnDataReceived(room, participant, data, isReliable);
			}).SetOnParticipantStatusChangedCallback((NativeRealTimeRoom room, GooglePlayGames.Native.PInvoke.MultiplayerParticipant participant) =>
			{
				session.OnParticipantStatusChanged(room, participant);
			})
				.SetOnRoomConnectedSetChangedCallback((NativeRealTimeRoom room) =>
				{
					session.OnConnectedSetChanged(room);
				})
				.SetOnRoomStatusChangedCallback((NativeRealTimeRoom room) =>
				{
					session.OnRoomStatusChanged(room);
				});
		}

		private void HandleAppPausing(bool paused)
		{
			if (paused)
			{
				Logger.d("Application is pausing, which disconnects the RTMP  client.  Leaving room.");
				LeaveRoom();
			}
		}

		public void CreateWithInvitationScreen(uint minOpponents, uint maxOppponents, uint variant, RealTimeMultiplayerListener listener)
		{
			lock (mSessionLock)
			{
				RoomSession newRoom = new RoomSession(mRealtimeManager, listener);
				if (mCurrentSession.IsActive())
				{
					Logger.e("Received attempt to create a new room without cleaning up the old one.");
					newRoom.LeaveRoom();
					return;
				}
				mCurrentSession = newRoom;
				mCurrentSession.ShowingUI = true;
				mRealtimeManager.ShowPlayerSelectUI(minOpponents, maxOppponents, true, (PlayerSelectUIResponse response) =>
				{
					mCurrentSession.ShowingUI = false;
					if (response.Status() != CommonErrorStatus.UIStatus.VALID)
					{
						Logger.d("User did not complete invitation screen.");
						newRoom.LeaveRoom();
						return;
					}
					mCurrentSession.MinPlayersToStart = (uint)((int)response.MinimumAutomatchingPlayers() + response.Count() + 1);
					using (RealtimeRoomConfigBuilder realtimeRoomConfigBuilder = RealtimeRoomConfigBuilder.Create())
					{
						realtimeRoomConfigBuilder.SetVariant(variant);
						realtimeRoomConfigBuilder.PopulateFromUIResponse(response);
						RealtimeRoomConfig config = realtimeRoomConfigBuilder.Build();
						try
						{
							GooglePlayGames.Native.PInvoke.RealTimeEventListenerHelper helper = HelperForSession(newRoom);
							try
							{
								newRoom.StartRoomCreation(mNativeClient.GetUserId(), () =>
								{
									mRealtimeManager.CreateRoom(config, helper, newRoom.HandleRoomResponse);
								});
							}
							finally
							{
								if (helper != null)
								{
									((IDisposable)helper).Dispose();
								}
							}
						}
						finally
						{
							if (config != null)
							{
								((IDisposable)config).Dispose();
							}
						}
					}
				});
			}
		}

		public void ShowWaitingRoomUI()
		{
			lock (mSessionLock)
			{
				mCurrentSession.ShowWaitingRoomUI();
			}
		}

		public void GetAllInvitations(Action<Invitation[]> callback)
		{
			mRealtimeManager.FetchInvitations((RealtimeManager.FetchInvitationsResponse response) =>
			{
				if (!response.RequestSucceeded())
				{
					Logger.e("Couldn't load invitations.");
					callback(new Invitation[0]);
				}
				else
				{
					List<Invitation> list = new List<Invitation>();
					foreach (GooglePlayGames.Native.PInvoke.MultiplayerInvitation item in response.Invitations())
					{
						using (item)
						{
							list.Add(item.AsInvitation());
						}
					}
					callback(list.ToArray());
				}
			});
		}

		public void AcceptFromInbox(RealTimeMultiplayerListener listener)
		{
			lock (mSessionLock)
			{
				RoomSession newRoom = new RoomSession(mRealtimeManager, listener);
				if (mCurrentSession.IsActive())
				{
					Logger.e("Received attempt to accept invitation without cleaning up active session.");
					newRoom.LeaveRoom();
					return;
				}
				mCurrentSession = newRoom;
				mCurrentSession.ShowingUI = true;
				mRealtimeManager.ShowRoomInboxUI((RealtimeManager.RoomInboxUIResponse response) =>
				{
					mCurrentSession.ShowingUI = false;
					if (response.ResponseStatus() != CommonErrorStatus.UIStatus.VALID)
					{
						Logger.d("User did not complete invitation screen.");
						newRoom.LeaveRoom();
						return;
					}
					GooglePlayGames.Native.PInvoke.MultiplayerInvitation invitation = response.Invitation();
					GooglePlayGames.Native.PInvoke.RealTimeEventListenerHelper helper = HelperForSession(newRoom);
					try
					{
						Logger.d("About to accept invitation " + invitation.Id());
						newRoom.StartRoomCreation(mNativeClient.GetUserId(), () =>
						{
							mRealtimeManager.AcceptInvitation(invitation, helper, (RealtimeManager.RealTimeRoomResponse acceptResponse) =>
							{
								using (invitation)
								{
									newRoom.HandleRoomResponse(acceptResponse);
									newRoom.SetInvitation(invitation.AsInvitation());
								}
							});
						});
					}
					finally
					{
						if (helper != null)
						{
							((IDisposable)helper).Dispose();
						}
					}
				});
			}
		}

		public void AcceptInvitation(string invitationId, RealTimeMultiplayerListener listener)
		{
			lock (mSessionLock)
			{
				RoomSession newRoom = new RoomSession(mRealtimeManager, listener);
				if (mCurrentSession.IsActive())
				{
					Logger.e("Received attempt to accept invitation without cleaning up active session.");
					newRoom.LeaveRoom();
					return;
				}
				mCurrentSession = newRoom;
				mRealtimeManager.FetchInvitations((RealtimeManager.FetchInvitationsResponse response) =>
				{
					if (!response.RequestSucceeded())
					{
						Logger.e("Couldn't load invitations.");
						newRoom.LeaveRoom();
					}
					else
					{
						foreach (GooglePlayGames.Native.PInvoke.MultiplayerInvitation invitation in response.Invitations())
						{
							using (invitation)
							{
								if (invitation.Id().Equals(invitationId))
								{
									mCurrentSession.MinPlayersToStart = invitation.AutomatchingSlots() + invitation.ParticipantCount();
									Logger.d("Setting MinPlayersToStart with invitation to : " + mCurrentSession.MinPlayersToStart);
									GooglePlayGames.Native.PInvoke.RealTimeEventListenerHelper helper = HelperForSession(newRoom);
									try
									{
										newRoom.StartRoomCreation(mNativeClient.GetUserId(), () =>
										{
											mRealtimeManager.AcceptInvitation(invitation, helper, newRoom.HandleRoomResponse);
										});
										return;
									}
									finally
									{
										if (helper != null)
										{
											((IDisposable)helper).Dispose();
										}
									}
								}
							}
						}
						Logger.e("Room creation failed since we could not find invitation with ID " + invitationId);
						newRoom.LeaveRoom();
					}
				});
			}
		}

		public Invitation GetInvitation()
		{
			return mCurrentSession.GetInvitation();
		}

		public void LeaveRoom()
		{
			mCurrentSession.LeaveRoom();
		}

		public void SendMessageToAll(bool reliable, byte[] data)
		{
			mCurrentSession.SendMessageToAll(reliable, data);
		}

		public void SendMessageToAll(bool reliable, byte[] data, int offset, int length)
		{
			mCurrentSession.SendMessageToAll(reliable, data, offset, length);
		}

		public void SendMessage(bool reliable, string participantId, byte[] data)
		{
			mCurrentSession.SendMessage(reliable, participantId, data);
		}

		public void SendMessage(bool reliable, string participantId, byte[] data, int offset, int length)
		{
			mCurrentSession.SendMessage(reliable, participantId, data, offset, length);
		}

		public List<Participant> GetConnectedParticipants()
		{
			return mCurrentSession.GetConnectedParticipants();
		}

		public Participant GetSelf()
		{
			return mCurrentSession.GetSelf();
		}

		public Participant GetParticipant(string participantId)
		{
			return mCurrentSession.GetParticipant(participantId);
		}

		public bool IsRoomConnected()
		{
			return mCurrentSession.IsRoomConnected();
		}

		public void DeclineInvitation(string invitationId)
		{
			mRealtimeManager.FetchInvitations((RealtimeManager.FetchInvitationsResponse response) =>
			{
				if (!response.RequestSucceeded())
				{
					Logger.e("Couldn't load invitations.");
					return;
				}
				foreach (GooglePlayGames.Native.PInvoke.MultiplayerInvitation item in response.Invitations())
				{
					using (item)
					{
						if (item.Id().Equals(invitationId))
						{
							mRealtimeManager.DeclineInvitation(item);
						}
					}
				}
			});
		}

		private static T WithDefault<T>(T presented, T defaultValue) where T : class
		{
			return (presented == null) ? defaultValue : presented;
		}
	}
}
