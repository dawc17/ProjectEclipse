using System;
using System.Runtime.InteropServices;
using GooglePlayGames.BasicApi;
using GooglePlayGames.Native.Cwrapper;

namespace GooglePlayGames.Native.PInvoke
{
	internal class NativePlayerStats : BaseReferenceHolder
	{
		internal NativePlayerStats(IntPtr selfPointer)
			: base(selfPointer)
		{
		}

		internal bool Valid()
		{
			return GooglePlayGames.Native.Cwrapper.PlayerStats.PlayerStats_Valid(SelfPtr());
		}

		internal bool HasAverageSessionLength()
		{
			return GooglePlayGames.Native.Cwrapper.PlayerStats.PlayerStats_HasAverageSessionLength(SelfPtr());
		}

		internal float AverageSessionLength()
		{
			return GooglePlayGames.Native.Cwrapper.PlayerStats.PlayerStats_AverageSessionLength(SelfPtr());
		}

		internal bool HasChurnProbability()
		{
			return GooglePlayGames.Native.Cwrapper.PlayerStats.PlayerStats_HasChurnProbability(SelfPtr());
		}

		internal float ChurnProbability()
		{
			return GooglePlayGames.Native.Cwrapper.PlayerStats.PlayerStats_ChurnProbability(SelfPtr());
		}

		internal bool HasDaysSinceLastPlayed()
		{
			return GooglePlayGames.Native.Cwrapper.PlayerStats.PlayerStats_HasDaysSinceLastPlayed(SelfPtr());
		}

		internal int DaysSinceLastPlayed()
		{
			return GooglePlayGames.Native.Cwrapper.PlayerStats.PlayerStats_DaysSinceLastPlayed(SelfPtr());
		}

		internal bool HasNumberOfPurchases()
		{
			return GooglePlayGames.Native.Cwrapper.PlayerStats.PlayerStats_HasNumberOfPurchases(SelfPtr());
		}

		internal int NumberOfPurchases()
		{
			return GooglePlayGames.Native.Cwrapper.PlayerStats.PlayerStats_NumberOfPurchases(SelfPtr());
		}

		internal bool HasNumberOfSessions()
		{
			return GooglePlayGames.Native.Cwrapper.PlayerStats.PlayerStats_HasNumberOfSessions(SelfPtr());
		}

		internal int NumberOfSessions()
		{
			return GooglePlayGames.Native.Cwrapper.PlayerStats.PlayerStats_NumberOfSessions(SelfPtr());
		}

		internal bool HasSessionPercentile()
		{
			return GooglePlayGames.Native.Cwrapper.PlayerStats.PlayerStats_HasSessionPercentile(SelfPtr());
		}

		internal float SessionPercentile()
		{
			return GooglePlayGames.Native.Cwrapper.PlayerStats.PlayerStats_SessionPercentile(SelfPtr());
		}

		internal bool HasSpendPercentile()
		{
			return GooglePlayGames.Native.Cwrapper.PlayerStats.PlayerStats_HasSpendPercentile(SelfPtr());
		}

		internal float SpendPercentile()
		{
			return GooglePlayGames.Native.Cwrapper.PlayerStats.PlayerStats_SpendPercentile(SelfPtr());
		}

		protected override void CallDispose(HandleRef selfPointer)
		{
			GooglePlayGames.Native.Cwrapper.PlayerStats.PlayerStats_Dispose(selfPointer);
		}

		internal GooglePlayGames.BasicApi.PlayerStats AsPlayerStats()
		{
			GooglePlayGames.BasicApi.PlayerStats playerStats = new GooglePlayGames.BasicApi.PlayerStats();
			playerStats.Valid = Valid();
			if (Valid())
			{
				playerStats.AvgSessonLength = AverageSessionLength();
				playerStats.ChurnProbability = ChurnProbability();
				playerStats.DaysSinceLastPlayed = DaysSinceLastPlayed();
				playerStats.NumberOfPurchases = NumberOfPurchases();
				playerStats.NumberOfSessions = NumberOfSessions();
				playerStats.SessPercentile = SessionPercentile();
				playerStats.SpendPercentile = SpendPercentile();
				playerStats.SpendProbability = -1f;
			}
			return playerStats;
		}
	}
}
