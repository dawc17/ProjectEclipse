using System;

namespace GooglePlayGames
{
	internal interface TokenClient
	{
		string GetEmail();

		string GetAuthCode();

		string GetIdToken();

		void Signout();

		void SetRequestAuthCode(bool flag, bool forceRefresh);

		void SetRequestEmail(bool flag);

		void SetRequestIdToken(bool flag);

		void SetWebClientId(string webClientId);

		void SetAccountName(string accountName);

		void AddOauthScopes(string[] scopes);

		void SetHidePopups(bool flag);

		bool NeedsToRun();

		void FetchTokens(Action<int> callback);
	}
}
