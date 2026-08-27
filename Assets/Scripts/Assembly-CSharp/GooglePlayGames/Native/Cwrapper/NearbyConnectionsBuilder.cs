using System;
using System.Runtime.InteropServices;

namespace GooglePlayGames.Native.Cwrapper
{
	internal static class NearbyConnectionsBuilder
	{
		internal delegate void OnInitializationFinishedCallback(NearbyConnectionsStatus.InitializationStatus arg0, IntPtr arg1);

		internal delegate void OnLogCallback(Types.LogLevel arg0, string arg1, IntPtr arg2);

		[DllImport("gpg")]
		internal static extern void NearbyConnections_Builder_SetOnInitializationFinished(HandleRef self, OnInitializationFinishedCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern IntPtr NearbyConnections_Builder_Construct();

		[DllImport("gpg")]
		internal static extern void NearbyConnections_Builder_SetClientId(HandleRef self, long client_id);

		[DllImport("gpg")]
		internal static extern void NearbyConnections_Builder_SetOnLog(HandleRef self, OnLogCallback callback, IntPtr callback_arg, Types.LogLevel min_level);

		[DllImport("gpg")]
		internal static extern void NearbyConnections_Builder_SetDefaultOnLog(HandleRef self, Types.LogLevel min_level);

		[DllImport("gpg")]
		internal static extern IntPtr NearbyConnections_Builder_Create(HandleRef self, IntPtr platform);

		[DllImport("gpg")]
		internal static extern void NearbyConnections_Builder_Dispose(HandleRef self);
	}
}
