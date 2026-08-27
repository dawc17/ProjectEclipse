using System;
using System.Runtime.InteropServices;
using GooglePlayGames.BasicApi.Nearby;
using GooglePlayGames.Native.Cwrapper;

namespace GooglePlayGames.Native.PInvoke
{
	internal class NativeConnectionRequest : BaseReferenceHolder
	{
		internal NativeConnectionRequest(IntPtr pointer)
			: base(pointer)
		{
		}

		internal string RemoteEndpointId()
		{
			return PInvokeUtilities.OutParamsToString((byte[] out_arg, UIntPtr out_size) => NearbyConnectionTypes.ConnectionRequest_GetRemoteEndpointId(SelfPtr(), out_arg, out_size));
		}

		internal string RemoteDeviceId()
		{
			return PInvokeUtilities.OutParamsToString((byte[] out_arg, UIntPtr out_size) => NearbyConnectionTypes.ConnectionRequest_GetRemoteDeviceId(SelfPtr(), out_arg, out_size));
		}

		internal string RemoteEndpointName()
		{
			return PInvokeUtilities.OutParamsToString((byte[] out_arg, UIntPtr out_size) => NearbyConnectionTypes.ConnectionRequest_GetRemoteEndpointName(SelfPtr(), out_arg, out_size));
		}

		internal byte[] Payload()
		{
			return PInvokeUtilities.OutParamsToArray((byte[] out_arg, UIntPtr out_size) => NearbyConnectionTypes.ConnectionRequest_GetPayload(SelfPtr(), out_arg, out_size));
		}

		protected override void CallDispose(HandleRef selfPointer)
		{
			NearbyConnectionTypes.ConnectionRequest_Dispose(selfPointer);
		}

		internal ConnectionRequest AsRequest()
		{
			return new ConnectionRequest(RemoteEndpointId(), RemoteDeviceId(), RemoteEndpointName(), NearbyConnectionsManager.ServiceId, Payload());
		}

		internal static NativeConnectionRequest FromPointer(IntPtr pointer)
		{
			if (pointer == IntPtr.Zero)
			{
				return null;
			}
			return new NativeConnectionRequest(pointer);
		}
	}
}
