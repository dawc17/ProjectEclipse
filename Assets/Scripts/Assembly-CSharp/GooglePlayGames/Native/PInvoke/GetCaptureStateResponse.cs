using System;
using System.Runtime.InteropServices;
using GooglePlayGames.Native.Cwrapper;

namespace GooglePlayGames.Native.PInvoke
{
	internal class GetCaptureStateResponse : BaseReferenceHolder
	{
		internal GetCaptureStateResponse(IntPtr selfPointer)
			: base(selfPointer)
		{
		}

		protected override void CallDispose(HandleRef selfPointer)
		{
			GooglePlayGames.Native.Cwrapper.VideoManager.VideoManager_GetCaptureStateResponse_Dispose(SelfPtr());
		}

		internal NativeVideoCaptureState GetData()
		{
			return NativeVideoCaptureState.FromPointer(GooglePlayGames.Native.Cwrapper.VideoManager.VideoManager_GetCaptureStateResponse_GetVideoCaptureState(SelfPtr()));
		}

		internal CommonErrorStatus.ResponseStatus GetStatus()
		{
			return GooglePlayGames.Native.Cwrapper.VideoManager.VideoManager_GetCaptureStateResponse_GetStatus(SelfPtr());
		}

		internal bool RequestSucceeded()
		{
			return GetStatus() > (CommonErrorStatus.ResponseStatus)0;
		}

		internal static GetCaptureStateResponse FromPointer(IntPtr pointer)
		{
			if (pointer.Equals(IntPtr.Zero))
			{
				return null;
			}
			return new GetCaptureStateResponse(pointer);
		}
	}
}
