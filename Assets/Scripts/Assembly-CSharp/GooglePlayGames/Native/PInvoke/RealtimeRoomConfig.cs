using System;
using System.Runtime.InteropServices;
using GooglePlayGames.Native.Cwrapper;

namespace GooglePlayGames.Native.PInvoke
{
	internal class RealtimeRoomConfig : BaseReferenceHolder
	{
		internal RealtimeRoomConfig(IntPtr selfPointer)
			: base(selfPointer)
		{
		}

		protected override void CallDispose(HandleRef selfPointer)
		{
			RealTimeRoomConfig.RealTimeRoomConfig_Dispose(selfPointer);
		}

		internal static RealtimeRoomConfig FromPointer(IntPtr selfPointer)
		{
			if (selfPointer.Equals(IntPtr.Zero))
			{
				return null;
			}
			return new RealtimeRoomConfig(selfPointer);
		}
	}
}
