namespace GooglePlayGames
{
	public static class GameInfo
	{
		private const string UnescapedApplicationId = "APP_ID";

		private const string UnescapedIosClientId = "IOS_CLIENTID";

		private const string UnescapedWebClientId = "WEB_CLIENTID";

		private const string UnescapedNearbyServiceId = "NEARBY_SERVICE_ID";

		public const string ApplicationId = "__APP_ID__";

		public const string IosClientId = "__IOS_CLIENTID__";

		public const string WebClientId = "__WEB_CLIENTID__";

		public const string NearbyConnectionServiceId = "__NEARBY_SERVICE_ID__";

		public static bool ApplicationIdInitialized()
		{
			return !string.IsNullOrEmpty("__APP_ID__") && !"__APP_ID__".Equals(ToEscapedToken("APP_ID"));
		}

		public static bool IosClientIdInitialized()
		{
			return !string.IsNullOrEmpty("__IOS_CLIENTID__") && !"__IOS_CLIENTID__".Equals(ToEscapedToken("IOS_CLIENTID"));
		}

		public static bool WebClientIdInitialized()
		{
			return !string.IsNullOrEmpty("__WEB_CLIENTID__") && !"__WEB_CLIENTID__".Equals(ToEscapedToken("WEB_CLIENTID"));
		}

		public static bool NearbyConnectionsInitialized()
		{
			return !string.IsNullOrEmpty("__NEARBY_SERVICE_ID__") && !"__NEARBY_SERVICE_ID__".Equals(ToEscapedToken("NEARBY_SERVICE_ID"));
		}

		private static string ToEscapedToken(string token)
		{
			return string.Format("__{0}__", token);
		}
	}
}
