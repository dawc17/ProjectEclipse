using System;
using System.Runtime.InteropServices;
using AOT;
using GooglePlayGames.Native.Cwrapper;
using GooglePlayGames.OurUtils;

namespace GooglePlayGames.Native.PInvoke
{
	internal class NativeMessageListenerHelper : BaseReferenceHolder
	{
		internal delegate void OnMessageReceived(long localClientId, string remoteEndpointId, byte[] data, bool isReliable);

		internal NativeMessageListenerHelper()
			: base(MessageListenerHelper.MessageListenerHelper_Construct())
		{
		}

		protected override void CallDispose(HandleRef selfPointer)
		{
			MessageListenerHelper.MessageListenerHelper_Dispose(selfPointer);
		}

		internal void SetOnMessageReceivedCallback(OnMessageReceived callback)
		{
			MessageListenerHelper.MessageListenerHelper_SetOnMessageReceivedCallback(SelfPtr(), InternalOnMessageReceivedCallback, Callbacks.ToIntPtr(callback));
		}

		[AOT.MonoPInvokeCallback(typeof(MessageListenerHelper.OnMessageReceivedCallback))]
		private static void InternalOnMessageReceivedCallback(long id, string name, IntPtr data, UIntPtr dataLength, bool isReliable, IntPtr userData)
		{
			OnMessageReceived onMessageReceived = Callbacks.IntPtrToPermanentCallback<OnMessageReceived>(userData);
			if (onMessageReceived == null)
			{
				return;
			}
			try
			{
				onMessageReceived(id, name, Callbacks.IntPtrAndSizeToByteArray(data, dataLength), isReliable);
			}
			catch (Exception ex)
			{
				Logger.e("Error encountered executing NativeMessageListenerHelper#InternalOnMessageReceivedCallback. Smothering to avoid passing exception into Native: " + ex);
			}
		}

		internal void SetOnDisconnectedCallback(Action<long, string> callback)
		{
			MessageListenerHelper.MessageListenerHelper_SetOnDisconnectedCallback(SelfPtr(), InternalOnDisconnectedCallback, Callbacks.ToIntPtr(callback));
		}

		[AOT.MonoPInvokeCallback(typeof(MessageListenerHelper.OnDisconnectedCallback))]
		private static void InternalOnDisconnectedCallback(long id, string lostEndpointId, IntPtr userData)
		{
			Action<long, string> action = Callbacks.IntPtrToPermanentCallback<Action<long, string>>(userData);
			if (action == null)
			{
				return;
			}
			try
			{
				action(id, lostEndpointId);
			}
			catch (Exception ex)
			{
				Logger.e("Error encountered executing NativeMessageListenerHelper#InternalOnDisconnectedCallback. Smothering to avoid passing exception into Native: " + ex);
			}
		}
	}
}
