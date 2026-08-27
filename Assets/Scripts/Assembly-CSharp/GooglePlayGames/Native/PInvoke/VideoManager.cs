using System;
using AOT;
using GooglePlayGames.Native.Cwrapper;
using GooglePlayGames.OurUtils;

namespace GooglePlayGames.Native.PInvoke
{
	internal class VideoManager
	{
		private readonly GameServices mServices;

		internal int NumCaptureModes
		{
			get
			{
				return 2;
			}
		}

		internal int NumQualityLevels
		{
			get
			{
				return 4;
			}
		}

		internal VideoManager(GameServices services)
		{
			mServices = Misc.CheckNotNull(services);
		}

		internal void GetCaptureCapabilities(Action<GetCaptureCapabilitiesResponse> callback)
		{
			GooglePlayGames.Native.Cwrapper.VideoManager.VideoManager_GetCaptureCapabilities(mServices.AsHandle(), InternalCaptureCapabilitiesCallback, Callbacks.ToIntPtr(callback, GetCaptureCapabilitiesResponse.FromPointer));
		}

		[AOT.MonoPInvokeCallback(typeof(GooglePlayGames.Native.Cwrapper.VideoManager.CaptureCapabilitiesCallback))]
		internal static void InternalCaptureCapabilitiesCallback(IntPtr response, IntPtr data)
		{
			Callbacks.PerformInternalCallback("VideoManager#CaptureCapabilitiesCallback", Callbacks.Type.Temporary, response, data);
		}

		internal void ShowCaptureOverlay()
		{
			GooglePlayGames.Native.Cwrapper.VideoManager.VideoManager_ShowCaptureOverlay(mServices.AsHandle());
		}

		internal void GetCaptureState(Action<GetCaptureStateResponse> callback)
		{
			GooglePlayGames.Native.Cwrapper.VideoManager.VideoManager_GetCaptureState(mServices.AsHandle(), InternalCaptureStateCallback, Callbacks.ToIntPtr(callback, GetCaptureStateResponse.FromPointer));
		}

		[AOT.MonoPInvokeCallback(typeof(GooglePlayGames.Native.Cwrapper.VideoManager.CaptureStateCallback))]
		internal static void InternalCaptureStateCallback(IntPtr response, IntPtr data)
		{
			Callbacks.PerformInternalCallback("VideoManager#CaptureStateCallback", Callbacks.Type.Temporary, response, data);
		}

		internal void IsCaptureAvailable(Types.VideoCaptureMode captureMode, Action<IsCaptureAvailableResponse> callback)
		{
			GooglePlayGames.Native.Cwrapper.VideoManager.VideoManager_IsCaptureAvailable(mServices.AsHandle(), captureMode, InternalIsCaptureAvailableCallback, Callbacks.ToIntPtr(callback, IsCaptureAvailableResponse.FromPointer));
		}

		[AOT.MonoPInvokeCallback(typeof(GooglePlayGames.Native.Cwrapper.VideoManager.IsCaptureAvailableCallback))]
		internal static void InternalIsCaptureAvailableCallback(IntPtr response, IntPtr data)
		{
			Callbacks.PerformInternalCallback("VideoManager#IsCaptureAvailableCallback", Callbacks.Type.Temporary, response, data);
		}

		internal bool IsCaptureSupported()
		{
			return GooglePlayGames.Native.Cwrapper.VideoManager.VideoManager_IsCaptureSupported(mServices.AsHandle());
		}

		internal void RegisterCaptureOverlayStateChangedListener(CaptureOverlayStateListenerHelper helper)
		{
			GooglePlayGames.Native.Cwrapper.VideoManager.VideoManager_RegisterCaptureOverlayStateChangedListener(mServices.AsHandle(), helper.AsPointer());
		}

		internal void UnregisterCaptureOverlayStateChangedListener()
		{
			GooglePlayGames.Native.Cwrapper.VideoManager.VideoManager_UnregisterCaptureOverlayStateChangedListener(mServices.AsHandle());
		}
	}
}
