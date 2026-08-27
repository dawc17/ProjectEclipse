using System;
using System.Runtime.InteropServices;
using GooglePlayGames.Native.Cwrapper;

namespace GooglePlayGames.Native.PInvoke
{
	internal class NativeVideoCapabilities : BaseReferenceHolder
	{
		internal NativeVideoCapabilities(IntPtr selfPtr)
			: base(selfPtr)
		{
		}

		protected override void CallDispose(HandleRef selfPointer)
		{
			VideoCapabilities.VideoCapabilities_Dispose(selfPointer);
		}

		internal bool IsCameraSupported()
		{
			return VideoCapabilities.VideoCapabilities_IsCameraSupported(SelfPtr());
		}

		internal bool IsMicSupported()
		{
			return VideoCapabilities.VideoCapabilities_IsMicSupported(SelfPtr());
		}

		internal bool IsWriteStorageSupported()
		{
			return VideoCapabilities.VideoCapabilities_IsWriteStorageSupported(SelfPtr());
		}

		internal bool SupportsCaptureMode(Types.VideoCaptureMode captureMode)
		{
			return VideoCapabilities.VideoCapabilities_SupportsCaptureMode(SelfPtr(), captureMode);
		}

		internal bool SupportsQualityLevel(Types.VideoQualityLevel qualityLevel)
		{
			return VideoCapabilities.VideoCapabilities_SupportsQualityLevel(SelfPtr(), qualityLevel);
		}

		internal static NativeVideoCapabilities FromPointer(IntPtr pointer)
		{
			if (pointer.Equals(IntPtr.Zero))
			{
				return null;
			}
			return new NativeVideoCapabilities(pointer);
		}
	}
}
