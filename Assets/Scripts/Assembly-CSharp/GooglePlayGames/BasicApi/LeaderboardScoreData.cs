using System.Collections.Generic;
using UnityEngine.SocialPlatforms;

namespace GooglePlayGames.BasicApi
{
	public class LeaderboardScoreData
	{
		private string mId;

		private ResponseStatus mStatus;

		private ulong mApproxCount;

		private string mTitle;

		private IScore mPlayerScore;

		private ScorePageToken mPrevPage;

		private ScorePageToken mNextPage;

		private List<PlayGamesScore> mScores = new List<PlayGamesScore>();

		public bool Valid
		{
			get
			{
				return mStatus == ResponseStatus.Success || mStatus == ResponseStatus.SuccessWithStale;
			}
		}

		public ResponseStatus Status
		{
			get
			{
				return mStatus;
			}
			internal set
			{
				mStatus = value;
			}
		}

		public ulong ApproximateCount
		{
			get
			{
				return mApproxCount;
			}
			internal set
			{
				mApproxCount = value;
			}
		}

		public string Title
		{
			get
			{
				return mTitle;
			}
			internal set
			{
				mTitle = value;
			}
		}

		public string Id
		{
			get
			{
				return mId;
			}
			internal set
			{
				mId = value;
			}
		}

		public IScore PlayerScore
		{
			get
			{
				return mPlayerScore;
			}
			internal set
			{
				mPlayerScore = value;
			}
		}

		public IScore[] Scores
		{
			get
			{
				return mScores.ToArray();
			}
		}

		public ScorePageToken PrevPageToken
		{
			get
			{
				return mPrevPage;
			}
			internal set
			{
				mPrevPage = value;
			}
		}

		public ScorePageToken NextPageToken
		{
			get
			{
				return mNextPage;
			}
			internal set
			{
				mNextPage = value;
			}
		}

		internal LeaderboardScoreData(string leaderboardId)
		{
			mId = leaderboardId;
		}

		internal LeaderboardScoreData(string leaderboardId, ResponseStatus status)
		{
			mId = leaderboardId;
			mStatus = status;
		}

		internal int AddScore(PlayGamesScore score)
		{
			mScores.Add(score);
			return mScores.Count;
		}

		public override string ToString()
		{
			return string.Format("[LeaderboardScoreData: mId={0},  mStatus={1}, mApproxCount={2}, mTitle={3}]", mId, mStatus, mApproxCount, mTitle);
		}
	}
}
