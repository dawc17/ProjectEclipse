using System;
using System.Runtime.InteropServices;
using GooglePlayGames.Native.Cwrapper;

namespace GooglePlayGames.Native.PInvoke
{
	internal class GetCaptureCapabilitiesResponse : BaseReferenceHolder
	{
		internal GetCaptureCapabilitiesResponse(IntPtr selfPointer)
			: base(selfPointer)
		{
		}

		protected override void CallDispose(HandleRef selfPointer)
		{
			GooglePlayGames.Native.Cwrapper.VideoManager.VideoManager_GetCaptureCapabilitiesResponse_Dispose(SelfPtr());
		}

		internal CommonErrorStatus.ResponseStatus GetStatus()
		{
			return GooglePlayGames.Native.Cwrapper.VideoManager.VideoManager_GetCaptureCapabilitiesResponse_GetStatus(SelfPtr());
		}

		internal bool RequestSucceeded()
		{
			return GetStatus() > (CommonErrorStatus.ResponseStatus)0;
		}

		internal NativeVideoCapabilities GetData()
		{
			return NativeVideoCapabilities.FromPointer(GooglePlayGames.Native.Cwrapper.VideoManager.VideoManager_GetCaptureCapabilitiesResponse_GetVideoCapabilities(SelfPtr()));
		}

		internal static GetCaptureCapabilitiesResponse FromPointer(IntPtr pointer)
		{
			if (pointer.Equals(IntPtr.Zero))
			{
				return null;
			}
			return new GetCaptureCapabilitiesResponse(pointer);
		}
	}
}
