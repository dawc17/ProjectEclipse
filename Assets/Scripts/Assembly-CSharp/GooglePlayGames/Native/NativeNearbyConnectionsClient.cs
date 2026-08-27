using System;
using System.Collections.Generic;
using System.Linq;
using GooglePlayGames.BasicApi.Nearby;
using GooglePlayGames.Native.PInvoke;
using GooglePlayGames.OurUtils;

namespace GooglePlayGames.Native
{
	internal class NativeNearbyConnectionsClient : INearbyConnectionClient
	{
		protected class OnGameThreadMessageListener : IMessageListener
		{
			private readonly IMessageListener mListener;

			public OnGameThreadMessageListener(IMessageListener listener)
			{
				mListener = Misc.CheckNotNull(listener);
			}

			public void OnMessageReceived(string remoteEndpointId, byte[] data, bool isReliableMessage)
			{
				PlayGamesHelperObject.RunOnGameThread(() =>
				{
					mListener.OnMessageReceived(remoteEndpointId, data, isReliableMessage);
				});
			}

			public void OnRemoteEndpointDisconnected(string remoteEndpointId)
			{
				PlayGamesHelperObject.RunOnGameThread(() =>
				{
					mListener.OnRemoteEndpointDisconnected(remoteEndpointId);
				});
			}
		}

		protected class OnGameThreadDiscoveryListener : IDiscoveryListener
		{
			private readonly IDiscoveryListener mListener;

			public OnGameThreadDiscoveryListener(IDiscoveryListener listener)
			{
				mListener = Misc.CheckNotNull(listener);
			}

			public void OnEndpointFound(EndpointDetails discoveredEndpoint)
			{
				PlayGamesHelperObject.RunOnGameThread(() =>
				{
					mListener.OnEndpointFound(discoveredEndpoint);
				});
			}

			public void OnEndpointLost(string lostEndpointId)
			{
				PlayGamesHelperObject.RunOnGameThread(() =>
				{
					mListener.OnEndpointLost(lostEndpointId);
				});
			}
		}

		private readonly NearbyConnectionsManager mManager;

		internal NativeNearbyConnectionsClient(NearbyConnectionsManager manager)
		{
			mManager = Misc.CheckNotNull(manager);
		}

		public int MaxUnreliableMessagePayloadLength()
		{
			return 1168;
		}

		public int MaxReliableMessagePayloadLength()
		{
			return 4096;
		}

		public void SendReliable(List<string> recipientEndpointIds, byte[] payload)
		{
			InternalSend(recipientEndpointIds, payload, true);
		}

		public void SendUnreliable(List<string> recipientEndpointIds, byte[] payload)
		{
			InternalSend(recipientEndpointIds, payload, false);
		}

		private void InternalSend(List<string> recipientEndpointIds, byte[] payload, bool isReliable)
		{
			if (recipientEndpointIds == null)
			{
				throw new ArgumentNullException("recipientEndpointIds");
			}
			if (payload == null)
			{
				throw new ArgumentNullException("payload");
			}
			if (recipientEndpointIds.Contains(null))
			{
				throw new InvalidOperationException("Cannot send a message to a null recipient");
			}
			if (recipientEndpointIds.Count == 0)
			{
				Logger.w("Attempted to send a reliable message with no recipients");
				return;
			}
			if (isReliable)
			{
				if (payload.Length > MaxReliableMessagePayloadLength())
				{
					throw new InvalidOperationException("cannot send more than " + MaxReliableMessagePayloadLength() + " bytes");
				}
			}
			else if (payload.Length > MaxUnreliableMessagePayloadLength())
			{
				throw new InvalidOperationException("cannot send more than " + MaxUnreliableMessagePayloadLength() + " bytes");
			}
			foreach (string recipientEndpointId in recipientEndpointIds)
			{
				if (isReliable)
				{
					mManager.SendReliable(recipientEndpointId, payload);
				}
				else
				{
					mManager.SendUnreliable(recipientEndpointId, payload);
				}
			}
		}

		public void StartAdvertising(string name, List<string> appIdentifiers, TimeSpan? advertisingDuration, Action<AdvertisingResult> resultCallback, Action<ConnectionRequest> requestCallback)
		{
			Misc.CheckNotNull(appIdentifiers, "appIdentifiers");
			Misc.CheckNotNull(resultCallback, "resultCallback");
			Misc.CheckNotNull(requestCallback, "connectionRequestCallback");
			if (advertisingDuration.HasValue && advertisingDuration.Value.Ticks < 0)
			{
				throw new InvalidOperationException("advertisingDuration must be positive");
			}
			resultCallback = GooglePlayGames.BasicApi.Callbacks.AsOnGameThreadCallback(resultCallback);
			requestCallback = GooglePlayGames.BasicApi.Callbacks.AsOnGameThreadCallback(requestCallback);
			mManager.StartAdvertising(name, appIdentifiers.Select(NativeAppIdentifier.FromString).ToList(), ToTimeoutMillis(advertisingDuration), (long localClientId, NativeStartAdvertisingResult result) =>
			{
				resultCallback(result.AsResult());
			}, (long localClientId, NativeConnectionRequest request) =>
			{
				requestCallback(request.AsRequest());
			});
		}

