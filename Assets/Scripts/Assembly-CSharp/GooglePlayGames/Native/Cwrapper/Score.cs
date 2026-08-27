using System;
using System.Runtime.InteropServices;

namespace GooglePlayGames.Native.Cwrapper
{
	internal static class Score
	{
		[DllImport("gpg")]
		internal static extern ulong Score_Value(HandleRef self);

		[DllImport("gpg")]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool Score_Valid(HandleRef self);

		[DllImport("gpg")]
		internal static extern ulong Score_Rank(HandleRef self);

		[DllImport("gpg")]
		internal static extern void Score_Dispose(HandleRef self);

		[DllImport("gpg")]
		internal static extern UIntPtr Score_Metadata(HandleRef self, [In][Out] byte[] out_arg, UIntPtr out_size);
	}
}
