using System;
using AOT;
using GooglePlayGames.BasicApi;
using GooglePlayGames.Native.Cwrapper;
using GooglePlayGames.OurUtils;

namespace GooglePlayGames.Native.PInvoke
{
	internal class LeaderboardManager
	{
		private readonly GameServices mServices;

		internal int LeaderboardMaxResults
		{
			get
			{
				return 25;
			}
		}

		internal LeaderboardManager(GameServices services)
		{
			mServices = Misc.CheckNotNull(services);
		}

		internal void SubmitScore(string leaderboardId, long score, string metadata)
		{
			Misc.CheckNotNull(leaderboardId, "leaderboardId");
			Logger.d("Native Submitting score: " + score + " for lb " + leaderboardId + " with metadata: " + metadata);
			GooglePlayGames.Native.Cwrapper.LeaderboardManager.LeaderboardManager_SubmitScore(mServices.AsHandle(), leaderboardId, (ulong)score, metadata ?? string.Empty);
		}

		internal void ShowAllUI(Action<CommonErrorStatus.UIStatus> callback)
		{
			Misc.CheckNotNull(callback);
			GooglePlayGames.Native.Cwrapper.LeaderboardManager.LeaderboardManager_ShowAllUI(mServices.AsHandle(), Callbacks.InternalShowUICallback, Callbacks.ToIntPtr(callback));
		}

		internal void ShowUI(string leaderboardId, LeaderboardTimeSpan span, Action<CommonErrorStatus.UIStatus> callback)
		{
			Misc.CheckNotNull(callback);
			GooglePlayGames.Native.Cwrapper.LeaderboardManager.LeaderboardManager_ShowUI(mServices.AsHandle(), leaderboardId, (Types.LeaderboardTimeSpan)span, Callbacks.InternalShowUICallback, Callbacks.ToIntPtr(callback));
		}

		public void LoadLeaderboardData(string leaderboardId, LeaderboardStart start, int rowCount, LeaderboardCollection collection, LeaderboardTimeSpan timeSpan, string playerId, Action<LeaderboardScoreData> callback)
		{
			NativeScorePageToken internalObject = new NativeScorePageToken(GooglePlayGames.Native.Cwrapper.LeaderboardManager.LeaderboardManager_ScorePageToken(mServices.AsHandle(), leaderboardId, (Types.LeaderboardStart)start, (Types.LeaderboardTimeSpan)timeSpan, (Types.LeaderboardCollection)collection));
			ScorePageToken token = new ScorePageToken(internalObject, leaderboardId, collection, timeSpan);
			GooglePlayGames.Native.Cwrapper.LeaderboardManager.LeaderboardManager_Fetch(mServices.AsHandle(), Types.DataSource.CACHE_OR_NETWORK, leaderboardId, InternalFetchCallback, Callbacks.ToIntPtr((FetchResponse rsp) =>
			{
				HandleFetch(token, rsp, playerId, rowCount, callback);
			}, FetchResponse.FromPointer));
		}

		[AOT.MonoPInvokeCallback(typeof(GooglePlayGames.Native.Cwrapper.LeaderboardManager.FetchCallback))]
		private static void InternalFetchCallback(IntPtr response, IntPtr data)
		{
			Callbacks.PerformInternalCallback("LeaderboardManager#InternalFetchCallback", Callbacks.Type.Temporary, response, data);
		}

		internal void HandleFetch(ScorePageToken token, FetchResponse response, string selfPlayerId, int maxResults, Action<LeaderboardScoreData> callback)
		{
			LeaderboardScoreData data = new LeaderboardScoreData(token.LeaderboardId, (ResponseStatus)response.GetStatus());
			if (response.GetStatus() != CommonErrorStatus.ResponseStatus.VALID && response.GetStatus() != CommonErrorStatus.ResponseStatus.VALID_BUT_STALE)
			{
				Logger.w("Error returned from fetch: " + response.GetStatus());
				callback(data);
				return;
			}
			data.Title = response.Leaderboard().Title();
			data.Id = token.LeaderboardId;
			GooglePlayGames.Native.Cwrapper.LeaderboardManager.LeaderboardManager_FetchScoreSummary(mServices.AsHandle(), Types.DataSource.CACHE_OR_NETWORK, token.LeaderboardId, (Types.LeaderboardTimeSpan)token.TimeSpan, (Types.LeaderboardCollection)token.Collection, InternalFetchSummaryCallback, Callbacks.ToIntPtr((FetchScoreSummaryResponse rsp) =>
			{
				HandleFetchScoreSummary(data, rsp, selfPlayerId, maxResults, token, callback);
			}, FetchScoreSummaryResponse.FromPointer));
		}

