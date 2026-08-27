using System;
using System.Runtime.InteropServices;
using GooglePlayGames.Native.Cwrapper;

namespace GooglePlayGames.Native.PInvoke
{
	internal class IsCaptureAvailableResponse : BaseReferenceHolder
	{
		internal IsCaptureAvailableResponse(IntPtr selfPointer)
			: base(selfPointer)
		{
		}

		protected override void CallDispose(HandleRef selfPointer)
		{
			GooglePlayGames.Native.Cwrapper.VideoManager.VideoManager_IsCaptureAvailableResponse_Dispose(selfPointer);
		}

		internal CommonErrorStatus.ResponseStatus GetStatus()
		{
			return GooglePlayGames.Native.Cwrapper.VideoManager.VideoManager_IsCaptureAvailableResponse_GetStatus(SelfPtr());
		}

		internal bool RequestSucceeded()
		{
			return GetStatus() > (CommonErrorStatus.ResponseStatus)0;
		}

		internal bool IsCaptureAvailable()
		{
			return GooglePlayGames.Native.Cwrapper.VideoManager.VideoManager_IsCaptureAvailableResponse_GetIsCaptureAvailable(SelfPtr());
		}

		internal static IsCaptureAvailableResponse FromPointer(IntPtr pointer)
		{
			if (pointer.Equals(IntPtr.Zero))
			{
				return null;
			}
			return new IsCaptureAvailableResponse(pointer);
		}
	}
}
