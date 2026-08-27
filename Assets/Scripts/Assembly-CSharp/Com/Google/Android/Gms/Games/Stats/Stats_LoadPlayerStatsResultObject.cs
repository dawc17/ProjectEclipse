using System;
using Com.Google.Android.Gms.Common.Api;
using Google.Developers;

namespace Com.Google.Android.Gms.Games.Stats
{
	public class Stats_LoadPlayerStatsResultObject : JavaObjWrapper, Result, Stats_LoadPlayerStatsResult
	{
		private const string CLASS_NAME = "com/google/android/gms/games/stats/Stats$LoadPlayerStatsResult";

		public Stats_LoadPlayerStatsResultObject(IntPtr ptr)
			: base(ptr)
		{
		}

		public PlayerStats getPlayerStats()
		{
			IntPtr ptr = InvokeCall<IntPtr>("getPlayerStats", "()Lcom/google/android/gms/games/stats/PlayerStats;", new object[0]);
			return new PlayerStatsObject(ptr);
		}

		public Status getStatus()
		{
			IntPtr ptr = InvokeCall<IntPtr>("getStatus", "()Lcom/google/android/gms/common/api/Status;", new object[0]);
			return new Status(ptr);
		}
	}
}
