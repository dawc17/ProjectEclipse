using System;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Video;
using GooglePlayGames.Native.Cwrapper;
using GooglePlayGames.Native.PInvoke;
using GooglePlayGames.OurUtils;

namespace GooglePlayGames.Native
{
	internal class NativeVideoClient : IVideoClient
	{
		private readonly GooglePlayGames.Native.PInvoke.VideoManager mManager;

		internal NativeVideoClient(GooglePlayGames.Native.PInvoke.VideoManager manager)
		{
			mManager = Misc.CheckNotNull(manager);
		}

		public void GetCaptureCapabilities(Action<ResponseStatus, GooglePlayGames.BasicApi.Video.VideoCapabilities> callback)
		{
			Misc.CheckNotNull(callback);
			callback = CallbackUtils.ToOnGameThread(callback);
			mManager.GetCaptureCapabilities((GetCaptureCapabilitiesResponse response) =>
			{
				ResponseStatus arg = ConversionUtils.ConvertResponseStatus(response.GetStatus());
				if (!response.RequestSucceeded())
				{
					callback(arg, null);
				}
				else
				{
					callback(arg, FromNativeVideoCapabilities(response.GetData()));
				}
			});
		}

		private GooglePlayGames.BasicApi.Video.VideoCapabilities FromNativeVideoCapabilities(NativeVideoCapabilities capabilities)
		{
			bool[] array = new bool[mManager.NumCaptureModes];
			array[0] = capabilities.SupportsCaptureMode(Types.VideoCaptureMode.FILE);
			array[1] = capabilities.SupportsCaptureMode(Types.VideoCaptureMode.STREAM);
			bool[] array2 = new bool[mManager.NumQualityLevels];
			array2[0] = capabilities.SupportsQualityLevel(Types.VideoQualityLevel.SD);
			array2[1] = capabilities.SupportsQualityLevel(Types.VideoQualityLevel.HD);
			array2[2] = capabilities.SupportsQualityLevel(Types.VideoQualityLevel.XHD);
			array2[3] = capabilities.SupportsQualityLevel(Types.VideoQualityLevel.FULLHD);
			return new GooglePlayGames.BasicApi.Video.VideoCapabilities(capabilities.IsCameraSupported(), capabilities.IsMicSupported(), capabilities.IsWriteStorageSupported(), array, array2);
		}

		public void ShowCaptureOverlay()
		{
			mManager.ShowCaptureOverlay();
		}

		public void GetCaptureState(Action<ResponseStatus, GooglePlayGames.BasicApi.Video.VideoCaptureState> callback)
		{
			Misc.CheckNotNull(callback);
			callback = CallbackUtils.ToOnGameThread(callback);
			mManager.GetCaptureState((GetCaptureStateResponse response) =>
			{
				ResponseStatus arg = ConversionUtils.ConvertResponseStatus(response.GetStatus());
				if (!response.RequestSucceeded())
				{
					callback(arg, null);
				}
				else
				{
					callback(arg, FromNativeVideoCaptureState(response.GetData()));
				}
			});
		}

		private GooglePlayGames.BasicApi.Video.VideoCaptureState FromNativeVideoCaptureState(NativeVideoCaptureState captureState)
		{
			return new GooglePlayGames.BasicApi.Video.VideoCaptureState(captureState.IsCapturing(), ConversionUtils.ConvertNativeVideoCaptureMode(captureState.CaptureMode()), ConversionUtils.ConvertNativeVideoQualityLevel(captureState.QualityLevel()), captureState.IsOverlayVisible(), captureState.IsPaused());
		}

		public void IsCaptureAvailable(VideoCaptureMode captureMode, Action<ResponseStatus, bool> callback)
		{
			Misc.CheckNotNull(callback);
			callback = CallbackUtils.ToOnGameThread(callback);
			mManager.IsCaptureAvailable(ConversionUtils.ConvertVideoCaptureMode(captureMode), (IsCaptureAvailableResponse response) =>
			{
				ResponseStatus arg = ConversionUtils.ConvertResponseStatus(response.GetStatus());
				if (!response.RequestSucceeded())
				{
					callback(arg, false);
				}
				else
				{
					callback(arg, response.IsCaptureAvailable());
				}
			});
		}

		public bool IsCaptureSupported()
		{
			return mManager.IsCaptureSupported();
		}

		public void RegisterCaptureOverlayStateChangedListener(CaptureOverlayStateListener listener)
		{
			Misc.CheckNotNull(listener);
			GooglePlayGames.Native.PInvoke.CaptureOverlayStateListenerHelper helper = GooglePlayGames.Native.PInvoke.CaptureOverlayStateListenerHelper.Create().SetOnCaptureOverlayStateChangedCallback((Types.VideoCaptureOverlayState response) =>
			{
				listener.OnCaptureOverlayStateChanged(ConversionUtils.ConvertNativeVideoCaptureOverlayState(response));
			});
			mManager.RegisterCaptureOverlayStateChangedListener(helper);
		}

		public void UnregisterCaptureOverlayStateChangedListener()
		{
			mManager.UnregisterCaptureOverlayStateChangedListener();
		}
	}
}
