using System;
using System.Runtime.InteropServices;

namespace GooglePlayGames.Native.Cwrapper
{
	internal static class LeaderboardManager
	{
		internal delegate void FetchCallback(IntPtr arg0, IntPtr arg1);

		internal delegate void FetchAllCallback(IntPtr arg0, IntPtr arg1);

		internal delegate void FetchScorePageCallback(IntPtr arg0, IntPtr arg1);

		internal delegate void FetchScoreSummaryCallback(IntPtr arg0, IntPtr arg1);

		internal delegate void FetchAllScoreSummariesCallback(IntPtr arg0, IntPtr arg1);

		internal delegate void ShowAllUICallback(CommonErrorStatus.UIStatus arg0, IntPtr arg1);

		internal delegate void ShowUICallback(CommonErrorStatus.UIStatus arg0, IntPtr arg1);

		[DllImport("gpg")]
		internal static extern void LeaderboardManager_FetchAll(HandleRef self, Types.DataSource data_source, FetchAllCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void LeaderboardManager_FetchScoreSummary(HandleRef self, Types.DataSource data_source, string leaderboard_id, Types.LeaderboardTimeSpan time_span, Types.LeaderboardCollection collection, FetchScoreSummaryCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern IntPtr LeaderboardManager_ScorePageToken(HandleRef self, string leaderboard_id, Types.LeaderboardStart start, Types.LeaderboardTimeSpan time_span, Types.LeaderboardCollection collection);

		[DllImport("gpg")]
		internal static extern void LeaderboardManager_ShowAllUI(HandleRef self, ShowAllUICallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void LeaderboardManager_FetchScorePage(HandleRef self, Types.DataSource data_source, IntPtr token, uint max_results, FetchScorePageCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void LeaderboardManager_FetchAllScoreSummaries(HandleRef self, Types.DataSource data_source, string leaderboard_id, FetchAllScoreSummariesCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void LeaderboardManager_ShowUI(HandleRef self, string leaderboard_id, Types.LeaderboardTimeSpan time_span, ShowUICallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void LeaderboardManager_Fetch(HandleRef self, Types.DataSource data_source, string leaderboard_id, FetchCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void LeaderboardManager_SubmitScore(HandleRef self, string leaderboard_id, ulong score, string metadata);

		[DllImport("gpg")]
		internal static extern void LeaderboardManager_FetchResponse_Dispose(HandleRef self);

		[DllImport("gpg")]
		internal static extern CommonErrorStatus.ResponseStatus LeaderboardManager_FetchResponse_GetStatus(HandleRef self);

		[DllImport("gpg")]
		internal static extern IntPtr LeaderboardManager_FetchResponse_GetData(HandleRef self);

		[DllImport("gpg")]
		internal static extern void LeaderboardManager_FetchAllResponse_Dispose(HandleRef self);

		[DllImport("gpg")]
		internal static extern CommonErrorStatus.ResponseStatus LeaderboardManager_FetchAllResponse_GetStatus(HandleRef self);

		[DllImport("gpg")]
		internal static extern UIntPtr LeaderboardManager_FetchAllResponse_GetData_Length(HandleRef self);

		[DllImport("gpg")]
		internal static extern IntPtr LeaderboardManager_FetchAllResponse_GetData_GetElement(HandleRef self, UIntPtr index);

		[DllImport("gpg")]
		internal static extern void LeaderboardManager_FetchScorePageResponse_Dispose(HandleRef self);

		[DllImport("gpg")]
		internal static extern CommonErrorStatus.ResponseStatus LeaderboardManager_FetchScorePageResponse_GetStatus(HandleRef self);

		[DllImport("gpg")]
		internal static extern IntPtr LeaderboardManager_FetchScorePageResponse_GetData(HandleRef self);

		[DllImport("gpg")]
		internal static extern void LeaderboardManager_FetchScoreSummaryResponse_Dispose(HandleRef self);

		[DllImport("gpg")]
		internal static extern CommonErrorStatus.ResponseStatus LeaderboardManager_FetchScoreSummaryResponse_GetStatus(HandleRef self);

		[DllImport("gpg")]
		internal static extern IntPtr LeaderboardManager_FetchScoreSummaryResponse_GetData(HandleRef self);

		[DllImport("gpg")]
		internal static extern void LeaderboardManager_FetchAllScoreSummariesResponse_Dispose(HandleRef self);

		[DllImport("gpg")]
		internal static extern CommonErrorStatus.ResponseStatus LeaderboardManager_FetchAllScoreSummariesResponse_GetStatus(HandleRef self);

		[DllImport("gpg")]
		internal static extern UIntPtr LeaderboardManager_FetchAllScoreSummariesResponse_GetData_Length(HandleRef self);

		[DllImport("gpg")]
		internal static extern IntPtr LeaderboardManager_FetchAllScoreSummariesResponse_GetData_GetElement(HandleRef self, UIntPtr index);
	}
}
