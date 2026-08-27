using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using AOT;
using GooglePlayGames.Native.Cwrapper;
using UnityEngine;

namespace GooglePlayGames.Native.PInvoke
{
	internal class NearbyConnectionsManager : BaseReferenceHolder
	{
		private static readonly string sServiceId = ReadServiceId();

		public string AppBundleId
		{
			get
			{
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
					return androidJavaObject.Call<string>("getPackageName", new object[0]);
				}
			}
		}

		public static string ServiceId
		{
			get
			{
				return sServiceId;
			}
		}

		internal NearbyConnectionsManager(IntPtr selfPointer)
			: base(selfPointer)
		{
		}

		protected override void CallDispose(HandleRef selfPointer)
		{
			NearbyConnections.NearbyConnections_Dispose(selfPointer);
		}

		internal void SendUnreliable(string remoteEndpointId, byte[] payload)
		{
			NearbyConnections.NearbyConnections_SendUnreliableMessage(SelfPtr(), remoteEndpointId, payload, new UIntPtr((ulong)payload.Length));
		}

		internal void SendReliable(string remoteEndpointId, byte[] payload)
		{
			NearbyConnections.NearbyConnections_SendReliableMessage(SelfPtr(), remoteEndpointId, payload, new UIntPtr((ulong)payload.Length));
		}

		internal void StartAdvertising(string name, List<NativeAppIdentifier> appIds, long advertisingDuration, Action<long, NativeStartAdvertisingResult> advertisingCallback, Action<long, NativeConnectionRequest> connectionRequestCallback)
		{
			NearbyConnections.NearbyConnections_StartAdvertising(SelfPtr(), name, appIds.Select((NativeAppIdentifier id) => id.AsPointer()).ToArray(), new UIntPtr((ulong)appIds.Count), advertisingDuration, InternalStartAdvertisingCallback, Callbacks.ToIntPtr(advertisingCallback, NativeStartAdvertisingResult.FromPointer), InternalConnectionRequestCallback, Callbacks.ToIntPtr(connectionRequestCallback, NativeConnectionRequest.FromPointer));
		}

		[AOT.MonoPInvokeCallback(typeof(NearbyConnectionTypes.StartAdvertisingCallback))]
		private static void InternalStartAdvertisingCallback(long id, IntPtr result, IntPtr userData)
		{
			Callbacks.PerformInternalCallback("NearbyConnectionsManager#InternalStartAdvertisingCallback", Callbacks.Type.Permanent, id, result, userData);
		}

		[AOT.MonoPInvokeCallback(typeof(NearbyConnectionTypes.ConnectionRequestCallback))]
		private static void InternalConnectionRequestCallback(long id, IntPtr result, IntPtr userData)
		{
			Callbacks.PerformInternalCallback("NearbyConnectionsManager#InternalConnectionRequestCallback", Callbacks.Type.Permanent, id, result, userData);
		}

		internal void StopAdvertising()
		{
			NearbyConnections.NearbyConnections_StopAdvertising(SelfPtr());
		}

		internal void SendConnectionRequest(string name, string remoteEndpointId, byte[] payload, Action<long, NativeConnectionResponse> callback, NativeMessageListenerHelper listener)
		{
			NearbyConnections.NearbyConnections_SendConnectionRequest(SelfPtr(), name, remoteEndpointId, payload, new UIntPtr((ulong)payload.Length), InternalConnectResponseCallback, Callbacks.ToIntPtr(callback, NativeConnectionResponse.FromPointer), listener.AsPointer());
		}

		[AOT.MonoPInvokeCallback(typeof(NearbyConnectionTypes.ConnectionResponseCallback))]
		private static void InternalConnectResponseCallback(long localClientId, IntPtr response, IntPtr userData)
		{
			Callbacks.PerformInternalCallback("NearbyConnectionManager#InternalConnectResponseCallback", Callbacks.Type.Temporary, localClientId, response, userData);
		}

		internal void AcceptConnectionRequest(string remoteEndpointId, byte[] payload, NativeMessageListenerHelper listener)
		{
			NearbyConnections.NearbyConnections_AcceptConnectionRequest(SelfPtr(), remoteEndpointId, payload, new UIntPtr((ulong)payload.Length), listener.AsPointer());
		}

		internal void DisconnectFromEndpoint(string remoteEndpointId)
		{
			NearbyConnections.NearbyConnections_Disconnect(SelfPtr(), remoteEndpointId);
		}

		internal void StopAllConnections()
		{
			NearbyConnections.NearbyConnections_Stop(SelfPtr());
		}

		internal void StartDiscovery(string serviceId, long duration, NativeEndpointDiscoveryListenerHelper listener)
		{
			NearbyConnections.NearbyConnections_StartDiscovery(SelfPtr(), serviceId, duration, listener.AsPointer());
		}

		internal void StopDiscovery(string serviceId)
		{
			NearbyConnections.NearbyConnections_StopDiscovery(SelfPtr(), serviceId);
		}

		internal void RejectConnectionRequest(string remoteEndpointId)
		{
			NearbyConnections.NearbyConnections_RejectConnectionRequest(SelfPtr(), remoteEndpointId);
		}

		internal void Shutdown()
		{
			NearbyConnections.NearbyConnections_Stop(SelfPtr());
		}

		internal string LocalEndpointId()
		{
			return PInvokeUtilities.OutParamsToString((byte[] out_arg, UIntPtr out_size) => NearbyConnections.NearbyConnections_GetLocalEndpointId(SelfPtr(), out_arg, out_size));
		}

		internal string LocalDeviceId()
		{
			return PInvokeUtilities.OutParamsToString((byte[] out_arg, UIntPtr out_size) => NearbyConnections.NearbyConnections_GetLocalDeviceId(SelfPtr(), out_arg, out_size));
		}

		internal static string ReadServiceId()
		{
			Debug.Log("Initializing ServiceId property!!!!");
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
				{
					string text = androidJavaObject.Call<string>("getPackageName", new object[0]);
					AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("getPackageManager", new object[0]);
					AndroidJavaObject androidJavaObject3 = androidJavaObject2.Call<AndroidJavaObject>("getApplicationInfo", new object[2] { text, 128 });
					AndroidJavaObject androidJavaObject4 = androidJavaObject3.Get<AndroidJavaObject>("metaData");
					string text2 = androidJavaObject4.Call<string>("getString", new object[1] { "com.google.android.gms.nearby.connection.SERVICE_ID" });
					Debug.Log("SystemId from Manifest: " + text2);
					return text2;
				}
			}
		}
	}
}
