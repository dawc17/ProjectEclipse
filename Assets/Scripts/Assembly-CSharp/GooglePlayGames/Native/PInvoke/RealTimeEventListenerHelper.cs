using System;
using System.Runtime.InteropServices;
using AOT;
using GooglePlayGames.Native.Cwrapper;
using GooglePlayGames.OurUtils;

namespace GooglePlayGames.Native.PInvoke
{
	internal class RealTimeEventListenerHelper : BaseReferenceHolder
	{
		internal RealTimeEventListenerHelper(IntPtr selfPointer)
			: base(selfPointer)
		{
		}

		protected override void CallDispose(HandleRef selfPointer)
		{
			GooglePlayGames.Native.Cwrapper.RealTimeEventListenerHelper.RealTimeEventListenerHelper_Dispose(selfPointer);
		}

		internal RealTimeEventListenerHelper SetOnRoomStatusChangedCallback(Action<NativeRealTimeRoom> callback)
		{
			GooglePlayGames.Native.Cwrapper.RealTimeEventListenerHelper.RealTimeEventListenerHelper_SetOnRoomStatusChangedCallback(SelfPtr(), InternalOnRoomStatusChangedCallback, ToCallbackPointer(callback));
			return this;
		}

		[AOT.MonoPInvokeCallback(typeof(GooglePlayGames.Native.Cwrapper.RealTimeEventListenerHelper.OnRoomStatusChangedCallback))]
		internal static void InternalOnRoomStatusChangedCallback(IntPtr response, IntPtr data)
		{
			Callbacks.PerformInternalCallback("RealTimeEventListenerHelper#InternalOnRoomStatusChangedCallback", Callbacks.Type.Permanent, response, data);
		}

		internal RealTimeEventListenerHelper SetOnRoomConnectedSetChangedCallback(Action<NativeRealTimeRoom> callback)
		{
			GooglePlayGames.Native.Cwrapper.RealTimeEventListenerHelper.RealTimeEventListenerHelper_SetOnRoomConnectedSetChangedCallback(SelfPtr(), InternalOnRoomConnectedSetChangedCallback, ToCallbackPointer(callback));
			return this;
		}

		[AOT.MonoPInvokeCallback(typeof(GooglePlayGames.Native.Cwrapper.RealTimeEventListenerHelper.OnRoomConnectedSetChangedCallback))]
		internal static void InternalOnRoomConnectedSetChangedCallback(IntPtr response, IntPtr data)
		{
			Callbacks.PerformInternalCallback("RealTimeEventListenerHelper#InternalOnRoomConnectedSetChangedCallback", Callbacks.Type.Permanent, response, data);
		}

		internal RealTimeEventListenerHelper SetOnP2PConnectedCallback(Action<NativeRealTimeRoom, MultiplayerParticipant> callback)
		{
			GooglePlayGames.Native.Cwrapper.RealTimeEventListenerHelper.RealTimeEventListenerHelper_SetOnP2PConnectedCallback(SelfPtr(), InternalOnP2PConnectedCallback, Callbacks.ToIntPtr(callback));
			return this;
		}

		[AOT.MonoPInvokeCallback(typeof(GooglePlayGames.Native.Cwrapper.RealTimeEventListenerHelper.OnP2PConnectedCallback))]
		internal static void InternalOnP2PConnectedCallback(IntPtr room, IntPtr participant, IntPtr data)
		{
			PerformRoomAndParticipantCallback("InternalOnP2PConnectedCallback", room, participant, data);
		}

		internal RealTimeEventListenerHelper SetOnP2PDisconnectedCallback(Action<NativeRealTimeRoom, MultiplayerParticipant> callback)
		{
			GooglePlayGames.Native.Cwrapper.RealTimeEventListenerHelper.RealTimeEventListenerHelper_SetOnP2PDisconnectedCallback(SelfPtr(), InternalOnP2PDisconnectedCallback, Callbacks.ToIntPtr(callback));
			return this;
		}

		[AOT.MonoPInvokeCallback(typeof(GooglePlayGames.Native.Cwrapper.RealTimeEventListenerHelper.OnP2PDisconnectedCallback))]
		internal static void InternalOnP2PDisconnectedCallback(IntPtr room, IntPtr participant, IntPtr data)
		{
			PerformRoomAndParticipantCallback("InternalOnP2PDisconnectedCallback", room, participant, data);
		}