		[AOT.MonoPInvokeCallback(typeof(GooglePlayGames.Native.Cwrapper.LeaderboardManager.FetchScoreSummaryCallback))]
		private static void InternalFetchSummaryCallback(IntPtr response, IntPtr data)
		{
			Callbacks.PerformInternalCallback("LeaderboardManager#InternalFetchSummaryCallback", Callbacks.Type.Temporary, response, data);
		}

		internal void HandleFetchScoreSummary(LeaderboardScoreData data, FetchScoreSummaryResponse response, string selfPlayerId, int maxResults, ScorePageToken token, Action<LeaderboardScoreData> callback)
		{
			if (response.GetStatus() != CommonErrorStatus.ResponseStatus.VALID && response.GetStatus() != CommonErrorStatus.ResponseStatus.VALID_BUT_STALE)
			{
				Logger.w("Error returned from fetchScoreSummary: " + response);
				data.Status = (ResponseStatus)response.GetStatus();
				callback(data);
				return;
			}
			NativeScoreSummary scoreSummary = response.GetScoreSummary();
			data.ApproximateCount = scoreSummary.ApproximateResults();
			data.PlayerScore = scoreSummary.LocalUserScore().AsScore(data.Id, selfPlayerId);
			if (maxResults <= 0)
			{
				callback(data);
			}
			else
			{
				LoadScorePage(data, maxResults, token, callback);
			}
		}

		public void LoadScorePage(LeaderboardScoreData data, int maxResults, ScorePageToken token, Action<LeaderboardScoreData> callback)
		{
			if (data == null)
			{
				data = new LeaderboardScoreData(token.LeaderboardId);
			}
			NativeScorePageToken nativeScorePageToken = (NativeScorePageToken)token.InternalObject;
			GooglePlayGames.Native.Cwrapper.LeaderboardManager.LeaderboardManager_FetchScorePage(mServices.AsHandle(), Types.DataSource.CACHE_OR_NETWORK, nativeScorePageToken.AsPointer(), (uint)maxResults, InternalFetchScorePage, Callbacks.ToIntPtr((FetchScorePageResponse rsp) =>
			{
				HandleFetchScorePage(data, token, rsp, callback);
			}, FetchScorePageResponse.FromPointer));
		}

		[AOT.MonoPInvokeCallback(typeof(GooglePlayGames.Native.Cwrapper.LeaderboardManager.FetchScorePageCallback))]
		private static void InternalFetchScorePage(IntPtr response, IntPtr data)
		{
			Callbacks.PerformInternalCallback("LeaderboardManager#InternalFetchScorePage", Callbacks.Type.Temporary, response, data);
		}

		internal void HandleFetchScorePage(LeaderboardScoreData data, ScorePageToken token, FetchScorePageResponse rsp, Action<LeaderboardScoreData> callback)
		{
			data.Status = (ResponseStatus)rsp.GetStatus();
			if (rsp.GetStatus() != CommonErrorStatus.ResponseStatus.VALID && rsp.GetStatus() != CommonErrorStatus.ResponseStatus.VALID_BUT_STALE)
			{
				callback(data);
			}
			NativeScorePage scorePage = rsp.GetScorePage();
			if (!scorePage.Valid())
			{
				callback(data);
			}
			if (scorePage.HasNextScorePage())
			{
				data.NextPageToken = new ScorePageToken(scorePage.GetNextScorePageToken(), token.LeaderboardId, token.Collection, token.TimeSpan);
			}
			if (scorePage.HasPrevScorePage())
			{
				data.PrevPageToken = new ScorePageToken(scorePage.GetPreviousScorePageToken(), token.LeaderboardId, token.Collection, token.TimeSpan);
			}
			foreach (NativeScoreEntry item in scorePage)
			{
				data.AddScore(item.AsScore(data.Id));
			}
			callback(data);
		}
	}
}
