using System;
using GooglePlayGames.OurUtils;

namespace GooglePlayGames.BasicApi.Nearby
{
	public struct NearbyConnectionConfiguration
	{
		public const int MaxUnreliableMessagePayloadLength = 1168;

		public const int MaxReliableMessagePayloadLength = 4096;

		private readonly Action<InitializationStatus> mInitializationCallback;

		private readonly long mLocalClientId;

		public long LocalClientId
		{
			get
			{
				return mLocalClientId;
			}
		}

		public Action<InitializationStatus> InitializationCallback
		{
			get
			{
				return mInitializationCallback;
			}
		}

		public NearbyConnectionConfiguration(Action<InitializationStatus> callback, long localClientId)
		{
			mInitializationCallback = Misc.CheckNotNull(callback);
			mLocalClientId = localClientId;
		}
	}
}
