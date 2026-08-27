namespace GooglePlayGames.BasicApi.Quests
{
	public enum QuestClaimMilestoneStatus
	{
		Success = 0,
		BadInput = 1,
		InternalError = 2,
		NotAuthorized = 3,
		Timeout = 4,
		MilestoneAlreadyClaimed = 5,
		MilestoneClaimFailed = 6
	}
}
