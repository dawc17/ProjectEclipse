using System;

namespace GooglePlayGames.BasicApi
{
	public class Achievement
	{
		private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

		private string mId = string.Empty;

		private bool mIsIncremental;

		private bool mIsRevealed;

		private bool mIsUnlocked;

		private int mCurrentSteps;

		private int mTotalSteps;

		private string mDescription = string.Empty;

		private string mName = string.Empty;

		private long mLastModifiedTime;

		private ulong mPoints;

		private string mRevealedImageUrl;

		private string mUnlockedImageUrl;

		public bool IsIncremental
		{
			get
			{
				return mIsIncremental;
			}
			set
			{
				mIsIncremental = value;
			}
		}

		public int CurrentSteps
		{
			get
			{
				return mCurrentSteps;
			}
			set
			{
				mCurrentSteps = value;
			}
		}

		public int TotalSteps
		{
			get
			{
				return mTotalSteps;
			}
			set
			{
				mTotalSteps = value;
			}
		}

		public bool IsUnlocked
		{
			get
			{
				return mIsUnlocked;
			}
			set
			{
				mIsUnlocked = value;
			}
		}

		public bool IsRevealed
		{
			get
			{
				return mIsRevealed;
			}
			set
			{
				mIsRevealed = value;
			}
		}

		public string Id
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

		public string Description
		{
			get
			{
				return mDescription;
			}
			set
			{
				mDescription = value;
			}
		}

		public string Name
		{
			get
			{
				return mName;
			}
			set
			{
				mName = value;
			}
		}

		public DateTime LastModifiedTime
		{
			get
			{
				return UnixEpoch.AddMilliseconds(mLastModifiedTime);
			}
			set
			{
				mLastModifiedTime = (long)(value - UnixEpoch).TotalMilliseconds;
			}
		}

		public ulong Points
		{
			get
			{
				return mPoints;
			}
			set
			{
				mPoints = value;
			}
		}

		public string RevealedImageUrl
		{
			get
			{
				return mRevealedImageUrl;
			}
			set
			{
				mRevealedImageUrl = value;
			}
		}

		public string UnlockedImageUrl
		{
			get
			{
				return mUnlockedImageUrl;
			}
			set
			{
				mUnlockedImageUrl = value;
			}
		}

		public override string ToString()
		{
			return string.Format("[Achievement] id={0}, name={1}, desc={2}, type={3}, revealed={4}, unlocked={5}, steps={6}/{7}", mId, mName, mDescription, (!mIsIncremental) ? "STANDARD" : "INCREMENTAL", mIsRevealed, mIsUnlocked, mCurrentSteps, mTotalSteps);
		}
	}
}