		private static long ToTimeoutMillis(TimeSpan? span)
		{
			return (!span.HasValue) ? 0 : PInvokeUtilities.ToMilliseconds(span.Value);
		}

		public void StopAdvertising()
		{
			mManager.StopAdvertising();
		}

		public void SendConnectionRequest(string name, string remoteEndpointId, byte[] payload, Action<ConnectionResponse> responseCallback, IMessageListener listener)
		{
			Misc.CheckNotNull(remoteEndpointId, "remoteEndpointId");
			Misc.CheckNotNull(payload, "payload");
			Misc.CheckNotNull(responseCallback, "responseCallback");
			Misc.CheckNotNull(listener, "listener");
			responseCallback = GooglePlayGames.BasicApi.Callbacks.AsOnGameThreadCallback(responseCallback);
			using (NativeMessageListenerHelper listener2 = ToMessageListener(listener))
			{
				mManager.SendConnectionRequest(name, remoteEndpointId, payload, (long localClientId, NativeConnectionResponse response) =>
				{
					responseCallback(response.AsResponse(localClientId));
				}, listener2);
			}
		}

		private static NativeMessageListenerHelper ToMessageListener(IMessageListener listener)
		{
			listener = new OnGameThreadMessageListener(listener);
			NativeMessageListenerHelper nativeMessageListenerHelper = new NativeMessageListenerHelper();
			nativeMessageListenerHelper.SetOnMessageReceivedCallback((long localClientId, string endpointId, byte[] data, bool isReliable) =>
			{
				listener.OnMessageReceived(endpointId, data, isReliable);
			});
			nativeMessageListenerHelper.SetOnDisconnectedCallback((long localClientId, string endpointId) =>
			{
				listener.OnRemoteEndpointDisconnected(endpointId);
			});
			return nativeMessageListenerHelper;
		}

		public void AcceptConnectionRequest(string remoteEndpointId, byte[] payload, IMessageListener listener)
		{
			Misc.CheckNotNull(remoteEndpointId, "remoteEndpointId");
			Misc.CheckNotNull(payload, "payload");
			Misc.CheckNotNull(listener, "listener");
			Logger.d("Calling AcceptConncectionRequest");
			mManager.AcceptConnectionRequest(remoteEndpointId, payload, ToMessageListener(listener));
			Logger.d("Called!");
		}

		public void StartDiscovery(string serviceId, TimeSpan? advertisingTimeout, IDiscoveryListener listener)
		{
			Misc.CheckNotNull(serviceId, "serviceId");
			Misc.CheckNotNull(listener, "listener");
			using (NativeEndpointDiscoveryListenerHelper listener2 = ToDiscoveryListener(listener))
			{
				mManager.StartDiscovery(serviceId, ToTimeoutMillis(advertisingTimeout), listener2);
			}
		}

		private static NativeEndpointDiscoveryListenerHelper ToDiscoveryListener(IDiscoveryListener listener)
		{
			listener = new OnGameThreadDiscoveryListener(listener);
			NativeEndpointDiscoveryListenerHelper nativeEndpointDiscoveryListenerHelper = new NativeEndpointDiscoveryListenerHelper();
			nativeEndpointDiscoveryListenerHelper.SetOnEndpointFound((long localClientId, NativeEndpointDetails endpoint) =>
			{
				listener.OnEndpointFound(endpoint.ToDetails());
			});
			nativeEndpointDiscoveryListenerHelper.SetOnEndpointLostCallback((long localClientId, string lostEndpointId) =>
			{
				listener.OnEndpointLost(lostEndpointId);
			});
			return nativeEndpointDiscoveryListenerHelper;
		}

		public void StopDiscovery(string serviceId)
		{
			Misc.CheckNotNull(serviceId, "serviceId");
			mManager.StopDiscovery(serviceId);
		}

		public void RejectConnectionRequest(string requestingEndpointId)
		{
			Misc.CheckNotNull(requestingEndpointId, "requestingEndpointId");
			mManager.RejectConnectionRequest(requestingEndpointId);
		}

		public void DisconnectFromEndpoint(string remoteEndpointId)
		{
			mManager.DisconnectFromEndpoint(remoteEndpointId);
		}

		public void StopAllConnections()
		{
			mManager.StopAllConnections();
		}

		public string LocalEndpointId()
		{
			return mManager.LocalEndpointId();
		}

		public string LocalDeviceId()
		{
			return mManager.LocalDeviceId();
		}

		public string GetAppBundleId()
		{
			return mManager.AppBundleId;
		}

		public string GetServiceId()
		{
			return NearbyConnectionsManager.ServiceId;
		}
	}
}
