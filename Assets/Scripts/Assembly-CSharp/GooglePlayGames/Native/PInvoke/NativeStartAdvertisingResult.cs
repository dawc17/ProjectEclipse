using System;
using System.Runtime.InteropServices;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Nearby;
using GooglePlayGames.Native.Cwrapper;

namespace GooglePlayGames.Native.PInvoke
{
	internal class NativeStartAdvertisingResult : BaseReferenceHolder
	{
		internal NativeStartAdvertisingResult(IntPtr pointer)
			: base(pointer)
		{
		}

		internal int GetStatus()
		{
			return NearbyConnectionTypes.StartAdvertisingResult_GetStatus(SelfPtr());
		}

		internal string LocalEndpointName()
		{
			return PInvokeUtilities.OutParamsToString((byte[] out_arg, UIntPtr out_size) => NearbyConnectionTypes.StartAdvertisingResult_GetLocalEndpointName(SelfPtr(), out_arg, out_size));
		}

		protected override void CallDispose(HandleRef selfPointer)
		{
			NearbyConnectionTypes.StartAdvertisingResult_Dispose(selfPointer);
		}

		internal AdvertisingResult AsResult()
		{
			return new AdvertisingResult((ResponseStatus)Enum.ToObject(typeof(ResponseStatus), GetStatus()), LocalEndpointName());
		}

		internal static NativeStartAdvertisingResult FromPointer(IntPtr pointer)
		{
			if (pointer == IntPtr.Zero)
			{
				return null;
			}
			return new NativeStartAdvertisingResult(pointer);
		}
	}
}
