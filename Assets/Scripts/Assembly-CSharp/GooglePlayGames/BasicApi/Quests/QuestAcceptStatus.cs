namespace GooglePlayGames.BasicApi.Quests
{
	public enum QuestAcceptStatus
	{
		Success = 0,
		BadInput = 1,
		InternalError = 2,
		NotAuthorized = 3,
		Timeout = 4,
		QuestNoLongerAvailable = 5,
		QuestNotStarted = 6
	}
}
