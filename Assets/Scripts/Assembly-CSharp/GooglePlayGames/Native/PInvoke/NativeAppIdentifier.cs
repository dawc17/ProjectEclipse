using System;
using System.Runtime.InteropServices;
using GooglePlayGames.Native.Cwrapper;

namespace GooglePlayGames.Native.PInvoke
{
	internal class NativeAppIdentifier : BaseReferenceHolder
	{
		internal NativeAppIdentifier(IntPtr pointer)
			: base(pointer)
		{
		}

		[DllImport("gpg")]
		internal static extern IntPtr NearbyUtils_ConstructAppIdentifier(string appId);

		internal string Id()
		{
			return PInvokeUtilities.OutParamsToString((byte[] out_arg, UIntPtr out_size) => NearbyConnectionTypes.AppIdentifier_GetIdentifier(SelfPtr(), out_arg, out_size));
		}

		protected override void CallDispose(HandleRef selfPointer)
		{
			NearbyConnectionTypes.AppIdentifier_Dispose(selfPointer);
		}

		internal static NativeAppIdentifier FromString(string appId)
		{
			return new NativeAppIdentifier(NearbyUtils_ConstructAppIdentifier(appId));
		}
	}
}
