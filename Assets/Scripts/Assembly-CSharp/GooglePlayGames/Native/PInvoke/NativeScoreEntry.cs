using System;
using System.Runtime.InteropServices;
using GooglePlayGames.Native.Cwrapper;

namespace GooglePlayGames.Native.PInvoke
{
	internal class NativeScoreEntry : BaseReferenceHolder
	{
		private const ulong MinusOne = ulong.MaxValue;

		internal NativeScoreEntry(IntPtr selfPtr)
			: base(selfPtr)
		{
		}

		protected override void CallDispose(HandleRef selfPointer)
		{
			ScorePage.ScorePage_Entry_Dispose(selfPointer);
		}

		internal ulong GetLastModifiedTime()
		{
			return ScorePage.ScorePage_Entry_LastModifiedTime(SelfPtr());
		}

		internal string GetPlayerId()
		{
			return PInvokeUtilities.OutParamsToString((byte[] out_string, UIntPtr out_size) => ScorePage.ScorePage_Entry_PlayerId(SelfPtr(), out_string, out_size));
		}

		internal NativeScore GetScore()
		{
			return new NativeScore(ScorePage.ScorePage_Entry_Score(SelfPtr()));
		}

		internal PlayGamesScore AsScore(string leaderboardId)
		{
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			ulong num = GetLastModifiedTime();
			if (num == ulong.MaxValue)
			{
				num = 0uL;
			}
			DateTime date = dateTime.AddMilliseconds(num);
			return new PlayGamesScore(date, leaderboardId, GetScore().GetRank(), GetPlayerId(), GetScore().GetValue(), GetScore().GetMetadata());
		}
	}
}
