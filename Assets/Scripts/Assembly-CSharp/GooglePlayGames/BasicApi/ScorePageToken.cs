namespace GooglePlayGames.BasicApi
{
	public class ScorePageToken
	{
		private string mId;

		private object mInternalObject;

		private LeaderboardCollection mCollection;

		private LeaderboardTimeSpan mTimespan;

		public LeaderboardCollection Collection
		{
			get
			{
				return mCollection;
			}
		}

		public LeaderboardTimeSpan TimeSpan
		{
			get
			{
				return mTimespan;
			}
		}

		public string LeaderboardId
		{
			get
			{
				return mId;
			}
		}

		internal object InternalObject
		{
			get
			{
				return mInternalObject;
			}
		}

		internal ScorePageToken(object internalObject, string id, LeaderboardCollection collection, LeaderboardTimeSpan timespan)
		{
			mInternalObject = internalObject;
			mId = id;
			mCollection = collection;
			mTimespan = timespan;
		}
	}
}
