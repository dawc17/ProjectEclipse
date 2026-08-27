using System;
using System.Runtime.InteropServices;
using GooglePlayGames.Native.Cwrapper;

namespace GooglePlayGames.Native.PInvoke
{
	internal class NativeScorePageToken : BaseReferenceHolder
	{
		internal NativeScorePageToken(IntPtr selfPtr)
			: base(selfPtr)
		{
		}

		protected override void CallDispose(HandleRef selfPointer)
		{
			ScorePage.ScorePage_ScorePageToken_Dispose(selfPointer);
		}
	}
}
