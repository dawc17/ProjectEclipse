using GooglePlayGames.OurUtils;

namespace GooglePlayGames.BasicApi.Nearby
{
	public struct AdvertisingResult
	{
		private readonly ResponseStatus mStatus;

		private readonly string mLocalEndpointName;

		public bool Succeeded
		{
			get
			{
				return mStatus == ResponseStatus.Success;
			}
		}

		public ResponseStatus Status
		{
			get
			{
				return mStatus;
			}
		}

		public string LocalEndpointName
		{
			get
			{
				return mLocalEndpointName;
			}
		}

		public AdvertisingResult(ResponseStatus status, string localEndpointName)
		{
			mStatus = status;
			mLocalEndpointName = Misc.CheckNotNull(localEndpointName);
		}
	}
}
