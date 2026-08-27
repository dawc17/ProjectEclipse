using System;
using System.Runtime.InteropServices;

namespace GooglePlayGames.Native.Cwrapper
{
	internal static class Sentinels
	{
		[DllImport("gpg")]
		internal static extern IntPtr Sentinels_AutomatchingParticipant();
	}
}
