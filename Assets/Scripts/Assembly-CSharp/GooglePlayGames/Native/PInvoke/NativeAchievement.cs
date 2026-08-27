using System;
using System.Runtime.InteropServices;
using GooglePlayGames.BasicApi;
using GooglePlayGames.Native.Cwrapper;

namespace GooglePlayGames.Native.PInvoke
{
	internal class NativeAchievement : BaseReferenceHolder
	{
		private const ulong MinusOne = ulong.MaxValue;

		internal NativeAchievement(IntPtr selfPointer)
			: base(selfPointer)
		{
		}

		internal uint CurrentSteps()
		{
			return GooglePlayGames.Native.Cwrapper.Achievement.Achievement_CurrentSteps(SelfPtr());
		}

		internal string Description()
		{
			return PInvokeUtilities.OutParamsToString((byte[] out_string, UIntPtr out_size) => GooglePlayGames.Native.Cwrapper.Achievement.Achievement_Description(SelfPtr(), out_string, out_size));
		}

		internal string Id()
		{
			return PInvokeUtilities.OutParamsToString((byte[] out_string, UIntPtr out_size) => GooglePlayGames.Native.Cwrapper.Achievement.Achievement_Id(SelfPtr(), out_string, out_size));
		}

		internal string Name()
		{
			return PInvokeUtilities.OutParamsToString((byte[] out_string, UIntPtr out_size) => GooglePlayGames.Native.Cwrapper.Achievement.Achievement_Name(SelfPtr(), out_string, out_size));
		}

		internal Types.AchievementState State()
		{
			return GooglePlayGames.Native.Cwrapper.Achievement.Achievement_State(SelfPtr());
		}

		internal uint TotalSteps()
		{
			return GooglePlayGames.Native.Cwrapper.Achievement.Achievement_TotalSteps(SelfPtr());
		}

		internal Types.AchievementType Type()
		{
			return GooglePlayGames.Native.Cwrapper.Achievement.Achievement_Type(SelfPtr());
		}

		internal ulong LastModifiedTime()
		{
			if (GooglePlayGames.Native.Cwrapper.Achievement.Achievement_Valid(SelfPtr()))
			{
				return GooglePlayGames.Native.Cwrapper.Achievement.Achievement_LastModifiedTime(SelfPtr());
			}
			return 0uL;
		}

		internal ulong getXP()
		{
			return GooglePlayGames.Native.Cwrapper.Achievement.Achievement_XP(SelfPtr());
		}

		internal string getRevealedImageUrl()
		{
			return PInvokeUtilities.OutParamsToString((byte[] out_string, UIntPtr out_size) => GooglePlayGames.Native.Cwrapper.Achievement.Achievement_RevealedIconUrl(SelfPtr(), out_string, out_size));
		}

		internal string getUnlockedImageUrl()
		{
			return PInvokeUtilities.OutParamsToString((byte[] out_string, UIntPtr out_size) => GooglePlayGames.Native.Cwrapper.Achievement.Achievement_UnlockedIconUrl(SelfPtr(), out_string, out_size));
		}

		protected override void CallDispose(HandleRef selfPointer)
		{
			GooglePlayGames.Native.Cwrapper.Achievement.Achievement_Dispose(selfPointer);
		}

		internal GooglePlayGames.BasicApi.Achievement AsAchievement()
		{
			GooglePlayGames.BasicApi.Achievement achievement = new GooglePlayGames.BasicApi.Achievement();
			achievement.Id = Id();
			achievement.Name = Name();
			achievement.Description = Description();
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			ulong num = LastModifiedTime();
			if (num == ulong.MaxValue)
			{
				num = 0uL;
			}
			achievement.LastModifiedTime = dateTime.AddMilliseconds(num);
			achievement.Points = getXP();
			achievement.RevealedImageUrl = getRevealedImageUrl();
			achievement.UnlockedImageUrl = getUnlockedImageUrl();
			if (Type() == Types.AchievementType.INCREMENTAL)
			{
				achievement.IsIncremental = true;
				achievement.CurrentSteps = (int)CurrentSteps();
				achievement.TotalSteps = (int)TotalSteps();
			}
			achievement.IsRevealed = State() == Types.AchievementState.REVEALED || State() == Types.AchievementState.UNLOCKED;
			achievement.IsUnlocked = State() == Types.AchievementState.UNLOCKED;
			return achievement;
		}
	}
}
