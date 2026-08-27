using System;
using GooglePlayGames.OurUtils;

namespace GooglePlayGames.Native
{
	internal static class CallbackUtils
	{
		internal static Action<T> ToOnGameThread<T>(Action<T> toConvert)
		{
			if (toConvert == null)
			{
				return delegate
				{
				};
			}
			return (T val) =>
			{
				PlayGamesHelperObject.RunOnGameThread(() =>
				{
					toConvert(val);
				});
			};
		}

		internal static Action<T1, T2> ToOnGameThread<T1, T2>(Action<T1, T2> toConvert)
		{
			if (toConvert == null)
			{
				return delegate
				{
				};
			}
			return (T1 val1, T2 val2) =>
			{
				PlayGamesHelperObject.RunOnGameThread(() =>
				{
					toConvert(val1, val2);
				});
			};
		}

		internal static Action<T1, T2, T3> ToOnGameThread<T1, T2, T3>(Action<T1, T2, T3> toConvert)
		{
			if (toConvert == null)
			{
				return delegate
				{
				};
			}
			return (T1 val1, T2 val2, T3 val3) =>
			{
				PlayGamesHelperObject.RunOnGameThread(() =>
				{
					toConvert(val1, val2, val3);
				});
			};
		}
	}
}