		internal RealTimeEventListenerHelper SetOnParticipantStatusChangedCallback(Action<NativeRealTimeRoom, MultiplayerParticipant> callback)
		{
			GooglePlayGames.Native.Cwrapper.RealTimeEventListenerHelper.RealTimeEventListenerHelper_SetOnParticipantStatusChangedCallback(SelfPtr(), InternalOnParticipantStatusChangedCallback, Callbacks.ToIntPtr(callback));
			return this;
		}

		[AOT.MonoPInvokeCallback(typeof(GooglePlayGames.Native.Cwrapper.RealTimeEventListenerHelper.OnParticipantStatusChangedCallback))]
		internal static void InternalOnParticipantStatusChangedCallback(IntPtr room, IntPtr participant, IntPtr data)
		{
			PerformRoomAndParticipantCallback("InternalOnParticipantStatusChangedCallback", room, participant, data);
		}

		internal static void PerformRoomAndParticipantCallback(string callbackName, IntPtr room, IntPtr participant, IntPtr data)
		{
			Logger.d("Entering " + callbackName);
			try
			{
				NativeRealTimeRoom arg = NativeRealTimeRoom.FromPointer(room);
				using (MultiplayerParticipant arg2 = MultiplayerParticipant.FromPointer(participant))
				{
					Action<NativeRealTimeRoom, MultiplayerParticipant> action = Callbacks.IntPtrToPermanentCallback<Action<NativeRealTimeRoom, MultiplayerParticipant>>(data);
					if (action != null)
					{
						action(arg, arg2);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.e("Error encountered executing " + callbackName + ". Smothering to avoid passing exception into Native: " + ex);
			}
		}

		internal RealTimeEventListenerHelper SetOnDataReceivedCallback(Action<NativeRealTimeRoom, MultiplayerParticipant, byte[], bool> callback)
		{
			IntPtr callback_arg = Callbacks.ToIntPtr(callback);
			Logger.d("OnData Callback has addr: " + callback_arg.ToInt64());
			GooglePlayGames.Native.Cwrapper.RealTimeEventListenerHelper.RealTimeEventListenerHelper_SetOnDataReceivedCallback(SelfPtr(), InternalOnDataReceived, callback_arg);
			return this;
		}

		[AOT.MonoPInvokeCallback(typeof(GooglePlayGames.Native.Cwrapper.RealTimeEventListenerHelper.OnDataReceivedCallback))]
		internal static void InternalOnDataReceived(IntPtr room, IntPtr participant, IntPtr data, UIntPtr dataLength, bool isReliable, IntPtr userData)
		{
			Logger.d("Entering InternalOnDataReceived: " + userData.ToInt64());
			Action<NativeRealTimeRoom, MultiplayerParticipant, byte[], bool> action = Callbacks.IntPtrToPermanentCallback<Action<NativeRealTimeRoom, MultiplayerParticipant, byte[], bool>>(userData);
			using (NativeRealTimeRoom arg = NativeRealTimeRoom.FromPointer(room))
			{
				using (MultiplayerParticipant arg2 = MultiplayerParticipant.FromPointer(participant))
				{
					if (action == null)
					{
						return;
					}
					byte[] array = null;
					if (dataLength.ToUInt64() != 0)
					{
						array = new byte[dataLength.ToUInt32()];
						Marshal.Copy(data, array, 0, (int)dataLength.ToUInt32());
					}
					try
					{
						action(arg, arg2, array, isReliable);
					}
					catch (Exception ex)
					{
						Logger.e("Error encountered executing InternalOnDataReceived. Smothering to avoid passing exception into Native: " + ex);
					}
				}
			}
		}

		private static IntPtr ToCallbackPointer(Action<NativeRealTimeRoom> callback)
		{
			Action<IntPtr> callback2 = (IntPtr result) =>
			{
				NativeRealTimeRoom nativeRealTimeRoom = NativeRealTimeRoom.FromPointer(result);
				if (callback != null)
				{
					callback(nativeRealTimeRoom);
				}
				else if (nativeRealTimeRoom != null)
				{
					nativeRealTimeRoom.Dispose();
				}
			};
			return Callbacks.ToIntPtr(callback2);
		}

		internal static RealTimeEventListenerHelper Create()
		{
			return new RealTimeEventListenerHelper(GooglePlayGames.Native.Cwrapper.RealTimeEventListenerHelper.RealTimeEventListenerHelper_Construct());
		}
	}
}
