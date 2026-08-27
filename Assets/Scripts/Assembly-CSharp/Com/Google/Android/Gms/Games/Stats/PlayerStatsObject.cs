using System;
using Google.Developers;

namespace Com.Google.Android.Gms.Games.Stats
{
	public class PlayerStatsObject : JavaObjWrapper, PlayerStats
	{
		private const string CLASS_NAME = "com/google/android/gms/games/stats/PlayerStats";

		public static float UNSET_VALUE
		{
			get
			{
				return JavaObjWrapper.GetStaticFloatField("com/google/android/gms/games/stats/PlayerStats", "UNSET_VALUE");
			}
		}

		public static int CONTENTS_FILE_DESCRIPTOR
		{
			get
			{
				return JavaObjWrapper.GetStaticIntField("com/google/android/gms/games/stats/PlayerStats", "CONTENTS_FILE_DESCRIPTOR");
			}
		}

		public static int PARCELABLE_WRITE_RETURN_VALUE
		{
			get
			{
				return JavaObjWrapper.GetStaticIntField("com/google/android/gms/games/stats/PlayerStats", "PARCELABLE_WRITE_RETURN_VALUE");
			}
		}

		public PlayerStatsObject(IntPtr ptr)
			: base(ptr)
		{
		}

		public float getAverageSessionLength()
		{
			return InvokeCall<float>("getAverageSessionLength", "()F", new object[0]);
		}

		public float getChurnProbability()
		{
			return InvokeCall<float>("getChurnProbability", "()F", new object[0]);
		}

		public int getDaysSinceLastPlayed()
		{
			return InvokeCall<int>("getDaysSinceLastPlayed", "()I", new object[0]);
		}

		public int getNumberOfPurchases()
		{
			return InvokeCall<int>("getNumberOfPurchases", "()I", new object[0]);
		}

		public int getNumberOfSessions()
		{
			return InvokeCall<int>("getNumberOfSessions", "()I", new object[0]);
		}

		public float getSessionPercentile()
		{
			return InvokeCall<float>("getSessionPercentile", "()F", new object[0]);
		}

		public float getSpendPercentile()
		{
			return InvokeCall<float>("getSpendPercentile", "()F", new object[0]);
		}

		public float getSpendProbability()
		{
			return InvokeCall<float>("getSpendProbability", "()F", new object[0]);
		}

		public float getHighSpenderProbability()
		{
			return InvokeCall<float>("getHighSpenderProbability", "()F", new object[0]);
		}

		public float getTotalSpendNext28Days()
		{
			return InvokeCall<float>("getTotalSpendNext28Days", "()F", new object[0]);
		}
	}
}
