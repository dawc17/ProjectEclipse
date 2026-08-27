using System;
using System.Runtime.InteropServices;

namespace GooglePlayGames.Native.Cwrapper
{
	internal static class NearbyConnectionTypes
	{
		internal enum ConnectionResponse_ResponseCode
		{
			ACCEPTED = 1,
			REJECTED = 2,
			ERROR_INTERNAL = -1,
			ERROR_NETWORK_NOT_CONNECTED = -2,
			ERROR_ENDPOINT_ALREADY_CONNECTED = -3,
			ERROR_ENDPOINT_NOT_CONNECTED = -4
		}

		internal delegate void ConnectionRequestCallback(long arg0, IntPtr arg1, IntPtr arg2);

		internal delegate void StartAdvertisingCallback(long arg0, IntPtr arg1, IntPtr arg2);

		internal delegate void ConnectionResponseCallback(long arg0, IntPtr arg1, IntPtr arg2);

		[DllImport("gpg")]
		internal static extern void AppIdentifier_Dispose(HandleRef self);

		[DllImport("gpg")]
		internal static extern UIntPtr AppIdentifier_GetIdentifier(HandleRef self, [In][Out] byte[] out_arg, UIntPtr out_size);

		[DllImport("gpg")]
		internal static extern void StartAdvertisingResult_Dispose(HandleRef self);

		[DllImport("gpg")]
		[return: MarshalAs(UnmanagedType.I4)]
		internal static extern int StartAdvertisingResult_GetStatus(HandleRef self);

		[DllImport("gpg")]
		internal static extern UIntPtr StartAdvertisingResult_GetLocalEndpointName(HandleRef self, [In][Out] byte[] out_arg, UIntPtr out_size);

		[DllImport("gpg")]
		internal static extern void EndpointDetails_Dispose(HandleRef self);

		[DllImport("gpg")]
		internal static extern UIntPtr EndpointDetails_GetEndpointId(HandleRef self, [In][Out] byte[] out_arg, UIntPtr out_size);

		[DllImport("gpg")]
		internal static extern UIntPtr EndpointDetails_GetDeviceId(HandleRef self, [In][Out] byte[] out_arg, UIntPtr out_size);

		[DllImport("gpg")]
		internal static extern UIntPtr EndpointDetails_GetName(HandleRef self, [In][Out] byte[] out_arg, UIntPtr out_size);

		[DllImport("gpg")]
		internal static extern UIntPtr EndpointDetails_GetServiceId(HandleRef self, [In][Out] byte[] out_arg, UIntPtr out_size);

		[DllImport("gpg")]
		internal static extern void ConnectionRequest_Dispose(HandleRef self);

		[DllImport("gpg")]
		internal static extern UIntPtr ConnectionRequest_GetRemoteEndpointId(HandleRef self, [In][Out] byte[] out_arg, UIntPtr out_size);

		[DllImport("gpg")]
		internal static extern UIntPtr ConnectionRequest_GetRemoteDeviceId(HandleRef self, [In][Out] byte[] out_arg, UIntPtr out_size);

		[DllImport("gpg")]
		internal static extern UIntPtr ConnectionRequest_GetRemoteEndpointName(HandleRef self, [In][Out] byte[] out_arg, UIntPtr out_size);

		[DllImport("gpg")]
		internal static extern UIntPtr ConnectionRequest_GetPayload(HandleRef self, [In][Out] byte[] out_arg, UIntPtr out_size);

		[DllImport("gpg")]
		internal static extern void ConnectionResponse_Dispose(HandleRef self);

		[DllImport("gpg")]
		internal static extern UIntPtr ConnectionResponse_GetRemoteEndpointId(HandleRef self, [In][Out] byte[] out_arg, UIntPtr out_size);

		[DllImport("gpg")]
		internal static extern ConnectionResponse_ResponseCode ConnectionResponse_GetStatus(HandleRef self);

		[DllImport("gpg")]
		internal static extern UIntPtr ConnectionResponse_GetPayload(HandleRef self, [In][Out] byte[] out_arg, UIntPtr out_size);
	}
}
