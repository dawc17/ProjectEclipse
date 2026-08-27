using System;
using System.Runtime.InteropServices;

namespace GooglePlayGames.Native.Cwrapper
{
	internal static class EndpointDiscoveryListenerHelper
	{
		internal delegate void OnEndpointFoundCallback(long arg0, IntPtr arg1, IntPtr arg2);

		internal delegate void OnEndpointLostCallback(long arg0, string arg1, IntPtr arg2);

		[DllImport("gpg")]
		internal static extern IntPtr EndpointDiscoveryListenerHelper_Construct();

		[DllImport("gpg")]
		internal static extern void EndpointDiscoveryListenerHelper_SetOnEndpointLostCallback(HandleRef self, OnEndpointLostCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void EndpointDiscoveryListenerHelper_SetOnEndpointFoundCallback(HandleRef self, OnEndpointFoundCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void EndpointDiscoveryListenerHelper_Dispose(HandleRef self);
	}
}
