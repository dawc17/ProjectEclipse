using System;

namespace GooglePlayGames.BasicApi.Quests
{
	[Obsolete("Quests are being removed in 2018.")]
	public interface IQuestMilestone
	{
		string Id { get; }

		string EventId { get; }

		string QuestId { get; }

		ulong CurrentCount { get; }

		ulong TargetCount { get; }

		byte[] CompletionRewardData { get; }

		MilestoneState State { get; }
	}
}
