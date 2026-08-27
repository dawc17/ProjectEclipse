using System;
using System.Runtime.InteropServices;
using GooglePlayGames.BasicApi.Nearby;
using GooglePlayGames.Native.Cwrapper;

namespace GooglePlayGames.Native.PInvoke
{
	internal class NativeConnectionResponse : BaseReferenceHolder
	{
		internal NativeConnectionResponse(IntPtr pointer)
			: base(pointer)
		{
		}

		internal string RemoteEndpointId()
		{
			return PInvokeUtilities.OutParamsToString((byte[] out_arg, UIntPtr out_size) => NearbyConnectionTypes.ConnectionResponse_GetRemoteEndpointId(SelfPtr(), out_arg, out_size));
		}

		internal NearbyConnectionTypes.ConnectionResponse_ResponseCode ResponseCode()
		{
			return NearbyConnectionTypes.ConnectionResponse_GetStatus(SelfPtr());
		}

		internal byte[] Payload()
		{
			return PInvokeUtilities.OutParamsToArray((byte[] out_arg, UIntPtr out_size) => NearbyConnectionTypes.ConnectionResponse_GetPayload(SelfPtr(), out_arg, out_size));
		}

		protected override void CallDispose(HandleRef selfPointer)
		{
			NearbyConnectionTypes.ConnectionResponse_Dispose(selfPointer);
		}

		internal ConnectionResponse AsResponse(long localClientId)
		{
			switch (ResponseCode())
			{
			case NearbyConnectionTypes.ConnectionResponse_ResponseCode.ACCEPTED:
				return ConnectionResponse.Accepted(localClientId, RemoteEndpointId(), Payload());
			case NearbyConnectionTypes.ConnectionResponse_ResponseCode.ERROR_ENDPOINT_ALREADY_CONNECTED:
				return ConnectionResponse.AlreadyConnected(localClientId, RemoteEndpointId());
			case NearbyConnectionTypes.ConnectionResponse_ResponseCode.REJECTED:
				return ConnectionResponse.Rejected(localClientId, RemoteEndpointId());
			case NearbyConnectionTypes.ConnectionResponse_ResponseCode.ERROR_ENDPOINT_NOT_CONNECTED:
				return ConnectionResponse.EndpointNotConnected(localClientId, RemoteEndpointId());
			case NearbyConnectionTypes.ConnectionResponse_ResponseCode.ERROR_NETWORK_NOT_CONNECTED:
				return ConnectionResponse.NetworkNotConnected(localClientId, RemoteEndpointId());
			case NearbyConnectionTypes.ConnectionResponse_ResponseCode.ERROR_INTERNAL:
				return ConnectionResponse.InternalError(localClientId, RemoteEndpointId());
			default:
				throw new InvalidOperationException("Found connection response of unknown type: " + ResponseCode());
			}
		}

		internal static NativeConnectionResponse FromPointer(IntPtr pointer)
		{
			if (pointer == IntPtr.Zero)
			{
				return null;
			}
			return new NativeConnectionResponse(pointer);
		}
	}
}
