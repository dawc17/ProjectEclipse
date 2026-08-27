using System;
using System.Runtime.InteropServices;
using AOT;
using GooglePlayGames.Native.Cwrapper;
using GooglePlayGames.OurUtils;

namespace GooglePlayGames.Native.PInvoke
{
	internal class GameServicesBuilder : BaseReferenceHolder
	{
		internal delegate void AuthFinishedCallback(Types.AuthOperation operation, CommonErrorStatus.AuthStatus status);

		internal delegate void AuthStartedCallback(Types.AuthOperation operation);

		private GameServicesBuilder(IntPtr selfPointer)
			: base(selfPointer)
		{
			InternalHooks.InternalHooks_ConfigureForUnityPlugin(SelfPtr(), "0.9.39a");
		}

		internal void SetOnAuthFinishedCallback(AuthFinishedCallback callback)
		{
			Builder.GameServices_Builder_SetOnAuthActionFinished(SelfPtr(), InternalAuthFinishedCallback, Callbacks.ToIntPtr(callback));
		}

		internal void EnableSnapshots()
		{
			Builder.GameServices_Builder_EnableSnapshots(SelfPtr());
		}

		internal void AddOauthScope(string scope)
		{
			Builder.GameServices_Builder_AddOauthScope(SelfPtr(), scope);
		}

		[AOT.MonoPInvokeCallback(typeof(Builder.OnAuthActionFinishedCallback))]
		private static void InternalAuthFinishedCallback(Types.AuthOperation op, CommonErrorStatus.AuthStatus status, IntPtr data)
		{
			AuthFinishedCallback authFinishedCallback = Callbacks.IntPtrToPermanentCallback<AuthFinishedCallback>(data);
			if (authFinishedCallback == null)
			{
				return;
			}
			try
			{
				authFinishedCallback(op, status);
			}
			catch (Exception ex)
			{
				Logger.e("Error encountered executing InternalAuthFinishedCallback. Smothering to avoid passing exception into Native: " + ex);
			}
		}

		internal void SetOnAuthStartedCallback(AuthStartedCallback callback)
		{
			Builder.GameServices_Builder_SetOnAuthActionStarted(SelfPtr(), InternalAuthStartedCallback, Callbacks.ToIntPtr(callback));
		}

		[AOT.MonoPInvokeCallback(typeof(Builder.OnAuthActionStartedCallback))]
		private static void InternalAuthStartedCallback(Types.AuthOperation op, IntPtr data)
		{
			AuthStartedCallback authStartedCallback = Callbacks.IntPtrToPermanentCallback<AuthStartedCallback>(data);
			try
			{
				if (authStartedCallback != null)
				{
					authStartedCallback(op);
				}
			}
			catch (Exception ex)
			{
				Logger.e("Error encountered executing InternalAuthStartedCallback. Smothering to avoid passing exception into Native: " + ex);
			}
		}

		internal void SetShowConnectingPopup(bool flag)
		{
			Builder.GameServices_Builder_SetShowConnectingPopup(SelfPtr(), flag);
		}

		protected override void CallDispose(HandleRef selfPointer)
		{
			Builder.GameServices_Builder_Dispose(selfPointer);
		}

		[AOT.MonoPInvokeCallback(typeof(Builder.OnTurnBasedMatchEventCallback))]
		private static void InternalOnTurnBasedMatchEventCallback(Types.MultiplayerEvent eventType, string matchId, IntPtr match, IntPtr userData)
		{
			Action<Types.MultiplayerEvent, string, NativeTurnBasedMatch> action = Callbacks.IntPtrToPermanentCallback<Action<Types.MultiplayerEvent, string, NativeTurnBasedMatch>>(userData);
			using (NativeTurnBasedMatch arg = NativeTurnBasedMatch.FromPointer(match))
			{
				try
				{
					if (action != null)
					{
						action(eventType, matchId, arg);
					}
				}
				catch (Exception ex)
				{
					Logger.e("Error encountered executing InternalOnTurnBasedMatchEventCallback. Smothering to avoid passing exception into Native: " + ex);
				}
			}
		}

		internal void SetOnTurnBasedMatchEventCallback(Action<Types.MultiplayerEvent, string, NativeTurnBasedMatch> callback)
		{
			IntPtr callback_arg = Callbacks.ToIntPtr(callback);
			Builder.GameServices_Builder_SetOnTurnBasedMatchEvent(SelfPtr(), InternalOnTurnBasedMatchEventCallback, callback_arg);
		}

		[AOT.MonoPInvokeCallback(typeof(Builder.OnMultiplayerInvitationEventCallback))]
		private static void InternalOnMultiplayerInvitationEventCallback(Types.MultiplayerEvent eventType, string matchId, IntPtr match, IntPtr userData)
		{
			Action<Types.MultiplayerEvent, string, MultiplayerInvitation> action = Callbacks.IntPtrToPermanentCallback<Action<Types.MultiplayerEvent, string, MultiplayerInvitation>>(userData);
			using (MultiplayerInvitation arg = MultiplayerInvitation.FromPointer(match))
			{
				try
				{
					if (action != null)
					{
						action(eventType, matchId, arg);
					}
				}
				catch (Exception ex)
				{
					Logger.e("Error encountered executing InternalOnMultiplayerInvitationEventCallback. Smothering to avoid passing exception into Native: " + ex);
				}
			}
		}

		internal void SetOnMultiplayerInvitationEventCallback(Action<Types.MultiplayerEvent, string, MultiplayerInvitation> callback)
		{
			IntPtr callback_arg = Callbacks.ToIntPtr(callback);
			Builder.GameServices_Builder_SetOnMultiplayerInvitationEvent(SelfPtr(), InternalOnMultiplayerInvitationEventCallback, callback_arg);
		}

		internal GameServices Build(PlatformConfiguration configRef)
		{
			IntPtr selfPointer = Builder.GameServices_Builder_Create(SelfPtr(), HandleRef.ToIntPtr(configRef.AsHandle()));
			if (selfPointer.Equals(IntPtr.Zero))
			{
				throw new InvalidOperationException("There was an error creating a GameServices object. Check for log errors from GamesNativeSDK");
			}
			return new GameServices(selfPointer);
		}

		internal static GameServicesBuilder Create()
		{
			IntPtr selfPointer = Builder.GameServices_Builder_Construct();
			return new GameServicesBuilder(selfPointer);
		}
	}
}
