using Com.Google.Android.Gms.Common.Api;

namespace Com.Google.Android.Gms.Games.Stats
{
	public interface Stats_LoadPlayerStatsResult : Result
	{
		PlayerStats getPlayerStats();
	}
}
