namespace GooglePlayGames.BasicApi.Quests
{
	public enum QuestUiResult
	{
		UserRequestsQuestAcceptance = 0,
		UserRequestsMilestoneClaiming = 1,
		BadInput = 2,
		InternalError = 3,
		UserCanceled = 4,
		NotAuthorized = 5,
		VersionUpdateRequired = 6,
		Timeout = 7,
		UiBusy = 8
	}
}
