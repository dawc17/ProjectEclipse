using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Multiplayer;
using GooglePlayGames.Native.Cwrapper;
using GooglePlayGames.OurUtils;

namespace GooglePlayGames.Native.PInvoke
{
	internal class PlayerManager
	{
		internal class FetchListResponse : BaseReferenceHolder, IEnumerable<NativePlayer>, IEnumerable
		{
			internal FetchListResponse(IntPtr selfPointer)
				: base(selfPointer)
			{
			}

			protected override void CallDispose(HandleRef selfPointer)
			{
				GooglePlayGames.Native.Cwrapper.PlayerManager.PlayerManager_FetchListResponse_Dispose(SelfPtr());
			}

			internal CommonErrorStatus.ResponseStatus Status()
			{
				return GooglePlayGames.Native.Cwrapper.PlayerManager.PlayerManager_FetchListResponse_GetStatus(SelfPtr());
			}

			public IEnumerator<NativePlayer> GetEnumerator()
			{
				return PInvokeUtilities.ToEnumerator(Length(), (UIntPtr index) => GetElement(index));
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			internal UIntPtr Length()
			{
				return GooglePlayGames.Native.Cwrapper.PlayerManager.PlayerManager_FetchListResponse_GetData_Length(SelfPtr());
			}

			internal NativePlayer GetElement(UIntPtr index)
			{
				if (index.ToUInt64() >= Length().ToUInt64())
				{
					throw new ArgumentOutOfRangeException();
				}
				return new NativePlayer(GooglePlayGames.Native.Cwrapper.PlayerManager.PlayerManager_FetchListResponse_GetData_GetElement(SelfPtr(), index));
			}

			internal static FetchListResponse FromPointer(IntPtr selfPointer)
			{
				if (PInvokeUtilities.IsNull(selfPointer))
				{
					return null;
				}
				return new FetchListResponse(selfPointer);
			}
		}

		internal class FetchResponseCollector
		{
			internal int pendingCount;

			internal List<NativePlayer> results = new List<NativePlayer>();

			internal Action<NativePlayer[]> callback;
		}

		internal class FetchResponse : BaseReferenceHolder
		{
			internal FetchResponse(IntPtr selfPointer)
				: base(selfPointer)
			{
			}

			protected override void CallDispose(HandleRef selfPointer)
			{
				GooglePlayGames.Native.Cwrapper.PlayerManager.PlayerManager_FetchResponse_Dispose(SelfPtr());
			}

			internal NativePlayer GetPlayer()
			{
				return new NativePlayer(GooglePlayGames.Native.Cwrapper.PlayerManager.PlayerManager_FetchResponse_GetData(SelfPtr()));
			}

			internal CommonErrorStatus.ResponseStatus Status()
			{
				return GooglePlayGames.Native.Cwrapper.PlayerManager.PlayerManager_FetchResponse_GetStatus(SelfPtr());
			}

			internal static FetchResponse FromPointer(IntPtr selfPointer)
			{
				if (PInvokeUtilities.IsNull(selfPointer))
				{
					return null;
				}
				return new FetchResponse(selfPointer);
			}
		}

		internal class FetchSelfResponse : BaseReferenceHolder
		{
			internal FetchSelfResponse(IntPtr selfPointer)
				: base(selfPointer)
			{
			}

			internal CommonErrorStatus.ResponseStatus Status()
			{
				return GooglePlayGames.Native.Cwrapper.PlayerManager.PlayerManager_FetchSelfResponse_GetStatus(SelfPtr());
			}

			internal NativePlayer Self()
			{
				return new NativePlayer(GooglePlayGames.Native.Cwrapper.PlayerManager.PlayerManager_FetchSelfResponse_GetData(SelfPtr()));
			}

			protected override void CallDispose(HandleRef selfPointer)
			{
				GooglePlayGames.Native.Cwrapper.PlayerManager.PlayerManager_FetchSelfResponse_Dispose(SelfPtr());
			}

			internal static FetchSelfResponse FromPointer(IntPtr selfPointer)
			{
				if (PInvokeUtilities.IsNull(selfPointer))
				{
					return null;
				}
				return new FetchSelfResponse(selfPointer);
			}
		}

