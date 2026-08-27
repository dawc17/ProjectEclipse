namespace GooglePlayGames.Native.Cwrapper
{
	internal static class Status
	{
		internal enum ResponseStatus
		{
			VALID = 1,
			VALID_BUT_STALE = 2,
			ERROR_LICENSE_CHECK_FAILED = -1,
			ERROR_INTERNAL = -2,
			ERROR_NOT_AUTHORIZED = -3,
			ERROR_VERSION_UPDATE_REQUIRED = -4,
			ERROR_TIMEOUT = -5
		}

		internal enum FlushStatus
		{
			FLUSHED = 4,
			ERROR_INTERNAL = -2,
			ERROR_NOT_AUTHORIZED = -3,
			ERROR_VERSION_UPDATE_REQUIRED = -4,
			ERROR_TIMEOUT = -5
		}

		internal enum AuthStatus
		{
			VALID = 1,
			ERROR_INTERNAL = -2,
			ERROR_NOT_AUTHORIZED = -3,
			ERROR_VERSION_UPDATE_REQUIRED = -4,
			ERROR_TIMEOUT = -5
		}

		internal enum UIStatus
		{
			VALID = 1,
			ERROR_INTERNAL = -2,
			ERROR_NOT_AUTHORIZED = -3,
			ERROR_VERSION_UPDATE_REQUIRED = -4,
			ERROR_TIMEOUT = -5,
			ERROR_CANCELED = -6,
			ERROR_UI_BUSY = -12,
			ERROR_LEFT_ROOM = -18
		}

		internal enum MultiplayerStatus
		{
			VALID = 1,
			VALID_BUT_STALE = 2,
			ERROR_INTERNAL = -2,
			ERROR_NOT_AUTHORIZED = -3,
			ERROR_VERSION_UPDATE_REQUIRED = -4,
			ERROR_TIMEOUT = -5,
			ERROR_MATCH_ALREADY_REMATCHED = -7,
			ERROR_INACTIVE_MATCH = -8,
			ERROR_INVALID_RESULTS = -9,
			ERROR_INVALID_MATCH = -10,
			ERROR_MATCH_OUT_OF_DATE = -11,
			ERROR_REAL_TIME_ROOM_NOT_JOINED = -17
		}

		internal enum QuestAcceptStatus
		{
			VALID = 1,
			ERROR_INTERNAL = -2,
			ERROR_NOT_AUTHORIZED = -3,
			ERROR_TIMEOUT = -5,
			ERROR_QUEST_NO_LONGER_AVAILABLE = -13,
			ERROR_QUEST_NOT_STARTED = -14
		}

		internal enum QuestClaimMilestoneStatus
		{
			VALID = 1,
			ERROR_INTERNAL = -2,
			ERROR_NOT_AUTHORIZED = -3,
			ERROR_TIMEOUT = -5,
			ERROR_MILESTONE_ALREADY_CLAIMED = -15,
			ERROR_MILESTONE_CLAIM_FAILED = -16
		}

		internal enum CommonErrorStatus
		{
			ERROR_INTERNAL = -2,
			ERROR_NOT_AUTHORIZED = -3,
			ERROR_TIMEOUT = -5
		}

		internal enum SnapshotOpenStatus
		{
			VALID = 1,
			VALID_WITH_CONFLICT = 3,
			ERROR_INTERNAL = -2,
			ERROR_NOT_AUTHORIZED = -3,
			ERROR_TIMEOUT = -5
		}
	}
}
