using System;
using System.Runtime.InteropServices;
using AOT;
using GooglePlayGames.Native.Cwrapper;
using GooglePlayGames.OurUtils;

namespace GooglePlayGames.Native.PInvoke
{
	internal class StatsManager
	{
		internal class FetchForPlayerResponse : BaseReferenceHolder
		{
			internal FetchForPlayerResponse(IntPtr selfPointer)
				: base(selfPointer)
			{
			}

			internal CommonErrorStatus.ResponseStatus Status()
			{
				return GooglePlayGames.Native.Cwrapper.StatsManager.StatsManager_FetchForPlayerResponse_GetStatus(SelfPtr());
			}

			internal NativePlayerStats PlayerStats()
			{
				IntPtr selfPointer = GooglePlayGames.Native.Cwrapper.StatsManager.StatsManager_FetchForPlayerResponse_GetData(SelfPtr());
				return new NativePlayerStats(selfPointer);
			}

			protected override void CallDispose(HandleRef selfPointer)
			{
				GooglePlayGames.Native.Cwrapper.StatsManager.StatsManager_FetchForPlayerResponse_Dispose(selfPointer);
			}

			internal static FetchForPlayerResponse FromPointer(IntPtr pointer)
			{
				if (pointer.Equals(IntPtr.Zero))
				{
					return null;
				}
				return new FetchForPlayerResponse(pointer);
			}
		}

		private readonly GameServices mServices;

		internal StatsManager(GameServices services)
		{
			mServices = Misc.CheckNotNull(services);
		}

		internal void FetchForPlayer(Action<FetchForPlayerResponse> callback)
		{
			Misc.CheckNotNull(callback);
			GooglePlayGames.Native.Cwrapper.StatsManager.StatsManager_FetchForPlayer(mServices.AsHandle(), Types.DataSource.CACHE_OR_NETWORK, InternalFetchForPlayerCallback, Callbacks.ToIntPtr(callback, FetchForPlayerResponse.FromPointer));
		}

		[AOT.MonoPInvokeCallback(typeof(GooglePlayGames.Native.Cwrapper.StatsManager.FetchForPlayerCallback))]
		private static void InternalFetchForPlayerCallback(IntPtr response, IntPtr data)
		{
			Callbacks.PerformInternalCallback("StatsManager#InternalFetchForPlayerCallback", Callbacks.Type.Temporary, response, data);
		}
	}
}
