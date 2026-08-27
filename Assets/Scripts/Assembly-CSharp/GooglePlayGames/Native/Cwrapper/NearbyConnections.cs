using System;
using System.Runtime.InteropServices;

namespace GooglePlayGames.Native.Cwrapper
{
	internal static class NearbyConnections
	{
		[DllImport("gpg")]
		internal static extern void NearbyConnections_StartDiscovery(HandleRef self, string service_id, long duration, IntPtr helper);

		[DllImport("gpg")]
		internal static extern void NearbyConnections_RejectConnectionRequest(HandleRef self, string remote_endpoint_id);

		[DllImport("gpg")]
		internal static extern void NearbyConnections_Disconnect(HandleRef self, string remote_endpoint_id);

		[DllImport("gpg")]
		internal static extern void NearbyConnections_SendUnreliableMessage(HandleRef self, string remote_endpoint_id, byte[] payload, UIntPtr payload_size);

		[DllImport("gpg")]
		internal static extern UIntPtr NearbyConnections_GetLocalDeviceId(HandleRef self, [In][Out] byte[] out_arg, UIntPtr out_size);

		[DllImport("gpg")]
		internal static extern void NearbyConnections_StopAdvertising(HandleRef self);

		[DllImport("gpg")]
		internal static extern void NearbyConnections_Dispose(HandleRef self);

		[DllImport("gpg")]
		internal static extern UIntPtr NearbyConnections_GetLocalEndpointId(HandleRef self, [In][Out] byte[] out_arg, UIntPtr out_size);

		[DllImport("gpg")]
		internal static extern void NearbyConnections_SendReliableMessage(HandleRef self, string remote_endpoint_id, byte[] payload, UIntPtr payload_size);

		[DllImport("gpg")]
		internal static extern void NearbyConnections_StopDiscovery(HandleRef self, string service_id);

		[DllImport("gpg")]
		internal static extern void NearbyConnections_SendConnectionRequest(HandleRef self, string name, string remote_endpoint_id, byte[] payload, UIntPtr payload_size, NearbyConnectionTypes.ConnectionResponseCallback callback, IntPtr callback_arg, IntPtr helper);

		[DllImport("gpg")]
		internal static extern void NearbyConnections_StartAdvertising(HandleRef self, string name, IntPtr[] app_identifiers, UIntPtr app_identifiers_size, long duration, NearbyConnectionTypes.StartAdvertisingCallback start_advertising_callback, IntPtr start_advertising_callback_arg, NearbyConnectionTypes.ConnectionRequestCallback request_callback, IntPtr request_callback_arg);

		[DllImport("gpg")]
		internal static extern void NearbyConnections_Stop(HandleRef self);

		[DllImport("gpg")]
		internal static extern void NearbyConnections_AcceptConnectionRequest(HandleRef self, string remote_endpoint_id, byte[] payload, UIntPtr payload_size, IntPtr helper);
	}
}
