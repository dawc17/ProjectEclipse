using System;
using System.Collections.Generic;
using GooglePlayGames.BasicApi;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace GooglePlayGames
{
	public class PlayGamesLeaderboard : ILeaderboard
	{
		private string mId;

		private UserScope mUserScope;

		private Range mRange;

		private TimeScope mTimeScope;

		private string[] mFilteredUserIds;

		private bool mLoading;

		private IScore mLocalUserScore;

		private uint mMaxRange;

		private List<PlayGamesScore> mScoreList = new List<PlayGamesScore>();

		private string mTitle;

		public bool loading
		{
			get
			{
				return mLoading;
			}
			internal set
			{
				mLoading = value;
			}
		}

		public string id
		{
			get
			{
				return mId;
			}
			set
			{
				mId = value;
			}
		}

		public UserScope userScope
		{
			get
			{
				return mUserScope;
			}
			set
			{
				mUserScope = value;
			}
		}

		public Range range
		{
			get
			{
				return mRange;
			}
			set
			{
				mRange = value;
			}
		}

		public TimeScope timeScope
		{
			get
			{
				return mTimeScope;
			}
			set
			{
				mTimeScope = value;
			}
		}

		public IScore localUserScore
		{
			get
			{
				return mLocalUserScore;
			}
		}

		public uint maxRange
		{
			get
			{
				return mMaxRange;
			}
		}

		public IScore[] scores
		{
			get
			{
				PlayGamesScore[] array = new PlayGamesScore[mScoreList.Count];
				mScoreList.CopyTo(array);
				return array;
			}
		}

		public string title
		{
			get
			{
				return mTitle;
			}
		}

		public int ScoreCount
		{
			get
			{
				return mScoreList.Count;
			}
		}

		public PlayGamesLeaderboard(string id)
		{
			mId = id;
		}

		public void SetUserFilter(string[] userIDs)
		{
			mFilteredUserIds = userIDs;
		}

		public void LoadScores(Action<bool> callback)
		{
			PlayGamesPlatform.Instance.LoadScores(this, callback);
		}

		internal bool SetFromData(LeaderboardScoreData data)
		{
			if (data.Valid)
			{
				Debug.Log("Setting leaderboard from: " + data);
				SetMaxRange(data.ApproximateCount);
				SetTitle(data.Title);
				SetLocalUserScore((PlayGamesScore)data.PlayerScore);
				IScore[] array = data.Scores;
				foreach (IScore score in array)
				{
					AddScore((PlayGamesScore)score);
				}
				mLoading = data.Scores.Length == 0 || HasAllScores();
			}
			return data.Valid;
		}

		internal void SetMaxRange(ulong val)
		{
			mMaxRange = (uint)val;
		}

		internal void SetTitle(string value)
		{
			mTitle = value;
		}

		internal void SetLocalUserScore(PlayGamesScore score)
		{
			mLocalUserScore = score;
		}

		internal int AddScore(PlayGamesScore score)
		{
			if (mFilteredUserIds == null || mFilteredUserIds.Length == 0)
			{
				mScoreList.Add(score);
			}
			else
			{
				string[] array = mFilteredUserIds;
				foreach (string text in array)
				{
					if (text.Equals(score.userID))
					{
						return mScoreList.Count;
					}
				}
				mScoreList.Add(score);
			}
			return mScoreList.Count;
		}

		internal bool HasAllScores()
		{
			return mScoreList.Count >= mRange.count || mScoreList.Count >= maxRange;
		}
	}
}
