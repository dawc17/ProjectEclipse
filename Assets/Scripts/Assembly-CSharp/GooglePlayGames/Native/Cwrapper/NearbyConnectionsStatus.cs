namespace GooglePlayGames.Native.Cwrapper
{
	internal static class NearbyConnectionsStatus
	{
		internal enum InitializationStatus
		{
			VALID = 1,
			ERROR_INTERNAL = -2,
			ERROR_VERSION_UPDATE_REQUIRED = -4
		}
	}
}