		private readonly GameServices mGameServices;

		internal PlayerManager(GameServices services)
		{
			mGameServices = Misc.CheckNotNull(services);
		}

		internal void FetchSelf(Action<FetchSelfResponse> callback)
		{
			GooglePlayGames.Native.Cwrapper.PlayerManager.PlayerManager_FetchSelf(mGameServices.AsHandle(), Types.DataSource.CACHE_OR_NETWORK, InternalFetchSelfCallback, Callbacks.ToIntPtr(callback, FetchSelfResponse.FromPointer));
		}

		[AOT.MonoPInvokeCallback(typeof(GooglePlayGames.Native.Cwrapper.PlayerManager.FetchSelfCallback))]
		private static void InternalFetchSelfCallback(IntPtr response, IntPtr data)
		{
			Callbacks.PerformInternalCallback("PlayerManager#InternalFetchSelfCallback", Callbacks.Type.Temporary, response, data);
		}

		internal void FetchList(string[] userIds, Action<NativePlayer[]> callback)
		{
			FetchResponseCollector coll = new FetchResponseCollector();
			coll.pendingCount = userIds.Length;
			coll.callback = callback;
			foreach (string player_id in userIds)
			{
				GooglePlayGames.Native.Cwrapper.PlayerManager.PlayerManager_Fetch(mGameServices.AsHandle(), Types.DataSource.CACHE_OR_NETWORK, player_id, InternalFetchCallback, Callbacks.ToIntPtr((FetchResponse rsp) =>
				{
					HandleFetchResponse(coll, rsp);
				}, FetchResponse.FromPointer));
			}
		}

		[AOT.MonoPInvokeCallback(typeof(GooglePlayGames.Native.Cwrapper.PlayerManager.FetchCallback))]
		private static void InternalFetchCallback(IntPtr response, IntPtr data)
		{
			Callbacks.PerformInternalCallback("PlayerManager#InternalFetchCallback", Callbacks.Type.Temporary, response, data);
		}

		internal void HandleFetchResponse(FetchResponseCollector collector, FetchResponse resp)
		{
			if (resp.Status() == CommonErrorStatus.ResponseStatus.VALID || resp.Status() == CommonErrorStatus.ResponseStatus.VALID_BUT_STALE)
			{
				NativePlayer player = resp.GetPlayer();
				collector.results.Add(player);
			}
			collector.pendingCount--;
			if (collector.pendingCount == 0)
			{
				collector.callback(collector.results.ToArray());
			}
		}

		internal void FetchFriends(Action<ResponseStatus, List<GooglePlayGames.BasicApi.Multiplayer.Player>> callback)
		{
			GooglePlayGames.Native.Cwrapper.PlayerManager.PlayerManager_FetchConnected(mGameServices.AsHandle(), Types.DataSource.CACHE_OR_NETWORK, InternalFetchConnectedCallback, Callbacks.ToIntPtr((FetchListResponse rsp) =>
			{
				HandleFetchCollected(rsp, callback);
			}, FetchListResponse.FromPointer));
		}

		[AOT.MonoPInvokeCallback(typeof(GooglePlayGames.Native.Cwrapper.PlayerManager.FetchListCallback))]
		private static void InternalFetchConnectedCallback(IntPtr response, IntPtr data)
		{
			Callbacks.PerformInternalCallback("PlayerManager#InternalFetchConnectedCallback", Callbacks.Type.Temporary, response, data);
		}

		internal void HandleFetchCollected(FetchListResponse rsp, Action<ResponseStatus, List<GooglePlayGames.BasicApi.Multiplayer.Player>> callback)
		{
			List<GooglePlayGames.BasicApi.Multiplayer.Player> list = new List<GooglePlayGames.BasicApi.Multiplayer.Player>();
			if (rsp.Status() == CommonErrorStatus.ResponseStatus.VALID || rsp.Status() == CommonErrorStatus.ResponseStatus.VALID_BUT_STALE)
			{
				Logger.d("Got " + rsp.Length().ToUInt64() + " players");
				foreach (NativePlayer item in rsp)
				{
					list.Add(item.AsPlayer());
				}
			}
			callback((ResponseStatus)rsp.Status(), list);
		}
	}
}
