using System;
using System.Runtime.InteropServices;
using GooglePlayGames.Native.Cwrapper;

namespace GooglePlayGames.Native.PInvoke
{
	internal class NativeLeaderboard : BaseReferenceHolder
	{
		internal NativeLeaderboard(IntPtr selfPtr)
			: base(selfPtr)
		{
		}

		protected override void CallDispose(HandleRef selfPointer)
		{
			Leaderboard.Leaderboard_Dispose(selfPointer);
		}

		internal string Title()
		{
			return PInvokeUtilities.OutParamsToString((byte[] out_string, UIntPtr out_size) => Leaderboard.Leaderboard_Name(SelfPtr(), out_string, out_size));
		}

		internal static NativeLeaderboard FromPointer(IntPtr pointer)
		{
			if (pointer.Equals(IntPtr.Zero))
			{
				return null;
			}
			return new NativeLeaderboard(pointer);
		}
	}
}
