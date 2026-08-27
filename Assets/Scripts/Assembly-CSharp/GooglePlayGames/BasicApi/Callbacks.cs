using System;

namespace GooglePlayGames.BasicApi
{
	internal static class Callbacks
	{
		internal static T AsOnGameThreadCallback<T>(T callback)
		{
			return callback;
		}

		internal static Action AsOnGameThreadCallback(Action callback)
		{
			return callback;
		}

		internal static Action<T> AsOnGameThreadCallback<T>(Action<T> callback)
		{
			return callback;
		}

		internal static Action<T1, T2> AsOnGameThreadCallback<T1, T2>(Action<T1, T2> callback)
		{
			return callback;
		}

		internal static T AsCoroutine<T>(T callback)
		{
			return callback;
		}
	}
}
