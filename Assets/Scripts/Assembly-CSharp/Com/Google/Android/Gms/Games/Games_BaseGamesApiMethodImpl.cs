using System;
using Com.Google.Android.Gms.Common.Api;
using Google.Developers;

namespace Com.Google.Android.Gms.Games
{
	public class Games_BaseGamesApiMethodImpl<R> : JavaObjWrapper where R : Result
	{
		private const string CLASS_NAME = "com/google/android/gms/games/Games$BaseGamesApiMethodImpl";

		public Games_BaseGamesApiMethodImpl(IntPtr ptr)
			: base(ptr)
		{
		}

		public Games_BaseGamesApiMethodImpl(GoogleApiClient arg_GoogleApiClient_1)
		{
			CreateInstance("com/google/android/gms/games/Games$BaseGamesApiMethodImpl", arg_GoogleApiClient_1);
		}
	}
}
