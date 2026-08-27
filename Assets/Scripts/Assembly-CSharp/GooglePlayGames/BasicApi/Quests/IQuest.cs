using System;

namespace GooglePlayGames.BasicApi.Quests
{
	public interface IQuest
	{
		string Id { get; }

		string Name { get; }

		string Description { get; }

		string BannerUrl { get; }

		string IconUrl { get; }

		DateTime StartTime { get; }

		DateTime ExpirationTime { get; }

		DateTime? AcceptedTime { get; }

		IQuestMilestone Milestone { get; }

		QuestState State { get; }
	}
}
