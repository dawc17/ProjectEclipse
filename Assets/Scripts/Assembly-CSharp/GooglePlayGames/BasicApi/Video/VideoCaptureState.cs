namespace GooglePlayGames.BasicApi.Video
{
	public class VideoCaptureState
	{
		private bool mIsCapturing;

		private VideoCaptureMode mCaptureMode;

		private VideoQualityLevel mQualityLevel;

		private bool mIsOverlayVisible;

		private bool mIsPaused;

		public bool IsCapturing
		{
			get
			{
				return mIsCapturing;
			}
		}

		public VideoCaptureMode CaptureMode
		{
			get
			{
				return mCaptureMode;
			}
		}

		public VideoQualityLevel QualityLevel
		{
			get
			{
				return mQualityLevel;
			}
		}

		public bool IsOverlayVisible
		{
			get
			{
				return mIsOverlayVisible;
			}
		}

		public bool IsPaused
		{
			get
			{
				return mIsPaused;
			}
		}

		internal VideoCaptureState(bool isCapturing, VideoCaptureMode captureMode, VideoQualityLevel qualityLevel, bool isOverlayVisible, bool isPaused)
		{
			mIsCapturing = isCapturing;
			mCaptureMode = captureMode;
			mQualityLevel = qualityLevel;
			mIsOverlayVisible = isOverlayVisible;
			mIsPaused = isPaused;
		}

		public override string ToString()
		{
			return string.Format("[VideoCaptureState: mIsCapturing={0}, mCaptureMode={1}, mQualityLevel={2}, mIsOverlayVisible={3}, mIsPaused={4}]", mIsCapturing, mCaptureMode.ToString(), mQualityLevel.ToString(), mIsOverlayVisible, mIsPaused);
		}
	}
}
