using System;
using System.Runtime.InteropServices;
using GooglePlayGames.Native.Cwrapper;

namespace GooglePlayGames.Native.PInvoke
{
	internal class GameServices : BaseReferenceHolder
	{
		internal GameServices(IntPtr selfPointer)
			: base(selfPointer)
		{
		}

		internal bool IsAuthenticated()
		{
			return GooglePlayGames.Native.Cwrapper.GameServices.GameServices_IsAuthorized(SelfPtr());
		}

		internal void SignOut()
		{
			GooglePlayGames.Native.Cwrapper.GameServices.GameServices_SignOut(SelfPtr());
		}

		internal void StartAuthorizationUI()
		{
			GooglePlayGames.Native.Cwrapper.GameServices.GameServices_StartAuthorizationUI(SelfPtr());
		}

		public AchievementManager AchievementManager()
		{
			return new AchievementManager(this);
		}

		public LeaderboardManager LeaderboardManager()
		{
			return new LeaderboardManager(this);
		}

		public PlayerManager PlayerManager()
		{
			return new PlayerManager(this);
		}

		public StatsManager StatsManager()
		{
			return new StatsManager(this);
		}

		internal HandleRef AsHandle()
		{
			return SelfPtr();
		}

		protected override void CallDispose(HandleRef selfPointer)
		{
			GooglePlayGames.Native.Cwrapper.GameServices.GameServices_Dispose(selfPointer);
		}
	}
}
