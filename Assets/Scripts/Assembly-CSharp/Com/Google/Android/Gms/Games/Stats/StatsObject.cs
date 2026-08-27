using System;
using Com.Google.Android.Gms.Common.Api;
using Google.Developers;

namespace Com.Google.Android.Gms.Games.Stats
{
	public class StatsObject : JavaObjWrapper, Stats
	{
		private const string CLASS_NAME = "com/google/android/gms/games/stats/Stats";

		public StatsObject(IntPtr ptr)
			: base(ptr)
		{
		}

		public PendingResult<Stats_LoadPlayerStatsResultObject> loadPlayerStats(GoogleApiClient arg_GoogleApiClient_1, bool arg_bool_2)
		{
			IntPtr ptr = InvokeCall<IntPtr>("loadPlayerStats", "(Lcom/google/android/gms/common/api/GoogleApiClient;Z)Lcom/google/android/gms/common/api/PendingResult;", new object[2] { arg_GoogleApiClient_1, arg_bool_2 });
			return new PendingResult<Stats_LoadPlayerStatsResultObject>(ptr);
		}
	}
}
