using System.Linq;
using GooglePlayGames.OurUtils;

namespace GooglePlayGames.BasicApi.Video
{
	public class VideoCapabilities
	{
		private bool mIsCameraSupported;

		private bool mIsMicSupported;

		private bool mIsWriteStorageSupported;

		private bool[] mCaptureModesSupported;

		private bool[] mQualityLevelsSupported;

		public bool IsCameraSupported
		{
			get
			{
				return mIsCameraSupported;
			}
		}

		public bool IsMicSupported
		{
			get
			{
				return mIsMicSupported;
			}
		}

		public bool IsWriteStorageSupported
		{
			get
			{
				return mIsWriteStorageSupported;
			}
		}

		internal VideoCapabilities(bool isCameraSupported, bool isMicSupported, bool isWriteStorageSupported, bool[] captureModesSupported, bool[] qualityLevelsSupported)
		{
			mIsCameraSupported = isCameraSupported;
			mIsMicSupported = isMicSupported;
			mIsWriteStorageSupported = isWriteStorageSupported;
			mCaptureModesSupported = captureModesSupported;
			mQualityLevelsSupported = qualityLevelsSupported;
		}

		public bool SupportsCaptureMode(VideoCaptureMode captureMode)
		{
			if (captureMode != VideoCaptureMode.Unknown)
			{
				return mCaptureModesSupported[(int)captureMode];
			}
			Logger.w("SupportsCaptureMode called with an unknown captureMode.");
			return false;
		}

		public bool SupportsQualityLevel(VideoQualityLevel qualityLevel)
		{
			if (qualityLevel != VideoQualityLevel.Unknown)
			{
				return mQualityLevelsSupported[(int)qualityLevel];
			}
			Logger.w("SupportsCaptureMode called with an unknown qualityLevel.");
			return false;
		}

		public override string ToString()
		{
			return string.Format("[VideoCapabilities: mIsCameraSupported={0}, mIsMicSupported={1}, mIsWriteStorageSupported={2}, mCaptureModesSupported={3}, mQualityLevelsSupported={4}]", mIsCameraSupported, mIsMicSupported, mIsWriteStorageSupported, string.Join(",", mCaptureModesSupported.Select((bool p) => p.ToString()).ToArray()), string.Join(",", mQualityLevelsSupported.Select((bool p) => p.ToString()).ToArray()));
		}
	}
}
