using System;
using UnityEngine.SocialPlatforms;

namespace GooglePlayGames
{
	public class PlayGamesScore : IScore
	{
		private string mLbId;

		private long mValue;

		private ulong mRank;

		private string mPlayerId = string.Empty;

		private string mMetadata = string.Empty;

		private DateTime mDate = new DateTime(1970, 1, 1, 0, 0, 0);

		public string leaderboardID
		{
			get
			{
				return mLbId;
			}
			set
			{
				mLbId = value;
			}
		}

		public long value
		{
			get
			{
				return mValue;
			}
			set
			{
				mValue = value;
			}
		}

		public DateTime date
		{
			get
			{
				return mDate;
			}
		}

		public string formattedValue
		{
			get
			{
				return mValue.ToString();
			}
		}

		public string userID
		{
			get
			{
				return mPlayerId;
			}
		}

		public int rank
		{
			get
			{
				return (int)mRank;
			}
		}

		public string metaData
		{
			get
			{
				return mMetadata;
			}
		}

		internal PlayGamesScore(DateTime date, string leaderboardId, ulong rank, string playerId, ulong value, string metadata)
		{
			mDate = date;
			mLbId = leaderboardID;
			mRank = rank;
			mPlayerId = playerId;
			mValue = (long)value;
			mMetadata = metadata;
		}

		public void ReportScore(Action<bool> callback)
		{
			PlayGamesPlatform.Instance.ReportScore(mValue, mLbId, mMetadata, callback);
		}
	}
}
